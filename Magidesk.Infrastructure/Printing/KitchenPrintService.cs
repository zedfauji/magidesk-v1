using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;

namespace Magidesk.Infrastructure.Printing;

public class KitchenPrintService : IKitchenPrintService
{
    private readonly IRawPrintService _rawPrintService;
    private readonly IPrinterMappingRepository _printerMappingRepository;
    private readonly IPrinterGroupRepository _printerGroupRepository;
    private readonly ITerminalContext _terminalContext;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<KitchenPrintService> _logger;

    public KitchenPrintService(
        IRawPrintService rawPrintService,
        IPrinterMappingRepository printerMappingRepository,
        IPrinterGroupRepository printerGroupRepository,
        ITerminalContext terminalContext,
        IUserRepository userRepository,
        ILogger<KitchenPrintService> logger)
    {
        _rawPrintService = rawPrintService;
        _printerMappingRepository = printerMappingRepository;
        _printerGroupRepository = printerGroupRepository;
        _terminalContext = terminalContext;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<KitchenPrintResult> PrintOrderLineAsync(OrderLine orderLine, Ticket ticket, CancellationToken cancellationToken = default)
    {
        if (!orderLine.ShouldPrintToKitchen) return KitchenPrintResult.SuccessResult(0);
        
        return await PrintSpecificLinesAsync(ticket, new[] { orderLine }, cancellationToken);
    }

    public async Task<KitchenPrintResult> PrintTicketAsync(Ticket ticket, CancellationToken cancellationToken = default)
    {
        var linesToPrint = ticket.OrderLines.Where(x => x.ShouldPrintToKitchen && !x.PrintedToKitchen).ToList();
        if (!linesToPrint.Any()) return KitchenPrintResult.SuccessResult(0);

        return await PrintSpecificLinesAsync(ticket, linesToPrint, cancellationToken);
    }

    public Task MarkOrderLinePrintedAsync(OrderLine orderLine, CancellationToken cancellationToken = default)
    {
        orderLine.MarkPrintedToKitchen();
        return Task.CompletedTask;
    }
    
    private async Task<KitchenPrintResult> PrintSpecificLinesAsync(Ticket ticket, IEnumerable<OrderLine> lines, CancellationToken cancellationToken)
    {
        var errors = new List<string>();
        try
        {
            var terminalId = _terminalContext.TerminalId ?? Guid.Empty;
            if (terminalId == Guid.Empty)
            {
                 var msg = "Current Terminal ID is empty. Cannot resolve printer mappings.";
                 _logger.LogWarning(msg);
                 return KitchenPrintResult.Failure(msg);
            }

            // Resolve Server Name
            string serverName = "Unknown";
            try
            {
                var user = await _userRepository.GetByIdAsync(ticket.CreatedBy.Value, cancellationToken);
                if (user != null)
                {
                    serverName = $"{user.FirstName} {user.LastName}".Trim();
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to resolve server name.");
            }

            var mappings = (await _printerMappingRepository.GetByTerminalIdAsync(terminalId, cancellationToken)).ToList();
            var groupedLines = lines.GroupBy(x => x.PrinterGroupId);

            bool overallSuccess = true;
            int printedCount = 0;

            foreach (var group in groupedLines)
            {
                var printerGroupId = group.Key;
                var groupLines = group.ToList();

                // Find mapping
                var mapping = mappings.FirstOrDefault(m => m.PrinterGroupId == printerGroupId);
                
                if (mapping == null)
                {
                    var msg = $"No printer mapping found for PrinterGroup {printerGroupId} on Terminal {terminalId}. Lines skipped.";
                    _logger.LogWarning(msg);
                    errors.Add(msg);
                    overallSuccess = false; 
                    continue; 
                }

                if (string.IsNullOrEmpty(mapping.PhysicalPrinterName))
                {
                    var msg = $"PhysicalPrinterName is empty for Group {printerGroupId}.";
                    _logger.LogWarning(msg);
                    errors.Add(msg);
                    overallSuccess = false;
                    continue;
                }

                // Resolve Printer Group Behavior
                var printerGroup = await _printerGroupRepository.GetByIdAsync(printerGroupId ?? Guid.Empty, cancellationToken);
                if (printerGroup == null)
                {
                    _logger.LogWarning($"PrinterGroup {printerGroupId} not found in DB. Using defaults.");
                }

                // Determine Cut Behavior
                bool shouldCut = ShouldCut(printerGroup, mapping);

                // Generate ESC/POS
                var bytes = GenerateTicketBytes(ticket, groupLines, serverName, mapping.PrintableWidthChars, shouldCut);
                
                // Print with Retry Policy
                bool printed = await ExecutePrintWithRetryAsync(mapping.PhysicalPrinterName, bytes, printerGroup);
                if (printed)
                {
                    // Mark as printed
                    foreach (var line in groupLines)
                    {
                        await MarkOrderLinePrintedAsync(line, cancellationToken);
                        printedCount++;
                    }
                }
                else
                {
                    var msg = $"Failed to print to {mapping.PhysicalPrinterName} after retries.";
                    errors.Add(msg);
                    overallSuccess = false;
                }
            }

            if (!overallSuccess)
            {
                return KitchenPrintResult.Failure("One or more printers failed. Check logs.", errors);
            }

            return KitchenPrintResult.SuccessResult(printedCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in PrintSpecificLinesAsync");
            return KitchenPrintResult.Failure($"System Error: {ex.Message}", new List<string> { ex.Message });
        }
    }

    private bool ShouldCut(PrinterGroup? group, PrinterMapping mapping)
    {
        if (group == null) return mapping.CutEnabled; // Default to physical capability

        return group.CutBehavior switch
        {
            Domain.Enumerations.CutBehavior.Always => true,
            Domain.Enumerations.CutBehavior.Never => false,
            Domain.Enumerations.CutBehavior.Auto => mapping.CutEnabled,
            _ => mapping.CutEnabled
        };
    }

    private async Task<bool> ExecutePrintWithRetryAsync(string printerName, byte[] data, PrinterGroup? group)
    {
        int maxRetries = group?.RetryCount ?? 0;
        int delayMs = group?.RetryDelayMs ?? 500;
        if (delayMs < 100) delayMs = 100; // Sanity check

        // Initial attempt + retries
        int attempts = 0;
        
        while (attempts <= maxRetries)
        {
            try
            {
                await _rawPrintService.PrintRawBytesAsync(printerName, data);
                return true;
            }
            catch (Exception ex)
            {
                attempts++;
                _logger.LogWarning(ex, $"Print failed to {printerName}. Attempt {attempts}/{maxRetries + 1}.");
                
                if (attempts > maxRetries)
                {
                    _logger.LogError($"Exhausted all retries for printer {printerName}.");
                    return false;
                }

                await Task.Delay(delayMs);
            }
        }
        return false;
    }

    private byte[] GenerateTicketBytes(Ticket ticket, List<OrderLine> lines, string serverName, int widthChars, bool shouldCut)
    {
        var cmds = new List<byte[]>();
        
        // Safety for width
        if (widthChars <= 0) widthChars = 32;

        // Init
        cmds.Add(EscPosHelper.Initialize());
        
        // Header
        cmds.Add(EscPosHelper.AlignCenter());
        cmds.Add(EscPosHelper.DoubleHeightOn());
        cmds.Add(EscPosHelper.BoldOn());
        cmds.Add(EscPosHelper.GetBytes($"ORDER #{ticket.TicketNumber}"));
        cmds.Add(EscPosHelper.NewLine());
        cmds.Add(EscPosHelper.NormalSize());
        cmds.Add(EscPosHelper.BoldOff());
        
        // Table Info
        string tableName = ticket.TableNumbers.Any() ? string.Join(",", ticket.TableNumbers) : "No Table";
        cmds.Add(EscPosHelper.BoldOn());
        cmds.Add(EscPosHelper.GetBytes($"Table: {tableName}"));
        cmds.Add(EscPosHelper.BoldOff());
        cmds.Add(EscPosHelper.NewLine());

        cmds.Add(EscPosHelper.AlignLeft());
        cmds.Add(EscPosHelper.GetBytes($"Server: {serverName}"));
        cmds.Add(EscPosHelper.NewLine());
        cmds.Add(EscPosHelper.GetBytes($"Date: {DateTime.Now:MM/dd/yyyy HH:mm}"));
        cmds.Add(EscPosHelper.NewLine());
        
        // Dynamic separator
        cmds.Add(EscPosHelper.GetBytes(new string('-', widthChars))); 
        cmds.Add(EscPosHelper.NewLine());

        // Items
        foreach (var line in lines)
        {
             cmds.Add(EscPosHelper.BoldOn());
             // Qty x Name
             cmds.Add(EscPosHelper.GetBytes($"{line.Quantity} {line.MenuItemName}"));
             cmds.Add(EscPosHelper.BoldOff());
             cmds.Add(EscPosHelper.NewLine());

             // Modifiers
             if (line.Modifiers.Any())
             {
                 foreach (var mod in line.Modifiers)
                 {
                     cmds.Add(EscPosHelper.GetBytes($"   + {mod.Name}"));
                     cmds.Add(EscPosHelper.NewLine());
                 }
             }

             // Instructions
             if (!string.IsNullOrWhiteSpace(line.Instructions))
             {
                 cmds.Add(EscPosHelper.BoldOn());
                 cmds.Add(EscPosHelper.GetBytes($"   ** {line.Instructions} **"));
                 cmds.Add(EscPosHelper.BoldOff());
                 cmds.Add(EscPosHelper.NewLine());
             }
             
             cmds.Add(EscPosHelper.NewLine());
        }

        cmds.Add(EscPosHelper.NewLine());
        cmds.Add(EscPosHelper.NewLine());
        cmds.Add(EscPosHelper.NewLine());
        
        // Conditional Cut
        if (shouldCut)
        {
            cmds.Add(EscPosHelper.Cut());
        }

        return EscPosHelper.GenerateTicketData(cmds);
    }
}

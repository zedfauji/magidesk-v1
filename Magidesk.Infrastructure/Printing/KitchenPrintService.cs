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
    private readonly ITerminalContext _terminalContext;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<KitchenPrintService> _logger;

    public KitchenPrintService(
        IRawPrintService rawPrintService,
        IPrinterMappingRepository printerMappingRepository,
        ITerminalContext terminalContext,
        IUserRepository userRepository,
        ILogger<KitchenPrintService> logger)
    {
        _rawPrintService = rawPrintService;
        _printerMappingRepository = printerMappingRepository;
        _terminalContext = terminalContext;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<bool> PrintOrderLineAsync(OrderLine orderLine, Ticket ticket, CancellationToken cancellationToken = default)
    {
        if (!orderLine.ShouldPrintToKitchen) return false;
        
        return await PrintSpecificLinesAsync(ticket, new[] { orderLine }, cancellationToken);
    }

    public async Task<int> PrintTicketAsync(Ticket ticket, CancellationToken cancellationToken = default)
    {
        var linesToPrint = ticket.OrderLines.Where(x => x.ShouldPrintToKitchen && !x.PrintedToKitchen).ToList();
        if (!linesToPrint.Any()) return 0;

        bool success = await PrintSpecificLinesAsync(ticket, linesToPrint, cancellationToken);
        return success ? linesToPrint.Count : 0;
    }

    public Task MarkOrderLinePrintedAsync(OrderLine orderLine, CancellationToken cancellationToken = default)
    {
        orderLine.MarkPrintedToKitchen();
        return Task.CompletedTask;
    }
    
    private async Task<bool> PrintSpecificLinesAsync(Ticket ticket, IEnumerable<OrderLine> lines, CancellationToken cancellationToken)
    {
        try
        {
            var terminalId = _terminalContext.TerminalId ?? Guid.Empty;
            if (terminalId == Guid.Empty)
            {
                 _logger.LogWarning("Current Terminal ID is empty. Cannot resolve printer mappings.");
                 return false;
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

            bool allPrinted = true;

            foreach (var group in groupedLines)
            {
                var printerGroupId = group.Key;
                var groupLines = group.ToList();

                // Find mapping
                var mapping = mappings.FirstOrDefault(m => m.PrinterGroupId == printerGroupId);
                
                if (mapping == null)
                {
                    _logger.LogWarning($"No printer mapping found for PrinterGroup {printerGroupId} on Terminal {terminalId}. Lines skipped.");
                    allPrinted = false; 
                    continue; 
                }

                if (string.IsNullOrEmpty(mapping.PhysicalPrinterName))
                {
                    _logger.LogWarning($"PhysicalPrinterName is empty for Group {printerGroupId}.");
                    allPrinted = false;
                    continue;
                }

                // Generate ESC/POS
                var bytes = GenerateTicketBytes(ticket, groupLines, serverName);
                
                // Print
                try
                {
                    await _rawPrintService.PrintRawBytesAsync(mapping.PhysicalPrinterName, bytes);
                    
                    // Mark as printed
                    foreach (var line in groupLines)
                    {
                        await MarkOrderLinePrintedAsync(line, cancellationToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Failed to print to {mapping.PhysicalPrinterName}.");
                    allPrinted = false;
                }
            }

            return allPrinted;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in PrintSpecificLinesAsync");
            return false;
        }
    }

    private byte[] GenerateTicketBytes(Ticket ticket, List<OrderLine> lines, string serverName)
    {
        var cmds = new List<byte[]>();

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
        cmds.Add(EscPosHelper.GetBytes(new string('-', 32))); 
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
        cmds.Add(EscPosHelper.Cut());

        return EscPosHelper.GenerateTicketData(cmds);
    }
}

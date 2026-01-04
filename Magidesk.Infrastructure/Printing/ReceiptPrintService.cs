using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Infrastructure.Printing;

public class ReceiptPrintService : IReceiptPrintService
{
    private readonly IRawPrintService _rawPrintService;
    private readonly IPrinterMappingRepository _printerMappingRepository;
    private readonly IPrinterGroupRepository _printerGroupRepository;
    private readonly ITerminalContext _terminalContext;
    private readonly IUserRepository _userRepository;
    private readonly IAuditEventRepository _auditRepo;
    private readonly ILogger<ReceiptPrintService> _logger;

    public ReceiptPrintService(
        IRawPrintService rawPrintService,
        IPrinterMappingRepository printerMappingRepository,
        IPrinterGroupRepository printerGroupRepository,
        ITerminalContext terminalContext,
        IUserRepository userRepository,
        IAuditEventRepository auditRepo,
        ILogger<ReceiptPrintService> logger)
    {
        _rawPrintService = rawPrintService;
        _printerMappingRepository = printerMappingRepository;
        _printerGroupRepository = printerGroupRepository;
        _terminalContext = terminalContext;
        _userRepository = userRepository;
        _auditRepo = auditRepo;
        _logger = logger;
    }

    public async Task<bool> PrintTicketReceiptAsync(Ticket ticket, Guid? userId = null, CancellationToken cancellationToken = default)
    {
        try
        {
            // 1. Resolve Printer Context
            var (mapping, group) = await ResolveReceiptPrinterContextAsync(cancellationToken);
            
            if (mapping == null || string.IsNullOrEmpty(mapping.PhysicalPrinterName))
            {
                _logger.LogWarning($"Receipt printing skipped: No Receipt Printer mapped for Terminal {_terminalContext.TerminalIdentity}");
                return false;
            }

            // 2. Resolve Server Name
            string serverName = await ResolveUserNameAsync(ticket.CreatedBy.Value, cancellationToken);

            // 3. Determine Layout Settings
            bool showPrices = group?.ShowPrices ?? true; // Default to true for Receipts
            bool shouldCut = ShouldCut(group, mapping);
            int width = mapping.PrintableWidthChars > 0 ? mapping.PrintableWidthChars : 32;

            // 3. Generate Data
            var bytes = GenerateReceiptBytes(ticket, serverName, isRefund: false, width, showPrices, shouldCut);

            // 4. Print with Retry
            bool printed = await ExecutePrintWithRetryAsync(mapping.PhysicalPrinterName, bytes, group);

            if (printed)
            {
                // 5. Audit
                await logAuditAsync("Ticket", ticket.Id, userId, $"Ticket #{ticket.TicketNumber} Printed", $"Total: {ticket.TotalAmount}", ticket.Id, cancellationToken);
                return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to print ticket receipt.");
            return false;
        }
    }

    public async Task<bool> PrintPaymentReceiptAsync(Payment payment, Ticket ticket, Guid? userId = null, CancellationToken cancellationToken = default)
    {
        return await PrintTicketReceiptAsync(ticket, userId, cancellationToken);
    }

    public async Task<bool> PrintRefundReceiptAsync(Payment refundPayment, Ticket ticket, Guid? userId = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var (mapping, group) = await ResolveReceiptPrinterContextAsync(cancellationToken);
            if (mapping == null || string.IsNullOrEmpty(mapping.PhysicalPrinterName)) return false;

            string serverName = await ResolveUserNameAsync(ticket.CreatedBy.Value, cancellationToken);
            
            bool showPrices = group?.ShowPrices ?? true;
            bool shouldCut = ShouldCut(group, mapping);
            int width = mapping.PrintableWidthChars > 0 ? mapping.PrintableWidthChars : 32;

            var bytes = GenerateReceiptBytes(ticket, serverName, isRefund: true, width, showPrices, shouldCut);

            bool printed = await ExecutePrintWithRetryAsync(mapping.PhysicalPrinterName, bytes, group);
            
            if (printed)
            {
                await logAuditAsync("Refund", refundPayment.Id, userId, "Refund Receipt Printed", $"Amount: {refundPayment.Amount}", ticket.Id, cancellationToken);
                return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to print refund receipt.");
            return false;
        }
    }

    private async Task<(PrinterMapping? mapping, PrinterGroup? group)> ResolveReceiptPrinterContextAsync(CancellationToken cancellationToken)
    {
        var terminalId = _terminalContext.TerminalId ?? Guid.Empty;
        if (terminalId == Guid.Empty) return (null, null);

        var groups = await _printerGroupRepository.GetAllAsync(cancellationToken);
        var receiptGroup = groups.FirstOrDefault(g => g.Type == PrinterType.Receipt);

        if (receiptGroup == null) return (null, null);

        var mappings = await _printerMappingRepository.GetByTerminalIdAsync(terminalId, cancellationToken);
        var mapping = mappings.FirstOrDefault(m => m.PrinterGroupId == receiptGroup.Id);

        return (mapping, receiptGroup);
    }

    private bool ShouldCut(PrinterGroup? group, PrinterMapping mapping)
    {
        if (group == null) return mapping.CutEnabled;

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
        // Sanity Check
        if (delayMs < 100) delayMs = 100;

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
                if (attempts > maxRetries) return false;
                await Task.Delay(delayMs);
            }
        }
        return false;
    }

    private async Task<string> ResolveUserNameAsync(Guid userId, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
            return user != null ? $"{user.FirstName} {user.LastName}".Trim() : "Unknown";
        }
        catch
        {
            return "Unknown";
        }
    }

    private async Task logAuditAsync(string entityType, Guid entityId, Guid? userId, string action, string details, Guid ticketId, CancellationToken token)
    {
        try
        {
            var audit = AuditEvent.Create(
                AuditEventType.Printed,
                entityType,
                entityId,
                userId ?? Guid.Empty,
                action,
                details,
                null,
                ticketId
            );
            await _auditRepo.AddAsync(audit, token);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to create audit log for printing.");
        }
    }

    private byte[] GenerateReceiptBytes(Ticket ticket, string serverName, bool isRefund, int width, bool showPrices, bool shouldCut)
    {
        var cmds = new List<byte[]>();

        cmds.Add(EscPosHelper.Initialize());
        cmds.Add(EscPosHelper.AlignCenter());
        
        // Logo or Header Text
        cmds.Add(EscPosHelper.BoldOn());
        cmds.Add(EscPosHelper.DoubleHeightOn());
        cmds.Add(EscPosHelper.GetBytes(isRefund ? "** REFUND RECEIPT **" : "MAGIDESK POS"));
        cmds.Add(EscPosHelper.NormalSize());
        cmds.Add(EscPosHelper.BoldOff());
        cmds.Add(EscPosHelper.NewLine());
        
        cmds.Add(EscPosHelper.GetBytes("123 Main Street, Cityville"));
        cmds.Add(EscPosHelper.NewLine());
        cmds.Add(EscPosHelper.GetBytes("Tel: (555) 123-4567"));
        cmds.Add(EscPosHelper.NewLine());
        cmds.Add(EscPosHelper.NewLine());

        // Ticket Info
        cmds.Add(EscPosHelper.AlignLeft());
        cmds.Add(EscPosHelper.GetBytes($"Chk: {ticket.TicketNumber}"));
        cmds.Add(EscPosHelper.NewLine());
        cmds.Add(EscPosHelper.GetBytes($"Date: {DateTime.Now:MM/dd/yyyy HH:mm}"));
        cmds.Add(EscPosHelper.NewLine());
        cmds.Add(EscPosHelper.GetBytes($"Svr: {serverName}"));
        cmds.Add(EscPosHelper.NewLine());
        
        // Separator
        cmds.Add(EscPosHelper.GetBytes(new string('-', width))); 
        cmds.Add(EscPosHelper.NewLine());

        // Items
        foreach (var line in ticket.OrderLines)
        {
             // Simple layout: Qty Name Price
             // Right align price is hard in pure bytes without calc, assuming 32 chars
             // Layout: "1 Burger        10.00"
             
             string lineStr = $"{line.Quantity} {line.MenuItemName}";
             string priceStr = showPrices ? line.TotalAmount.ToString() : ""; 

             cmds.Add(EscPosHelper.GetBytes(lineStr));
             cmds.Add(EscPosHelper.NewLine());
             // Print modifiers indented
             foreach(var mod in line.Modifiers)
             {
                 string modPrice = showPrices ? mod.TotalAmount.ToString() : "";
                 cmds.Add(EscPosHelper.GetBytes($"  + {mod.Name}  {modPrice}"));
                 cmds.Add(EscPosHelper.NewLine());
             }
             
             // Price on next line right aligned?
             if (showPrices)
             {
                 cmds.Add(EscPosHelper.AlignRight());
                 cmds.Add(EscPosHelper.GetBytes(priceStr));
                 cmds.Add(EscPosHelper.AlignLeft());
                 cmds.Add(EscPosHelper.NewLine());
             }
        }

        cmds.Add(EscPosHelper.GetBytes(new string('-', width)));
        cmds.Add(EscPosHelper.NewLine());

        // Totals
        if (showPrices)
        {
            cmds.Add(EscPosHelper.AlignRight());
            cmds.Add(EscPosHelper.GetBytes($"Subtotal: {ticket.SubtotalAmount}"));
            cmds.Add(EscPosHelper.NewLine());
            cmds.Add(EscPosHelper.GetBytes($"Tax: {ticket.TaxAmount}"));
            cmds.Add(EscPosHelper.NewLine());
            cmds.Add(EscPosHelper.DoubleHeightOn());
            cmds.Add(EscPosHelper.GetBytes($"TOTAL: {ticket.TotalAmount}"));
            cmds.Add(EscPosHelper.NormalSize());
            cmds.Add(EscPosHelper.NewLine());
            cmds.Add(EscPosHelper.NewLine());
        }

        // Payments
        if (showPrices && ticket.Payments.Any())
        {
            cmds.Add(EscPosHelper.AlignLeft());
            cmds.Add(EscPosHelper.GetBytes("Payments:"));
            cmds.Add(EscPosHelper.NewLine());
            foreach(var pay in ticket.Payments)
            {
                 cmds.Add(EscPosHelper.GetBytes($"{pay.PaymentType}: {pay.Amount}"));
                 cmds.Add(EscPosHelper.NewLine());
            }
            
            cmds.Add(EscPosHelper.AlignRight());
            cmds.Add(EscPosHelper.GetBytes($"Balance Due: {ticket.DueAmount}"));
            cmds.Add(EscPosHelper.NewLine());
        }

        cmds.Add(EscPosHelper.AlignCenter());
        cmds.Add(EscPosHelper.NewLine());
        cmds.Add(EscPosHelper.GetBytes("Thank You!"));
        cmds.Add(EscPosHelper.NewLine());
        cmds.Add(EscPosHelper.NewLine());
        cmds.Add(EscPosHelper.NewLine());
        
        if (shouldCut)
        {
            cmds.Add(EscPosHelper.Cut());
        }

        return EscPosHelper.GenerateTicketData(cmds);
    }
}

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
            // 1. Resolve Printer
            var printerName = await ResolveReceiptPrinterAsync(cancellationToken);
            if (string.IsNullOrEmpty(printerName))
            {
                _logger.LogWarning($"Receipt printing skipped: No Receipt Printer mapped for Terminal {_terminalContext.TerminalIdentity}");
                return false;
            }

            // 2. Resolve Server Name
            string serverName = await ResolveUserNameAsync(ticket.CreatedBy.Value, cancellationToken);

            // 3. Generate Data
            var bytes = GenerateReceiptBytes(ticket, serverName, isRefund: false);

            // 4. Print
            await _rawPrintService.PrintRawBytesAsync(printerName, bytes);

            // 5. Audit
            await logAuditAsync("Ticket", ticket.Id, userId, $"Ticket #{ticket.TicketNumber} Printed", $"Total: {ticket.TotalAmount}", ticket.Id, cancellationToken);
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to print ticket receipt.");
            return false;
        }
    }

    public async Task<bool> PrintPaymentReceiptAsync(Payment payment, Ticket ticket, Guid? userId = null, CancellationToken cancellationToken = default)
    {
        // For now, payment receipt is just a full receipt with payment info emphasized, or maybe just a chit?
        // Usually customers want the full bill with the payment line added.
        // So we can re-use the main receipt logic but ensure payments are up to date in the ticket object passed.
        
        // However, if we need a specific "Credit Card Slip", that requires different logic.
        // Following "No Silent Failures", we will attempt to print the full ticket receipt which includes payments.
        
        return await PrintTicketReceiptAsync(ticket, userId, cancellationToken);
    }

    public async Task<bool> PrintRefundReceiptAsync(Payment refundPayment, Ticket ticket, Guid? userId = null, CancellationToken cancellationToken = default)
    {
        try
        {
             var printerName = await ResolveReceiptPrinterAsync(cancellationToken);
            if (string.IsNullOrEmpty(printerName)) return false;

            string serverName = await ResolveUserNameAsync(ticket.CreatedBy.Value, cancellationToken);
            var bytes = GenerateReceiptBytes(ticket, serverName, isRefund: true);

            await _rawPrintService.PrintRawBytesAsync(printerName, bytes);
            
            await logAuditAsync("Refund", refundPayment.Id, userId, "Refund Receipt Printed", $"Amount: {refundPayment.Amount}", ticket.Id, cancellationToken);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to print refund receipt.");
            return false;
        }
    }

    private async Task<string?> ResolveReceiptPrinterAsync(CancellationToken cancellationToken)
    {
        var terminalId = _terminalContext.TerminalId ?? Guid.Empty;
        if (terminalId == Guid.Empty) return null;

        var groups = await _printerGroupRepository.GetAllAsync(cancellationToken);
        var receiptGroup = groups.FirstOrDefault(g => g.Type == PrinterType.Receipt);

        if (receiptGroup == null) return null;

        var mappings = await _printerMappingRepository.GetByTerminalIdAsync(terminalId, cancellationToken);
        var mapping = mappings.FirstOrDefault(m => m.PrinterGroupId == receiptGroup.Id);

        return mapping?.PhysicalPrinterName;
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

    private byte[] GenerateReceiptBytes(Ticket ticket, string serverName, bool isRefund)
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
        cmds.Add(EscPosHelper.GetBytes(new string('-', 32))); // Separator in 32 col mode usually safe
        cmds.Add(EscPosHelper.NewLine());

        // Items
        foreach (var line in ticket.OrderLines)
        {
             // Simple layout: Qty Name Price
             // Right align price is hard in pure bytes without calc, assuming 32 chars
             // Layout: "1 Burger        10.00"
             
             string lineStr = $"{line.Quantity} {line.MenuItemName}";
             string priceStr = line.TotalAmount.ToString(); // ToString calls Money.ToString() which has symbol

             cmds.Add(EscPosHelper.GetBytes(lineStr));
             cmds.Add(EscPosHelper.NewLine());
             // Print modifiers indented
             foreach(var mod in line.Modifiers)
             {
                 cmds.Add(EscPosHelper.GetBytes($"  + {mod.Name}  {mod.TotalAmount}"));
                 cmds.Add(EscPosHelper.NewLine());
             }
             
             // Price on next line right aligned? Or trying to fit?
             // For simple implementation:
             cmds.Add(EscPosHelper.AlignRight());
             cmds.Add(EscPosHelper.GetBytes(priceStr));
             cmds.Add(EscPosHelper.AlignLeft());
             cmds.Add(EscPosHelper.NewLine());
        }

        cmds.Add(EscPosHelper.GetBytes(new string('-', 32)));
        cmds.Add(EscPosHelper.NewLine());

        // Totals
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

        // Payments
        if (ticket.Payments.Any())
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
        cmds.Add(EscPosHelper.Cut());

        return EscPosHelper.GenerateTicketData(cmds);
    }
}

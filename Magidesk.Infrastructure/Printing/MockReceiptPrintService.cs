using Microsoft.Extensions.Logging;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Infrastructure.Printing;

/// <summary>
/// Mock implementation of receipt print service for development.
/// Logs print operations instead of actually printing.
/// </summary>
public class MockReceiptPrintService : IReceiptPrintService
{
    private readonly ILogger<MockReceiptPrintService>? _logger;
    private readonly IAuditEventRepository _auditRepo;

    public MockReceiptPrintService(IAuditEventRepository auditRepo, ILogger<MockReceiptPrintService>? logger = null)
    {
        _auditRepo = auditRepo;
        _logger = logger;
    }

    public async Task<bool> PrintTicketReceiptAsync(Ticket ticket, Guid? userId = null, CancellationToken cancellationToken = default)
    {
        // 1. Audit Log
        try 
        {
            var audit = AuditEvent.Create(
                AuditEventType.Printed,
                "Ticket",
                ticket.Id,
                userId ?? Guid.Empty,
                $"Ticket #{ticket.TicketNumber} Printed",
                $"Receipt printed. Total: {ticket.TotalAmount}",
                null,
                ticket.Id
            );
            await _auditRepo.AddAsync(audit, cancellationToken);
        }
        catch(Exception ex)
        {
             _logger?.LogError(ex, "Failed to log audit event for print.");
        }

        // 2. Mock Print Log
        _logger?.LogInformation(
            "RECEIPT PRINT: Ticket #{TicketNumber} - Total: {TotalAmount}",
            ticket.TicketNumber,
            ticket.TotalAmount);

        _logger?.LogInformation("  Order Lines:");
        foreach (var orderLine in ticket.OrderLines)
        {
            _logger?.LogInformation(
                "    - {MenuItemName} x{Quantity} @ {UnitPrice} = {LineTotal}",
                orderLine.MenuItemName,
                orderLine.Quantity,
                orderLine.UnitPrice,
                orderLine.TotalAmount);
        }

        _logger?.LogInformation("  Payments:");
        foreach (var payment in ticket.Payments)
        {
            _logger?.LogInformation(
                "    - {PaymentType} {Amount}",
                payment.PaymentType,
                payment.Amount);
        }

        _logger?.LogInformation(
            "  Subtotal: {Subtotal}, Tax: {Tax}, Total: {Total}",
            ticket.SubtotalAmount,
            ticket.TaxAmount,
            ticket.TotalAmount);

        return true;
    }

    public async Task<bool> PrintPaymentReceiptAsync(Payment payment, Ticket ticket, Guid? userId = null, CancellationToken cancellationToken = default)
    {
         // Audit
         try
         {
             var audit = AuditEvent.Create(
                AuditEventType.Printed,
                "Payment",
                payment.Id,
                userId ?? Guid.Empty,
                $"Payment ({payment.PaymentType}) Receipt Printed",
                $"Amount: {payment.Amount}",
                null,
                ticket.Id
            );
            await _auditRepo.AddAsync(audit, cancellationToken);
         }
         catch(Exception ex)
         {
             _logger?.LogError(ex, "Failed to log audit event for payment print.");
         }

        _logger?.LogInformation(
            "PAYMENT RECEIPT: Payment {PaymentId} - {PaymentType} {Amount} (Ticket #{TicketNumber})",
            payment.Id,
            payment.PaymentType,
            payment.Amount,
            ticket.TicketNumber);

        if (payment.PaymentType == PaymentType.Cash && payment is CashPayment cashPayment)
        {
            _logger?.LogInformation(
                "  Tender: {TenderAmount}, Change: {ChangeAmount}",
                cashPayment.TenderAmount,
                cashPayment.ChangeAmount);
        }

        if (payment.TipsAmount > Money.Zero())
        {
            _logger?.LogInformation("  Tips: {TipsAmount}", payment.TipsAmount);
        }

        return true;
    }

    public async Task<bool> PrintRefundReceiptAsync(Payment refundPayment, Ticket ticket, Guid? userId = null, CancellationToken cancellationToken = default)
    {
         // Audit
         try
         {
             var audit = AuditEvent.Create(
                AuditEventType.Printed,
                "Refund",
                refundPayment.Id,
                userId ?? Guid.Empty,
                $"Refund ({refundPayment.PaymentType}) Receipt Printed",
                $"Amount: {refundPayment.Amount}",
                null,
                ticket.Id
            );
            await _auditRepo.AddAsync(audit, cancellationToken);
         }
         catch(Exception ex)
         {
             _logger?.LogError(ex, "Failed to log audit event for refund print.");
         }

        _logger?.LogInformation(
            "REFUND RECEIPT: Refund {RefundId} - {PaymentType} {Amount} (Ticket #{TicketNumber})",
            refundPayment.Id,
            refundPayment.PaymentType,
            refundPayment.Amount,
            ticket.TicketNumber);

        if (!string.IsNullOrEmpty(refundPayment.Note))
        {
            _logger?.LogInformation("  Reason: {Reason}", refundPayment.Note);
        }

        return true;
    }

    public async Task<bool> OpenCashDrawerAsync(CancellationToken cancellationToken = default)
    {
        _logger?.LogInformation("RECEIPT PRINT: [MOCK] OPEN CASH DRAWER (Pulse Sent)");
        await Task.Delay(50, cancellationToken); // Simulate hardware latency
        return true;
    }
}

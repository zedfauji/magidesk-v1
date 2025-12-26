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

    public MockReceiptPrintService(ILogger<MockReceiptPrintService>? logger = null)
    {
        _logger = logger;
    }

    public Task<bool> PrintTicketReceiptAsync(Ticket ticket, CancellationToken cancellationToken = default)
    {
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

        return Task.FromResult(true);
    }

    public Task<bool> PrintPaymentReceiptAsync(Payment payment, Ticket ticket, CancellationToken cancellationToken = default)
    {
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

        return Task.FromResult(true);
    }

    public Task<bool> PrintRefundReceiptAsync(Payment refundPayment, Ticket ticket, CancellationToken cancellationToken = default)
    {
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

        return Task.FromResult(true);
    }
}


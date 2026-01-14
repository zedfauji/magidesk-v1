using Magidesk.Domain.ValueObjects;

namespace Magidesk.Application.Commands;

/// <summary>
/// Command to process a split payment for a ticket.
/// </summary>
public record ProcessSplitPaymentCommand
{
    public Guid TicketId { get; init; }
    public IReadOnlyList<SplitPaymentEntry> Payments { get; init; }
    public UserId ProcessedBy { get; init; }

    public ProcessSplitPaymentCommand(
        Guid ticketId,
        IReadOnlyList<SplitPaymentEntry> payments,
        UserId processedBy)
    {
        if (payments == null || payments.Count == 0)
        {
            throw new ArgumentException("Payments list cannot be null or empty.", nameof(payments));
        }

        TicketId = ticketId;
        Payments = payments;
        ProcessedBy = processedBy;
    }
}

/// <summary>
/// Result of processing a split payment.
/// </summary>
public class ProcessSplitPaymentResult
{
    public IReadOnlyList<Guid> PaymentIds { get; set; } = null!;
    public Money ChangeAmount { get; set; } = null!;
    public Money RemainingAmount { get; set; } = null!;
    public bool TicketIsPaid { get; set; }
    public bool IsUnderpayment { get; set; }
}

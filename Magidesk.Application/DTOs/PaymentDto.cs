using Magidesk.Domain.Enumerations;

namespace Magidesk.Application.DTOs;

/// <summary>
/// Data transfer object for Payment entity.
/// </summary>
public class PaymentDto
{
    public Guid Id { get; set; }
    public string? GlobalId { get; set; }
    public Guid TicketId { get; set; }
    public TransactionType TransactionType { get; set; }
    public PaymentType PaymentType { get; set; }
    public decimal Amount { get; set; }
    public decimal TipsAmount { get; set; }
    public decimal TenderAmount { get; set; }
    public decimal ChangeAmount { get; set; }
    public DateTime TransactionTime { get; set; }
    public Guid ProcessedBy { get; set; }
    public Guid TerminalId { get; set; }
    public bool IsCaptured { get; set; }
    public bool IsVoided { get; set; }
    public bool IsAuthorizable { get; set; }
    public Guid? CashSessionId { get; set; }
    public string? Note { get; set; }
}


namespace Magidesk.Application.Commands;

/// <summary>
/// Command to print a receipt.
/// </summary>
public class PrintReceiptCommand
{
    public Guid TicketId { get; set; }
    public Guid? PaymentId { get; set; } // If null, prints ticket receipt; if set, prints payment receipt
    public ReceiptType ReceiptType { get; set; } = ReceiptType.Ticket;
}

/// <summary>
/// Type of receipt to print.
/// </summary>
public enum ReceiptType
{
    Ticket,
    Payment,
    Refund
}

/// <summary>
/// Result of printing receipt.
/// </summary>
public class PrintReceiptResult
{
    public bool Success { get; set; }
}


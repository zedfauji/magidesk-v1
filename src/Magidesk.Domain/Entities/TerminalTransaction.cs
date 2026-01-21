using System;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Domain.Entities;

public class TerminalTransaction
{
    public Guid Id { get; private set; }
    public Guid CashSessionId { get; private set; }
    public TerminalTransactionType Type { get; private set; }
    public Money Amount { get; private set; }
    public string Reference { get; private set; } = string.Empty; // TicketID, Note, etc.
    public DateTime Timestamp { get; private set; }
    
    // Optional: Link to User who performed it? 
    // Usually implicitly the Session User, but Manager Override might differ.
    // For now, keep simple.

    protected TerminalTransaction() 
    {
        Amount = Money.Zero(); // Fix CS8618
    }

    public TerminalTransaction(Guid cashSessionId, TerminalTransactionType type, Money amount, string reference)
    {
        Id = Guid.NewGuid();
        CashSessionId = cashSessionId;
        Type = type;
        Amount = amount;
        Reference = reference ?? string.Empty;
        Timestamp = DateTime.UtcNow;
    }
}

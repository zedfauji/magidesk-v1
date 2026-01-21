namespace Magidesk.Domain.DomainEvents;

/// <summary>
/// Base class for domain events.
/// </summary>
public abstract class DomainEventBase : IDomainEvent
{
    public DateTime OccurredAt { get; protected set; }
    public Guid? CorrelationId { get; protected set; }

    protected DomainEventBase()
    {
        OccurredAt = DateTime.UtcNow;
    }

    protected DomainEventBase(Guid? correlationId) : this()
    {
        CorrelationId = correlationId;
    }
}


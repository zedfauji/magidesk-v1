namespace Magidesk.Domain.DomainEvents;

/// <summary>
/// Marker interface for domain events.
/// </summary>
public interface IDomainEvent
{
    /// <summary>
    /// Gets the timestamp when the event occurred.
    /// </summary>
    DateTime OccurredAt { get; }

    /// <summary>
    /// Gets the correlation ID for grouping related events.
    /// </summary>
    Guid? CorrelationId { get; }
}


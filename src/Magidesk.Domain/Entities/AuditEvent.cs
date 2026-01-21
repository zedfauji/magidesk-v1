using System;
using Magidesk.Domain.Enumerations;

namespace Magidesk.Domain.Entities;

/// <summary>
/// Immutable audit record of all financial mutations.
/// </summary>
public class AuditEvent
{
    public Guid Id { get; private set; }
    public AuditEventType EventType { get; private set; }
    public string EntityType { get; private set; } = string.Empty;
    public Guid EntityId { get; private set; }
    public Guid UserId { get; private set; }
    public DateTime Timestamp { get; private set; }
    public string? BeforeState { get; private set; }
    public string AfterState { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public Guid? CorrelationId { get; private set; }

    // Private constructor for EF Core
    private AuditEvent()
    {
    }

    /// <summary>
    /// Creates a new audit event.
    /// </summary>
    public static AuditEvent Create(
        AuditEventType eventType,
        string entityType,
        Guid entityId,
        Guid userId,
        string afterState,
        string description,
        string? beforeState = null,
        Guid? correlationId = null)
    {
        if (string.IsNullOrWhiteSpace(entityType))
        {
            throw new ArgumentException("Entity type cannot be null or empty.", nameof(entityType));
        }

        if (string.IsNullOrWhiteSpace(afterState))
        {
            throw new ArgumentException("After state cannot be null or empty.", nameof(afterState));
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentException("Description cannot be null or empty.", nameof(description));
        }

        return new AuditEvent
        {
            Id = Guid.NewGuid(),
            EventType = eventType,
            EntityType = entityType,
            EntityId = entityId,
            UserId = userId,
            Timestamp = DateTime.UtcNow,
            BeforeState = beforeState,
            AfterState = afterState,
            Description = description,
            CorrelationId = correlationId
        };
    }
}


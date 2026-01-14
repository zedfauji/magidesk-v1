using Magidesk.Domain.Enumerations;

namespace Magidesk.Application.DTOs;

/// <summary>
/// DTO for displaying audit log entries.
/// </summary>
public class AuditLogDto
{
    public Guid Id { get; set; }
    public AuditEventType EventType { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public Guid EntityId { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string? BeforeState { get; set; }
    public string AfterState { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid? CorrelationId { get; set; }
}

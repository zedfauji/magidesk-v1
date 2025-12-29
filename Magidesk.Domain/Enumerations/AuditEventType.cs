namespace Magidesk.Domain.Enumerations;

/// <summary>
/// Represents the type of audit event.
/// </summary>
public enum AuditEventType
{
    Created = 0,
    Modified = 1,
    Deleted = 2,
    StatusChanged = 3,
    PaymentProcessed = 4,
    RefundProcessed = 5,
    Voided = 6,
    SystemShutdown = 7,
    Printed = 8,
    TicketTransferred = 9
}


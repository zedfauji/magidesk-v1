using System;

namespace Magidesk.Application.Commands.TableSessions;

/// <summary>
/// Command to transfer an active session between tables with data preservation validation.
/// </summary>
public record TransferSessionCommand(
    Guid SessionId,
    Guid TargetTableId,
    string Reason,
    Guid StaffId
);

/// <summary>
/// Result of transferring a session.
/// </summary>
public record TransferSessionResult(
    Guid OriginalSessionId,
    Guid NewSessionId,
    Guid OriginalTableId,
    Guid NewTableId,
    decimal PreservedCharge,
    TimeSpan PreservedDuration,
    DateTime TransferredAt
);
using System;

namespace Magidesk.Application.Commands.TableOperations;

/// <summary>
/// Command to transfer a session from one table to another.
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
    Guid SessionId,
    Guid SourceTableId,
    Guid TargetTableId,
    string TargetTableName,
    DateTime TransferredAt
);
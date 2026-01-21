using System;
using System.Collections.Generic;

namespace Magidesk.Application.Commands.TableOperations;

/// <summary>
/// Command to merge multiple tables into a single session.
/// </summary>
public record MergeTablesCommand(
    Guid PrimaryTableId,
    IEnumerable<Guid> SecondaryTableIds,
    string Reason,
    Guid StaffId
);

/// <summary>
/// Result of merging tables.
/// </summary>
public record MergeTablesResult(
    IReadOnlyList<Guid> MergedTableIds,
    Guid MergedSessionId,
    decimal TotalCharge,
    int TotalGuestCount,
    DateTime MergedAt,
    Guid StaffId
);
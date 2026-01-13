using System;
using System.Collections.Generic;

namespace Magidesk.Application.Commands.TableOperations;

/// <summary>
/// Command to split a merged table back into individual tables.
/// </summary>
public record SplitTablesCommand(
    Guid MergedSessionId,
    IEnumerable<TableSplitAllocationInfo> SplitAllocations,
    string Reason,
    Guid StaffId
);

/// <summary>
/// Information about how to allocate charges and guests when splitting tables.
/// </summary>
public record TableSplitAllocationInfo(
    Guid TargetTableId,
    decimal AllocationPercentage,
    int GuestCount
);

/// <summary>
/// Result of splitting tables.
/// </summary>
public record SplitTablesResult(
    Guid OriginalMergedSessionId,
    IReadOnlyList<Guid> SplitTableIds,
    IReadOnlyList<SplitSessionInfo> SplitSessions,
    decimal TotalChargesAllocated,
    DateTime SplitAt,
    Guid StaffId
);

/// <summary>
/// Information about a session created during table split.
/// </summary>
public record SplitSessionInfo(
    Guid SessionId,
    Guid TableId,
    decimal AllocatedCharge,
    int GuestCount
);
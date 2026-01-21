using System;

namespace Magidesk.Application.Commands.TableSessions;

/// <summary>
/// Command to update the guest count for an active session.
/// </summary>
public record UpdateGuestCountCommand(
    Guid SessionId,
    int NewGuestCount,
    Guid? StaffId = null
);

/// <summary>
/// Result of updating guest count.
/// </summary>
public record UpdateGuestCountResult(
    Guid SessionId,
    int PreviousGuestCount,
    int NewGuestCount,
    decimal CurrentCharge,
    DateTime UpdatedAt,
    Guid? StaffId
);
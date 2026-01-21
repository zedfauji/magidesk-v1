using System;

namespace Magidesk.Application.Commands.Security;

/// <summary>
/// Command to authorize a manager for a privileged operation.
/// Used for permission escalation workflow (e.g., void ticket, apply discount, refund).
/// </summary>
public record AuthorizeManagerCommand(
    string Pin,
    string OperationType  // "VoidTicket", "ApplyDiscount", "RefundPayment", "OpenDrawer", etc.
);

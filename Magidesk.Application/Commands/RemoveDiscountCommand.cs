using System;

namespace Magidesk.Application.Commands;

/// <summary>
/// Command to remove a discount from a ticket.
/// Task 2.1.6: Create RemoveDiscountCommand and handler
/// </summary>
/// <param name="TicketId">The ID of the ticket to remove the discount from</param>
/// <param name="DiscountId">The ID of the TicketDiscount to remove</param>
/// <param name="RemovedBy">The user removing the discount</param>
public record RemoveDiscountCommand(
    Guid TicketId,
    Guid DiscountId,
    Guid RemovedBy
);

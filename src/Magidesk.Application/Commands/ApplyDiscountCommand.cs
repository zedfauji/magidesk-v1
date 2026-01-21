using Magidesk.Domain.ValueObjects;

namespace Magidesk.Application.Commands;

/// <summary>
/// Command to apply a predefined discount to a ticket.
/// Task 2.1.5: Enhanced to work with Ticket.ApplyDiscount(Discount, UserId, UserId?) method.
/// </summary>
public class ApplyDiscountCommand
{
    /// <summary>
    /// The ticket to apply the discount to.
    /// </summary>
    public Guid TicketId { get; set; }
    
    /// <summary>
    /// The predefined discount to apply.
    /// </summary>
    public Guid DiscountId { get; set; }
    
    /// <summary>
    /// The user applying the discount.
    /// </summary>
    public UserId AppliedBy { get; set; } = null!;
    
    /// <summary>
    /// Optional manager who authorized the discount (required for discounts > 50%).
    /// </summary>
    public UserId? AuthorizedBy { get; set; }
    
    // Legacy properties for backward compatibility with ad-hoc discounts
    public Guid? OrderLineId { get; set; } // If null, applies to entire ticket
    public Magidesk.Domain.Enumerations.DiscountType? Type { get; set; }
    public decimal? Value { get; set; } // Percentage or Amount
    public string? Reason { get; set; } // Required for overrides
    public Guid? AuthorizingUserId { get; set; } // Required for overrides (legacy)
}


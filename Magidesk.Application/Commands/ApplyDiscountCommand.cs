namespace Magidesk.Application.Commands;

/// <summary>
/// Command to apply a discount to a ticket or order line.
/// </summary>
public class ApplyDiscountCommand
{
    public Guid TicketId { get; set; }
    public Guid? OrderLineId { get; set; } // If null, applies to entire ticket
    
    // Original property - made nullable for ad-hoc discounts
    public Guid? DiscountId { get; set; }
    
    // New properties for C.7 (Ad-hoc / Manager Override / Member)
    public Magidesk.Domain.Enumerations.DiscountType? Type { get; set; }
    public decimal? Value { get; set; } // Percentage or Amount
    public string? Reason { get; set; } // Required for overrides
    public Guid? AuthorizingUserId { get; set; } // Required for overrides
}


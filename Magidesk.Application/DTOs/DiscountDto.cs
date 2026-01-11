using Magidesk.Domain.Enumerations;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Application.DTOs;

public class DiscountDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DiscountType Type { get; set; }
    public decimal Value { get; set; }
    public Money? MinimumBuy { get; set; }
    public int? MinimumQuantity { get; set; }
    public bool AutoApply { get; set; }
    public bool IsActive { get; set; }
    public string? CouponCode { get; set; }
    public DateTime? ExpirationDate { get; set; }
    
    // Display helper
    public string DisplayValue => Type == DiscountType.Percentage ? $"{Value}%" : $"{Value:C}";
}

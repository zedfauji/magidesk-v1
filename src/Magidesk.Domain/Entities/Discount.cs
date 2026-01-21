using Magidesk.Domain.Enumerations;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Domain.Entities;

/// <summary>
/// Discount definition (reference data).
/// </summary>
public class Discount
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public DiscountType Type { get; private set; }
    public decimal Value { get; private set; }
    public Money? MinimumBuy { get; private set; }
    public int? MinimumQuantity { get; private set; }
    public QualificationType QualificationType { get; private set; }
    public ApplicationType ApplicationType { get; private set; }
    public bool AutoApply { get; private set; }
    public bool IsActive { get; private set; }
    public string? CouponCode { get; private set; }
    public DateTime? ExpirationDate { get; private set; }
    
    /// <summary>
    /// Indicates if this discount requires manager authorization (e.g., discounts > 50%).
    /// </summary>
    public bool RequiresAuthorization { get; private set; }

    // Private constructor for EF Core
    private Discount()
    {
    }

    /// <summary>
    /// Creates a new discount.
    /// </summary>
    public static Discount Create(
        string name,
        DiscountType type,
        decimal value,
        QualificationType qualificationType,
        ApplicationType applicationType,
        Money? minimumBuy = null,
        int? minimumQuantity = null,
        bool autoApply = false,
        string? couponCode = null,
        DateTime? expirationDate = null,
        bool requiresAuthorization = false)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Discount name cannot be null or empty.", nameof(name));
        }

        if (value < 0)
        {
            throw new Exceptions.BusinessRuleViolationException("Discount value cannot be negative.");
        }

        // Validate percentage is between 0-100
        if (type == DiscountType.Percentage && (value < 0 || value > 100))
        {
            throw new Exceptions.BusinessRuleViolationException("Percentage discount value must be between 0 and 100.");
        }

        return new Discount
        {
            Id = Guid.NewGuid(),
            Name = name,
            Type = type,
            Value = value,
            MinimumBuy = minimumBuy,
            MinimumQuantity = minimumQuantity,
            QualificationType = qualificationType,
            ApplicationType = applicationType,
            AutoApply = autoApply,
            CouponCode = couponCode,
            ExpirationDate = expirationDate,
            IsActive = true,
            RequiresAuthorization = requiresAuthorization
        };
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Activate()
    {
        IsActive = true;
    }

    /// <summary>
    /// Calculates the discount amount for a given base amount.
    /// </summary>
    /// <param name="amount">The base amount to apply the discount to.</param>
    /// <returns>The calculated discount amount.</returns>
    public Money CalculateDiscount(Money amount)
    {
        if (amount == null || amount.Amount <= 0)
        {
            return Money.Zero();
        }

        return Type switch
        {
            DiscountType.Percentage => new Money(amount.Amount * (Value / 100m), amount.Currency),
            DiscountType.FixedAmount => new Money(Math.Min(Value, amount.Amount), amount.Currency),
            DiscountType.Amount => new Money(Math.Min(Value, amount.Amount), amount.Currency),
            _ => Money.Zero()
        };
    }
}


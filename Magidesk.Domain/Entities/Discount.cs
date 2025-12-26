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
        DateTime? expirationDate = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Discount name cannot be null or empty.", nameof(name));
        }

        if (value < 0)
        {
            throw new Exceptions.BusinessRuleViolationException("Discount value cannot be negative.");
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
            IsActive = true
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
}


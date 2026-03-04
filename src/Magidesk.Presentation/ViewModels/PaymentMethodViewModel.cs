using Magidesk.Domain.Enumerations;

namespace Magidesk.Presentation.ViewModels;

/// <summary>
/// Represents a payment method option in the UI.
/// </summary>
public class PaymentMethodViewModel
{
    public PaymentMethodViewModel(PaymentType type, string displayName, string iconName, string backgroundColor)
    {
        Type = type;
        DisplayName = displayName;
        IconName = iconName;
        BackgroundColor = backgroundColor;
        IsEnabled = true;
    }

    public PaymentType Type { get; }
    public string DisplayName { get; }
    public string IconName { get; }
    public string BackgroundColor { get; }
    public bool IsEnabled { get; }
}

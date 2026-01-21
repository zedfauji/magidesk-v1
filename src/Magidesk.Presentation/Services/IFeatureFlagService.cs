namespace Magidesk.Presentation.Services;

/// <summary>
/// Service for managing feature flags to enable/disable features.
/// </summary>
public interface IFeatureFlagService
{
    /// <summary>
    /// Gets whether the redesigned Order/Settle pages are enabled.
    /// </summary>
    bool UseRedesignedOrderPages { get; }

    /// <summary>
    /// Sets whether the redesigned Order/Settle pages are enabled.
    /// </summary>
    void SetRedesignedOrderPages(bool enabled);
}

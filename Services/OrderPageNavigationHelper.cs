using System;

namespace Magidesk.Presentation.Services;

/// <summary>
/// Helper service to determine which order page to navigate to based on feature flags.
/// Centralizes the logic for switching between old and new UI.
/// </summary>
public class OrderPageNavigationHelper
{
    private readonly IFeatureFlagService _featureFlagService;

    public OrderPageNavigationHelper(IFeatureFlagService featureFlagService)
    {
        _featureFlagService = featureFlagService ?? throw new ArgumentNullException(nameof(featureFlagService));
    }

    /// <summary>
    /// Gets the appropriate order page type based on feature flags.
    /// </summary>
    public Type GetOrderPageType()
    {
        return _featureFlagService.UseRedesignedOrderPages
            ? typeof(Magidesk.Presentation.Views.OrderPageView)
            : typeof(Magidesk.Presentation.Views.OrderEntryPage);
    }

    /// <summary>
    /// Gets the appropriate settle page type based on feature flags.
    /// </summary>
    public Type GetSettlePageType()
    {
        return _featureFlagService.UseRedesignedOrderPages
            ? typeof(Magidesk.Presentation.Views.SettlePageView)
            : typeof(Magidesk.Presentation.Views.SettlePage);
    }
}

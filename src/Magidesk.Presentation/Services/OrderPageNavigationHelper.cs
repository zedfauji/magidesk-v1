using System;

namespace Magidesk.Presentation.Services;

/// <summary>
/// Helper service to determine which order page to navigate to.
/// Hard-wired to use the redesigned OrderPageView and SettlePageView.
/// </summary>
public class OrderPageNavigationHelper
{
    /// <summary>
    /// Gets the order page type (always OrderPageView).
    /// </summary>
    public Type GetOrderPageType()
    {
        return typeof(Magidesk.Presentation.Views.OrderPageView);
    }

    /// <summary>
    /// Gets the settle page type (always SettlePageView).
    /// </summary>
    public Type GetSettlePageType()
    {
        return typeof(Magidesk.Presentation.Views.SettlePageView);
    }
}

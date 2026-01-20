using Microsoft.Extensions.Configuration;

namespace Magidesk.Presentation.Services;

/// <summary>
/// Implementation of feature flag service.
/// Reads feature flags from configuration and allows runtime toggling.
/// </summary>
public class FeatureFlagService : IFeatureFlagService
{
    private bool _useRedesignedOrderPages;

    public FeatureFlagService(IConfiguration configuration)
    {
        // Read from configuration, default to true (enable new UI by default)
        _useRedesignedOrderPages = configuration.GetValue<bool>("FeatureFlags:UseRedesignedOrderPages", true);
    }

    public bool UseRedesignedOrderPages => _useRedesignedOrderPages;

    public void SetRedesignedOrderPages(bool enabled)
    {
        _useRedesignedOrderPages = enabled;
    }
}

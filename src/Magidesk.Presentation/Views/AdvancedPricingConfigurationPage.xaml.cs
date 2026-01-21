using Microsoft.UI.Xaml.Controls;
using Microsoft.Extensions.DependencyInjection;
using Magidesk.Presentation.ViewModels;

namespace Magidesk.Presentation.Views;

/// <summary>
/// Page for advanced pricing configuration and simulation.
/// </summary>
public sealed partial class AdvancedPricingConfigurationPage : Page
{
    public AdvancedPricingConfigurationViewModel ViewModel { get; }

    public AdvancedPricingConfigurationPage()
    {
        this.InitializeComponent();
        
        // Get ViewModel from DI container
        ViewModel = App.Services.GetRequiredService<AdvancedPricingConfigurationViewModel>();
        this.DataContext = ViewModel;
        
        // Initialize the ViewModel
        _ = InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        try
        {
            await ViewModel.InitializeAsync();
        }
        catch (Exception ex)
        {
            // Log error but don't crash the page
            System.Diagnostics.Debug.WriteLine($"Error initializing AdvancedPricingConfigurationPage: {ex.Message}");
        }
    }
}
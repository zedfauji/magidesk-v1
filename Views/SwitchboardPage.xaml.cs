using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Magidesk.Presentation.ViewModels;

namespace Magidesk.Presentation.Views;

public sealed partial class SwitchboardPage : Page
{
    public SwitchboardViewModel ViewModel { get; }
    private IServiceScope? _scope;

    public SwitchboardPage()
    {
        InitializeComponent();
        _scope = App.Services.CreateScope();
        ViewModel = _scope.ServiceProvider.GetRequiredService<SwitchboardViewModel>();
        DataContext = ViewModel;

        this.Unloaded += (s, e) =>
        {
             _scope?.Dispose();
             _scope = null;
        };
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        
        // Load open tickets when page is displayed
        _ = ViewModel.LoadTicketsAsync();
    }
}

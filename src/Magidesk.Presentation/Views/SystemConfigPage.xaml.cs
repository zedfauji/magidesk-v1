using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Magidesk.Presentation.ViewModels;

namespace Magidesk.Presentation.Views;

public sealed partial class SystemConfigPage : Page
{
    public SystemConfigViewModel ViewModel { get; }

    public SystemConfigPage()
    {
        InitializeComponent();
        ViewModel = App.Services.GetRequiredService<SystemConfigViewModel>();
        DataContext = ViewModel;
    }

    protected override async void OnNavigatedTo(Microsoft.UI.Xaml.Navigation.NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        await ViewModel.InitializeAsync();
    }
}

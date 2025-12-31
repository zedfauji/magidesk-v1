using Microsoft.UI.Xaml.Controls;
using Magidesk.Presentation.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Magidesk.Presentation.Views;

public sealed partial class DrawerPullReportDialog : ContentDialog
{
    public DrawerPullReportViewModel ViewModel { get; }

    public DrawerPullReportDialog()
    {
        InitializeComponent();
        ViewModel = App.Services.GetRequiredService<DrawerPullReportViewModel>();
        this.DataContext = ViewModel;
        this.Loaded += DrawerPullReportDialog_Loaded;
    }

    private async void DrawerPullReportDialog_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        await ViewModel.InitializeAsync();
    }
}

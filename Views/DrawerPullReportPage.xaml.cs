using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Magidesk.Presentation.ViewModels;

namespace Magidesk.Presentation.Views;

public sealed partial class DrawerPullReportPage : Page
{
    public DrawerPullReportViewModel ViewModel { get; }

    public DrawerPullReportPage()
    {
        InitializeComponent();
        ViewModel = App.Services.GetRequiredService<DrawerPullReportViewModel>();
        DataContext = ViewModel;
    }
}

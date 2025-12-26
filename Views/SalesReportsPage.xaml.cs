using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Magidesk.Presentation.ViewModels;

namespace Magidesk.Presentation.Views;

public sealed partial class SalesReportsPage : Page
{
    public SalesReportsViewModel ViewModel { get; }

    public SalesReportsPage()
    {
        InitializeComponent();
        ViewModel = App.Services.GetRequiredService<SalesReportsViewModel>();
        DataContext = ViewModel;
    }
}

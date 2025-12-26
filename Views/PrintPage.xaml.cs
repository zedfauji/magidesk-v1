using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Magidesk.Presentation.ViewModels;

namespace Magidesk.Presentation.Views;

public sealed partial class PrintPage : Page
{
    public PrintViewModel ViewModel { get; }

    public PrintPage()
    {
        InitializeComponent();
        ViewModel = App.Services.GetRequiredService<PrintViewModel>();
        DataContext = ViewModel;
    }
}

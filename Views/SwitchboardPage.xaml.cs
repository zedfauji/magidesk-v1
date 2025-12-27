using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Magidesk.Presentation.ViewModels;

namespace Magidesk.Presentation.Views;

public sealed partial class SwitchboardPage : Page
{
    public SwitchboardViewModel ViewModel { get; }

    public SwitchboardPage()
    {
        InitializeComponent();
        ViewModel = App.Services.GetRequiredService<SwitchboardViewModel>();
        DataContext = ViewModel;
    }
}

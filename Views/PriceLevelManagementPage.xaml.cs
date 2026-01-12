using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Magidesk.Presentation.ViewModels;

namespace Magidesk.Presentation.Views;

public sealed partial class PriceLevelManagementPage : Page
{
    public PriceLevelManagementViewModel ViewModel { get; }

    public PriceLevelManagementPage()
    {
        this.InitializeComponent();
        ViewModel = App.Services.GetService<PriceLevelManagementViewModel>();
        DataContext = ViewModel;
    }
}

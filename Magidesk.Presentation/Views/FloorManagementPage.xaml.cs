using Microsoft.UI.Xaml.Controls;
using Microsoft.Extensions.DependencyInjection;
using Magidesk.Presentation.ViewModels;

namespace Magidesk.Presentation.Views;

public sealed partial class FloorManagementPage : Page
{
    public FloorManagementViewModel ViewModel { get; }

    public FloorManagementPage()
    {
        this.InitializeComponent();
        ViewModel = App.Services.GetRequiredService<FloorManagementViewModel>();
    }
}

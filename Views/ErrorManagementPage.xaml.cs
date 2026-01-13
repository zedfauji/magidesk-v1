using Microsoft.UI.Xaml.Controls;
using Magidesk.Presentation.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Magidesk.Presentation.Views;

/// <summary>
/// Error management dashboard page for managers to view and resolve system errors.
/// </summary>
public sealed partial class ErrorManagementPage : Page
{
    public ErrorManagementViewModel ViewModel { get; }

    public ErrorManagementPage()
    {
        this.InitializeComponent();
        ViewModel = App.Services.GetRequiredService<ErrorManagementViewModel>();
        this.DataContext = ViewModel;
    }
}
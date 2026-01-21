using Microsoft.UI.Xaml.Controls;
using Magidesk.Presentation.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Magidesk.Presentation.Views;

public sealed partial class CashDropManagementDialog : ContentDialog
{
    public CashDropManagementViewModel ViewModel { get; }

    public CashDropManagementDialog()
    {
        this.InitializeComponent();
        ViewModel = App.Services.GetRequiredService<CashDropManagementViewModel>();
        this.DataContext = ViewModel;
    }
}

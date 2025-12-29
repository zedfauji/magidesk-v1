using Microsoft.UI.Xaml.Controls;
using Magidesk.Presentation.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Magidesk.Presentation.Views;

public sealed partial class OpenTicketsListDialog : ContentDialog
{
    public OpenTicketsListViewModel ViewModel { get; }

    public OpenTicketsListDialog()
    {
        this.InitializeComponent();
        ViewModel = App.Services.GetRequiredService<OpenTicketsListViewModel>();
        this.DataContext = ViewModel;
    }
}

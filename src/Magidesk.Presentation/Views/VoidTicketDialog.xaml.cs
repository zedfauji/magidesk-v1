using Microsoft.UI.Xaml.Controls;
using Microsoft.Extensions.DependencyInjection;
using Magidesk.Presentation.ViewModels;

namespace Magidesk.Presentation.Views;

public sealed partial class VoidTicketDialog : ContentDialog
{
    public VoidTicketViewModel ViewModel { get; }

    public VoidTicketDialog()
    {
        this.InitializeComponent();
        ViewModel = App.Services.GetRequiredService<VoidTicketViewModel>();
        DataContext = ViewModel;
    }
}

using Microsoft.UI.Xaml.Controls;
using Microsoft.Extensions.DependencyInjection;
using Magidesk.Presentation.ViewModels;

namespace Magidesk.Presentation.Views;

public sealed partial class RefundTicketDialog : ContentDialog
{
    public RefundTicketViewModel ViewModel { get; }

    public RefundTicketDialog()
    {
        this.InitializeComponent();
        ViewModel = App.Services.GetRequiredService<RefundTicketViewModel>();
        DataContext = ViewModel;
    }
}

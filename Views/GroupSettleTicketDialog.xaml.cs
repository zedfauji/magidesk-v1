using Magidesk.Presentation.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;

namespace Magidesk.Presentation.Views
{
    public sealed partial class GroupSettleTicketDialog : ContentDialog
    {
        public GroupSettleTicketViewModel ViewModel { get; }

        public GroupSettleTicketDialog()
        {
            this.InitializeComponent();
            ViewModel = App.Services.GetRequiredService<GroupSettleTicketViewModel>();
            this.DataContext = ViewModel;
            
            // Set the close action for the ViewModel
            ViewModel.CloseDialog = () => Hide();
        }
    }
}

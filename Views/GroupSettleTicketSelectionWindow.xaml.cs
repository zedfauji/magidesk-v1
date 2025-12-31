using Magidesk.Presentation.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;

namespace Magidesk.Presentation.Views
{
    public sealed partial class GroupSettleTicketSelectionWindow : ContentDialog
    {
        public GroupSettleTicketSelectionViewModel ViewModel { get; }

        public GroupSettleTicketSelectionWindow()
        {
            this.InitializeComponent();
            ViewModel = App.Services.GetRequiredService<GroupSettleTicketSelectionViewModel>();
            this.DataContext = ViewModel;
        }
    }
}

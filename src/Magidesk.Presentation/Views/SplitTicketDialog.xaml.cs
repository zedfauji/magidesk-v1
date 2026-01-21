using Magidesk.Presentation.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;

namespace Magidesk.Presentation.Views
{
    public sealed partial class SplitTicketDialog : ContentDialog
    {
        public SplitTicketViewModel ViewModel { get; }

        public SplitTicketDialog()
        {
            this.InitializeComponent();
            ViewModel = App.Services.GetRequiredService<SplitTicketViewModel>();
            this.DataContext = ViewModel;
        }
    }
}

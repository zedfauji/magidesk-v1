using Magidesk.Presentation.ViewModels.Dialogs;
using Microsoft.UI.Xaml.Controls;

namespace Magidesk.Views.Dialogs
{
    public sealed partial class SeatSelectionDialog : ContentDialog
    {
        public SeatSelectionViewModel ViewModel { get; set; }

        public SeatSelectionDialog()
        {
            this.InitializeComponent();
            ViewModel = new SeatSelectionViewModel();
            this.DataContext = ViewModel;
        }
    }
}

using Magidesk.Presentation.ViewModels.Dialogs;
using Microsoft.UI.Xaml.Controls;

namespace Magidesk.Views.Dialogs
{
    public sealed partial class MergeTicketsDialog : ContentDialog
    {
        public MergeTicketsViewModel ViewModel { get; set; }

        public MergeTicketsDialog()
        {
            this.InitializeComponent();
        }
    }
}

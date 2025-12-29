using Magidesk.Presentation.ViewModels;
using Microsoft.UI.Xaml.Controls;

namespace Magidesk.Views;

public sealed partial class QuantityDialog : ContentDialog
{
    public QuantityViewModel ViewModel { get; }

    public QuantityDialog()
    {
        this.InitializeComponent();
        ViewModel = new QuantityViewModel();
        this.DataContext = ViewModel;
    }
}

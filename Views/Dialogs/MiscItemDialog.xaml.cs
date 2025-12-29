using Magidesk.ViewModels;
using Microsoft.UI.Xaml.Controls;

namespace Magidesk.Views.Dialogs;

public sealed partial class MiscItemDialog : ContentDialog
{
    public MiscItemViewModel ViewModel { get; }

    public MiscItemDialog()
    {
        this.InitializeComponent();
        ViewModel = new MiscItemViewModel();
    }
}

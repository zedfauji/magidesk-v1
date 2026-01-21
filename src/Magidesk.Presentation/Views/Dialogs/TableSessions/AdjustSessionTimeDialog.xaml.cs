using Magidesk.Presentation.ViewModels.Dialogs.TableSessions;
using Microsoft.UI.Xaml.Controls;

namespace Magidesk.Presentation.Views.Dialogs.TableSessions;

public sealed partial class AdjustSessionTimeDialog : ContentDialog
{
    public AdjustSessionTimeDialogViewModel ViewModel => (AdjustSessionTimeDialogViewModel)DataContext;

    public AdjustSessionTimeDialog()
    {
        this.InitializeComponent();
    }
}

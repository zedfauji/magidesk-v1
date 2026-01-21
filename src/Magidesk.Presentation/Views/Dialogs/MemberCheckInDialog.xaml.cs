using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Magidesk.Presentation.ViewModels;
using System.Threading.Tasks;

namespace Magidesk.Presentation.Views.Dialogs;

public sealed partial class MemberCheckInDialog : ContentDialog
{
    public MemberCheckInViewModel ViewModel { get; }

    public MemberCheckInDialog()
    {
        this.InitializeComponent();
        ViewModel = App.Services.GetRequiredService<MemberCheckInViewModel>();
    }

    private void SearchTerm_KeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            if (ViewModel.SearchCommand.CanExecute(null))
            {
                ViewModel.SearchCommand.Execute(null);
            }
            e.Handled = true;
        }
    }
}

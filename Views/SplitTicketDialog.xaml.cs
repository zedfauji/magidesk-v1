using Magidesk.Presentation.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;

namespace Magidesk.Views;

public sealed partial class SplitTicketDialog : ContentDialog
{
    public SplitTicketViewModel ViewModel { get; }

    public SplitTicketDialog()
    {
        this.InitializeComponent();
        ViewModel = Magidesk.Presentation.App.Services.GetService<SplitTicketViewModel>();
        this.DataContext = ViewModel;
    }
}

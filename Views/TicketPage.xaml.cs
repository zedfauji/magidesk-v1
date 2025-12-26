using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Magidesk.Presentation.ViewModels;

namespace Magidesk.Presentation.Views;

public sealed partial class TicketPage : Page
{
    public TicketViewModel ViewModel { get; }

    public TicketPage()
    {
        InitializeComponent();
        ViewModel = App.Services.GetRequiredService<TicketViewModel>();
        DataContext = ViewModel;
    }
}

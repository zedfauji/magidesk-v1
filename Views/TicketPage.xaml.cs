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

    protected override void OnNavigatedTo(Microsoft.UI.Xaml.Navigation.NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        if (e.Parameter is Magidesk.Domain.Entities.OrderType orderType)
        {
            ViewModel.OrderTypeIdText = orderType.Id.ToString();
            // TODO: In future, we might auto-create the ticket here if context allows
        }
    }
}

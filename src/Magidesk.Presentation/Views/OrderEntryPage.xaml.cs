using Magidesk.Presentation.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace Magidesk.Presentation.Views;

public sealed partial class OrderEntryPage : Page
{
    public OrderEntryViewModel ViewModel { get; }

    public OrderEntryPage()
    {
        System.Diagnostics.Debug.WriteLine("OrderEntryPage constructor called");
        InitializeComponent();
        ViewModel = App.Services.GetRequiredService<OrderEntryViewModel>();
        DataContext = ViewModel;
        System.Diagnostics.Debug.WriteLine("OrderEntryPage constructor completed");
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        
        Guid? ticketId = null;
        
        if (e.Parameter is OrderEntryNavigationContext context)
        {
            ticketId = context.TicketId;
            ViewModel.SetNavigationContext(context);
        }
        else if (e.Parameter is Guid id)
        {
            ticketId = id;
        }

        await ViewModel.InitializeAsync(ticketId);
    }
}

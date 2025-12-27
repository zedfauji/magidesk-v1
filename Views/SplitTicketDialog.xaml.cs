using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Magidesk.Application.DTOs;
using Magidesk.Domain.ValueObjects;
using Magidesk.Presentation.ViewModels;

namespace Magidesk.Presentation.Views;

public sealed partial class SplitTicketDialog : ContentDialog
{
    private readonly TicketDto _originalTicket;

    public SplitTicketViewModel ViewModel { get; }

    public SplitTicketDialog(TicketDto originalTicket)
    {
        InitializeComponent();
        _originalTicket = originalTicket;
        ViewModel = App.Services.GetRequiredService<SplitTicketViewModel>();
        this.Loaded += SplitTicketDialog_Loaded;
        this.Closing += SplitTicketDialog_Closing;
    }

    private void SplitTicketDialog_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        ViewModel.Initialize(_originalTicket);
    }

    private async void SplitTicketDialog_Closing(ContentDialog sender, ContentDialogClosingEventArgs args)
    {
        if (args.Result == ContentDialogResult.Primary)
        {
            var deferral = args.GetDeferral();
            try
            {
                // Execute Split
                // TODO: Get actual user ID from context
                var userId = new UserId(Guid.Parse("00000000-0000-0000-0000-000000000001")); 
                var result = await ViewModel.ExecuteSplitAsync(userId);
                
                if (!result.Success)
                {
                    args.Cancel = true; 
                    // Consider showing error message to user here? 
                    // For now, ViewModel might have error property or we assume valid state if button enabled.
                }
            }
            finally
            {
                deferral.Complete();
            }
        }
    }
}

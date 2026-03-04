using System.Collections.ObjectModel;
using System.Threading;
using Magidesk.Application.DTOs;
using Microsoft.Extensions.DependencyInjection;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries;
using Magidesk.Application.Commands;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.ValueObjects;
using Magidesk.Presentation.Views;
using Magidesk.Presentation.Services;
using Magidesk.Presentation.Controls;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Linq;
using Magidesk.Application.Commands.TableSessions;
using Microsoft.UI.Xaml.Controls;

using System.Collections.ObjectModel;
using System.Threading;
using Magidesk.Application.DTOs;
using Microsoft.Extensions.DependencyInjection;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries;
using Magidesk.Application.Commands;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.ValueObjects;
using Magidesk.Presentation.Views;
using Magidesk.Presentation.Services;
using Magidesk.Presentation.Controls;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Linq;
using Magidesk.Application.Commands.TableSessions;
using Microsoft.UI.Xaml.Controls;

using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Enumerations;
using Magidesk.Presentation.Views.Dialogs;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;

namespace Magidesk.Presentation.ViewModels;

/// <summary>
/// Partial class for table selection and basic action handling.
/// Manages table clicks, session start/end, and basic table operations.
/// </summary>
public partial class TableMapViewModel
{
    private async Task SelectTableAsync(TableDto? table)
    {
        if (table == null) return;
        
        // Set as selected table for toolbar buttons
        SelectedTable = table;
        
        if (SourceTicketId.HasValue)
        {
            // F-0080: Move Table Logic
            if (table.Status != TableStatus.Available)
            {
                 // TODO: Show Error or Offer Merge
                 return;
            }
            
            IsBusy = true;
            try
            {
                var result = await _changeTable.HandleAsync(new ChangeTableCommand
                {
                    TicketId = SourceTicketId.Value,
                    NewTableId = table.Id,
                    UserId = new UserId(_userContextService.GetCurrentUserId())
                });

                if (result.Success)
                {
                     // Return to Ticket Page
                     _navigationService.Navigate(_orderPageNavigationHelper.GetOrderPageType(), new OrderEntryNavigationContext(SourceTicketId.Value, true));
                     
                     // Reset Context
                     SetContext(null);
                }
                else
                {
                    // Show error? For now just log/ignore
                }
            }
            finally
            {
                IsBusy = false;
            }
            
            return;
        }

        // F-0082: Normal Navigation Logic
        if (table.Status == TableStatus.Seat)
        {
             if (table.CurrentTicketId.HasValue)
             {
                 // Resume existing ticket
                 _navigationService.Navigate(_orderPageNavigationHelper.GetOrderPageType(), new OrderEntryNavigationContext(table.CurrentTicketId.Value, true));
             }
             else if (table.SessionId.HasValue)
             {
                 // No ticket, but has active session - Open Session Control Dialog
                 // This handles the "Session Only" case (e.g., pool table time tracking without F&B orders)
                 await OpenSessionControlDialogAsync(table);
             }
             else
             {
                 // Seat state but no ticket and no session? 
                 // This is an inconsistent state, but we should at least let the user inspect it or reset it.
                 // For now, treat it as Details request if possible, or log warning.
                 System.Diagnostics.Debug.WriteLine($"Table {table.TableNumber} is SEAT but has no Ticket or Session.");
                 // Fallback to table operations
                 await OpenTableOperationsDialogAsync(table);
             }
        }
        else if (table.Status == TableStatus.Available)
        {
             // Check for existing open tickets for this table
             try 
             {
                 IsBusy = true;
                 
                 if (_userContextService.GetCurrentUserId() == Guid.Empty) return;
                 
                 // Check if there's already an open ticket for this table
                 using (var scope = _serviceScopeFactory.CreateScope())
                 {
                     var ticketRepository = scope.ServiceProvider.GetRequiredService<ITicketRepository>();
                     var existingTicket = await ticketRepository.GetOpenTicketByTableNumberAsync(table.TableNumber);
                     
                     if (existingTicket != null)
                     {
                         // Show dialog informing user about existing ticket
                         var dialog = new Views.Dialogs.OpenTicketConfirmationDialog();
                         dialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot;
                         dialog.Initialize(table.TableNumber.ToString(), hasExistingTicket: true, existingTicketId: existingTicket.Id);
                         
                         var result = await dialog.ShowAsync();
                         
                         if (result == ContentDialogResult.Primary)
                         {
                             // Open existing ticket
                             _navigationService.Navigate(_orderPageNavigationHelper.GetOrderPageType(), new OrderEntryNavigationContext(existingTicket.Id, true));
                         }
                         // If Secondary or None, do nothing (cancel)
                         
                         return;
                     }
                 }
                 
                 // No existing ticket - show confirmation dialog
                 var confirmDialog = new Views.Dialogs.OpenTicketConfirmationDialog();
                 confirmDialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot;
                 confirmDialog.Initialize(table.TableNumber.ToString());
                 
                 var confirmResult = await confirmDialog.ShowAsync();
                 
                 if (confirmResult == ContentDialogResult.Primary)
                 {
                     // User confirmed - create new ticket
                     var ticketId = await _ticketCreationService.CreateTicketForTableAsync(table.Id, _userContextService.GetCurrentUserId());
                     
                     // Navigate with new Ticket ID
                     _navigationService.Navigate(_orderPageNavigationHelper.GetOrderPageType(), new OrderEntryNavigationContext(ticketId, true));
                 }
                 else if (confirmResult == ContentDialogResult.Secondary)
                 {
                     // User chose "No, Just View Table" - navigate to table page without creating ticket
                     // For now, we'll just stay on the table map
                     // In the future, you could navigate to a table details page
                     System.Diagnostics.Debug.WriteLine($"User chose to view table {table.TableNumber} without opening a ticket");
                 }
                 // If None (Cancel), do nothing
             }
             catch (Exception ex)
             {
                 // TODO: Show visual error
                 System.Diagnostics.Debug.WriteLine($"Failed to create ticket from map: {ex.Message}");
             }
             finally
             {
                 IsBusy = false;
             }
        }    
    }


    private async Task StartSessionAsync(TableDto? table)
    {
        if (table == null) return;
        await OpenStartSessionDialogAsync(table);
    }


    private async Task ViewDetailsAsync(TableDto? table)
    {
        if (table == null || !table.SessionId.HasValue) return;
        
        // Navigate to session details or show session control dialog
        await OpenSessionControlDialogAsync(table);
    }


    private async Task EndSessionAsync(TableDto? table)
    {
        if (table == null || !table.SessionId.HasValue) return;
        await OpenEndSessionDialogAsync(table);
    }

}

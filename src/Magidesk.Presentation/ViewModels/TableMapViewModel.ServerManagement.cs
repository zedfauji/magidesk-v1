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

using System.Collections.ObjectModel;
using Magidesk.Application.Commands;
using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Enumerations;
using Magidesk.Presentation.Controls;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using System.Windows.Input;

namespace Magidesk.Presentation.ViewModels;

/// <summary>
/// Partial class for server assignment and context menu operations.
/// Handles server assignments and dynamic menu creation.
/// </summary>
public partial class TableMapViewModel
{
    private async Task AssignServerAsync(ServerAssignmentEventArgs? args)
    {
        if (args == null || args.Table == null) return;

        try
        {
            // TODO: Implement server assignment logic
            // This would call a command handler to assign the server to the table/session
            
            System.Diagnostics.Debug.WriteLine($"Assigning server {args.ServerName} (ID: {args.ServerId}) to table {args.Table.TableNumber}");
            
            // For now, just show a success message
            // In a full implementation, this would:
            // 1. Call a command handler to update the table/session with the server assignment
            // 2. Refresh the table map to show the updated assignment
            // 3. Show a toast notification for success/failure
            
            await RefreshTablesAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error assigning server: {ex.Message}");
            // TODO: Show error to user via IDialogService or IToastNotificationService
        }
    }

    /// <summary>
    /// Generates context menu items based on table status
    /// </summary>
    /// <param name="table">The table to generate menu items for</param>
    /// <returns>Collection of menu flyout items</returns>
    public ObservableCollection<MenuFlyoutItemBase> GetContextMenuItems(TableDto table)
    {
        var items = new ObservableCollection<MenuFlyoutItemBase>();

        if (table == null) return items;

        // Available table actions
        if (table.Status == TableStatus.Available)
        {
            items.Add(CreateMenuFlyoutItem(
                "Start Session",
                Symbol.Play,
                StartSessionCommand,
                table
            ));
        }

        // Occupied table actions
        if (table.Status == TableStatus.Seat && table.SessionId.HasValue)
        {
            items.Add(CreateMenuFlyoutItem(
                "View Details",
                Symbol.View,
                ViewDetailsCommand,
                table
            ));

            items.Add(new MenuFlyoutSeparator());

            // Pause/Resume based on session status
            if (table.SessionStatus == TableSessionStatus.Active)
            {
                items.Add(CreateMenuFlyoutItem(
                    "Pause Session",
                    Symbol.Pause,
                    PauseSessionCommand,
                    table
                ));
            }
            else if (table.SessionStatus == TableSessionStatus.Paused)
            {
                items.Add(CreateMenuFlyoutItem(
                    "Resume Session",
                    Symbol.Play,
                    ResumeSessionCommand,
                    table
                ));
            }

            items.Add(new MenuFlyoutSeparator());

            items.Add(CreateMenuFlyoutItem(
                "End Session",
                Symbol.Stop,
                EndSessionCommand,
                table
            ));
        }

        return items;
    }

    private MenuFlyoutItem CreateMenuFlyoutItem(string text, Symbol icon, ICommand command, TableDto table)
    {
        var item = new MenuFlyoutItem
        {
            Text = text,
            Icon = new SymbolIcon(icon),
            Command = command,
            CommandParameter = table
        };
        return item;
    }

}

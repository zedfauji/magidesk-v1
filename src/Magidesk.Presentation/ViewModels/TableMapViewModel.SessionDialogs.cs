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

using Magidesk.Application.Commands.TableSessions;
using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.ValueObjects;
using Magidesk.Presentation.ViewModels.Dialogs;
using Magidesk.Presentation.Views.Dialogs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;

namespace Magidesk.Presentation.ViewModels;

/// <summary>
/// Partial class for session dialog operations.
/// Handles opening and managing session-related dialogs and controls.
/// </summary>
public partial class TableMapViewModel
{
    private async Task OpenStartSessionDialogAsync(TableDto? table)
    {
        if (table == null) return;

        try
        {
            // Resolve dialog ViewModel from DI
            var dialogViewModel = _serviceProvider.GetRequiredService<ViewModels.Dialogs.StartSessionDialogViewModel>();
            
            // Get table type information from the table or use default
            var tableTypeRepository = _serviceProvider.GetRequiredService<ITableTypeRepository>();
            
            // For now, use a default table type since TableDto doesn't have TableTypeId
            // In a future version, this should be retrieved from table configuration
            var defaultTableTypes = await tableTypeRepository.GetAllAsync();
            var tableType = defaultTableTypes.FirstOrDefault();
            
            Guid tableTypeId;
            string tableTypeName;
            decimal hourlyRate;
            
            if (tableType != null)
            {
                tableTypeId = tableType.Id;
                tableTypeName = tableType.Name;
                hourlyRate = tableType.HourlyRate;
            }
            else
            {
                // Fallback to default values if no table type found
                tableTypeId = Guid.Parse("00000000-0000-0000-0000-000000000001"); // Placeholder
                tableTypeName = "Standard";
                hourlyRate = 15.00m; // Default rate
            }
            
            // Get current shift
            var getCurrentShiftHandler = _serviceProvider.GetRequiredService<IQueryHandler<GetCurrentShiftQuery, GetCurrentShiftResult>>();
            var currentShiftResult = await getCurrentShiftHandler.HandleAsync(new GetCurrentShiftQuery());
            var currentShiftId = currentShiftResult.Shift?.Id;
            
            // Initialize dialog
            dialogViewModel.Initialize(
                table.Id, 
                tableTypeId, 
                $"Table {table.TableNumber}", 
                tableTypeName, 
                hourlyRate, 
                ticketId: null, // No ticket yet
                userId: _userContextService.GetCurrentUserId(),
                terminalId: _terminalContext.TerminalId,
                shiftId: currentShiftId,
                orderTypeId: Guid.Parse("00000000-0000-0000-0000-000000000001"), // DEFAULT
                createTicket: true);
            
            // Create and show dialog
            var dialog = new Views.Dialogs.StartSessionDialog(dialogViewModel);
            dialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot;
            
            // Handle dialog result
            dialogViewModel.SessionStarted += async (s, result) =>
            {
                // Refresh table map to show new session
                await RefreshTablesAsync();
            };
            
            await dialog.ShowAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error opening start session dialog: {ex.Message}");
            // TODO: Show error to user via IDialogService
        }
    }


    private async Task OpenEndSessionDialogAsync(TableDto? table)
    {
        if (table == null || !table.SessionId.HasValue) return;

        try
        {
            // Resolve dialog ViewModel from DI
            var dialogViewModel = _serviceProvider.GetRequiredService<ViewModels.Dialogs.EndSessionDialogViewModel>();
            
            // Calculate session duration and charge
            var duration = table.SessionElapsedTime ?? TimeSpan.Zero;
            var hourlyRate = table.SessionHourlyRate ?? 0m;
            var totalCharge = table.SessionRunningCharge ?? 0m;
            
            // Initialize dialog with session ID and calculated values
            dialogViewModel.Initialize(
                table.SessionId.Value, 
                duration, 
                hourlyRate, 
                totalCharge,
                userId: _userContextService.GetCurrentUserId(),
                terminalId: _terminalContext.TerminalId,
                hasExistingTicket: true);
            
            // Create and show dialog
            var dialog = new Views.Dialogs.EndSessionDialog(dialogViewModel);
            dialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot;
            
            // Handle dialog result
            dialogViewModel.SessionEnded += async (s, result) =>
            {
                // Refresh table map to clear session
                await RefreshTablesAsync();
            };
            
            await dialog.ShowAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error opening end session dialog: {ex.Message}");
            // TODO: Show error to user via IDialogService
        }
    }


    private async Task PauseSessionAsync(TableDto? table)
    {
        if (table == null || !table.SessionId.HasValue) return;

        try
        {
            var pauseHandler = _serviceProvider.GetRequiredService<ICommandHandler<PauseTableSessionCommand, PauseTableSessionResult>>();
            var command = new PauseTableSessionCommand(table.SessionId.Value);
            
            var result = await pauseHandler.HandleAsync(command);
            
            // Refresh the table map to show updated status
            await RefreshTablesAsync();
            
            System.Diagnostics.Debug.WriteLine($"Session paused for table {table.TableNumber} at {result.PausedAt}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error pausing session: {ex.Message}");
            
            var dialogService = _serviceProvider.GetRequiredService<IDialogService>();
            await dialogService.ShowMessageAsync("Error", $"Failed to pause session: {ex.Message}");
        }
    }


    private async Task ResumeSessionAsync(TableDto? table)
    {
        if (table == null || !table.SessionId.HasValue) return;

        try
        {
            var resumeHandler = _serviceProvider.GetRequiredService<ICommandHandler<ResumeTableSessionCommand, ResumeTableSessionResult>>();
            var command = new ResumeTableSessionCommand(table.SessionId.Value);
            
            var result = await resumeHandler.HandleAsync(command);
            
            // Refresh the table map to show updated status
            await RefreshTablesAsync();
            
            System.Diagnostics.Debug.WriteLine($"Session resumed for table {table.TableNumber} at {result.ResumedAt}. Total paused duration: {result.TotalPausedDuration}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error resuming session: {ex.Message}");
            
            var dialogService = _serviceProvider.GetRequiredService<IDialogService>();
            await dialogService.ShowMessageAsync("Error", $"Failed to resume session: {ex.Message}");
        }
    }


    private async Task PerformTimeAdjustmentAsync(TableDto? table)
    {
        if (table == null || !table.SessionId.HasValue) return;

        try
        {
            // Resolve dialog ViewModel from DI
            var dialogViewModel = new ViewModels.Dialogs.TableSessions.AdjustSessionTimeDialogViewModel(
                _serviceProvider.GetRequiredService<ICommandHandler<AdjustSessionTimeCommand, AdjustSessionTimeResult>>(),
                table.SessionId.Value
            );

            // Create and show dialog
            var dialog = new Views.Dialogs.TableSessions.AdjustSessionTimeDialog();
            dialog.DataContext = dialogViewModel;
            dialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot;

            await dialog.ShowAsync();
            
            // Refresh tables to show updated time
            await RefreshTablesAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error adjusting session time: {ex.Message}");
            var dialogService = _serviceProvider.GetRequiredService<IDialogService>();
            await dialogService.ShowMessageAsync("Error", $"Failed to open adjustment dialog: {ex.Message}");
        }
    }


    private async Task OpenSessionControlDialogAsync(TableDto? table)
    {
        if (table == null || !table.SessionId.HasValue) return;

        try
        {
            // Resolve dialog ViewModel from DI
            var dialogViewModel = _serviceProvider.GetRequiredService<ViewModels.Dialogs.SessionControlDialogViewModel>();
            
            // Initialize dialog with session information
            dialogViewModel.Initialize(
                table.SessionId.Value,
                $"Table {table.TableNumber}",
                table.SessionStatus ?? TableSessionStatus.Ended,
                4, // TODO: Get actual guest count from session data
                table.SessionElapsedTime ?? TimeSpan.Zero,
                table.SessionPausedDuration ?? TimeSpan.Zero,
                table.SessionRunningCharge ?? 0m
            );
            
            // Create and show dialog
            var dialog = new Views.Dialogs.SessionControlDialog(dialogViewModel);
            dialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot;
            
            // Handle dialog result
            dialogViewModel.SessionControlCompleted += async (s, result) =>
            {
                // Refresh table map to show updated session state
                await RefreshTablesAsync();
            };
            
            await dialog.ShowAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error opening session control dialog: {ex.Message}");
            // TODO: Show error to user via IDialogService
        }
    }

}

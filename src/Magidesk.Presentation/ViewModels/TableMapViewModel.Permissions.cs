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
using Magidesk.Presentation.ViewModels.Dialogs;
using Magidesk.Presentation.Views.Dialogs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;

namespace Magidesk.Presentation.ViewModels;

/// <summary>
/// Partial class for permissions and cleanup operations.
/// Handles manager overrides, permission checks, and resource disposal.
/// </summary>
public partial class TableMapViewModel
{
    private async Task CheckPermissionsAsync()
    {
        try 
        {
            if (_userContextService.GetCurrentUserId() == Guid.Empty) 
            {
                CanAdjustTime = false;
                return;
            }

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var securityService = scope.ServiceProvider.GetRequiredService<ISecurityService>();
                var userId = new UserId(_userContextService.GetCurrentUserId());
                CanAdjustTime = await securityService.HasPermissionAsync(userId, UserPermission.AdjustSessionTime);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error checking permissions: {ex.Message}");
            CanAdjustTime = false;
        }
    }


    private async Task OpenManagerOverrideDialogAsync(TableDto? table)
    {
        if (table == null || !table.SessionId.HasValue) return;

        try
        {
            // Show override type selection first
            var overrideTypeDialog = new ContentDialog
            {
                Title = "Select Override Type",
                PrimaryButtonText = "Continue",
                SecondaryButtonText = "Cancel",
                XamlRoot = App.MainWindowInstance.Content.XamlRoot
            };

            var overrideTypeSelection = new ComboBox
            {
                PlaceholderText = "Select override type...",
                HorizontalAlignment = Microsoft.UI.Xaml.HorizontalAlignment.Stretch,
                Margin = new Microsoft.UI.Xaml.Thickness(0, 8, 0, 0)
            };

            overrideTypeSelection.Items.Add("Time Adjustment");
            overrideTypeSelection.Items.Add("Pricing Override");
            overrideTypeSelection.Items.Add("Force End Session");

            var stackPanel = new StackPanel();
            stackPanel.Children.Add(new TextBlock { Text = "Select the type of manager override to perform:" });
            stackPanel.Children.Add(overrideTypeSelection);

            overrideTypeDialog.Content = stackPanel;

            var typeResult = await overrideTypeDialog.ShowAsync();
            if (typeResult != ContentDialogResult.Primary || overrideTypeSelection.SelectedItem == null)
                return;

            // Determine override type
            var overrideType = overrideTypeSelection.SelectedItem.ToString() switch
            {
                "Time Adjustment" => ViewModels.Dialogs.ManagerOverrideType.TimeAdjustment,
                "Pricing Override" => ViewModels.Dialogs.ManagerOverrideType.PricingOverride,
                "Force End Session" => ViewModels.Dialogs.ManagerOverrideType.ForceEnd,
                _ => ViewModels.Dialogs.ManagerOverrideType.TimeAdjustment
            };

            // Resolve dialog ViewModel from DI
            var dialogViewModel = _serviceProvider.GetRequiredService<ViewModels.Dialogs.ManagerOverrideDialogViewModel>();
            
            // Initialize dialog with session and override information
            dialogViewModel.Initialize(
                table.SessionId.Value,
                $"Table {table.TableNumber}",
                overrideType,
                table.SessionElapsedTime ?? TimeSpan.Zero,
                table.SessionRunningCharge ?? 0m
            );
            
            // Create and show dialog
            var dialog = new Views.Dialogs.ManagerOverrideDialog(dialogViewModel);
            dialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot;
            
            // Handle dialog result
            dialogViewModel.OverrideCompleted += async (s, result) =>
            {
                // Refresh table map to show updated session state
                await RefreshTablesAsync();
            };
            
            await dialog.ShowAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error opening manager override dialog: {ex.Message}");
            // TODO: Show error to user via IDialogService
        }
    }


    public void Dispose()
    {
        _cancellationTokenSource.Cancel();
        StopRealTimePolling();
        StopUIRefreshTimer();
        _cancellationTokenSource.Dispose();
    }
}

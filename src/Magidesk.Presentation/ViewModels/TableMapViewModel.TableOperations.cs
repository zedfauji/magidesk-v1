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
using Magidesk.Presentation.ViewModels.Dialogs;
using Magidesk.Presentation.Views.Dialogs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;

namespace Magidesk.Presentation.ViewModels;

/// <summary>
/// Partial class for table operations dialog.
/// Handles merge, split, and transfer operations.
/// </summary>
public partial class TableMapViewModel
{
    private async Task OpenTableOperationsDialogAsync(TableDto? table)
    {
        if (table == null) return;

        try
        {
            // Show operation type selection first
            var operationTypeDialog = new ContentDialog
            {
                Title = "Select Table Operation",
                PrimaryButtonText = "Continue",
                SecondaryButtonText = "Cancel",
                XamlRoot = App.MainWindowInstance.Content.XamlRoot
            };

            var operationTypeSelection = new ComboBox
            {
                PlaceholderText = "Select operation type...",
                HorizontalAlignment = Microsoft.UI.Xaml.HorizontalAlignment.Stretch,
                Margin = new Microsoft.UI.Xaml.Thickness(0, 8, 0, 0)
            };

            operationTypeSelection.Items.Add("Merge Tables");
            operationTypeSelection.Items.Add("Split Tables");
            if (table.SessionId.HasValue)
            {
                operationTypeSelection.Items.Add("Transfer Session");
            }

            var stackPanel = new StackPanel();
            stackPanel.Children.Add(new TextBlock { Text = "Select the table operation to perform:" });
            stackPanel.Children.Add(operationTypeSelection);

            operationTypeDialog.Content = stackPanel;

            var typeResult = await operationTypeDialog.ShowAsync();
            if (typeResult != ContentDialogResult.Primary || operationTypeSelection.SelectedItem == null)
                return;

            // Determine operation type
            var operationType = operationTypeSelection.SelectedItem.ToString() switch
            {
                "Merge Tables" => ViewModels.Dialogs.TableOperationType.Merge,
                "Split Tables" => ViewModels.Dialogs.TableOperationType.Split,
                "Transfer Session" => ViewModels.Dialogs.TableOperationType.Transfer,
                _ => ViewModels.Dialogs.TableOperationType.Merge
            };

            // Resolve dialog ViewModel from DI
            var dialogViewModel = _serviceProvider.GetRequiredService<ViewModels.Dialogs.TableOperationsDialogViewModel>();
            
            // Initialize dialog with table and operation information
            await dialogViewModel.InitializeAsync(
                operationType,
                table.Id,
                $"Table {table.TableNumber}",
                table.SessionId,
                table.SessionRunningCharge ?? 0m,
                table.SessionElapsedTime ?? TimeSpan.Zero,
                4 // TODO: Get actual guest count from session data
            );
            
            // Create and show dialog
            var dialog = new Views.Dialogs.TableOperationsDialog(dialogViewModel);
            dialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot;
            
            // Handle dialog result
            dialogViewModel.OperationCompleted += async (s, result) =>
            {
                // Refresh table map to show updated table states
                await RefreshTablesAsync();
            };
            
            await dialog.ShowAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error opening table operations dialog: {ex.Message}");
            // TODO: Show error to user via IDialogService
        }
    }

}

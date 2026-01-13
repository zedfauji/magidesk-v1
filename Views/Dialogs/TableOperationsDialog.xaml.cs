using Microsoft.UI.Xaml.Controls;
using Magidesk.Presentation.ViewModels.Dialogs;

namespace Magidesk.Presentation.Views.Dialogs;

/// <summary>
/// Dialog for table operations including merge, split, and transfer functionality.
/// </summary>
public sealed partial class TableOperationsDialog : ContentDialog
{
    public TableOperationsDialogViewModel ViewModel { get; }

    public TableOperationsDialog(TableOperationsDialogViewModel viewModel)
    {
        ViewModel = viewModel;
        this.InitializeComponent();
        this.DataContext = viewModel;

        // Subscribe to ViewModel events
        ViewModel.RequestClose += OnRequestClose;
        ViewModel.OperationCompleted += OnOperationCompleted;
        this.Closed += OnDialogClosed;
    }

    private void OnRequestClose(object? sender, System.EventArgs e)
    {
        this.Hide();
    }

    private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        foreach (var item in e.AddedItems)
        {
            if (item is Magidesk.Application.DTOs.TableDto table && !ViewModel.SelectedTables.Contains(table))
            {
                ViewModel.SelectedTables.Add(table);
            }
        }

        foreach (var item in e.RemovedItems)
        {
            if (item is Magidesk.Application.DTOs.TableDto table)
            {
                ViewModel.SelectedTables.Remove(table);
            }
        }
    }

    private void OnOperationCompleted(object? sender, TableOperationEventArgs e)
    {
        // Operation completed successfully, dialog will close via RequestClose event
    }

    private void OnDialogClosed(ContentDialog sender, ContentDialogClosedEventArgs args)
    {
        ViewModel.RequestClose -= OnRequestClose;
        ViewModel.OperationCompleted -= OnOperationCompleted;
    }
}
using Microsoft.UI.Xaml.Controls;
using Magidesk.Presentation.ViewModels.Dialogs;

namespace Magidesk.Presentation.Views.Dialogs;

/// <summary>
/// Dialog for manager override operations including time adjustments, pricing overrides, and force session end.
/// </summary>
public sealed partial class ManagerOverrideDialog : ContentDialog
{
    public ManagerOverrideDialogViewModel ViewModel { get; }

    public ManagerOverrideDialog(ManagerOverrideDialogViewModel viewModel)
    {
        ViewModel = viewModel;
        this.InitializeComponent();
        this.DataContext = viewModel;

        // Subscribe to ViewModel events
        ViewModel.RequestClose += OnRequestClose;
        ViewModel.OverrideCompleted += OnOverrideCompleted;

        // Focus the PIN box when dialog opens
        this.Loaded += (s, e) => ManagerPinBox.Focus(Microsoft.UI.Xaml.FocusState.Programmatic);
        this.Closed += OnDialogClosed;
    }

    private void OnRequestClose(object? sender, System.EventArgs e)
    {
        this.Hide();
    }

    private void OnOverrideCompleted(object? sender, ManagerOverrideEventArgs e)
    {
        // Override completed successfully, dialog will close via RequestClose event
    }

    private void OnDialogClosed(ContentDialog sender, ContentDialogClosedEventArgs args)
    {
        // Unsubscribe from events to prevent memory leaks
        ViewModel.RequestClose -= OnRequestClose;
        ViewModel.OverrideCompleted -= OnOverrideCompleted;
    }
}
using Microsoft.UI.Xaml.Controls;
using Magidesk.Presentation.ViewModels.Dialogs;

namespace Magidesk.Presentation.Views.Dialogs;

/// <summary>
/// Dialog for session control operations including pause/resume and guest count updates.
/// </summary>
public sealed partial class SessionControlDialog : ContentDialog
{
    public SessionControlDialogViewModel ViewModel { get; }

    public SessionControlDialog(SessionControlDialogViewModel viewModel)
    {
        ViewModel = viewModel;
        this.InitializeComponent();
        this.DataContext = viewModel;

        // Subscribe to ViewModel events
        ViewModel.RequestClose += OnRequestClose;
        ViewModel.SessionControlCompleted += OnSessionControlCompleted;
        this.Closed += OnDialogClosed;
    }

    private void OnRequestClose(object? sender, System.EventArgs e)
    {
        this.Hide();
    }

    private void OnSessionControlCompleted(object? sender, SessionControlEventArgs e)
    {
        // Session control operation completed successfully, dialog will close via RequestClose event
    }

    private void OnDialogClosed(ContentDialog sender, ContentDialogClosedEventArgs args)
    {
        // Unsubscribe from events to prevent memory leaks
        ViewModel.RequestClose -= OnRequestClose;
        ViewModel.SessionControlCompleted -= OnSessionControlCompleted;
    }
}
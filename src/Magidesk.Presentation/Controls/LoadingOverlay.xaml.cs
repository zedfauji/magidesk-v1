using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Windows.Input;

namespace Magidesk.Presentation.Controls;

/// <summary>
/// A loading overlay control that displays a semi-transparent backdrop with a progress indicator.
/// Used to indicate asynchronous operations and prevent user interaction during processing.
/// </summary>
public sealed partial class LoadingOverlay : UserControl
{
    // Dependency Properties
    public static readonly DependencyProperty IsLoadingProperty =
        DependencyProperty.Register(
            nameof(IsLoading),
            typeof(bool),
            typeof(LoadingOverlay),
            new PropertyMetadata(false, OnIsLoadingChanged));

    public static readonly DependencyProperty LoadingMessageProperty =
        DependencyProperty.Register(
            nameof(LoadingMessage),
            typeof(string),
            typeof(LoadingOverlay),
            new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty IsCancellableProperty =
        DependencyProperty.Register(
            nameof(IsCancellable),
            typeof(bool),
            typeof(LoadingOverlay),
            new PropertyMetadata(false));

    public static readonly DependencyProperty CancelCommandProperty =
        DependencyProperty.Register(
            nameof(CancelCommand),
            typeof(ICommand),
            typeof(LoadingOverlay),
            new PropertyMetadata(null));

    /// <summary>
    /// Gets or sets whether the loading overlay is currently displayed.
    /// </summary>
    public bool IsLoading
    {
        get => (bool)GetValue(IsLoadingProperty);
        set => SetValue(IsLoadingProperty, value);
    }

    /// <summary>
    /// Gets or sets the descriptive message displayed during loading.
    /// </summary>
    public string LoadingMessage
    {
        get => (string)GetValue(LoadingMessageProperty);
        set => SetValue(LoadingMessageProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the loading operation can be cancelled.
    /// </summary>
    public bool IsCancellable
    {
        get => (bool)GetValue(IsCancellableProperty);
        set => SetValue(IsCancellableProperty, value);
    }

    /// <summary>
    /// Gets or sets the command to execute when the Cancel button is clicked.
    /// </summary>
    public ICommand? CancelCommand
    {
        get => (ICommand?)GetValue(CancelCommandProperty);
        set => SetValue(CancelCommandProperty, value);
    }

    public LoadingOverlay()
    {
        this.InitializeComponent();
    }

    private static void OnIsLoadingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is LoadingOverlay control)
        {
            // When loading state changes, update the control's hit test visibility
            // This ensures that when IsLoading is true, the overlay blocks all interaction
            var isLoading = (bool)e.NewValue;
            control.IsHitTestVisible = isLoading;
        }
    }
}

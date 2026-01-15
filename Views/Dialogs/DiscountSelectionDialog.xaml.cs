using Microsoft.UI.Xaml.Controls;
using Magidesk.Presentation.ViewModels.Dialogs;

namespace Magidesk.Presentation.Views.Dialogs;

/// <summary>
/// Dialog for selecting and applying discounts to tickets.
/// Task 2.1.14: Discount selection UI with quick discount buttons and manager PIN integration.
/// </summary>
public sealed partial class DiscountSelectionDialog : ContentDialog
{
    public DiscountSelectionViewModel ViewModel { get; }

    public DiscountSelectionDialog(DiscountSelectionViewModel viewModel)
    {
        this.InitializeComponent();
        ViewModel = viewModel;
        DataContext = ViewModel;
        
        // Subscribe to property changes to show/hide error InfoBar
        ViewModel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(ViewModel.ErrorMessage))
            {
                ErrorInfoBar.IsOpen = !string.IsNullOrEmpty(ViewModel.ErrorMessage);
            }
        };
    }

    /// <summary>
    /// Loads available discounts when dialog is shown.
    /// </summary>
    private async void OnLoaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        await ViewModel.LoadDiscountsAsync();
    }

    /// <summary>
    /// Handles the Primary button click to apply the discount asynchronously.
    /// </summary>
    private async void OnPrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        // Get a deferral to keep the dialog open while we process the async operation
        var deferral = args.GetDeferral();
        
        try
        {
            // Apply the discount and wait for completion
            var success = await ViewModel.ApplyDiscountAsync();
            
            // If the operation failed, cancel the dialog close
            if (!success)
            {
                args.Cancel = true;
            }
        }
        finally
        {
            // Complete the deferral to allow the dialog to close
            deferral.Complete();
        }
    }

    /// <summary>
    /// Gets the result of the discount application.
    /// </summary>
    public bool IsSuccess => ViewModel.IsSuccess;
}

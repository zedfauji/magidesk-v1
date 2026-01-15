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
    }

    /// <summary>
    /// Loads available discounts when dialog is shown.
    /// </summary>
    private async void OnLoaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        await ViewModel.LoadDiscountsAsync();
    }

    /// <summary>
    /// Gets the result of the discount application.
    /// </summary>
    public bool IsSuccess => ViewModel.IsSuccess;
}

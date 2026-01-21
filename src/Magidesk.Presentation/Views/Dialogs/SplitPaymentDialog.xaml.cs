using Microsoft.UI.Xaml.Controls;
using Magidesk.Presentation.ViewModels.Dialogs;

namespace Magidesk.Presentation.Views.Dialogs;

/// <summary>
/// Dialog for processing split payments across multiple payment methods.
/// </summary>
public sealed partial class SplitPaymentDialog : ContentDialog
{
    public SplitPaymentViewModel ViewModel { get; }

    public SplitPaymentDialog(SplitPaymentViewModel viewModel)
    {
        this.InitializeComponent();
        ViewModel = viewModel;
        DataContext = ViewModel;
    }

    /// <summary>
    /// Gets the result of the split payment operation.
    /// </summary>
    public bool IsSuccess => ViewModel.IsSuccess;

    /// <summary>
    /// Gets the change amount if there was an overpayment.
    /// </summary>
    public Domain.ValueObjects.Money ChangeAmount => ViewModel.ChangeAmount;
}

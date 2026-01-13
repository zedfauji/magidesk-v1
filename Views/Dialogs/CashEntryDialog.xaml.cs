using Microsoft.UI.Xaml.Controls;
using Microsoft.Extensions.DependencyInjection;
using Magidesk.Presentation.ViewModels.Dialogs;
using System.Threading.Tasks;
using Windows.Globalization.NumberFormatting;

namespace Magidesk.Presentation.Views.Dialogs;

/// <summary>
/// Cash entry dialog with denomination breakdown for cash drops, payouts, and reconciliation.
/// </summary>
public sealed partial class CashEntryDialog : ContentDialog
{
    public CashEntryDialogViewModel ViewModel { get; }
    public DecimalFormatter CurrencyFormatter { get; }

    public CashEntryDialog()
    {
        InitializeComponent();
        ViewModel = App.Services.GetRequiredService<CashEntryDialogViewModel>();
        DataContext = ViewModel;

        // Create currency formatter
        CurrencyFormatter = new DecimalFormatter();
        CurrencyFormatter.IntegerDigits = 1;
        CurrencyFormatter.FractionDigits = 2;
        CurrencyFormatter.NumberRounder = new IncrementNumberRounder();
        ((IncrementNumberRounder)CurrencyFormatter.NumberRounder).Increment = 0.01;

        // Handle primary button click
        PrimaryButtonClick += OnPrimaryButtonClick;
    }

    /// <summary>
    /// Shows the cash entry dialog with the specified parameters.
    /// </summary>
    /// <param name="title">Dialog title</param>
    /// <param name="message">Main message</param>
    /// <param name="showDenominationBreakdown">Whether to show denomination breakdown</param>
    /// <param name="requireReason">Whether reason is required</param>
    /// <returns>Dialog result and entered data</returns>
    public async Task<(ContentDialogResult Result, decimal Amount, string Reason)> ShowCashEntryAsync(
        string title, 
        string message, 
        bool showDenominationBreakdown = true, 
        bool requireReason = true)
    {
        ViewModel.Initialize(title, message, showDenominationBreakdown, requireReason);
        
        var result = await ShowAsync();
        
        return (result, ViewModel.TotalAmount, ViewModel.Reason);
    }

    /// <summary>
    /// Shows the cash entry dialog pre-populated with an amount.
    /// </summary>
    /// <param name="title">Dialog title</param>
    /// <param name="message">Main message</param>
    /// <param name="initialAmount">Initial amount to populate</param>
    /// <param name="showDenominationBreakdown">Whether to show denomination breakdown</param>
    /// <param name="requireReason">Whether reason is required</param>
    /// <returns>Dialog result and entered data</returns>
    public async Task<(ContentDialogResult Result, decimal Amount, string Reason)> ShowCashEntryAsync(
        string title, 
        string message, 
        decimal initialAmount,
        bool showDenominationBreakdown = true, 
        bool requireReason = true)
    {
        ViewModel.Initialize(title, message, showDenominationBreakdown, requireReason);
        ViewModel.SetAmount(initialAmount);
        
        var result = await ShowAsync();
        
        return (result, ViewModel.TotalAmount, ViewModel.Reason);
    }

    private void OnPrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        // Validate input before allowing dialog to close
        if (!ViewModel.ValidateInput())
        {
            args.Cancel = true;
        }
    }
}
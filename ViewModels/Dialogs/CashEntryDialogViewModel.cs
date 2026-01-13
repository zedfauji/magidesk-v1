using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Magidesk.Presentation.ViewModels.Dialogs;

/// <summary>
/// ViewModel for cash entry dialog with denomination breakdown.
/// Used for cash drops, payouts, and reconciliation.
/// </summary>
public partial class CashEntryDialogViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _title = "Cash Entry";

    [ObservableProperty]
    private string _message = "Enter cash amount:";

    [ObservableProperty]
    private string _reason = string.Empty;

    [ObservableProperty]
    private decimal _totalAmount = 0m;

    [ObservableProperty]
    private bool _showDenominationBreakdown = true;

    [ObservableProperty]
    private bool _requireReason = true;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private bool _hasError = false;

    public ObservableCollection<CashDenominationViewModel> Denominations { get; }

    public ICommand CalculateTotalCommand { get; }
    public ICommand ClearAllCommand { get; }

    public CashEntryDialogViewModel()
    {
        Denominations = new ObservableCollection<CashDenominationViewModel>
        {
            new CashDenominationViewModel { Value = 100m, Name = "$100 Bills", Count = 0 },
            new CashDenominationViewModel { Value = 50m, Name = "$50 Bills", Count = 0 },
            new CashDenominationViewModel { Value = 20m, Name = "$20 Bills", Count = 0 },
            new CashDenominationViewModel { Value = 10m, Name = "$10 Bills", Count = 0 },
            new CashDenominationViewModel { Value = 5m, Name = "$5 Bills", Count = 0 },
            new CashDenominationViewModel { Value = 1m, Name = "$1 Bills", Count = 0 },
            new CashDenominationViewModel { Value = 0.25m, Name = "Quarters", Count = 0 },
            new CashDenominationViewModel { Value = 0.10m, Name = "Dimes", Count = 0 },
            new CashDenominationViewModel { Value = 0.05m, Name = "Nickels", Count = 0 },
            new CashDenominationViewModel { Value = 0.01m, Name = "Pennies", Count = 0 }
        };

        // Subscribe to count changes
        foreach (var denomination in Denominations)
        {
            denomination.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(CashDenominationViewModel.Count))
                {
                    CalculateTotal();
                }
            };
        }

        CalculateTotalCommand = new RelayCommand(CalculateTotal);
        ClearAllCommand = new RelayCommand(ClearAll);
    }

    public void Initialize(string title, string message, bool showDenominationBreakdown = true, bool requireReason = true)
    {
        Title = title;
        Message = message;
        ShowDenominationBreakdown = showDenominationBreakdown;
        RequireReason = requireReason;
        
        // Reset values
        Reason = string.Empty;
        TotalAmount = 0m;
        ErrorMessage = string.Empty;
        HasError = false;
        
        ClearAll();
    }

    private void CalculateTotal()
    {
        TotalAmount = Denominations.Sum(d => d.Value * d.Count);
    }

    private void ClearAll()
    {
        foreach (var denomination in Denominations)
        {
            denomination.Count = 0;
        }
        CalculateTotal();
    }

    public bool ValidateInput()
    {
        HasError = false;
        ErrorMessage = string.Empty;

        if (TotalAmount <= 0)
        {
            ErrorMessage = "Amount must be greater than zero.";
            HasError = true;
            return false;
        }

        if (RequireReason && string.IsNullOrWhiteSpace(Reason))
        {
            ErrorMessage = "Reason is required.";
            HasError = true;
            return false;
        }

        return true;
    }

    public void SetAmount(decimal amount)
    {
        TotalAmount = amount;
        
        // If not showing denomination breakdown, just set the total
        if (!ShowDenominationBreakdown)
        {
            return;
        }

        // Auto-calculate denomination breakdown for the amount
        ClearAll();
        
        var remainingAmount = amount;
        foreach (var denomination in Denominations.OrderByDescending(d => d.Value))
        {
            if (remainingAmount >= denomination.Value)
            {
                denomination.Count = (int)(remainingAmount / denomination.Value);
                remainingAmount -= denomination.Count * denomination.Value;
                remainingAmount = Math.Round(remainingAmount, 2); // Handle floating point precision
            }
        }
    }
}

/// <summary>
/// ViewModel for individual cash denomination entry.
/// </summary>
public partial class CashDenominationViewModel : ObservableObject
{
    [ObservableProperty]
    private decimal _value;

    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private int _count;

    public decimal Total => Value * Count;

    partial void OnCountChanged(int value)
    {
        OnPropertyChanged(nameof(Total));
    }
}
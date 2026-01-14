using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Magidesk.Application.Commands;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Presentation.ViewModels.Dialogs;

/// <summary>
/// ViewModel for Split Payment dialog.
/// Handles multiple payment entries and validates that the sum equals the ticket total.
/// </summary>
public partial class SplitPaymentViewModel : ViewModelBase
{
    private readonly ICommandHandler<ProcessSplitPaymentCommand, ProcessSplitPaymentResult> _processSplitPaymentHandler;
    private readonly IUserService _userService;

    [ObservableProperty]
    private Money _ticketTotal = Money.Zero();

    [ObservableProperty]
    private Money _totalEntered = Money.Zero();

    [ObservableProperty]
    private Money _remainingAmount = Money.Zero();

    [ObservableProperty]
    private Money _changeAmount = Money.Zero();

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private bool _isSuccess = false;

    /// <summary>
    /// Ticket ID to process split payment for.
    /// </summary>
    public Guid TicketId { get; set; }

    /// <summary>
    /// Collection of payment entries.
    /// </summary>
    public ObservableCollection<PaymentEntryViewModel> Payments { get; } = new();

    /// <summary>
    /// Available payment methods.
    /// </summary>
    public PaymentType[] AvailablePaymentMethods { get; } = new[]
    {
        PaymentType.Cash,
        PaymentType.CreditCard,
        PaymentType.DebitCard
    };

    public SplitPaymentViewModel(
        ICommandHandler<ProcessSplitPaymentCommand, ProcessSplitPaymentResult> processSplitPaymentHandler,
        IUserService userService)
    {
        _processSplitPaymentHandler = processSplitPaymentHandler;
        _userService = userService;
    }

    /// <summary>
    /// Can process payment when remaining amount is zero or negative (overpayment).
    /// </summary>
    public bool CanProcessPayment => RemainingAmount <= Money.Zero() && Payments.Count > 0;

    /// <summary>
    /// Adds a new payment entry.
    /// </summary>
    [RelayCommand]
    public void AddPayment()
    {
        var entry = new PaymentEntryViewModel
        {
            Method = PaymentType.Cash,
            Amount = RemainingAmount > Money.Zero() ? RemainingAmount : Money.Zero()
        };

        entry.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(PaymentEntryViewModel.Amount))
            {
                RecalculateTotals();
            }
        };

        Payments.Add(entry);
        RecalculateTotals();
    }

    /// <summary>
    /// Removes a payment entry.
    /// </summary>
    [RelayCommand]
    public void RemovePayment(PaymentEntryViewModel entry)
    {
        if (entry != null && Payments.Contains(entry))
        {
            Payments.Remove(entry);
            RecalculateTotals();
        }
    }

    /// <summary>
    /// Splits the ticket total equally among N payers.
    /// </summary>
    [RelayCommand]
    public void QuickSplit(int numberOfPayers)
    {
        if (numberOfPayers < 2)
        {
            return;
        }

        // Clear existing payments
        Payments.Clear();

        // Calculate split amount
        var splitAmount = new Money(TicketTotal.Amount / numberOfPayers);
        var remainder = TicketTotal - (splitAmount * numberOfPayers);

        // Add payment entries
        for (int i = 0; i < numberOfPayers; i++)
        {
            var amount = splitAmount;
            
            // Add remainder to the last payment to ensure exact total
            if (i == numberOfPayers - 1 && remainder > Money.Zero())
            {
                amount = amount + remainder;
            }

            var entry = new PaymentEntryViewModel
            {
                Method = PaymentType.Cash,
                Amount = amount
            };

            entry.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(PaymentEntryViewModel.Amount))
                {
                    RecalculateTotals();
                }
            };

            Payments.Add(entry);
        }

        RecalculateTotals();
    }

    /// <summary>
    /// Processes the split payment.
    /// </summary>
    [RelayCommand]
    public async Task<bool> ProcessSplitPaymentAsync()
    {
        if (Payments.Count == 0)
        {
            ErrorMessage = "Please add at least one payment entry.";
            return false;
        }

        var currentUser = _userService.CurrentUser;
        if (currentUser == null)
        {
            ErrorMessage = "No user logged in.";
            return false;
        }

        IsBusy = true;
        ErrorMessage = string.Empty;

        try
        {
            var paymentEntries = Payments
                .Select(p => new SplitPaymentEntry(p.Method, p.Amount))
                .ToList();

            var command = new ProcessSplitPaymentCommand(
                TicketId,
                paymentEntries,
                new UserId(currentUser.Id)
            );

            var result = await _processSplitPaymentHandler.HandleAsync(command);

            if (result.IsUnderpayment)
            {
                ErrorMessage = $"Underpayment: Remaining amount is {result.RemainingAmount}";
                return false;
            }

            ChangeAmount = result.ChangeAmount;
            IsSuccess = true;
            return true;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to process split payment: {ex.Message}";
            return false;
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Recalculates total entered and remaining amount.
    /// </summary>
    private void RecalculateTotals()
    {
        TotalEntered = Payments.Aggregate(Money.Zero(), (sum, p) => sum + p.Amount);
        RemainingAmount = TicketTotal - TotalEntered;
        
        OnPropertyChanged(nameof(CanProcessPayment));
    }

    /// <summary>
    /// Initializes the dialog with ticket total.
    /// </summary>
    public void Initialize(Guid ticketId, Money ticketTotal)
    {
        TicketId = ticketId;
        TicketTotal = ticketTotal;
        
        Payments.Clear();
        ErrorMessage = string.Empty;
        IsSuccess = false;
        ChangeAmount = Money.Zero();
        
        RecalculateTotals();
    }

    /// <summary>
    /// Resets the dialog state.
    /// </summary>
    public void Reset()
    {
        TicketId = Guid.Empty;
        TicketTotal = Money.Zero();
        Payments.Clear();
        ErrorMessage = string.Empty;
        IsSuccess = false;
        ChangeAmount = Money.Zero();
        
        RecalculateTotals();
    }
}

/// <summary>
/// Represents a single payment entry in the split payment UI.
/// </summary>
public partial class PaymentEntryViewModel : ObservableObject
{
    [ObservableProperty]
    private PaymentType _method = PaymentType.Cash;

    [ObservableProperty]
    private Money _amount = Money.Zero();
}

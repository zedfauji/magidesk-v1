using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Magidesk.Application.Commands;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Presentation.ViewModels.Dialogs;

/// <summary>
/// ViewModel for Discount Selection dialog.
/// Task 2.1.13: Handles discount selection, authorization checking, and application.
/// </summary>
public partial class DiscountSelectionViewModel : ViewModelBase
{
    private readonly IDiscountRepository _discountRepository;
    private readonly ICommandHandler<ApplyDiscountCommand> _applyDiscountHandler;
    private readonly IUserService _userService;
    private readonly ManagerPinDialogViewModel _managerPinDialog;

    [ObservableProperty]
    private ObservableCollection<Discount> _availableDiscounts = new();

    [ObservableProperty]
    private Discount? _selectedDiscount;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private bool _isSuccess = false;

    [ObservableProperty]
    private bool _isLoadingDiscounts = false;

    /// <summary>
    /// Ticket ID to apply discount to.
    /// </summary>
    public Guid TicketId { get; set; }

    /// <summary>
    /// Ticket total amount (used to check if authorization required).
    /// </summary>
    public Money? TicketTotal { get; set; }

    public DiscountSelectionViewModel(
        IDiscountRepository discountRepository,
        ICommandHandler<ApplyDiscountCommand> applyDiscountHandler,
        IUserService userService,
        ManagerPinDialogViewModel managerPinDialog)
    {
        _discountRepository = discountRepository;
        _applyDiscountHandler = applyDiscountHandler;
        _userService = userService;
        _managerPinDialog = managerPinDialog;
    }

    /// <summary>
    /// Can apply discount if one is selected.
    /// </summary>
    public bool CanApplyDiscount => SelectedDiscount != null && !IsBusy;

    /// <summary>
    /// Loads available active discounts.
    /// </summary>
    [RelayCommand]
    public async Task LoadDiscountsAsync()
    {
        IsLoadingDiscounts = true;
        ErrorMessage = string.Empty;

        try
        {
            var discounts = await _discountRepository.GetActiveDiscountsAsync();
            AvailableDiscounts = new ObservableCollection<Discount>(discounts);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to load discounts: {ex.Message}";
        }
        finally
        {
            IsLoadingDiscounts = false;
        }
    }

    /// <summary>
    /// Applies the selected discount to the ticket.
    /// Prompts for manager authorization if required (discount > 50% of total).
    /// </summary>
    [RelayCommand]
    public async Task<bool> ApplyDiscountAsync()
    {
        if (SelectedDiscount == null)
        {
            ErrorMessage = "Please select a discount.";
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
            // Check if authorization is required
            UserId? authorizedBy = null;
            
            if (RequiresAuthorization(SelectedDiscount))
            {
                // Prompt for manager PIN
                _managerPinDialog.OperationType = "Apply Large Discount";
                _managerPinDialog.Reset();
                
                var authResult = await _managerPinDialog.AuthorizeAsync();
                
                if (authResult == null || !authResult.Authorized)
                {
                    ErrorMessage = "Manager authorization required for this discount.";
                    return false;
                }
                
                // Use AuthorizingUserId instead of ManagerId
                if (authResult.AuthorizingUserId.HasValue)
                {
                    authorizedBy = new UserId(authResult.AuthorizingUserId.Value);
                }
            }

            // Apply the discount
            // Note: We use the injected handler which comes from the same scope as this ViewModel
            // The scope was created fresh in SettleViewModel.OnApplyDiscountAsync, ensuring
            // a fresh DbContext with no stale tracked entities
            var command = new ApplyDiscountCommand
            {
                TicketId = TicketId,
                DiscountId = SelectedDiscount.Id,
                AppliedBy = new UserId(currentUser.Id),
                AuthorizedBy = authorizedBy
            };

            await _applyDiscountHandler.HandleAsync(command);

            IsSuccess = true;
            return true;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to apply discount: {ex.Message}";
            return false;
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Checks if the discount requires manager authorization.
    /// Authorization required if:
    /// 1. Discount has RequiresAuthorization flag set, OR
    /// 2. Discount amount exceeds 50% of ticket total
    /// </summary>
    private bool RequiresAuthorization(Discount discount)
    {
        // Check explicit flag
        if (discount.RequiresAuthorization)
        {
            return true;
        }

        // Check if discount exceeds 50% of total
        if (TicketTotal != null && TicketTotal.Amount > 0)
        {
            var discountAmount = discount.CalculateDiscount(TicketTotal);
            var discountPercentage = (discountAmount.Amount / TicketTotal.Amount) * 100;
            
            return discountPercentage > 50;
        }

        return false;
    }

    /// <summary>
    /// Resets the dialog state.
    /// </summary>
    public void Reset()
    {
        SelectedDiscount = null;
        ErrorMessage = string.Empty;
        IsSuccess = false;
        TicketId = Guid.Empty;
        TicketTotal = null;
        AvailableDiscounts.Clear();
        
        OnPropertyChanged(nameof(CanApplyDiscount));
    }

    /// <summary>
    /// Selects a quick discount by percentage (10%, 20%, 50%).
    /// Note: This is a placeholder - actual implementation would need to find or create
    /// a discount with the specified percentage.
    /// </summary>
    [RelayCommand]
    private void SelectQuickDiscount(string percentage)
    {
        // Find a discount matching the percentage
        if (int.TryParse(percentage, out var pct))
        {
            var matchingDiscount = AvailableDiscounts.FirstOrDefault(d => 
                d.Type == Domain.Enumerations.DiscountType.Percentage && 
                d.Value == pct);
            
            if (matchingDiscount != null)
            {
                SelectedDiscount = matchingDiscount;
            }
            else
            {
                ErrorMessage = $"No {pct}% discount available. Please select from the list.";
            }
        }
    }

    /// <summary>
    /// Opens custom discount entry (placeholder for future implementation).
    /// </summary>
    [RelayCommand]
    private void SelectCustomDiscount()
    {
        ErrorMessage = "Custom discount entry not yet implemented. Please select from the list.";
    }

    partial void OnSelectedDiscountChanged(Discount? value)
    {
        OnPropertyChanged(nameof(CanApplyDiscount));
    }
}

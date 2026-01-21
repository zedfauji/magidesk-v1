using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Magidesk.Application.Commands;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Presentation.ViewModels.Dialogs;

/// <summary>
/// ViewModel for Hold Ticket dialog.
/// Handles capturing the reason for holding a ticket and executing the hold command.
/// </summary>
public partial class HoldTicketDialogViewModel : ViewModelBase
{
    private readonly ICommandHandler<HoldTicketCommand> _holdTicketHandler;
    private readonly IUserService _userService;

    [ObservableProperty]
    private string _holdReason = string.Empty;

    [ObservableProperty]
    private string _selectedReasonCode = string.Empty;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private bool _isSuccess = false;

    /// <summary>
    /// Ticket ID to hold.
    /// </summary>
    public Guid TicketId { get; set; }

    /// <summary>
    /// Predefined reason codes for quick selection.
    /// </summary>
    public List<string> ReasonCodes { get; } = new()
    {
        "Customer Request",
        "Payment Issue",
        "Waiting for Additional Items",
        "Customer Stepped Out",
        "Other"
    };

    public HoldTicketDialogViewModel(
        ICommandHandler<HoldTicketCommand> holdTicketHandler,
        IUserService userService)
    {
        _holdTicketHandler = holdTicketHandler;
        _userService = userService;
    }

    /// <summary>
    /// Can submit if reason is provided.
    /// </summary>
    public bool CanSubmit => !string.IsNullOrWhiteSpace(GetFinalReason());

    /// <summary>
    /// Gets the final reason (selected code or custom reason).
    /// </summary>
    private string GetFinalReason()
    {
        if (!string.IsNullOrWhiteSpace(SelectedReasonCode) && SelectedReasonCode != "Other")
        {
            return SelectedReasonCode;
        }
        return HoldReason;
    }

    /// <summary>
    /// Holds the ticket with the specified reason.
    /// </summary>
    [RelayCommand]
    public async Task<bool> HoldTicketAsync()
    {
        var finalReason = GetFinalReason();
        
        if (string.IsNullOrWhiteSpace(finalReason))
        {
            ErrorMessage = "Please provide a reason for holding the ticket.";
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
            var command = new HoldTicketCommand(
                TicketId,
                finalReason,
                new UserId(currentUser.Id)
            );

            await _holdTicketHandler.HandleAsync(command);

            IsSuccess = true;
            return true;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to hold ticket: {ex.Message}";
            return false;
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Resets the dialog state.
    /// </summary>
    public void Reset()
    {
        HoldReason = string.Empty;
        SelectedReasonCode = string.Empty;
        ErrorMessage = string.Empty;
        IsSuccess = false;
        TicketId = Guid.Empty;
        
        OnPropertyChanged(nameof(CanSubmit));
    }

    partial void OnSelectedReasonCodeChanged(string value)
    {
        OnPropertyChanged(nameof(CanSubmit));
    }

    partial void OnHoldReasonChanged(string value)
    {
        OnPropertyChanged(nameof(CanSubmit));
    }
}

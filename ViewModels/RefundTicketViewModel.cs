using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Magidesk.Application.Commands;
using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Magidesk.Presentation.Services;
using Microsoft.UI.Xaml.Controls;
using CommunityToolkit.Mvvm.Input;

namespace Magidesk.Presentation.ViewModels;

public class RefundTicketViewModel : ViewModelBase
{
    private readonly ICommandHandler<RefundTicketCommand> _refundTicketHandler;
    private readonly IUserService _userService;
    private readonly ITerminalContext _terminalContext;

    private TicketDto _ticket;
    public TicketDto Ticket
    {
        get => _ticket;
        set => SetProperty(ref _ticket, value);
    }

    private ObservableCollection<string> _refundReasons;
    public ObservableCollection<string> RefundReasons
    {
        get => _refundReasons;
        set => SetProperty(ref _refundReasons, value);
    }

    private string _selectedReason;
    public string SelectedReason
    {
        get => _selectedReason;
        set
        {
            if (SetProperty(ref _selectedReason, value))
            {
                ((AsyncRelayCommand<object?>)RefundCommand).NotifyCanExecuteChanged();
            }
        }
    }

    private string _errorMessage;
    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    private bool _hasError;
    public bool HasError
    {
        get => _hasError;
        set => SetProperty(ref _hasError, value);
    }

    public ICommand RefundCommand { get; }

    public RefundTicketViewModel(
        ICommandHandler<RefundTicketCommand> refundTicketHandler,
        IUserService userService,
        ITerminalContext terminalContext)
    {
        _refundTicketHandler = refundTicketHandler;
        _userService = userService;
        _terminalContext = terminalContext;
        
        // Manual Command Initialization
        RefundCommand = new AsyncRelayCommand<object?>(RefundAsync, CanRefund);

        // Standard Refund Reasons
        RefundReasons = new ObservableCollection<string>
        {
            "Customer Complaint",
            "Accidental Charge",
            "Order Error",
            "Other"
        };
    }

    public void Initialize(TicketDto ticket)
    {
        Ticket = ticket;
        SelectedReason = null;
        ErrorMessage = string.Empty;
        HasError = false;
    }

    private async Task RefundAsync(object? parameter)
    {
        if (parameter is not ContentDialog dialog) return;

        if (string.IsNullOrEmpty(SelectedReason))
        {
            ErrorMessage = "Please select a reason.";
            HasError = true;
            return;
        }

        // Show confirmation dialog first
        var confirmationDialog = new Views.Dialogs.ConfirmationDialog();
        confirmationDialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot;
        
        var refundAmount = Ticket.PaidAmount;
        var confirmed = await confirmationDialog.ShowConfirmationAsync(
            "Confirm Refund",
            $"Are you sure you want to refund this ticket?",
            "Refund Ticket",
            "Cancel",
            "💰",
            "Warning",
            $"Ticket #{Ticket.TicketNumber} - Amount: {refundAmount:C}\nReason: {SelectedReason}");

        if (!confirmed)
        {
            return; // User cancelled
        }

        // Manager Authorization Required
        var authDialog = App.Services.GetRequiredService<Views.Dialogs.ManagerPinDialog>();
        authDialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot;
        
        var authResult = await authDialog.ShowForOperationAsync($"Refund Ticket #{Ticket.TicketNumber}");
        if (authResult == null || !authResult.Authorized)
        {
            return;
        }

        try
        {
            var currentUser = _userService.CurrentUser;
            if (currentUser == null)
            {
                 ErrorMessage = "No user logged in.";
                 HasError = true;
                 return;
            }
            
            if (_terminalContext.TerminalId == null)
            {
                 ErrorMessage = "No terminal context.";
                 HasError = true;
                 return;
            }

            var command = new RefundTicketCommand
            {
                TicketId = Ticket.Id,
                Amount = new Magidesk.Domain.ValueObjects.Money(refundAmount),
                Reason = SelectedReason,
                RefundedBy = new Magidesk.Domain.ValueObjects.UserId(currentUser.Id),
                AuthorizedBy = new Magidesk.Domain.ValueObjects.UserId(authResult.AuthorizingUserId!.Value)
            };

            await _refundTicketHandler.HandleAsync(command);

            // Show success confirmation
            var successDialog = new Views.Dialogs.ConfirmationDialog();
            successDialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot;
            
            await successDialog.ShowConfirmationAsync(
                "Refund Successful",
                $"Ticket #{Ticket.TicketNumber} has been refunded successfully.",
                "OK",
                "",
                "✅",
                "Success",
                $"Amount refunded: {refundAmount:C}\nAuthorized by: {authResult.AuthorizingUserName}");

            if (dialog is ContentDialog cd)
            {
                cd.Hide();
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Refund failed: {ex.Message}";
            HasError = true;
        }
    }

    private bool CanRefund(object? parameter)
    {
        return !string.IsNullOrEmpty(SelectedReason);
    }
}

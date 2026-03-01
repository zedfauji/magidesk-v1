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

public class VoidTicketViewModel : ViewModelBase
{
    private readonly ICommandHandler<VoidTicketCommand> _voidTicketHandler;
    private readonly IUserContextService _userContextService;

    private TicketDto _ticket;
    public TicketDto Ticket
    {
        get => _ticket;
        set => SetProperty(ref _ticket, value);
    }

    private ObservableCollection<string> _voidReasons;
    public ObservableCollection<string> VoidReasons
    {
        get => _voidReasons;
        set => SetProperty(ref _voidReasons, value);
    }

    private string _selectedReason;
    public string SelectedReason
    {
        get => _selectedReason;
        set
        {
            if (SetProperty(ref _selectedReason, value))
            {
                ((AsyncRelayCommand<object?>)VoidCommand).NotifyCanExecuteChanged();
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

    public ICommand VoidCommand { get; }

    public VoidTicketViewModel(
        ICommandHandler<VoidTicketCommand> voidTicketHandler,
        IUserContextService userContextService)
    {
        _voidTicketHandler = voidTicketHandler;
        _userContextService = userContextService;
        
        // Manual Command Initialization
        VoidCommand = new AsyncRelayCommand<object?>(VoidAsync, CanVoid);

        // Predefined void reasons
        VoidReasons = new ObservableCollection<string>
        {
            "Mistake",
            "Customer Changed Mind",
            "Server Error",
            "Testing",
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

    private async Task VoidAsync(object? parameter)
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
        
        var totalAmount = Ticket.TotalAmount;
        
        var confirmed = await confirmationDialog.ShowConfirmationAsync(
            "Confirm Void",
            $"Are you sure you want to void this ticket?",
            "Void Ticket",
            "Cancel",
            "🗑️",
            "Error",
            $"Ticket #{Ticket.TicketNumber} - Total: {totalAmount:C}\nReason: {SelectedReason}");

        if (!confirmed)
        {
            return; // User cancelled
        }

        // Manager Authorization Required
        var overrideResult = await _userContextService.RequireManagerOverrideAsync($"Void Ticket #{Ticket.TicketNumber}");
        if (!overrideResult.Success || !overrideResult.ManagerId.HasValue)
        {
            // Authorization failed or cancelled - do not proceed
            return;
        }

        try
        {
            var userId = _userContextService.GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                 ErrorMessage = "No user logged in.";
                 HasError = true;
                 return;
            }

            var command = new VoidTicketCommand
            {
                TicketId = Ticket.Id,
                VoidedBy = new Magidesk.Domain.ValueObjects.UserId(userId),
                AuthorizedBy = new Magidesk.Domain.ValueObjects.UserId(overrideResult.ManagerId.Value), 
                Reason = SelectedReason
            };

            await _voidTicketHandler.HandleAsync(command);

            // Show success confirmation
            var successDialog = new Views.Dialogs.ConfirmationDialog();
            successDialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot;
            
            await successDialog.ShowConfirmationAsync(
                "Void Successful",
                $"Ticket #{Ticket.TicketNumber} has been voided successfully.",
                "OK",
                "",
                "✅",
                "Success",
                $"Total voided: {totalAmount:C}\nAuthorized by: Manager");

            if (dialog is ContentDialog cd)
            {
                cd.Hide();
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Void failed: {ex.Message}";
            HasError = true;
        }
    }

    private bool CanVoid(object? parameter)
    {
        return !string.IsNullOrEmpty(SelectedReason);
    }
}

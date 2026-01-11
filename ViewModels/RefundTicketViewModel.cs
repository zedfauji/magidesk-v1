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
    private readonly ICommandHandler<RefundTicketCommand, RefundTicketResult> _refundTicketHandler;
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
        ICommandHandler<RefundTicketCommand, RefundTicketResult> refundTicketHandler,
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

        // Manager Authorization Required
        var authDialog = App.Services.GetRequiredService<Views.Dialogs.ManagerPinDialog>();
        authDialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot;
        
        var authResult = await authDialog.ShowForOperationAsync("Refund Ticket");
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
                ProcessedBy = new Magidesk.Domain.ValueObjects.UserId(currentUser.Id),
                TerminalId = _terminalContext.TerminalId.Value,
                Reason = SelectedReason
            };

            var result = await _refundTicketHandler.HandleAsync(command);

            if (result.Success)
            {
                if (dialog is ContentDialog cd)
                {
                    cd.Hide();
                }
            }
            else
            {
                ErrorMessage = result.ErrorMessage ?? "Refund failed.";
                HasError = true;
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            HasError = true;
        }
    }

    private bool CanRefund(object? parameter)
    {
        return !string.IsNullOrEmpty(SelectedReason);
    }
}

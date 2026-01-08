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
    private readonly IUserService _userService;

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

    private bool _isWasted;
    public bool IsWasted
    {
        get => _isWasted;
        set => SetProperty(ref _isWasted, value);
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
        IUserService userService)
    {
        _voidTicketHandler = voidTicketHandler;
        _userService = userService;
        
        // Manual Command Initialization
        VoidCommand = new AsyncRelayCommand<object?>(VoidAsync, CanVoid);

        // Mock Reasons until Backend table exists (Per Implementation Plan)
        VoidReasons = new ObservableCollection<string>
        {
            "Mistake",
            "Customer Changed Mind",
            "Server Error",
            "Testing",
            "Other"
        };
        IsWasted = true; // Default to waste per Audit implication suitable for food
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

        // Manager Authorization Required
        var authDialog = App.Services.GetRequiredService<Views.Dialogs.ManagerPinDialog>();
        authDialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot;
        
        var authResult = await authDialog.ShowForOperationAsync("Void Ticket");
        if (authResult == null || !authResult.Authorized)
        {
            // Authorization failed or cancelled - do not proceed
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

            var command = new VoidTicketCommand
            {
                TicketId = Ticket.Id,
                VoidedBy = new Magidesk.Domain.ValueObjects.UserId(currentUser.Id),
                Reason = SelectedReason,
                IsWasted = IsWasted
            };

            await _voidTicketHandler.HandleAsync(command);

            if (dialog is ContentDialog cd)
            {
                cd.Hide();
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            HasError = true;
        }
    }

    private bool CanVoid(object? parameter)
    {
        return !string.IsNullOrEmpty(SelectedReason);
    }
}

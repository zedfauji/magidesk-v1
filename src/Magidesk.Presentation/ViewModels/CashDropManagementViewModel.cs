using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Presentation.Services;
using Magidesk.Presentation.Views;
using Microsoft.UI.Xaml.Controls;
using CommunityToolkit.Mvvm.Input;

namespace Magidesk.Presentation.ViewModels;

public class CashDropManagementViewModel : ViewModelBase
{
    private readonly ICashSessionRepository _cashSessionRepository;
    private readonly NavigationService _navigationService;
    private readonly ISecurityService _securityService; // To get usernames if needed
    private readonly IUserService _userService;
    private readonly ITerminalContext _terminalContext;
    private readonly ICashBalanceTrackingService _cashBalanceService;

    private ObservableCollection<CashTransactionUiDto> _transactions = new();
    public ObservableCollection<CashTransactionUiDto> Transactions
    {
        get => _transactions;
        set => SetProperty(ref _transactions, value);
    }

    private CashTransactionUiDto? _selectedTransaction;
    public CashTransactionUiDto? SelectedTransaction
    {
        get => _selectedTransaction;
        set
        {
            if (SetProperty(ref _selectedTransaction, value))
            {
                ((AsyncRelayCommand)DeleteCommand).NotifyCanExecuteChanged();
            }
        }
    }

    public ICommand AddCashDropCommand { get; }
    public ICommand AddDrawerBleedCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand CloseCommand { get; }

    public CashDropManagementViewModel(
        ICashSessionRepository cashSessionRepository,
        NavigationService navigationService,
        ISecurityService securityService,
        IUserService userService,
        ITerminalContext terminalContext,
        ICashBalanceTrackingService cashBalanceService)
    {
        _cashSessionRepository = cashSessionRepository;
        _navigationService = navigationService;
        _securityService = securityService;
        _userService = userService;
        _terminalContext = terminalContext;
        _cashBalanceService = cashBalanceService;

        AddCashDropCommand = new AsyncRelayCommand(AddCashDropAsync);
        AddDrawerBleedCommand = new AsyncRelayCommand(AddDrawerBleedAsync);
        DeleteCommand = new AsyncRelayCommand(DeleteTransactionAsync, () => SelectedTransaction != null);
        CloseCommand = new RelayCommand(Close);

        // Load data on init
        _ = LoadTransactionsAsync();
    }

    private async Task LoadTransactionsAsync()
    {
        if (_terminalContext.TerminalId == null)
        {
            return;
        }

        var terminalId = _terminalContext.TerminalId.Value;
        var session = await _cashSessionRepository.GetOpenSessionByTerminalIdAsync(terminalId);

        if (session != null)
        {
            var drops = session.CashDrops.Select(d => new CashTransactionUiDto
            {
                Id = d.Id,
                ProcessedAt = d.ProcessedAt,
                Amount = d.Amount.Amount,
                Reason = d.Reason,
                Type = "Drop",
                ProcessedBy = d.ProcessedBy.Value.ToString() // Ideally lookup name
            });

            var bleeds = session.DrawerBleeds.Select(b => new CashTransactionUiDto
            {
                Id = b.Id,
                ProcessedAt = b.ProcessedAt,
                Amount = b.Amount.Amount,
                Reason = b.Reason,
                Type = "Bleed",
                ProcessedBy = b.ProcessedBy.Value.ToString()
            });

            var all = drops.Concat(bleeds).OrderByDescending(x => x.ProcessedAt);
            Transactions = new ObservableCollection<CashTransactionUiDto>(all);
        }
    }

    private async Task AddCashDropAsync()
    {
        await PerformDrawerOperationAsync(isBleed: false);
    }

    private async Task AddDrawerBleedAsync()
    {
        await PerformDrawerOperationAsync(isBleed: true);
    }

    private async Task PerformDrawerOperationAsync(bool isBleed)
    {
        string title = isBleed ? "New Drawer Bleed" : "New Cash Drop";
        string message = isBleed ? "Enter amount to bleed from drawer." : "Enter amount to drop from drawer.";

        var dialog = new Views.Dialogs.CashEntryDialog();
        dialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot;
        
        var (result, amount, reason) = await dialog.ShowCashEntryAsync(title, message, true, true);

        if (result == ContentDialogResult.Primary)
        {
            if (_terminalContext.TerminalId == null || _userService.CurrentUser?.Id == null)
            {
                var errorDialog = new Views.Dialogs.ConfirmationDialog();
                errorDialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot;
                
                await errorDialog.ShowConfirmationAsync(
                    "Error",
                    "Unable to process transaction: missing terminal or user context.",
                    "OK",
                    "",
                    "❌",
                    "Error",
                    "Please ensure you are logged in and the terminal is properly configured.");
                return;
            }

            // Manager Authorization Required for cash operations
            var authDialog = App.Services.GetRequiredService<Views.Dialogs.ManagerPinDialog>();
            authDialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot;
            
            var operationType = isBleed ? $"Drawer Bleed ({amount:C})" : $"Cash Drop ({amount:C})";
            var authResult = await authDialog.ShowForOperationAsync(operationType);
            if (authResult == null || !authResult.Authorized)
            {
                return;
            }

            try
            {
                var terminalId = _terminalContext.TerminalId.Value;
                var userId = _userService.CurrentUser.Id;
                var moneyAmount = new Magidesk.Domain.ValueObjects.Money(amount);

                var session = await _cashSessionRepository.GetOpenSessionByTerminalIdAsync(terminalId);
                if (session == null)
                {
                    var errorDialog = new Views.Dialogs.ConfirmationDialog();
                    errorDialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot;
                    
                    await errorDialog.ShowConfirmationAsync(
                        "No Active Session",
                        "No active cash session found for this terminal.",
                        "OK",
                        "",
                        "⚠️",
                        "Warning",
                        "Please start a cash session before performing cash operations.");
                    return;
                }

                if (isBleed)
                {
                    var bleed = DrawerBleed.Create(session.Id, moneyAmount, userId, reason);
                    session.AddDrawerBleed(bleed);
                }
                else
                {
                    var drop = CashDrop.Create(session.Id, moneyAmount, userId, reason);
                    session.AddCashDrop(drop);
                }

                await _cashSessionRepository.UpdateAsync(session);
                
                // Update real-time cash balance tracking
                var transactionType = isBleed ? 
                    Magidesk.Application.Interfaces.CashTransactionType.DrawerBleed : 
                    Magidesk.Application.Interfaces.CashTransactionType.CashDrop;
                await _cashBalanceService.UpdateCashBalanceAsync(terminalId, amount, transactionType);
                
                await LoadTransactionsAsync(); // Refresh list

                // Show success confirmation
                var successDialog = new Views.Dialogs.ConfirmationDialog();
                successDialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot;
                
                var operationName = isBleed ? "Drawer bleed" : "Cash drop";
                await successDialog.ShowConfirmationAsync(
                    "Success",
                    $"{operationName} processed successfully.",
                    "OK",
                    "",
                    "✅",
                    "Success",
                    $"Amount: {moneyAmount.Amount:C}\nReason: {reason}\nAuthorized by: {authResult.AuthorizingUserName}");
            }
            catch (Exception ex)
            {
                var errorDialog = new Views.Dialogs.ConfirmationDialog();
                errorDialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot;
                
                var operationName = isBleed ? "drawer bleed" : "cash drop";
                await errorDialog.ShowConfirmationAsync(
                    "Error",
                    $"Failed to process {operationName}.",
                    "OK",
                    "",
                    "❌",
                    "Error",
                    $"Error details: {ex.Message}");
            }
        }
    }

    private async Task DeleteTransactionAsync()
    {
        if (SelectedTransaction == null) return;

        // Show confirmation dialog
        var confirmationDialog = new Views.Dialogs.ConfirmationDialog();
        confirmationDialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot;
        
        var confirmed = await confirmationDialog.ShowConfirmationAsync(
            "Delete Transaction",
            $"Are you sure you want to delete this {SelectedTransaction.Type.ToLower()}?",
            "Delete",
            "Cancel",
            "🗑️",
            "Warning",
            $"Amount: {SelectedTransaction.Amount:C}\nReason: {SelectedTransaction.Reason}\nProcessed: {SelectedTransaction.ProcessedAt:g}");

        if (!confirmed)
        {
            return; // User cancelled
        }

        // Manager Authorization Required
        var authDialog = App.Services.GetRequiredService<Views.Dialogs.ManagerPinDialog>();
        authDialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot;
        
        var authResult = await authDialog.ShowForOperationAsync($"Delete {SelectedTransaction.Type}");
        if (authResult == null || !authResult.Authorized)
        {
            return;
        }

        try
        {
            // TODO: Implement delete logic in Domain/Repository
            // Domain currently doesn't expose RemoveCashDrop/RemoveDrawerBleed on CashSession
            // This is a domain model limitation that needs to be addressed
            
            // For now, show a more informative error message
            var errorDialog = new Views.Dialogs.ConfirmationDialog();
            errorDialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot;
            
            await errorDialog.ShowConfirmationAsync(
                "Feature Not Available",
                "Cash transaction deletion is not currently supported by the system.",
                "OK",
                "",
                "ℹ️",
                "Info",
                "This feature requires domain model enhancements. Please contact your system administrator if this functionality is needed.");
        }
        catch (Exception ex)
        {
            // Show error dialog
            var errorDialog = new Views.Dialogs.ConfirmationDialog();
            errorDialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot;
            
            await errorDialog.ShowConfirmationAsync(
                "Error",
                "An error occurred while deleting the transaction.",
                "OK",
                "",
                "❌",
                "Error",
                $"Error details: {ex.Message}");
        }
    }

    private void Close()
    {
        // This is a dialog, so we assume the view will handle closing, or we navigation service to close it?
        // NavigationService.CloseDialogAsync() ?
        // If this VM is used in a ContentDialog, the CloseCommand usually binds to the dialog's primary/close button logic or we close programmatically.
    }
}

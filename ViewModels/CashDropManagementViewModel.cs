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
        ITerminalContext terminalContext)
    {
        _cashSessionRepository = cashSessionRepository;
        _navigationService = navigationService;
        _securityService = securityService;
        _userService = userService;
        _terminalContext = terminalContext;

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
        string message = isBleed ? "Enter amount to bleed." : "Enter amount to drop.";

        var dialog = new Magidesk.Presentation.Views.CashEntryDialog(title, message);
        dialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot;
        var result = await _navigationService.ShowDialogAsync(dialog);

        if (result == ContentDialogResult.Primary)
        {
            if (_terminalContext.TerminalId == null || _userService.CurrentUser?.Id == null)
            {
                return;
            }

            var terminalId = _terminalContext.TerminalId.Value;
            var userId = _userService.CurrentUser.Id;
            var amount = new Magidesk.Domain.ValueObjects.Money(dialog.Amount);
            var reason = dialog.Reason;

            var session = await _cashSessionRepository.GetOpenSessionByTerminalIdAsync(terminalId);
            if (session != null)
            {
                if (isBleed)
                {
                    var bleed = DrawerBleed.Create(session.Id, amount, userId, reason);
                    session.AddDrawerBleed(bleed);
                }
                else
                {
                    var drop = CashDrop.Create(session.Id, amount, userId, reason);
                    session.AddCashDrop(drop);
                }

                await _cashSessionRepository.UpdateAsync(session);
                await LoadTransactionsAsync(); // Refresh list
            }
        }
    }

    private async Task DeleteTransactionAsync()
    {
        // TODO: Implement delete logic in Domain/Repository
        // Domain currently doesn't expose RemoveCashDrop/RemoveDrawerBleed on CashSession
        // This might be a limitation of the current domain model.
        // For F-0010 parity, delete is required.
        // We need to add Remove methods to CashSession.
        
        if (SelectedTransaction == null) return;

        // Placeholder message until domain supports removal
        var dialog = new ContentDialog
        {
            Title = "Not Implemented",
            Content = "Deleting cash drops is not yet supported in the domain model.",
            CloseButtonText = "OK",
            XamlRoot = App.MainWindowInstance.Content.XamlRoot
        };
        await _navigationService.ShowDialogAsync(dialog);
    }

    private void Close()
    {
        // This is a dialog, so we assume the view will handle closing, or we navigation service to close it?
        // NavigationService.CloseDialogAsync() ?
        // If this VM is used in a ContentDialog, the CloseCommand usually binds to the dialog's primary/close button logic or we close programmatically.
    }
}

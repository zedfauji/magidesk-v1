using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Magidesk.Application.Commands;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Domain.ValueObjects;
using Magidesk.Presentation.ViewModels;

namespace Magidesk.Presentation.ViewModels.Dialogs;

public partial class ShiftEndViewModel : ViewModelBase
{
    private readonly ICommandHandler<CloseCashSessionCommand, CloseCashSessionResult> _closeSessionHandler;
    private readonly CashSession _session;
    private readonly UserId _closedBy;

    public ShiftEndViewModel(
        CashSession session,
        UserId closedBy,
        ICommandHandler<CloseCashSessionCommand, CloseCashSessionResult> closeSessionHandler)
    {
        _session = session;
        _closedBy = closedBy;
        _closeSessionHandler = closeSessionHandler;
        
        _expectedCash = session.ExpectedCash.Amount;
    }

    private decimal _expectedCash;
    public decimal ExpectedCash
    {
        get => _expectedCash;
        set => SetProperty(ref _expectedCash, value);
    }

    private decimal _closingCash;
    public decimal ClosingCash
    {
        get => _closingCash;
        set => SetProperty(ref _closingCash, value);
    }

    private double _closingBalanceDouble;
    public double ClosingBalanceDouble
    {
        get => _closingBalanceDouble;
        set => SetProperty(ref _closingBalanceDouble, value);
    }

    public Action? CloseAction { get; set; }
    public bool IsConfirmed { get; private set; }

    [RelayCommand]
    private async Task ConfirmAsync()
    {
        IsBusy = true;
        try
        {
            var command = new CloseCashSessionCommand
            {
                CashSessionId = _session.Id,
                ClosedBy = _closedBy,
                ActualCash = new Money((decimal)_closingBalanceDouble)
            };

            await _closeSessionHandler.HandleAsync(command);
            IsConfirmed = true;
            CloseAction?.Invoke();
        }
        catch (Exception ex)
        {
            // Handle error (e.g. show message)
            System.Diagnostics.Debug.WriteLine($"Error closing session: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void Cancel()
    {
        CloseAction?.Invoke();
    }
}

using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using Magidesk.Application.Commands;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Domain.ValueObjects;
using Magidesk.Presentation.Services;

namespace Magidesk.Presentation.ViewModels;

public sealed partial class ShiftStartViewModel : ViewModelBase
{
    private readonly IShiftRepository _shiftRepository;
    private readonly ICommandHandler<OpenCashSessionCommand, OpenCashSessionResult> _openSessionHandler;
    private readonly IUserService _userService;
    private readonly ITerminalContext _terminalContext;

    private ObservableCollection<Shift> _availableShifts = new();
    public ObservableCollection<Shift> AvailableShifts
    {
        get => _availableShifts;
        set => SetProperty(ref _availableShifts, value);
    }

    private Shift? _selectedShift;
    public Shift? SelectedShift
    {
        get => _selectedShift;
        set
        {
            if (SetProperty(ref _selectedShift, value))
            {
                ConfirmCommand.NotifyCanExecuteChanged();
            }
        }
    }

    private double _openingBalance;
    public double OpeningBalance
    {
        get => _openingBalance;
        set
        {
            if (SetProperty(ref _openingBalance, value))
            {
                ConfirmCommand.NotifyCanExecuteChanged();
            }
        }
    }

    private bool _isBusy;
    public new bool IsBusy
    {
        get => _isBusy;
        set => SetProperty(ref _isBusy, value);
    }

    public CommunityToolkit.Mvvm.Input.IAsyncRelayCommand ConfirmCommand { get; }
    public ICommand CancelCommand { get; }

    public Action<ContentDialogResult>? CloseAction { get; set; }

    public ShiftStartViewModel(
        IShiftRepository shiftRepository,
        ICommandHandler<OpenCashSessionCommand, OpenCashSessionResult> openSessionHandler,
        IUserService userService,
        ITerminalContext terminalContext)
    {
        _shiftRepository = shiftRepository;
        _openSessionHandler = openSessionHandler;
        _userService = userService;
        _terminalContext = terminalContext;

        ConfirmCommand = new CommunityToolkit.Mvvm.Input.AsyncRelayCommand(ConfirmAsync, CanConfirm);
        CancelCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(Cancel);
    }

    public async Task InitializeAsync()
    {
        IsBusy = true;
        try
        {
            var shifts = await _shiftRepository.GetActiveShiftsAsync();
            AvailableShifts = new ObservableCollection<Shift>(shifts);

            // Auto-select current shift if match found
            var currentShift = await _shiftRepository.GetCurrentShiftAsync();
            if (currentShift != null)
            {
                SelectedShift = AvailableShifts.FirstOrDefault(s => s.Id == currentShift.Id);
            }
            
            if (SelectedShift == null && AvailableShifts.Any())
            {
                SelectedShift = AvailableShifts.First();
            }
        }
        catch (Exception ex)
        {
            // Log error
            System.Diagnostics.Debug.WriteLine($"Error init shift start: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private bool CanConfirm()
    {
        return SelectedShift != null && OpeningBalance >= 0;
    }

    private async Task ConfirmAsync()
    {
        if (!CanConfirm()) return;
        if (_userService.CurrentUser == null || _terminalContext.TerminalId == null) return;

        IsBusy = true;
        try
        {
            var command = new OpenCashSessionCommand
            {
                UserId = new UserId(_userService.CurrentUser.Id),
                TerminalId = _terminalContext.TerminalId.Value,
                ShiftId = SelectedShift!.Id,
                OpeningBalance = new Money((decimal)OpeningBalance)
            };

            await _openSessionHandler.HandleAsync(command);
            
            CloseAction?.Invoke(ContentDialogResult.Primary);
        }
        catch (Exception ex)
        {
            // TODO: Show error in dialog
            System.Diagnostics.Debug.WriteLine($"Failed to open session: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void Cancel()
    {
        CloseAction?.Invoke(ContentDialogResult.Secondary);
    }
}

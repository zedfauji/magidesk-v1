using Magidesk.Application.Commands;
using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries;
using Magidesk.Domain.ValueObjects;
using Magidesk.Presentation.Services;
using CommunityToolkit.Mvvm.Input;

namespace Magidesk.Presentation.ViewModels;

public sealed class CashSessionViewModel : ViewModelBase
{
    private readonly IQueryHandler<GetCurrentCashSessionQuery, GetCurrentCashSessionResult> _getCurrent;
    private readonly ICommandHandler<OpenCashSessionCommand, OpenCashSessionResult> _open;
    private readonly ICommandHandler<CloseCashSessionCommand, CloseCashSessionResult> _close;
    private readonly IUserService _userService;
    private readonly ITerminalContext _terminalContext;
    private readonly NavigationService _navigationService;

    private CashSessionDto? _current;
    private string _userIdText = Guid.Empty.ToString();
    private string _terminalIdText = Guid.Empty.ToString();
    private string _shiftIdText = Guid.Empty.ToString();
    private string _openingBalanceText = "0";
    private string _actualCashText = "0";
    private string? _error;

    public CashSessionViewModel(
        IQueryHandler<GetCurrentCashSessionQuery, GetCurrentCashSessionResult> getCurrent,
        ICommandHandler<OpenCashSessionCommand, OpenCashSessionResult> open,
        ICommandHandler<CloseCashSessionCommand, CloseCashSessionResult> close,
        IUserService userService,
        ITerminalContext terminalContext,
        NavigationService navigationService)
    {
        _getCurrent = getCurrent;
        _open = open;
        _close = close;
        _userService = userService;
        _terminalContext = terminalContext;
        _navigationService = navigationService;

        RefreshCommand = new AsyncRelayCommand(RefreshAsync);
        OpenCommand = new AsyncRelayCommand(OpenAsync);
        CloseCommand = new AsyncRelayCommand(CloseAsync);
        GoBackCommand = new RelayCommand(GoBack);

        Title = "Cash Session";

        if (_userService.CurrentUser?.Id != null)
        {
            UserIdText = _userService.CurrentUser.Id.ToString();
        }

        if (_terminalContext.TerminalId != null)
        {
            TerminalIdText = _terminalContext.TerminalId.Value.ToString();
        }
    }

    public AsyncRelayCommand RefreshCommand { get; }
    public AsyncRelayCommand OpenCommand { get; }
    public AsyncRelayCommand CloseCommand { get; }
    public RelayCommand GoBackCommand { get; }

    public CashSessionDto? Current
    {
        get => _current;
        private set
        {
            if (SetProperty(ref _current, value))
            {
                OnPropertyChanged(nameof(HasOpenSession));
                OnPropertyChanged(nameof(CurrentStatusText));
                OnPropertyChanged(nameof(CurrentSessionIdText));
                OnPropertyChanged(nameof(CurrentOpenedAtText));
                OnPropertyChanged(nameof(CurrentExpectedCashText));
            }
        }
    }

    public bool HasOpenSession => Current != null && Current.ClosedAt == null;

    public string CurrentStatusText => Current == null ? "None" : "OPEN";

    public string CurrentSessionIdText => Current == null ? string.Empty : $"SessionId: {Current.Id}";

    public string CurrentOpenedAtText => Current == null ? string.Empty : $"OpenedAt: {Current.OpenedAt}";

    public string CurrentExpectedCashText => Current == null ? string.Empty : $"ExpectedCash: {Current.ExpectedCash}";

    public string UserIdText
    {
        get => _userIdText;
        set => SetProperty(ref _userIdText, value);
    }

    public string TerminalIdText
    {
        get => _terminalIdText;
        set => SetProperty(ref _terminalIdText, value);
    }

    public string ShiftIdText
    {
        get => _shiftIdText;
        set => SetProperty(ref _shiftIdText, value);
    }

    public string OpeningBalanceText
    {
        get => _openingBalanceText;
        set => SetProperty(ref _openingBalanceText, value);
    }

    public string ActualCashText
    {
        get => _actualCashText;
        set => SetProperty(ref _actualCashText, value);
    }

    public string? Error
    {
        get => _error;
        private set
        {
            if (SetProperty(ref _error, value))
            {
                OnPropertyChanged(nameof(HasError));
            }
        }
    }

    public bool HasError => !string.IsNullOrWhiteSpace(Error);

    private void GoBack()
    {
        if (_navigationService.CanGoBack)
        {
            _navigationService.GoBack();
        }
    }

    public async Task RefreshAsync()
    {
        Error = null;
        IsBusy = true;
        try
        {
            if (!Guid.TryParse(UserIdText, out var userId))
            {
                Error = "Invalid UserId.";
                return;
            }

            var result = await _getCurrent.HandleAsync(new GetCurrentCashSessionQuery { UserId = userId });
            Current = result.CashSession;
        }
        catch (Exception ex)
        {
            Error = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task OpenAsync()
    {
        Error = null;
        IsBusy = true;
        try
        {
            if (!Guid.TryParse(UserIdText, out var userId)) { Error = "Invalid UserId."; return; }
            if (!Guid.TryParse(TerminalIdText, out var terminalId)) { Error = "Invalid TerminalId."; return; }
            if (!Guid.TryParse(ShiftIdText, out var shiftId)) { Error = "Invalid ShiftId."; return; }
            if (!decimal.TryParse(OpeningBalanceText, out var openingBalance)) { Error = "Invalid Opening Balance."; return; }

            await _open.HandleAsync(new OpenCashSessionCommand
            {
                UserId = new UserId(userId),
                TerminalId = terminalId,
                ShiftId = shiftId,
                OpeningBalance = new Money(openingBalance)
            });

            await RefreshAsync();
        }
        catch (Exception ex)
        {
            Error = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task CloseAsync()
    {
        Error = null;
        IsBusy = true;
        try
        {
            if (Current == null) { Error = "No open cash session."; return; }
            if (!Guid.TryParse(UserIdText, out var userId)) { Error = "Invalid UserId."; return; }
            if (!decimal.TryParse(ActualCashText, out var actualCash)) { Error = "Invalid Actual Cash."; return; }

            await _close.HandleAsync(new CloseCashSessionCommand
            {
                CashSessionId = Current.Id,
                ClosedBy = new UserId(userId),
                ActualCash = new Money(actualCash)
            });

            await RefreshAsync();
        }
        catch (Exception ex)
        {
            Error = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }
}

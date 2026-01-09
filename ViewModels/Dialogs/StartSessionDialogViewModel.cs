using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using Magidesk.Application.Commands.TableSessions;
using Magidesk.Application.Interfaces;
using Magidesk.Presentation.ViewModels;
using Microsoft.Extensions.Logging;

namespace Magidesk.Presentation.ViewModels.Dialogs;

/// <summary>
/// ViewModel for starting a new table session.
/// </summary>
public class StartSessionDialogViewModel : ViewModelBase
{
    private readonly ICommandHandler<StartTableSessionCommand, StartTableSessionResult> _startSessionHandler;
    private readonly ILogger<StartSessionDialogViewModel> _logger;

    // Table context
    private Guid _tableId;
    private Guid _tableTypeId;
    private string _tableName = string.Empty;
    private string _tableTypeName = string.Empty;
    private decimal _hourlyRate;
    private Guid? _ticketId;

    // Input properties
    private int _guestCount = 1;
    private Guid? _customerId;
    private string? _customerName;
    private string? _error;

    public string TableName
    {
        get => _tableName;
        set => SetProperty(ref _tableName, value);
    }

    public string TableTypeName
    {
        get => _tableTypeName;
        set => SetProperty(ref _tableTypeName, value);
    }

    public string HourlyRateDisplay => $"${_hourlyRate:F2}/hour";

    public int GuestCount
    {
        get => _guestCount;
        set
        {
            if (value > 0 && value <= 20)
            {
                SetProperty(ref _guestCount, value);
                Error = null;
            }
            else
            {
                Error = "Guest count must be between 1 and 20.";
            }
        }
    }

    public string? CustomerName
    {
        get => _customerName;
        set => SetProperty(ref _customerName, value);
    }

    public string? Error
    {
        get => _error;
        set => SetProperty(ref _error, value);
    }

    public AsyncRelayCommand StartCommand { get; }
    public RelayCommand CancelCommand { get; }

    public event EventHandler? RequestClose;
    public event EventHandler<StartTableSessionResult>? SessionStarted;

    public StartSessionDialogViewModel(
        ICommandHandler<StartTableSessionCommand, StartTableSessionResult> startSessionHandler,
        ILogger<StartSessionDialogViewModel> logger)
    {
        _startSessionHandler = startSessionHandler ?? throw new ArgumentNullException(nameof(startSessionHandler));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        StartCommand = new AsyncRelayCommand(StartSessionAsync, () => !IsBusy);
        CancelCommand = new RelayCommand(Cancel);
    }

    /// <summary>
    /// Initializes the dialog with table information.
    /// </summary>
    /// <summary>
    /// Initializes the dialog with table information.
    /// </summary>
    public void Initialize(Guid tableId, Guid tableTypeId, string tableName, string tableTypeName, decimal hourlyRate, Guid? ticketId = null)
    {
        _tableId = tableId;
        _tableTypeId = tableTypeId;
        TableName = tableName;
        TableTypeName = tableTypeName;
        _hourlyRate = hourlyRate;
        _ticketId = ticketId;
        
        // Reset state
        GuestCount = 1;
        _customerId = null;
        CustomerName = null;
        Error = null;
        
        Title = $"Start Session - {tableName}";
    }

    /// <summary>
    /// Sets the selected customer (optional).
    /// </summary>
    public void SetCustomer(Guid? customerId, string? customerName)
    {
        _customerId = customerId;
        CustomerName = customerName;
    }

    private async Task StartSessionAsync()
    {
        try
        {
            IsBusy = true;
            Error = null;

            // Validate
            if (GuestCount <= 0)
            {
                Error = "Please enter a valid guest count.";
                return;
            }

            // Create command
            var command = new StartTableSessionCommand(
                _tableId,
                _tableTypeId,
                _customerId,
                GuestCount,
                _ticketId
            );

            // Execute
            var result = await _startSessionHandler.HandleAsync(command);

            _logger.LogInformation(
                "Table session started: SessionId={SessionId}, TableId={TableId}, GuestCount={GuestCount}",
                result.SessionId, _tableId, GuestCount);

            // Notify success and close
            SessionStarted?.Invoke(this, result);
            RequestClose?.Invoke(this, EventArgs.Empty);
        }
        catch (InvalidOperationException ex)
        {
            // Business rule violation (e.g., table not available)
            _logger.LogWarning(ex, "Failed to start session for table {TableId}: {Message}", _tableId, ex.Message);
            Error = ex.Message;
        }
        catch (Exception ex)
        {
            // Unexpected error
            _logger.LogError(ex, "Unexpected error starting session for table {TableId}", _tableId);
            Error = $"Failed to start session: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void Cancel()
    {
        RequestClose?.Invoke(this, EventArgs.Empty);
    }
}

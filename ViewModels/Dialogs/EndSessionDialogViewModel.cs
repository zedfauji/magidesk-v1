using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Magidesk.Application.Commands.TableSessions;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Magidesk.Presentation.ViewModels.Dialogs;

/// <summary>
/// ViewModel for the End Session Dialog.
/// </summary>
public partial class EndSessionDialogViewModel : ViewModelBase
{
    private readonly ICommandHandler<EndTableSessionCommand, EndTableSessionResult> _endSessionHandler;
    private readonly ILogger<EndSessionDialogViewModel> _logger;

    [ObservableProperty]
    private Guid _sessionId;

    [ObservableProperty]
    private string _duration = string.Empty;

    [ObservableProperty]
    private decimal _hourlyRate;

    [ObservableProperty]
    private decimal _totalCharge;

    [ObservableProperty]
    private bool _createNewTicket = true;

    [ObservableProperty]
    private bool _addToExistingTicket;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string? _errorMessage;

    [ObservableProperty]
    private bool _hasError;

    public event EventHandler? RequestClose;
    public event EventHandler<EndTableSessionResult>? SessionEnded;

    public EndSessionDialogViewModel(
        ICommandHandler<EndTableSessionCommand, EndTableSessionResult> endSessionHandler,
        ILogger<EndSessionDialogViewModel> logger)
    {
        _endSessionHandler = endSessionHandler ?? throw new ArgumentNullException(nameof(endSessionHandler));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        EndSessionCommand = new AsyncRelayCommand(EndSessionAsync);
        CancelCommand = new RelayCommand(Cancel);
    }

    public AsyncRelayCommand EndSessionCommand { get; }
    public RelayCommand CancelCommand { get; }

    /// <summary>
    /// Initializes the dialog with session data.
    /// </summary>
    public void Initialize(Guid sessionId, TimeSpan duration, decimal hourlyRate, decimal totalCharge)
    {
        SessionId = sessionId;
        Duration = $"{(int)duration.TotalHours:D2}:{duration.Minutes:D2}:{duration.Seconds:D2}";
        HourlyRate = hourlyRate;
        TotalCharge = totalCharge;
        HasError = false;
        ErrorMessage = null;
    }

    private async Task EndSessionAsync()
    {
        try
        {
            IsLoading = true;
            HasError = false;
            ErrorMessage = null;

            var command = new EndTableSessionCommand(
                SessionId,
                CreateNewTicket
            );

            var result = await _endSessionHandler.HandleAsync(command);

            _logger.LogInformation(
                "Session {SessionId} ended successfully. Ticket: {TicketId}, Charge: {Charge}",
                result.SessionId, result.TicketId, result.TotalCharge);

            // Notify listeners
            SessionEnded?.Invoke(this, result);

            // Close dialog
            RequestClose?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to end session {SessionId}", SessionId);
            HasError = true;
            ErrorMessage = $"Failed to end session: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void Cancel()
    {
        RequestClose?.Invoke(this, EventArgs.Empty);
    }

    partial void OnCreateNewTicketChanged(bool value)
    {
        if (value)
        {
            AddToExistingTicket = false;
        }
    }

    partial void OnAddToExistingTicketChanged(bool value)
    {
        if (value)
        {
            CreateNewTicket = false;
        }
    }
}

using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Magidesk.Application.Commands.TableSessions;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Services;
using Magidesk.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Magidesk.Presentation.ViewModels.Dialogs;

/// <summary>
/// ViewModel for manager override operations including time adjustments, pricing overrides, and force session end.
/// </summary>
public partial class ManagerOverrideDialogViewModel : ViewModelBase
{
    private readonly IManagerOverrideService _managerOverrideService;
    private readonly ILogger<ManagerOverrideDialogViewModel> _logger;

    [ObservableProperty]
    private Guid _sessionId;

    [ObservableProperty]
    private string _tableName = string.Empty;

    [ObservableProperty]
    private ManagerOverrideType _overrideType;

    [ObservableProperty]
    private string _managerPin = string.Empty;

    [ObservableProperty]
    private int _timeAdjustmentMinutes;

    [ObservableProperty]
    private decimal _pricingOverrideAmount;

    [ObservableProperty]
    private string _reason = string.Empty;

    [ObservableProperty]
    private string _selectedReasonCode = string.Empty;

    [ObservableProperty]
    private TimeSpan _currentSessionTime;

    [ObservableProperty]
    private decimal _currentCharge;

    [ObservableProperty]
    private string? _errorMessage;

    [ObservableProperty]
    private bool _hasError;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _isAuthorized;

    [ObservableProperty]
    private Guid? _authorizedManagerId;

    public ObservableCollection<string> TimeAdjustmentReasons { get; } = new()
    {
        "Customer complaint resolution",
        "Equipment malfunction",
        "Staff error correction",
        "System technical issue",
        "Customer service gesture",
        "Other (specify in notes)"
    };

    public ObservableCollection<string> PricingOverrideReasons { get; } = new()
    {
        "Customer loyalty discount",
        "Service recovery",
        "Promotional pricing",
        "Group discount",
        "Special circumstances",
        "Other (specify in notes)"
    };

    public ObservableCollection<string> ForceEndReasons { get; } = new()
    {
        "Emergency situation",
        "Equipment failure",
        "Customer dispute",
        "Safety concern",
        "System malfunction",
        "Other (specify in notes)"
    };

    public string OverrideTypeDisplay => OverrideType switch
    {
        ManagerOverrideType.TimeAdjustment => "Time Adjustment",
        ManagerOverrideType.PricingOverride => "Pricing Override",
        ManagerOverrideType.ForceEnd => "Force End Session",
        _ => "Unknown"
    };

    public string TimeAdjustmentDisplay => TimeAdjustmentMinutes >= 0 
        ? $"Add {TimeAdjustmentMinutes} minutes" 
        : $"Subtract {Math.Abs(TimeAdjustmentMinutes)} minutes";

    public ObservableCollection<string> CurrentReasonCodes => OverrideType switch
    {
        ManagerOverrideType.TimeAdjustment => TimeAdjustmentReasons,
        ManagerOverrideType.PricingOverride => PricingOverrideReasons,
        ManagerOverrideType.ForceEnd => ForceEndReasons,
        _ => new ObservableCollection<string>()
    };

    public event EventHandler? RequestClose;
    public event EventHandler<ManagerOverrideEventArgs>? OverrideCompleted;

    public ManagerOverrideDialogViewModel(
        IManagerOverrideService managerOverrideService,
        ILogger<ManagerOverrideDialogViewModel> logger)
    {
        _managerOverrideService = managerOverrideService ?? throw new ArgumentNullException(nameof(managerOverrideService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        AuthorizeCommand = new AsyncRelayCommand(AuthorizeAsync, () => !string.IsNullOrWhiteSpace(ManagerPin) && !IsLoading);
        ApplyOverrideCommand = new AsyncRelayCommand(ApplyOverrideAsync, () => IsAuthorized && CanApplyOverride() && !IsLoading);
        CancelCommand = new RelayCommand(Cancel);
    }

    public AsyncRelayCommand AuthorizeCommand { get; }
    public AsyncRelayCommand ApplyOverrideCommand { get; }
    public RelayCommand CancelCommand { get; }

    /// <summary>
    /// Initializes the dialog with session and override information.
    /// </summary>
    public void Initialize(
        Guid sessionId,
        string tableName,
        ManagerOverrideType overrideType,
        TimeSpan currentSessionTime,
        decimal currentCharge)
    {
        SessionId = sessionId;
        TableName = tableName;
        OverrideType = overrideType;
        CurrentSessionTime = currentSessionTime;
        CurrentCharge = currentCharge;
        
        // Reset state
        ManagerPin = string.Empty;
        TimeAdjustmentMinutes = 0;
        PricingOverrideAmount = currentCharge;
        Reason = string.Empty;
        SelectedReasonCode = string.Empty;
        HasError = false;
        ErrorMessage = null;
        IsAuthorized = false;
        AuthorizedManagerId = null;
        
        // Update command states
        AuthorizeCommand.NotifyCanExecuteChanged();
        ApplyOverrideCommand.NotifyCanExecuteChanged();
        OnPropertyChanged(nameof(OverrideTypeDisplay));
        OnPropertyChanged(nameof(CurrentReasonCodes));
    }

    private async Task AuthorizeAsync()
    {
        try
        {
            IsLoading = true;
            HasError = false;
            ErrorMessage = null;

            if (string.IsNullOrWhiteSpace(ManagerPin))
            {
                ErrorMessage = "Please enter manager PIN.";
                HasError = true;
                return;
            }

            // For now, use a placeholder user ID - in real implementation, get from current user context
            var userId = Guid.NewGuid(); // TODO: Get from IUserService
            
            var result = await _managerOverrideService.ValidateManagerAuthorizationAsync(ManagerPin, userId);

            if (!result.IsSuccessful)
            {
                ErrorMessage = result.ErrorMessage ?? "Authorization failed.";
                HasError = true;
                ManagerPin = string.Empty; // Clear PIN for security
                return;
            }

            IsAuthorized = true;
            AuthorizedManagerId = userId; // TODO: Get actual manager ID from result
            
            _logger.LogInformation("Manager authorization successful for override type {OverrideType} on session {SessionId}",
                OverrideType, SessionId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to authorize manager for session {SessionId}", SessionId);
            HasError = true;
            ErrorMessage = $"Authorization failed: {ex.Message}";
            ManagerPin = string.Empty; // Clear PIN for security
        }
        finally
        {
            IsLoading = false;
            AuthorizeCommand.NotifyCanExecuteChanged();
            ApplyOverrideCommand.NotifyCanExecuteChanged();
        }
    }

    private async Task ApplyOverrideAsync()
    {
        try
        {
            IsLoading = true;
            HasError = false;
            ErrorMessage = null;

            if (!IsAuthorized || !AuthorizedManagerId.HasValue)
            {
                ErrorMessage = "Manager authorization required.";
                HasError = true;
                return;
            }

            if (string.IsNullOrWhiteSpace(Reason) && string.IsNullOrWhiteSpace(SelectedReasonCode))
            {
                ErrorMessage = "Please provide a reason for the override.";
                HasError = true;
                return;
            }

            var fullReason = string.IsNullOrWhiteSpace(SelectedReasonCode) 
                ? Reason 
                : string.IsNullOrWhiteSpace(Reason) 
                    ? SelectedReasonCode 
                    : $"{SelectedReasonCode}: {Reason}";

            OverrideResult result;

            switch (OverrideType)
            {
                case ManagerOverrideType.TimeAdjustment:
                    if (TimeAdjustmentMinutes == 0)
                    {
                        ErrorMessage = "Please specify a time adjustment amount.";
                        HasError = true;
                        return;
                    }
                    result = await _managerOverrideService.ApplyTimeAdjustmentAsync(
                        SessionId, TimeSpan.FromMinutes(TimeAdjustmentMinutes), fullReason, AuthorizedManagerId.Value);
                    break;

                case ManagerOverrideType.PricingOverride:
                    if (PricingOverrideAmount < 0)
                    {
                        ErrorMessage = "Override amount cannot be negative.";
                        HasError = true;
                        return;
                    }
                    result = await _managerOverrideService.ApplyPricingOverrideAsync(
                        SessionId, new Money(PricingOverrideAmount), fullReason, AuthorizedManagerId.Value);
                    break;

                case ManagerOverrideType.ForceEnd:
                    result = await _managerOverrideService.ForceEndSessionAsync(
                        SessionId, fullReason, AuthorizedManagerId.Value);
                    break;

                default:
                    ErrorMessage = "Invalid override type.";
                    HasError = true;
                    return;
            }

            if (!result.IsSuccessful)
            {
                ErrorMessage = result.ErrorMessage ?? "Override operation failed.";
                HasError = true;
                return;
            }

            _logger.LogInformation("Manager override applied successfully: {OverrideType} for session {SessionId}",
                OverrideType, SessionId);

            // Notify completion
            OverrideCompleted?.Invoke(this, new ManagerOverrideEventArgs(
                SessionId, OverrideType, true, $"{OverrideTypeDisplay} applied successfully"));

            RequestClose?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to apply manager override for session {SessionId}", SessionId);
            HasError = true;
            ErrorMessage = $"Override failed: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
            ApplyOverrideCommand.NotifyCanExecuteChanged();
        }
    }

    private bool CanApplyOverride()
    {
        if (!IsAuthorized) return false;

        return OverrideType switch
        {
            ManagerOverrideType.TimeAdjustment => TimeAdjustmentMinutes != 0,
            ManagerOverrideType.PricingOverride => PricingOverrideAmount >= 0,
            ManagerOverrideType.ForceEnd => true,
            _ => false
        };
    }

    private void Cancel()
    {
        RequestClose?.Invoke(this, EventArgs.Empty);
    }

    partial void OnManagerPinChanged(string value)
    {
        AuthorizeCommand.NotifyCanExecuteChanged();
        if (HasError && !string.IsNullOrWhiteSpace(value))
        {
            HasError = false;
            ErrorMessage = null;
        }
    }

    partial void OnTimeAdjustmentMinutesChanged(int value)
    {
        ApplyOverrideCommand.NotifyCanExecuteChanged();
        OnPropertyChanged(nameof(TimeAdjustmentDisplay));
    }

    partial void OnPricingOverrideAmountChanged(decimal value)
    {
        ApplyOverrideCommand.NotifyCanExecuteChanged();
    }

    partial void OnReasonChanged(string value)
    {
        ApplyOverrideCommand.NotifyCanExecuteChanged();
        if (HasError && (!string.IsNullOrWhiteSpace(value) || !string.IsNullOrWhiteSpace(SelectedReasonCode)))
        {
            HasError = false;
            ErrorMessage = null;
        }
    }

    partial void OnSelectedReasonCodeChanged(string value)
    {
        ApplyOverrideCommand.NotifyCanExecuteChanged();
        if (HasError && (!string.IsNullOrWhiteSpace(value) || !string.IsNullOrWhiteSpace(Reason)))
        {
            HasError = false;
            ErrorMessage = null;
        }
    }
}

/// <summary>
/// Types of manager overrides.
/// </summary>
public enum ManagerOverrideType
{
    TimeAdjustment,
    PricingOverride,
    ForceEnd
}

/// <summary>
/// Event arguments for manager override operations.
/// </summary>
public class ManagerOverrideEventArgs : EventArgs
{
    public Guid SessionId { get; }
    public ManagerOverrideType OverrideType { get; }
    public bool Success { get; }
    public string Message { get; }

    public ManagerOverrideEventArgs(Guid sessionId, ManagerOverrideType overrideType, bool success, string message)
    {
        SessionId = sessionId;
        OverrideType = overrideType;
        Success = success;
        Message = message;
    }
}
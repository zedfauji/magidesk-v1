using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Magidesk.Application.Commands.Security;
using Magidesk.Application.DTOs.Security;
using Magidesk.Application.Interfaces;

namespace Magidesk.Presentation.ViewModels.Dialogs;

/// <summary>
/// ViewModel for Manager PIN authorization dialog.
/// Handles PIN entry, validation, and manager authorization for privileged operations.
/// </summary>
public partial class ManagerPinDialogViewModel : ViewModelBase, IDisposable
{
    private readonly ICommandHandler<AuthorizeManagerCommand, Magidesk.Application.DTOs.Security.AuthorizationResult> _authorizeHandler;

    [ObservableProperty]
    private string _pin = string.Empty;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private string _operationType = string.Empty;

    [ObservableProperty]
    private int _failedAttempts = 0;

    [ObservableProperty]
    private bool _isLocked = false;

    [ObservableProperty]
    private int _lockoutTimeRemaining = 0;

    private Timer? _lockoutTimer;

    public ManagerPinDialogViewModel(
        ICommandHandler<AuthorizeManagerCommand, Magidesk.Application.DTOs.Security.AuthorizationResult> authorizeHandler)
    {
        _authorizeHandler = authorizeHandler;
    }

    /// <summary>
    /// Masked PIN display (e.g., "••••").
    /// </summary>
    public string MaskedPin => new string('•', Pin.Length);

    /// <summary>
    /// Can submit if PIN has at least 1 digit and not locked out.
    /// </summary>
    public bool CanSubmit => !string.IsNullOrEmpty(Pin) && !IsLocked;

    /// <summary>
    /// Gets the lockout status message.
    /// </summary>
    public string LockoutMessage => IsLocked ? $"Too many failed attempts. Try again in {LockoutTimeRemaining} seconds." : string.Empty;

    /// <summary>
    /// Maximum allowed failed attempts before lockout.
    /// </summary>
    private const int MaxFailedAttempts = 3;

    /// <summary>
    /// Lockout duration in seconds.
    /// </summary>
    private const int LockoutDurationSeconds = 30;

    [RelayCommand]
    private void AppendDigit(string? digit)
    {
        if (digit != null && Pin.Length < 10 && !IsLocked) // Limit length and check lockout
        {
            Pin += digit;
            ErrorMessage = string.Empty;
            OnPropertyChanged(nameof(MaskedPin));
            OnPropertyChanged(nameof(CanSubmit));
        }
    }

    [RelayCommand]
    private void Clear()
    {
        if (!IsLocked)
        {
            Pin = string.Empty;
            ErrorMessage = string.Empty;
            OnPropertyChanged(nameof(MaskedPin));
            OnPropertyChanged(nameof(CanSubmit));
        }
    }

    [RelayCommand]
    private void RemoveLastDigit()
    {
        if (!string.IsNullOrEmpty(Pin) && !IsLocked)
        {
            Pin = Pin.Substring(0, Pin.Length - 1);
            ErrorMessage = string.Empty;
            OnPropertyChanged(nameof(MaskedPin));
            OnPropertyChanged(nameof(CanSubmit));
        }
    }

    /// <summary>
    /// Authorizes the manager PIN and returns the result.
    /// </summary>
    [RelayCommand]
    public async Task<Magidesk.Application.DTOs.Security.AuthorizationResult?> AuthorizeAsync()
    {
        if (string.IsNullOrEmpty(Pin))
        {
            ErrorMessage = "Please enter PIN.";
            return null;
        }

        if (IsLocked)
        {
            ErrorMessage = LockoutMessage;
            return null;
        }

        IsBusy = true;
        ErrorMessage = string.Empty;

        try
        {
            var command = new AuthorizeManagerCommand(Pin, OperationType);
            var result = await _authorizeHandler.HandleAsync(command);

            if (!result.Authorized)
            {
                // Increment failed attempts
                FailedAttempts++;
                
                // Check if we should lock out
                if (FailedAttempts >= MaxFailedAttempts)
                {
                    StartLockout();
                    ErrorMessage = LockoutMessage;
                }
                else
                {
                    var remainingAttempts = MaxFailedAttempts - FailedAttempts;
                    ErrorMessage = $"{result.FailureReason ?? "Invalid PIN"}. {remainingAttempts} attempt{(remainingAttempts == 1 ? "" : "s")} remaining.";
                }
                
                // Clear PIN for security
                Pin = string.Empty;
                OnPropertyChanged(nameof(MaskedPin));
                OnPropertyChanged(nameof(CanSubmit));
                return null;
            }

            // Success - reset failed attempts
            FailedAttempts = 0;
            return result;
        }
        catch (Exception ex)
        {
            // Increment failed attempts for system errors too
            FailedAttempts++;
            
            if (FailedAttempts >= MaxFailedAttempts)
            {
                StartLockout();
                ErrorMessage = LockoutMessage;
            }
            else
            {
                ErrorMessage = $"System error: {ex.Message}";
            }
            
            Pin = string.Empty;
            OnPropertyChanged(nameof(MaskedPin));
            OnPropertyChanged(nameof(CanSubmit));
            return null;
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Starts the lockout timer.
    /// </summary>
    private void StartLockout()
    {
        IsLocked = true;
        LockoutTimeRemaining = LockoutDurationSeconds;
        OnPropertyChanged(nameof(LockoutMessage));
        OnPropertyChanged(nameof(CanSubmit));

        _lockoutTimer = new Timer(OnLockoutTick, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
    }

    /// <summary>
    /// Handles lockout timer tick.
    /// </summary>
    private void OnLockoutTick(object? state)
    {
        LockoutTimeRemaining--;
        OnPropertyChanged(nameof(LockoutMessage));

        if (LockoutTimeRemaining <= 0)
        {
            // End lockout
            _lockoutTimer?.Dispose();
            _lockoutTimer = null;
            IsLocked = false;
            FailedAttempts = 0;
            ErrorMessage = string.Empty;
            OnPropertyChanged(nameof(CanSubmit));
            OnPropertyChanged(nameof(LockoutMessage));
        }
    }

    /// <summary>
    /// Resets the authorization state.
    /// </summary>
    public void Reset()
    {
        Pin = string.Empty;
        ErrorMessage = string.Empty;
        FailedAttempts = 0;
        IsLocked = false;
        LockoutTimeRemaining = 0;
        _lockoutTimer?.Dispose();
        _lockoutTimer = null;
        
        OnPropertyChanged(nameof(MaskedPin));
        OnPropertyChanged(nameof(CanSubmit));
        OnPropertyChanged(nameof(LockoutMessage));
    }

    /// <summary>
    /// Dispose resources.
    /// </summary>
    public void Dispose()
    {
        _lockoutTimer?.Dispose();
    }
}

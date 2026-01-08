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
public partial class ManagerPinDialogViewModel : ViewModelBase
{
    private readonly ICommandHandler<AuthorizeManagerCommand, Magidesk.Application.DTOs.Security.AuthorizationResult> _authorizeHandler;

    [ObservableProperty]
    private string _pin = string.Empty;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private string _operationType = string.Empty;

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
    /// Can submit if PIN has at least 1 digit.
    /// </summary>
    public bool CanSubmit => !string.IsNullOrEmpty(Pin);

    [RelayCommand]
    private void AppendDigit(string? digit)
    {
        if (digit != null && Pin.Length < 10) // Limit length
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
        Pin = string.Empty;
        ErrorMessage = string.Empty;
        OnPropertyChanged(nameof(MaskedPin));
        OnPropertyChanged(nameof(CanSubmit));
    }

    [RelayCommand]
    private void RemoveLastDigit()
    {
        if (!string.IsNullOrEmpty(Pin))
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

        IsBusy = true;
        ErrorMessage = string.Empty;

        try
        {
            var command = new AuthorizeManagerCommand(Pin, OperationType);
            var result = await _authorizeHandler.HandleAsync(command);

            if (!result.Authorized)
            {
                // Show error and clear PIN
                ErrorMessage = result.FailureReason ?? "Authorization failed.";
                Pin = string.Empty;
                OnPropertyChanged(nameof(MaskedPin));
                OnPropertyChanged(nameof(CanSubmit));
                return null;
            }

            // Success - return result
            return result;
        }
        catch (Exception ex)
        {
            // Surface error to UI via ErrorMessage property to avoid dialog conflict
            ErrorMessage = $"System error: {ex.Message}";
            
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
}

using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Magidesk.Application.Interfaces;
using Magidesk.Presentation.Services;

namespace Magidesk.Presentation.ViewModels;

public class LoginViewModel : ViewModelBase
{
    private readonly ISecurityService _securityService;
    private readonly IAesEncryptionService _encryptionService;
    private readonly NavigationService _navigationService;

    private string _pin = string.Empty;
    private string _errorMessage = string.Empty;

    public LoginViewModel(
        ISecurityService securityService,
        IAesEncryptionService encryptionService,
        NavigationService navigationService)
    {
        _securityService = securityService;
        _encryptionService = encryptionService;
        _navigationService = navigationService;

        AppendDigitCommand = new RelayCommand<string>(AppendDigit);
        ClearCommand = new RelayCommand(Clear);
        LoginCommand = new AsyncRelayCommand(LoginAsync);
        ShutdownCommand = new RelayCommand(Shutdown);
    }

    public string Pin
    {
        get => _pin;
        set
        {
            if (SetProperty(ref _pin, value))
            {
                OnPropertyChanged(nameof(MaskedPin));
            }
        }
    }

    public string MaskedPin => new string('•', Pin.Length);

    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }


    public ICommand AppendDigitCommand { get; }
    public ICommand ClearCommand { get; }
    public ICommand LoginCommand { get; }
    public ICommand ShutdownCommand { get; }

    private void AppendDigit(string? digit)
    {
        if (digit != null && Pin.Length < 10) // Limit length
        {
            Pin += digit;
            ErrorMessage = string.Empty;
        }
    }

    private void Clear()
    {
        Pin = string.Empty;
        ErrorMessage = string.Empty;
    }

    private async Task LoginAsync()
    {
        if (string.IsNullOrEmpty(Pin))
        {
            ErrorMessage = "Please enter PIN.";
            return;
        }

        IsBusy = true;
        ErrorMessage = string.Empty;

        try
        {
            var encryptedPin = _encryptionService.Encrypt(Pin);
            var user = await _securityService.GetUserByPinAsync(encryptedPin);

            if (user != null)
            {
                // Login Success
                // F-0002 Integration: Update Main Window Status
                if (App.MainWindowInstance is MainWindow mainWindow)
                {
                    mainWindow.SetUser($"{user.FirstName} {user.LastName}");
                }
                
                // Navigate to Switchboard
                _navigationService.Navigate(typeof(Views.SwitchboardPage));
                Pin = string.Empty; // Reset for next time (logout)
            }
            else
            {
                ErrorMessage = "Invalid PIN.";
                Pin = string.Empty;
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Login Error: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void Shutdown()
    {
        App.Current.Exit();
    }
}

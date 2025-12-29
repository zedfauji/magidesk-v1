using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Commands;
using Magidesk.Presentation.Services;
using Microsoft.UI.Xaml.Controls;

namespace Magidesk.Presentation.ViewModels;

public class LoginViewModel : ViewModelBase
{
    private readonly ISecurityService _securityService;
    private readonly IAesEncryptionService _encryptionService;
    private readonly NavigationService _navigationService;
    private readonly IUserService _userService;
    private readonly ICommandHandler<ClockInCommand> _clockInHandler;
    private readonly ICommandHandler<ClockOutCommand> _clockOutHandler;
    private readonly IAttendanceRepository _attendanceRepository;

    private string _pin = string.Empty;
    private string _errorMessage = string.Empty;

    public LoginViewModel(
        ISecurityService securityService,
        IAesEncryptionService encryptionService,
        NavigationService navigationService,
        IUserService userService,
        ICommandHandler<ClockInCommand> clockInHandler,
        ICommandHandler<ClockOutCommand> clockOutHandler,
        IAttendanceRepository attendanceRepository)
    {
        _securityService = securityService;
        _encryptionService = encryptionService;
        _navigationService = navigationService;
        _userService = userService;
        _clockInHandler = clockInHandler;
        _clockOutHandler = clockOutHandler;
        _attendanceRepository = attendanceRepository;

        AppendDigitCommand = new RelayCommand<string>(AppendDigit);
        ClearCommand = new RelayCommand(Clear);
        LoginCommand = new AsyncRelayCommand(LoginAsync);
        ShutdownCommand = new RelayCommand(Shutdown);
        ClockInOutCommand = new AsyncRelayCommand(ClockInOutAsync);
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
    public ICommand ClockInOutCommand { get; }

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

    private async Task ClockInOutAsync()
    {
        if (string.IsNullOrEmpty(Pin))
        {
            ErrorMessage = "Enter PIN to Clock In/Out.";
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
                var openAttendance = await _attendanceRepository.GetOpenByUserIdAsync(user.Id);
                string message;

                if (openAttendance != null)
                {
                    await _clockOutHandler.HandleAsync(new ClockOutCommand { UserId = user.Id });
                    message = $"Goodbye, {user.FirstName}. Clocked OUT.";
                }
                else
                {
                    await _clockInHandler.HandleAsync(new ClockInCommand { UserId = user.Id });
                    message = $"Welcome, {user.FirstName}. Clocked IN.";
                }

                var dialog = new ContentDialog
                {
                    Title = "Attendance",
                    Content = message,
                    CloseButtonText = "OK",
                    XamlRoot = App.MainWindowInstance.Content.XamlRoot
                };
                await _navigationService.ShowDialogAsync(dialog);
                
                Pin = string.Empty;
            }
            else
            {
                ErrorMessage = "Invalid PIN.";
                Pin = string.Empty;
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
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
                // Set Current User
                // Set Current User
                _userService.CurrentUser = new Magidesk.Application.DTOs.UserDto 
                { 
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Username = user.Username
                };

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

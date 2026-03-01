using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Commands;
using Magidesk.Application.DTOs;
using Magidesk.Presentation.Services;
using Microsoft.UI.Xaml.Controls;
using CommunityToolkit.Mvvm.Input;

namespace Magidesk.Presentation.ViewModels;

public class LoginViewModel : ViewModelBase
{
    private readonly ISecurityService _securityService;
    private readonly IAesEncryptionService _encryptionService;
    private readonly NavigationService _navigationService;
    private readonly IUserService _userService;
    private readonly IUserRepository _userRepository;
    private readonly ICommandHandler<ClockInCommand> _clockInHandler;
    private readonly ICommandHandler<ClockOutCommand> _clockOutHandler;
    private readonly IAttendanceRepository _attendanceRepository;
    private readonly IDefaultViewRoutingService _defaultViewRoutingService;
    private readonly ITerminalContext _terminalContext;

    private string _pin = string.Empty;
    private string _errorMessage = string.Empty;
    private UserDto? _selectedUser;

    private readonly IServiceProvider _serviceProvider;

    public LoginViewModel(
        ISecurityService securityService,
        IAesEncryptionService encryptionService,
        NavigationService navigationService,
        IUserService userService,
        IUserRepository userRepository,
        ICommandHandler<ClockInCommand> clockInHandler,
        ICommandHandler<ClockOutCommand> clockOutHandler,
        IAttendanceRepository attendanceRepository,
        IDefaultViewRoutingService defaultViewRoutingService,
        ITerminalContext terminalContext,
        IServiceProvider serviceProvider,
        Services.LocalizationService localizationService)
    {
        _securityService = securityService;
        _encryptionService = encryptionService;
        _navigationService = navigationService;
        _userService = userService;
        _userRepository = userRepository;
        _clockInHandler = clockInHandler;
        _clockOutHandler = clockOutHandler;
        _attendanceRepository = attendanceRepository;
        _defaultViewRoutingService = defaultViewRoutingService;
        _terminalContext = terminalContext;
        _serviceProvider = serviceProvider;
        Localization = localizationService;

        Users = new ObservableCollection<UserDto>();

        AppendDigitCommand = new RelayCommand<string>(AppendDigit);
        ClearCommand = new RelayCommand(Clear);
        RemoveLastDigitCommand = new RelayCommand(RemoveLastDigit);
        LoginCommand = new AsyncRelayCommand(LoginAsync);
        ShutdownCommand = new RelayCommand(Shutdown);
        ClockInOutCommand = new AsyncRelayCommand(ClockInOutAsync);
        ChangeLanguageCommand = new AsyncRelayCommand(ChangeLanguageAsync);
        SelectUserCommand = new RelayCommand<UserDto>(SelectUser);
        LoadUsersCommand = new AsyncRelayCommand(LoadUsersAsync);
    }

    public Services.LocalizationService Localization { get; }

    /// <summary>
    /// Terminal ID for display
    /// </summary>
    public string TerminalId => $"Terminal: {_terminalContext.TerminalIdentity ?? "POS-01"}";

    /// <summary>
    /// Collection of all active users available for login
    /// </summary>
    public ObservableCollection<UserDto> Users { get; }

    /// <summary>
    /// Currently selected user for login
    /// </summary>
    public UserDto? SelectedUser
    {
        get => _selectedUser;
        set
        {
            if (SetProperty(ref _selectedUser, value))
            {
                // Clear PIN when user selection changes
                Pin = string.Empty;
                ErrorMessage = string.Empty;
            }
        }
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
    public ICommand RemoveLastDigitCommand { get; }
    public ICommand LoginCommand { get; }
    public ICommand ShutdownCommand { get; }
    public ICommand ClockInOutCommand { get; }
    public ICommand ChangeLanguageCommand { get; }
    public ICommand SelectUserCommand { get; }
    public ICommand LoadUsersCommand { get; }

    /// <summary>
    /// Loads all active users from the repository
    /// </summary>
    private async Task LoadUsersAsync()
    {
        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            var users = await _userRepository.GetAllAsync();
            
            Users.Clear();
            foreach (var user in users.Where(u => u.IsActive).OrderBy(u => u.FirstName))
            {
                Users.Add(new UserDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Username = user.Username,
                    IsActive = user.IsActive,
                    PreferredLanguage = user.PreferredLanguage,
                    RoleName = user.Role?.Name ?? "Unknown"
                });
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to load users: {ex.Message}";
            System.Diagnostics.Debug.WriteLine($"Error loading users: {ex}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Selects a user for login
    /// </summary>
    private void SelectUser(UserDto? user)
    {
        if (user != null)
        {
            SelectedUser = user;
            // Focus should move to PIN entry in the UI
        }
    }

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

    private void RemoveLastDigit()
    {
        if (!string.IsNullOrEmpty(Pin))
        {
            Pin = Pin.Substring(0, Pin.Length - 1);
            ErrorMessage = string.Empty;
        }
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
             // T-003: Visible Failure
             var errorDialog = new ContentDialog
             {
                 Title = "Clock In/Out Error",
                 Content = $"System Error:\n{ex.Message}",
                 CloseButtonText = "OK",
                 XamlRoot = App.MainWindowInstance.Content.XamlRoot
             };
             await _navigationService.ShowDialogAsync(errorDialog);
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
                // If a user was selected, verify the PIN matches the selected user
                if (SelectedUser != null && user.Id != SelectedUser.Id)
                {
                    ErrorMessage = "Invalid PIN for selected user.";
                    Pin = string.Empty;
                    return;
                }

                // Set Current User
                _userService.CurrentUser = new Magidesk.Application.DTOs.UserDto 
                { 
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Username = user.Username,
                    TerminalId = _terminalContext.TerminalId,
                    PreferredLanguage = user.PreferredLanguage,
                    RoleName = user.Role?.Name ?? "Unknown"
                };

                // Login Success
                // F-0002 Integration: Update Main Window Status
                if (App.MainWindowInstance is MainWindow mainWindow)
                {
                    mainWindow.SetUser($"{user.FirstName} {user.LastName}");
                }
                
                // Navigate to default view based on terminal configuration (FloreantPOS-aligned)
                try
                {
                    var defaultViewType = await _defaultViewRoutingService.GetDefaultViewTypeAsync(_userService.CurrentUser?.TerminalId);
                    _navigationService.Navigate(defaultViewType);
                }
                catch (Exception routingEx)
                {
                    // T-005: Visible Failure
                    // Fallback to SwitchboardPage if routing fails
                    _navigationService.Navigate(typeof(Views.SwitchboardPage));
                    // Toast/Non-blocking warning
                    await _navigationService.ShowMessageAsync("Navigation Alert", $"Could not load default view ({routingEx.Message}). Sending to Switchboard.");
                }
                Pin = string.Empty; // Reset for next time (logout)
                SelectedUser = null; // Clear selection
            }
            else
            {
                ErrorMessage = "Invalid PIN.";
                Pin = string.Empty;
            }
        }
        catch (Exception ex)
        {
             // T-002: Visible Failure
             var errorDialog = new ContentDialog
             {
                 Title = "Login Failed",
                 Content = $"System Critical Error:\n{ex.Message}\n\nTerminals cannot authenticate if database is down.",
                 CloseButtonText = "OK",
                 XamlRoot = App.MainWindowInstance.Content.XamlRoot
             };
             await _navigationService.ShowDialogAsync(errorDialog);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task ChangeLanguageAsync()
    {
        try 
        {
            var dialog = Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<Views.LanguageSelectionDialog>(_serviceProvider);
            dialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot;
            await dialog.ShowAsync();
            // Note: Update of UI resources would happen here or via event
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error showing language dialog: {ex}");
        }
    }

    private void Shutdown()
    {
        App.Current.Exit();
    }
}

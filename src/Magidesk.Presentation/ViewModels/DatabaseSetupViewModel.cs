using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Microsoft.Extensions.Logging;
using System.Threading;

namespace Magidesk.Presentation.ViewModels;

public partial class DatabaseSetupViewModel : ObservableObject
{
    private readonly IDatabaseConfigurationService _configService;
    private readonly IDatabaseSeedingService _seedingService;
    private readonly ILogger<DatabaseSetupViewModel> _logger;
    private CancellationTokenSource? _seedingCancellation;

    public DatabaseSetupViewModel(
        IDatabaseConfigurationService configService,
        IDatabaseSeedingService seedingService,
        ILogger<DatabaseSetupViewModel> logger)
    {
        _configService = configService;
        _seedingService = seedingService;
        _logger = logger;

        // Subscribe to seeding progress
        _seedingService.ProgressChanged += OnSeedingProgressChanged;

        // Set defaults
        Host = "localhost";
        Port = 5432;
    }

    #region Properties

    [ObservableProperty]
    private string _host = "localhost";

    [ObservableProperty]
    private int _port = 5432;

    [ObservableProperty]
    private string _databaseName = string.Empty;

    [ObservableProperty]
    private string _username = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    private bool _isTestingConnection;

    [ObservableProperty]
    private bool _isSeedingDatabase;

    [ObservableProperty]
    private string _statusMessage = "Enter your database connection details to begin.";

    [ObservableProperty]
    private string _seedingProgressMessage = string.Empty;

    [ObservableProperty]
    private int _seedingProgressPercent;

    [ObservableProperty]
    private bool _connectionTestPassed;

    [ObservableProperty]
    private bool _seedingCompleted;

    [ObservableProperty]
    private bool _hasError;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    public bool CanTestConnection => !IsTestingConnection && !IsSeedingDatabase && 
                                     !string.IsNullOrWhiteSpace(Host) &&
                                     Port > 0 &&
                                     !string.IsNullOrWhiteSpace(DatabaseName) &&
                                     !string.IsNullOrWhiteSpace(Username) &&
                                     !string.IsNullOrWhiteSpace(Password);

    public bool CanSaveConfiguration => ConnectionTestPassed && !IsSeedingDatabase;

    public bool CanRunSetup => ConnectionTestPassed && !IsSeedingDatabase;

    public bool CanContinueToApp => SeedingCompleted;

    #endregion

    #region Commands

    [RelayCommand(CanExecute = nameof(CanTestConnection))]
    private async Task TestConnectionAsync()
    {
        try
        {
            IsTestingConnection = true;
            HasError = false;
            StatusMessage = "Testing database connection...";

            var config = new DatabaseConfig
            {
                Host = Host,
                Port = Port,
                DatabaseName = DatabaseName,
                Username = Username,
                Password = Password
            };

            var result = await _configService.TestConnectionAsync(config);

            if (result.Success)
            {
                ConnectionTestPassed = true;
                StatusMessage = "✓ Connection successful! You can now save the configuration and run database setup.";
                _logger.LogInformation("Database connection test passed");
            }
            else
            {
                ConnectionTestPassed = false;
                HasError = true;
                ErrorMessage = result.Message;
                StatusMessage = "✗ Connection failed. Please check your settings.";
                _logger.LogWarning("Database connection test failed: {Message}", result.Message);
            }
        }
        catch (Exception ex)
        {
            ConnectionTestPassed = false;
            HasError = true;
            ErrorMessage = $"Unexpected error: {ex.Message}";
            StatusMessage = "✗ Connection test failed.";
            _logger.LogError(ex, "Error during connection test");
        }
        finally
        {
            IsTestingConnection = false;
            TestConnectionCommand.NotifyCanExecuteChanged();
            SaveConfigurationCommand.NotifyCanExecuteChanged();
            RunDatabaseSetupCommand.NotifyCanExecuteChanged();
        }
    }

    [RelayCommand(CanExecute = nameof(CanSaveConfiguration))]
    private async Task SaveConfigurationAsync()
    {
        try
        {
            var config = new DatabaseConfig
            {
                Host = Host,
                Port = Port,
                DatabaseName = DatabaseName,
                Username = Username,
                Password = Password
            };

            await _configService.SaveConfigurationAsync(config);
            StatusMessage = "✓ Configuration saved securely.";
            _logger.LogInformation("Database configuration saved");
        }
        catch (Exception ex)
        {
            HasError = true;
            ErrorMessage = $"Failed to save configuration: {ex.Message}";
            _logger.LogError(ex, "Error saving configuration");
        }
    }

    [RelayCommand(CanExecute = nameof(CanRunSetup))]
    private async Task RunDatabaseSetupAsync()
    {
        try
        {
            IsSeedingDatabase = true;
            HasError = false;
            SeedingCompleted = false;
            SeedingProgressPercent = 0;
            StatusMessage = "Running database setup...";

            // Save configuration first
            await SaveConfigurationAsync();

            // Run seeding
            _seedingCancellation = new CancellationTokenSource();
            var result = await _seedingService.SeedDatabaseAsync(_seedingCancellation.Token);

            if (result.Success)
            {
                SeedingCompleted = true;
                StatusMessage = "✓ Database setup complete! Click 'Continue to Application' to proceed.";
                _logger.LogInformation("Database seeding completed successfully");
            }
            else
            {
                HasError = true;
                ErrorMessage = result.Message;
                StatusMessage = "✗ Database setup failed. Please check the error and try again.";
                _logger.LogError("Database seeding failed: {Message}", result.Message);
            }
        }
        catch (Exception ex)
        {
            HasError = true;
            ErrorMessage = $"Setup failed: {ex.Message}";
            StatusMessage = "✗ Database setup failed.";
            _logger.LogError(ex, "Error during database setup");
        }
        finally
        {
            IsSeedingDatabase = false;
            _seedingCancellation?.Dispose();
            _seedingCancellation = null;
            
            RunDatabaseSetupCommand.NotifyCanExecuteChanged();
            ContinueToAppCommand.NotifyCanExecuteChanged();
        }
    }

    [RelayCommand(CanExecute = nameof(CanContinueToApp))]
    private void ContinueToApp()
    {
        // This will be handled by the view to navigate to main app
        _logger.LogInformation("User continuing to application after successful setup");
    }

    #endregion

    #region Event Handlers

    private void OnSeedingProgressChanged(object? sender, SeedingProgressEventArgs e)
    {
        SeedingProgressMessage = e.Message;
        SeedingProgressPercent = e.PercentComplete;
    }

    partial void OnHostChanged(string value) => TestConnectionCommand.NotifyCanExecuteChanged();
    partial void OnPortChanged(int value) => TestConnectionCommand.NotifyCanExecuteChanged();
    partial void OnDatabaseNameChanged(string value) => TestConnectionCommand.NotifyCanExecuteChanged();
    partial void OnUsernameChanged(string value) => TestConnectionCommand.NotifyCanExecuteChanged();
    partial void OnPasswordChanged(string value) => TestConnectionCommand.NotifyCanExecuteChanged();

    #endregion
}

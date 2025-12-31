using Magidesk.Application.Commands;
using Magidesk.Application.Commands.SystemConfig;
using Magidesk.Application.DTOs.SystemConfig;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries;
using Magidesk.Application.Queries.SystemConfig;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;

namespace Magidesk.Presentation.ViewModels;

public partial class SystemConfigViewModel : ViewModelBase
{
    private readonly IQueryHandler<GetSystemBackupsQuery, GetSystemBackupsResult> _getBackups;
    private readonly ICommandHandler<CreateSystemBackupCommand, CreateSystemBackupResult> _createBackup;
    private readonly ICommandHandler<RestoreSystemBackupCommand, RestoreSystemBackupResult> _restoreBackup;

    public ObservableCollection<BackupInfoDto> Backups { get; } = new();

    private string _statusMessage = string.Empty;
    public string StatusMessage
    {
        get => _statusMessage;
        set { SetProperty(ref _statusMessage, value); OnPropertyChanged(nameof(HasStatusMessage)); }
    }

    public bool HasStatusMessage => !string.IsNullOrEmpty(StatusMessage);

    private bool _isRestoring;
    public bool IsRestoring
    {
        get => _isRestoring;
        set => SetProperty(ref _isRestoring, value);
    }

    public AsyncRelayCommand LoadBackupsCommand { get; }
    public AsyncRelayCommand CreateBackupCommand { get; }
    public AsyncRelayCommand<string> RestoreBackupCommand { get; }

    public SystemConfigViewModel(
        IQueryHandler<GetSystemBackupsQuery, GetSystemBackupsResult> getBackups,
        ICommandHandler<CreateSystemBackupCommand, CreateSystemBackupResult> createBackup,
        ICommandHandler<RestoreSystemBackupCommand, RestoreSystemBackupResult> restoreBackup)
    {
        _getBackups = getBackups;
        _createBackup = createBackup;
        _restoreBackup = restoreBackup;

        Title = "System Configuration";
        
        LoadBackupsCommand = new AsyncRelayCommand(LoadBackupsAsync);
        CreateBackupCommand = new AsyncRelayCommand(CreateBackupAsync);
        RestoreBackupCommand = new AsyncRelayCommand<string>(RestoreBackupAsync, _ => !IsRestoring);
    }

    public async Task InitializeAsync()
    {
        await LoadBackupsAsync();
    }

    private async Task LoadBackupsAsync()
    {
        IsBusy = true;
        StatusMessage = string.Empty;
        try
        {
            var result = await _getBackups.HandleAsync(new GetSystemBackupsQuery());
            Backups.Clear();
            foreach (var backup in result.Backups)
            {
                Backups.Add(backup);
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error loading backups: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task CreateBackupAsync()
    {
        IsBusy = true;
        StatusMessage = "Creating backup...";
        try
        {
            var result = await _createBackup.HandleAsync(new CreateSystemBackupCommand());
            StatusMessage = $"Backup created successfully: {result.FileName}";
            await LoadBackupsAsync();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Backup failed: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task RestoreBackupAsync(string? fileName)
    {
        if (string.IsNullOrEmpty(fileName)) return;

        var dialog = new Microsoft.UI.Xaml.Controls.ContentDialog
        {
            Title = "Restore Backup",
            Content = $"Restore backup '{fileName}'? This will overwrite current data.",
            PrimaryButtonText = "Restore",
            CloseButtonText = "Cancel",
            DefaultButton = Microsoft.UI.Xaml.Controls.ContentDialogButton.Close,
            XamlRoot = App.MainWindowInstance.Content.XamlRoot
        };

        var confirmation = await dialog.ShowAsync();
        if (confirmation != Microsoft.UI.Xaml.Controls.ContentDialogResult.Primary)
        {
            return;
        }
        
        IsRestoring = true;
        IsBusy = true;
        StatusMessage = $"Resulting backup: {fileName}...";
        
        try
        {
            await _restoreBackup.HandleAsync(new RestoreSystemBackupCommand(fileName));
            StatusMessage = "Restore completed successfully. Please restart the application.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Restore failed: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
            IsRestoring = false;
        }
    }
}

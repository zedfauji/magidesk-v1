using Magidesk.Application.Commands;
using Magidesk.Application.Commands.SystemConfig;
using Magidesk.Application.DTOs.SystemConfig;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries;
using Magidesk.Application.Queries.SystemConfig;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using Magidesk.Presentation.Services;

namespace Magidesk.Presentation.ViewModels;

public partial class SystemConfigViewModel : ViewModelBase
{
    private readonly IQueryHandler<GetSystemBackupsQuery, GetSystemBackupsResult> _getBackups;
    private readonly ICommandHandler<CreateSystemBackupCommand, CreateSystemBackupResult> _createBackup;
    private readonly ICommandHandler<RestoreSystemBackupCommand, RestoreSystemBackupResult> _restoreBackup;
    private readonly IQueryHandler<GetRestaurantConfigQuery, GetRestaurantConfigResult> _getConfig;
    private readonly ICommandHandler<UpdateRestaurantConfigCommand, UpdateRestaurantConfigResult> _updateConfig;
    private readonly IQueryHandler<GetTerminalConfigQuery, GetTerminalConfigResult> _getTerminalConfig;
    private readonly ICommandHandler<UpdateTerminalConfigCommand, UpdateTerminalConfigResult> _updateTerminalConfig;
    private readonly IQueryHandler<GetCardConfigQuery, GetCardConfigResult> _getCardConfig;
    private readonly ICommandHandler<UpdateCardConfigCommand, UpdateCardConfigResult> _updateCardConfig;
    private readonly IQueryHandler<GetPrinterGroupsQuery, GetPrinterGroupsResult> _getPrinterGroups;
    private readonly IQueryHandler<GetPrinterMappingsQuery, GetPrinterMappingsResult> _getPrinterMappings;
    private readonly ICommandHandler<UpdatePrinterGroupsCommand, UpdatePrinterGroupsResult> _updatePrinterGroups;
    private readonly ICommandHandler<UpdatePrinterMappingsCommand, UpdatePrinterMappingsResult> _updatePrinterMappings;
    private readonly IPrintingService _printingService;
    private readonly ITerminalContext _terminalContext;

    public ObservableCollection<BackupInfoDto> Backups { get; } = new();
    public ObservableCollection<PrinterGroupDto> PrinterGroups { get; } = new();
    public ObservableCollection<PrinterMappingDto> PrinterMappings { get; } = new();
    public ObservableCollection<string> SystemPrinters { get; } = new();

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
    
    private RestaurantConfigurationDto _configuration;
    public RestaurantConfigurationDto Configuration
    {
        get => _configuration;
        set => SetProperty(ref _configuration, value);
    }

    private TerminalDto _terminalConfiguration;
    public TerminalDto TerminalConfiguration
    {
        get => _terminalConfiguration;
        set => SetProperty(ref _terminalConfiguration, value);
    }

    private CardConfigDto _cardConfiguration;
    public CardConfigDto CardConfiguration
    {
        get => _cardConfiguration;
        set => SetProperty(ref _cardConfiguration, value);
    }

    public AsyncRelayCommand LoadBackupsCommand { get; }
    public AsyncRelayCommand CreateBackupCommand { get; }
    public AsyncRelayCommand<string> RestoreBackupCommand { get; }
    public AsyncRelayCommand SaveConfigurationCommand { get; }
    public AsyncRelayCommand SaveTerminalConfigurationCommand { get; }
    public AsyncRelayCommand SaveCardConfigurationCommand { get; }
    public AsyncRelayCommand SavePrintersCommand { get; }

    public SystemConfigViewModel(
        IQueryHandler<GetSystemBackupsQuery, GetSystemBackupsResult> getBackups,
        ICommandHandler<CreateSystemBackupCommand, CreateSystemBackupResult> createBackup,
        ICommandHandler<RestoreSystemBackupCommand, RestoreSystemBackupResult> restoreBackup,
        IQueryHandler<GetRestaurantConfigQuery, GetRestaurantConfigResult> getConfig,
        ICommandHandler<UpdateRestaurantConfigCommand, UpdateRestaurantConfigResult> updateConfig,
        IQueryHandler<GetTerminalConfigQuery, GetTerminalConfigResult> getTerminalConfig,
        ICommandHandler<UpdateTerminalConfigCommand, UpdateTerminalConfigResult> updateTerminalConfig,
        IQueryHandler<GetCardConfigQuery, GetCardConfigResult> getCardConfig,
        ICommandHandler<UpdateCardConfigCommand, UpdateCardConfigResult> updateCardConfig,
        IQueryHandler<GetPrinterGroupsQuery, GetPrinterGroupsResult> getPrinterGroups,
        IQueryHandler<GetPrinterMappingsQuery, GetPrinterMappingsResult> getPrinterMappings,
        ICommandHandler<UpdatePrinterGroupsCommand, UpdatePrinterGroupsResult> updatePrinterGroups,
        ICommandHandler<UpdatePrinterMappingsCommand, UpdatePrinterMappingsResult> updatePrinterMappings,
        IPrintingService printingService,
        ITerminalContext terminalContext)
    {
        _getBackups = getBackups;
        _createBackup = createBackup;
        _restoreBackup = restoreBackup;
        _getConfig = getConfig;
        _updateConfig = updateConfig;
        _getTerminalConfig = getTerminalConfig;
        _updateTerminalConfig = updateTerminalConfig;
        _getCardConfig = getCardConfig;
        _updateCardConfig = updateCardConfig;
        _getPrinterGroups = getPrinterGroups;
        _getPrinterMappings = getPrinterMappings;
        _updatePrinterGroups = updatePrinterGroups;
        _updatePrinterMappings = updatePrinterMappings;
        _printingService = printingService;
        _terminalContext = terminalContext;

        Title = "System Configuration";
        _terminalConfiguration = new TerminalDto(); // Initialize to avoid null binding issues
        _cardConfiguration = new CardConfigDto(); // Initialize to avoid null binding issues
        LoadBackupsCommand = new AsyncRelayCommand(LoadBackupsAsync);
        CreateBackupCommand = new AsyncRelayCommand(CreateBackupAsync);
        RestoreBackupCommand = new AsyncRelayCommand<string>(RestoreBackupAsync, _ => !IsRestoring);
        SaveConfigurationCommand = new AsyncRelayCommand(SaveConfigurationAsync);
        SaveTerminalConfigurationCommand = new AsyncRelayCommand(SaveTerminalConfigurationAsync);
        SaveCardConfigurationCommand = new AsyncRelayCommand(SaveCardConfigurationAsync);
        SavePrintersCommand = new AsyncRelayCommand(SavePrintersAsync);
    }

    public async Task InitializeAsync()
    {
        await LoadBackupsAsync();
        await LoadConfigurationAsync();
        await LoadTerminalConfigurationAsync();
        await LoadCardConfigurationAsync();
        await LoadPrintersAsync();
    }

    private async Task LoadConfigurationAsync()
    {
        try
        {
            var result = await _getConfig.HandleAsync(new GetRestaurantConfigQuery());
            Configuration = result.Configuration;
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error loading configuration: {ex.Message}";
        }
    }

    private async Task LoadTerminalConfigurationAsync()
    {
        try
        {
            var terminalKey = _terminalContext.TerminalIdentity ?? Environment.MachineName;
            var result = await _getTerminalConfig.HandleAsync(new GetTerminalConfigQuery(terminalKey));
            TerminalConfiguration = result.Terminal;
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error loading terminal configuration: {ex.Message}";
        }
    }

    private async Task LoadCardConfigurationAsync()
    {
        try
        {
            var terminalId = _terminalContext.TerminalId;
            if (!terminalId.HasValue) return;

            var result = await _getCardConfig.HandleAsync(new GetCardConfigQuery(terminalId.Value));
            CardConfiguration = result.Config;
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error loading card configuration: {ex.Message}";
        }
    }

    private async Task LoadPrintersAsync()
    {
        try
        {
            // Load system printers
            var systemPrinters = await _printingService.GetSystemPrintersAsync();
            SystemPrinters.Clear();
            foreach (var printer in systemPrinters)
            {
                SystemPrinters.Add(printer);
            }

            // Load printer groups
            var groupsResult = await _getPrinterGroups.HandleAsync(new GetPrinterGroupsQuery());
            PrinterGroups.Clear();
            foreach (var group in groupsResult.Groups)
            {
                PrinterGroups.Add(group);
            }

            // Load printer mappings
            var terminalId = _terminalContext.TerminalId;
            if (terminalId.HasValue)
            {
                var mappingsResult = await _getPrinterMappings.HandleAsync(new GetPrinterMappingsQuery(terminalId.Value));
                PrinterMappings.Clear();
                foreach (var mapping in mappingsResult.Mappings)
                {
                    PrinterMappings.Add(mapping);
                }

                // If some groups don't have mappings for this terminal, create them in-memory
                foreach (var group in PrinterGroups)
                {
                    if (!PrinterMappings.Any(m => m.PrinterGroupId == group.Id))
                    {
                        PrinterMappings.Add(new PrinterMappingDto
                        {
                            TerminalId = terminalId.Value,
                            PrinterGroupId = group.Id,
                            PrinterGroupName = group.Name,
                            PrinterName = string.Empty
                        });
                    }
                }
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error loading printer settings: {ex.Message}";
        }
    }

    private async Task SavePrintersAsync()
    {
        IsBusy = true;
        StatusMessage = "Saving printer settings...";
        try
        {
            await _updatePrinterGroups.HandleAsync(new UpdatePrinterGroupsCommand(new List<PrinterGroupDto>(PrinterGroups)));
            if (_terminalContext.TerminalId.HasValue)
            {
                await _updatePrinterMappings.HandleAsync(new UpdatePrinterMappingsCommand(_terminalContext.TerminalId.Value, new List<PrinterMappingDto>(PrinterMappings)));
            }
            StatusMessage = "Printer settings saved successfully.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error saving printer settings: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task SaveCardConfigurationAsync()
    {
        if (CardConfiguration == null) return;

        IsBusy = true;
        StatusMessage = "Saving card configuration...";
        try
        {
            var result = await _updateCardConfig.HandleAsync(new UpdateCardConfigCommand(CardConfiguration));
            StatusMessage = "Card configuration saved successfully.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error saving card configuration: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task SaveConfigurationAsync()
    {
        if (Configuration == null) return;

        IsBusy = true;
        StatusMessage = "Saving configuration...";
        try
        {
            var result = await _updateConfig.HandleAsync(new UpdateRestaurantConfigCommand(Configuration));
            StatusMessage = "Configuration saved successfully.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error saving configuration: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task SaveTerminalConfigurationAsync()
    {
        if (TerminalConfiguration == null) return;

        IsBusy = true;
        StatusMessage = "Saving terminal configuration...";
        try
        {
            var result = await _updateTerminalConfig.HandleAsync(new UpdateTerminalConfigCommand(TerminalConfiguration));
            StatusMessage = "Terminal configuration saved successfully.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error saving terminal configuration: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
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

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Magidesk.Application.Commands.TableTypes;
using Magidesk.Application.Queries.TableTypes;
using Magidesk.Application.Interfaces;
using Magidesk.Application.DTOs;
using Magidesk.Domain.Services;
using Magidesk.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Magidesk.Presentation.ViewModels;

/// <summary>
/// ViewModel for advanced pricing configuration and simulation.
/// </summary>
public partial class AdvancedPricingConfigurationViewModel : ViewModelBase
{
    private readonly ICommandHandler<UpdateTableTypeCommand, UpdateTableTypeResult> _updateTableTypeHandler;
    private readonly IQueryHandler<GetTableTypesQuery, IEnumerable<TableTypeDto>> _getTableTypesHandler;
    private readonly IAdvancedPricingService _pricingService;
    private readonly ILogger<AdvancedPricingConfigurationViewModel> _logger;

    [ObservableProperty]
    private TableTypeDto? _selectedTableType;

    [ObservableProperty]
    private decimal _hourlyRate;

    [ObservableProperty]
    private decimal? _firstHourRate;

    [ObservableProperty]
    private decimal _minimumCharge;

    [ObservableProperty]
    private TimeRoundingRule _timeRoundingRule = TimeRoundingRule.FifteenMinutes;

    [ObservableProperty]
    private bool _hasFirstHourPricing;

    [ObservableProperty]
    private string _tableTypeName = string.Empty;

    [ObservableProperty]
    private string _description = string.Empty;

    [ObservableProperty]
    private string? _errorMessage;

    [ObservableProperty]
    private bool _hasError;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _hasUnsavedChanges;

    // Simulation properties
    [ObservableProperty]
    private TimeSpan _simulationDuration = TimeSpan.FromHours(2);

    [ObservableProperty]
    private int _simulationGuestCount = 2;

    [ObservableProperty]
    private DateTime _simulationStartTime = DateTime.Now;

    [ObservableProperty]
    private PricingSimulationResult? _simulationResult;

    public ObservableCollection<TableTypeDto> TableTypes { get; } = new();

    public ObservableCollection<TimeRoundingRule> RoundingRules { get; } = new()
    {
        TimeRoundingRule.FifteenMinutes,
        TimeRoundingRule.ThirtyMinutes,
        TimeRoundingRule.SixtyMinutes
    };

    public string RoundingRuleDisplay => TimeRoundingRule switch
    {
        TimeRoundingRule.FifteenMinutes => "15 minutes",
        TimeRoundingRule.ThirtyMinutes => "30 minutes",
        TimeRoundingRule.SixtyMinutes => "60 minutes",
        _ => "Unknown"
    };

    public bool CanSave => SelectedTableType != null && !string.IsNullOrWhiteSpace(TableTypeName) && 
                          HourlyRate > 0 && MinimumCharge >= 0 && !IsLoading;

    public bool CanSimulate => SelectedTableType != null && SimulationDuration > TimeSpan.Zero && 
                              SimulationGuestCount > 0 && !IsLoading;

    public event EventHandler<PricingConfigurationEventArgs>? ConfigurationSaved;

    public AdvancedPricingConfigurationViewModel(
        ICommandHandler<UpdateTableTypeCommand, UpdateTableTypeResult> updateTableTypeHandler,
        IQueryHandler<GetTableTypesQuery, IEnumerable<TableTypeDto>> getTableTypesHandler,
        IAdvancedPricingService pricingService,
        ILogger<AdvancedPricingConfigurationViewModel> logger)
    {
        _updateTableTypeHandler = updateTableTypeHandler ?? throw new ArgumentNullException(nameof(updateTableTypeHandler));
        _getTableTypesHandler = getTableTypesHandler ?? throw new ArgumentNullException(nameof(getTableTypesHandler));
        _pricingService = pricingService ?? throw new ArgumentNullException(nameof(pricingService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        LoadTableTypesCommand = new AsyncRelayCommand(LoadTableTypesAsync);
        SaveConfigurationCommand = new AsyncRelayCommand(SaveConfigurationAsync, () => CanSave);
        SimulatePricingCommand = new AsyncRelayCommand(SimulatePricingAsync, () => CanSimulate);
        ResetChangesCommand = new RelayCommand(ResetChanges, () => HasUnsavedChanges);
        CreateNewTableTypeCommand = new RelayCommand(CreateNewTableType);
    }

    public AsyncRelayCommand LoadTableTypesCommand { get; }
    public AsyncRelayCommand SaveConfigurationCommand { get; }
    public AsyncRelayCommand SimulatePricingCommand { get; }
    public RelayCommand ResetChangesCommand { get; }
    public RelayCommand CreateNewTableTypeCommand { get; }

    /// <summary>
    /// Initializes the view model and loads table types.
    /// </summary>
    public async Task InitializeAsync()
    {
        await LoadTableTypesAsync();
    }

    private async Task LoadTableTypesAsync()
    {
        try
        {
            IsLoading = true;
            HasError = false;
            ErrorMessage = null;

            var query = new GetTableTypesQuery();
            var tableTypes = await _getTableTypesHandler.HandleAsync(query);

            TableTypes.Clear();
            foreach (var tableType in tableTypes)
            {
                TableTypes.Add(tableType);
            }

            _logger.LogInformation("Loaded {Count} table types for pricing configuration", TableTypes.Count);

            // Select first table type if available
            if (TableTypes.Any() && SelectedTableType == null)
            {
                SelectedTableType = TableTypes.First();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load table types");
            HasError = true;
            ErrorMessage = $"Failed to load table types: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task SaveConfigurationAsync()
    {
        try
        {
            IsLoading = true;
            HasError = false;
            ErrorMessage = null;

            if (SelectedTableType == null)
            {
                ErrorMessage = "Please select a table type to configure.";
                HasError = true;
                return;
            }

            // Validate configuration
            if (HourlyRate <= 0)
            {
                ErrorMessage = "Hourly rate must be greater than zero.";
                HasError = true;
                return;
            }

            if (MinimumCharge < 0)
            {
                ErrorMessage = "Minimum charge cannot be negative.";
                HasError = true;
                return;
            }

            if (HasFirstHourPricing && (!FirstHourRate.HasValue || FirstHourRate.Value <= 0))
            {
                ErrorMessage = "First hour rate must be specified and greater than zero when first hour pricing is enabled.";
                HasError = true;
                return;
            }

            // Create update command
            var command = new UpdateTableTypeCommand(
                SelectedTableType.Id,
                TableTypeName,
                Description,
                HourlyRate,
                HasFirstHourPricing ? FirstHourRate : null,
                MinimumCharge,
                (Magidesk.Application.Commands.TableTypes.TimeRoundingRule)(int)TimeRoundingRule
            );

            var result = await _updateTableTypeHandler.HandleAsync(command);

            _logger.LogInformation("Updated table type {TableTypeId} pricing configuration", SelectedTableType.Id);

            // Update the selected table type with new values
            var updatedTableType = new TableTypeDto
            {
                Id = SelectedTableType.Id,
                Name = TableTypeName,
                Description = Description,
                HourlyRate = HourlyRate,
                FirstHourRate = HasFirstHourPricing ? FirstHourRate : null,
                MinimumCharge = MinimumCharge,
                TimeRoundingRule = (Magidesk.Application.DTOs.TimeRoundingRule)(int)TimeRoundingRule,
                IsActive = SelectedTableType.IsActive
            };

            // Update in collection
            var index = TableTypes.ToList().FindIndex(t => t.Id == SelectedTableType.Id);
            if (index >= 0)
            {
                TableTypes[index] = updatedTableType;
                SelectedTableType = updatedTableType;
            }

            HasUnsavedChanges = false;

            // Notify completion
            ConfigurationSaved?.Invoke(this, new PricingConfigurationEventArgs(
                SelectedTableType.Id, true, "Pricing configuration saved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save pricing configuration for table type {TableTypeId}", SelectedTableType?.Id);
            HasError = true;
            ErrorMessage = $"Failed to save configuration: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
            SaveConfigurationCommand.NotifyCanExecuteChanged();
        }
    }

    private async Task SimulatePricingAsync()
    {
        try
        {
            IsLoading = true;
            HasError = false;
            ErrorMessage = null;
            SimulationResult = null;

            if (SelectedTableType == null)
            {
                ErrorMessage = "Please select a table type to simulate.";
                HasError = true;
                return;
            }

            // Create a temporary table type with current configuration
            var tableType = Domain.Entities.TableType.Create(
                TableTypeName,
                HourlyRate,
                Description
            );
            
            tableType.UpdateRates(HourlyRate, HasFirstHourPricing ? FirstHourRate : null);
            tableType.SetMinimumCharge(new Money(MinimumCharge));
            tableType.SetRoundingRule((Magidesk.Domain.Enumerations.TimeRoundingRule)(int)TimeRoundingRule);

            // Create pricing scenario
            var scenario = new PricingScenario(
                SimulationDuration,
                tableType,
                SimulationGuestCount,
                SimulationStartTime
            );

            // Run simulation
            var result = await _pricingService.SimulatePricingAsync(scenario);
            SimulationResult = result;

            _logger.LogInformation("Pricing simulation completed for duration {Duration}, guest count {GuestCount}",
                SimulationDuration, SimulationGuestCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to simulate pricing");
            HasError = true;
            ErrorMessage = $"Simulation failed: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void ResetChanges()
    {
        if (SelectedTableType != null)
        {
            LoadTableTypeConfiguration(SelectedTableType);
            HasUnsavedChanges = false;
        }
    }

    private void CreateNewTableType()
    {
        // Create a new table type with default values
        var newTableType = new TableTypeDto
        {
            Id = Guid.NewGuid(),
            Name = "New Table Type",
            Description = "",
            HourlyRate = 15.00m,
            FirstHourRate = null,
            MinimumCharge = 5.00m,
            TimeRoundingRule = (Magidesk.Application.DTOs.TimeRoundingRule)(int)TimeRoundingRule.FifteenMinutes,
            IsActive = true
        };

        TableTypes.Add(newTableType);
        SelectedTableType = newTableType;
        HasUnsavedChanges = true;
    }

    private void LoadTableTypeConfiguration(TableTypeDto tableType)
    {
        TableTypeName = tableType.Name;
        Description = tableType.Description ?? string.Empty;
        HourlyRate = tableType.HourlyRate;
        FirstHourRate = tableType.FirstHourRate;
        HasFirstHourPricing = tableType.FirstHourRate.HasValue;
        MinimumCharge = tableType.MinimumCharge;
        TimeRoundingRule = (TimeRoundingRule)(int)tableType.TimeRoundingRule;
        
        // Clear simulation result when switching table types
        SimulationResult = null;
    }

    partial void OnSelectedTableTypeChanged(TableTypeDto? value)
    {
        if (value != null)
        {
            LoadTableTypeConfiguration(value);
            HasUnsavedChanges = false;
        }
        
        SaveConfigurationCommand.NotifyCanExecuteChanged();
        SimulatePricingCommand.NotifyCanExecuteChanged();
    }

    partial void OnTableTypeNameChanged(string value)
    {
        HasUnsavedChanges = true;
        SaveConfigurationCommand.NotifyCanExecuteChanged();
        OnPropertyChanged(nameof(CanSave));
    }

    partial void OnHourlyRateChanged(decimal value)
    {
        HasUnsavedChanges = true;
        SaveConfigurationCommand.NotifyCanExecuteChanged();
        OnPropertyChanged(nameof(CanSave));
    }

    partial void OnFirstHourRateChanged(decimal? value)
    {
        HasUnsavedChanges = true;
        SaveConfigurationCommand.NotifyCanExecuteChanged();
    }

    partial void OnMinimumChargeChanged(decimal value)
    {
        HasUnsavedChanges = true;
        SaveConfigurationCommand.NotifyCanExecuteChanged();
        OnPropertyChanged(nameof(CanSave));
    }

    partial void OnTimeRoundingRuleChanged(TimeRoundingRule value)
    {
        HasUnsavedChanges = true;
        SaveConfigurationCommand.NotifyCanExecuteChanged();
        OnPropertyChanged(nameof(RoundingRuleDisplay));
    }

    partial void OnHasFirstHourPricingChanged(bool value)
    {
        if (!value)
        {
            FirstHourRate = null;
        }
        HasUnsavedChanges = true;
        SaveConfigurationCommand.NotifyCanExecuteChanged();
    }

    partial void OnDescriptionChanged(string value)
    {
        HasUnsavedChanges = true;
        SaveConfigurationCommand.NotifyCanExecuteChanged();
    }

    public double SimulationDurationHours
    {
        get => SimulationDuration.TotalHours;
        set
        {
            if (Math.Abs(SimulationDuration.TotalHours - value) > 0.001)
            {
                SimulationDuration = TimeSpan.FromHours(value);
            }
        }
    }

    partial void OnSimulationDurationChanged(TimeSpan value)
    {
        SimulatePricingCommand.NotifyCanExecuteChanged();
        OnPropertyChanged(nameof(CanSimulate));
        OnPropertyChanged(nameof(SimulationDurationHours));
    }

    partial void OnSimulationGuestCountChanged(int value)
    {
        SimulatePricingCommand.NotifyCanExecuteChanged();
        OnPropertyChanged(nameof(CanSimulate));
    }
}

/// <summary>
/// Time rounding rules for pricing calculations.
/// </summary>
public enum TimeRoundingRule
{
    FifteenMinutes,
    ThirtyMinutes,
    SixtyMinutes
}

/// <summary>
/// Event arguments for pricing configuration operations.
/// </summary>
public class PricingConfigurationEventArgs : EventArgs
{
    public Guid TableTypeId { get; }
    public bool Success { get; }
    public string Message { get; }

    public PricingConfigurationEventArgs(Guid tableTypeId, bool success, string message)
    {
        TableTypeId = tableTypeId;
        Success = success;
        Message = message;
    }
}
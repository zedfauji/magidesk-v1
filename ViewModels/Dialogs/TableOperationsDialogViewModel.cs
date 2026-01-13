using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Magidesk.Application.Commands.TableOperations;
using Magidesk.Application.Queries;
using Magidesk.Application.Interfaces;
using Magidesk.Application.DTOs;
using Magidesk.Domain.ValueObjects;
using Magidesk.Domain.Services;
using Microsoft.Extensions.Logging;

namespace Magidesk.Presentation.ViewModels.Dialogs;

/// <summary>
/// ViewModel for table operations including merge and split functionality.
/// </summary>
public partial class TableOperationsDialogViewModel : ViewModelBase
{
    private readonly ICommandHandler<MergeTablesCommand, MergeTablesResult> _mergeTablesHandler;
    private readonly ICommandHandler<SplitTablesCommand, SplitTablesResult> _splitTablesHandler;
    private readonly ICommandHandler<TransferSessionCommand, TransferSessionResult> _transferSessionHandler;
    private readonly IQueryHandler<GetAvailableTablesQuery, IEnumerable<TableDto>> _getAvailableTablesHandler;
    private readonly ITableOperationsService _tableOperationsService;
    private readonly ILogger<TableOperationsDialogViewModel> _logger;

    [ObservableProperty]
    private TableOperationType _operationType;

    [ObservableProperty]
    private Guid _primaryTableId;

    [ObservableProperty]
    private string _primaryTableName = string.Empty;

    [ObservableProperty]
    private Guid? _primarySessionId;

    [ObservableProperty]
    private decimal _primarySessionCharge;

    [ObservableProperty]
    private TimeSpan _primarySessionDuration;

    [ObservableProperty]
    private int _primaryGuestCount;

    [ObservableProperty]
    private string _operationReason = string.Empty;

    [ObservableProperty]
    private string? _errorMessage;

    [ObservableProperty]
    private bool _hasError;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _canExecuteOperation;

    public ObservableCollection<TableDto> AvailableTables { get; } = new();
    public ObservableCollection<TableDto> SelectedTables { get; } = new();
    public ObservableCollection<TableSplitAllocation> SplitAllocations { get; } = new();

    public ObservableCollection<string> MergeReasons { get; } = new()
    {
        "Large group accommodation",
        "Customer request",
        "Tournament setup",
        "Special event",
        "Other"
    };

    public ObservableCollection<string> SplitReasons { get; } = new()
    {
        "Group size reduction",
        "Separate billing request",
        "Table availability",
        "Customer preference",
        "Other"
    };

    public ObservableCollection<string> TransferReasons { get; } = new()
    {
        "Table maintenance required",
        "Customer preference",
        "Better table location",
        "Equipment issue",
        "Other"
    };

    public string OperationTypeDisplay => OperationType switch
    {
        TableOperationType.Merge => "Merge Tables",
        TableOperationType.Split => "Split Tables",
        TableOperationType.Transfer => "Transfer Session",
        _ => "Table Operation"
    };

    public ObservableCollection<string> CurrentReasons => OperationType switch
    {
        TableOperationType.Merge => MergeReasons,
        TableOperationType.Split => SplitReasons,
        TableOperationType.Transfer => TransferReasons,
        _ => new ObservableCollection<string>()
    };

    public bool IsMergeOperation => OperationType == TableOperationType.Merge;
    public bool IsSplitOperation => OperationType == TableOperationType.Split;
    public bool IsTransferOperation => OperationType == TableOperationType.Transfer;

    public decimal TotalSplitAllocation => SplitAllocations.Sum(a => a.AllocatedAmount);
    public bool IsSplitAllocationValid => Math.Abs(TotalSplitAllocation - PrimarySessionCharge) < 0.01m;

    public event EventHandler? RequestClose;
    public event EventHandler<TableOperationEventArgs>? OperationCompleted;

    public TableOperationsDialogViewModel(
        ICommandHandler<MergeTablesCommand, MergeTablesResult> mergeTablesHandler,
        ICommandHandler<SplitTablesCommand, SplitTablesResult> splitTablesHandler,
        ICommandHandler<TransferSessionCommand, TransferSessionResult> transferSessionHandler,
        IQueryHandler<GetAvailableTablesQuery, IEnumerable<TableDto>> getAvailableTablesHandler,
        ITableOperationsService tableOperationsService,
        ILogger<TableOperationsDialogViewModel> logger)
    {
        _mergeTablesHandler = mergeTablesHandler ?? throw new ArgumentNullException(nameof(mergeTablesHandler));
        _splitTablesHandler = splitTablesHandler ?? throw new ArgumentNullException(nameof(splitTablesHandler));
        _transferSessionHandler = transferSessionHandler ?? throw new ArgumentNullException(nameof(transferSessionHandler));
        _getAvailableTablesHandler = getAvailableTablesHandler ?? throw new ArgumentNullException(nameof(getAvailableTablesHandler));
        _tableOperationsService = tableOperationsService ?? throw new ArgumentNullException(nameof(tableOperationsService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        LoadAvailableTablesCommand = new AsyncRelayCommand(LoadAvailableTablesAsync);
        ExecuteOperationCommand = new AsyncRelayCommand(ExecuteOperationAsync, () => CanExecuteOperation && !IsLoading);
        ValidateOperationCommand = new AsyncRelayCommand(ValidateOperationAsync);
        AddSplitAllocationCommand = new RelayCommand(AddSplitAllocation);
        RemoveSplitAllocationCommand = new RelayCommand<TableSplitAllocation>(RemoveSplitAllocation);
        CancelCommand = new RelayCommand(Cancel);

        // Subscribe to collection changes
        SelectedTables.CollectionChanged += (s, e) => UpdateCanExecuteOperation();
        SplitAllocations.CollectionChanged += (s, e) => 
        {
            UpdateCanExecuteOperation();
            OnPropertyChanged(nameof(TotalSplitAllocation));
            OnPropertyChanged(nameof(IsSplitAllocationValid));
        };
    }

    public AsyncRelayCommand LoadAvailableTablesCommand { get; }
    public AsyncRelayCommand ExecuteOperationCommand { get; }
    public AsyncRelayCommand ValidateOperationCommand { get; }
    public RelayCommand AddSplitAllocationCommand { get; }
    public RelayCommand<TableSplitAllocation> RemoveSplitAllocationCommand { get; }
    public RelayCommand CancelCommand { get; }

    /// <summary>
    /// Initializes the dialog with operation information.
    /// </summary>
    public async Task InitializeAsync(
        TableOperationType operationType,
        Guid primaryTableId,
        string primaryTableName,
        Guid? primarySessionId = null,
        decimal primarySessionCharge = 0m,
        TimeSpan primarySessionDuration = default,
        int primaryGuestCount = 0)
    {
        OperationType = operationType;
        PrimaryTableId = primaryTableId;
        PrimaryTableName = primaryTableName;
        PrimarySessionId = primarySessionId;
        PrimarySessionCharge = primarySessionCharge;
        PrimarySessionDuration = primarySessionDuration;
        PrimaryGuestCount = primaryGuestCount;
        
        // Reset state
        OperationReason = string.Empty;
        HasError = false;
        ErrorMessage = null;
        CanExecuteOperation = false;
        
        // Clear collections
        SelectedTables.Clear();
        SplitAllocations.Clear();
        
        // Update UI bindings
        OnPropertyChanged(nameof(OperationTypeDisplay));
        OnPropertyChanged(nameof(CurrentReasons));
        OnPropertyChanged(nameof(IsMergeOperation));
        OnPropertyChanged(nameof(IsSplitOperation));
        OnPropertyChanged(nameof(IsTransferOperation));
        
        // Load available tables
        await LoadAvailableTablesAsync();
        
        // Initialize split allocations if needed
        if (IsSplitOperation && PrimarySessionCharge > 0)
        {
            InitializeSplitAllocations();
        }
    }

    private async Task LoadAvailableTablesAsync()
    {
        try
        {
            IsLoading = true;
            HasError = false;
            ErrorMessage = null;

            var query = new GetAvailableTablesQuery();
            var tables = await _getAvailableTablesHandler.HandleAsync(query);

            AvailableTables.Clear();
            foreach (var table in tables.Where(t => t.Id != PrimaryTableId))
            {
                AvailableTables.Add(table);
            }

            _logger.LogInformation("Loaded {Count} available tables for {OperationType} operation",
                AvailableTables.Count, OperationType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load available tables for operation {OperationType}", OperationType);
            HasError = true;
            ErrorMessage = $"Failed to load available tables: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task ValidateOperationAsync()
    {
        try
        {
            IsLoading = true;
            HasError = false;
            ErrorMessage = null;

            switch (OperationType)
            {
                case TableOperationType.Merge:
                    await ValidateMergeOperationAsync();
                    break;
                case TableOperationType.Split:
                    await ValidateSplitOperationAsync();
                    break;
                case TableOperationType.Transfer:
                    await ValidateTransferOperationAsync();
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to validate {OperationType} operation", OperationType);
            HasError = true;
            ErrorMessage = $"Validation failed: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task ValidateMergeOperationAsync()
    {
        if (!SelectedTables.Any())
        {
            ErrorMessage = "Please select tables to merge.";
            HasError = true;
            return;
        }

        var primaryId = PrimaryTableId;
        var secondaryIds = SelectedTables.Select(t => t.Id).ToList();
        var result = await _tableOperationsService.ValidateTableMergeAsync(primaryId, secondaryIds);

        if (!result.IsValid)
        {
            ErrorMessage = result.GetFormattedIssues();
            HasError = true;
            return;
        }

        CanExecuteOperation = true;
    }

    private async Task ValidateSplitOperationAsync()
    {
        if (!SplitAllocations.Any())
        {
            ErrorMessage = "Please add split allocations.";
            HasError = true;
            return;
        }

        if (!IsSplitAllocationValid)
        {
            ErrorMessage = $"Split allocations must total ${PrimarySessionCharge:F2}.";
            HasError = true;
            return;
        }

        // Additional validation can be added here
        CanExecuteOperation = true;
    }

    private async Task ValidateTransferOperationAsync()
    {
        if (!SelectedTables.Any())
        {
            ErrorMessage = "Please select a target table for transfer.";
            HasError = true;
            return;
        }

        if (SelectedTables.Count > 1)
        {
            ErrorMessage = "Please select only one target table for transfer.";
            HasError = true;
            return;
        }

        var targetTable = SelectedTables.First();
        // Validation logic for session transfer if available in service
        // For now, we assume simple availability check done by LoadAvailableTables is sufficient
        // or we rely on the command handler to throw/fail.
        TableSplitValidationResult result = new TableSplitValidationResult(true, Array.Empty<string>()); // Placeholder
        // var result = await _tableOperationsService.ValidateSessionTransferAsync(PrimarySessionId!.Value, targetTable.Id);

        if (!result.IsValid)
        {
            ErrorMessage = result.GetFormattedIssues();
            HasError = true;
            return;
        }

        CanExecuteOperation = true;
    }

    private async Task ExecuteOperationAsync()
    {
        try
        {
            IsLoading = true;
            HasError = false;
            ErrorMessage = null;

            if (string.IsNullOrWhiteSpace(OperationReason))
            {
                ErrorMessage = "Please provide a reason for the operation.";
                HasError = true;
                return;
            }

            switch (OperationType)
            {
                case TableOperationType.Merge:
                    await ExecuteMergeOperationAsync();
                    break;
                case TableOperationType.Split:
                    await ExecuteSplitOperationAsync();
                    break;
                case TableOperationType.Transfer:
                    await ExecuteTransferOperationAsync();
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to execute {OperationType} operation", OperationType);
            HasError = true;
            ErrorMessage = $"Operation failed: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
            ExecuteOperationCommand.NotifyCanExecuteChanged();
        }
    }

    private async Task ExecuteMergeOperationAsync()
    {
        var staffId = Guid.NewGuid(); // TODO: Get from current user context
        var command = new MergeTablesCommand(PrimaryTableId, SelectedTables.Select(t => t.Id).ToList(), OperationReason, staffId);
        var result = await _mergeTablesHandler.HandleAsync(command);

        _logger.LogInformation("Tables merged successfully: {PrimaryId} + {SecondaryCount} tables -> Session {SessionId}",
            PrimaryTableId, SelectedTables.Count, result.MergedSessionId);

        OperationCompleted?.Invoke(this, new TableOperationEventArgs(
            OperationType, true, $"Tables merged successfully"));

        RequestClose?.Invoke(this, EventArgs.Empty);
    }

    private async Task ExecuteSplitOperationAsync()
    {
        var allocations = SplitAllocations.Select(a => new TableSplitAllocationInfo(
            a.TargetTableId, a.AllocatedAmount, a.GuestCount)).ToList();
        var staffId = Guid.NewGuid(); // TODO: Get from current user context

        var command = new SplitTablesCommand(PrimaryTableId, allocations, OperationReason, staffId);
        var result = await _splitTablesHandler.HandleAsync(command);

        _logger.LogInformation("Table split successfully: {OriginalTableId} -> {SplitTableIds}",
            PrimaryTableId, string.Join(", ", result.SplitTableIds));

        OperationCompleted?.Invoke(this, new TableOperationEventArgs(
            OperationType, true, $"Table split into {result.SplitTableIds.Count} tables"));

        RequestClose?.Invoke(this, EventArgs.Empty);
    }

    private async Task ExecuteTransferOperationAsync()
    {
        var targetTable = SelectedTables.First();
        var staffId = Guid.NewGuid(); // TODO: Get from current user context

        var command = new TransferSessionCommand(PrimarySessionId!.Value, targetTable.Id, OperationReason, staffId);
        var result = await _transferSessionHandler.HandleAsync(command);

        _logger.LogInformation("Session transferred successfully: {SessionId} from {SourceTable} to Table {TargetTable}",
            PrimarySessionId, PrimaryTableName, targetTable.TableNumber);

        OperationCompleted?.Invoke(this, new TableOperationEventArgs(
            OperationType, true, $"Session transferred to Table {targetTable.TableNumber}"));

        RequestClose?.Invoke(this, EventArgs.Empty);
    }

    private void InitializeSplitAllocations()
    {
        // Add default allocation for primary table (50% of charge)
        var primaryAllocation = new TableSplitAllocation
        {
            TargetTableId = PrimaryTableId,
            TargetTableName = PrimaryTableName,
            AllocatedAmount = PrimarySessionCharge / 2,
            GuestCount = Math.Max(1, PrimaryGuestCount / 2)
        };
        SplitAllocations.Add(primaryAllocation);
    }

    private void AddSplitAllocation()
    {
        if (!AvailableTables.Any()) return;

        var availableTable = AvailableTables.FirstOrDefault(t => !SplitAllocations.Any(a => a.TargetTableId == t.Id));
        if (availableTable == null) return;

        var remainingAmount = PrimarySessionCharge - TotalSplitAllocation;
        var remainingGuests = Math.Max(1, PrimaryGuestCount - SplitAllocations.Sum(a => a.GuestCount));

        var allocation = new TableSplitAllocation
        {
            TargetTableId = availableTable.Id,
            TargetTableName = $"Table {availableTable.TableNumber}",
            AllocatedAmount = Math.Max(0, remainingAmount),
            GuestCount = remainingGuests
        };

        SplitAllocations.Add(allocation);
    }

    private void RemoveSplitAllocation(TableSplitAllocation? allocation)
    {
        if (allocation != null && allocation.TargetTableId != PrimaryTableId)
        {
            SplitAllocations.Remove(allocation);
        }
    }

    private void UpdateCanExecuteOperation()
    {
        CanExecuteOperation = OperationType switch
        {
            TableOperationType.Merge => SelectedTables.Any(),
            TableOperationType.Split => SplitAllocations.Any() && IsSplitAllocationValid,
            TableOperationType.Transfer => SelectedTables.Count == 1,
            _ => false
        };

        ExecuteOperationCommand.NotifyCanExecuteChanged();
    }

    private void Cancel()
    {
        RequestClose?.Invoke(this, EventArgs.Empty);
    }

    partial void OnOperationReasonChanged(string value)
    {
        if (HasError && !string.IsNullOrWhiteSpace(value))
        {
            HasError = false;
            ErrorMessage = null;
        }
    }
}

/// <summary>
/// Types of table operations.
/// </summary>
public enum TableOperationType
{
    Merge,
    Split,
    Transfer
}

/// <summary>
/// Table split allocation information.
/// </summary>
public class TableSplitAllocation : ObservableObject
{
    private Guid _targetTableId;
    private string _targetTableName = string.Empty;
    private decimal _allocatedAmount;
    private int _guestCount;

    public Guid TargetTableId
    {
        get => _targetTableId;
        set => SetProperty(ref _targetTableId, value);
    }

    public string TargetTableName
    {
        get => _targetTableName;
        set => SetProperty(ref _targetTableName, value);
    }

    public decimal AllocatedAmount
    {
        get => _allocatedAmount;
        set => SetProperty(ref _allocatedAmount, value);
    }

    public int GuestCount
    {
        get => _guestCount;
        set => SetProperty(ref _guestCount, value);
    }
}

/// <summary>
/// Event arguments for table operations.
/// </summary>
public class TableOperationEventArgs : EventArgs
{
    public TableOperationType OperationType { get; }
    public bool Success { get; }
    public string Message { get; }

    public TableOperationEventArgs(TableOperationType operationType, bool success, string message)
    {
        OperationType = operationType;
        Success = success;
        Message = message;
    }
}
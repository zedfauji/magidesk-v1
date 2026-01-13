using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Magidesk.Application.Commands.Equipment;
using Magidesk.Application.Queries.Equipment;
using Magidesk.Application.Interfaces;
using Magidesk.Application.DTOs;
using Magidesk.Domain.Enumerations;
using Microsoft.Extensions.Logging;

namespace Magidesk.Presentation.ViewModels.Dialogs;

/// <summary>
/// ViewModel for equipment management operations including assignment, status updates, and maintenance scheduling.
/// </summary>
public partial class EquipmentManagementDialogViewModel : ViewModelBase
{
    private readonly ICommandHandler<AssignEquipmentCommand, AssignEquipmentResult> _assignEquipmentHandler;
    private readonly ICommandHandler<UnassignEquipmentCommand, UnassignEquipmentResult> _unassignEquipmentHandler;
    private readonly ICommandHandler<UpdateEquipmentStatusCommand, UpdateEquipmentStatusResult> _updateStatusHandler;
    private readonly ICommandHandler<ScheduleMaintenanceCommand, ScheduleMaintenanceResult> _scheduleMaintenanceHandler;
    private readonly IQueryHandler<GetAvailableEquipmentQuery, IEnumerable<EquipmentDto>> _getAvailableEquipmentHandler;
    private readonly IQueryHandler<GetTableEquipmentQuery, IEnumerable<EquipmentDto>> _getTableEquipmentHandler;
    private readonly ILogger<EquipmentManagementDialogViewModel> _logger;

    [ObservableProperty]
    private Guid _tableId;

    [ObservableProperty]
    private string _tableName = string.Empty;

    [ObservableProperty]
    private EquipmentManagementMode _mode;

    [ObservableProperty]
    private string? _errorMessage;

    [ObservableProperty]
    private bool _hasError;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private DateTime _maintenanceDate = DateTime.Today.AddDays(1);

    [ObservableProperty]
    private string _maintenanceNotes = string.Empty;

    public ObservableCollection<EquipmentDto> AvailableEquipment { get; } = new();
    public ObservableCollection<EquipmentDto> AssignedEquipment { get; } = new();
    public ObservableCollection<EquipmentDto> SelectedAvailableEquipment { get; } = new();
    public ObservableCollection<EquipmentDto> SelectedAssignedEquipment { get; } = new();

    public string ModeDisplay => Mode switch
    {
        EquipmentManagementMode.Assignment => "Equipment Assignment",
        EquipmentManagementMode.Status => "Equipment Status",
        EquipmentManagementMode.Maintenance => "Maintenance Scheduling",
        _ => "Equipment Management"
    };

    public bool IsAssignmentMode => Mode == EquipmentManagementMode.Assignment;
    public bool IsStatusMode => Mode == EquipmentManagementMode.Status;
    public bool IsMaintenanceMode => Mode == EquipmentManagementMode.Maintenance;

    public event EventHandler? RequestClose;
    public event EventHandler<EquipmentManagementEventArgs>? EquipmentManagementCompleted;

    public EquipmentManagementDialogViewModel(
        ICommandHandler<AssignEquipmentCommand, AssignEquipmentResult> assignEquipmentHandler,
        ICommandHandler<UnassignEquipmentCommand, UnassignEquipmentResult> unassignEquipmentHandler,
        ICommandHandler<UpdateEquipmentStatusCommand, UpdateEquipmentStatusResult> updateStatusHandler,
        ICommandHandler<ScheduleMaintenanceCommand, ScheduleMaintenanceResult> scheduleMaintenanceHandler,
        IQueryHandler<GetAvailableEquipmentQuery, IEnumerable<EquipmentDto>> getAvailableEquipmentHandler,
        IQueryHandler<GetTableEquipmentQuery, IEnumerable<EquipmentDto>> getTableEquipmentHandler,
        ILogger<EquipmentManagementDialogViewModel> logger)
    {
        _assignEquipmentHandler = assignEquipmentHandler ?? throw new ArgumentNullException(nameof(assignEquipmentHandler));
        _unassignEquipmentHandler = unassignEquipmentHandler ?? throw new ArgumentNullException(nameof(unassignEquipmentHandler));
        _updateStatusHandler = updateStatusHandler ?? throw new ArgumentNullException(nameof(updateStatusHandler));
        _scheduleMaintenanceHandler = scheduleMaintenanceHandler ?? throw new ArgumentNullException(nameof(scheduleMaintenanceHandler));
        _getAvailableEquipmentHandler = getAvailableEquipmentHandler ?? throw new ArgumentNullException(nameof(getAvailableEquipmentHandler));
        _getTableEquipmentHandler = getTableEquipmentHandler ?? throw new ArgumentNullException(nameof(getTableEquipmentHandler));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        LoadEquipmentCommand = new AsyncRelayCommand(LoadEquipmentAsync);
        AssignSelectedCommand = new AsyncRelayCommand(AssignSelectedAsync, () => SelectedAvailableEquipment.Any() && !IsLoading);
        UnassignSelectedCommand = new AsyncRelayCommand(UnassignSelectedAsync, () => SelectedAssignedEquipment.Any() && !IsLoading);
        UpdateStatusCommand = new AsyncRelayCommand<EquipmentStatus>(UpdateStatusAsync, _ => !IsLoading);
        ScheduleMaintenanceCommand = new AsyncRelayCommand(ScheduleMaintenanceAsync, () => SelectedAssignedEquipment.Any() && MaintenanceDate > DateTime.Today && !IsLoading);
        CancelCommand = new RelayCommand(Cancel);

        // Subscribe to collection changes to update command states
        SelectedAvailableEquipment.CollectionChanged += (s, e) => AssignSelectedCommand.NotifyCanExecuteChanged();
        SelectedAssignedEquipment.CollectionChanged += (s, e) => 
        {
            UnassignSelectedCommand.NotifyCanExecuteChanged();
            ScheduleMaintenanceCommand.NotifyCanExecuteChanged();
        };
    }

    public AsyncRelayCommand LoadEquipmentCommand { get; }
    public AsyncRelayCommand AssignSelectedCommand { get; }
    public AsyncRelayCommand UnassignSelectedCommand { get; }
    public AsyncRelayCommand<EquipmentStatus> UpdateStatusCommand { get; }
    public AsyncRelayCommand ScheduleMaintenanceCommand { get; }
    public RelayCommand CancelCommand { get; }

    /// <summary>
    /// Initializes the dialog with table and mode information.
    /// </summary>
    public async Task InitializeAsync(
        Guid tableId,
        string tableName,
        EquipmentManagementMode mode)
    {
        TableId = tableId;
        TableName = tableName;
        Mode = mode;
        
        // Reset state
        HasError = false;
        ErrorMessage = null;
        MaintenanceDate = DateTime.Today.AddDays(1);
        MaintenanceNotes = string.Empty;
        
        // Clear selections
        SelectedAvailableEquipment.Clear();
        SelectedAssignedEquipment.Clear();
        
        OnPropertyChanged(nameof(ModeDisplay));
        OnPropertyChanged(nameof(IsAssignmentMode));
        OnPropertyChanged(nameof(IsStatusMode));
        OnPropertyChanged(nameof(IsMaintenanceMode));
        
        // Load equipment data
        await LoadEquipmentAsync();
    }

    private async Task LoadEquipmentAsync()
    {
        try
        {
            IsLoading = true;
            HasError = false;
            ErrorMessage = null;

            // Load available equipment
            var availableQuery = new GetAvailableEquipmentQuery();
            var availableEquipment = await _getAvailableEquipmentHandler.HandleAsync(availableQuery);

            AvailableEquipment.Clear();
            foreach (var equipment in availableEquipment)
            {
                AvailableEquipment.Add(equipment);
            }

            // Load assigned equipment for this table
            var assignedQuery = new GetTableEquipmentQuery(TableId);
            var assignedEquipment = await _getTableEquipmentHandler.HandleAsync(assignedQuery);

            AssignedEquipment.Clear();
            foreach (var equipment in assignedEquipment)
            {
                AssignedEquipment.Add(equipment);
            }

            _logger.LogInformation("Loaded equipment for table {TableId}: {AvailableCount} available, {AssignedCount} assigned",
                TableId, AvailableEquipment.Count, AssignedEquipment.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load equipment for table {TableId}", TableId);
            HasError = true;
            ErrorMessage = $"Failed to load equipment: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task AssignSelectedAsync()
    {
        try
        {
            IsLoading = true;
            HasError = false;
            ErrorMessage = null;

            if (!SelectedAvailableEquipment.Any())
            {
                ErrorMessage = "Please select equipment to assign.";
                HasError = true;
                return;
            }

            var equipmentIds = SelectedAvailableEquipment.Select(e => e.Id).ToList();
            var staffId = Guid.NewGuid(); // TODO: Get from current user context

            var command = new AssignEquipmentCommand(TableId, equipmentIds, staffId);
            var result = await _assignEquipmentHandler.HandleAsync(command);

            _logger.LogInformation("Assigned {Count} equipment items to table {TableId}",
                result.AssignedEquipment.Count, TableId);

            // Refresh equipment lists
            await LoadEquipmentAsync();

            // Clear selections
            SelectedAvailableEquipment.Clear();

            // Notify completion
            EquipmentManagementCompleted?.Invoke(this, new EquipmentManagementEventArgs(
                TableId, EquipmentManagementOperation.Assign, true, $"Assigned {result.AssignedEquipment.Count} equipment items"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to assign equipment to table {TableId}", TableId);
            HasError = true;
            ErrorMessage = $"Failed to assign equipment: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
            AssignSelectedCommand.NotifyCanExecuteChanged();
        }
    }

    private async Task UnassignSelectedAsync()
    {
        try
        {
            IsLoading = true;
            HasError = false;
            ErrorMessage = null;

            if (!SelectedAssignedEquipment.Any())
            {
                ErrorMessage = "Please select equipment to unassign.";
                HasError = true;
                return;
            }

            var equipmentIds = SelectedAssignedEquipment.Select(e => e.Id).ToList();
            var staffId = Guid.NewGuid(); // TODO: Get from current user context

            var command = new UnassignEquipmentCommand(TableId, equipmentIds, staffId);
            var result = await _unassignEquipmentHandler.HandleAsync(command);

            _logger.LogInformation("Unassigned {Count} equipment items from table {TableId}",
                equipmentIds.Count, TableId);

            // Refresh equipment lists
            await LoadEquipmentAsync();

            // Clear selections
            SelectedAssignedEquipment.Clear();

            // Notify completion
            EquipmentManagementCompleted?.Invoke(this, new EquipmentManagementEventArgs(
                TableId, EquipmentManagementOperation.Unassign, true, $"Unassigned {equipmentIds.Count} equipment items"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to unassign equipment from table {TableId}", TableId);
            HasError = true;
            ErrorMessage = $"Failed to unassign equipment: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
            UnassignSelectedCommand.NotifyCanExecuteChanged();
        }
    }

    private async Task UpdateStatusAsync(EquipmentStatus newStatus)
    {
        try
        {
            IsLoading = true;
            HasError = false;
            ErrorMessage = null;

            if (!SelectedAssignedEquipment.Any())
            {
                ErrorMessage = "Please select equipment to update status.";
                HasError = true;
                return;
            }

            var equipmentIds = SelectedAssignedEquipment.Select(e => e.Id).ToList();
            var staffId = Guid.NewGuid(); // TODO: Get from current user context

            var command = new UpdateEquipmentStatusCommand(equipmentIds, newStatus, staffId);
            var result = await _updateStatusHandler.HandleAsync(command);

            _logger.LogInformation("Updated status for {Count} equipment items to {Status}",
                equipmentIds.Count, newStatus);

            // Refresh equipment lists
            await LoadEquipmentAsync();

            // Clear selections
            SelectedAssignedEquipment.Clear();

            // Notify completion
            EquipmentManagementCompleted?.Invoke(this, new EquipmentManagementEventArgs(
                TableId, EquipmentManagementOperation.UpdateStatus, true, $"Updated status for {equipmentIds.Count} equipment items"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update equipment status for table {TableId}", TableId);
            HasError = true;
            ErrorMessage = $"Failed to update equipment status: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task ScheduleMaintenanceAsync()
    {
        try
        {
            IsLoading = true;
            HasError = false;
            ErrorMessage = null;

            if (!SelectedAssignedEquipment.Any())
            {
                ErrorMessage = "Please select equipment to schedule maintenance.";
                HasError = true;
                return;
            }

            if (MaintenanceDate <= DateTime.Today)
            {
                ErrorMessage = "Maintenance date must be in the future.";
                HasError = true;
                return;
            }

            var equipmentIds = SelectedAssignedEquipment.Select(e => e.Id).ToList();
            var staffId = Guid.NewGuid(); // TODO: Get from current user context

            var command = new ScheduleMaintenanceCommand(equipmentIds, MaintenanceDate, "Routine Maintenance", MaintenanceNotes, staffId);
            var result = await _scheduleMaintenanceHandler.HandleAsync(command);

            _logger.LogInformation("Scheduled maintenance for {Count} equipment items on {Date}",
                equipmentIds.Count, MaintenanceDate);

            // Refresh equipment lists
            await LoadEquipmentAsync();

            // Clear selections and reset form
            SelectedAssignedEquipment.Clear();
            MaintenanceDate = DateTime.Today.AddDays(1);
            MaintenanceNotes = string.Empty;

            // Notify completion
            EquipmentManagementCompleted?.Invoke(this, new EquipmentManagementEventArgs(
                TableId, EquipmentManagementOperation.ScheduleMaintenance, true, $"Scheduled maintenance for {equipmentIds.Count} equipment items"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to schedule maintenance for table {TableId}", TableId);
            HasError = true;
            ErrorMessage = $"Failed to schedule maintenance: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
            ScheduleMaintenanceCommand.NotifyCanExecuteChanged();
        }
    }

    private void Cancel()
    {
        RequestClose?.Invoke(this, EventArgs.Empty);
    }

    partial void OnMaintenanceDateChanged(DateTime value)
    {
        ScheduleMaintenanceCommand.NotifyCanExecuteChanged();
    }
}

/// <summary>
/// Equipment management modes.
/// </summary>
public enum EquipmentManagementMode
{
    Assignment,
    Status,
    Maintenance
}

/// <summary>
/// Equipment management operations.
/// </summary>
public enum EquipmentManagementOperation
{
    Assign,
    Unassign,
    UpdateStatus,
    ScheduleMaintenance
}

/// <summary>
/// Event arguments for equipment management operations.
/// </summary>
public class EquipmentManagementEventArgs : EventArgs
{
    public Guid TableId { get; }
    public EquipmentManagementOperation Operation { get; }
    public bool Success { get; }
    public string Message { get; }

    public EquipmentManagementEventArgs(Guid tableId, EquipmentManagementOperation operation, bool success, string message)
    {
        TableId = tableId;
        Operation = operation;
        Success = success;
        Message = message;
    }
}
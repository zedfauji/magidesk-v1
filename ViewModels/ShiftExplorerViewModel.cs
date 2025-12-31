using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Input;
using Magidesk.Application.Commands;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using CommunityToolkit.Mvvm.Input;

namespace Magidesk.Presentation.ViewModels;

public sealed class ShiftExplorerViewModel : ViewModelBase
{
    private readonly IShiftRepository _shiftRepository;
    private readonly ICommandHandler<CreateShiftCommand, CreateShiftResult> _create;
    private readonly ICommandHandler<UpdateShiftCommand, UpdateShiftResult> _update;

    public ObservableCollection<Shift> Shifts { get; } = new();

    private Shift? _selectedShift;
    public Shift? SelectedShift
    {
        get => _selectedShift;
        set
        {
            if (SetProperty(ref _selectedShift, value))
            {
                HydrateEditorFromSelection();
                OnPropertyChanged(nameof(HasSelection));
            }
        }
    }

    public bool HasSelection => SelectedShift != null;

    private string _editingName = string.Empty;
    public string EditingName
    {
        get => _editingName;
        set => SetProperty(ref _editingName, value);
    }

    private string _editingStartTime = "09:00";
    public string EditingStartTime
    {
        get => _editingStartTime;
        set => SetProperty(ref _editingStartTime, value);
    }

    private string _editingEndTime = "17:00";
    public string EditingEndTime
    {
        get => _editingEndTime;
        set => SetProperty(ref _editingEndTime, value);
    }

    private bool _editingIsActive;
    public bool EditingIsActive
    {
        get => _editingIsActive;
        set => SetProperty(ref _editingIsActive, value);
    }

    private string _statusMessage = "Ready";
    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    public ICommand LoadCommand { get; }
    public ICommand AddCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand DeactivateCommand { get; }

    public ShiftExplorerViewModel(
        IShiftRepository shiftRepository,
        ICommandHandler<CreateShiftCommand, CreateShiftResult> create,
        ICommandHandler<UpdateShiftCommand, UpdateShiftResult> update)
    {
        _shiftRepository = shiftRepository;
        _create = create;
        _update = update;

        Title = "Shift Explorer";

        LoadCommand = new AsyncRelayCommand(LoadAsync);
        AddCommand = new AsyncRelayCommand(AddAsync);
        SaveCommand = new AsyncRelayCommand(SaveAsync, () => SelectedShift != null);
        DeactivateCommand = new AsyncRelayCommand(DeactivateAsync, () => SelectedShift != null);
    }

    private void HydrateEditorFromSelection()
    {
        if (SelectedShift == null)
        {
            EditingName = string.Empty;
            EditingStartTime = "09:00";
            EditingEndTime = "17:00";
            EditingIsActive = false;
            return;
        }

        EditingName = SelectedShift.Name;
        EditingStartTime = SelectedShift.StartTime.ToString("hh\\:mm", CultureInfo.InvariantCulture);
        EditingEndTime = SelectedShift.EndTime.ToString("hh\\:mm", CultureInfo.InvariantCulture);
        EditingIsActive = SelectedShift.IsActive;
    }

    private static bool TryParseTime(string value, out TimeSpan time)
    {
        return TimeSpan.TryParseExact(value, "h\\:mm", CultureInfo.InvariantCulture, out time)
            || TimeSpan.TryParseExact(value, "hh\\:mm", CultureInfo.InvariantCulture, out time);
    }

    private async Task LoadAsync()
    {
        IsBusy = true;
        try
        {
            var all = await _shiftRepository.GetAllAsync();
            Shifts.Clear();
            foreach (var s in all)
            {
                Shifts.Add(s);
            }

            StatusMessage = "Loaded.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Load Failed: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task AddAsync()
    {
        IsBusy = true;
        try
        {
            var result = await _create.HandleAsync(new CreateShiftCommand
            {
                Name = "New Shift",
                StartTime = new TimeSpan(9, 0, 0),
                EndTime = new TimeSpan(17, 0, 0),
                IsActive = true
            });

            await LoadAsync();
            SelectedShift = Shifts.FirstOrDefault(x => x.Id == result.ShiftId);
            StatusMessage = "Shift created.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Create Failed: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task SaveAsync()
    {
        if (SelectedShift == null) return;

        if (!TryParseTime(EditingStartTime, out var start))
        {
            StatusMessage = "Invalid Start Time. Use HH:mm.";
            return;
        }

        if (!TryParseTime(EditingEndTime, out var end))
        {
            StatusMessage = "Invalid End Time. Use HH:mm.";
            return;
        }

        IsBusy = true;
        try
        {
            await _update.HandleAsync(new UpdateShiftCommand
            {
                ShiftId = SelectedShift.Id,
                Name = EditingName,
                StartTime = start,
                EndTime = end,
                IsActive = EditingIsActive
            });

            await LoadAsync();
            SelectedShift = Shifts.FirstOrDefault(x => x.Id == SelectedShift.Id);
            StatusMessage = "Saved.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Save Failed: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task DeactivateAsync()
    {
        if (SelectedShift == null) return;

        IsBusy = true;
        try
        {
            await _update.HandleAsync(new UpdateShiftCommand
            {
                ShiftId = SelectedShift.Id,
                IsActive = false
            });

            await LoadAsync();
            SelectedShift = null;
            StatusMessage = "Deactivated.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Deactivate Failed: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }
}

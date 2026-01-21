using System.Collections.ObjectModel;
using Magidesk.Application.Commands;
using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries;
using CommunityToolkit.Mvvm.Input;

namespace Magidesk.Presentation.ViewModels.Dialogs;

public class PromotionScheduleDialogViewModel : ViewModelBase
{
    private readonly IQueryHandler<GetPromotionSchedulesQuery, IEnumerable<PromotionScheduleDto>> _getSchedules;
    private readonly ICommandHandler<AddPromotionScheduleCommand, AddPromotionScheduleResult> _addSchedule;
    private readonly ICommandHandler<DeletePromotionScheduleCommand, DeletePromotionScheduleResult> _deleteSchedule;

    private DiscountDto _discount;
    private ObservableCollection<PromotionScheduleDto> _schedules = new();
    
    // Form Inputs
    private DayOfWeek _selectedDay = DayOfWeek.Friday; // Default
    private TimeSpan _startTime = new TimeSpan(17, 0, 0); // 5 PM
    private TimeSpan _endTime = new TimeSpan(19, 0, 0);   // 7 PM
    private string? _errorMessage;

    public PromotionScheduleDialogViewModel(
        IQueryHandler<GetPromotionSchedulesQuery, IEnumerable<PromotionScheduleDto>> getSchedules,
        ICommandHandler<AddPromotionScheduleCommand, AddPromotionScheduleResult> addSchedule,
        ICommandHandler<DeletePromotionScheduleCommand, DeletePromotionScheduleResult> deleteSchedule)
    {
        _getSchedules = getSchedules;
        _addSchedule = addSchedule;
        _deleteSchedule = deleteSchedule;

        LoadSchedulesCommand = new AsyncRelayCommand(LoadSchedulesAsync);
        AddScheduleCommand = new AsyncRelayCommand(AddScheduleAsync);
        DeleteScheduleCommand = new AsyncRelayCommand<PromotionScheduleDto>(DeleteScheduleAsync);
    }

    public void Initialize(DiscountDto discount)
    {
        _discount = discount;
        // Trigger load
        LoadSchedulesCommand.Execute(null);
    }

    public string Title => _discount == null ? "Manage Schedules" : $"Schedule: {_discount.Name}";

    public ObservableCollection<PromotionScheduleDto> Schedules
    {
        get => _schedules;
        set => SetProperty(ref _schedules, value);
    }

    // Input Properties
    public DayOfWeek SelectedDay
    {
        get => _selectedDay;
        set => SetProperty(ref _selectedDay, value);
    }

    public TimeSpan StartTime
    {
        get => _startTime;
        set => SetProperty(ref _startTime, value);
    }

    public TimeSpan EndTime
    {
        get => _endTime;
        set => SetProperty(ref _endTime, value);
    }

    public string? ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    public AsyncRelayCommand LoadSchedulesCommand { get; }
    public AsyncRelayCommand AddScheduleCommand { get; }
    public AsyncRelayCommand<PromotionScheduleDto> DeleteScheduleCommand { get; }

    public IReadOnlyList<DayOfWeek> DaysOfWeek => Enum.GetValues<DayOfWeek>();

    private async Task LoadSchedulesAsync()
    {
        if (_discount == null) return;
        
        IsBusy = true;
        try
        {
            var schedules = await _getSchedules.HandleAsync(new GetPromotionSchedulesQuery { DiscountId = _discount.Id });
            Schedules = new ObservableCollection<PromotionScheduleDto>(schedules);
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task AddScheduleAsync()
    {
        ErrorMessage = null;
        if (EndTime <= StartTime)
        {
            ErrorMessage = "End time must be after start time.";
            return;
        }

        IsBusy = true;
        try
        {
            var result = await _addSchedule.HandleAsync(new AddPromotionScheduleCommand
            {
                DiscountId = _discount.Id,
                DayOfWeek = SelectedDay,
                StartTime = StartTime,
                EndTime = EndTime
            });

            if (result.Success)
            {
                await LoadSchedulesAsync();
            }
            else
            {
                ErrorMessage = result.ErrorMessage;
            }
        }
        catch(Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task DeleteScheduleAsync(PromotionScheduleDto? schedule)
    {
        if (schedule == null) return;

        IsBusy = true;
        try
        {
            var result = await _deleteSchedule.HandleAsync(new DeletePromotionScheduleCommand { ScheduleId = schedule.Id });
            if (result.Success)
            {
                await LoadSchedulesAsync();
            }
            else
            {
                ErrorMessage = result.ErrorMessage;
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }
}

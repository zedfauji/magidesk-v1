using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;


namespace Magidesk.Application.Commands;

public class AddPromotionScheduleCommand
{
    public Guid DiscountId { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
}

public class AddPromotionScheduleResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public Guid? ScheduleId { get; set; }
}

public class AddPromotionScheduleCommandHandler : ICommandHandler<AddPromotionScheduleCommand, AddPromotionScheduleResult>
{
    private readonly IPromotionRepository _repository;

    public AddPromotionScheduleCommandHandler(IPromotionRepository repository)
    {
        _repository = repository;
    }

    public async Task<AddPromotionScheduleResult> HandleAsync(AddPromotionScheduleCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var schedule = PromotionSchedule.Create(
                command.DiscountId,
                command.DayOfWeek,
                command.StartTime,
                command.EndTime
            );

            await _repository.AddAsync(schedule, cancellationToken);

            return new AddPromotionScheduleResult { Success = true, ScheduleId = schedule.Id };
        }
        catch (Exception ex)
        {
            return new AddPromotionScheduleResult { Success = false, ErrorMessage = ex.Message };
        }
    }
}

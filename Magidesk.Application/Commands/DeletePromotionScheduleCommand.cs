using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;



namespace Magidesk.Application.Commands;

public class DeletePromotionScheduleCommand
{
    public Guid ScheduleId { get; set; }
}

public class DeletePromotionScheduleResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
}

public class DeletePromotionScheduleCommandHandler : ICommandHandler<DeletePromotionScheduleCommand, DeletePromotionScheduleResult>
{
    private readonly IPromotionRepository _repository;

    public DeletePromotionScheduleCommandHandler(IPromotionRepository repository)
    {
        _repository = repository;
    }

    public async Task<DeletePromotionScheduleResult> HandleAsync(DeletePromotionScheduleCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var schedule = await _repository.GetByIdAsync(command.ScheduleId, cancellationToken);

            if (schedule == null)
            {
                return new DeletePromotionScheduleResult { Success = false, ErrorMessage = "Schedule not found." };
            }

            await _repository.DeleteAsync(schedule, cancellationToken);

            return new DeletePromotionScheduleResult { Success = true };
        }
        catch (Exception ex)
        {
            return new DeletePromotionScheduleResult { Success = false, ErrorMessage = ex.Message };
        }
    }
}

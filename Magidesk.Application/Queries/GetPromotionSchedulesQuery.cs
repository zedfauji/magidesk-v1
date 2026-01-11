using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;

namespace Magidesk.Application.Queries;

public class GetPromotionSchedulesQuery : IQuery<IEnumerable<PromotionScheduleDto>>
{
    public Guid DiscountId { get; set; }
}

public class GetPromotionSchedulesQueryHandler : IQueryHandler<GetPromotionSchedulesQuery, IEnumerable<PromotionScheduleDto>>
{
    private readonly IPromotionRepository _repository;

    public GetPromotionSchedulesQueryHandler(IPromotionRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<PromotionScheduleDto>> HandleAsync(GetPromotionSchedulesQuery query, CancellationToken cancellationToken = default)
    {
        var schedules = await _repository.GetSchedulesByDiscountIdAsync(query.DiscountId, cancellationToken);

        // Ensure we sort it in memory if the repository doesn't guarantee order, 
        // though typically UI or repo does it. 
        // We'll trust the repo or just map it here.
        return schedules
            .OrderBy(ps => ps.DayOfWeek)
            .ThenBy(ps => ps.StartTime)
            .Select(ps => new PromotionScheduleDto
            {
                Id = ps.Id,
                DiscountId = ps.DiscountId,
                DayOfWeek = ps.DayOfWeek,
                StartTime = ps.StartTime,
                EndTime = ps.EndTime,
                IsActive = ps.IsActive
            });
    }
}

using Magidesk.Domain.Entities;

namespace Magidesk.Application.Interfaces;

public interface IPromotionRepository : IRepository<PromotionSchedule>
{
    /// <summary>
    /// Gets all promotion schedules that are active and applicable for the given day of week.
    /// </summary>
    Task<IEnumerable<PromotionSchedule>> GetActivePromotionsByDayAsync(DayOfWeek dayOfWeek, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all schedules for a specific discount.
    /// </summary>
    Task<IEnumerable<PromotionSchedule>> GetSchedulesByDiscountIdAsync(Guid discountId, CancellationToken cancellationToken = default);
}

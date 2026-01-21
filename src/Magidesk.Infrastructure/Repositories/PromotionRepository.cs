using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Magidesk.Infrastructure.Repositories;

public class PromotionRepository : EfRepository<PromotionSchedule>, IPromotionRepository
{
    public PromotionRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IEnumerable<PromotionSchedule>> GetActivePromotionsByDayAsync(DayOfWeek dayOfWeek, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<PromotionSchedule>()
            .Include(ps => ps.Discount)
            .Where(ps => ps.IsActive && 
                         ps.DayOfWeek == dayOfWeek &&
                         ps.Discount.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<PromotionSchedule>> GetSchedulesByDiscountIdAsync(Guid discountId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<PromotionSchedule>()
            .Where(ps => ps.DiscountId == discountId)
            .ToListAsync(cancellationToken);
    }
}

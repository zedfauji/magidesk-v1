using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;

namespace Magidesk.Application.Services;

public interface IPromotionDomainService
{
    /// <summary>
    /// Returns all discounts that are currently active based on the schedule.
    /// </summary>
    Task<IEnumerable<Discount>> GetActivePromotionsAsync(DateTime currentDateTime, CancellationToken cancellationToken = default);
}

public class PromotionDomainService : IPromotionDomainService
{
    private readonly IPromotionRepository _promotionRepository;

    public PromotionDomainService(IPromotionRepository promotionRepository)
    {
        _promotionRepository = promotionRepository;
    }

    public async Task<IEnumerable<Discount>> GetActivePromotionsAsync(DateTime currentDateTime, CancellationToken cancellationToken = default)
    {
        var dayOfWeek = currentDateTime.DayOfWeek;
        
        // Get all schedules for the current day
        var schedules = await _promotionRepository.GetActivePromotionsByDayAsync(dayOfWeek, cancellationToken);
        
        // Filter by time window
        return schedules
            .Where(s => s.IsApplicable(currentDateTime))
            .Select(s => s.Discount)
            .ToList();
    }
}

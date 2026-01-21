using Magidesk.Domain.Entities;

namespace Magidesk.Application.Interfaces;

public interface IRestaurantConfigurationRepository
{
    Task<RestaurantConfiguration> GetConfigurationAsync(CancellationToken cancellationToken = default);
    Task UpdateConfigurationAsync(RestaurantConfiguration configuration, CancellationToken cancellationToken = default);
}

using Magidesk.Application.DTOs.SystemConfig;
using Magidesk.Application.Interfaces;

namespace Magidesk.Application.Queries.SystemConfig;

public class GetRestaurantConfigQuery;

public record GetRestaurantConfigResult(RestaurantConfigurationDto Configuration);

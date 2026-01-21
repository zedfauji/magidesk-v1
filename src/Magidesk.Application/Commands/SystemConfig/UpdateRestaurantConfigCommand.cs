using Magidesk.Application.DTOs.SystemConfig;
using Magidesk.Application.Interfaces;

namespace Magidesk.Application.Commands.SystemConfig;

public record UpdateRestaurantConfigCommand(RestaurantConfigurationDto Configuration);

public record UpdateRestaurantConfigResult(bool IsSuccess, string Message);

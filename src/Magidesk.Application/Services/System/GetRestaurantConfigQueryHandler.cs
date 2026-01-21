using Magidesk.Application.DTOs.SystemConfig;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries.SystemConfig;

namespace Magidesk.Application.Services.SystemConfig;

public class GetRestaurantConfigQueryHandler : IQueryHandler<GetRestaurantConfigQuery, GetRestaurantConfigResult>
{
    private readonly IRestaurantConfigurationRepository _repository;

    public GetRestaurantConfigQueryHandler(IRestaurantConfigurationRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetRestaurantConfigResult> HandleAsync(GetRestaurantConfigQuery query, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetConfigurationAsync(cancellationToken);
        
        var dto = new RestaurantConfigurationDto
        {
            Name = entity.Name,
            Address = entity.Address,
            Phone = entity.Phone,
            Email = entity.Email,
            Website = entity.Website,
            ReceiptFooterMessage = entity.ReceiptFooterMessage,
            TaxId = entity.TaxId,
            ZipCode = entity.ZipCode,
            Capacity = entity.Capacity,
            CurrencySymbol = entity.CurrencySymbol,
            ServiceChargePercentage = entity.ServiceChargePercentage,
            DefaultGratuityPercentage = entity.DefaultGratuityPercentage,
            IsKioskMode = entity.IsKioskMode
        };
        
        return new GetRestaurantConfigResult(dto);
    }
}

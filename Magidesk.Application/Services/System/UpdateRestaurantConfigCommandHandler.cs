using Magidesk.Application.Commands.SystemConfig;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;

namespace Magidesk.Application.Services.SystemConfig;

public class UpdateRestaurantConfigCommandHandler : ICommandHandler<UpdateRestaurantConfigCommand, UpdateRestaurantConfigResult>
{
    private readonly IRestaurantConfigurationRepository _repository;

    public UpdateRestaurantConfigCommandHandler(IRestaurantConfigurationRepository repository)
    {
        _repository = repository;
    }

    public async Task<UpdateRestaurantConfigResult> HandleAsync(UpdateRestaurantConfigCommand command, CancellationToken cancellationToken = default)
    {
        var dto = command.Configuration;
        
        // We reuse the entity structure for update since ID is handled by repo (singleton)
        var entity = new RestaurantConfiguration
        {
            Name = dto.Name,
            Address = dto.Address,
            Phone = dto.Phone,
            Email = dto.Email,
            Website = dto.Website,
            ReceiptFooterMessage = dto.ReceiptFooterMessage,
            TaxId = dto.TaxId,
            IsKioskMode = dto.IsKioskMode
        };

        await _repository.UpdateConfigurationAsync(entity, cancellationToken);
        
        return new UpdateRestaurantConfigResult(true, "Configuration updated successfully.");
    }
}

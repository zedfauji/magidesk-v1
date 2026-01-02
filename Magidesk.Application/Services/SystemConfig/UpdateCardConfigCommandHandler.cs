using System.Threading;
using System.Threading.Tasks;
using Magidesk.Application.Commands.SystemConfig;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;

namespace Magidesk.Application.Services.SystemConfig;

public class UpdateCardConfigCommandHandler : ICommandHandler<UpdateCardConfigCommand, UpdateCardConfigResult>
{
    private readonly IMerchantGatewayConfigurationRepository _repository;
    private readonly IAesEncryptionService _encryptionService;

    public UpdateCardConfigCommandHandler(
        IMerchantGatewayConfigurationRepository repository,
        IAesEncryptionService encryptionService)
    {
        _repository = repository;
        _encryptionService = encryptionService;
    }

    public async Task<UpdateCardConfigResult> HandleAsync(UpdateCardConfigCommand command, CancellationToken cancellationToken = default)
    {
        var dto = command.Config;
        var config = await _repository.GetByTerminalIdAsync(dto.TerminalId, cancellationToken);
        
        var encryptedApiKey = _encryptionService.Encrypt(dto.ApiKey);

        if (config == null)
        {
            config = MerchantGatewayConfiguration.Create(
                dto.TerminalId,
                dto.ProviderName,
                dto.MerchantId,
                encryptedApiKey,
                dto.GatewayUrl);
            
            config.UpdateSettings(
                dto.CardTypesAccepted,
                dto.SignatureThreshold,
                dto.AllowTipAdjustment,
                dto.IsExternalTerminal,
                dto.AllowManualEntry,
                dto.EnablePreAuth);

            await _repository.AddAsync(config, cancellationToken);
        }
        else
        {
            config.UpdateCredentials(dto.MerchantId, encryptedApiKey, dto.GatewayUrl);
            config.UpdateSettings(
                dto.CardTypesAccepted,
                dto.SignatureThreshold,
                dto.AllowTipAdjustment,
                dto.IsExternalTerminal,
                dto.AllowManualEntry,
                dto.EnablePreAuth);
            
            await _repository.UpdateAsync(config, cancellationToken);
        }

        return new UpdateCardConfigResult(true, "Card configuration updated successfully.");
    }
}

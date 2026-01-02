using System.Threading;
using System.Threading.Tasks;
using Magidesk.Application.DTOs.SystemConfig;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries.SystemConfig;
using Magidesk.Domain.Entities;

namespace Magidesk.Application.Services.SystemConfig;

public class GetCardConfigQueryHandler : IQueryHandler<GetCardConfigQuery, GetCardConfigResult>
{
    private readonly IMerchantGatewayConfigurationRepository _repository;
    private readonly IAesEncryptionService _encryptionService;

    public GetCardConfigQueryHandler(
        IMerchantGatewayConfigurationRepository repository,
        IAesEncryptionService encryptionService)
    {
        _repository = repository;
        _encryptionService = encryptionService;
    }

    public async Task<GetCardConfigResult> HandleAsync(GetCardConfigQuery query, CancellationToken cancellationToken = default)
    {
        var config = await _repository.GetByTerminalIdAsync(query.TerminalId, cancellationToken);
        
        if (config == null)
        {
            // Return empty/default if not found
            return new GetCardConfigResult(new CardConfigDto
            {
                TerminalId = query.TerminalId,
                ProviderName = "AUTHORIZE_NET",
                IsActive = false
            });
        }

        return new GetCardConfigResult(new CardConfigDto
        {
            Id = config.Id,
            TerminalId = config.TerminalId,
            ProviderName = config.ProviderName,
            MerchantId = config.MerchantId,
            ApiKey = _encryptionService.Decrypt(config.EncryptedApiKey),
            GatewayUrl = config.GatewayUrl,
            IsActive = config.IsActive,
            CardTypesAccepted = config.CardTypesAccepted,
            SignatureThreshold = config.SignatureThreshold,
            AllowTipAdjustment = config.AllowTipAdjustment,
            IsExternalTerminal = config.IsExternalTerminal,
            AllowManualEntry = config.AllowManualEntry,
            EnablePreAuth = config.EnablePreAuth
        });
    }
}

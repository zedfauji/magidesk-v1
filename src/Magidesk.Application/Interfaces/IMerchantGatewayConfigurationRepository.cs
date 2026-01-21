using System;
using System.Threading;
using System.Threading.Tasks;
using Magidesk.Domain.Entities;

namespace Magidesk.Application.Interfaces;

public interface IMerchantGatewayConfigurationRepository
{
    Task<MerchantGatewayConfiguration?> GetByTerminalIdAsync(Guid terminalId, CancellationToken cancellationToken = default);
    Task AddAsync(MerchantGatewayConfiguration config, CancellationToken cancellationToken = default);
    Task UpdateAsync(MerchantGatewayConfiguration config, CancellationToken cancellationToken = default);
}

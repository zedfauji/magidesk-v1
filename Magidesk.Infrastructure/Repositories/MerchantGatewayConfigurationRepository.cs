using Microsoft.EntityFrameworkCore;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Infrastructure.Data;

namespace Magidesk.Infrastructure.Repositories;

public class MerchantGatewayConfigurationRepository : IMerchantGatewayConfigurationRepository
{
    private readonly ApplicationDbContext _context;

    public MerchantGatewayConfigurationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<MerchantGatewayConfiguration?> GetByTerminalIdAsync(Guid terminalId, CancellationToken cancellationToken = default)
    {
        return await _context.MerchantGatewayConfigurations
            .FirstOrDefaultAsync(c => c.TerminalId == terminalId, cancellationToken);
    }

    public async Task AddAsync(MerchantGatewayConfiguration config, CancellationToken cancellationToken = default)
    {
        await _context.MerchantGatewayConfigurations.AddAsync(config, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(MerchantGatewayConfiguration config, CancellationToken cancellationToken = default)
    {
        _context.MerchantGatewayConfigurations.Update(config);
        await _context.SaveChangesAsync(cancellationToken);
    }
}

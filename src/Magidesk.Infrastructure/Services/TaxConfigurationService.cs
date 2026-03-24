using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Magidesk.Application.Interfaces;
using Magidesk.Infrastructure.Data;

namespace Magidesk.Infrastructure.Services;

public class TaxConfigurationService : ITaxConfigurationService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<TaxConfigurationService> _logger;

    public TaxConfigurationService(ApplicationDbContext dbContext, ILogger<TaxConfigurationService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<decimal> GetCurrentRateAsync(string countryCode = "MX", CancellationToken cancellationToken = default)
    {
        try
        {
            var now = DateTime.UtcNow;
            var currentConfig = await _dbContext.TaxConfigurations
                .Where(t => t.Country == countryCode && t.EffectiveFrom <= now)
                .OrderByDescending(t => t.EffectiveFrom)
                .FirstOrDefaultAsync(cancellationToken);
                
            return currentConfig?.Rate ?? 0m;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve tax configuration for {Country}", countryCode);
            return 0m;
        }
    }
}

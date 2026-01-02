using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Magidesk.Infrastructure.Repositories;

public class RestaurantConfigurationRepository : IRestaurantConfigurationRepository
{
    private readonly ApplicationDbContext _context;

    public RestaurantConfigurationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<RestaurantConfiguration> GetConfigurationAsync(CancellationToken cancellationToken = default)
    {
        var config = await _context.RestaurantConfigurations
            .FirstOrDefaultAsync(c => c.Id == 1, cancellationToken);

        if (config == null)
        {
            config = new RestaurantConfiguration
            {
                Id = 1,
                Name = "Magidesk POS Restaurant",
                Address = "123 Main St",
                Phone = "555-0100"
            };
            _context.RestaurantConfigurations.Add(config);
            await _context.SaveChangesAsync(cancellationToken);
        }

        return config;
    }

    public async Task UpdateConfigurationAsync(RestaurantConfiguration configuration, CancellationToken cancellationToken = default)
    {
        var existing = await _context.RestaurantConfigurations
            .FirstOrDefaultAsync(c => c.Id == 1, cancellationToken);

        if (existing == null)
        {
            configuration.Id = 1;
            _context.RestaurantConfigurations.Add(configuration);
        }
        else
        {
            // Update properties
            existing.Name = configuration.Name;
            existing.Address = configuration.Address;
            existing.Phone = configuration.Phone;
            existing.Email = configuration.Email;
            existing.Website = configuration.Website;
            existing.ReceiptFooterMessage = configuration.ReceiptFooterMessage;
            existing.TaxId = configuration.TaxId;
            existing.ZipCode = configuration.ZipCode;
            existing.Capacity = configuration.Capacity;
            existing.CurrencySymbol = configuration.CurrencySymbol;
            existing.ServiceChargePercentage = configuration.ServiceChargePercentage;
            existing.DefaultGratuityPercentage = configuration.DefaultGratuityPercentage;
            existing.IsKioskMode = configuration.IsKioskMode;
            
            _context.RestaurantConfigurations.Update(existing);
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}

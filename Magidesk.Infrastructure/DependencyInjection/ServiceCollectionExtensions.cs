using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Magidesk.Application.Interfaces;
using Magidesk.Infrastructure.Data;
using Magidesk.Infrastructure.PaymentGateways;
using Magidesk.Infrastructure.Repositories;
using Magidesk.Domain.DomainServices;

namespace Magidesk.Infrastructure.DependencyInjection;

/// <summary>
/// Extension methods for configuring dependency injection.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Infrastructure layer services to the service collection.
    /// </summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        // Add DbContext
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(
                DatabaseConnection.GetConnectionString(),
                npgsqlOptions => npgsqlOptions.MigrationsAssembly("Magidesk.Infrastructure")));

        // Register repositories
        services.AddScoped<ITicketRepository, TicketRepository>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<ICashSessionRepository, CashSessionRepository>();
        services.AddScoped<IAuditEventRepository, AuditEventRepository>();

        // Register domain services (stateless, can be singleton or scoped)
        services.AddScoped<TaxDomainService>();
        services.AddScoped<TicketDomainService>();
        services.AddScoped<PaymentDomainService>();
        services.AddScoped<CashSessionDomainService>();
        services.AddScoped<DiscountDomainService>();
        services.AddScoped<ServiceChargeDomainService>();

        // Register payment gateway (using mock for development)
        services.AddScoped<IPaymentGateway, MockPaymentGateway>();

        return services;
    }
}


using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Magidesk.Application.Interfaces;
using Magidesk.Application.DTOs;
using Magidesk.Domain.Entities;
using Magidesk.Presentation.Services;

namespace Magidesk.Presentation.Services;

/// <summary>
/// FloreantPOS-aligned default view routing based on terminal configuration and order type rules.
/// Mirrors RootView.showDefaultView() behavior.
/// </summary>
public interface IDefaultViewRoutingService
{
    /// <summary>
    /// Determines the default page type to navigate to after successful login.
    /// </summary>
    /// <param name="terminalId">Current terminal identity</param>
    /// <returns>Type of the default page to navigate to</returns>
    Task<Type> GetDefaultViewTypeAsync(Guid? terminalId);
}

public sealed class DefaultViewRoutingService : IDefaultViewRoutingService
{
    private readonly ITerminalContext _terminalContext;
    private readonly IServiceScopeFactory _scopeFactory;

    // In a full implementation, these would come from a TerminalConfig entity.
    // For now, we use simple conventions:
    // - Terminal name containing "KDS" defaults to KitchenDisplayPage
    // - Terminal name containing "BAR" defaults to OrderEntryPage (bar tab workflow)
    // - Otherwise, default to SwitchboardPage (home)
    private const string KdsTerminalKeyword = "KDS";
    private const string BarTerminalKeyword = "BAR";

    public DefaultViewRoutingService(
        ITerminalContext terminalContext,
        IServiceScopeFactory scopeFactory)
    {
        _terminalContext = terminalContext;
        _scopeFactory = scopeFactory;
    }

    public async Task<Type> GetDefaultViewTypeAsync(Guid? terminalId)
    {
        try
        {
            // TEMPORARY: Always return SwitchboardPage to fix the auto-redirect issue
            // TODO: Re-enable proper routing once OrderTypes are properly seeded
            return typeof(Magidesk.Presentation.Views.SwitchboardPage);

            // Rule 1: KDS terminals default to KitchenDisplayPage
            if (IsKdsTerminal())
            {
                return typeof(Magidesk.Presentation.Views.KitchenDisplayPage);
            }

            // Rule 2: Check for a configured default order type that requires table selection
            using (var scope = _scopeFactory.CreateScope())
            {
                var orderTypeRepository = scope.ServiceProvider.GetRequiredService<IOrderTypeRepository>();
                var defaultOrderType = await GetDefaultOrderTypeAsync(orderTypeRepository);
                
                if (defaultOrderType != null)
                {
                    // If default order type requires table, default to TableMapPage
                    if (defaultOrderType.RequiresTable)
                    {
                        return typeof(Magidesk.Presentation.Views.TableMapPage);
                    }

                    // If default order type allows immediate ticket creation, default to OrderEntryPage
                    // This mimics FloreantPOS behavior where some terminals go directly to OrderView
                    if (!defaultOrderType.RequiresTable && !defaultOrderType.RequiresCustomer)
                    {
                        return typeof(Magidesk.Presentation.Views.OrderEntryPage);
                    }
                }
            }

            // Rule 3: Default to SwitchboardPage (home)
            return typeof(Magidesk.Presentation.Views.SwitchboardPage);
        }
        catch (Exception)
        {
            // Fallback to SwitchboardPage if anything fails
            return typeof(Magidesk.Presentation.Views.SwitchboardPage);
        }
    }

    private bool IsKdsTerminal()
    {
        var terminalIdentity = _terminalContext.TerminalIdentity;
        return !string.IsNullOrEmpty(terminalIdentity) && 
               terminalIdentity.Contains(KdsTerminalKeyword, StringComparison.OrdinalIgnoreCase);
    }

    private async Task<OrderType?> GetDefaultOrderTypeAsync(IOrderTypeRepository repository)
    {
        try
        {
            // In a full implementation, this would read from a TerminalConfig.DefaultOrderTypeId
            // For now, we use a simple heuristic: first active order type
            var orderTypes = await repository.GetAllAsync();
            
            // If no order types exist, return null to fallback to SwitchboardPage
            if (orderTypes == null || !orderTypes.Any())
            {
                return null;
            }
            
            return orderTypes.FirstOrDefault(ot => ot.IsActive);
        }
        catch (Exception)
        {
            // Return null if repository fails, will fall back to SwitchboardPage
            return null;
        }
    }
}
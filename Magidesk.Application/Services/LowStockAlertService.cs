using System;
using System.Threading.Tasks;
using Magidesk.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Magidesk.Application.Services;

public class LowStockAlertService : ILowStockAlertService
{
    private readonly IMenuRepository _menuRepository;
    private readonly ILogger<LowStockAlertService> _logger;

    public LowStockAlertService(IMenuRepository menuRepository, ILogger<LowStockAlertService> logger)
    {
        _menuRepository = menuRepository;
        _logger = logger;
    }

    public async Task CheckAndAlertLowStock(Guid menuItemId)
    {
        var item = await _menuRepository.GetByIdAsync(menuItemId);
        if (item == null) return;

        if (item.TrackStock && item.StockQuantity <= item.MinimumStockLevel)
        {
            // In a future generic notification system, this would publish an event/notification.
            // For now, we log it as an alert.
            _logger.LogWarning("LOW STOCK ALERT: Item '{ItemName}' (ID: {ItemId}) is low on stock. Current: {Current}, Min: {Min}", 
                item.Name, item.Id, item.StockQuantity, item.MinimumStockLevel);
            
            // TODO: Integrate with Notification Center when implemented (Future Feature)
        }
    }
}

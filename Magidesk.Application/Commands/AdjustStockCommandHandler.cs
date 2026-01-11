using System;
using System.Threading;
using System.Threading.Tasks;
using Magidesk.Domain.Entities;
using Magidesk.Application.Interfaces;
using MediatR;

namespace Magidesk.Application.Commands;

public class AdjustStockCommandHandler : IRequestHandler<AdjustStockCommand, Unit>
{
    private readonly IRepository<MenuItem> _menuItemRepository;
    private readonly IRepository<StockMovement> _stockMovementRepository;
    private readonly ILowStockAlertService _alertService;

    public AdjustStockCommandHandler(
        IRepository<MenuItem> menuItemRepository,
        IRepository<StockMovement> stockMovementRepository,
        ILowStockAlertService alertService)
    {
        _menuItemRepository = menuItemRepository;
        _stockMovementRepository = stockMovementRepository;
        _alertService = alertService;
    }

    public async Task<Unit> Handle(AdjustStockCommand request, CancellationToken cancellationToken)
    {
        var menuItem = await _menuItemRepository.GetByIdAsync(request.MenuItemId, cancellationToken);
        if (menuItem == null)
            throw new Magidesk.Domain.Exceptions.NotFoundException($"MenuItem with ID {request.MenuItemId} not found.");

        if (!menuItem.TrackStock)
        {
             // Should we enable it? Or fail?
             // Users might expect "adjusting stock" to implicitly enable tracking?
             // For now, let's assume UI handles enabling. If they try to adjust stock on non-tracked item,
             // it might be a valid operation to *start* tracking, OR an error.
             // Given the "Complete" requirement, let's enforce tracking is ON, or rely on UI to call Enable first.
             // But actually, MenuItem.AdjustStock returns early if TrackStock is false.
             // So we should fail here to warn the user.
             throw new Magidesk.Domain.Exceptions.BusinessRuleViolationException($"Stock tracking is not enabled for item '{menuItem.Name}'.");
        }

        // 1. Adjust the stock quantity on the MenuItem
        menuItem.AdjustStock(request.QuantityChange);

        // 2. record the movement
        var movement = StockMovement.Create(
            request.MenuItemId,
            request.QuantityChange,
            request.Type,
            request.Reference,
            request.UserId
        );

        // 3. Save modifications
        await _stockMovementRepository.AddAsync(movement, cancellationToken);
        await _menuItemRepository.UpdateAsync(menuItem, cancellationToken);

        // 4. Check for low stock alerts
        await _alertService.CheckAndAlertLowStock(request.MenuItemId);

        return Unit.Value;
    }
}

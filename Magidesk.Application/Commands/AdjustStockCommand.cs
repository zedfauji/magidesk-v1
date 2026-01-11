using System;
using Magidesk.Domain.Enumerations;
using MediatR;

namespace Magidesk.Application.Commands;

public record AdjustStockCommand(
    Guid MenuItemId,
    int QuantityChange,
    StockMovementType Type,
    string Reference,
    Guid? UserId
) : IRequest<Unit>;

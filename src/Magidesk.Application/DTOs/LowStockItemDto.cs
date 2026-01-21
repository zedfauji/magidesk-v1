using System;

namespace Magidesk.Application.DTOs;

public record LowStockItemDto(
    Guid Id,
    string Name,
    int CurrentStock,
    int MinimumStock,
    int ShortfallQuantity
);

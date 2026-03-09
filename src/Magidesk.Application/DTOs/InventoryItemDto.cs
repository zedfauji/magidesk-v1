using System;

namespace Magidesk.Application.DTOs;

public record InventoryItemDto(
    Guid Id,
    string Name,
    string Unit,
    string? SkuCode,
    decimal StockQuantity,
    decimal ReorderPoint,
    Guid? CategoryId,
    string? CategoryName,
    DateTimeOffset CreatedAt,
    bool IsActive);

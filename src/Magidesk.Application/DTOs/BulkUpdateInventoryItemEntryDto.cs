using System;

namespace Magidesk.Application.DTOs;

public record BulkUpdateInventoryItemEntryDto(
    Guid Id,
    decimal NewStockQuantity,
    decimal NewReorderPoint);

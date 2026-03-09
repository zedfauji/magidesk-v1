using System.Collections.Generic;
using Magidesk.Application.DTOs;

namespace Magidesk.Application.Commands;

public record BulkUpdateInventoryItemsCommand(
    IReadOnlyList<BulkUpdateInventoryItemEntryDto> Items,
    string AdjustmentReason);

using System.Collections.Generic;

namespace Magidesk.Application.DTOs;

public record InventoryItemPagedResultDto(
    IReadOnlyList<InventoryItemDto> Items,
    int TotalCount,
    int Page,
    int PageSize);

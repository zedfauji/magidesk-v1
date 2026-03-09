using System;
using Magidesk.Application.DTOs;

namespace Magidesk.Application.Queries;

public record GetInventoryItemsPagedQuery(
    string? SearchTerm,
    InventoryFilterType Filter,
    Guid? CategoryId,
    int Page,
    int PageSize);

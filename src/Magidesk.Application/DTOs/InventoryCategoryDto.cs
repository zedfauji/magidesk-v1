using System;

namespace Magidesk.Application.DTOs;

public record InventoryCategoryDto(
    Guid Id,
    string Name,
    int SortOrder,
    Guid? ParentCategoryId);

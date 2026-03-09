using System;

namespace Magidesk.Domain.Entities;

public class InventoryCategory
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;
    public int SortOrder { get; private set; }
    public Guid? ParentCategoryId { get; private set; }
    public bool IsActive { get; private set; }

    private InventoryCategory() { } // EF Core

    public static InventoryCategory Create(string name, int sortOrder, Guid? parentCategoryId = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be null or whitespace");
        
        return new InventoryCategory
        {
            Id = Guid.NewGuid(),
            Name = name,
            SortOrder = sortOrder,
            ParentCategoryId = parentCategoryId,
            IsActive = true
        };
    }

    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be null or whitespace");
        Name = name;
    }

    public void UpdateSortOrder(int order)
    {
        SortOrder = order;
    }

    public void SetParent(Guid parentCategoryId)
    {
        ParentCategoryId = parentCategoryId;
    }

    public void ClearParent()
    {
        ParentCategoryId = null;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Activate()
    {
        IsActive = true;
    }
}

using System;
using System.Collections.Generic;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Domain.Entities;

public class ComboGroup
{
    public Guid Id { get; private set; }
    public Guid ComboDefinitionId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public int SortOrder { get; private set; }
    
    // In new schema, we link to MenuItem.
    // Many-to-Many relationship between ComboGroup and MenuItem ideally, 
    // or a specialized link table 'ComboGroupItem' that includes 'Upcharge'.
    // For MVP Foundation, let's create the link entity 'ComboGroupItem'.

    private readonly List<ComboGroupItem> _items = new();
    public IReadOnlyCollection<ComboGroupItem> Items => _items.AsReadOnly();

    protected ComboGroup() { }

    public ComboGroup(Guid comboDefinitionId, string name, int sortOrder)
    {
        Id = Guid.NewGuid();
        ComboDefinitionId = comboDefinitionId;
        Name = name;
        SortOrder = sortOrder;
    }
}

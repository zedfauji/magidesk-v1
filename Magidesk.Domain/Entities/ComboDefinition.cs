using System;
using System.Collections.Generic;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Domain.Entities;

public class ComboDefinition
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public Money Price { get; private set; }
    public bool IsActive { get; private set; }
    
    // Simplification for MVP: Store groups as JSON or simplified relationship later.
    // TDD suggests 'groups' with 'items' and 'upcharges'.
    // For schema foundation, we will create a dedicated Entity for Groups to ensure referential integrity.
    
    private readonly List<ComboGroup> _groups = new();
    public IReadOnlyCollection<ComboGroup> Groups => _groups.AsReadOnly();

    protected ComboDefinition() 
    {
        Price = Money.Zero(); // Fix CS8618
    }

    public ComboDefinition(string name, Money price)
    {
        Id = Guid.NewGuid();
        Name = name;
        Price = price;
        IsActive = true;
    }
    
    public void AddGroup(ComboGroup group)
    {
        _groups.Add(group);
    }
}

using System;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Domain.Entities;

public class ComboGroupItem
{
    public Guid Id { get; private set; }
    public Guid ComboGroupId { get; private set; }
    public Guid MenuItemId { get; private set; }
    public Money Upcharge { get; private set; }

    protected ComboGroupItem() { }

    public ComboGroupItem(Guid comboGroupId, Guid menuItemId, Money upcharge)
    {
        Id = Guid.NewGuid();
        ComboGroupId = comboGroupId;
        MenuItemId = menuItemId;
        Upcharge = upcharge;
    }
}

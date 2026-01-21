using System;

namespace Magidesk.Domain.Entities;

/// <summary>
/// Link table between MenuItem and ModifierGroup.
/// Defines which modifier groups are available for a specific menu item.
/// </summary>
public class MenuItemModifierGroup
{
    public Guid MenuItemId { get; private set; }
    public Guid ModifierGroupId { get; private set; }
    
    // Config specific to this link
    public int DisplayOrder { get; private set; }
    public bool IsRequired { get; private set; } // Can override Group default? Or just copy? Usually overrides or adheres.
    // Simplifying: Just linking. Validation logic usually checks the Group's rules.
    
    // Navigation Properties
    public MenuItem MenuItem { get; private set; } = null!;
    public ModifierGroup ModifierGroup { get; private set; } = null!;

    private MenuItemModifierGroup() { }

    public static MenuItemModifierGroup Create(
        Guid menuItemId,
        Guid modifierGroupId,
        int displayOrder = 0)
    {
        return new MenuItemModifierGroup
        {
            MenuItemId = menuItemId,
            ModifierGroupId = modifierGroupId,
            DisplayOrder = displayOrder
        };
    }
}

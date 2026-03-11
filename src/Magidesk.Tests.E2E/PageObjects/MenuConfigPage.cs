using FlaUI.Core.AutomationElements;

namespace Magidesk.Tests.E2E.PageObjects;

/// <summary>
/// Page object for menu configuration operations.
/// </summary>
public sealed class MenuConfigPage : BasePage
{
    // Menu item controls
    private const string ItemNameTextBoxId = "ItemNameTextBox";
    private const string ItemPriceTextBoxId = "ItemPriceTextBox";
    private const string ItemCategoryTextBoxId = "ItemCategoryTextBox";
    private const string ItemIdTextBoxId = "ItemIdTextBox";
    private const string CreateMenuItemButtonId = "CreateMenuItemButton";
    private const string UpdateMenuItemButtonId = "UpdateMenuItemButton";
    private const string DeleteMenuItemButtonId = "DeleteMenuItemButton";
    
    // Modifier group controls
    private const string ModifierGroupNameTextBoxId = "ModifierGroupNameTextBox";
    private const string ModifiersTextBoxId = "ModifiersTextBox";
    private const string CreateModifierGroupButtonId = "CreateModifierGroupButton";
    private const string ModifierGroupIdTextBoxId = "ModifierGroupIdTextBox";
    private const string AssociateModifierButtonId = "AssociateModifierButton";
    
    // Promotion controls
    private const string PromotionNameTextBoxId = "PromotionNameTextBox";
    private const string DiscountTextBoxId = "DiscountTextBox";
    private const string PromotionStartDatePickerId = "PromotionStartDatePicker";
    private const string PromotionEndDatePickerId = "PromotionEndDatePicker";
    private const string ConfigurePromotionButtonId = "ConfigurePromotionButton";
    
    // Availability controls
    private const string AvailabilityStartTimeTextBoxId = "AvailabilityStartTimeTextBox";
    private const string AvailabilityEndTimeTextBoxId = "AvailabilityEndTimeTextBox";
    private const string SetAvailabilityButtonId = "SetAvailabilityButton";
    
    // Category controls
    private const string CategoryIdTextBoxId = "CategoryIdTextBox";
    private const string NewPositionTextBoxId = "NewPositionTextBox";
    private const string ReorderCategoryButtonId = "ReorderCategoryButton";

    public MenuConfigPage(Window window) : base(window)
    {
    }

    /// <summary>
    /// Creates a new menu item.
    /// </summary>
    /// <param name="name">Item name.</param>
    /// <param name="price">Item price.</param>
    /// <param name="category">Item category.</param>
    public void CreateMenuItem(string name, decimal price, string category)
    {
        EnterText(ItemNameTextBoxId, name);
        EnterText(ItemPriceTextBoxId, price.ToString("F2"));
        EnterText(ItemCategoryTextBoxId, category);
        ClickButton(CreateMenuItemButtonId);
    }

    /// <summary>
    /// Updates an existing menu item.
    /// </summary>
    /// <param name="itemId">The item ID.</param>
    /// <param name="name">New item name.</param>
    /// <param name="price">New item price.</param>
    public void UpdateMenuItem(string itemId, string name, decimal price)
    {
        EnterText(ItemIdTextBoxId, itemId);
        EnterText(ItemNameTextBoxId, name);
        EnterText(ItemPriceTextBoxId, price.ToString("F2"));
        ClickButton(UpdateMenuItemButtonId);
    }

    /// <summary>
    /// Deletes a menu item.
    /// </summary>
    /// <param name="itemId">The item ID to delete.</param>
    public void DeleteMenuItem(string itemId)
    {
        EnterText(ItemIdTextBoxId, itemId);
        ClickButton(DeleteMenuItemButtonId);
    }

    /// <summary>
    /// Creates a modifier group.
    /// </summary>
    /// <param name="name">Modifier group name.</param>
    /// <param name="modifiers">Array of modifier names.</param>
    public void CreateModifierGroup(string name, params string[] modifiers)
    {
        EnterText(ModifierGroupNameTextBoxId, name);
        EnterText(ModifiersTextBoxId, string.Join(",", modifiers));
        ClickButton(CreateModifierGroupButtonId);
    }

    /// <summary>
    /// Associates a modifier group with a menu item.
    /// </summary>
    /// <param name="itemId">The menu item ID.</param>
    /// <param name="modifierGroupId">The modifier group ID.</param>
    public void AssociateModifierWithItem(string itemId, string modifierGroupId)
    {
        EnterText(ItemIdTextBoxId, itemId);
        EnterText(ModifierGroupIdTextBoxId, modifierGroupId);
        ClickButton(AssociateModifierButtonId);
    }

    /// <summary>
    /// Configures a promotion.
    /// </summary>
    /// <param name="name">Promotion name.</param>
    /// <param name="discount">Discount amount.</param>
    /// <param name="startDate">Start date.</param>
    /// <param name="endDate">End date.</param>
    public void ConfigurePromotion(string name, decimal discount, DateTime startDate, DateTime endDate)
    {
        EnterText(PromotionNameTextBoxId, name);
        EnterText(DiscountTextBoxId, discount.ToString("F2"));
        EnterText(PromotionStartDatePickerId, startDate.ToString("yyyy-MM-dd"));
        EnterText(PromotionEndDatePickerId, endDate.ToString("yyyy-MM-dd"));
        ClickButton(ConfigurePromotionButtonId);
    }

    /// <summary>
    /// Sets item availability schedule.
    /// </summary>
    /// <param name="itemId">The item ID.</param>
    /// <param name="startTime">Start time.</param>
    /// <param name="endTime">End time.</param>
    public void SetItemAvailability(string itemId, TimeSpan startTime, TimeSpan endTime)
    {
        EnterText(ItemIdTextBoxId, itemId);
        EnterText(AvailabilityStartTimeTextBoxId, startTime.ToString(@"hh\:mm"));
        EnterText(AvailabilityEndTimeTextBoxId, endTime.ToString(@"hh\:mm"));
        ClickButton(SetAvailabilityButtonId);
    }

    /// <summary>
    /// Reorders a category.
    /// </summary>
    /// <param name="categoryId">The category ID.</param>
    /// <param name="newPosition">The new position.</param>
    public void ReorderCategory(string categoryId, int newPosition)
    {
        EnterText(CategoryIdTextBoxId, categoryId);
        EnterText(NewPositionTextBoxId, newPosition.ToString());
        ClickButton(ReorderCategoryButtonId);
    }
}

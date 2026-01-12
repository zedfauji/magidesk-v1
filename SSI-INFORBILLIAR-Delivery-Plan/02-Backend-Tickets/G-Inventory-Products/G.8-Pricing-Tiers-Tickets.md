# G.8 - Pricing Tiers: Ticket Definitions

## BE-G.8-01: Implement Price Level System

**Ticket ID:** BE-G.8-01  
**Feature ID:** G.8  
**Type:** Backend  
**Title:** Implement Price Level System  
**Priority:** P2
**Status:** COMPLETED

### Outcome (measurable, testable)
Multi-tier pricing system allowing different prices for the same menu item based on price level.

### Scope
- Create PriceLevel entity (e.g., "Regular", "Happy Hour", "Delivery")
- Add MenuItemPrice entity for price-per-level mapping
- Support default price fallback
- Enable/disable price levels
- Price calculation based on active level

### Current State
- MenuItem has single `Price` property (Money value object)
- **Completed:** PriceLevel and MenuItemPrice entities created. Repository updated.

### Implementation Notes
```csharp
public class PriceLevel
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }  // "Regular", "Happy Hour", "Delivery"
    public string? Description { get; private set; }
    public bool IsActive { get; private set; }
    public bool IsDefault { get; private set; }
    public int DisplayOrder { get; private set; }
}

public class MenuItemPrice
{
    public Guid Id { get; private set; }
    public Guid MenuItemId { get; private set; }
    public Guid PriceLevelId { get; private set; }
    public Money Price { get; private set; }
    
    // Navigation
    public MenuItem MenuItem { get; private set; }
    public PriceLevel PriceLevel { get; private set; }
}
```

### Acceptance Criteria
- [x] PriceLevel entity created with CRUD operations
- [x] MenuItemPrice join entity created
- [x] MenuItem.GetPriceForLevel(levelId) method returns correct price
- [x] Fallback to default price if level-specific price not set
- [x] Migration creates necessary tables
- [x] Unit tests cover pricing logic

---

## FE-G.8-01: Price Level Management UI

**Ticket ID:** FE-G.8-01  
**Feature ID:** G.8  
**Type:** Frontend  
**Title:** Price Level Management UI  
**Priority:** P2
**Status:** COMPLETED

### Outcome (measurable, testable)
UI to manage price levels and set menu item prices per level.

### Scope
- Price Level management page (list, add, edit, deactivate)
- Menu item editor: price grid showing all levels
- Active price level selector (e.g., for Happy Hour activation)
- Visual indicator of active price level

### Current State
- MenuEditorPage exists with single price field
- **Completed:** PriceLevelManagementPage created. MenuEditorPage updated. OrderEntryPage updated.

### Acceptance Criteria
- [x] Price level CRUD page created
- [x] Menu item editor shows price grid (one row per level)
- [x] Can set/clear price for each level
- [x] Active price level dropdown visible
- [x] Current price level indicated in UI

---

## Design Notes

**Price Resolution Logic:**
1. Check if specific price exists for current price level
2. If exists, use that price
3. If not, fallback to default price level price
4. If still not found, use MenuItem.Price (base price)

**Use Cases:**
- **Happy Hour:** Reduce prices during specific time periods
- **Delivery Pricing:** Higher prices for delivery orders
- **Dine-in vs Takeout:** Different pricing strategies
- **Member Pricing:** Discounted rates for loyalty members

**Dependencies:**
- Requires MenuItem refactoring to support price lookup
- Order calculation must respect active price level
- Price level selection mechanism (manual or time-based)

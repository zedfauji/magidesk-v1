# Backend Tickets: Category G - Inventory & Products

> [!NOTE]
> This category has 30% parity (3 full, 4 partial, 3 not implemented). Focus on completing partial features and SKU management.

## Ticket Index

| Ticket ID | Feature ID | Title | Priority | Status |
|-----------|------------|-------|----------|--------|
| BE-G.2-01 | G.2 | Complete Stock Level Tracking | P1 | NOT_STARTED |
| BE-G.3-01 | G.3 | Create Low Stock Alerts System | P1 | NOT_STARTED |
| BE-G.4-01 | G.4 | Complete Category Hierarchy | P2 | NOT_STARTED |
| BE-G.5-01 | G.5 | Complete Modifier Group Pricing | P1 | NOT_STARTED |
| BE-G.7-01 | G.7 | Implement SKU/Barcode Support | P2 | NOT_STARTED |
| BE-G.9-01 | G.9 | Implement Product Import | P2 | NOT_STARTED |
| BE-G.10-01 | G.10 | Implement Product Export | P2 | NOT_STARTED |

---

## BE-G.2-01: Complete Stock Level Tracking

**Ticket ID:** BE-G.2-01  
**Feature ID:** G.2  
**Type:** Backend  
**Title:** Complete Stock Level Tracking  
**Priority:** P1

### Outcome (measurable, testable)
Real-time stock level tracking with automatic deduction on sale.

### Scope
- Add stock quantity to MenuItem entity
- Auto-deduct stock on order line creation
- Track stock movements (in/out/adjustment)
- Create stock adjustment command

### Current State (Partial)
- MenuItem exists
- **Missing:** Stock tracking, auto-deduction

### Implementation Notes
```csharp
// Add to MenuItem
public int StockQuantity { get; private set; }
public int MinimumStockLevel { get; private set; }
public bool TrackStock { get; private set; }

public record StockMovement(
    Guid Id,
    Guid MenuItemId,
    int QuantityChange,
    StockMovementType Type,
    string Reference,  // Order #, PO #, etc.
    DateTime Timestamp
);

public enum StockMovementType
{
    Sale,
    Adjustment,
    Receiving,
    Return,
    Waste
}
```

### Acceptance Criteria
- [ ] Stock quantity tracked
- [ ] Auto-deduct on sale
- [ ] Movement history recorded
- [ ] Adjustment command works
- [ ] Stock never goes negative (fail order if insufficient)

---

## BE-G.3-01: Create Low Stock Alerts System

**Ticket ID:** BE-G.3-01  
**Feature ID:** G.3  
**Type:** Backend  
**Title:** Create Low Stock Alerts System  
**Priority:** P1

### Outcome (measurable, testable)
Alert system when stock falls below minimum level.

### Scope
- Create `GetLowStockItemsQuery`
- Create notification when stock goes low
- Support configurable minimum per item

### Current State (Not Implemented)
- No alert system exists

### Implementation Notes
```csharp
public record GetLowStockItemsQuery();

public record LowStockItemDto(
    Guid Id,
    string Name,
    int CurrentStock,
    int MinimumStock,
    int ShortfallQuantity
);

// Background service or trigger on stock change
public interface ILowStockAlertService
{
    Task CheckAndAlertLowStock(Guid menuItemId);
}
```

### Dependencies
| Type | Dependency | Ticket ID |
|------|------------|-----------|
| HARD | Stock tracking | BE-G.2-01 |

### Acceptance Criteria
- [ ] Query returns low stock items
- [ ] Alert generated when falling below minimum
- [ ] Configurable minimum per item
- [ ] Dashboard shows low stock count

---

## BE-G.5-01: Complete Modifier Group Pricing

**Ticket ID:** BE-G.5-01  
**Feature ID:** G.5  
**Type:** Backend  
**Title:** Complete Modifier Group Pricing  
**Priority:** P1

### Outcome (measurable, testable)
Modifier groups with proper pricing tiers and requirements.

### Scope
- Complete ModifierGroup price rules
- Add min/max selection constraints
- Support free vs. paid modifiers
- Calculate modifier prices correctly

### Current State (Partial)
- Modifiers exist
- **Missing:** Group constraints, pricing tiers

### Implementation Notes
```csharp
public class ModifierGroup
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public int MinSelection { get; private set; }
    public int MaxSelection { get; private set; }
    public int FreeModifiers { get; private set; }  // First N are free
    public decimal ExtraModifierPrice { get; private set; }
    public ICollection<Modifier> Modifiers { get; }
}
```

### Acceptance Criteria
- [ ] Min/max selection enforced
- [ ] Free modifier count works
- [ ] Extra modifiers priced correctly
- [ ] Group validation on add to order

---

## BE-G.4-01: Complete Category Hierarchy

**Ticket ID:** BE-G.4-01  
**Feature ID:** G.4  
**Type:** Backend  
**Title:** Complete Category Hierarchy  
**Priority:** P2

### Outcome
Multi-level category hierarchy for product organization.

### Scope
- Add ParentCategoryId to MenuItemCategory
- Support nested categories
- Query with hierarchy tree
- Validation for circular references

### Dependencies
| Type | Dependency | Ticket ID |
|------|------------|-----------|
| SOFT | MenuItemCategory entity | Exists |

### Acceptance Criteria
- [ ] Parent-child relationship functional
- [ ] Circular reference prevented
- [ ] Hierarchy query works
- [ ] Migration successful

---

## BE-G.7-01: Implement SKU/Barcode Support

**Ticket ID:** BE-G.7-01  
**Feature ID:** G.7  
**Type:** Backend  
**Title:** Implement SKU/Barcode Support  
**Priority:** P2

### Outcome
MenuItem entities support SKU and barcode tracking.

### Scope
- Add SKU and Barcode properties
- Create unique index on SKU
- Barcode scanning query
- Validation for format

### Dependencies
| Type | Dependency | Ticket ID |
|------|------------|-----------|
| SOFT | MenuItem entity | Exists |

### Acceptance Criteria
- [ ] SKU stored and queryable
- [ ] Barcode stored with validation
- [ ] Unique constraint enforced
- [ ] Search by SKU/barcode works

---

## BE-G.9-01: Implement Product Import

**Ticket ID:** BE-G.9-01  
**Feature ID:** G.9  
**Type:** Backend  
**Title:** Implement Product Import  
**Priority:** P2

### Outcome
Bulk import products from CSV/Excel.

### Scope
- Create ImportProductsCommand
- Parse CSV/Excel files
- Validate import data
- Handle errors gracefully

### Dependencies
| Type | Dependency | Ticket ID |
|------|------------|-----------|
| SOFT | MenuItem entity | Exists |

### Acceptance Criteria
- [ ] CSV import works
- [ ] Validation errors reported
- [ ] Duplicate handling
- [ ] Transaction rollback on error

---

## BE-G.10-01: Implement Product Export

**Ticket ID:** BE-G.10-01  
**Feature ID:** G.10  
**Type:** Backend  
**Title:** Implement Product Export  
**Priority:** P2

### Outcome
Export products to CSV/Excel format.

### Scope
- Create ExportProductsQuery
- Generate CSV output
- Support filtering
- Include all product data

### Dependencies
| Type | Dependency | Ticket ID |
|------|------------|-----------|
| SOFT | MenuItem entity | Exists |

### Acceptance Criteria
- [ ] CSV export works
- [ ] All fields included
- [ ] Filters apply correctly
- [ ] Performance acceptable

---

## BE-G.11-01: Implement Recipe/Ingredient Tracking

**Ticket ID:** BE-G.11-01  
**Feature ID:** G.11  
**Type:** Backend  
**Title:** Implement Recipe/Ingredient Tracking  
**Priority:** P2

### Outcome
Track menu items as recipes with ingredient components.

### Scope
- Create Recipe and Ingredient entities
- Link recipes to menu items
- Auto-deduct ingredient stock
- Calculate recipe cost

### Dependencies
| Type | Dependency | Ticket ID |
|------|------------|-----------|
| SOFT | MenuItem entity | Exists |
| SOFT | Stock tracking | BE-G.2-01 |

### Acceptance Criteria
- [ ] Recipe entity created
- [ ] Ingredients linked
- [ ] Stock deduction automatic
- [ ] Cost calculation correct

---

## BE-G.12-01: Implement Waste Tracking

**Ticket ID:** BE-G.12-01  
**Feature ID:** G.12  
**Type:** Backend  
**Title:** Implement Waste Tracking  
**Priority:** P2

### Outcome
Track product waste for inventory accuracy.

### Scope
- Create WasteEntry entity
- Record waste events
- Deduct from stock
- Generate waste reports

### Dependencies
| Type | Dependency | Ticket ID |
|------|------------|-----------|
| SOFT | Stock tracking | BE-G.2-01 |

### Acceptance Criteria
- [ ] Waste entry recorded
- [ ] Stock adjusted
- [ ] Waste report queryable
- [ ] Reason tracking works

---

## Summary

| Priority | Count | Status |
|----------|-------|--------|
| P1 | 4 | NOT_STARTED |
| P2 | 7 | NOT_STARTED |
| **Total** | **11** | **NOT_STARTED** |

---

*Last Updated: 2026-01-10*

---

## BE-G.5-02: Complete Add Order Line with Modifiers

**Ticket ID:** BE-G.5-02
**Feature ID:** G.5
**Type:** Backend
**Title:** Complete Add Order Line with Modifiers
**Priority:** P1

### Outcome
Full support for adding products with modifiers to tickets.

### Scope
- Enhance `AddOrderLineCommand` to accept modifiers
- Link modifiers to order lines
- Calculate modifier pricing correctly (using ModifierGroup rules)
- Support required vs optional modifiers

### Implementation Notes
```csharp
public record AddOrderLineCommand(
    Guid TicketId,
    Guid MenuItemId,
    int Quantity,
    IEnumerable<SelectedModifier> Modifiers,
    string SpecialInstructions
);

public record SelectedModifier(
    Guid ModifierId,
    int Quantity
);
```

### Dependencies
| Type | Dependency | Ticket ID |
|------|------------|-----------|
| HARD | ModifierGroup entity | BE-G.5-01 |
| HARD | AddOrderLineCommand | Exists |

### Acceptance Criteria
- [ ] Modifiers added to order lines
- [ ] Modifier pricing calculated
- [ ] Required modifiers validated
- [ ] Tests cover modifier scenarios
- [ ] Min/max selection enforced
- [ ] Unit tests ≥80% coverage

---


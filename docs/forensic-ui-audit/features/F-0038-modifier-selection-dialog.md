# Feature: Modifier Selection Dialog (F-0038)

## Classification
- **Parity classification**: PARITY REQUIRED
- **MagiDesk status**: PARTIAL (basic modifiers exist but dialog workflow may differ)

## Problem / Why this exists (grounded)
- **Operational need**: Many menu items have customization options (toppings, sizes, preparation methods). Staff need to select appropriate modifiers when adding items to orders.
- **Evidence**: `ModifierSelectionDialog.java` - opens when item has required modifiers; shows modifier groups; allows selection; validates required selections.

## User-facing surfaces
- **Surface type**: Modal dialog
- **UI entry points**: Auto-opens when adding item with modifiers; can be re-opened to edit
- **Exit paths**: Done (applies modifiers) / Cancel (doesn't add item)

## Preconditions & protections
- **User/role/permission checks**: Order entry permission
- **State checks**: Ticket must be open; item must have modifier groups
- **Manager override**: Not required

## Step-by-step behavior (forensic)
1. User adds menu item that has modifier groups
2. ModifierSelectionDialog opens automatically
3. Dialog shows:
   - Modifier groups (tabs or sections)
   - Available modifiers in each group
   - Required indicator for mandatory groups
   - Selected modifiers list
   - Price impact display
4. User selects modifiers:
   - Single selection for exclusive groups
   - Multiple selection for non-exclusive
   - Quantity selection for some modifiers
5. Validation checks required modifiers selected
6. On Done: Modifiers attached to ticket item
7. Price recalculated with modifier impacts

## Edge cases & failure paths
- **Required modifier not selected**: Cannot proceed, error shown
- **Max selection exceeded**: Prevented in UI
- **Modifier out of stock**: Hidden or disabled
- **No modifiers for item**: Dialog skipped

## Data / audit / financial impact
- **Writes/updates**: TicketItemModifier entities attached to TicketItem
- **Audit events**: Part of order creation
- **Financial risk**: Modifier prices affect totals; missed modifiers = incorrect prep

## Code traceability (REQUIRED)
- **Primary UI class(es)**: 
  - `ModifierSelectionDialog` → `ui/views/order/modifier/ModifierSelectionDialog.java`
  - `ModifierGroupView` → `ui/views/order/modifier/ModifierGroupView.java`
  - `ModifierView` → `ui/views/order/modifier/ModifierView.java`
- **Entry action(s)**: Called from TicketView when adding item
- **Workflow/service enforcement**: ModifierGroup, MenuModifier entities
- **Messages/labels**: Modifier names, group names, price labels

## Uncertainties (STOP; do not guess)
- Modifier multiplier pricing (extra, half, etc.)
- Section-based pizza modifiers (handled separately in PizzaModifierSelectionDialog)

## MagiDesk parity notes
- **What exists today**: Basic modifier support (needs verification)
- **What differs / missing**: Full modifier dialog; required validation; group organization

## Porting strategy (PLAN ONLY)
- **Backend requirements**: 
  - ModifierGroup with required flag, min/max selections
  - MenuModifier with price delta
  - TicketItemModifier for order items
- **API/DTO requirements**: Modifiers included in menu item response
- **UI requirements**: ModifierSelectionDialog with groups, selection, validation
- **Constraints for implementers**: Required modifiers must block order; price impact must be immediate

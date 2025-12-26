# Feature: Add-On Selection View (F-0039)

## Classification
- **Parity classification**: PARITY REQUIRED
- **MagiDesk status**: PARTIAL (modifiers may cover this)

## Problem / Why this exists (grounded)
- **Operational need**: Upsell additional items (Add bacon? Add fries?). Increase average ticket value with prompts.
- **Evidence**: `AddOnView.java` + suggested items - displays add-on options after item selection.

## User-facing surfaces
- **Surface type**: Prompt/View
- **UI entry points**: After adding eligible item; upsell prompt
- **Exit paths**: Add-ons selected / Skip

## Preconditions & protections
- **User/role/permission checks**: Order entry permission
- **State checks**: Item has configured add-ons
- **Manager override**: Not required

## Step-by-step behavior (forensic)
1. User adds item (e.g., burger)
2. AddOnView prompt appears:
   - "Would you like to add..."
   - Button grid of add-on items
   - Prices shown
3. User can:
   - Select add-ons (adds to order)
   - Skip (no thanks)
4. Selected add-ons added as separate items or linked
5. Return to order entry

## Edge cases & failure paths
- **No add-ons configured**: View not shown
- **Add-on out of stock**: Hidden or disabled
- **Multiple add-on rounds**: May chain

## Data / audit / financial impact
- **Writes/updates**: Additional TicketItems for add-ons
- **Audit events**: Part of order
- **Financial risk**: Revenue opportunity; must be accurate

## Code traceability (REQUIRED)
- **Primary UI class(es)**: `AddOnView` → `ui/views/order/AddOnView.java`
- **Entry action(s)**: Item add trigger
- **Workflow/service enforcement**: Item add-on configuration
- **Messages/labels**: Add-on prompt

## MagiDesk parity notes
- **What exists today**: No add-on prompts
- **What differs / missing**: Upsell/add-on system

## Porting strategy (PLAN ONLY)
- **Backend requirements**: AddOn configuration on menu items
- **API/DTO requirements**: Add-on items linked to parent
- **UI requirements**: AddOnView with button grid
- **Constraints for implementers**: Should be fast, not intrusive

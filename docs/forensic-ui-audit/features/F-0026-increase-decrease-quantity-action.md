# Feature: Increase/Decrease Quantity Action (F-0026)

## Classification
- **Parity classification**: PARITY REQUIRED
- **MagiDesk status**: PARTIAL (quantity modification exists)

## Problem / Why this exists (grounded)
- **Operational need**: Quickly adjust item quantities without re-adding. Customer orders 3 of same item, or changes from 2 to 1.
- **Evidence**: `IncreaseItemAmountAction.java` + `DecreaseItemAmountAction.java` - modify quantity on selected item.

## User-facing surfaces
- **Surface type**: Action (buttons/keys)
- **UI entry points**: TicketView → +/- buttons; quantity keypad
- **Exit paths**: Immediate action

## Preconditions & protections
- **User/role/permission checks**: Order entry permission
- **State checks**: Item selected; ticket open; unsent preferred
- **Manager override**: May be required for sent items

## Step-by-step behavior (forensic)
1. User selects item in ticket
2. User clicks +/- or enters new quantity
3. Increase:
   - Quantity incremented
   - Totals recalculated
4. Decrease:
   - Quantity decremented
   - If quantity = 0: Item removed or prevented
   - Totals recalculated
5. Item display updated
6. Kitchen ticket may need reprint (if quantity increased)

## Edge cases & failure paths
- **Decrease to zero**: Prompts for deletion or prevents
- **Sent item quantity change**: May require void/reorder
- **Fractional quantities**: Some items may support (weight-based)

## Data / audit / financial impact
- **Writes/updates**: TicketItem.quantity; ticket totals
- **Audit events**: Quantity changes on sent items logged
- **Financial risk**: Undercharging; inventory discrepancy

## Code traceability (REQUIRED)
- **Primary UI class(es)**: Action only
- **Entry action(s)**: 
  - `IncreaseItemAmountAction` → `actions/IncreaseItemAmountAction.java`
  - `DecreaseItemAmountAction` → `actions/DecreaseItemAmountAction.java`
- **Workflow/service enforcement**: Ticket item update
- **Messages/labels**: Button labels

## MagiDesk parity notes
- **What exists today**: Quantity can be modified
- **What differs / missing**: +/- action buttons; sent item handling

## Porting strategy (PLAN ONLY)
- **Backend requirements**: UpdateOrderLineQuantityCommand
- **API/DTO requirements**: PATCH /tickets/{id}/items/{itemId}/quantity
- **UI requirements**: +/- buttons or quantity entry
- **Constraints for implementers**: Sent item changes need audit

# Feature: Size Selection Dialog (F-0032)

## Classification
- **Parity classification**: PARITY REQUIRED
- **MagiDesk status**: PARTIAL (may use modifiers)

## Problem / Why this exists (grounded)
- **Operational need**: Items available in sizes (Small, Medium, Large). Size affects price.
- **Evidence**: Size handling via modifiers or item variants - size selection during order.

## User-facing surfaces
- **Surface type**: Modal dialog or inline selection
- **UI entry points**: Add item with size options → auto-open
- **Exit paths**: Size selected / Cancel

## Preconditions & protections
- **User/role/permission checks**: Order entry permission
- **State checks**: Item has size options
- **Manager override**: Not required

## Step-by-step behavior (forensic)
1. User adds item with sizes
2. SizeSelectionDialog opens:
   - Size options (S, M, L, XL)
   - Price for each size
   - Visual selection buttons
3. User selects size
4. Item added with:
   - Selected size
   - Size-based price
5. Size shown on ticket

## Edge cases & failure paths
- **Required selection**: Must choose before add
- **Default size**: May pre-select
- **Size unavailable**: Hidden or disabled

## Data / audit / financial impact
- **Writes/updates**: TicketItem with size modifier
- **Audit events**: Part of order
- **Financial risk**: Correct pricing per size

## Code traceability (REQUIRED)
- **Primary UI class(es)**: SizeSelection or modifier dialog
- **Entry action(s)**: Item add trigger
- **Workflow/service enforcement**: Size-price mapping
- **Messages/labels**: Size names

## MagiDesk parity notes
- **What exists today**: May use modifier groups
- **What differs / missing**: Dedicated size selection UI

## Porting strategy (PLAN ONLY)
- **Backend requirements**: Size as modifier group or item variant
- **API/DTO requirements**: Size in order item
- **UI requirements**: SizeSelectionDialog
- **Constraints for implementers**: Price by size

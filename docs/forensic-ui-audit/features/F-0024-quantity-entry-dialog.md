# Feature: Quantity Entry Dialog (F-0024)

## Classification
- **Parity classification**: PARITY REQUIRED
- **MagiDesk status**: PARTIAL (quantity modification exists)

## Problem / Why this exists (grounded)
- **Operational need**: Enter specific quantity for items (e.g., order 5 of an item at once instead of clicking 5 times).
- **Evidence**: `NumberSelectionDialog2.java` - numeric entry for quantity; multiplied add.

## User-facing surfaces
- **Surface type**: Modal dialog or inline
- **UI entry points**: Before item add; quantity modification
- **Exit paths**: Quantity entered → items added

## Preconditions & protections
- **User/role/permission checks**: Order entry permission
- **State checks**: Ticket open
- **Manager override**: Not required

## Step-by-step behavior (forensic)
1. User pre-selects quantity (Qty button)
2. NumberDialog opens:
   - Numeric keypad
   - Current quantity displayed
3. User enters quantity (e.g., 5)
4. User selects menu item
5. Item added with entered quantity
6. OR: User selects item first, then modifies quantity

## Edge cases & failure paths
- **Zero quantity**: May delete or prevent
- **Very large quantity**: May warn
- **Fractional**: For weight-based items

## Data / audit / financial impact
- **Writes/updates**: TicketItem.quantity
- **Audit events**: Part of order
- **Financial risk**: Incorrect quantity

## Code traceability (REQUIRED)
- **Primary UI class(es)**: `NumberSelectionDialog2` → `ui/dialog/NumberSelectionDialog2.java`
- **Entry action(s)**: Qty button in OrderView
- **Workflow/service enforcement**: Item quantity handling
- **Messages/labels**: Quantity prompt

## MagiDesk parity notes
- **What exists today**: Quantity on order line
- **What differs / missing**: Pre-entry quantity dialog

## Porting strategy (PLAN ONLY)
- **Backend requirements**: Quantity field on OrderLine
- **API/DTO requirements**: Quantity in order item
- **UI requirements**: NumberEntryDialog; Qty button
- **Constraints for implementers**: Pre-entry vs post-entry mode

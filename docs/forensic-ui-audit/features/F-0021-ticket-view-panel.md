# Feature: Ticket View Panel (F-0021)

## Classification
- **Parity classification**: PARITY REQUIRED
- **MagiDesk status**: EXISTS (TicketItemsPanel/OrderItemsList)

## Problem / Why this exists (grounded)
- **Operational need**: Display current ticket items with quantities, modifiers, prices. Allow item selection for editing.
- **Evidence**: `TicketView.java` + `TicketViewerTable.java` - lists ticket items; selection; totals display.

## User-facing surfaces
- **Surface type**: Panel
- **UI entry points**: OrderView → left side panel (ticket items)
- **Exit paths**: Item selected for action

## Preconditions & protections
- **User/role/permission checks**: Order entry permission
- **State checks**: Ticket exists
- **Manager override**: Not required

## Step-by-step behavior (forensic)
1. Ticket created or opened
2. TicketView displays:
   - List of ticket items
   - Item name
   - Quantity
   - Modifiers
   - Price
   - Subtotal
3. Totals section shows:
   - Subtotal
   - Tax
   - Discount (if any)
   - Total
4. User can select item for:
   - Modify quantity (+/-)
   - Delete
   - Add modifiers
   - Add notes
5. Visual indicators for sent items

## Edge cases & failure paths
- **Empty ticket**: Shows empty state
- **Long item names**: Truncated with ellipsis
- **Many items**: Scrollable list

## Data / audit / financial impact
- **Writes/updates**: Item selection only (display)
- **Audit events**: None (view only)
- **Financial risk**: Display accuracy critical

## Code traceability (REQUIRED)
- **Primary UI class(es)**: 
  - `TicketView` → `ui/views/order/TicketView.java`
  - `TicketViewerTable` → `ui/views/order/TicketViewerTable.java`
- **Entry action(s)**: Part of OrderView
- **Workflow/service enforcement**: Ticket data binding
- **Messages/labels**: Item names, totals labels

## MagiDesk parity notes
- **What exists today**: TicketItemsPanel or OrderItemsList
- **What differs / missing**: May need UI refinement; sent item indicators

## Porting strategy (PLAN ONLY)
- **Backend requirements**: Ticket with items exposed
- **API/DTO requirements**: Ticket DTO includes items
- **UI requirements**: TicketView with item list, totals
- **Constraints for implementers**: Real-time update; touch selection

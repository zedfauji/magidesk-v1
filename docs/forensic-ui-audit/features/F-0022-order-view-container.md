# Feature: Order View Container (F-0022)

## Classification
- **Parity classification**: PARITY REQUIRED
- **MagiDesk status**: EXISTS (TableWorkspacePage / OrderPage)

## Problem / Why this exists (grounded)
- **Operational need**: Main order entry screen combining ticket view, menu selection, and action buttons.
- **Evidence**: `OrderView.java` - container layout; integrates TicketView, MenuItemView, action bar.

## User-facing surfaces
- **Surface type**: Full screen view
- **UI entry points**: New ticket; resume ticket; table tap
- **Exit paths**: Hold; Pay; Settle

## Preconditions & protections
- **User/role/permission checks**: Order entry permission; ticket ownership or override
- **State checks**: Ticket must be open
- **Manager override**: Required to edit others' tickets

## Step-by-step behavior (forensic)
1. Ticket opened or created
2. OrderView layout displays:
   - Left: TicketView (current items)
   - Right: Menu selection (categories/items)
   - Top/Bottom: Action buttons
3. User adds items from menu
4. Items appear in TicketView
5. User can:
   - Modify items (+/-, delete, modifiers)
   - Apply discounts
   - Add customer
   - Add notes
6. Exit options:
   - Hold: Save and return to switchboard
   - Pay: Open payment view
   - Settle: Direct to settlement

## Edge cases & failure paths
- **Stale ticket data**: Refresh on open
- **Ticket locked by other user**: Show warning
- **Ticket paid while editing**: Refresh and show message

## Data / audit / financial impact
- **Writes/updates**: Ticket and items as modified
- **Audit events**: Item adds/changes logged
- **Financial risk**: Core sales entry point

## Code traceability (REQUIRED)
- **Primary UI class(es)**: 
  - `OrderView` → `ui/views/order/OrderView.java`
  - `RootView` → `ui/views/RootView.java`
- **Entry action(s)**: New ticket, open ticket actions
- **Workflow/service enforcement**: Ticket management
- **Messages/labels**: Action labels

## MagiDesk parity notes
- **What exists today**: TableWorkspacePage / similar order view
- **What differs / missing**: Layout may differ; action button arrangement

## Porting strategy (PLAN ONLY)
- **Backend requirements**: Ticket CRUD operations
- **API/DTO requirements**: Full ticket management endpoints
- **UI requirements**: OrderView with split layout
- **Constraints for implementers**: Touch-optimized; responsive layout

# Feature: Order Type Selection Dialog (F-0020)

## Classification
- **Parity classification**: PARITY REQUIRED
- **MagiDesk status**: PARTIAL (order types exist but selection flow may differ)

## Problem / Why this exists (grounded)
- **Operational need**: Different order types (dine-in, takeout, delivery, bar tab) have different workflows, pricing, tax rules, and requirements. Staff need to select the appropriate type when starting an order.
- **Evidence**: `OrderTypeSelectionDialog.java` - shows available order types as buttons; some types require table selection, customer info, or other inputs.

## User-facing surfaces
- **Surface type**: Modal dialog
- **UI entry points**: Login screen order type buttons; OrderView when changing type; NewBarTabAction
- **Exit paths**: Select type (proceeds to order) / Cancel

## Preconditions & protections
- **User/role/permission checks**: Permission per order type (e.g., can_create_bar_tab)
- **State checks**: Some types require table availability, customer info
- **Manager override**: Some types may require manager approval

## Step-by-step behavior (forensic)
1. User initiates new order (from login or switchboard)
2. OrderTypeSelectionDialog shows configured order types:
   - DINE IN → opens table selection
   - TAKE OUT → direct to order entry
   - PICKUP → customer required
   - HOME DELIVERY → customer + address required
   - DRIVE THRU → direct to order entry
   - BAR TAB → opens bar tab selection
3. User clicks order type button
4. Type-specific flow:
   - Table selection for dine-in
   - Customer lookup for delivery
   - Bar tab authorization for bar tab
5. New ticket created with selected type
6. Order entry view opens

## Edge cases & failure paths
- **No permission for type**: Button hidden/disabled
- **No tables available**: Message shown
- **No customer for delivery**: Must enter customer first
- **Order type disabled**: Button not shown

## Data / audit / financial impact
- **Writes/updates**: Ticket.orderType; type-specific associations
- **Audit events**: Order creation logged
- **Financial risk**: Incorrect type affects pricing, taxes, workflow

## Code traceability (REQUIRED)
- **Primary UI class(es)**: 
  - `OrderTypeSelectionDialog` → `ui/views/order/OrderTypeSelectionDialog.java`
  - `OrderTypeSelectionDialog2` → `ui/views/order/OrderTypeSelectionDialog2.java`
- **Entry action(s)**: LoginView order type buttons; NewBarTabAction
- **Workflow/service enforcement**: OrderType entity; TerminalConfig for available types
- **Messages/labels**: Order type names, button labels

## Uncertainties (STOP; do not guess)
- Order type specific pricing lookup
- Order type properties (JSON-based configuration)

## MagiDesk parity notes
- **What exists today**: Order types via table context menu
- **What differs / missing**: Dedicated order type dialog; delivery/pickup flows; bar tab workflow

## Porting strategy (PLAN ONLY)
- **Backend requirements**: 
  - OrderType entity with properties
  - Type-specific validation (table required, customer required, etc.)
- **API/DTO requirements**: GET /order-types (available for terminal)
- **UI requirements**: OrderTypeSelectionDialog with type buttons; conditional flows
- **Constraints for implementers**: Type must be set before order entry; properties affect workflow

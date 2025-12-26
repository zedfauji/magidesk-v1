# Feature: Kitchen Display Window (F-0088)

## Classification
- **Parity classification**: DEFER (Phase 2+)
- **MagiDesk status**: MISSING

## Problem / Why this exists (grounded)
- **Operational need**: Kitchen staff need to see orders as they come in, track cooking status, and mark items as ready. Separate display from POS terminals.
- **Evidence**: `KitchenDisplayWindow.java` + `KitchenDisplayView.java` - standalone window for kitchen; shows ticket list with items; status tracking; filters by printer group.

## User-facing surfaces
- **Surface type**: Standalone window
- **UI entry points**: ShowKitchenDisplayAction from login/switchboard
- **Exit paths**: Close window

## Preconditions & protections
- **User/role/permission checks**: Kitchen display permission or none (dedicated terminal)
- **State checks**: Tickets must have items marked for kitchen printing
- **Manager override**: Not applicable

## Step-by-step behavior (forensic)
1. Kitchen display window opened (can be on separate monitor)
2. KitchenDisplayView shows:
   - List of pending tickets (KitchenTicketListPanel)
   - Each ticket with items to prepare
   - Time since order placed
   - Status selector (KitchenTicketStatusSelector)
3. Kitchen staff can:
   - View order items
   - Mark items as in-progress, ready
   - Filter by status, printer group
4. Updates in real-time (polling or push)
5. Items marked ready notify servers

## Edge cases & failure paths
- **No pending tickets**: Empty display
- **Stale data**: Auto-refresh handles
- **Multiple kitchens**: Filter by printer group

## Data / audit / financial impact
- **Writes/updates**: Ticket item status (kitchen status)
- **Audit events**: Status changes logged
- **Financial risk**: None (operational efficiency)

## Code traceability (REQUIRED)
- **Primary UI class(es)**: 
  - `KitchenDisplayWindow` → `demo/KitchenDisplayWindow.java`
  - `KitchenDisplayView` → `demo/KitchenDisplayView.java`
  - `KitchenTicketView` → `demo/KitchenTicketView.java`
  - `KitchenTicketListPanel` → `demo/KitchenTicketListPanel.java`
- **Entry action(s)**: `ShowKitchenDisplayAction` → `actions/ShowKitchenDisplayAction.java`
- **Workflow/service enforcement**: TicketDAO for pending items query
- **Messages/labels**: Kitchen status labels

## Uncertainties (STOP; do not guess)
- Real-time update mechanism (polling interval)
- Bump bar / touch screen integration
- Audio alerts for new orders

## MagiDesk parity notes
- **What exists today**: No kitchen display
- **What differs / missing**: Entire kitchen display system

## Porting strategy (PLAN ONLY)
- **Backend requirements**: 
  - Kitchen order queue query
  - Status update endpoints
  - Real-time notifications (SignalR or polling)
- **API/DTO requirements**: GET /kitchen/tickets; PUT /tickets/{id}/kitchen-status
- **UI requirements**: Separate KitchenDisplayWindow; ticket list; status buttons
- **Constraints for implementers**: Must work on dedicated terminal; touch-friendly UI

# Feature: Switchboard Panel (F-0010)

## Classification
- **Parity classification**: PARITY REQUIRED
- **MagiDesk status**: PARTIAL (switchboard concept exists but may differ)

## Problem / Why this exists (grounded)
- **Operational need**: Central navigation hub for POS operations. Quick access to common actions, open tickets, tables.
- **Evidence**: `SwitchboardView.java` - main navigation panel; action buttons; open tickets list; quick stats.

## User-facing surfaces
- **Surface type**: Main screen/view
- **UI entry points**: After login; Hold action; Settle complete
- **Exit paths**: Action button pressed → navigate to function

## Preconditions & protections
- **User/role/permission checks**: User authenticated
- **State checks**: None
- **Manager override**: Not required

## Step-by-step behavior (forensic)
1. User logs in
2. SwitchboardView displayed as home screen
3. Shows:
   - Action buttons (new order, tables, open tickets)
   - Open tickets list (user's tickets)
   - Statistics (today's sales, open count)
   - Manager functions (if permitted)
4. User clicks action:
   - New Order → Order type selection
   - Tables → TableMapView
   - Open Tickets → resume order
   - Manager → ManagerDialog
5. Navigation to selected function

## Edge cases & failure paths
- **No open tickets**: List empty
- **No permissions for action**: Button hidden/disabled

## Data / audit / financial impact
- **Writes/updates**: None (navigation only)
- **Audit events**: None
- **Financial risk**: None

## Code traceability (REQUIRED)
- **Primary UI class(es)**: 
  - `SwitchboardView` → `ui/views/SwitchboardView.java`
  - `SwitchboardOtherFunctionsView` → `ui/views/SwitchboardOtherFunctionsView.java`
- **Entry action(s)**: Base POS navigation
- **Workflow/service enforcement**: Permission-based button visibility
- **Messages/labels**: Button labels

## MagiDesk parity notes
- **What exists today**: Main navigation page
- **What differs / missing**: Quick stats; open tickets list in switchboard

## Porting strategy (PLAN ONLY)
- **Backend requirements**: Open tickets query; stats query
- **API/DTO requirements**: GET /dashboard or GET /open-tickets
- **UI requirements**: SwitchboardView with navigation grid
- **Constraints for implementers**: Permission-based button visibility

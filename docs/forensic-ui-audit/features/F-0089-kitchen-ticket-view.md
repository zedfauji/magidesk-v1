# Feature: Kitchen Ticket View (F-0089)

## Classification
- **Parity classification**: DEFER (Phase 2+)
- **MagiDesk status**: MISSING

## Problem / Why this exists (grounded)
- **Operational need**: Individual ticket display in kitchen showing items to prepare, modifiers, notes, time.
- **Evidence**: `KitchenTicketView.java` - single ticket card in KDS; item list; status buttons.

## User-facing surfaces
- **Surface type**: Card/Panel component
- **UI entry points**: Part of KitchenDisplayWindow
- **Exit paths**: Status action (bump, ready)

## Preconditions & protections
- **User/role/permission checks**: Kitchen display access
- **State checks**: Ticket has kitchen items
- **Manager override**: Not required

## Step-by-step behavior (forensic)
1. Ticket sent to kitchen
2. KitchenTicketView created:
   - Order number / table
   - Server name
   - Time elapsed
   - Items with modifiers
   - Special notes
   - Status indicator
3. Kitchen staff views items
4. Can mark statuses:
   - Started / In Progress
   - Ready
   - Bumped (done)
5. Visual feedback (color coding, timer alerts)

## Edge cases & failure paths
- **Rush order**: Highlighted
- **VIP ticket**: Special indicator
- **Void while cooking**: Update shown

## Data / audit / financial impact
- **Writes/updates**: Ticket kitchen status
- **Audit events**: Status changes logged
- **Financial risk**: None (operational)

## Code traceability (REQUIRED)
- **Primary UI class(es)**: `KitchenTicketView` → `demo/KitchenTicketView.java`
- **Entry action(s)**: Part of KitchenDisplayView
- **Workflow/service enforcement**: Kitchen status updates
- **Messages/labels**: Status labels

## MagiDesk parity notes
- **What exists today**: No kitchen display
- **What differs / missing**: Entire KDS component

## Porting strategy (PLAN ONLY)
- **Backend requirements**: Kitchen status on ticket items
- **API/DTO requirements**: Kitchen queue subscription
- **UI requirements**: KitchenTicketView card
- **Constraints for implementers**: Touch-friendly; color-coded; timer display

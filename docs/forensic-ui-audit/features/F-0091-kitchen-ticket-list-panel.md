# Feature: Kitchen Ticket List Panel (F-0091)

## Classification
- **Parity classification**: DEFER (Phase 2+)
- **MagiDesk status**: MISSING

## Problem / Why this exists (grounded)
- **Operational need**: Grid/list of all pending kitchen tickets for cook line overview.
- **Evidence**: `KitchenTicketListPanel.java` - scrollable panel of KitchenTicketViews; layout management.

## User-facing surfaces
- **Surface type**: Panel with grid layout
- **UI entry points**: Main content area of KitchenDisplayWindow
- **Exit paths**: Individual ticket selection

## Preconditions & protections
- **User/role/permission checks**: Kitchen display access
- **State checks**: Tickets pending in kitchen
- **Manager override**: Not required

## Step-by-step behavior (forensic)
1. Kitchen display open
2. KitchenTicketListPanel displays:
   - Grid of ticket cards
   - Sorted by time (oldest first)
   - Color-coded by wait time
   - Scrollable if many tickets
3. Auto-refresh as tickets:
   - Enter (new orders)
   - Update (status changes)
   - Exit (bumped/completed)

## Edge cases & failure paths
- **No tickets**: Empty state message
- **Many tickets**: Grid scrolls; oldest highlighted
- **Network issue**: Show stale data indicator

## Data / audit / financial impact
- **Writes/updates**: None (display only)
- **Audit events**: None
- **Financial risk**: None

## Code traceability (REQUIRED)
- **Primary UI class(es)**: `KitchenTicketListPanel` → `demo/KitchenTicketListPanel.java`
- **Entry action(s)**: Part of KitchenDisplayView
- **Workflow/service enforcement**: Kitchen ticket query
- **Messages/labels**: Empty state message

## MagiDesk parity notes
- **What exists today**: No KDS
- **What differs / missing**: Entire list panel

## Porting strategy (PLAN ONLY)
- **Backend requirements**: Kitchen queue query
- **API/DTO requirements**: GET /kitchen/tickets
- **UI requirements**: Grid layout with cards
- **Constraints for implementers**: Real-time updates; time-based coloring

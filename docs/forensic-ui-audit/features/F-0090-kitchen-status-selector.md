# Feature: Kitchen Status Selector (F-0090)

## Classification
- **Parity classification**: DEFER (Phase 2+)
- **MagiDesk status**: MISSING

## Problem / Why this exists (grounded)
- **Operational need**: Filter kitchen display by status (pending, in progress, ready). Focus on what needs attention.
- **Evidence**: `KitchenTicketStatusSelector.java` - status filter buttons; tab-like selection.

## User-facing surfaces
- **Surface type**: Filter bar
- **UI entry points**: KitchenDisplayWindow header
- **Exit paths**: Filter selected → view updates

## Preconditions & protections
- **User/role/permission checks**: Kitchen display access
- **State checks**: None
- **Manager override**: Not required

## Step-by-step behavior (forensic)
1. Kitchen display open
2. StatusSelector shows:
   - All (default)
   - New/Pending
   - In Progress
   - Ready
3. User taps status button
4. KitchenTicketListPanel filters to show only selected status
5. Counts shown per status

## Edge cases & failure paths
- **No tickets in status**: Empty message
- **Status changes while viewing**: Real-time update

## Data / audit / financial impact
- **Writes/updates**: None (filter only)
- **Audit events**: None
- **Financial risk**: None

## Code traceability (REQUIRED)
- **Primary UI class(es)**: `KitchenTicketStatusSelector` → `demo/KitchenTicketStatusSelector.java`
- **Entry action(s)**: Part of KitchenDisplayView
- **Workflow/service enforcement**: Filter logic
- **Messages/labels**: Status labels

## MagiDesk parity notes
- **What exists today**: No KDS
- **What differs / missing**: Entire filter component

## Porting strategy (PLAN ONLY)
- **Backend requirements**: Status enum
- **API/DTO requirements**: Filter parameter in kitchen query
- **UI requirements**: StatusSelector buttons
- **Constraints for implementers**: Real-time counts; touch-friendly

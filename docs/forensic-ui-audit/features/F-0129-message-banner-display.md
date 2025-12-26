# Feature: Message Banner Display (F-0129)

## Classification
- **Parity classification**: PARITY WITH MODERNIZATION
- **MagiDesk status**: MISSING

## Problem / Why this exists (grounded)
- **Operational need**: Display system-wide messages (86'd items, specials, announcements) to all terminals.
- **Evidence**: `MessageDialog.java` + `POSMessageDialog.java` - system message display.

## User-facing surfaces
- **Surface type**: Banner/Toast notification
- **UI entry points**: Auto-display on login; manager broadcast
- **Exit paths**: Dismiss / Auto-hide

## Preconditions & protections
- **User/role/permission checks**: View all; create requires manager
- **State checks**: Active messages exist
- **Manager override**: Not required to view

## Step-by-step behavior (forensic)
1. Manager creates message:
   - Text content
   - Priority level
   - Expiry time
   - Target terminals (all or specific)
2. Message pushed to terminals
3. Banner displays on POS screens
4. Staff acknowledges/dismisses
5. Message logged/archived

## Edge cases & failure paths
- **Terminal offline**: Show on reconnect
- **Expired message**: Auto-hide
- **Too many messages**: Queue or prioritize

## Data / audit / financial impact
- **Writes/updates**: Message entity
- **Audit events**: Message broadcast logged
- **Financial risk**: None (informational)

## Code traceability (REQUIRED)
- **Primary UI class(es)**: 
  - `MessageDialog` → `ui/dialog/MessageDialog.java`
  - `POSMessageDialog` → `ui/dialog/POSMessageDialog.java`
- **Entry action(s)**: System broadcast
- **Workflow/service enforcement**: Message service
- **Messages/labels**: Message content

## MagiDesk parity notes
- **What exists today**: No message banner system
- **What differs / missing**: Entire broadcast message system

## Porting strategy (PLAN ONLY)
- **Backend requirements**: Message entity with broadcast capability
- **API/DTO requirements**: POST /messages; WebSocket/SignalR push
- **UI requirements**: Banner display component
- **Constraints for implementers**: Real-time push to terminals

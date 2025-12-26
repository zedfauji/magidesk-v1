# Feature: Ticket Explorer (F-0068)

## Classification
- **Parity classification**: PARITY REQUIRED
- **MagiDesk status**: PARTIAL (open tickets list exists but full explorer may differ)

## Problem / Why this exists (grounded)
- **Operational need**: View all tickets across system - open, closed, voided. Essential for managers to review operations, find specific orders, audit.
- **Evidence**: `TicketExplorer.java` - lists all tickets with filters; search; date range; status.

## User-facing surfaces
- **Surface type**: Explorer panel (in Back Office)
- **UI entry points**: BackOfficeWindow → Tickets; also OpenTicketsListDialog for POS
- **Exit paths**: Close tab

## Preconditions & protections
- **User/role/permission checks**: Ticket viewing permission (back office level)
- **State checks**: None
- **Manager override**: Not required

## Step-by-step behavior (forensic)
1. Open Ticket Explorer
2. View shows ticket table:
   - Ticket number
   - Date/time
   - Server
   - Table/Order type
   - Status (open, closed, voided)
   - Total
   - Paid amount
3. Filters:
   - Date range
   - Status
   - Server
   - Order type
4. Search by ticket number
5. Double-click to view details
6. Actions: Print, Edit (limited), Void (permission)

## Edge cases & failure paths
- **Large volume**: Pagination required
- **Old tickets**: May archive

## Data / audit / financial impact
- **Writes/updates**: Read-only (unless editing allowed)
- **Audit events**: Access may be logged
- **Financial risk**: Audit trail access

## Code traceability (REQUIRED)
- **Primary UI class(es)**: `TicketExplorer` → `bo/ui/explorer/TicketExplorer.java`
- **Entry action(s)**: `TicketExplorerAction` → `bo/actions/TicketExplorerAction.java`
- **Workflow/service enforcement**: TicketDAO queries
- **Messages/labels**: Column headers

## MagiDesk parity notes
- **What exists today**: Open tickets in UI
- **What differs / missing**: Full explorer with all tickets, filters

## Porting strategy (PLAN ONLY)
- **Backend requirements**: Ticket query with filters, pagination
- **API/DTO requirements**: GET /tickets?status=&from=&to=&page=
- **UI requirements**: TicketExplorer with filters, actions
- **Constraints for implementers**: Performance with large datasets

# Feature: Server Productivity Report (F-0099)

## Classification
- **Parity classification**: PARITY WITH MODERNIZATION
- **MagiDesk status**: MISSING

## Problem / Why this exists (grounded)
- **Operational need**: Managers evaluate server performance - sales totals, average ticket, tips collected. Informs scheduling and performance reviews.
- **Evidence**: `ServerProductivityReportView.java` - lists servers with sales metrics; date range filter; sortable columns.

## User-facing surfaces
- **Surface type**: Report view (in Back Office)
- **UI entry points**: BackOfficeWindow → Reports → Server Productivity
- **Exit paths**: Close tab / Print / Export

## Preconditions & protections
- **User/role/permission checks**: Report viewing permission; HR/management
- **State checks**: None
- **Manager override**: Not required

## Step-by-step behavior (forensic)
1. Open Server Productivity Report
2. Select date range (from/to)
3. Generate report shows for each server:
   - Number of tickets
   - Total sales
   - Net sales
   - Tips collected
   - Average ticket size
   - Average tip percentage
4. Sort by any column
5. Totals row at bottom
6. Print or export

## Edge cases & failure paths
- **Server with no tickets**: Zero row
- **Large date range**: Performance consideration

## Data / audit / financial impact
- **Writes/updates**: None (read-only)
- **Audit events**: Report access logged
- **Financial risk**: Low (informational)

## Code traceability (REQUIRED)
- **Primary UI class(es)**: `ServerProductivityReportView` → `report/ServerProductivityReportView.java`
- **Entry action(s)**: `ServerProductivityReportAction` → `bo/actions/ServerProductivityReportAction.java`
- **Workflow/service enforcement**: Ticket and gratuity aggregation queries
- **Messages/labels**: Column headers

## MagiDesk parity notes
- **What exists today**: No server productivity report
- **What differs / missing**: Entire report

## Porting strategy (PLAN ONLY)
- **Backend requirements**: Query tickets/tips grouped by user
- **API/DTO requirements**: GET /reports/server-productivity
- **UI requirements**: ServerProductivityReportView
- **Constraints for implementers**: Must include tip data

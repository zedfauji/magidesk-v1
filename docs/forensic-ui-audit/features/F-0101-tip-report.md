# Feature: Tip Report (F-0101)

## Classification
- **Parity classification**: PARITY REQUIRED
- **MagiDesk status**: MISSING

## Problem / Why this exists (grounded)
- **Operational need**: Track tips/gratuities collected by server. Essential for tip reporting, IRS compliance, tip pooling calculations.
- **Evidence**: `TipReportView.java` - tips by server; date range; declared vs auto-gratuity.

## User-facing surfaces
- **Surface type**: Report view (in Back Office)
- **UI entry points**: BackOfficeWindow → Reports → Tip Report
- **Exit paths**: Close tab / Print / Export

## Preconditions & protections
- **User/role/permission checks**: Tip report permission (HR/manager)
- **State checks**: None
- **Manager override**: Not required

## Step-by-step behavior (forensic)
1. Open Tip Report
2. Select date range
3. Generate shows by server:
   - Server name
   - Cash tips
   - Credit card tips
   - Auto-gratuity
   - Total tips
   - Tip percentage of sales
4. Totals row
5. Print or export

## Edge cases & failure paths
- **Server with no tips**: Zero row
- **Non-tipped roles**: Excluded

## Data / audit / financial impact
- **Writes/updates**: None (read-only)
- **Audit events**: Report access logged
- **Financial risk**: IRS reporting; tip pool calculations

## Code traceability (REQUIRED)
- **Primary UI class(es)**: `TipReportView` → `report/TipReportView.java`
- **Entry action(s)**: `TipReportAction` → (path)
- **Workflow/service enforcement**: Gratuity aggregation
- **Messages/labels**: Column headers

## MagiDesk parity notes
- **What exists today**: Gratuity entity exists
- **What differs / missing**: Tip report view

## Porting strategy (PLAN ONLY)
- **Backend requirements**: Aggregate gratuities by user
- **API/DTO requirements**: GET /reports/tips
- **UI requirements**: TipReportView
- **Constraints for implementers**: Separate cash vs card tips

# Feature: Cash Out Report (F-0104)

## Classification
- **Parity classification**: PARITY REQUIRED
- **MagiDesk status**: MISSING

## Problem / Why this exists (grounded)
- **Operational need**: End of day cash reconciliation report. Expected vs actual with variance.
- **Evidence**: Drawer/shift reports + cash tracking - cash out reconciliation.

## User-facing surfaces
- **Surface type**: Report view
- **UI entry points**: BackOfficeWindow → Reports → Cash Out; Shift End
- **Exit paths**: Close / Print

## Preconditions & protections
- **User/role/permission checks**: Report permission
- **State checks**: None (can run anytime)
- **Manager override**: Not required

## Step-by-step behavior (forensic)
1. Open Cash Out Report
2. Select parameters:
   - Date/shift
   - Drawer/terminal
3. Generate shows:
   - Opening balance
   - Cash sales received
   - Cash drops made
   - Expected cash
   - Actual count (if entered)
   - Over/short variance
4. Print or export

## Edge cases & failure paths
- **No transactions**: Empty
- **Missing count**: Can't calculate variance

## Data / audit / financial impact
- **Writes/updates**: None (read-only)
- **Audit events**: Report access
- **Financial risk**: Primary cash accountability

## Code traceability (REQUIRED)
- **Primary UI class(es)**: Cash out report view
- **Entry action(s)**: Report menu
- **Workflow/service enforcement**: Cash calculation
- **Messages/labels**: Cash labels

## MagiDesk parity notes
- **What exists today**: No cash out report
- **What differs / missing**: Entire cash reconciliation report

## Porting strategy (PLAN ONLY)
- **Backend requirements**: Cash flow aggregation
- **API/DTO requirements**: GET /reports/cash-out
- **UI requirements**: CashOutReportView
- **Constraints for implementers**: Track all cash movements

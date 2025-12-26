# Feature: Sales Balance Report (F-0094)

## Classification
- **Parity classification**: PARITY REQUIRED
- **MagiDesk status**: MISSING

## Problem / Why this exists (grounded)
- **Operational need**: End-of-day/shift financial reconciliation - total sales vs payments received vs cash in drawer. Essential for closing procedures.
- **Evidence**: `SalesBalanceReportView.java` + `SalesBalanceReport.java` - compares expected vs actual; shows variances.

## User-facing surfaces
- **Surface type**: Report view (in Back Office)
- **UI entry points**: BackOfficeWindow → Reports → Sales Balance
- **Exit paths**: Close tab / Print

## Preconditions & protections
- **User/role/permission checks**: Financial report permission
- **State checks**: None
- **Manager override**: Not required

## Step-by-step behavior (forensic)
1. Open Sales Balance Report
2. Select date/shift
3. Generate shows:
   - Gross Sales
   - Discounts given
   - Net Sales
   - Taxes collected
   - Tips/Gratuities
   - Cash payments received
   - Card payments received
   - Other payments
   - Voids/Refunds
   - Cash drops
   - Payouts
   - Expected cash balance
   - Actual count (if entered)
   - Over/Short variance
4. Print for records

## Edge cases & failure paths
- **Drawer not pulled**: Shows expected only
- **Multi-shift**: Combine or separate by shift

## Data / audit / financial impact
- **Writes/updates**: None (read-only)
- **Audit events**: Report generation logged
- **Financial risk**: Primary reconciliation document

## Code traceability (REQUIRED)
- **Primary UI class(es)**: 
  - `SalesBalanceReportView` → `report/SalesBalanceReportView.java`
  - `SalesBalanceReport` → `report/SalesBalanceReport.java`
- **Entry action(s)**: `SalesBalanceReportAction` → `bo/actions/SalesBalanceReportAction.java`
- **Workflow/service enforcement**: Transaction aggregation
- **Messages/labels**: Section headers

## MagiDesk parity notes
- **What exists today**: No sales balance report
- **What differs / missing**: Entire report

## Porting strategy (PLAN ONLY)
- **Backend requirements**: Aggregate all transaction types
- **API/DTO requirements**: GET /reports/sales-balance
- **UI requirements**: SalesBalanceReportView
- **Constraints for implementers**: Must reconcile exactly with transactions

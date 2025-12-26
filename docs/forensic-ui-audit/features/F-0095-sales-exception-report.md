# Feature: Sales Exception Report (F-0095)

## Classification
- **Parity classification**: PARITY REQUIRED
- **MagiDesk status**: MISSING

## Problem / Why this exists (grounded)
- **Operational need**: Track exceptions/anomalies - voids, refunds, discounts, no-sales. Essential for loss prevention and audit.
- **Evidence**: `SalesExceptionReportView.java` + `SalesExceptionReport.java` - lists all exception transactions with details and totals.

## User-facing surfaces
- **Surface type**: Report view (in Back Office)
- **UI entry points**: BackOfficeWindow → Reports → Sales Exception
- **Exit paths**: Close tab / Print

## Preconditions & protections
- **User/role/permission checks**: Exception report permission (sensitive)
- **State checks**: None
- **Manager override**: Not required

## Step-by-step behavior (forensic)
1. Open Sales Exception Report
2. Select date range
3. Generate shows sections:
   - **Voids**: Voided tickets with reason, amount, who voided
   - **Refunds**: Refund transactions with amount, reason
   - **Discounts**: Applied discounts with type, amount, who applied
   - **No Sales**: Cash drawer opens without sale
   - **Manager Overrides**: Actions requiring manager auth
4. Totals per section
5. User breakdown (who performed each)
6. Print for audit

## Edge cases & failure paths
- **No exceptions**: Clean report (good sign!)
- **High volume**: May indicate training or fraud issues

## Data / audit / financial impact
- **Writes/updates**: None (read-only)
- **Audit events**: Report generation logged
- **Financial risk**: Primary loss prevention tool

## Code traceability (REQUIRED)
- **Primary UI class(es)**: 
  - `SalesExceptionReportView` → `report/SalesExceptionReportView.java`
  - `SalesExceptionReport` → `report/SalesExceptionReport.java`
- **Entry action(s)**: `SalesExceptionReportAction` → `bo/actions/SalesExceptionReportAction.java`
- **Workflow/service enforcement**: Exception transaction queries
- **Messages/labels**: Exception type labels

## MagiDesk parity notes
- **What exists today**: No exception report
- **What differs / missing**: Entire report

## Porting strategy (PLAN ONLY)
- **Backend requirements**: Query transactions by type (void, refund, etc.)
- **API/DTO requirements**: GET /reports/exceptions
- **UI requirements**: SalesExceptionReportView
- **Constraints for implementers**: Must include all exception types; user attribution

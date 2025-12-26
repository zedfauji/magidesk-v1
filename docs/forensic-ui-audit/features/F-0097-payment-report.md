# Feature: Payment Report (F-0097)

## Classification
- **Parity classification**: PARITY REQUIRED
- **MagiDesk status**: MISSING

## Problem / Why this exists (grounded)
- **Operational need**: Breakdown of payments by type (cash, card, gift cert, house account). Essential for reconciliation.
- **Evidence**: `PaymentReportView.java` - payment totals by type; date range; terminal.

## User-facing surfaces
- **Surface type**: Report view (in Back Office)
- **UI entry points**: BackOfficeWindow → Reports → Payment Report
- **Exit paths**: Close tab / Print / Export

## Preconditions & protections
- **User/role/permission checks**: Report viewing permission
- **State checks**: None
- **Manager override**: Not required

## Step-by-step behavior (forensic)
1. Open Payment Report
2. Select date range
3. Optional terminal filter
4. Generate shows by payment type:
   - Cash payments
   - Credit card payments (by type)
   - Debit card payments
   - Gift certificates
   - House accounts
   - Checks
   - Other
5. Totals for each type
6. Grand total
7. Print or export

## Edge cases & failure paths
- **No payments in range**: Empty report
- **Payment types not used**: Not shown or zero

## Data / audit / financial impact
- **Writes/updates**: None (read-only)
- **Audit events**: Report access logged
- **Financial risk**: Reconciliation document

## Code traceability (REQUIRED)
- **Primary UI class(es)**: `PaymentReportView` → `report/PaymentReportView.java`
- **Entry action(s)**: `PaymentReportAction` → (path)
- **Workflow/service enforcement**: Payment aggregation queries
- **Messages/labels**: Payment type labels

## MagiDesk parity notes
- **What exists today**: No payment report
- **What differs / missing**: Entire payment report

## Porting strategy (PLAN ONLY)
- **Backend requirements**: Aggregate payments by type
- **API/DTO requirements**: GET /reports/payments
- **UI requirements**: PaymentReportView
- **Constraints for implementers**: Must match drawer reports

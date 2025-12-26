# Feature: Credit Card Report (F-0096)

## Classification
- **Parity classification**: PARITY REQUIRED
- **MagiDesk status**: MISSING

## Problem / Why this exists (grounded)
- **Operational need**: Managers need to reconcile credit card transactions with payment processor statements. Track CC authorizations, captures, voids, refunds.
- **Evidence**: `CreditCardReportView.java` + `CardReportModel.java` - lists all card transactions; by date range, card type, transaction type; totals for reconciliation.

## User-facing surfaces
- **Surface type**: Report view (in Back Office)
- **UI entry points**: BackOfficeWindow → Reports → Credit Card Report
- **Exit paths**: Close tab / Print / Export

## Preconditions & protections
- **User/role/permission checks**: Report viewing permission
- **State checks**: None (historical query)
- **Manager override**: Not required

## Step-by-step behavior (forensic)
1. User opens Credit Card Report
2. View shows:
   - Date range selector
   - Card type filter (Visa, MC, Amex, etc.)
   - Transaction type filter (sale, void, refund)
   - Terminal filter
3. Generate report shows:
   - Transaction list with details
   - Card last 4 digits
   - Auth codes
   - Amounts (including tips)
   - Transaction status
   - Totals by card type
4. User can print/export report

## Edge cases & failure paths
- **No transactions in range**: Empty report
- **Pending authorizations**: Shown separately
- **Failed transactions**: Included for audit

## Data / audit / financial impact
- **Writes/updates**: None (read-only)
- **Audit events**: Report access may be logged
- **Financial risk**: Discrepancies with processor indicate problems

## Code traceability (REQUIRED)
- **Primary UI class(es)**: 
  - `CreditCardReportView` → `report/CreditCardReportView.java`
  - `CardReportModel` → `report/CardReportModel.java`
- **Entry action(s)**: `CreditCardReportAction` → `bo/actions/CreditCardReportAction.java`
- **Workflow/service enforcement**: PosTransaction queries filtered by payment type
- **Messages/labels**: Report headers, column labels

## Uncertainties (STOP; do not guess)
- Integration with payment processor batch reports
- Chargeback tracking

## MagiDesk parity notes
- **What exists today**: No card-specific reporting
- **What differs / missing**: Entire credit card report

## Porting strategy (PLAN ONLY)
- **Backend requirements**: 
  - Query payments by type=Card with date range
  - Aggregate by card type
- **API/DTO requirements**: GET /reports/credit-cards?from=&to=
- **UI requirements**: CreditCardReportView with filters, table, totals
- **Constraints for implementers**: Must match payment processor settlement

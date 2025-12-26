# Feature: Journal Report View (F-0103)

## Classification
- **Parity classification**: PARITY REQUIRED
- **MagiDesk status**: MISSING

## Problem / Why this exists (grounded)
- **Operational need**: Complete audit trail of all financial transactions - sales, voids, refunds, payouts, cash drops. Essential for reconciliation and auditing.
- **Evidence**: `JournalReportView.java` + `JournalReportModel.java` - chronological list of all transactions with details.

## User-facing surfaces
- **Surface type**: Report view (in Back Office)
- **UI entry points**: BackOfficeWindow → Reports → Journal
- **Exit paths**: Close tab / Print / Export

## Preconditions & protections
- **User/role/permission checks**: Financial report permission
- **State checks**: None
- **Manager override**: Not required

## Step-by-step behavior (forensic)
1. Open Journal Report
2. Select date range
3. Optional filters:
   - Transaction type
   - User
   - Terminal
4. Generate shows all transactions:
   - Timestamp
   - Transaction type
   - User
   - Amount
   - Reference (ticket #, etc.)
   - Description
5. Totals by type
6. Print or export

## Edge cases & failure paths
- **Voided transactions**: Shown with void flag
- **Large volume**: Pagination needed

## Data / audit / financial impact
- **Writes/updates**: None (read-only)
- **Audit events**: Report access logged
- **Financial risk**: Primary audit document

## Code traceability (REQUIRED)
- **Primary UI class(es)**: 
  - `JournalReportView` → `report/JournalReportView.java`
  - `JournalReportModel` → `report/JournalReportModel.java`
- **Entry action(s)**: `JournalReportAction` → `bo/actions/JournalReportAction.java`
- **Workflow/service enforcement**: PosTransaction queries
- **Messages/labels**: Column headers

## MagiDesk parity notes
- **What exists today**: Transaction logging exists
- **What differs / missing**: Journal report UI

## Porting strategy (PLAN ONLY)
- **Backend requirements**: All transactions queryable
- **API/DTO requirements**: GET /reports/journal
- **UI requirements**: JournalReportView with filters
- **Constraints for implementers**: Must include all transaction types; immutable records

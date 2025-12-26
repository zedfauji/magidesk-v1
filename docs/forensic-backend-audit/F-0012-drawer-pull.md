# Backend Forensic Audit: F-0012 Drawer Pull Report

## Feature Context
- **Feature**: Drawer Pull Report Dialog
- **Trace from**: `F-0012-drawer-pull-report-dialog.md`
- **Reference**: `DrawerPullReportDialog.java`, `DrawerPullReport.java`

## Backend Invariants
1.  **Calculation Integrity**: `ExpectedCash` MUST be calculated as `BeginningBalance + CashSales + CashDeposits - CashDrops - TipsPaidOut`.
2.  **Snapshot**: The report data must represent a point-in-time snapshot. Sales occurring *during* the generation must be either included or excluded consistently.
3.  **Scope**: The report MUST be scoped to the specific Terminal/Drawer ID.

## Forbidden States
-   **Negative Variance**: Not forbidden per se, but `Actual < Expected` is a theft indicator (Audit Warning).
-   **Cross-Contamination**: Including sales from Terminal B in Terminal A's drawer pull report.

## Audit Requirements
-   **Event**: `DRAWER_REPORT_GENERATED`
    -   Payload: TerminalId, OperatorId, ExpectedAmount.
    -   Severity: INFO.

## Concurrency Semantics
-   **Live Data**: If the store is active, the report is "approximate" until the specific `DrawerClose` event locks the drawer.

## MagiDesk Backend Parity
-   **Reporting**: ⚠️ Need specific `DrawerDomainService` to handle this calculation.

## Alignment Strategy
1.  **Implement** `GetDrawerStatusQuery` which aggregates all cash transactions for the current session.

# Backend Forensic Audit: F-0076 Drawer Balance Report

## Feature Context
- **Feature**: Drawer Balance Report
- **Trace from**: `F-0076-drawer-balance-report.md`
- **Reference**: `DrawerBalanceDialog.java`

## Backend Invariants
1.  **Calculation**: Start Balance + CASH_SALES + CASH_IN - CASH_OUT = Calculated Balance.
2.  **Scope**: Current Open Shift/Drawer only.

## Forbidden States
-   **None**.

## Audit Requirements
-   **Event**: `DRAWER_BALANCE_CHECKED`.

## Concurrency Semantics
-   **None**.

## MagiDesk Backend Parity
-   **Query**: ⚠️ `GetDrawerBalanceQuery` needed.

## Alignment Strategy
1.  **Implement** Query to aggregate terminal transactions.

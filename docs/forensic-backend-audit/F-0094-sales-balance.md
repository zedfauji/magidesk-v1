# Backend Forensic Audit: F-0094 Sales Balance Report

## Feature Context
- **Feature**: Sales Balance Report
- **Trace from**: `F-0094-sales-balance-report.md`
- **Reference**: `SalesBalanceReport.java`

## Backend Invariants
1.  **Reconciliation**: Compare "Expected Cash" (Sales) vs "Actual Cash" (Drawer Pull).
2.  **Variance**: Show Overage/Shortage.

## Forbidden States
-   **None**.

## Audit Requirements
-   **Event**: None.

## Concurrency Semantics
-   **None**.

## MagiDesk Backend Parity
-   **Logic**: ⚠️ `CashReconciliationService`.

## Alignment Strategy
1.  **Implement** Reconciliation Logic.

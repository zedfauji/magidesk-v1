# Backend Forensic Audit: F-0104 Cash Out Report

## Feature Context
- **Feature**: Cash Out Report
- **Trace from**: `F-0104-cash-out-report.md`
- **Reference**: `CashOutReport.java` (Server Checkout)

## Backend Invariants
1.  **Scope**: Per User/Shift.
2.  **Calculation**: Net Sales - Tips Paid + Tips Due = Cash Owed (or Owed to Server).

## Forbidden States
-   **None**.

## Audit Requirements
-   **Event**: None.

## Concurrency Semantics
-   **None**.

## MagiDesk Backend Parity
-   **Query**: ✅ `ServerCheckoutQuery`.

## Alignment Strategy
1.  **None**.

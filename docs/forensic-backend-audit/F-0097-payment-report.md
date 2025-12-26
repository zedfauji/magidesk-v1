# Backend Forensic Audit: F-0097 Payment Report

## Feature Context
- **Feature**: Payment Report
- **Trace from**: `F-0097-payment-report.md`
- **Reference**: `PaymentReport.java`

## Backend Invariants
1.  **Aggregation**: Group by Tender Type (Cash, Card, Check, Gift).
2.  **Total**: Must match Sales Summary "Paid" amount.

## Forbidden States
-   **None**.

## Audit Requirements
-   **Event**: None.

## Concurrency Semantics
-   **None**.

## MagiDesk Backend Parity
-   **Query**: ✅ Exists.

## Alignment Strategy
1.  **None**.

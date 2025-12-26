# Backend Forensic Audit: F-0095 Sales Exception Report

## Feature Context
- **Feature**: Sales Exception Report
- **Trace from**: `F-0095-sales-exception-report.md`
- **Reference**: `SalesExceptionReport.java`

## Backend Invariants
1.  **Triggers**: High Voids, Zero-Price Sales, High Discounts.
2.  **Rule**: Flag tickets > X% Discount.

## Forbidden States
-   **None**.

## Audit Requirements
-   **Event**: None.

## Concurrency Semantics
-   **None**.

## MagiDesk Backend Parity
-   **Job**: ⚠️ `FraudDetectionJob`.

## Alignment Strategy
1.  **Implement** Anomaly Queries.

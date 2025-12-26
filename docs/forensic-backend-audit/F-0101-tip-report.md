# Backend Forensic Audit: F-0101 Tip Report

## Feature Context
- **Feature**: Tip Report
- **Trace from**: `F-0101-tip-report.md`
- **Reference**: `TipReport.java`

## Backend Invariants
1.  **Source**: Credit Card Tips + Declared Cash Tips.
2.  **Attribution**: Assigned to Server (or pooled if configured).

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

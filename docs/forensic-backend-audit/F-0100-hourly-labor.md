# Backend Forensic Audit: F-0100 Hourly Labor Report

## Feature Context
- **Feature**: Hourly Labor Report
- **Trace from**: `F-0100-hourly-labor-report.md`
- **Reference**: `HourlyLaborReport.java`

## Backend Invariants
1.  **Source**: Clock In/Out entries (F-0009).
2.  **Cost**: Hours * PayRate.
3.  **Analysis**: Compare Labor Cost % vs Sales (Labor/Sales Ratio).

## Forbidden States
-   **None**.

## Audit Requirements
-   **Event**: None.

## Concurrency Semantics
-   **None**.

## MagiDesk Backend Parity
-   **Query**: ⚠️ `LaborCostAnalysis`.

## Alignment Strategy
1.  **Implement** Query.

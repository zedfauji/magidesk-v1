# Backend Forensic Audit: F-0098 Menu Usage Report

## Feature Context
- **Feature**: Menu Usage Report
- **Trace from**: `F-0098-menu-usage-report.md`
- **Reference**: `MenuUsageReport.java` (Product Mix)

## Backend Invariants
1.  **Metric**: Quantity Sold (Velocity) and Revenue Contribution.
2.  **Grouping**: By Category / Group / Item.

## Forbidden States
-   **None**.

## Audit Requirements
-   **Event**: None.

## Concurrency Semantics
-   **None**.

## MagiDesk Backend Parity
-   **Query**: ✅ `GetProductMixQuery`.

## Alignment Strategy
1.  **None**.

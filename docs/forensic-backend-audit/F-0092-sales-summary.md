# Backend Forensic Audit: F-0092 Sales Summary Report

## Feature Context
- **Feature**: Sales Summary Report
- **Trace from**: `F-0092-sales-summary-report.md`
- **Reference**: `SalesSummaryReport.java`

## Backend Invariants
1.  **Formula**: Gross Sales - Discounts - Refunds = Net Sales.
2.  **Tax**: Sales Tax collected is LIABILTY, not Revenue.

## Forbidden States
-   **Discrepancy**: Sum of Ticket Totals != Report Total.

## Audit Requirements
-   **Event**: `REPORT_GENERATED`.

## Concurrency Semantics
-   **Snapshot**: Reporting on "Live" data vs "End of Day" snapshot.

## MagiDesk Backend Parity
-   **Query**: ⚠️ `GetSalesSummaryQuery` needed.

## Alignment Strategy
1.  **Implement** efficient aggregation query.

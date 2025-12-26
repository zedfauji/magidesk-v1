# Backend Forensic Audit: F-0093 Sales Detail Report

## Feature Context
- **Feature**: Sales Detail Report
- **Trace from**: `F-0093-sales-detail-report.md`
- **Reference**: `SalesDetailReport.java`

## Backend Invariants
1.  **Granularity**: Line-item level (Ticket Item).
2.  **Columns**: Time, TicketID, Item, Price, Tax, User.

## Forbidden States
-   **None**.

## Audit Requirements
-   **Event**: `REPORT_EXPORTED`.

## Concurrency Semantics
-   **Performance**: Querying 1M rows might timeout.

## MagiDesk Backend Parity
-   **Query**: ✅ Exists.

## Alignment Strategy
1.  **None**.

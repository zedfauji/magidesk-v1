# Backend Forensic Audit: F-0099 Server Productivity Report

## Feature Context
- **Feature**: Server Productivity Report
- **Trace from**: `F-0099-server-productivity-report.md`
- **Reference**: `ServerProductivityReport.java`

## Backend Invariants
1.  **Metrics**: Sales per Hour, Average Ticket Size, Tip %.
2.  **Attribution**: Links Tickets to "Owner" (User).

## Forbidden States
-   **None**.

## Audit Requirements
-   **Event**: None.

## Concurrency Semantics
-   **None**.

## MagiDesk Backend Parity
-   **Query**: ⚠️ `UserPerformanceStats`.

## Alignment Strategy
1.  **Implement** Query.

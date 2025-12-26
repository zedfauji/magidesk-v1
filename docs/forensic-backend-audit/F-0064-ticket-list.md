# Backend Forensic Audit: F-0064 Ticket List View

## Feature Context
- **Feature**: Ticket List View
- **Trace from**: `F-0064-ticket-list-view.md`
- **Reference**: `TicketListPanel.java`

## Backend Invariants
1.  **Scope**: Historical View (unlike "Open Tickets"). Show All, Open, Paid, Void.
2.  **Performance**: Must use Pagination or Date Range filter.

## Forbidden States
-   **Data Dump**: Loading 100k tickets into memory.

## Audit Requirements
-   **Event**: None.

## Concurrency Semantics
-   **None**.

## MagiDesk Backend Parity
-   **Query**: ✅ `GetTicketsQuery` with filters.

## Alignment Strategy
1.  **Ensure** index on `TicketDate`.

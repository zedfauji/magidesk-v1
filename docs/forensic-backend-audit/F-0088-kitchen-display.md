# Backend Forensic Audit: F-0088 Kitchen Display Window

## Feature Context
- **Feature**: Kitchen Display Window
- **Trace from**: `F-0088-kitchen-display-window.md`
- **Reference**: `KitchenDisplayWindow.java` (KDS)

## Backend Invariants
1.  **Filtering**: Only shows items where `TicketItem.ShouldPrintToKitchen` is true AND `TicketItem.Status != FINISHED`.
2.  **Ordering**: FIFO (First In First Out) by default.

## Forbidden States
-   **Zombie Orders**: Orders remaining on KDS 24 hours after completion.

## Audit Requirements
-   **Event**: None.

## Concurrency Semantics
-   **Polling**: Clients poll for new orders.

## MagiDesk Backend Parity
-   **Query**: ⚠️ `GetActiveKitchenOrdersQuery` needed.

## Alignment Strategy
1.  **Implement** KDS Query.

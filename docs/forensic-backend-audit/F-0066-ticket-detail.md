# Backend Forensic Audit: F-0066 Ticket Detail View

## Feature Context
- **Feature**: Ticket Detail View
- **Trace from**: `F-0066-ticket-detail-view.md`
- **Reference**: `TicketViewerTable.java`

## Backend Invariants
1.  **Immutability**: If viewing a Closed ticket, the view is Read-Only.
2.  **Completeness**: Must show Items, Modifiers, Tax, Tips, Payments, Transactions.

## Forbidden States
-   **None**.

## Audit Requirements
-   **Event**: `TICKET_VIEWED` (Optional, if strict privacy needed).

## Concurrency Semantics
-   **None**.

## MagiDesk Backend Parity
-   **Logic**: ✅ Exists.

## Alignment Strategy
1.  **None**.

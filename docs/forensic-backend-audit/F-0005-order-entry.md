# Backend Forensic Audit: F-0005 Order Entry View Container

## Feature Context
- **Feature**: Order Entry View Container
- **Trace from**: `F-0005-order-entry-view-container.md`
- **Reference**: `OrderView.java`, `Ticket.java`

## Backend Invariants
1.  **Ticket Integrity**: A Ticket MUST always belong to a valid `OrderType` and `Terminal`.
2.  **Transience vs Persistence**: A new ticket is transient until "Saved" or "Sent to Kitchen". However, created tickets should ideally have a persistent ID early to prevent data loss on crash.
3.  **Active Session**: Modifications to a Ticket (adding items) MUST be attributed to the current `UserSession`.
4.  **Zero-Price Handling**: Items with zero price (if allowed) must still be recorded as line items, not nulls.

## Forbidden States
-   **Orphaned Items**: `TicketItem` records existing without a parent `Ticket`.
-   **Negative Total**: A Ticket Total cannot be negative (unless it's a specific Refund type, but arguably a standard Order should be >= 0).
-   **Type Mutation**: Changing a Ticket from "Dine In" to "Delivery" must validate all required fields (e.g., Customer Address) before commit.

## Audit Requirements
-   **Event**: `TICKET_CREATED`
    -   Payload: TicketId, Type.
    -   Severity: INFO.
-   **Event**: `TICKET_UPDATED`
    -   Payload: TicketId, ItemCount, NewTotal.
    -   Severity: INFO.

## Concurrency Semantics
-   **Write Lock**: When a user is in `OrderView` for Ticket #123, writes to Ticket #123 from other terminals should be blocked or merged carefully.
-   **Item Availability**: Adding an item that goes out-of-stock during the session? (Inventory check usually happens on add).

## MagiDesk Backend Parity
-   **Entity**: ✅ `Ticket` entity is central.
-   **State Machine**: ⚠️ Explicit status transitions (Created -> Active -> Printed -> Paid) need strict enforcement.

## Alignment Strategy
1.  **Enforce** strict State Machine in `TicketDomainService`.
2.  **Ensure** auto-save mechanism or "Draft" status to prevent data loss.

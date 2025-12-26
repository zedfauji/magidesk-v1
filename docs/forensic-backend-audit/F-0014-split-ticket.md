# Backend Forensic Audit: F-0014 Split Ticket

## Feature Context
- **Feature**: Split Ticket Dialog
- **Trace from**: `F-0014-split-ticket-dialog.md`
- **Reference**: `SplitTicketDialog.java`, `TicketService`

## Backend Invariants
1.  **Conservation of Value**: `Sum(SplitTickets.Total) == OriginalTicket.Total`. No value can be created or destroyed during a split.
2.  **Item Atomicity**: A `TicketItem` must move entirely to the new ticket (unless functionality allows splitting qty 2 -> 1, 1).
3.  **Modifier Integrity**: Modifiers MUST stay attached to their parent Item during the move.

## Forbidden States
-   **Orphaned Payments**: If the original ticket had partial payments, they must be handled (either refunded or assigned to one of the splits). The system CANNOT split a ticket with confusing payment state.
-   **Empty Ticket**: A split operation shouldn't result in an empty ticket unless it is automatically deleted.

## Audit Requirements
-   **Event**: `TICKET_SPLIT`
    -   Payload: OriginalTicketId, NewTicketId, ItemsMovedCount.
    -   Severity: INFO.

## Concurrency Semantics
-   **Transactional**: The move operation (Remove from A, Add to B) MUST be ACID.

## MagiDesk Backend Parity
-   **Command**: ✅ `SplitTicketCommand` implemented.
-   **Tests**: ✅ Verified in Phase 3.

## Alignment Strategy
1.  **Maintain** current implementation.
2.  **Add** check for "Has Payments" to block splitting if payments exist (simplify first).

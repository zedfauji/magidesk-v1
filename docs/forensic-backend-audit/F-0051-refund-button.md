# Backend Forensic Audit: F-0051 Refund Button

## Feature Context
- **Feature**: Refund Button
- **Trace from**: `F-0051-refund-button.md`
- **Reference**: `RefundAction.java`

## Backend Invariants
1.  **Permission**: Requires `REFUND_TICKET` permission.
2.  **Context**: Operates on a specific Paid/Closed ticket.
3.  **Result**: Initiates a Refund Workflow (Negative Payment).

## Forbidden States
-   **Open Ticket**: Cannot refund a ticket that hasn't been paid yet (That's a Void).
-   **Excess Refund**: Refund amount > Original Payment (unless explicitly allowed as "Goodwill").

## Audit Requirements
-   **Event**: `REFUND_INITIATED`
    -   Payload: TicketId, OperatorId.
    -   Severity: WARN.

## Concurrency Semantics
-   **Locking**: Ticket should be locked against concurrent Settle/Void.

## MagiDesk Backend Parity
-   **Command**: ✅ `RefundCommand`.

## Alignment Strategy
1.  **Enforce** `Ticket.IsPaid` check.

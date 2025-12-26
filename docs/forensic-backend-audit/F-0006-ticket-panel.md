# Backend Forensic Audit: F-0006 Ticket Panel

## Feature Context
- **Feature**: Ticket Panel Search, Qty, and Pay Now
- **Trace from**: `F-0006-ticket-panel-search-qty-and-pay-now.md`
- **Reference**: `PayNowAction.java`, `TicketView.java`

## Backend Invariants
1.  **Search Scope**: Item search MUST respect current menu availability (e.g., hidden items, out of stock).
2.  **Atomic Quantity Update**: Changing quantity from 1 to 5 must atomically recalculate all dependent taxes and modifiers.
3.  **Pay Now Workflow**: "Pay Now" implies immediate status transition: `Created` -> `Settled` IF full amount is covered.
4.  **Implicit Ticket Creation**: If "Pay Now" is clicked on a transient (unsaved) ticket, the system MUST persist the ticket first before processing payment.

## Forbidden States
-   **Negative Quantity**: Unless explicitly a "Return" line item, quantity cannot be negative.
-   **Partial Payment w/o Ticket**: Processing a payment without a persisted Ticket ID is forbidden.

## Audit Requirements
-   **Event**: `ITEM_QTY_CHANGE`
    -   Payload: TicketId, ItemId, OldQty, NewQty.
    -   Severity: INFO.

## Concurrency Semantics
-   **Stock Check**: Increasing quantity should trigger an inventory check (if inventory enabled).

## MagiDesk Backend Parity
-   **Search**: ✅ `MenuService` supports filtering.
-   **Pay Now**: ⚠️ Need to ensure atomic Save+Pay workflow in a single transaction or robust two-phase commit.

## Alignment Strategy
1.  **Implement** `PayNowCommand` which orchestrates `CreateTicket` + `ProcessPayment` in one transaction.

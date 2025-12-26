# Backend Forensic Audit: F-0008 Settle Ticket Dialog

## Feature Context
- **Feature**: Settle Ticket Dialog
- **Trace from**: `F-0008-settle-ticket-dialog.md`
- **Reference**: `SettleTicketDialog.java`, `SettleTicketProcessor.java`

## Backend Invariants
1.  **Immutability**: Once settled, a Ticket acts as a financial record and its line items/prices MUST NOT be modified. (Void/Refund is a separate reverse transaction).
2.  **Financial Balance**: The sum of `Transactions` must equal `Ticket.TotalAmount` (plus tips/overpayment handling).
3.  **Drawer Association**: The settlement must be attributed to the specific Cash Drawer session of the logged-in user (or terminal).

## Forbidden States
-   **Unbalanced Settlement**: Closing a ticket when `PaidAmount < DueAmount` (except for specific Write-Off workflows).
-   **Double Closure**: Settling a ticket that is already `CLOSED` or `VOID`.

## Audit Requirements
-   **Event**: `TICKET_SETTLED`
    -   Payload: TicketId, TotalAmount, PaymentModes.
    -   Severity: INFO.

## Concurrency Semantics
-   **Idempotency**: Retrying a settlement request (due to network timeout) must not create duplicate transactions.

## MagiDesk Backend Parity
-   **Settlement**: ✅ `SettleTicketCommand` exists.
-   **Drawer**: ⚠️ Need verification of Drawer attribution logic in multi-drawer terminals.

## Alignment Strategy
1.  **Enforce** `IsPaid` status check in `SettleTicketHandler` to prevent double processing.
2.  **Implement** strict Drawer Session validation.

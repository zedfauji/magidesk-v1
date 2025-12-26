# Backend Forensic Audit: F-0027 Send to Kitchen Action

## Feature Context
- **Feature**: Send to Kitchen Action
- **Trace from**: `F-0027-send-to-kitchen-action.md`
- **Reference**: `OrderController.java`, `KitchenDisplayService`

## Backend Invariants
1.  **State Transition**: Sending items MUST transition their status from `CREATED` to `SENT/SUBMITTED` (or similar).
2.  **Filter**: Only "Unsent" items should be targeted. Sending already sent items (duplicates) is forbidden without explicit "Resend" intent.
3.  **Inventory**: Typically, inventory is decremented either at "Send" (Cook starts) or "Pay" (Settle). "Send" is safer for accurate stock.

## Forbidden States
-   **Partial Send**: Sending a parent item but failing to send its required modifiers to the kitchen printer.

## Audit Requirements
-   **Event**: `KITCHEN_ORDER_SUBMITTED`
    -   Payload: TicketId, ItemCount, DestinationPrinters.
    -   Severity: INFO.

## Concurrency Semantics
-   **Locking**: Ticket should be locked during the "Prepare print job" phase to prevent user adding items mid-stream that get missed.

## MagiDesk Backend Parity
-   **KDS**: ⚠️ `KitchenDisplayService` needs to be robust.

## Alignment Strategy
1.  **Implement** `SendTicketToKitchenCommand` that handles the status update + KDS/Print event generation in a transaction.

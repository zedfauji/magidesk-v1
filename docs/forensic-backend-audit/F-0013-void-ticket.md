# Backend Forensic Audit: F-0013 Void Ticket

## Feature Context
- **Feature**: Void Ticket Dialog
- **Trace from**: `F-0013-void-ticket-dialog.md`
- **Reference**: `VoidTicketDialog.java`, `TicketDomainService`

## Backend Invariants
1.  **Reversal Enforced**: Voiding a ticket MUST reverse all associated financial impacts (Tax, Sales, Gratuity).
2.  **Inventory Restoration**: Voided items MUST be returned to inventory (unless `Waste` flag is set).
3.  **Mandatory Reason**: A `VoidReason` MUST be attached to the transaction.
4.  **Authorization**: The user implementing the void MUST have `VOID_TICKET` permission.

## Forbidden States
-   **Partial Void**: A "Void Ticket" action applies to the WHOLE ticket. Partial removal is a different operation (Void Item / Refund).
-   **Settled Void**: If a ticket is already `PAID`, it cannot be `VOIDED` without first reversing the payment (Refund).

## Audit Requirements
-   **Event**: `TICKET_VOIDED`
    -   Payload: TicketId, AuthorizedBy, ReasonCode, Amount.
    -   Severity: CRITICAL.

## Concurrency Semantics
-   **Locking**: The ticket must be exclusive-locked during the void process to prevent simultaneous Payment or Modification.

## MagiDesk Backend Parity
-   **Logic**: ✅ `VoidTicketCommand` implemented.
-   **Inventory**: ⚠️ Inventory integration needs verification.

## Alignment Strategy
1.  **Enforce** `TicketStatus` check.
2.  **Ensure** `InventoryService` is notified of voided items.

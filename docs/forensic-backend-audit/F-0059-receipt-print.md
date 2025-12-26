# Backend Forensic Audit: F-0059 Receipt Print Action

## Feature Context
- **Feature**: Receipt Print Action
- **Trace from**: `F-0059-receipt-print-action.md`
- **Reference**: `PrintTicketAction.java` (Receipt Mode)

## Backend Invariants
1.  **Content**: Must print the *current* state of the Ticket (Saved + Unsaved items? Usually Saved only).
2.  **Marking**: Reprints should be marked "** DUPLICATE **" if already printed/paid.

## Forbidden States
-   **Silent Reprint**: Determining fraud (Server reprinting receipt to collect cash from another customer) requires "Reprint Count" tracking.

## Audit Requirements
-   **Event**: `RECEIPT_PRINTED`
    -   Payload: TicketId, IsReprint(true/false).
    -   Severity: INFO.

## Concurrency Semantics
-   **None**.

## MagiDesk Backend Parity
-   **Logic**: ✅ Printing exists, check "Duplicate" logic.

## Alignment Strategy
1.  **Add** `ReprintCount` to Ticket.

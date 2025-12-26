# Backend Forensic Audit: F-0025 Print Ticket Action

## Feature Context
- **Feature**: Print Ticket Action
- **Trace from**: `F-0025-print-ticket-action.md`
- **Reference**: `PrintTicketAction.java`, `TicketService`

## Backend Invariants
1.  **Status Flag**: Printing a guest check typically sets `Ticket.Printed = true`, locking certain concurrent edits or changing the UI representation (color).
2.  **Idempotency**: Reprinting an already printed ticket should be logged as "Reprint" (Fraud prevention).
3.  **Fiscal Data**: In fiscal jurisdictions, printing implies tax commitment.

## Forbidden States
-   **Silent Print**: Printing without updating the `Printed` status or LastPrinted time.

## Audit Requirements
-   **Event**: `TICKET_PRINTED`
    -   Payload: TicketId, PrinterName.
    -   Severity: INFO.

## Concurrency Semantics
-   **Async**: Printing is slow. The backend status update should happen *before* or *during* the spooling, not after, to prevent race conditions during the print delay.

## MagiDesk Backend Parity
-   **Service**: ⚠️ `PrintingService` implementation needed.

## Alignment Strategy
1.  **Implement** `MarkTicketAsPrintedCommand`.

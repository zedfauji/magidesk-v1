# Backend Forensic Audit: F-0015 Payment Process Wait Dialog

## Feature Context
- **Feature**: Payment Process Wait Dialog
- **Trace from**: `F-0015-payment-process-wait-dialog.md`
- **Reference**: `PaymentProcessWaitDialog.java`

## Backend Invariants
1.  **Ticket Lock**: The backend must reject any concurrently received modification requests for the Ticket while a payment is "In Process".
2.  **Idempotency Key**: The request triggering this wait (e.g., `ProcessCard`) must carry a unique RequestID to allow safe retries if the client disconnects.

## Forbidden States
-   **Orphaned Transaction**: If the client crashes during the "Wait", the backend must either complete the transaction or void it. It cannot leave the payment in `PENDING` indefinitely.

## Audit Requirements
-   **Event**: `PAYMENT_PROCESSING_START`
    -   Payload: RequestId, TicketId.
    -   Severity: TRACE.
-   **Event**: `PAYMENT_PROCESSING_END`
    -   Payload: RequestId, Success, DurationMs.
    -   Severity: TRACE.

## Concurrency Semantics
-   **Blocking**: The operation is a blocking synchronous call from the perspective of the Ticket state machine.

## MagiDesk Backend Parity
-   **Async**: ✅ CQRS handles async flows.
-   **Locking**: ⚠️ Need verification of optimistic concurrency failure modes if UI doesn't block effectively.

## Alignment Strategy
1.  **Use** `Idempotency-Key` header for financial APIs.

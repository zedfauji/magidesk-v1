# Backend Forensic Audit: F-0018 Authorization Capture Batch Dialog

## Feature Context
- **Feature**: Authorization Capture Batch Dialog
- **Trace from**: `F-0018-authorization-capture-batch-dialog.md`
- **Reference**: `AuthorizationCaptureBatchDialog.java`

## Backend Invariants
1.  **Validation**: Only transactions in `AUTHORIZED` (Pre-Auth) state should be captured. `SETTLED` or `VOID` transactions must be skipped.
2.  **Gateway Consistency**: The capture amount must match the Authorized amount (or be less, if tip adjust supported). It cannot exceed the auth without re-auth (usually).
3.  **Result Persistence**: Success/Failure from the gateway MUST be updated on the Transaction record immediately.

## Forbidden States
-   **Double Capture**: Submitting the same transaction to the gateway twice in the same batch.
-   **Stale Capture**: Attempting to capture an auth that has expired (gateway specific rule, backend should track auth timestamps).

## Audit Requirements
-   **Event**: `BATCH_CAPTURE_START`
    -   Payload: Count, TotalAmount.
    -   Severity: INFO.
-   **Event**: `BATCH_CAPTURE_RESULT`
    -   Payload: SuccessCount, FailureCount.
    -   Severity: INFO.

## Concurrency Semantics
-   **Batch Lock**: While the batch dialog is open/processing, no other terminal should act on these transactions.

## MagiDesk Backend Parity
-   **Batch**: ⚠️ Missing dedicated Batch Capture service/orchestrator.

## Alignment Strategy
1.  **Implement** `PaymentBatchDomainService` to manage the collection and submission of pending authorizations.

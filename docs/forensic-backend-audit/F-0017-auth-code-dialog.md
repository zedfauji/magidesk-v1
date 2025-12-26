# Backend Forensic Audit: F-0017 Authorization Code Dialog

## Feature Context
- **Feature**: Authorization Code Dialog
- **Trace from**: `F-0017-authorization-code-dialog.md`
- **Reference**: `AuthorizationCodeDialog.java`

## Backend Invariants
1.  **Code Integrity**: The manually entered code acts as the `GatewayReference` or `AuthCode` and MUST be preserved in the `Transaction` entity.
2.  **Settlement Flag**: Transactions with a manual auth code are typically "Offline" or "Voice Auth" and may require different handling during End-of-Day batch processing (e.g., "Force Capture").

## Forbidden States
-   **Empty Code**: Accepting a Credit Card payment without EITHER a Gateway Response OR a Manual Auth Code.
-   **Duplication**: Using the same Auth Code for multiple tickets (unless gateway allows split-auth, usually unique per transaction).

## Audit Requirements
-   **Event**: `MANUAL_AUTH_ENTRY`
    -   Payload: TicketId, AuthCode, OperatorId.
    -   Severity: WARN (Higher risk of fraud/error).

## Concurrency Semantics
-   **Atomicity**: Entering the code completes the payment transaction. This must follow standard Settle concurrency rules.

## MagiDesk Backend Parity
-   **Model**: ✅ `Transaction` entity has `AuthCode` field.
-   **Logic**: ⚠️ Need to ensure this flow bypasses the live gateway call but correctly sets the transaction state to `AUTHORIZED`.

## Alignment Strategy
1.  **Add** `PaymentSource.ManualVoiceAuth` enum member to distinguish these transactions from online ones.

# Backend Forensic Audit: F-0016 Swipe Card Dialog

## Feature Context
- **Feature**: Swipe Card Dialog
- **Trace from**: `F-0016-swipe-card-dialog.md`
- **Reference**: `SwipeCardDialog.java`

## Backend Invariants
1.  **Data Minimization**: The backend must NEVER persist full Track 1/Track 2 data. Only Last4, Bin, and Token/AuthCode.
2.  **P2PE**: Ideally, the swipe is encrypted by hardware. If software handles read, it must be transient memory only.
3.  **Timeout**: The "Waiting for Swipe" state must expire server-side transaction tokens if applicable.

## Forbidden States
-   **Cleartext Logging**: Writing the MAGSTRIPE data to logs is a PCI-DSS violation.
-   **Bypassed Auth**: Processing a card payment without a valid `AuthCode` from the gateway.

## Audit Requirements
-   **Event**: `CARD_SWIPE_RECEIVED`
    -   Payload: MaskedPan (************1234), CardType.
    -   Severity: INFO.

## Concurrency Semantics
-   **Single Consumer**: Only one `SwipeCardDialog` should be awaiting input on a terminal.

## MagiDesk Backend Parity
-   **Security**: ✅ `TokenizationService` planned.
-   **Hardware**: ⚠️ Need abstraction for hardware Events vs Keyboard Emulation.

## Alignment Strategy
1.  **Enforce** `[JsonIgnore]` on any DTO field carrying raw card data to prevent accidental serialization logging.

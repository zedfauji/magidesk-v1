# Backend Forensic Audit: F-0072 Cash Drop Dialog

## Feature Context
- **Feature**: Cash Drop Dialog
- **Trace from**: `F-0072-cash-drop-dialog.md`
- **Reference**: `CashDropDialog.java`

## Backend Invariants
1.  **Cash Movement**: Decreases Drawer Balance (Cash in Drawer).
2.  **Reason**: Must specify "Drop" (Safe Deposit).

## Forbidden States
-   **Negative Drop**: Cannot drop negative conversion.

## Audit Requirements
-   **Event**: `DRAWER_TRANSACTION` (Type=DROP).

## Concurrency Semantics
-   **Lock**: Drawer record lock.

## MagiDesk Backend Parity
-   **Model**: ✅ `TerminalTransaction`.

## Alignment Strategy
1.  **None**.

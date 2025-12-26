# Backend Forensic Audit: F-0073 Pay Out Dialog

## Feature Context
- **Feature**: Pay Out Dialog
- **Trace from**: `F-0073-pay-out-dialog.md`
- **Reference**: `PayoutDialog.java`

## Backend Invariants
1.  **Cash Movement**: Decreases Balance.
2.  **Reason**: "Vendor Payment" or "Expense".
3.  **Recursion**: Unlike Drop, Pay Out implies spending, not banking.

## Forbidden States
-   **Excess**: Pay Out > Current Balance (Drawer cannot go negative physically).

## Audit Requirements
-   **Event**: `DRAWER_TRANSACTION` (Type=PAYOUT).

## Concurrency Semantics
-   **None**.

## MagiDesk Backend Parity
-   **Model**: ✅ `TerminalTransaction`.

## Alignment Strategy
1.  **None**.

# Backend Forensic Audit: F-0044 Cash Payment Button

## Feature Context
- **Feature**: Cash Payment Button
- **Trace from**: `F-0044-cash-payment-button.md`
- **Reference**: `PaymentView.java`

## Backend Invariants
1.  **Processing**: Triggers `SettleTicketCommand` with `PaymentType=CASH`.

## Forbidden States
-   **None**.

## Audit Requirements
-   **Event**: None.

## Concurrency Semantics
-   **None**.

## MagiDesk Backend Parity
-   **Logic**: ✅ Exists.

## Alignment Strategy
1.  **None**.

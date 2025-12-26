# Backend Forensic Audit: F-0045 Credit Card Payment Button

## Feature Context
- **Feature**: Credit Card Payment Button
- **Trace from**: `F-0045-credit-card-payment-button.md`
- **Reference**: `PaymentView.java`

## Backend Invariants
1.  **Processing**: Triggers `ProcessPaymentCommand` which may invoke Gateway.
2.  **Wait State**: Puts ticket into `Processing` state.

## Forbidden States
-   **Concurrent Cash**: Cannot pay Cash while Card is Processing.

## Audit Requirements
-   **Event**: None.

## Concurrency Semantics
-   **Lock**: See F-0015.

## MagiDesk Backend Parity
-   **Logic**: ✅ Exists.

## Alignment Strategy
1.  **None**.

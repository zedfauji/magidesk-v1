# Backend Forensic Audit: F-0043 Quick Cash Buttons

## Feature Context
- **Feature**: Quick Cash Buttons ($10, $20, $50)
- **Trace from**: `F-0043-quick-cash-buttons.md`
- **Reference**: `PaymentView.java`

## Backend Invariants
1.  **Tender Type**: Always `CASH`.
2.  **Change Calculation**: Allowed. `TenderAmount > BalanceDue` -> Change.

## Forbidden States
-   **None**.

## Audit Requirements
-   **Event**: None.

## Concurrency Semantics
-   **None**.

## MagiDesk Backend Parity
-   **Config**: ✅ Denominations Configurable.

## Alignment Strategy
1.  **None**.

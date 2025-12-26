# Backend Forensic Audit: F-0024 Quantity Entry Dialog

## Feature Context
- **Feature**: Quantity Entry Dialog
- **Trace from**: `F-0024-quantity-entry-dialog.md`
- **Reference**: `QuantitySelectionDialog.java`

## Backend Invariants
1.  **Positive Integer**: Quantity MUST generally be > 0. (Returns/Refunds might use negative, but usually separate flag).
2.  **Fractional Units**: If the system supports weighted items (e.g., 1.5 kg), the backend MUST support `double/decimal` quantity.
3.  **Recalculation**: Setting quantity = 10 must immediately trigger `Total = Price * 10` and `Tax` update.

## Forbidden States
-   **Zero Quantity**: A line item with Qty=0 should be removed, not persisted.

## Audit Requirements
-   **Event**: `QUANTITY_SET`
    -   Payload: TicketId, ItemId, NewQty.
    -   Severity: TRACE.

## Concurrency Semantics
-   **Stock Check**: Setting a high quantity (e.g., 50) must validate against `AvailableStock`.

## MagiDesk Backend Parity
-   **Command**: ✅ `SetItemQuantityCommand` implemented.

## Alignment Strategy
1.  **Refine** validator to check `AllowFractional` flag on the Item definition.

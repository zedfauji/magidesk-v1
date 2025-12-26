# Backend Forensic Audit: F-0026 Increase/Decrease Quantity Action

## Feature Context
- **Feature**: Increase/Decrease Quantity Action
- **Trace from**: `F-0026-increase-decrease-quantity-action.md`
- **Reference**: `OrderController.java`

## Backend Invariants
1.  **Boundaries**: Decrementing from 1 -> 0 triggers `RemoveItem`.
2.  **Modifiers**: Incrementing an item with modifiers (e.g., "Burger + Extra Cheese") usually replicates the modifiers ? OR increments the scalar quantity? (Usually Scalar).
3.  **Sent/Cooked Prevention**: If an item is `SENT`, decrementing is forbidden (Action becomes "Void Item").

## Forbidden States
-   **Negative Result**: 1 - 2 = -1 (Forbidden unless Refund mode).

## Audit Requirements
-   **Event**: `QUANTITY_ADJUST`
    -   Payload: TicketId, ItemId, Delta (+1/-1).
    -   Severity: TRACE.

## Concurrency Semantics
-   **Optimistic**: +1 is safe. -1 requires checking strictly that current qty >= 1.

## MagiDesk Backend Parity
-   **Logic**: ✅ `AdjustItemQuantityCommand` logic exists.
-   **Sent Check**: ⚠️ Need to ensure we block decrement on Sent items.

## Alignment Strategy
1.  **Enforce** `ItemStatus.IsSent` check in the command handler.

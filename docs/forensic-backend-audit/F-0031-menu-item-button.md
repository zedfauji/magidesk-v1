# Backend Forensic Audit: F-0031 Menu Item Button View

## Feature Context
- **Feature**: Menu Item Button View
- **Trace from**: `F-0031-menu-item-button-view.md`
- **Reference**: `MenuItemButton.java`

## Backend Invariants
1.  **Data Source**: The button represents a `MenuItem` entity.
2.  **State Reflection**: If the backend knows the item is `OutOfStock`, the UI should receive this signal (disabled state or visual cue).

## Forbidden States
-   **Invalid Reference**: Button linked to a deleted MenuItem ID.

## Audit Requirements
-   **Event**: None (UI View).

## Concurrency Semantics
-   **Stock Updates**: Real-time stock updates should refresh the button state if possible.

## MagiDesk Backend Parity
-   **Query**: ✅ `GetMenuGroupsQuery` returns items.

## Alignment Strategy
1.  **Ensure** `MenuItemDTO` includes `StockStatus` and `ColorCode`.

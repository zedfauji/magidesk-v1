# Backend Forensic Audit: F-0032 Size Selection Dialog

## Feature Context
- **Feature**: Size Selection Dialog
- **Trace from**: `F-0032-size-selection-dialog.md`
- **Reference**: `ModifierGroupSelectionDialog.java` (often shared logic) or dedicated `SizeSelectionDialog.java`.

## Backend Invariants
1.  **Price Impact**: Size change (Small -> Large) triggers a base price update on the `TicketItem`.
2.  **Modifier Pricing**: Some modifiers might scale in price with size (e.g., Pizza Toppings). The backend must support "Size-Dependent Modifier Pricing".

## Forbidden States
-   **Ambiguous Price**: Selecting a size that has no defined price in the menu.

## Audit Requirements
-   **Event**: `ITEM_SIZE_SELECTED` (Implicit in Item Add/Update).

## Concurrency Semantics
-   **Inventory**: "Small" cups might be out of stock while "Large" are available.

## MagiDesk Backend Parity
-   **Model**: ⚠️ `Size` concept vs `Variant` concept needs alignment.

## Alignment Strategy
1.  **Model** Size as a `ProductVariant` or a special `ModifierGroup` (Required, Max 1).

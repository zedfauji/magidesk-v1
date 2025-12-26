# Backend Forensic Audit: F-0040 Combo Item Selection Dialog

## Feature Context
- **Feature**: Combo Item Selection Dialog
- **Trace from**: `F-0040-combo-item-selection-dialog.md`
- **Reference**: `ComboItemSelectionDialog.java`

## Backend Invariants
1.  **Composition**: A Combo is a Parent Item + Child Components (Burger + Fries + Drink).
2.  **Price Override**: The Combo Price usually overrides the sum of components.
3.  **Selection**: Components usually drawn from specific Menu Groups (e.g., "Any Drink").

## Forbidden States
-   **Incomplete Combo**: Adding the Combo Parent without making the required component selections (Backend must block).

## Audit Requirements
-   **Event**: `COMBO_ADDED`.

## Concurrency Semantics
-   **None**.

## MagiDesk Backend Parity
-   **Model**: ⚠️ `ComboProduct` logic missing/partial.

## Alignment Strategy
1.  **Implement** `TicketItemComponent` list for Combos.

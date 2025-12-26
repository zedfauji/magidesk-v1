# Backend Forensic Audit: F-0037 Pizza Modifiers View

## Feature Context
- **Feature**: Pizza Modifiers View
- **Trace from**: `F-0037-pizza-modifiers-view.md`
- **Reference**: `PizzaModifierSelectionDialog.java`

## Backend Invariants
1.  **Portion Logic**: `1st Half`, `2nd Half`, `Whole`.
2.  **Pricing Rules**:
    -   Half Topping != Half Price usually. Often it's `(ToppingPrice / 2)` or `Full Price`.
    -   System defined rule: `Average`, `Highest`, or `Sum`.
3.  **Visual Representation**: The persistence model must store WHICH half (Left/Right) for the kitchen display.

## Forbidden States
-   **Overlap**: Assigning "Whole" AND "1st Half" for the same topping on the same item.

## Audit Requirements
-   **Event**: None.

## Concurrency Semantics
-   **None**.

## MagiDesk Backend Parity
-   **Model**: ⚠️ Missing implicit "Portion" support in standard `Modifier`.

## Alignment Strategy
1.  **Add** `Portion` enum (Whole, Left, Right) to `TicketItemModifier`.

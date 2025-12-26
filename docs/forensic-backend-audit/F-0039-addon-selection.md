# Backend Forensic Audit: F-0039 Add-on Selection View

## Feature Context
- **Feature**: Add-on Selection View
- **Trace from**: `F-0039-addon-selection-view.md`
- **Reference**: `AddOnSelectionView.java`

## Backend Invariants
1.  **Distinction**: Add-ons are distinct from Modifiers. Usually they are separate Line Items linked to the parent, or Modifiers without grouping constraints.
2.  **Pricing**: Almost always additive.

## Forbidden States
-   **None**.

## Audit Requirements
-   **Event**: None.

## Concurrency Semantics
-   **None**.

## MagiDesk Backend Parity
-   **Model**: ⚠️ `AddOn` entity or just `Modifier`? Need clarification.

## Alignment Strategy
1.  **Map** Add-ons to `TicketItemModifier` but in a specific "Upsell" group.

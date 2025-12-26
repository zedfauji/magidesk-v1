# Backend Forensic Audit: F-0035 Price Entry Dialog

## Feature Context
- **Feature**: Price Entry Dialog
- **Trace from**: `F-0035-price-entry-dialog.md`
- **Reference**: `PriceEntryDialog.java`

## Backend Invariants
1.  **Applicability**: Only triggers for items marked `VariablePrice = true`.
2.  **Validation**: Price cannot be negative (unless Return flow).

## Forbidden States
-   **Bypass**: Saving a variable price item with Price=0.00 (unless explicit 0 allowed).

## Audit Requirements
-   **Event**: `VARIABLE_PRICE_SET`
    -   Payload: TicketId, ItemId, Price.
    -   Severity: INFO.

## Concurrency Semantics
-   **None**.

## MagiDesk Backend Parity
-   **Model**: ✅ `MenuItem` has `IsVariablePrice` flag.

## Alignment Strategy
1.  **Enforce** `Price > 0` in `AddItemCommand` if `Item.IsVariablePrice` is true.

# Backend Forensic Audit: F-0068 Ticket Type Selection Dialog

## Feature Context
- **Feature**: Ticket Type Selection Dialog
- **Trace from**: `F-0068-ticket-type-selection-dialog.md`
- **Reference**: `OrderTypeSelectionDialog.java` (Likely alias to F-0020, but this might be switching active type).

## Backend Invariants
1.  **Switching**: Changing type (e.g., Dine In -> Take Out) might Require Address (F-0061) or Remove Tax (F-0047).
2.  **Recalc**: Trigger auto-tax recalculation.

## Forbidden States
-   **Invalid Switch**: Switching to "Delivery" without assigning a Customer.

## Audit Requirements
-   **Event**: `ORDER_TYPE_CHANGED`.

## Concurrency Semantics
-   **None**.

## MagiDesk Backend Parity
-   **Command**: ✅ `SetOrderTypeCommand` needs validation rules.

## Alignment Strategy
1.  **Enforce** Validation Rules on Type Switch.

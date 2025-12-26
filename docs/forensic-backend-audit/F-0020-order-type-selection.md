# Backend Forensic Audit: F-0020 Order Type Selection Dialog

## Feature Context
- **Feature**: Order Type Selection Dialog
- **Trace from**: `F-0020-order-type-selection-dialog.md`
- **Reference**: `OrderTypeSelectionDialog.java`

## Backend Invariants
1.  **Tax Rules**: Changing Order Type (e.g., Dine In -> To Go) MUST trigger a recalculation of Taxes if the tax rates differ by type.
2.  **Required Fields**: The backend must refuse to SAVE a ticket if the selected Order Type requires Customer/Table and they are missing.
3.  **Price Strategy**: If Order Type dictates a specific Price List (e.g., Happy Hour vs Regular), items must be repriced.

## Forbidden States
-   **Null Type**: A persisted Ticket MUST have a non-null `OrderType` reference.
-   **Invalid Transition**: Changing type to "Bar Tab" if the system configuration forbids it for the current terminal.

## Audit Requirements
-   **Event**: `ORDER_TYPE_CHANGED`
    -   Payload: TicketId, OldType, NewType.
    -   Severity: INFO.

## Concurrency Semantics
-   **Optimization**: If re-pricing is expensive, do it locally? No, backend invariant says Domain must calculate.

## MagiDesk Backend Parity
-   **Entity**: ✅ `OrderType` entity exists.
-   **Logic**: ⚠️ Need to verify `TicketDomainService.SetOrderType` handles tax/price recalc automatically.

## Alignment Strategy
1.  **Implement** `SetOrderTypeCommand` that runs `RecalculateTotals`.
2.  **Add** validation rules to `Ticket.Validate()`.

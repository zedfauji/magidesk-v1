# Backend Forensic Audit: F-0028 Delete Ticket Item Action

## Feature Context
- **Feature**: Delete Ticket Item Action
- **Trace from**: `F-0028-delete-ticket-item-action.md`
- **Reference**: `OrderController.java`

## Backend Invariants
1.  **Unsent vs Sent**:
    -   **Unsent**: Hard delete allowed (remove row).
    -   **Sent**: Hard delete FORBIDDEN. Must use `Void Item` workflow (Soft delete + Reverse Inventory + Log).
2.  **Dependency**: Deleting a parent item MUST delete all child modifiers.

## Forbidden States
-   **Orphaned Modifiers**: Modifiers left behind after parent deletion.

## Audit Requirements
-   **Event**: `ITEM_DELETED`
    -   Payload: TicketId, ItemId, Name, WasSent(false).
    -   Severity: TRACE.

## Concurrency Semantics
-   **Race**: If Cook marks item "Started" while Server hits "Delete", the Delete should fail or convert to Void Request.

## MagiDesk Backend Parity
-   **Validation**: ✅ `TicketDomainService` handles deletion rules.

## Alignment Strategy
1.  **Enforce** `Item.CanDelete` property based on Status.

# Backend Forensic Audit: F-0060 Drawer Assignment Dialog

## Feature Context
- **Feature**: Drawer Assignment Dialog
- **Trace from**: `F-0060-drawer-assignment-dialog.md`
- **Reference**: `DrawerAssignmentView.java`

## Backend Invariants
1.  **Exclusivity**: A Drawer is typically assigned to ONE User at a feature (though "Shared Drawer" exists).
2.  **Timeframe**: Assignment lasts until `Drawer Pull` / `Close`.
3.  **Conflict**: Cannot assign a drawer that is already open/assigned to another active user.

## Forbidden States
-   **Double Assignment**: User A and User B both "Own" Drawer 1 simultaneously (leads to cash accountability issues).

## Audit Requirements
-   **Event**: `DRAWER_ASSIGNED`
    -   Payload: TerminalId, UserId, DrawerId.
    -   Severity: INFO.

## Concurrency Semantics
-   **Strict Lock**: Drawer Assignment Table/Entity must be locked during assignment.

## MagiDesk Backend Parity
-   **Service**: ⚠️ `DrawerAssignmentService` needed.

## Alignment Strategy
1.  **Create** `DrawerAssignment` entity linking `User` + `Terminal` + `Drawer`.

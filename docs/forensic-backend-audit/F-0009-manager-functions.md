# Backend Forensic Audit: F-0009 Manager Functions Dialog

## Feature Context
- **Feature**: Manager Functions Dialog
- **Trace from**: `F-0009-manager-functions-dialog.md`
- **Reference**: `ManagerDialog.java`

## Backend Invariants
1.  **RBAC Enforcement**: Every button/function displayed MUST have a corresponding server-side permission check. The UI hiding the button is NOT sufficient security.
2.  **Session Elevation**: If "Manager Override" is used, the system must distinguish between the "Logged In User" (Server) and the "Authorizing User" (Manager).

## Forbidden States
-   **Unauthorized Access**: Executing a manager command (e.g., `ResetDrawer`) without a valid Manager Token.
-   **Ghost Override**: Applying a manager override without logging WHO provided the override.

## Audit Requirements
-   **Event**: `MANAGER_ACTION`
    -   Payload: ActionName, ManagerId, TargetId (e.g., TicketId).
    -   Severity: WARN.

## Concurrency Semantics
-   **Status Updates**: Server restart/maintenance mode triggered by manager must gracefully handle active sessions on other terminals (e.g., "Drain Mode").

## MagiDesk Backend Parity
-   **RBAC**: ⚠️ Need granular permission mapping for all 20+ manager functions.
-   **Elevation**: ⚠️ Need "Authorizing User" context in Command/Query handlers.

## Alignment Strategy
1.  **Refactor** Commands to accept optional `AuthorizerId` for auditing overrides.
2.  **Audit** all API endpoints to ensure `@RequiresPermission` attributes match Floreant permissions.

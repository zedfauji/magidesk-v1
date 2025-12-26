# Backend Forensic Audit: F-0010 Switchboard Panel

## Feature Context
- **Feature**: Switchboard Panel
- **Trace from**: `F-0010-switchboard-panel.md`
- **Reference**: `SwitchboardView.java` (Panel components)

## Backend Invariants
1.  **Context Consistency**: The panel MUST accurately reflect the logged-in User's permissions and assigned Terminal ID.
2.  **Navigation State**: The backend state "Current View" must match the UI panel to prevent ghost interactions.

## Forbidden States
-   **Desynchronized User**: Panel showing "Manager Options" when the active session has degraded or timed out to "Server".

## Audit Requirements
-   **Event**: `VIEW_ACCESS`
    -   Payload: ViewName, UserId.
    -   Severity: TRACE.

## Concurrency Semantics
-   **Auto-Logout**: If backend enforces auto-logout, it must trigger a view transition away from Switchboard Panel to Login.

## MagiDesk Backend Parity
-   **Navigation**: ✅ `NavigationService` in MVVM handles this.
-   **State**: ✅ `SessionService` holds state.

## Alignment Strategy
1.  **Bind** visibility properties directly to `Session.Permissions`.

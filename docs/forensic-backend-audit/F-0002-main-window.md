# Backend Forensic Audit: F-0002 POS Main Window Shell

## Feature Context
- **Feature**: POS Main Window Shell
- **Trace from**: `F-0002-pos-main-window-shell.md`
- **Reference**: `PosWindow.java`

## Backend Invariants
1.  **Session Scope**: The shell represents a single authenticated session (or an idle terminal state). It implies that specific backend resources (e.g., Cash Drawer allocation) are bound to the *Terminal*, not just the Window.
2.  **Status Broadcast**: Updates to global system status (e.g., "Network Offline", "Printer Error") must be pushed to the shell for display.
3.  **Clock Synchronization**: The time displayed (and used for timestamps) MUST be synchronized with the server time (if client-server) or consistent across the transaction lifecycle.

## Forbidden States
-   **Split Brain**: Displaying "Online" status while backend services are unreachable.
-   **Unbound State**: The shell active and processing inputs without an assigned `TerminalID` context.

## Audit Requirements
-   **Event**: `TERMINAL_STATUS_CHANGE`
    -   Payload: Status (Online/Offline/Locked).
    -   Severity: INFO.

## Concurrency Semantics
-   **Heartbeat**: The backend should expect a heartbeat from the active shell to detect zombie terminals.
-   **Push Notifications**: Backend must trigger "Force Logout" or "Message Broadcast" asynchronously.

## MagiDesk Backend Parity
-   **Status Monitoring**: ❌ Missing heartbeat/monitoring service.
-   **Time Sync**: ✅ Using UTC internally, but UI display needs to match.

## Alignment Strategy
1.  **Implement** `TerminalHeartbeatService` to track active shells.
2.  **Ensure** server-side time is used for all transactions, shell clock is display-only.

# Backend Forensic Audit: F-0057 Shutdown Button

## Feature Context
- **Feature**: Shutdown Button
- **Trace from**: `F-0057-shutdown-button.md`
- **Reference**: `ShutdownAction.java`

## Backend Invariants
1.  **Permission**: Often restricted to Manager or Administrator (to prevent unauthorized terminal shutdown).
2.  **Cleanup**: Must close DB connections, release Printer resources, and potentially shut down the OS (if Kiosk mode).

## Forbidden States
-   **Force Kill**: Data corruption risk if not graceful.

## Audit Requirements
-   **Event**: `SYSTEM_SHUTDOWN`
    -   Payload: UserId.
    -   Severity: CRITICAL.

## Concurrency Semantics
-   **Broadcast**: Should notify Server that "Terminal X is Offline".

## MagiDesk Backend Parity
-   **Service**: ✅ `ApplicationService.Shutdown`.

## Alignment Strategy
1.  **Ensure** Audit Log is flushed before exit.

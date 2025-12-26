# Backend Forensic Audit: F-0001 Application Bootstrap

## Feature Context
- **Feature**: Application Bootstrap and System Initialization
- **Trace from**: `F-0001-application-bootstrap-and-system-initialization.md`
- **Reference**: `Application.java`, `Main.java`

## Backend Invariants
1.  **Single Instance**: The application MUST NOT allow multiple instances to bind to the same peripheral ports (though multiple UI instances might run, hardware locks are exclusive).
2.  **Database Connection**: The system MUST refuse to transition to "Ready" state without a validated connection to the Database.
3.  **Terminal Identity**: The process MUST resolve a unique `TerminalID` from local configuration before processing any transactions.
4.  **Configuration Integrity**: Critical configuration (Tax rates, Currency symbol) MUST be loaded from the database; using hardcoded defaults for these is FORBIDDEN.

## Forbidden States
-   **Partial Initialization**: The system entering a "User Login" state while the Database connection is lost.
-   **Anonymous Terminal**: Creating a Ticket without a resolved `TerminalID`.
-   **Zombie State**: Application running in background but UI closed (shutdown must be total).

## Audit Requirements
-   **Event**: `SYSTEM_STARTUP`
    -   Payload: Timestamp, TerminalID, AppVersion, DatabaseVersion.
    -   Severity: INFO.
-   **Event**: `DB_CONNECTION_FAILURE`
    -   Payload: ErrorCode, ConnectionString (redacted).
    -   Severity: CRITICAL.

## Concurrency Semantics
-   **Startup Lock**: System initialization logic (migrations, cache warming) must be thread-safe if typically run on a background thread.
-   **Global State**: Singleton access to `Application.getInstance()` must be immutable after initialization.

## MagiDesk Backend Parity
-   **Database**: ✅ Managed by `DatabaseConnection.cs`.
-   **Terminal ID**: ⚠️ Missing implicit resolution logic (currently mostly configuration based).
-   **Shutdown**: ✅ Managed by App lifecycle.

## Alignment Strategy
1.  **Implement** `TerminalIdentityService` to validate and enforce unique Terminal IDs on startup.
2.  **Enforce** `StartupHealthCheck` that blocks the Login Screen until DB + Peripherals are "Green".

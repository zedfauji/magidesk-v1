# Backend Forensic Audit: F-0090 Kitchen Status Selector

## Feature Context
- **Feature**: Kitchen Status Selector
- **Trace from**: `F-0090-kitchen-status-selector.md`
- **Reference**: `KitchenStatusSelector.java`

## Backend Invariants
1.  **Transitions**: `NEW` -> `COOKING` -> `DONE`.
2.  **Bump**: "Bumping" a ticket moves it to `DONE` (and removes from view).

## Forbidden States
-   **Reversal**: Moving from DONE back to COOKING (Undo Bump) should be allowed but logged.

## Audit Requirements
-   **Event**: `KDS_BUMP`
    -   Payload: TicketId, StationId.
    -   Severity: INFO.

## Concurrency Semantics
-   **Sync**: Status update must broadcast to other KDS screens.

## MagiDesk Backend Parity
-   **Command**: ⚠️ `UpdateKitchenStatusCommand` needed.

## Alignment Strategy
1.  **Implement** KDS Status Logic.

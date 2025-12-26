# Backend Forensic Audit: F-0080 Change Table Action

## Feature Context
- **Feature**: Change Table Action
- **Trace from**: `F-0080-change-table-action.md`
- **Reference**: `ChangeTableAction.java`

## Backend Invariants
1.  **Transfer**: Moves Ticket from Table A to Table B.
2.  **State**: Table A becomes `FREE` (if no other tickets). Table B becomes `OCCUPIED`.

## Forbidden States
-   **Target Occupied**: Merging functionality is distinct. If Target Occupied, user decision required (Merge or Error). Default "Change Table" usually expects Empty Target.

## Audit Requirements
-   **Event**: `TABLE_CHANGED`
    -   Payload: TicketId, FromTable, ToTable.
    -   Severity: INFO.

## Concurrency Semantics
-   **Lock**: Both tables locked.

## MagiDesk Backend Parity
-   **Command**: ⚠️ `MoveTicketCommand` needed.

## Alignment Strategy
1.  **Implement** atomic transfer logic.

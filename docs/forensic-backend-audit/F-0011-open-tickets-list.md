# Backend Forensic Audit: F-0011 Open Tickets List Dialog

## Feature Context
- **Feature**: Open Tickets List Dialog
- **Trace from**: `F-0011-open-tickets-list-dialog.md`
- **Reference**: `OpenTicketsListDialog.java`

## Backend Invariants
1.  **Unrestricted Access**: When accessed by a Manager, the list query MUST NOT apply the "OwnerID = CurrentUser" filter (unless explicit "My Tickets" toggle is used).
2.  **Status Filter**: Must exclusively return `ACTIVE` tickets.

## Forbidden States
-   **Leakage**: Showing "Voided" or "Closed" tickets in the Open List (unless specifically "All Tickets" audit view).
-   **Zombie Interaction**: Allowing "Edit" on a ticket that was settled on another terminal 1ms ago.

## Audit Requirements
-   **Event**: `MANAGER_TICKET_VIEW`
    -   Payload: ManagerId, TicketId (Resumed).
    -   Severity: INFO.

## Concurrency Semantics
-   **Check-Then-Act**: On selecting a ticket to resume/edit, the backend MUST verify `Status == ACTIVE`. If it changed since the list loaded, reject the action and refresh.

## MagiDesk Backend Parity
-   **Queries**: ✅ `GetTicketListQuery` supports filters.
-   **Validation**: ⚠️ Need explicit `TicketStatus` check in `ResumeTicketCommand`.

## Alignment Strategy
1.  **Refine** `ResumeTicketCommand` to throw `TicketStatusException` if not Active.
2.  **Implement** "Recent Activity" push updates if possible, else rely on manual Refresh.

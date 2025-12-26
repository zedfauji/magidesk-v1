# Backend Forensic Audit: F-0004 Switchboard

## Feature Context
- **Feature**: Switchboard - Open Tickets and Activity
- **Trace from**: `F-0004-switchboard-open-tickets-and-activity.md`
- **Reference**: `SwitchboardView.java`, `TicketDAO.java`

## Backend Invariants
1.  **Status Filtering**: The Switchboard MUST only return keys/tickets with status `ACTIVE` (or equivalent open state). Closed/Voided tickets must not appear in the "Open" list.
2.  **Assignment Visibility**: If the system is configured for "Server-only View", the query MUST be scoped to the `OwnerID` of the current session.
3.  **Ticket Summary**: The summary data (Total, Table Name, Server Name) returned for the list MUST match the detailed `Ticket` record. Consistency is mandatory.

## Forbidden States
-   **Ghost Tickets**: Displaying a ticket ID that does not exist in the database.
-   **Stale Status**: Showing a ticket as "Open" when it has already been paid/closed on another terminal.

## Audit Requirements
-   **Event**: `TICKET_RESUME`
    -   Payload: TicketId, UserId, TerminalId.
    -   Severity: INFO.

## Concurrency Semantics
-   **List Freshness**: The list view is a snapshot. Using a "Stale" ticket (clicking it) must perform a check-before-lock.
-   **Race Condition**: Two users clicking the same ticket simultaneously -> First lock wins, second gets (optional) notification "Ticket locked by X".

## MagiDesk Backend Parity
-   **Queries**: ✅ `GetOpenTicketsQuery` exists.
-   **Locking**: ⚠️ Missing explicit "Ticket Lock" mechanism for edits.
-   **Real-time**: ⚠️ No signalR/Socket push for new tickets created by others.

## Alignment Strategy
1.  **Implement** Optimistic Concurrency on Ticket edits.
2.  **Refresh** the list on navigate-to or timer-based polling if Push is not MVP.

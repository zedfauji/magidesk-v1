# Backend Forensic Audit: F-0019 New Ticket Action

## Feature Context
- **Feature**: New Ticket Action
- **Trace from**: `F-0019-new-ticket-action.md`
- **Reference**: `NewTicketAction.java`, `OrderController.java`

## Backend Invariants
1.  **Freshness**: The action must instantiate a FRESH `Ticket` object. Reuse of pooled objects must be carefully reset (User, Terminal, Items cleared).
2.  **Context**: The new ticket must immediately be bound to the Current User and Terminal.

## Forbidden States
-   **Unsaved Leak**: If the user cancels the "New Ticket" flow (e.g., at Order Type selection), the transient ticket must be discarded and NOT persisted as an empty record.

## Audit Requirements
-   **Event**: `TICKET_INITIATED`
    -   Payload: UserId, TerminalId.
    -   Severity: TRACE.

## Concurrency Semantics
-   **Session Scope**: A single user session typically handles one active ticket creation at a time. Multi-tab concurrency handled by Switchboard, not this action individually.

## MagiDesk Backend Parity
-   **Factory**: ✅ `TicketFactory` handles initialization logic.

## Alignment Strategy
1.  **Enforce** `TicketFactory.Create()` to populate default values (Currency, Tax Exempt=False).

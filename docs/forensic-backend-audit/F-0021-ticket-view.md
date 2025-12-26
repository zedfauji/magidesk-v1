# Backend Forensic Audit: F-0021 Ticket View Panel

## Feature Context
- **Feature**: Ticket View Panel
- **Trace from**: `F-0021-ticket-view-panel.md`
- **Reference**: `TicketView.java`, `TicketItem.java`

## Backend Invariants
1.  **Line Item Hierarchy**: The structure must strictly preserve parent-child relationships (Main Item -> Modifiers).
2.  **Display Accuracy**: The "Due Amount" shown MUST be `Total - PaidAmount`.
3.  **Seat Grouping**: If items are assigned to seats, this assignment prevents grouping/aggregation with items from other seats.

## Forbidden States
-   **Phantom Tax**: Displaying a tax line that doesn't sum from the line items.
-   **Desync**: Showing a Total that doesn't match the sum of lines.

## Audit Requirements
-   **Event**: `TICKET_VIEW_LOAD`
    -   Payload: TicketId.
    -   Severity: TRACE.

## Concurrency Semantics
-   **Read Snapshot**: The view is a read-model. Updates (from other terminals) might not reflect instantly without polling/push.

## MagiDesk Backend Parity
-   **Model**: ✅ `Ticket` aggregate root handles this structure.

## Alignment Strategy
1.  **Ensure** `TicketDTO` includes explicit `SeatNumber` and `ModifierGroup` hierarchy.

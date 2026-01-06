# Table System Forensic Audit - Master Report
**Date**: 2024-05-23
**Scope**: Table Designer, Table Map, Table Explorer

## Executive Summary
The Table System suffers from significant **fragmentation of business logic** and **state synchronization failures**. The most critical issue is that creating a ticket from the Map does not correctly update the Table's status in the database, leading to a permanent "Available" state even when occupied. Additionally, the "Table Explorer" feature is functionally broken, as it fails to assign the selected table to the ticket it creates.

## Critical Findings (P0)

1.  **State Desynchronization (The "Available" Bug)**
    *   **Root Cause**: `CreateTicketCommandHandler` adds a table number to the *Ticket* entity but fails to update the *Table* entity's `Status` or `CurrentTicketId`.
    *   **Impact**: Tables never appear "Red/Occupied" on the map, leading to double-seating.

2.  **Broken Explorer Flow**
    *   **Root Cause**: `TableExplorerViewModel` navigates to Order Entry with `null` ticket ID. `OrderEntryViewModel` creates a generic "Walk-in" ticket, ignoring the table usage completely.
    *   **Impact**: Waiters selecting "Table 5" in Explorer create an unassigned ticket.

3.  **Designer Visibility Blindspot**
    *   **Root Cause**: The Designer loads tables using `GetAvailableAsync`, which filters out occupied tables.
    *   **Impact**: Occupied tables disappear from the layout editor. Managers cannot move or adjust tables while the restaurant is active.

## Major Findings (P1)

4.  **Canvas Size Mismatch**
    *   **Details**: Designer allows infinite placement; Map is hardcoded to 2000x2000.
    *   **Impact**: Tables placed at X=2100 are invisible to operations staff.

5.  **Drag & Drop Data Integrity**
    *   **Details**: UI updates position immediately via Code-Behind; Database update is async/detached.
    *   **Risk**: Network failure leaves UI showing false position until reload.

6.  **Navigation Dead Ends**
    *   **Details**: Returning from "Order Entry" (initiated via Explorer) jumps to "Map" instead of "Explorer".

## Recommendations & Roadmap

### Phase 1: Fix Core Integrity (Immediate)
1.  **Patch `CreateTicketCommandHandler`**: Inject `ITableRepository` and enforce `table.AssignTicket(ticket.Id)` within the transaction.
2.  **Fix Designer Loading**: Change `TableDesignerViewModel` to use `GetActiveAsync` instead of `GetAvailableAsync`.

### Phase 2: Unify Logic (Short Term)
3.  **Refactor Explorer Flow**: Update `TableExplorerViewModel` to call `CreateTicketCommand` (with table #) *before* navigating, mirroring the Map's behavior.
4.  **Consolidate Ticket Creation**: Abstract the ticket creation logic (User/Terminal/Shift checks) from `TableMapViewModel` into a shared `ITicketService`.

### Phase 3: UX & Resilience (Medium Term)
5.  **Dynamic Map Canvas**: Bind `Canvas.Width/Height` in `TableMapPage` to the floor dimensions or enable infinite scrolling.
6.  **Optimistic Concurrency**: Implement "Snap Back" behavior in Designer if `UpdateTablePosition` fails.

## Artifacts Produced
- [Surface Inventory](surface_inventory.md)
- [State Model Trace](state_model_trace.md)
- [Consistency Gaps](consistency_gaps.md)
- [Failure & Edge Cases](failure_edge_cases.md)
- [Navigation Flows](navigation_flows.md)

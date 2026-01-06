# Ticket: Phase 1 - Core Integrity Fixes
**Priority**: P0 (Critical)
**Status**: Ready for Implementation

## Problem Statement
The Table System currently has two critical defects affecting data integrity and operations:
1.  **State Desynchronization**: Creating a ticket from the Table Map creates a `Ticket` entity but fails to update the `Table` entity's `Status`. The table remains "Available" in the database, allowing double-seating and causing confusion.
2.  **Designer Visibility**: The Table Designer uses `GetAvailableAsync`, which filters out occupied tables. Managers cannot see or edit the layout of occupied tables during service.

## Objectives
1.  Ensure atomic consistency between `Ticket` creation and `Table` status.
2.  Ensure the Table Designer displays the TRUE state of the floor, regardless of occupancy.

## Technical Specifications

### 1. Fix `CreateTicketCommandHandler`
**File**: `Magidesk.Application/Services/CreateTicketCommandHandler.cs`

**Changes**:
-   Inject `ITableRepository`.
-   In `HandleAsync`, iterate through `command.TableNumbers` (if present).
-   For each table number:
    -   Load the `Table` entity using `_tableRepository.GetByTableNumberAsync`.
    -   **Validation**: Ensure table is not null.
    -   **Validation**: Check `table.Status`. If not `Available`, decide on behavior (Throw exception? Force merge? For now, throw `BusinessRuleViolationException`).
    -   Call `table.AssignTicket(ticket.Id)`.
    -   Call `_tableRepository.UpdateAsync(table)`.
    -   **IMPORTANT**: Ensure this happens within the same transaction scope if implicit, or logically coupled.

### 2. Fix `TableDesignerViewModel`
**File**: `Magidesk/ViewModels/TableDesignerViewModel.cs`

**Changes**:
-   Locate `LoadTablesAsync` method.
-   Replace call to `_tableRepository.GetAvailableAsync` (or the Query equivalent) with `_tableRepository.GetActiveAsync`.
-   Ensure the mapping logic correctly handles `Status` property so occupied tables render (even if they can't be moved, though ideally they SHOULD be movable unless locked).
-   *Note*: The Designer has a `Status` property on `TableDto`. Ensure the UI `TableStatusToBrushConverter` handles generic statuses if they differ from "Available".

## Acceptance Criteria
-   [ ] **Sync Test**: Create a ticket for "Table 5" via Map. Refresh DB or restart app. "Table 5" must show as `Status = Seat` (Red).
-   [ ] **Designer Test**: Seat "Table 1". Open Table Designer. "Table 1" must be visible.
-   [ ] **Double Seat Prevention**: Try to create another ticket for "Table 5". System should throw/block or prompt (depending on decided validation).

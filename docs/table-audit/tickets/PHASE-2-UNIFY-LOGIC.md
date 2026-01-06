# Ticket: Phase 2 - Logic Unification
**Priority**: P1 (High)
**Status**: Pending Phase 1

## Problem Statement
The logic for creating tickets and validating context (Shift, Terminal, User) is fragmented.
1.  **Table Explorer Broken**: Selecting a table navigates to Order Entry with a `null` Ticket ID. `OrderEntryViewModel` then creates a *new*, unassociated ticket ("Walk-in"), ignoring the selected table.
2.  **Duplicated Logic**: `TableMapViewModel` contains complex logic for Shift/Terminal validation and OrderType lookup that should be shared.

## Objectives
1.  Fix the Table Explorer workflow so selecting a table actually assigns it.
2.  Centralize ticket creation business rules to prevent drift.

## Technical Specifications

### 1. Introduce `TicketCreationService` (Domain Service)
**Location**: `Magidesk.Application/Services/TicketCreationService.cs` (Interface extracts to `Interfaces/`)

**Responsibilities**:
-   Validate `CurrentUser`, `TerminalContext`, `OpenSession`.
-   Lookup default `OrderType` (Dine In).
-   Construct and dispatch `CreateTicketCommand`.
-   Return `TicketId`.

### 2. Refactor `TableMapViewModel`
**File**: `Magidesk/ViewModels/TableMapViewModel.cs`

**Changes**:
-   Remove raw logic for OrderType/Session lookups.
-   Inject `ITicketCreationService`.
-   Call `_ticketCreationService.CreateTicketForTableAsync(tableId)`.

### 3. Refactor `TableExplorerViewModel`
**File**: `Magidesk/ViewModels/TableExplorerViewModel.cs`

**Changes**:
-   **Current Behavior**: `_navigationService.Navigate(..., null)`.
-   **New Behavior**:
    -   Inject `ITicketCreationService`.
    -   In `SelectTableCommand`:
        -   If `table.Status == Seat`: Navigate to existing `table.CurrentTicketId`.
        -   If `table.Status == Available`:
            -   Call `_ticketCreationService.CreateTicketForTableAsync(table.Id)`.
            -   Await result.
            -   Navigate to `OrderEntryPage` with `result.TicketId`.

## Acceptance Criteria
-   [ ] **Explorer Test**: Open Table Explorer. Search "Table 10". Click Select.
    -   Order Entry opens.
    -   Ticket # is generated.
    -   Ticket is linked to "Table 10" (DB Check).
-   [ ] **Map Regression Test**: Clicking a table in Map still creates a ticket correctly using the new Service.

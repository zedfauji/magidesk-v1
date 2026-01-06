# Table System State Model Trace

## 1. State Definition
- **Table Entity**: `Magidesk.Domain.Entities.Table`
  - Properties: `Id`, `TableNumber`, `Status`, `CurrentTicketId`, `X`, `Y`.
  - **Status Invariants**: `Available` <-> `Seat`.
- **Ticket Entity**: `Magidesk.Domain.Entities.Ticket`
  - Properties: `TableNumbers` (List<int>).

## 2. Read Path
- **UI**: `TableMapViewModel` / `TableDesignerViewModel`
- **Query**: `GetTableMapQuery` / `GetAvailableTablesQuery`
- **Source**: `ITableRepository` -> SQL Database.
- **Projection**: `TableDto` mirrors `Table` entity fields.

## 3. Write Paths

### Path A: Designer Updates (Position)
- **Command**: `UpdateTablePositionCommand`
- **Handler**: `UpdateTablePositionCommandHandler` (Assumed)
- **Effect**: Updates `X`, `Y`, `Shape` on `Table` entity.
- **Verdict**: **SAFE** (Isolated from business state).

### Path B: Assign Table to Existing Ticket (Move/Merge)
- **Command**: `AssignTableToTicketCommand`
- **Handler**: `AssignTableToTicketCommandHandler`
- **Logic**:
  1. Load Table & Ticket.
  2. Call `table.AssignTicket(ticketId)`.
  3. Call `ticket.AddTableNumber(tableNum)`.
  4. Persist both.
- **Verdict**: **CORRECT**. Consistent state.

### Path C: New Ticket Creation (from Map)
- **Command**: `CreateTicketCommand`
- **Handler**: `CreateTicketCommandHandler`
- **Logic**:
  1. Creates `Ticket`.
  2. Adds table numbers to `Ticket`.
  3. SAVES `Ticket`.
- **CRITICAL FAILURE**:
  - **Does NOT load Table entities.**
  - **Does NOT call `AssignTicket`.**
  - **Result**: Ticket created with Table #5, but Table #5 remains "Available" in DB.
  - **Symptom**: Map will not update status to "Occupied" even after Refresh.

## 4. Single Source of Truth
- **Violated**. Ticket knows it has a table, Table does not know it has a ticket.
- **Remediation**: `CreateTicketCommandHandler` must orchestrate `Table` updates within the same transaction.

## 5. State Storage locations
- **SQL Table `Tables`**: Authoritative for Status, Position.
- **SQL Table `Tickets`**: Authoritative for financial state.
- **UI ObservableCollection**: Ephemeral, synced via polling.

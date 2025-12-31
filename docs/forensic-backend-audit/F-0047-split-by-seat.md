# Backend Forensic Audit: F-0047 Split by Seat Dialog

## Feature Context
- **Feature**: Split by Seat Dialog
- **Trace from**: `F-0047-split-by-seat-dialog.md`
- **Reference**: `SplitTicketDialog.java` seat mode, `TicketService`

## Backend Invariants
1. **Conservation of Value**: `Sum(NewTickets.Total) == OriginalTicket.Total`. No value can be created or destroyed during a split.
2. **Seat Grouping**: Items with the same `SeatNumber` must move to the same new ticket.
3. **Item Atomicity**: A `TicketItem` must move entirely to its seat's new ticket (unless fractional split allowed).
4. **Modifier Integrity**: Modifiers MUST stay attached to their parent Item during the move.

## Forbidden States
- **Orphaned Payments**: If the original ticket had partial payments, they must be handled (cannot split paid tickets).
- **Unassigned Items**: Items without `SeatNumber` must prompt for assignment before split.
- **Empty Seat Tickets**: Resulting tickets must have at least one item.

## Audit Requirements
- **Event**: `TICKET_SPLIT_BY_SEAT`
  - Payload: OriginalTicketId, NewTicketIds, SeatsCount, ItemsPerSeat.
  - Severity: INFO.

## Concurrency Semantics
- **Transactional**: The entire split operation (create new tickets, move items, close original) MUST be ACID.

## MagiDesk Backend Parity
- **Split Logic**: ✅ `SplitTicketCommand` exists (F-0014)
- **Seat Assignment**: ✅ `TicketItem.SeatNumber` exists (F-0086)
- **Split Trigger**: ✅ Split button exists (F-0053)
- **Missing**: SplitBySeatCommand with seat grouping logic

## Alignment Strategy
1. **Create** `SplitBySeatCommand` that extends existing split logic
2. **Leverage** existing `SplitTicketCommand` for core operations
3. **Use** `TicketItem.SeatNumber` for grouping
4. **Validate** all items have seat assignments before split
5. **Create** separate tickets per seat with items grouped by seat

## Required Components
- **Command**: `SplitBySeatCommand`
- **Handler**: `SplitBySeatCommandHandler`
- **Validation**: Seat assignment validation
- **Logic**: Group items by seat, create tickets per seat
- **Integration**: Extend existing SplitTicketDialog with seat mode

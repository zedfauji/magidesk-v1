# Backend Forensic Audit: Split by Amount Dialog (F-0049)

## Feature Context
- **Feature**: Split by Amount Dialog
- **Trace from**: UI audit `F-0049-split-by-amount-dialog.md`
- **Reference**: `SplitTicketDialog.java` amount mode

## Backend Invariants
1. **Conservation of value**: Sum of amounts assigned to new tickets + remainder MUST equal the original ticket total (within currency rounding rules).
2. **Validation**: Each requested split amount MUST be > 0 and <= remaining balance at time of creation.
3. **Remainder**: If the user-provided amounts do not sum to total, the remainder MUST stay on original or be assigned to a final ticket per UI flow.
4. **Ticket state**: Original ticket MUST be OPEN and have items.
5. **Atomicity**: Operation MUST be transactional.

## Forbidden States
- **Amount > total**: Requested amount greater than current ticket total.
- **Zero/negative amount**: Any split amount <= 0.

## Audit Requirements
- **Event**: `TICKET_SPLIT_BY_AMOUNT`
  - Payload: OriginalTicketId, SplitAmounts, NewTicketIds, RemainderAmount.
  - Severity: INFO.

## Concurrency Semantics
- **Ticket modification concurrency**: If the ticket was modified during split, operation MUST fail or retry per repository semantics.

## MagiDesk Backend Parity
- Status: **MISSING**
- Needs: `SplitByAmountCommand`/handler implementing invariants above.

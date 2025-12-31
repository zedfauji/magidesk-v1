# Backend Forensic Audit: Split Even Dialog (F-0048)

## Feature Context
- **Feature**: Split Even Dialog
- **Trace from**: UI audit `F-0048-split-even-dialog.md`
- **Reference**: `SplitTicketDialog.java` even mode

## Backend Invariants
1. **Conservation of value**: Sum of totals across all resulting tickets MUST equal the original ticket total (within currency rounding rules).
2. **Rounding rule**: When Total / N is not evenly divisible in currency, the **last split** MUST absorb the remainder so totals still reconcile.
3. **Ticket state**: Original ticket MUST be OPEN and have at least one order line.
4. **Atomicity**: Split operation MUST be transactional (all new tickets created + original updated/closed, or none).

## Forbidden States
- **N < 2**: Cannot split into fewer than 2 ways.
- **Empty ticket**: Cannot split an empty ticket.
- **Negative/zero totals**: Any resulting ticket total <= 0 is forbidden.

## Audit Requirements
- **Event**: `TICKET_SPLIT_EVEN`
  - Payload: OriginalTicketId, Ways(N), NewTicketIds, AmountPerTicket, RemainderAmount.
  - Severity: INFO.

## Concurrency Semantics
- **Ticket modification concurrency**: If the ticket was modified during the split, the operation MUST fail or retry based on repository concurrency strategy.

## MagiDesk Backend Parity
- Status: **MISSING**
- Needs: `SplitEvenCommand`/handler (or extend existing split workflow) implementing invariants above.

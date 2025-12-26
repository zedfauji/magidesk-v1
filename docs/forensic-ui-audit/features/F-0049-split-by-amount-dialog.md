# Feature: Split by Amount Dialog (F-0049)

## Classification
- **Parity classification**: PARITY REQUIRED
- **MagiDesk status**: MISSING

## Problem / Why this exists (grounded)
- **Operational need**: Split specific amounts off main ticket. "I'll pay $20 of this bill."
- **Evidence**: `SplitTicketDialog.java` with amount mode - split by dollar amount.

## User-facing surfaces
- **Surface type**: Modal dialog (mode within SplitTicketDialog)
- **UI entry points**: SplitTicketDialog → Split by Amount
- **Exit paths**: Tickets created / Cancel

## Preconditions & protections
- **User/role/permission checks**: Split permission
- **State checks**: Ticket has sufficient balance
- **Manager override**: May be required

## Step-by-step behavior (forensic)
1. User opens SplitTicketDialog
2. Selects "Split by Amount" mode
3. Enters amount(s) to split off:
   - First person pays: $25
   - Second person pays: $30
   - Remainder: $X
4. System creates tickets:
   - New ticket for each amount
   - Remaining balance on original or last ticket
5. Original updated or closed

## Edge cases & failure paths
- **Amount > total**: Prevented
- **Doesn't add up**: Remainder calculated
- **Multiple amounts**: Sequential or all at once

## Data / audit / financial impact
- **Writes/updates**: New tickets for amounts; original reduced
- **Audit events**: Split logged
- **Financial risk**: Amounts must total correctly

## Code traceability (REQUIRED)
- **Primary UI class(es)**: SplitTicketDialog amount mode
- **Entry action(s)**: Split by amount action
- **Workflow/service enforcement**: Amount split calculation
- **Messages/labels**: Amount entry

## MagiDesk parity notes
- **What exists today**: No amount-based split
- **What differs / missing**: Split by amount mode

## Porting strategy (PLAN ONLY)
- **Backend requirements**: SplitByAmountCommand
- **API/DTO requirements**: Split by amount endpoint
- **UI requirements**: SplitTicketDialog amount mode
- **Constraints for implementers**: Validate total; handle remainder

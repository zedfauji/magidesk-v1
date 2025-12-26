# Feature: Split Even Dialog (F-0048)

## Classification
- **Parity classification**: PARITY REQUIRED
- **MagiDesk status**: MISSING

## Problem / Why this exists (grounded)
- **Operational need**: Split ticket evenly among N people. Common request: "Split it four ways."
- **Evidence**: `SplitTicketDialog.java` with even mode - divide total by N guests.

## User-facing surfaces
- **Surface type**: Modal dialog (mode within SplitTicketDialog)
- **UI entry points**: SplitTicketDialog → Split Evenly
- **Exit paths**: Tickets created / Cancel

## Preconditions & protections
- **User/role/permission checks**: Split permission
- **State checks**: Ticket open with items
- **Manager override**: May be required

## Step-by-step behavior (forensic)
1. User opens SplitTicketDialog
2. Selects "Split Evenly" mode
3. Enters number of ways (N)
4. System calculates:
   - Total / N = amount per ticket
   - Handles rounding (last ticket gets remainder)
5. Separate tickets created
6. Original closed

## Edge cases & failure paths
- **Rounding issues**: Last ticket absorbs difference
- **Tips**: Split or separate
- **Already paid partial**: Complex handling

## Data / audit / financial impact
- **Writes/updates**: N new tickets with equal shares
- **Audit events**: Split logged
- **Financial risk**: Rounding discrepancies

## Code traceability (REQUIRED)
- **Primary UI class(es)**: SplitTicketDialog even mode
- **Entry action(s)**: Split even action
- **Workflow/service enforcement**: Even split calculation
- **Messages/labels**: Number entry

## MagiDesk parity notes
- **What exists today**: No even split
- **What differs / missing**: Split evenly mode

## Porting strategy (PLAN ONLY)
- **Backend requirements**: SplitEvenlyCommand
- **API/DTO requirements**: Split by N endpoint
- **UI requirements**: SplitTicketDialog even mode
- **Constraints for implementers**: Handle rounding; maintain total

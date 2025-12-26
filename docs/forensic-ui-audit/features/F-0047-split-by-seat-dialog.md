# Feature: Split by Seat Dialog (F-0047)

## Classification
- **Parity classification**: PARITY WITH MODERNIZATION
- **MagiDesk status**: MISSING

## Problem / Why this exists (grounded)
- **Operational need**: Split ticket by seat numbers - each diner gets their own items on their own check.
- **Evidence**: `SplitTicketDialog.java` with seat mode - separate tickets by seat assignment.

## User-facing surfaces
- **Surface type**: Modal dialog (mode within SplitTicketDialog)
- **UI entry points**: SplitTicketDialog → Split by Seat
- **Exit paths**: Tickets created / Cancel

## Preconditions & protections
- **User/role/permission checks**: Split permission
- **State checks**: Items have seat assignments
- **Manager override**: May be required

## Step-by-step behavior (forensic)
1. User opens SplitTicketDialog
2. Selects "Split by Seat" mode
3. System shows:
   - Items grouped by seat number
   - Seat 1: items...
   - Seat 2: items...
4. User confirms split
5. Separate tickets created per seat
6. Original ticket replaced/closed

## Edge cases & failure paths
- **Unassigned items**: Prompt for assignment or default
- **Shared items**: Prompt for split or assignment
- **Single seat**: No split needed

## Data / audit / financial impact
- **Writes/updates**: New tickets per seat; original ticket split
- **Audit events**: Split logged
- **Financial risk**: Correct item attribution

## Code traceability (REQUIRED)
- **Primary UI class(es)**: SplitTicketDialog seat mode
- **Entry action(s)**: Split by seat action
- **Workflow/service enforcement**: Ticket split logic
- **Messages/labels**: Seat labels

## MagiDesk parity notes
- **What exists today**: No seat-based split
- **What differs / missing**: Split by seat mode

## Porting strategy (PLAN ONLY)
- **Backend requirements**: SplitBySeatCommand
- **API/DTO requirements**: Split by seat endpoint
- **UI requirements**: SplitTicketDialog seat mode
- **Constraints for implementers**: Requires seat tracking on items

# Feature: Notes Dialog (F-0125)

## Classification
- **Parity classification**: PARITY REQUIRED
- **MagiDesk status**: MISSING

## Problem / Why this exists (grounded)
- **Operational need**: Staff need to add special instructions or notes to tickets or items (allergies, special requests, delivery instructions).
- **Evidence**: `NotesDialog.java` - text entry dialog; applies to ticket or ticket item; prints on kitchen ticket.

## User-facing surfaces
- **Surface type**: Modal dialog
- **UI entry points**: TicketView → Note button; OrderView → Add Note
- **Exit paths**: OK (saves note) / Cancel

## Preconditions & protections
- **User/role/permission checks**: Order entry permission
- **State checks**: Ticket/item must exist
- **Manager override**: Not required

## Step-by-step behavior (forensic)
1. User selects item or ticket
2. Clicks Note/Add Note button
3. NotesDialog opens with text area
4. User types note (free text)
5. On OK: Note attached to item/ticket
6. Note displays on receipt and kitchen ticket
7. Note visible in ticket view

## Edge cases & failure paths
- **Empty note**: May clear existing
- **Very long note**: May truncate on print
- **Multiple notes**: Appended or replaced

## Data / audit / financial impact
- **Writes/updates**: Ticket.note or TicketItem.cookingNote
- **Audit events**: Part of order modification
- **Financial risk**: None (informational)

## Code traceability (REQUIRED)
- **Primary UI class(es)**: 
  - `NotesDialog` → `ui/dialog/NotesDialog.java`
  - `NoteView` → `ui/views/NoteView.java`
- **Entry action(s)**: Button in TicketView
- **Workflow/service enforcement**: Ticket update
- **Messages/labels**: Note prompt

## MagiDesk parity notes
- **What exists today**: Basic note fields may exist
- **What differs / missing**: NotesDialog UI

## Porting strategy (PLAN ONLY)
- **Backend requirements**: Note field on Ticket and OrderLine
- **API/DTO requirements**: Included in ticket update
- **UI requirements**: NotesDialog with text entry
- **Constraints for implementers**: Notes must print on kitchen ticket

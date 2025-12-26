# Feature: Guest Count Entry Dialog (F-0023)

## Classification
- **Parity classification**: PARITY WITH MODERNIZATION
- **MagiDesk status**: MISSING

## Problem / Why this exists (grounded)
- **Operational need**: Track number of guests per ticket for covers reporting, per-person averages, capacity management.
- **Evidence**: `NumberSelectionDialog2.java` + guest count in ticket - numeric entry for guest count.

## User-facing surfaces
- **Surface type**: Modal dialog (or inline input)
- **UI entry points**: New dine-in order; ticket modification
- **Exit paths**: Guest count entered / Skip

## Preconditions & protections
- **User/role/permission checks**: Order entry permission
- **State checks**: Ticket open; dine-in order type
- **Manager override**: Not required

## Step-by-step behavior (forensic)
1. New dine-in ticket created
2. GuestCountDialog opens (if configured)
3. Shows numeric input:
   - Current guest count (if set)
   - Numeric buttons 1-9
   - Clear/backspace
4. User enters guest count
5. On OK: Guest count stored with ticket
6. Affects per-person average calculations

## Edge cases & failure paths
- **Zero guests**: May prevent or allow
- **Large party**: No upper limit typically
- **Skip guest count**: Default to 1 or table capacity

## Data / audit / financial impact
- **Writes/updates**: Ticket.guestCount
- **Audit events**: Part of ticket
- **Financial risk**: Affects covers reporting

## Code traceability (REQUIRED)
- **Primary UI class(es)**: `NumberSelectionDialog2` → `ui/dialog/NumberSelectionDialog2.java`
- **Entry action(s)**: Part of new ticket flow
- **Workflow/service enforcement**: Ticket creation
- **Messages/labels**: Guest count prompt

## MagiDesk parity notes
- **What exists today**: GuestCount field may exist on ticket
- **What differs / missing**: Guest count entry dialog

## Porting strategy (PLAN ONLY)
- **Backend requirements**: GuestCount field on Ticket
- **API/DTO requirements**: Included in ticket DTO
- **UI requirements**: NumberEntryDialog for guest count
- **Constraints for implementers**: Optional based on order type

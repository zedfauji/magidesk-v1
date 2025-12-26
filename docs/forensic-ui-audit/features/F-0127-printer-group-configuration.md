# Feature: Printer Group Configuration (F-0127)

## Classification
- **Parity classification**: PARITY REQUIRED
- **MagiDesk status**: MISSING

## Problem / Why this exists (grounded)
- **Operational need**: Route orders to correct kitchen printer (bar, hot line, cold line). Menu items assigned to printer groups.
- **Evidence**: `PrinterGroup` entity + `VirtualPrinter` + routing - printer assignment and routing.

## User-facing surfaces
- **Surface type**: Configuration view (in Back Office)
- **UI entry points**: BackOfficeWindow → Configuration → Printer Groups
- **Exit paths**: Save / Cancel

## Preconditions & protections
- **User/role/permission checks**: Printer configuration permission
- **State checks**: Physical printers configured
- **Manager override**: Not required

## Step-by-step behavior (forensic)
1. Open Printer Group Configuration
2. View shows groups:
   - Group name
   - Assigned printers
   - Assigned menu categories
3. Create/Edit/Delete groups
4. Group form:
   - Name
   - Printers (multi-select)
   - Categories/items (what prints here)
   - Print order (priority)
5. Save persists group
6. Items route to correct printers

## Edge cases & failure paths
- **Printer offline**: Route to backup or queue
- **Item not assigned**: Default printer group

## Data / audit / financial impact
- **Writes/updates**: PrinterGroup entity
- **Audit events**: Config changes
- **Financial risk**: None (operational)

## Code traceability (REQUIRED)
- **Primary UI class(es)**: PrinterGroup configuration
- **Entry action(s)**: Configuration menu
- **Workflow/service enforcement**: Print routing
- **Messages/labels**: Group labels

## MagiDesk parity notes
- **What exists today**: No printer groups
- **What differs / missing**: Entire print routing system

## Porting strategy (PLAN ONLY)
- **Backend requirements**: PrinterGroup entity; item-group assignment
- **API/DTO requirements**: GET/POST/PUT/DELETE /printer-groups
- **UI requirements**: PrinterGroup configuration view
- **Constraints for implementers**: Routes by item category/group

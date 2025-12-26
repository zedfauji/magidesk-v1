# Feature: Shift Explorer (F-0119)

## Classification
- **Parity classification**: PARITY WITH MODERNIZATION
- **MagiDesk status**: MISSING

## Problem / Why this exists (grounded)
- **Operational need**: Define work shifts (morning, evening, split) with start/end times. Shifts affect labor reporting, employee scheduling, cash accountability.
- **Evidence**: `ShiftExplorer.java` - CRUD for shift definitions; name, start time, end time; used in attendance and reporting.

## User-facing surfaces
- **Surface type**: Explorer panel (in Back Office)
- **UI entry points**: BackOfficeWindow → Explorers → Shifts
- **Exit paths**: Close tab

## Preconditions & protections
- **User/role/permission checks**: Shift management permission
- **State checks**: None
- **Manager override**: Not required

## Step-by-step behavior (forensic)
1. Open Shift Explorer from Back Office
2. View shows table of shifts:
   - Shift name
   - Start time
   - End time
   - Active status
3. Actions:
   - New: Create new shift
   - Edit: Modify shift times
   - Delete: Remove shift (if unused)
4. Shift form includes:
   - Name (e.g., "Morning", "Evening", "Split")
   - Start time (hour:minute)
   - End time (hour:minute)
5. Shifts assigned to users and used in reports

## Edge cases & failure paths
- **Overlapping shifts**: Allowed (flexible scheduling)
- **Delete shift in use**: Prevent or warn
- **Overnight shift**: End time < start time = next day

## Data / audit / financial impact
- **Writes/updates**: Shift entity
- **Audit events**: Shift changes logged
- **Financial risk**: Low - affects labor reports

## Code traceability (REQUIRED)
- **Primary UI class(es)**: `ShiftExplorer` → `bo/ui/explorer/ShiftExplorer.java`
- **Entry action(s)**: `ShiftExplorerAction` → `bo/actions/ShiftExplorerAction.java`
- **Workflow/service enforcement**: ShiftDAO
- **Messages/labels**: Shift name, time labels

## Uncertainties (STOP; do not guess)
- Shift differential pay
- Automatic shift detection based on clock in time

## MagiDesk parity notes
- **What exists today**: Shift entity exists
- **What differs / missing**: Shift Explorer UI

## Porting strategy (PLAN ONLY)
- **Backend requirements**: 
  - Shift entity with name, startTime, endTime
  - ShiftService for CRUD
- **API/DTO requirements**: GET/POST/PUT/DELETE /shifts
- **UI requirements**: ShiftExplorer with table and form
- **Constraints for implementers**: Time handling must consider overnight shifts

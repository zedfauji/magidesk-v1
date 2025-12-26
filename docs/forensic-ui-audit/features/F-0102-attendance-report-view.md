# Feature: Attendance Report View (F-0102)

## Classification
- **Parity classification**: PARITY WITH MODERNIZATION
- **MagiDesk status**: MISSING

## Problem / Why this exists (grounded)
- **Operational need**: HR/managers need to view employee attendance history for payroll, scheduling, and compliance.
- **Evidence**: `AttendanceReportView.java` + `AttendanceHistoryExplorer.java` - lists clock in/out records by employee and date range.

## User-facing surfaces
- **Surface type**: Report view (in Back Office)
- **UI entry points**: BackOfficeWindow → Reports → Attendance
- **Exit paths**: Close tab / Print / Export

## Preconditions & protections
- **User/role/permission checks**: HR/Attendance report permission
- **State checks**: None
- **Manager override**: Not required

## Step-by-step behavior (forensic)
1. Open Attendance Report
2. Select filters:
   - Date range
   - Employee (optional)
   - Shift (optional)
3. Generate report shows:
   - Employee name
   - Clock in time
   - Clock out time
   - Hours worked
   - Overtime flag
   - Notes/edits
4. Totals per employee
5. Print or export

## Edge cases & failure paths
- **Missing clock out**: Shows open shift
- **Edited entries**: Shows edit history
- **No data**: Empty report

## Data / audit / financial impact
- **Writes/updates**: None (read-only)
- **Audit events**: Report access logged
- **Financial risk**: Payroll accuracy

## Code traceability (REQUIRED)
- **Primary UI class(es)**: 
  - `AttendanceReportView` → `report/AttendanceReportView.java`
  - `AttendanceHistoryExplorer` → `bo/ui/explorer/AttendanceHistoryExplorer.java`
- **Entry action(s)**: `AttendanceHistoryAction` → `bo/actions/AttendanceHistoryAction.java`
- **Workflow/service enforcement**: AttendanceHistory queries
- **Messages/labels**: Column headers

## MagiDesk parity notes
- **What exists today**: No attendance tracking
- **What differs / missing**: Entire attendance report

## Porting strategy (PLAN ONLY)
- **Backend requirements**: AttendanceHistory entity; clock in/out queries
- **API/DTO requirements**: GET /reports/attendance
- **UI requirements**: AttendanceReportView
- **Constraints for implementers**: Must handle open shifts; overtime calculation

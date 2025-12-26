# Feature: Clock In/Out Action (F-0009)

## Classification
- **Parity classification**: PARITY WITH MODERNIZATION
- **MagiDesk status**: MISSING

## Problem / Why this exists (grounded)
- **Operational need**: Track employee work hours for payroll, attendance reports, labor cost analysis. Staff must clock in to work and clock out when done.
- **Evidence**: `ClockInOutAction.java` - toggles employee attendance state; creates AttendanceHistory records; affects employee availability for assignments.

## User-facing surfaces
- **Surface type**: Action (button/menu item)
- **UI entry points**: Login screen → Clock In/Out; Switchboard
- **Exit paths**: Immediate action (no dialog or confirmation)

## Preconditions & protections
- **User/role/permission checks**: User must be authenticated
- **State checks**: Checks current clock status to determine in/out
- **Manager override**: Not required; but early clock out may need approval

## Step-by-step behavior (forensic)
1. User authenticated (usually at login)
2. User clicks Clock In/Out button
3. System checks current attendance status
4. If not clocked in:
   - Creates AttendanceHistory with clock-in time
   - User now available for assignments
5. If already clocked in:
   - Updates AttendanceHistory with clock-out time
   - Calculates hours worked
6. Confirmation message shown
7. Attendance updated for reporting

## Edge cases & failure paths
- **Already clocked in/out**: Shows current status
- **Missed clock out**: Previous day left open (needs manager fix)
- **Clock out with open tickets**: May require transfer first
- **Overtime**: Flagged for payroll

## Data / audit / financial impact
- **Writes/updates**: AttendanceHistory entity with clockInTime, clockOutTime
- **Audit events**: Clock events logged
- **Financial risk**: Incorrect hours affect payroll; buddy punching

## Code traceability (REQUIRED)
- **Primary UI class(es)**: Action-based, no dedicated dialog
- **Entry action(s)**: `ClockInOutAction` → `actions/ClockInOutAction.java`
- **Workflow/service enforcement**: AttendanceDAO; UserDAO
- **Messages/labels**: Clock in/out status messages

## Uncertainties (STOP; do not guess)
- PIN/password re-verification for clock in
- Location/terminal tracking for punches
- Break tracking

## MagiDesk parity notes
- **What exists today**: No attendance/time tracking
- **What differs / missing**: Clock in/out action; AttendanceHistory entity; reports

## Porting strategy (PLAN ONLY)
- **Backend requirements**: 
  - AttendanceHistory entity with user, clockIn, clockOut
  - ClockInCommand, ClockOutCommand
  - Attendance validation rules
- **API/DTO requirements**: POST /attendance/clock-in; POST /attendance/clock-out
- **UI requirements**: Clock In/Out button; status indicator
- **Constraints for implementers**: Time must be server-side; cannot edit after lock period

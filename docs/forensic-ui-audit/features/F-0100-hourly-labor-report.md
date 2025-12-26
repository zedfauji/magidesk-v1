# Feature: Hourly Labor Report (F-0100)

## Classification
- **Parity classification**: PARITY WITH MODERNIZATION
- **MagiDesk status**: MISSING

## Problem / Why this exists (grounded)
- **Operational need**: Track labor costs against sales by hour. Essential for scheduling optimization and labor cost control.
- **Evidence**: `HourlyLaborReportView.java` - labor hours and cost by time period; sales comparison.

## User-facing surfaces
- **Surface type**: Report view (in Back Office)
- **UI entry points**: BackOfficeWindow → Reports → Hourly Labor
- **Exit paths**: Close tab / Print / Export

## Preconditions & protections
- **User/role/permission checks**: Labor report permission (HR/manager)
- **State checks**: None
- **Manager override**: Not required

## Step-by-step behavior (forensic)
1. Open Hourly Labor Report
2. Select date (day)
3. Generate shows hourly breakdown:
   - Hour
   - Employees working
   - Total labor hours
   - Labor cost
   - Sales
   - Labor percentage
4. Highlights high labor % hours
5. Print or export

## Edge cases & failure paths
- **No attendance data**: Empty
- **Missing wage rates**: May show hours only

## Data / audit / financial impact
- **Writes/updates**: None (read-only)
- **Audit events**: Report access logged
- **Financial risk**: Labor cost management

## Code traceability (REQUIRED)
- **Primary UI class(es)**: `HourlyLaborReportView` → `report/HourlyLaborReportView.java`
- **Entry action(s)**: `HourlyLaborReportAction` → (path)
- **Workflow/service enforcement**: Attendance + wage aggregation
- **Messages/labels**: Column headers

## MagiDesk parity notes
- **What exists today**: No labor reporting
- **What differs / missing**: Entire labor report

## Porting strategy (PLAN ONLY)
- **Backend requirements**: Join attendance with employee wage data
- **API/DTO requirements**: GET /reports/hourly-labor
- **UI requirements**: HourlyLaborReportView
- **Constraints for implementers**: Requires accurate clock-in/out data

# Feature: About Dialog (F-0130)

## Classification
- **Parity classification**: PARITY REQUIRED
- **MagiDesk status**: PARTIAL (version display may exist)

## Problem / Why this exists (grounded)
- **Operational need**: Display system version, license info, support contacts. Essential for troubleshooting.
- **Evidence**: `AboutDialog.java` - version info; license; credits.

## User-facing surfaces
- **Surface type**: Modal dialog
- **UI entry points**: BackOfficeWindow → Help → About
- **Exit paths**: Close

## Preconditions & protections
- **User/role/permission checks**: None
- **State checks**: None
- **Manager override**: Not required

## Step-by-step behavior (forensic)
1. User clicks About menu
2. AboutDialog displays:
   - Application name
   - Version number
   - Build date
   - License holder
   - Database version
   - Support contact
   - Credits/acknowledgments
3. User reviews information
4. Closes dialog

## Edge cases & failure paths
- **License expired**: Warning displayed
- **Database version mismatch**: Warning

## Data / audit / financial impact
- **Writes/updates**: None
- **Audit events**: None
- **Financial risk**: None

## Code traceability (REQUIRED)
- **Primary UI class(es)**: `AboutDialog` → `ui/dialog/AboutDialog.java`
- **Entry action(s)**: Help menu
- **Workflow/service enforcement**: Version info lookup
- **Messages/labels**: Version strings

## MagiDesk parity notes
- **What exists today**: Version info somewhere
- **What differs / missing**: Formal About dialog

## Porting strategy (PLAN ONLY)
- **Backend requirements**: Version info assembly
- **API/DTO requirements**: GET /system/info
- **UI requirements**: AboutDialog
- **Constraints for implementers**: Keep updated with builds

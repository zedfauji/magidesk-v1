# Feature: Database Backup Dialog (F-0128)

## Classification
- **Parity classification**: PARITY WITH MODERNIZATION
- **MagiDesk status**: MISSING

## Problem / Why this exists (grounded)
- **Operational need**: Regular database backups essential for disaster recovery. Built-in backup tool for non-technical staff.
- **Evidence**: `DatabaseBackup.java` + related - backup database to file; scheduled/manual.

## User-facing surfaces
- **Surface type**: Modal dialog
- **UI entry points**: BackOfficeWindow → Tools → Database Backup
- **Exit paths**: Backup complete / Cancel

## Preconditions & protections
- **User/role/permission checks**: Admin permission
- **State checks**: Database accessible
- **Manager override**: Permission required

## Step-by-step behavior (forensic)
1. Open Database Backup
2. Dialog shows:
   - Last backup date/time
   - Backup destination
   - Backup now button
   - Schedule settings
3. User initiates backup
4. Progress indicator
5. Backup file created
6. Success confirmation

## Edge cases & failure paths
- **Disk full**: Error with message
- **Database locked**: Wait or error
- **Backup file corrupt**: Verify option

## Data / audit / financial impact
- **Writes/updates**: Backup file created
- **Audit events**: Backup logged
- **Financial risk**: Data loss prevention

## Code traceability (REQUIRED)
- **Primary UI class(es)**: `DatabaseBackupDialog` → utilities
- **Entry action(s)**: Tools menu
- **Workflow/service enforcement**: pg_dump or equivalent
- **Messages/labels**: Backup prompts

## MagiDesk parity notes
- **What exists today**: No built-in backup UI
- **What differs / missing**: Database backup dialog

## Porting strategy (PLAN ONLY)
- **Backend requirements**: Database backup command
- **API/DTO requirements**: POST /admin/backup
- **UI requirements**: Backup dialog with progress
- **Constraints for implementers**: Must be non-blocking; verify backup

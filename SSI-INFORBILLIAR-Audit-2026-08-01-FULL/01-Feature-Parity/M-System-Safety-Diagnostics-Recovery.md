# Category M: System Safety, Diagnostics & Recovery

## M.1 Error surfacing to operator

**Feature ID:** M.1  
**Feature Name:** Error surfacing to operator  
**Status:** PARTIALLY IMPLEMENTED

**Backend Evidence:**
- Database tables / columns: N/A
- Domain entities: Exception handling
- Services: Try-catch with logging
- APIs / handlers: Error responses
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): Some error dialogs
- ViewModels: Error handling in some VMs
- Navigation path: On error occurrence
- User-visible workflow: Error messages shown

**Notes:**
- Basic error handling exists
- Not consistent across all operations

**Risks / Gaps:**
- Some errors may be silent
- User confusion on failures

**Recommendation:** EXTEND - Standardize error display

---

## M.2 Crash handling and recovery

**Feature ID:** M.2  
**Feature Name:** Crash handling and recovery  
**Status:** PARTIALLY IMPLEMENTED

**Backend Evidence:**
- Database tables / columns: N/A
- Domain entities: N/A
- Services: Global exception handler
- APIs / handlers: UnhandledException event
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): App crash dialog (Windows default)
- ViewModels: N/A
- Navigation path: On crash
- User-visible workflow: App restarts

**Notes:**
- Windows handles crash reporting
- App restarts from clean state

**Risks / Gaps:**
- In-progress orders may be lost
- No crash dump collection

**Recommendation:** EXTEND - Add crash reporting service

---

## M.3 Diagnostics export for support

**Feature ID:** M.3  
**Feature Name:** Diagnostics export for support  
**Status:** NOT IMPLEMENTED

**Backend Evidence:**
- Database tables / columns: N/A
- Domain entities: N/A
- Services: NO EVIDENCE FOUND
- APIs / handlers: NO EVIDENCE FOUND
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): NO EVIDENCE FOUND
- ViewModels: NO EVIDENCE FOUND
- Navigation path: NO EVIDENCE FOUND
- User-visible workflow: NO EVIDENCE FOUND

**Notes:**
- Would package logs, config for support
- Essential for remote troubleshooting

**Risks / Gaps:**
- Support cannot diagnose remotely

**Recommendation:** IMPLEMENT - Add diagnostic bundle export

---

## M.4 Structured logging

**Feature ID:** M.4  
**Feature Name:** Structured logging  
**Status:** FULLY IMPLEMENTED

**Backend Evidence:**
- Database tables / columns: Log files
- Domain entities: N/A
- Services: ILogger throughout
- APIs / handlers: Logging in handlers
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): N/A
- ViewModels: Logging in VMs
- Navigation path: N/A
- User-visible workflow: Logs generated

**Notes:**
- Microsoft.Extensions.Logging used
- Structured log output
- Debug.WriteLine for development

**Risks / Gaps:**
- None significant

**Recommendation:** COMPLETE - Logging framework in place

---

## M.5 Backup configuration

**Feature ID:** M.5  
**Feature Name:** Backup configuration  
**Status:** PARTIALLY IMPLEMENTED

**Backend Evidence:**
- Database tables / columns: Backup settings possible
- Domain entities: NO EVIDENCE FOUND
- Services: NO EVIDENCE FOUND for scheduled backup
- APIs / handlers: NO EVIDENCE FOUND
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): `SystemConfigPage.xaml` may have backup settings
- ViewModels: Basic backup options
- Navigation path: Settings → Backup
- User-visible workflow: Configure backup

**Notes:**
- UI mentions backup
- Implementation unclear

**Risks / Gaps:**
- May not actually schedule backups

**Recommendation:** VERIFY - Confirm backup functionality

---

## M.6 Manual backup

**Feature ID:** M.6  
**Feature Name:** Manual backup  
**Status:** PARTIALLY IMPLEMENTED

**Backend Evidence:**
- Database tables / columns: Database file
- Domain entities: N/A
- Services: Backup command may exist
- APIs / handlers: NO EVIDENCE FOUND
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): Backup button possible
- ViewModels: Backup command
- Navigation path: Settings → Backup Now
- User-visible workflow: Click to backup

**Notes:**
- Manual trigger for backup
- File copy or export

**Risks / Gaps:**
- May not work correctly

**Recommendation:** VERIFY - Test manual backup

---

## M.7 Automatic backup

**Feature ID:** M.7  
**Feature Name:** Automatic backup  
**Status:** NOT IMPLEMENTED

**Backend Evidence:**
- Database tables / columns: NO EVIDENCE FOUND
- Domain entities: NO EVIDENCE FOUND
- Services: NO EVIDENCE FOUND for scheduler
- APIs / handlers: NO EVIDENCE FOUND
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): NO EVIDENCE FOUND
- ViewModels: NO EVIDENCE FOUND
- Navigation path: NO EVIDENCE FOUND
- User-visible workflow: NO EVIDENCE FOUND

**Notes:**
- Would run on schedule
- Daily, weekly, etc.

**Risks / Gaps:**
- No automated protection

**Recommendation:** IMPLEMENT - Add scheduled backup

---

## M.8 Restore from backup

**Feature ID:** M.8  
**Feature Name:** Restore from backup  
**Status:** NOT IMPLEMENTED

**Backend Evidence:**
- Database tables / columns: N/A
- Domain entities: N/A
- Services: NO EVIDENCE FOUND
- APIs / handlers: NO EVIDENCE FOUND
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): NO EVIDENCE FOUND
- ViewModels: NO EVIDENCE FOUND
- Navigation path: NO EVIDENCE FOUND
- User-visible workflow: NO EVIDENCE FOUND

**Notes:**
- Restore database from file
- Critical for disaster recovery

**Risks / Gaps:**
- Cannot recover from backup

**Recommendation:** IMPLEMENT - Add restore capability

---

## M.9 Migration safety checks

**Feature ID:** M.9  
**Feature Name:** Migration safety checks  
**Status:** PARTIALLY IMPLEMENTED

**Backend Evidence:**
- Database tables / columns: __EFMigrationsHistory
- Domain entities: Migrations
- Services: EF migration on startup
- APIs / handlers: N/A
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): NO EVIDENCE FOUND for explicit check
- ViewModels: NO EVIDENCE FOUND
- Navigation path: Auto on startup
- User-visible workflow: Migrations run

**Notes:**
- EF checks pending migrations
- Applies automatically

**Risks / Gaps:**
- No warning before migration

**Recommendation:** EXTEND - Prompt before migration

---

## M.10 Rollback after failed upgrade

**Feature ID:** M.10  
**Feature Name:** Rollback after failed upgrade  
**Status:** NOT IMPLEMENTED

**Backend Evidence:**
- Database tables / columns: NO EVIDENCE FOUND
- Domain entities: NO EVIDENCE FOUND
- Services: NO EVIDENCE FOUND
- APIs / handlers: NO EVIDENCE FOUND
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): NO EVIDENCE FOUND
- ViewModels: NO EVIDENCE FOUND
- Navigation path: NO EVIDENCE FOUND
- User-visible workflow: NO EVIDENCE FOUND

**Notes:**
- Would revert to previous version
- Undo failed migrations

**Risks / Gaps:**
- Failed upgrades leave broken state

**Recommendation:** IMPLEMENT - Add rollback capability

---

## M.11 Data corruption detection

**Feature ID:** M.11  
**Feature Name:** Data corruption detection  
**Status:** NOT IMPLEMENTED

**Backend Evidence:**
- Database tables / columns: NO EVIDENCE FOUND
- Domain entities: NO EVIDENCE FOUND
- Services: NO EVIDENCE FOUND
- APIs / handlers: NO EVIDENCE FOUND
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): NO EVIDENCE FOUND
- ViewModels: NO EVIDENCE FOUND
- Navigation path: NO EVIDENCE FOUND
- User-visible workflow: NO EVIDENCE FOUND

**Notes:**
- Checksums, validation
- Detect database issues

**Risks / Gaps:**
- Corruption goes unnoticed

**Recommendation:** CONSIDER - Add integrity checks

---

## Category M COMPLETE

- Features audited: 11
- Fully implemented: 1
- Partially implemented: 5
- Not implemented: 5

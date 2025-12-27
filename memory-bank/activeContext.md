# Active Context

## Current Work Focus
- **Phase 3 Wrap-Up & Phase 4 Prep** 🚀
  - **Phase 3 Complete**: All scheduled features for Phase 3 (Complex Menus, Reporting Engine, System Services) are implemented and verified.
  - **Phase 4**: Frontend Integration (WinUI) - Connecting the new backend (Reports, Settle, Modifiers) to the existing UI shell.
  - **Immediate Goal**: Synchronize backend progress with checking into the main branch or preparing for UI wiring.

## Guardrails (See .clinerules for full list)
- **FORENSIC PARITY**: `docs/forensic-ui-audit/features/F-XXXX.md` is the Source of Truth.
- **FINANCIAL INTEGRITY**: `decimal` only. Domain-layer naming only.
- **ARCHITECTURE**: Clean Architecture (Domain -> Application -> Infrastructure -> Presentation).
- **MEMORY BANK**: Keep this directory updated.

## Recent Changes
- ✅ **System Services**: `BackupService` [F-0128] and `FiscalLogging` [F-0025] implemented.
- ✅ **Reporting Engine**: 7 Operational Reports (Sales, Labor, Delivery, Productivity etc.) implemented via CQRS.
- ✅ **Complex Menus**: Completed Pizza fractional modifiers and Combo pricing logic.
- ✅ **Backend Forensic Audit**: Switched focus to Implementation after initial audit.

## Active Documents
- `c:/Users/giris/.gemini/antigravity/brain/de13f5fa-ac56-468c-b54a-d49fbb90981f/task.md` (Detailed Task List)
- `.clinerules` (Rules)
- `Magidesk.Application/Services/Reports/` (Recent Report Handlers)
- `Magidesk.Infrastructure/Services/PostgresBackupService.cs` (Recent Backup Service)

## Next Steps
1. **Planning Phase 4**: Define UI tasks for Report screens (Sales, Labor, Delivery).
2. **Wire Up**: Connect `ReportsController` to WinUI Views.
3. **Verify End-to-End**: Run full cycle tests from UI → API → DB → Report.

## Active Decisions
- **Reports**: Implemented as raw SQL/LINQ aggregations in Repositories for performance (CQRS read model optimization).
- **Backup**: Uses `pg_dump` external process, requires `appsettings.json` configuration for paths.
- **Audit**: Printing is now a partial "Fiscal" action, requiring `AuditEvent` logging.

## Important Patterns
- Clean Architecture: Domain → Application → Infrastructure → Presentation
- Repository pattern for data access
- CQRS for commands and queries
- Domain events for audit trail

## Workspace / MCP Filesystem Path (Source of Truth)
- **MCP filesystem root**: `/projects`
- **Repository path (use for MCP file tools)**: `/projects/Code/Redesign-POS/Windows Based POS/Magidesk`

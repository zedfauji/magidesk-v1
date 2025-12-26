# Backend Forensic Audit: F-0128 Database Backup Dialog

## Feature Context
- **Feature**: Database Backup Dialog
- **Trace from**: `F-0128-database-backup-dialog.md`
- **Reference**: `DatabaseConfiguration.java`

## Backend Invariants
1.  **Consistency**: Should lock DB or use WAL/Snapshot for consistent backup.
2.  **Action**: `pg_dump` or equivalent trigger.

## Forbidden States
-   **None**.

## Audit Requirements
-   **Event**: `DB_BACKUP_INITIATED`.

## Concurrency Semantics
-   **Block**: May block Writes.

## MagiDesk Backend Parity
-   **Service**: ⚠️ `BackupService` needed.

## Alignment Strategy
1.  **Implement** Backup endpoint.

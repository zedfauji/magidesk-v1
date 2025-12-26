# Backend Forensic Audit: F-0103 Journal Report View

## Feature Context
- **Feature**: Journal Report View
- **Trace from**: `F-0103-journal-report-view.md`
- **Reference**: `JournalReport.java`

## Backend Invariants
1.  **Immutability**: The Journal (Audit Log) is Write-Once, Read-Many.
2.  **Scope**: Every sensitive action (Void, Refund, Drawer Open) MUST appear here.

## Forbidden States
-   **Deletion**: Deleting entries from the Journal is strictly forbidden (Anti-Forensic).

## Audit Requirements
-   **Event**: `JOURNAL_VIEWED` (Meta-audit).

## Concurrency Semantics
-   **None**.

## MagiDesk Backend Parity
-   **Query**: ⚠️ `GetAuditLogQuery` exists but needs full coverage.

## Alignment Strategy
1.  **Ensure** all `Severity > INFO` events land in Journal table.

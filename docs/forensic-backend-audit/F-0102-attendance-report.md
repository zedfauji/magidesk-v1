# Backend Forensic Audit: F-0102 Attendance Report View

## Feature Context
- **Feature**: Attendance Report View
- **Trace from**: `F-0102-attendance-report-view.md`
- **Reference**: `AttendanceReport.java`

## Backend Invariants
1.  **Redundancy**: Similar to Hourly Labor but User-centric.
2.  **Fields**: Clock In, Clock Out, Duration, Break Time.

## Forbidden States
-   **Open Shift**: Reporting on currently open shifts might show duration="Running".

## Audit Requirements
-   **Event**: None.

## Concurrency Semantics
-   **None**.

## MagiDesk Backend Parity
-   **Query**: ✅ Exists.

## Alignment Strategy
1.  **None**.

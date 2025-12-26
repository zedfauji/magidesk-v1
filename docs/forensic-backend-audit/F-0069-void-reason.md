# Backend Forensic Audit: F-0069 Void Reason Selection Dialog

## Feature Context
- **Feature**: Void Reason Selection Dialog
- **Trace from**: `F-0069-void-reason-selection-dialog.md`
- **Reference**: `VoidReasonDialog.java`

## Backend Invariants
1.  **Mandatory**: Void operations MUST have a reason.
2.  **Selection**: Pick from standardized list (e.g., "Customer Changed Mind", "Error").

## Forbidden States
-   **Empty Reason**: Backend must block Void if reason is null/empty.

## Audit Requirements
-   **Event**: `VOID_REASON_CAPTURED`.

## Concurrency Semantics
-   **None**.

## MagiDesk Backend Parity
-   **Model**: ✅ `VoidReason` entity exists.

## Alignment Strategy
1.  **None**.

# Backend Forensic Audit: F-0058 Kitchen Print Status Dialog

## Feature Context
- **Feature**: Kitchen Print Status Dialog
- **Trace from**: `F-0058-kitchen-print-status-dialog.md`
- **Reference**: `KitchenDisplayView.java` (Status Monitoring)

## Backend Invariants
1.  **Source of Truth**: The `KitchenOrder` entity stores the status (`Printed`, `Failed`, `Queued`).
2.  **Retry**: Allows re-sending jobs to printers.

## Forbidden States
-   **False Positive**: Marking as "Printed" when the printer is offline/errored.

## Audit Requirements
-   **Event**: `KITCHEN_PRINT_FAILED` / `KITCHEN_PRINT_RETRY`.

## Concurrency Semantics
-   **None**.

## MagiDesk Backend Parity
-   **Model**: ⚠️ `KitchenOrder` tracking missing.

## Alignment Strategy
1.  **Implement** `KitchenOrder` aggregate that tracks print attempts separate from `TicketItem`.

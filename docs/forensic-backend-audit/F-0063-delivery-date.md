# Backend Forensic Audit: F-0063 Delivery Date Selection Dialog

## Feature Context
- **Feature**: Delivery Date Selection Dialog
- **Trace from**: `F-0063-delivery-date-selection-dialog.md`
- **Reference**: `DeliveryDateSelectionDialog.java`

## Backend Invariants
1.  **Scheduling**: Sets `DeliveryDate` and `Status = PRE_ORDER` (or equivalent).
2.  **Production Timing**: Must fire to kitchen X minutes before Delivery Date (Auto-Send).

## Forbidden States
-   **Past Date**: Cannot schedule delivery for the past.

## Audit Requirements
-   **Event**: `DELIVERY_SCHEDULED`
    -   Payload: TicketId, DueTime.
    -   Severity: INFO.

## Concurrency Semantics
-   **None**.

## MagiDesk Backend Parity
-   **Logic**: ⚠️ Auto-fire logic needed.

## Alignment Strategy
1.  **Implement** `PreOrderReleaseService` background job.

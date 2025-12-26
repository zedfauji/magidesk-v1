# Backend Forensic Audit: F-0029 Misc Ticket Item Dialog

## Feature Context
- **Feature**: Misc Ticket Item Dialog
- **Trace from**: `F-0029-misc-ticket-item-dialog.md`
- **Reference**: `MiscTicketItemDialog.java`

## Backend Invariants
1.  **Complete Definition**: An ad-hoc item MUST have: Name, Price, Tax Group. It cannot be "Unknown Tax".
2.  **Reporting Category**: Must be assigned to a "Misc" or "General" category for Sales Reports.

## Forbidden States
-   **Anonymous Item**: Item with empty name/description.
-   **Negative Price**: Unless strictly controlled as a discount/coupon (which should use Discount workflow, not Misc Item).

## Audit Requirements
-   **Event**: `MISC_ITEM_ADDED`
    -   Payload: TicketId, Price, Description.
    -   Severity: INFO (Track for excessive use).

## Concurrency Semantics
-   **None**.

## MagiDesk Backend Parity
-   **Model**: ⚠️ Need `MiscItem` handling in `TicketItem`.

## Alignment Strategy
1.  **Add** `IsMisc` flag or `ManualPrice` property to `TicketItem`.

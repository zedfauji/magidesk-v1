# Backend Forensic Audit: F-0030 Ticket Fee Dialog

## Feature Context
- **Feature**: Ticket Fee Dialog
- **Trace from**: `F-0030-ticket-fee-dialog.md`
- **Reference**: `TicketFeeDialog.java`

## Backend Invariants
1.  **Type**: Fee implies a Surcharge (Fixed or %) added to the Subtotal.
2.  **Taxability**: Fees can be taxable (e.g., Service Charge) or non-taxable. Backend MUST calculate tax on the fee if configured.
3.  **Persistence**: Fee is a top-level collection on Ticket, separate from Items.

## Forbidden States
-   **Hidden Fee**: A fee that affects Total but isn't listed in the breakdown.

## Audit Requirements
-   **Event**: `FEE_ADDED`
    -   Payload: TicketId, Amount/Percentage, Type.
    -   Severity: INFO.

## Concurrency Semantics
-   **None**.

## MagiDesk Backend Parity
-   **Model**: ✅ `TicketAdjustment` or `ServiceCharge` entity.

## Alignment Strategy
1.  **Ensure** `TaxService` includes Fees in the tax base if `Fee.IsTaxable` is true.

# Backend Forensic Audit: F-0061 Customer Selection Dialog

## Feature Context
- **Feature**: Customer Selection Dialog
- **Trace from**: `F-0061-customer-selection-dialog.md`
- **Reference**: `CustomerSelectionDialog.java`

## Backend Invariants
1.  **Association**: Assigning a customer binds the Ticket to the `CustomerId`.
2.  **Loyalty**: Often triggers a check for Loyalty Points or available Coupons.
3.  **Delivery**: If Order Type = Delivery, Customer is Mandatory.

## Forbidden States
-   **Anonymous Delivery**: Saving a Delivery Ticket without an assigned Customer/Address.

## Audit Requirements
-   **Event**: `CUSTOMER_ASSIGNED`
    -   Payload: TicketId, CustomerId.
    -   Severity: INFO.

## Concurrency Semantics
-   **None**.

## MagiDesk Backend Parity
-   **Service**: ✅ `CustomerService` exists.

## Alignment Strategy
1.  **Enforce** Mandatory Customer for specific `OrderTypes`.

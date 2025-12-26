# Backend Forensic Audit: F-0062 Customer Editor Dialog

## Feature Context
- **Feature**: Customer Editor Dialog
- **Trace from**: `F-0062-customer-editor-dialog.md`
- **Reference**: `CustomerForm.java`

## Backend Invariants
1.  **Uniqueness**: Phone Number acts as the primary key/unique index in most POS systems.
2.  **Validation**: Name and Phone are mandatory. Address mandatory if Delivery.

## Forbidden States
-   **Duplicate Phone**: Creating a second customer record with the same phone number (fragmenting history).

## Audit Requirements
-   **Event**: `CUSTOMER_CREATED` / `CUSTOMER_UPDATED`.

## Concurrency Semantics
-   **None**.

## MagiDesk Backend Parity
-   **Model**: ✅ `Customer` Entity.

## Alignment Strategy
1.  **Enforce** Unique Phone constraint in DB.

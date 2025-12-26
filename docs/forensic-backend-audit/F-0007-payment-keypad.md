# Backend Forensic Audit: F-0007 Payment Keypad

## Feature Context
- **Feature**: Payment Keypad and Tender Types
- **Trace from**: `F-0007-payment-keypad-and-tender-types.md`
- **Reference**: `PaymentView.java`, `PaymentType.java`

## Backend Invariants
1.  **Tender Validity**: The system MUST valid payment amounts against the Ticket Balance.
2.  **Overpayment/Change**:
    -   `Cash`: Allowed (Calculate Change).
    -   `Credit`: Forbidden (Amount cannot exceed balance, except for Tips which are separate).
    -   `Gift Card`: Forbidden (Cannot cash out gift card usually, depends on config).
3.  **Currency Precision**: All inputs handled as `decimal(18,4)` or equivalent.

## Forbidden States
-   **Negative Payment**: Cannot accept negative tender (unless it is a Refund transaction).
-   **Unknown Tender**: Payment record MUST link to a valid `PaymentType` definition.

## Audit Requirements
-   **Event**: `PAYMENT_RECEIVED`
    -   Payload: TicketId, Amount, TenderType, ChangeDue.
    -   Severity: INFO.

## Concurrency Semantics
-   **Double Pay Prevention**: Locking the ticket during payment processing to prevent race condition of two terminals paying same ticket.

## MagiDesk Backend Parity
-   **Tender Types**: ✅ TPH hierarchy implemented.
-   **Validation**: ✅ `PaymentDomainService` logic exists.
-   **Change Calculation**: ✅ Implemented.

## Alignment Strategy
1.  **Refine** validation rules for non-cash overpayments.
2.  **Hard-block** negative inputs at the API level.

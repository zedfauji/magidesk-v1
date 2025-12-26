# Feature: Gift Certificate Entry (F-0054)

## Classification
- **Parity classification**: PARITY REQUIRED
- **MagiDesk status**: MISSING

## Problem / Why this exists (grounded)
- **Operational need**: Restaurants sell and accept gift certificates as payment. Staff need to enter gift cert number and face value to apply as payment toward ticket.
- **Evidence**: `GiftCertDialog.java` - prompts for certificate number (up to 64 chars) and face value (defaults to $50). Validates non-empty number and positive face value.

## User-facing surfaces
- **Surface type**: Modal dialog
- **UI entry points**: PaymentView → Gift Certificate button (when configured)
- **Exit paths**: OK (applies gift cert) / Cancel (returns to payment view)

## Preconditions & protections
- **User/role/permission checks**: Standard payment permissions
- **State checks**: Ticket must exist with due amount > 0
- **Manager override**: Not required for standard gift cert redemption

## Step-by-step behavior (forensic)
1. User clicks Gift Certificate button in PaymentView
2. GiftCertDialog opens with text fields for cert number and face value
3. User enters gift certificate number (required, max 64 chars)
4. User enters face value (defaults to $50, must be > 0)
5. QWERTY keypad available for entry
6. On OK: Validates inputs, returns values to payment processor
7. If face value > due amount, difference can be refunded as cash back
8. Transaction created with gift cert details

## Edge cases & failure paths
- **Empty cert number**: Error message "Please enter gift certificate number"
- **Zero/negative face value**: Error message "Please enter face value"
- **Face value exceeds due amount**: Allowed, generates cash back transaction
- **Cert already redeemed**: Not validated in dialog (business logic in processor)

## Data / audit / financial impact
- **Writes/updates**: PosTransaction with giftCertNumber, giftCertFaceValue, giftCertPaidAmount, giftCertCashBackAmount
- **Audit events**: Payment transaction logged
- **Financial risk**: Duplicate redemption if cert tracking not implemented; cash back fraud

## Code traceability (REQUIRED)
- **Primary UI class(es)**: `GiftCertDialog` → `ui/views/payment/GiftCertDialog.java`
- **Entry action(s)**: Button in PaymentView
- **Workflow/service enforcement**: SettleTicketProcessor handles gift cert payment logic
- **Messages/labels**: `GiftCertDialog.0` through `GiftCertDialog.15`

## Uncertainties (STOP; do not guess)
- Gift certificate validity checking (appears to be external/store-side)
- Cash back maximum limits (not visible in dialog code)

## MagiDesk parity notes
- **What exists today**: No gift certificate payment type implemented
- **What differs / missing**: Entire gift certificate payment flow missing

## Porting strategy (PLAN ONLY)
- **Backend requirements**: GiftCertificatePayment entity extending Payment; gift cert validation service interface
- **API/DTO requirements**: CreateGiftCertPaymentCommand with certNumber, faceValue, ticketId
- **UI requirements**: GiftCertDialog with cert number input, face value input, QWERTY keyboard
- **Constraints for implementers**: Must track cash back separately; cannot exceed face value in payments

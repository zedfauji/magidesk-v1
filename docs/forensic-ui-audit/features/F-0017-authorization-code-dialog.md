# Feature: Authorization code entry (card-type selection + qwerty keypad)

## Classification
- **Parity classification**: PARITY REQUIRED
- **MagiDesk status**: UNCERTAIN (not assessed yet)

## Problem / Why this exists (grounded)
- **Operational need**: Support external-terminal or voice authorization workflows where the operator must key an authorization code and specify card type.
- **Evidence**: Dialog contains card brand toggles (visa/master/amex/discover) plus debit variants, and returns a `PaymentType` to `CardInputListener`.

## User-facing surfaces
- **Surface type**: Dialog (modal)
- **UI entry points**: From `SwipeCardDialog.openAuthorizationEntryDialog()`.
- **Controls observed**:
  - Card type selectors (credit + debit)
  - Authorization code text field
  - On-screen qwerty keypad
  - Submit / Cancel

## Preconditions & protections
- Defaults to Visa selected.

## Step-by-step behavior (forensic)
1. User selects card type via toggle buttons.
2. User enters authorization code.
3. On Submit:
   - Dialog disposes and calls `cardInputListener.cardInputted(this, getPaymentType())`.
4. `getPaymentType()` returns:
   - CREDIT_VISA / CREDIT_MASTER_CARD / CREDIT_AMEX / CREDIT_DISCOVERY
   - DEBIT_MASTER_CARD / DEBIT_VISA

## Code traceability (REQUIRED)
- **Primary UI**: `com.floreantpos.ui.views.payment.AuthorizationCodeDialog` → `/projects/Code/Redesign-POS/floreantpos/src/com/floreantpos/ui/views/payment/AuthorizationCodeDialog.java`

## Uncertainties (STOP; do not guess)
- **Validation**: No explicit validation of authorization code format is shown here.

## MagiDesk parity notes
- **What exists today**: Not yet assessed.

## Porting strategy (PLAN ONLY)
- **Backend requirements**: Payment type must support these card subtypes; settlement processors must accept an authorization code input.
- **UI requirements**: Card-type selection and authorization code entry with keypad.
- **Constraints for implementers**: Preserve the exact mapping from UI selection to `PaymentType`.

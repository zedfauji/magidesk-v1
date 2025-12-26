# Feature: Payment processing wait dialog

## Classification
- **Parity classification**: PARITY REQUIRED
- **MagiDesk status**: UNCERTAIN (not assessed yet)

## Problem / Why this exists (grounded)
- **Operational need**: Provide a non-blocking visual indicator that a payment is being processed, while preventing user from closing the dialog mid-operation.
- **Evidence**: Dialog is non-modal (`super(parent, false)`) and `DO_NOTHING_ON_CLOSE`.

## User-facing surfaces
- **Surface type**: Dialog (non-modal)
- **UI entry points**: Not proven here; likely invoked by payment processors.

## Preconditions & protections
- **Close protection**: `setDefaultCloseOperation(DO_NOTHING_ON_CLOSE)`.

## Step-by-step behavior (forensic)
- Shows a centered label with bold, large font using message keys `PaymentProcessWaitDialog.*`.
- Sizes to 500x400 and centers relative to parent.

## Code traceability (REQUIRED)
- **Primary UI**: `com.floreantpos.ui.views.payment.PaymentProcessWaitDialog` → `/projects/Code/Redesign-POS/floreantpos/src/com/floreantpos/ui/views/payment/PaymentProcessWaitDialog.java`

## Uncertainties (STOP; do not guess)
- **Where it is used**: Call sites not traced yet.

## MagiDesk parity notes
- **What exists today**: Not yet assessed.

## Porting strategy (PLAN ONLY)
- **UI requirements**: A non-dismissable “processing” surface.
- **Constraints for implementers**: Must be non-closeable until payment completion/cancel is fully handled.

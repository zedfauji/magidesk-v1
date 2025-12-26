# Feature: Credit Card Payment Button (F-0045)

## Classification
- **Parity classification**: PARITY REQUIRED
- **MagiDesk status**: PARTIAL (card payment may exist but integration differs)

## Problem / Why this exists (grounded)
- **Operational need**: Accept credit/debit card payments. Requires card reader integration or manual entry.
- **Evidence**: `PaymentView.java` - Card buttons (Visa, MC, etc.); swipe dialog; processor integration.

## User-facing surfaces
- **Surface type**: Button → swipe sequence
- **UI entry points**: PaymentView → Card button
- **Exit paths**: Payment authorized / Declined / Cancel

## Preconditions & protections
- **User/role/permission checks**: Card payment permission
- **State checks**: Card processor configured; ticket has balance
- **Manager override**: Not required

## Step-by-step behavior (forensic)
1. User in PaymentView
2. Clicks Card button (or specific card type)
3. SwipeCardDialog opens:
   - Wait for card swipe
   - OR Manual entry option
   - OR External terminal entry
4. Card data captured
5. Authorization request sent to processor
6. Wait for response (PaymentProcessWaitDialog)
7. If approved:
   - CardPayment transaction created
   - Auth code stored
   - Ticket balance reduced
8. If declined: Show error, retry options

## Edge cases & failure paths
- **Declined**: Show reason, offer retry or alternate
- **Timeout**: Show error, retry
- **Partial approval**: May approve less than requested
- **Pre-authorization**: Store for later capture

## Data / audit / financial impact
- **Writes/updates**: 
  - PosTransaction with card details (masked)
  - Auth code, reference number
  - Ticket.paidAmount
- **Audit events**: Card transaction logged
- **Financial risk**: Chargebacks; processor fees; PCI compliance

## Code traceability (REQUIRED)
- **Primary UI class(es)**: 
  - Button in `PaymentView`
  - `SwipeCardDialog` → `ui/views/payment/SwipeCardDialog.java`
  - `PaymentProcessWaitDialog` → `ui/views/payment/PaymentProcessWaitDialog.java`
- **Entry action(s)**: Card button click
- **Workflow/service enforcement**: CardProcessor integration
- **Messages/labels**: Card type buttons, auth prompts

## MagiDesk parity notes
- **What exists today**: Card payment type exists
- **What differs / missing**: Full swipe dialog; processor integration

## Porting strategy (PLAN ONLY)
- **Backend requirements**: CardPaymentCommand; processor service
- **API/DTO requirements**: POST /payments with type=CARD
- **UI requirements**: Card buttons; SwipeCardDialog
- **Constraints for implementers**: PCI compliance; no card data storage

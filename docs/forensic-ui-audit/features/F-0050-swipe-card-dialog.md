# Feature: Swipe Card Dialog (F-0050)

## Classification
- **Parity classification**: PARITY WITH MODERNIZATION
- **MagiDesk status**: MISSING

## Problem / Why this exists (grounded)
- **Operational need**: For card payments, must capture card data via swipe, insert, tap, or manual entry. Different input methods based on terminal type and card configuration.
- **Evidence**: `SwipeCardDialog.java` - captures card swipe; branches to manual entry or auth code entry; handles CardConfig settings for external terminals.

## User-facing surfaces
- **Surface type**: Modal dialog
- **UI entry points**: PaymentView → Card payment button; SettleTicketDialog
- **Exit paths**: Card captured / Manual Entry / External Auth / Cancel

## Preconditions & protections
- **User/role/permission checks**: Card payment permission
- **State checks**: Ticket exists with due amount; payment processor configured
- **Manager override**: Not required

## Step-by-step behavior (forensic)
1. User selects card payment
2. SwipeCardDialog opens
3. Depending on CardConfig:
   - **Integrated reader**: Wait for swipe/insert
   - **External terminal**: Option for auth code entry
   - **Manual entry**: Option to type card number
4. For swipe:
   - Raw track data captured
   - Card number extracted
   - Passed to card processor
5. For manual entry:
   - Opens ManualCardEntryDialog
   - Card number, expiry, CVV entered
6. For external terminal:
   - Opens AuthorizationCodeDialog
   - Auth code entered manually
7. Card data passed to payment processor

## Edge cases & failure paths
- **Bad swipe**: Error, retry
- **Card declined**: Error from processor
- **Timeout**: Cancel or retry
- **Unsupported card type**: Error message

## Data / audit / financial impact
- **Writes/updates**: PosTransaction with card details (masked)
- **Audit events**: Card transaction logged
- **Financial risk**: Card fraud; PCI compliance; chargebacks

## Code traceability (REQUIRED)
- **Primary UI class(es)**: 
  - `SwipeCardDialog` → `ui/views/payment/SwipeCardDialog.java`
  - `ManualCardEntryDialog` → `ui/views/payment/ManualCardEntryDialog.java`
- **Entry action(s)**: Called from PaymentView card buttons
- **Workflow/service enforcement**: CardProcessor, CardConfig, TerminalConfig
- **Messages/labels**: Swipe prompt, card type labels

## Uncertainties (STOP; do not guess)
- Specific card reader APIs (vendor dependent)
- EMV chip vs. swipe handling
- NFC/contactless support

## MagiDesk parity notes
- **What exists today**: No card payment UI
- **What differs / missing**: Entire card input workflow

## Porting strategy (PLAN ONLY)
- **Backend requirements**: 
  - CardPaymentCommand
  - Card processor integration service
  - PCI-compliant card handling
- **API/DTO requirements**: Card data never stored; passed directly to processor
- **UI requirements**: SwipeCardDialog with swipe detection, manual entry option
- **Constraints for implementers**: PCI compliance mandatory; no storage of full card data

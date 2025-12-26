# Feature: Cash Payment Button (F-0044)

## Classification
- **Parity classification**: PARITY REQUIRED
- **MagiDesk status**: EXISTS (cash payment implemented)

## Problem / Why this exists (grounded)
- **Operational need**: Accept cash as payment. Most common payment method. Must handle exact, over-tender with change.
- **Evidence**: `PaymentView.java` - Cash button; tender amount entry; change calculation.

## User-facing surfaces
- **Surface type**: Button + keypad
- **UI entry points**: PaymentView/SettleDialog → Cash button
- **Exit paths**: Payment accepted; ticket settled

## Preconditions & protections
- **User/role/permission checks**: Cash payment permission
- **State checks**: Drawer assigned; ticket has balance due
- **Manager override**: Not typically required

## Step-by-step behavior (forensic)
1. User in PaymentView with ticket
2. Due amount displayed
3. User clicks Cash (or enters amount and clicks Cash)
4. Tender amount validation:
   - Must be >= 0
   - If < due: Partial payment applied
   - If >= due: Ticket can be settled
5. Change calculated (tender - due)
6. Payment applied:
   - CashPayment transaction created
   - Terminal balance updated
   - Ticket balance reduced
7. If fully paid: Ticket closed
8. Change displayed for giving back

## Edge cases & failure paths
- **Exact amount**: No change
- **Over-tender**: Calculate change
- **Under-tender**: Partial payment or reject
- **Multi-currency**: Opens CurrencyTenderDialog

## Data / audit / financial impact
- **Writes/updates**: 
  - PosTransaction (cash)
  - Terminal.currentBalance increased
  - Ticket.paidAmount increased
- **Audit events**: Cash payment logged
- **Financial risk**: Incorrect change; drawer discrepancy

## Code traceability (REQUIRED)
- **Primary UI class(es)**: 
  - Button in `PaymentView` → `ui/views/payment/PaymentView.java`
- **Entry action(s)**: Cash button click
- **Workflow/service enforcement**: SettleTicketProcessor.doSettle()
- **Messages/labels**: Cash button, change display

## MagiDesk parity notes
- **What exists today**: Cash payment
- **What differs / missing**: Multi-currency; exact cash button layout

## Porting strategy (PLAN ONLY)
- **Backend requirements**: CashPaymentCommand; drawer balance update
- **API/DTO requirements**: POST /payments with type=CASH
- **UI requirements**: Cash button; amount entry; change display
- **Constraints for implementers**: Drawer required; change calculation

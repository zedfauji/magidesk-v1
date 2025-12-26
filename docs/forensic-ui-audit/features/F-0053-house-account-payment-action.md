# Feature: House Account Payment Action (F-0053)

## Classification
- **Parity classification**: DEFER (advanced feature)
- **MagiDesk status**: MISSING

## Problem / Why this exists (grounded)
- **Operational need**: Some customers (regulars, businesses) have credit accounts. Charge to account instead of immediate payment.
- **Evidence**: `HouseAccountPaymentAction.java` - charge to customer's house account; track balance.

## User-facing surfaces
- **Surface type**: Action + dialog
- **UI entry points**: PaymentView → House Account button
- **Exit paths**: Charge to account / Cancel

## Preconditions & protections
- **User/role/permission checks**: House account payment permission
- **State checks**: Customer assigned to ticket; customer has house account; credit limit not exceeded
- **Manager override**: May be required above limit

## Step-by-step behavior (forensic)
1. User in PaymentView with customer attached
2. Clicks House Account
3. Dialog shows:
   - Customer name
   - Current balance
   - Credit limit
   - Available credit
   - Charge amount
4. On charge:
   - HouseAccountTransaction created
   - Customer balance increased
   - Ticket marked paid
5. Balance appears on customer statement

## Edge cases & failure paths
- **No customer assigned**: Error, prompt to add
- **No house account**: Offer to create
- **Exceeds credit limit**: Prevented or manager override
- **Suspended account**: Prevented

## Data / audit / financial impact
- **Writes/updates**: 
  - HouseAccountTransaction
  - Customer.balance
  - Ticket marked paid
- **Audit events**: House account charge logged
- **Financial risk**: Bad debt; credit management

## Code traceability (REQUIRED)
- **Primary UI class(es)**: Action + customer balance view
- **Entry action(s)**: `HouseAccountPaymentAction` → (path)
- **Workflow/service enforcement**: Customer credit management
- **Messages/labels**: Account prompts

## MagiDesk parity notes
- **What exists today**: No house accounts
- **What differs / missing**: Entire house account system

## Porting strategy (PLAN ONLY)
- **Backend requirements**: 
  - HouseAccount with credit limit
  - HouseAccountTransaction
  - Customer balance tracking
- **API/DTO requirements**: POST /house-accounts/{id}/charge
- **UI requirements**: House Account dialog
- **Constraints for implementers**: Credit limit enforcement; statement generation

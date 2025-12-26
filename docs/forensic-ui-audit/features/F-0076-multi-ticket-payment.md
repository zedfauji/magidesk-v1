# Feature: Multi-Ticket Payment (F-0076)

## Classification
- **Parity classification**: PARITY WITH MODERNIZATION
- **MagiDesk status**: MISSING

## Problem / Why this exists (grounded)
- **Operational need**: Pay multiple tickets with one transaction. Corporate card for multiple guests.
- **Evidence**: Multi-ticket selection in payment flow - batch payment capability.

## User-facing surfaces
- **Surface type**: Action + dialog
- **UI entry points**: OpenTicketsDialog → Multi-select → Pay
- **Exit paths**: Tickets paid / Cancel

## Preconditions & protections
- **User/role/permission checks**: Payment permission
- **State checks**: Multiple open tickets
- **Manager override**: May be required

## Step-by-step behavior (forensic)
1. User selects multiple tickets
2. Initiates group payment
3. MultiPaymentDialog shows:
   - Selected tickets list
   - Individual totals
   - Combined total
4. Accept payment (one transaction)
5. On payment:
   - All tickets settled
   - Single payment split across tickets
6. Receipts for each ticket

## Edge cases & failure paths
- **Partial payment**: Apply to selected or error
- **Different owners**: May require override
- **Payment failure**: None settled

## Data / audit / financial impact
- **Writes/updates**: Multiple tickets settled; shared payment
- **Audit events**: Multi-pay logged
- **Financial risk**: Correct allocation per ticket

## Code traceability (REQUIRED)
- **Primary UI class(es)**: Multi-ticket payment dialog
- **Entry action(s)**: Multi-select payment
- **Workflow/service enforcement**: Batch settlement
- **Messages/labels**: Ticket list

## MagiDesk parity notes
- **What exists today**: No multi-ticket payment
- **What differs / missing**: Entire batch payment flow

## Porting strategy (PLAN ONLY)
- **Backend requirements**: BatchPaymentCommand
- **API/DTO requirements**: POST /payments/batch
- **UI requirements**: MultiPaymentDialog
- **Constraints for implementers**: Atomic - all or nothing

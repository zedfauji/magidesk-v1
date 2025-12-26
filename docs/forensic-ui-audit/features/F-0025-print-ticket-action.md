# Feature: Print Ticket Action (F-0025)

## Classification
- **Parity classification**: PARITY REQUIRED
- **MagiDesk status**: MISSING

## Problem / Why this exists (grounded)
- **Operational need**: Print ticket for customer (receipt) or kitchen (order). Multiple print scenarios: guest check, receipt, reprint, kitchen ticket.
- **Evidence**: `PrintTicketAction.java` + `ReceiptPrintService.java` - triggers print with options; routes to appropriate printer.

## User-facing surfaces
- **Surface type**: Action (no dialog)
- **UI entry points**: TicketView → Print button; automatic after payment
- **Exit paths**: Immediate action

## Preconditions & protections
- **User/role/permission checks**: Order permission
- **State checks**: Ticket must exist
- **Manager override**: Not required

## Step-by-step behavior (forensic)
1. User clicks Print or payment auto-triggers
2. System determines print type:
   - Kitchen ticket (new items)
   - Guest check (before payment)
   - Receipt (after payment, with tip line option)
3. Route to appropriate printer
4. Format ticket with restaurant info:
   - Restaurant header
   - Order details
   - Modifiers
   - Prices
   - Tax
   - Total
   - Payment info (if paid)
   - Tip line (if applicable)
5. Print executes
6. Print logged

## Edge cases & failure paths
- **Printer offline**: Error message
- **Paper out**: Error handling
- **Reprint**: Allowed, marked as duplicate

## Data / audit / financial impact
- **Writes/updates**: Print count may be tracked
- **Audit events**: Reprints logged
- **Financial risk**: Receipt fraud (reprinting for chargebacks)

## Code traceability (REQUIRED)
- **Primary UI class(es)**: Action only
- **Entry action(s)**: `PrintTicketAction` → `actions/PrintTicketAction.java`
- **Workflow/service enforcement**: `ReceiptPrintService` → `report/ReceiptPrintService.java`
- **Messages/labels**: Receipt template

## MagiDesk parity notes
- **What exists today**: No printing
- **What differs / missing**: Entire print system

## Porting strategy (PLAN ONLY)
- **Backend requirements**: Receipt template generation
- **API/DTO requirements**: Receipt data from ticket
- **UI requirements**: Print button; printer status
- **Constraints for implementers**: Support ESC/POS for receipt printers

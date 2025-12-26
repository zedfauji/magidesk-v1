# Feature: Reopen Ticket Action (F-0072)

## Classification
- **Parity classification**: PARITY WITH MODERNIZATION
- **MagiDesk status**: MISSING

## Problem / Why this exists (grounded)
- **Operational need**: Paid/closed tickets sometimes need to be reopened - customer returns, adds items, needs modification. Must be controlled.
- **Evidence**: `ReopenTicketAction.java` - reopens closed ticket; reverses payment; returns to order entry.

## User-facing surfaces
- **Surface type**: Action
- **UI entry points**: TicketExplorer → Reopen; Manager functions
- **Exit paths**: Ticket reopened / Cancel

## Preconditions & protections
- **User/role/permission checks**: Reopen ticket permission (manager-level)
- **State checks**: Ticket must be closed/paid; not voided
- **Manager override**: Permission typically IS manager-level

## Step-by-step behavior (forensic)
1. User selects closed ticket
2. Clicks Reopen
3. System validates:
   - Ticket is closed
   - User has permission
   - Within reopen window (if configured)
4. Confirmation dialog
5. On confirm:
   - Ticket status changed to OPEN
   - Payment(s) voided/reversed
   - Drawer balance adjusted
   - Ticket available for editing
6. Ticket opens in order view

## Edge cases & failure paths
- **Card payment reopen**: May require refund instead
- **Cash payment reopen**: Cash back to drawer
- **Reopen window expired**: Prevented
- **Till closed**: May complicate

## Data / audit / financial impact
- **Writes/updates**: 
  - Ticket.status = OPEN
  - Payment voided
  - Terminal balance adjusted
- **Audit events**: Reopen logged (important)
- **Financial risk**: Payment reversal; reconciliation

## Code traceability (REQUIRED)
- **Primary UI class(es)**: Action
- **Entry action(s)**: `ReopenTicketAction` → `actions/ReopenTicketAction.java`
- **Workflow/service enforcement**: Ticket state change; payment reversal
- **Messages/labels**: Reopen prompts

## MagiDesk parity notes
- **What exists today**: Basic ticket editing
- **What differs / missing**: Formal reopen workflow with payment reversal

## Porting strategy (PLAN ONLY)
- **Backend requirements**: 
  - ReopenTicketCommand
  - Payment reversal logic
  - Drawer adjustment
- **API/DTO requirements**: POST /tickets/{id}/reopen
- **UI requirements**: Reopen action; confirmation
- **Constraints for implementers**: Must reverse all financial impacts

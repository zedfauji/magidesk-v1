# Feature: Send to Kitchen Action (F-0027)

## Classification
- **Parity classification**: PARITY REQUIRED
- **MagiDesk status**: MISSING

## Problem / Why this exists (grounded)
- **Operational need**: After adding items, send order to kitchen for preparation. Kitchen receives printed ticket or KDS update.
- **Evidence**: `SendKitchenPrintAction.java` + related - sends new items to kitchen printer/display; marks items as sent.

## User-facing surfaces
- **Surface type**: Action (button)
- **UI entry points**: OrderView → Send/Fire button; automatic on hold
- **Exit paths**: Immediate action

## Preconditions & protections
- **User/role/permission checks**: Order entry permission
- **State checks**: Must have unsent items
- **Manager override**: Not required

## Step-by-step behavior (forensic)
1. User adds items to ticket
2. User clicks Send/Fire
3. System identifies unsent items
4. Routes items to appropriate kitchen:
   - By printer group assignment
   - Different printers for bar vs kitchen
5. Kitchen ticket printed or KDS updated:
   - Table/seat number
   - Server name
   - Items with modifiers
   - Special instructions
6. Items marked as "sent"
7. Ticket view updates (sent items typically gray)

## Edge cases & failure paths
- **No unsent items**: Button disabled or no-op
- **Printer offline**: Error message, retry option
- **KDS offline**: Queue for later

## Data / audit / financial impact
- **Writes/updates**: TicketItem.printed = true; kitchen status
- **Audit events**: Send time logged
- **Financial risk**: Low - operational

## Code traceability (REQUIRED)
- **Primary UI class(es)**: Action only
- **Entry action(s)**: `SendKitchenPrintAction` → `actions/SendKitchenPrintAction.java`
- **Workflow/service enforcement**: KitchenPrintService; PrinterRouting
- **Messages/labels**: Button label; kitchen ticket format

## MagiDesk parity notes
- **What exists today**: No kitchen printing
- **What differs / missing**: Entire kitchen send workflow

## Porting strategy (PLAN ONLY)
- **Backend requirements**: 
  - MarkAsSentCommand
  - Kitchen ticket generation
  - Printer routing by item group
- **API/DTO requirements**: POST /tickets/{id}/send-to-kitchen
- **UI requirements**: Send button; sent item indicator
- **Constraints for implementers**: Must track sent status per item

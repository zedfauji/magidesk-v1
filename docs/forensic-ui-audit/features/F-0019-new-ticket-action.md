# Feature: New Ticket Action (F-0019)

## Classification
- **Parity classification**: PARITY REQUIRED
- **MagiDesk status**: EXISTS (new order flow exists)

## Problem / Why this exists (grounded)
- **Operational need**: Start a new order/ticket. Entry point for all sales transactions.
- **Evidence**: `NewTicketAction.java` - creates new ticket; assigns to user/terminal; opens order entry.

## User-facing surfaces
- **Surface type**: Action
- **UI entry points**: Switchboard → New Order; Login → order type buttons
- **Exit paths**: OrderView opened with new ticket

## Preconditions & protections
- **User/role/permission checks**: Order entry permission
- **State checks**: Depends on order type (table required for dine-in)
- **Manager override**: Not required

## Step-by-step behavior (forensic)
1. User clicks New Order (or order type button)
2. Order type selection (if not pre-selected)
3. Type-specific requirements:
   - Dine-in: Table selection
   - Delivery: Customer selection
   - Takeout: Optional customer
4. New Ticket created:
   - Ticket ID generated
   - Owner = current user
   - Terminal = current terminal
   - Order type set
   - Status = OPEN
5. OrderView opens for item entry

## Edge cases & failure paths
- **No table available**: For dine-in, must select or wait
- **No drawer assigned**: May prompt for cash orders
- **Shift not open**: May prevent new orders

## Data / audit / financial impact
- **Writes/updates**: Ticket created in database
- **Audit events**: Ticket creation logged
- **Financial risk**: Low - just starts transaction

## Code traceability (REQUIRED)
- **Primary UI class(es)**: Action only
- **Entry action(s)**: `NewTicketAction` → `actions/NewTicketAction.java`
- **Workflow/service enforcement**: TicketService.createTicket()
- **Messages/labels**: None

## MagiDesk parity notes
- **What exists today**: New order creation
- **What differs / missing**: Order type flow; table pre-selection

## Porting strategy (PLAN ONLY)
- **Backend requirements**: CreateTicketCommand
- **API/DTO requirements**: POST /tickets
- **UI requirements**: Integrated with order type selection
- **Constraints for implementers**: Must assign owner, terminal, type

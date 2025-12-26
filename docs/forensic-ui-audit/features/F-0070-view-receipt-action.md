# Feature: View Receipt Action (F-0070)

## Classification
- **Parity classification**: PARITY REQUIRED
- **MagiDesk status**: MISSING

## Problem / Why this exists (grounded)
- **Operational need**: View/reprint receipt for closed ticket. Customer requests, record keeping.
- **Evidence**: Receipt viewing and reprinting in ticket explorer - view/print historical receipts.

## User-facing surfaces
- **Surface type**: Action + viewer
- **UI entry points**: TicketExplorer → View Receipt; Manager functions
- **Exit paths**: Close viewer / Print

## Preconditions & protections
- **User/role/permission checks**: Receipt access permission
- **State checks**: Ticket must exist
- **Manager override**: Not typically required

## Step-by-step behavior (forensic)
1. User locates ticket in explorer
2. Clicks View Receipt
3. ReceiptViewer opens:
   - Receipt preview (formatted)
   - Print button
   - Close button
4. User can:
   - View receipt details
   - Reprint if needed (marked as DUPLICATE)
5. Close viewer

## Edge cases & failure paths
- **No receipt data**: Generate from ticket
- **Printer not available**: Save as PDF option

## Data / audit / financial impact
- **Writes/updates**: None (read-only)
- **Audit events**: Reprints logged
- **Financial risk**: Reprint tracking for fraud prevention

## Code traceability (REQUIRED)
- **Primary UI class(es)**: Receipt viewer component
- **Entry action(s)**: TicketExplorer action
- **Workflow/service enforcement**: Receipt generation
- **Messages/labels**: Receipt labels

## MagiDesk parity notes
- **What exists today**: No receipt viewer
- **What differs / missing**: Receipt view/reprint functionality

## Porting strategy (PLAN ONLY)
- **Backend requirements**: Receipt template; historical data
- **API/DTO requirements**: GET /tickets/{id}/receipt
- **UI requirements**: ReceiptViewer component
- **Constraints for implementers**: Mark reprints as DUPLICATE

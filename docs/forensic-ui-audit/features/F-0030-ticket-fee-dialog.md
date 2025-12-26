# Feature: Ticket Fee Dialog (F-0030)

## Classification
- **Parity classification**: PARITY REQUIRED
- **MagiDesk status**: MISSING

## Problem / Why this exists (grounded)
- **Operational need**: Add additional fees to tickets (delivery fee, service charge, split check fee). Configurable per situation.
- **Evidence**: Fee handling in ticket calculation + `ServiceChargeDialog` - add fees to ticket total.

## User-facing surfaces
- **Surface type**: Modal dialog or automatic
- **UI entry points**: OrderView → Add Fee; automatic for order types
- **Exit paths**: Fee added / Cancel

## Preconditions & protections
- **User/role/permission checks**: Fee modification permission
- **State checks**: Ticket open
- **Manager override**: May be required to waive

## Step-by-step behavior (forensic)
1. Fee trigger:
   - Manual: User clicks Add Fee
   - Automatic: Order type requires fee (delivery)
2. Fee Selection/Entry:
   - Select fee type
   - Amount (fixed or calculated)
3. Fee added to ticket:
   - Separate line item or in totals
   - Tax calculation if applicable
4. Visible on ticket and receipt

## Edge cases & failure paths
- **Waive fee**: Requires permission
- **Multiple fees**: Allowed
- **Fee on partial**: Pro-rate if split

## Data / audit / financial impact
- **Writes/updates**: Ticket fee field or fee line item
- **Audit events**: Fee addition/waiver logged
- **Financial risk**: Revenue via fees; customer disputes

## Code traceability (REQUIRED)
- **Primary UI class(es)**: Fee dialog or inline
- **Entry action(s)**: Fee button or auto-trigger
- **Workflow/service enforcement**: Fee calculation
- **Messages/labels**: Fee type names

## MagiDesk parity notes
- **What exists today**: No fee system
- **What differs / missing**: Fee dialog and management

## Porting strategy (PLAN ONLY)
- **Backend requirements**: Fee entity; fee-ticket relationship
- **API/DTO requirements**: Fee in ticket DTO
- **UI requirements**: Fee selection dialog
- **Constraints for implementers**: Tax on fees configurable

# Feature: Beverage Quick Add (F-0033)

## Classification
- **Parity classification**: PARITY WITH MODERNIZATION
- **MagiDesk status**: MISSING

## Problem / Why this exists (grounded)
- **Operational need**: Bar service needs ultra-fast item entry. One-touch add for common drinks.
- **Evidence**: Beverage category handling + quick add patterns - beverages marked for fast entry.

## User-facing surfaces
- **Surface type**: Quick add panel
- **UI entry points**: OrderView → Beverage section
- **Exit paths**: Item added immediately

## Preconditions & protections
- **User/role/permission checks**: Order entry permission
- **State checks**: Ticket open
- **Manager override**: Not required

## Step-by-step behavior (forensic)
1. User in OrderView
2. Beverage quick panel shows:
   - Common drinks (pre-configured)
   - Single tap to add
   - No modifier dialog for simple beverages
3. User taps drink
4. Drink added immediately
5. Focus stays for rapid entry

## Edge cases & failure paths
- **Modifications needed**: Opens modifier dialog
- **86'd item**: Disabled or hidden
- **Count limit**: Bar may track pours

## Data / audit / financial impact
- **Writes/updates**: TicketItem added
- **Audit events**: Part of order
- **Financial risk**: Low - fast but accurate

## Code traceability (REQUIRED)
- **Primary UI class(es)**: Quick add panel
- **Entry action(s)**: Single tap
- **Workflow/service enforcement**: Item add
- **Messages/labels**: Drink names

## MagiDesk parity notes
- **What exists today**: Standard menu item add
- **What differs / missing**: Quick add mode for beverages

## Porting strategy (PLAN ONLY)
- **Backend requirements**: Beverage category flag
- **API/DTO requirements**: Standard order item
- **UI requirements**: Quick add panel
- **Constraints for implementers**: Skip modifier dialog for simple beverages

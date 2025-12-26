# Feature: Price Entry Dialog (F-0035)

## Classification
- **Parity classification**: PARITY REQUIRED
- **MagiDesk status**: MISSING

## Problem / Why this exists (grounded)
- **Operational need**: Some items need open pricing (market price, weigh items). Enter price at order time.
- **Evidence**: `PriceEntryDialog.java` - numeric keypad for price entry; item must be marked for open price.

## User-facing surfaces
- **Surface type**: Modal dialog
- **UI entry points**: Auto-opens when adding open-price item
- **Exit paths**: Enter price / Cancel

## Preconditions & protections
- **User/role/permission checks**: Order entry permission; may require manager for high prices
- **State checks**: Item must be marked as open-price
- **Manager override**: May be required above threshold

## Step-by-step behavior (forensic)
1. User adds menu item marked as open-price
2. PriceEntryDialog opens automatically
3. Shows:
   - Item name
   - Numeric keypad
   - Price field
   - Suggested price (if any)
4. User enters price
5. On OK: Item added with entered price
6. Price appears on ticket

## Edge cases & failure paths
- **Zero price**: May be allowed or prevented
- **Very high price**: Manager confirmation
- **Negative price**: Prevented

## Data / audit / financial impact
- **Writes/updates**: TicketItem with custom price
- **Audit events**: Open price logged
- **Financial risk**: Incorrect pricing; no consistency

## Code traceability (REQUIRED)
- **Primary UI class(es)**: `PriceEntryDialog` → `ui/dialog/PriceEntryDialog.java`
- **Entry action(s)**: Auto-triggered from item add
- **Workflow/service enforcement**: MenuItem.openPrice flag
- **Messages/labels**: Price prompt

## MagiDesk parity notes
- **What exists today**: Fixed pricing
- **What differs / missing**: Open price dialog

## Porting strategy (PLAN ONLY)
- **Backend requirements**: OrderLine with custom price
- **API/DTO requirements**: Price in order item creation
- **UI requirements**: PriceEntryDialog
- **Constraints for implementers**: Track when custom price used

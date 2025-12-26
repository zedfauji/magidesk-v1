# Feature: Menu Usage Report (F-0098)

## Classification
- **Parity classification**: PARITY WITH MODERNIZATION
- **MagiDesk status**: MISSING

## Problem / Why this exists (grounded)
- **Operational need**: Analyze which menu items sell most/least. Informs inventory, menu pricing, 86'd items, specials planning.
- **Evidence**: `MenuUsageReportView.java` + `MenuUsageReport.java` - item sales frequency, revenue contribution.

## User-facing surfaces
- **Surface type**: Report view (in Back Office)
- **UI entry points**: BackOfficeWindow → Reports → Menu Usage
- **Exit paths**: Close tab / Print / Export

## Preconditions & protections
- **User/role/permission checks**: Report viewing permission
- **State checks**: None
- **Manager override**: Not required

## Step-by-step behavior (forensic)
1. Open Menu Usage Report
2. Select date range
3. Optional filters:
   - Category
   - Order type
4. Generate shows for each item:
   - Item name
   - Category
   - Quantity sold
   - Percentage of total
   - Revenue
   - Average price (with modifiers)
5. Sort by quantity or revenue
6. Top/bottom performers highlighted
7. Print or export

## Edge cases & failure paths
- **New item**: Low numbers expected
- **Seasonal items**: Compare similar periods
- **Items with variants**: May aggregate or separate

## Data / audit / financial impact
- **Writes/updates**: None (read-only)
- **Audit events**: Report access logged
- **Financial risk**: Informs menu engineering decisions

## Code traceability (REQUIRED)
- **Primary UI class(es)**: 
  - `MenuUsageReportView` → `report/MenuUsageReportView.java`
  - `MenuUsageReport` → `report/MenuUsageReport.java`
- **Entry action(s)**: `MenuUsageReportAction` → `bo/actions/MenuUsageReportAction.java`
- **Workflow/service enforcement**: TicketItem aggregation by menu item
- **Messages/labels**: Column headers

## MagiDesk parity notes
- **What exists today**: No menu usage report
- **What differs / missing**: Entire report

## Porting strategy (PLAN ONLY)
- **Backend requirements**: Aggregate sales by menu item
- **API/DTO requirements**: GET /reports/menu-usage
- **UI requirements**: MenuUsageReportView
- **Constraints for implementers**: Handle modifiers; consider combo components

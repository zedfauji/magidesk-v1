# Feature: Sales Detail Report (F-0093)

## Classification
- **Parity classification**: PARITY REQUIRED
- **MagiDesk status**: MISSING

## Problem / Why this exists (grounded)
- **Operational need**: Detailed breakdown of all sales by item, category, time period. Essential for inventory planning, menu engineering, profitability analysis.
- **Evidence**: `SalesDetailReportView.java` + `SalesDetailedReport.java` - itemized sales with quantities, revenue by item.

## User-facing surfaces
- **Surface type**: Report view (in Back Office)
- **UI entry points**: BackOfficeWindow → Reports → Sales Detail
- **Exit paths**: Close tab / Print / Export

## Preconditions & protections
- **User/role/permission checks**: Report viewing permission
- **State checks**: None
- **Manager override**: Not required

## Step-by-step behavior (forensic)
1. Open Sales Detail Report
2. Select date range
3. Optional filters:
   - Category
   - Group
   - Specific item
4. Generate shows:
   - Item name
   - Quantity sold
   - Gross sales
   - Discounts
   - Net sales
   - Food cost (if tracked)
   - Profit margin
5. Grouped by category with subtotals
6. Grand totals at bottom
7. Print or export

## Edge cases & failure paths
- **Item with no sales**: Not shown or zero row
- **Voided items**: May be excluded or separate

## Data / audit / financial impact
- **Writes/updates**: None (read-only)
- **Audit events**: Report access logged
- **Financial risk**: Data used for pricing decisions

## Code traceability (REQUIRED)
- **Primary UI class(es)**: 
  - `SalesDetailReportView` → `report/SalesDetailReportView.java`
  - `SalesDetailedReport` → `report/SalesDetailedReport.java`
- **Entry action(s)**: `SalesDetailReportAction` → `bo/actions/SalesDetailReportAction.java`
- **Workflow/service enforcement**: TicketItem aggregation queries
- **Messages/labels**: Column headers

## MagiDesk parity notes
- **What exists today**: No sales detail report
- **What differs / missing**: Entire report

## Porting strategy (PLAN ONLY)
- **Backend requirements**: Aggregate ticket items by menu item
- **API/DTO requirements**: GET /reports/sales-detail
- **UI requirements**: SalesDetailReportView
- **Constraints for implementers**: Must handle modifiers; consider performance

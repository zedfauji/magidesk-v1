# Feature: Sales Summary Report (F-0092)

## Classification
- **Parity classification**: PARITY REQUIRED
- **MagiDesk status**: MISSING

## Problem / Why this exists (grounded)
- **Operational need**: Managers need to view sales performance summaries by day, week, month. Essential for business decisions, staffing, inventory planning.
- **Evidence**: `SalesSummaryReportView.java` + `SalesReportModel.java` - generates sales summary with totals, averages, trends; date range selection; grouping options.

## User-facing surfaces
- **Surface type**: Report view (in Back Office)
- **UI entry points**: BackOfficeWindow → Reports menu → Sales Report
- **Exit paths**: Close tab / Export / Print

## Preconditions & protections
- **User/role/permission checks**: Back office access; report viewing permission
- **State checks**: None (historical data query)
- **Manager override**: Not required

## Step-by-step behavior (forensic)
1. User opens Back Office
2. Navigates to Reports → Sales Report
3. Report view opens with:
   - Date range selector (from/to)
   - Grouping options (by day, week, server, order type)
4. User selects parameters and generates report
5. Report displays:
   - Total sales
   - Net sales
   - Tax collected
   - Discounts given
   - Number of tickets
   - Average ticket size
   - Breakdown by category
6. User can print or export report

## Edge cases & failure paths
- **No data in range**: Empty report with message
- **Large date range**: May be slow; consider pagination
- **Data inconsistency**: Report shows calculated vs stored totals

## Data / audit / financial impact
- **Writes/updates**: None (read-only report)
- **Audit events**: Report generation may be logged
- **Financial risk**: Incorrect reporting leads to bad decisions

## Code traceability (REQUIRED)
- **Primary UI class(es)**: 
  - `SalesSummaryReportView` → `report/SalesSummaryReportView.java`
  - `SalesReportModel` → `report/SalesReportModel.java`
- **Entry action(s)**: `SalesReportAction` → `bo/actions/SalesReportAction.java`
- **Workflow/service enforcement**: TicketDAO aggregation queries
- **Messages/labels**: Report headers, labels

## Uncertainties (STOP; do not guess)
- Export formats supported (PDF, Excel, CSV)
- Report template (Jasper Reports likely used)

## MagiDesk parity notes
- **What exists today**: No reports system
- **What differs / missing**: Entire reporting infrastructure

## Porting strategy (PLAN ONLY)
- **Backend requirements**: 
  - SalesReportQuery with date range, grouping
  - Aggregation queries on tickets/transactions
- **API/DTO requirements**: GET /reports/sales?from=&to=&groupBy=
- **UI requirements**: ReportViewer control; date pickers; export buttons
- **Constraints for implementers**: Reports must match financial records; consider caching for large datasets

# Feature: Drawer Pull Report (HTML preview + print)

## Classification
- **Parity classification**: PARITY REQUIRED
- **MagiDesk status**: UNCERTAIN (not assessed yet)

## Problem / Why this exists (grounded)
- **Operational need**: Produce an end-of-shift / cash-control reconciliation report summarizing sales, taxes, tenders, tips, payouts, refunds, opening balance, drawer bleed, and exceptions (voids/discounts), with a print action.
- **Evidence**: `createReport()` constructs detailed sections including gross receipts, receipt differential, tips differential, cash to deposit, multi-currency balances, void exceptions, and discount summaries.

## User-facing surfaces
- **Surface type**: Dialog (modal)
- **UI entry points**: Via `ManagerDialog.doShowDrawerPullReport()`.
- **Controls observed**: Print, Finish, scrollable HTML preview.

## Preconditions & protections
- **Terminal state refresh**: `terminal = Application.refreshAndGetTerminal()`.
- **Report build**: `DrawerpullReportService.buildDrawerPullReport()` assigns the report’s user from terminal assigned user.

## Step-by-step behavior (forensic)
1. `initialize()` refreshes terminal and builds `DrawerPullReport`.
2. Report preview is rendered into a `JEditorPane` as HTML.
3. `createReport()` outputs sections:
   - Header with terminal name and current date.
   - Sales summary (net sales, sales tax, delivery charge, total revenue, charged tips, gross receipts).
   - Tender breakdown (cash receipts, credit cards, debit cards, gift returns, gift cert change, cash back, refunds) → receipt differential.
   - Tips breakdown (charged tips, tips paid) → tips differential.
   - Cash section computing drawer accountable and cash to deposit using:
     - Cash receipts
     - Tips paid
     - Pay out amount/count
     - Cash back
     - Refund amount/count
     - Terminal opening balance
     - Drawer bleed amount/count
   - If multi-currency enabled: fetches `CashDrawer` by terminal and lists `CurrencyBalance` entries.
   - Void tickets exception table from `drawerPullReport.getVoidTickets()`.
   - Discount summary from report totals.
4. Print button calls `PosPrintService.printDrawerPullReport(drawerPullReport, terminal)`.

## Edge cases & failure paths
- Print failures:
  - `PosException` shows error message.
  - Other exceptions show a concatenated message `DrawerPullReportDialog.122` + exception message.

## Data / audit / financial impact
- **Financial impact**: This report is a reconciliation artifact; errors are high severity.
- **Audit impact**: Includes void/discount exception sections and aggregates used for cash accountability.

## Code traceability (REQUIRED)
- **Primary UI**: `com.floreantpos.ui.dialog.DrawerPullReportDialog` → `/projects/Code/Redesign-POS/floreantpos/src/com/floreantpos/ui/dialog/DrawerPullReportDialog.java`
- **Report builder**: `com.floreantpos.print.DrawerpullReportService.buildDrawerPullReport()`
- **Printer**: `com.floreantpos.print.PosPrintService.printDrawerPullReport(...)`
- **Multi-currency**: `TerminalConfig.isEnabledMultiCurrency()`, `CashDrawerDAO.findByTerminal(...)`

## Uncertainties (STOP; do not guess)
- **Exact formulas inside `DrawerPullReport`**: This UI displays computed fields; the computation logic is in report builder/model not proven here.

## MagiDesk parity notes
- **What exists today**: Not yet assessed.

## Porting strategy (PLAN ONLY)
- **Backend requirements**: A drawer pull report aggregate with the same fields and exception sets; print formatting.
- **UI requirements**: Preview + print + finish.
- **Constraints for implementers**: Preserve inclusion of multi-currency balances and void/discount exception sections.

# UI Feature Gaps
**Audit Date:** 2026-01-01

## 1. Specialized Dialogs
- **[MISSING] Table Reservation UI:** `TableBookingInfo` editor / calendar view on Table Map.
- **[MISSING] Pizza Builder:** A specialized matrix UI for Whole/Half/Quarter toppings. Magidesk relies on generic modifier lists which are slower and more error-prone for pizza ordering.
- **[MISSING] Database Configuration:** `DatabaseConfigurationDialog` (Floreant startup). Magidesk likely relies on `appsettings.json`, missing a user-friendly way to point to a new DB server at runtime.

## 2. Reporting & Backoffice
- **[PARTIAL] Report Viewer:** Floreant has a dedicated `ReportViewer` leveraging JasperReports for print-perfect output. Magidesk has `SalesReportsPage` (DataGrid-based?). The "Print Preview" and "Export to PDF/Excel" capability for *regulatory* reports (e.g. Tax) is likely a gap compared to Jasper.
- **[MISSING] Ticket Viewer / Journal:** `JournalReportView` in Floreant allows deep diving into past transaction logs. Magidesk `SalesReportsPage` aggregates, but a raw "Journal" view for auditing line-by-line actions (voids, no-sales) is missing.

## 3. Hardware & Peripherals
- **[PARTIAL] Scale Integration:** Floreant has `AutomatedWeightInputDialog` for weighing items (Frozen Yogurt/Deli). Magidesk has `QuantityViewModel` but no explicit serial/scale UI found.

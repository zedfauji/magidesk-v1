# Category H: Reporting & Export

## H.1 Sales reports by product

**Feature ID:** H.1  
**Feature Name:** Sales reports by product  
**Status:** PARTIALLY IMPLEMENTED

**Backend Evidence:**
- Database tables / columns: Ticket, OrderLine tables
- Domain entities: `SalesDetailReportDto.cs`, `MenuUsageReportDto.cs`
- Services: `GetMenuUsageReportQueryHandler.cs`
- APIs / handlers: `ReportsController.cs`
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): `SalesReportsPage.xaml` exists
- ViewModels: `SalesReportsViewModel.cs`
- Navigation path: Reports → Sales
- User-visible workflow: Select date range, run report

**Notes:**
- MenuUsageReport tracks product sales
- Backend infrastructure exists

**Risks / Gaps:**
- Report detail level unclear
- May need drill-down capability

**Recommendation:** VERIFY - Confirm data completeness

---

## H.2 Sales reports by date range

**Feature ID:** H.2  
**Feature Name:** Sales reports by date range  
**Status:** FULLY IMPLEMENTED

**Backend Evidence:**
- Database tables / columns: Ticket.CreatedAt for filtering
- Domain entities: Date parameters in all report DTOs
- Services: All report handlers accept date range
- APIs / handlers: StartDate/EndDate parameters
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): Date pickers in `SalesReportsPage.xaml`
- ViewModels: SetDateRangeCommand with presets (Today, Week, Month)
- Navigation path: Reports → Set Date Range → Run
- User-visible workflow: Quick buttons for common ranges

**Notes:**
- Full date range filtering implemented
- Preset buttons for convenience

**Risks / Gaps:**
- None significant

**Recommendation:** COMPLETE - Works as expected

---

## H.3 Sales reports by table

**Feature ID:** H.3  
**Feature Name:** Sales reports by table  
**Status:** PARTIALLY IMPLEMENTED

**Backend Evidence:**
- Database tables / columns: Ticket.TableNumber
- Domain entities: TableNumber in ticket data
- Services: NO EVIDENCE FOUND for specific by-table report
- APIs / handlers: NO EVIDENCE FOUND
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): NO EVIDENCE FOUND
- ViewModels: NO EVIDENCE FOUND
- Navigation path: NO EVIDENCE FOUND
- User-visible workflow: NO EVIDENCE FOUND

**Notes:**
- Data exists but no dedicated report
- Could filter existing reports by table

**Risks / Gaps:**
- Cannot analyze per-table performance

**Recommendation:** IMPLEMENT - Add table-based sales report

---

## H.4 Usage reports (tables / time)

**Feature ID:** H.4  
**Feature Name:** Usage reports (tables / time)  
**Status:** NOT IMPLEMENTED

**Backend Evidence:**
- Database tables / columns: No table session duration tracking
- Domain entities: NO EVIDENCE FOUND
- Services: NO EVIDENCE FOUND
- APIs / handlers: NO EVIDENCE FOUND
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): NO EVIDENCE FOUND
- ViewModels: NO EVIDENCE FOUND
- Navigation path: NO EVIDENCE FOUND
- User-visible workflow: NO EVIDENCE FOUND

**Notes:**
- Critical for billiard club operation
- Requires TableSession entity first

**Risks / Gaps:**
- Cannot analyze table utilization
- No peak hours analysis

**Recommendation:** IMPLEMENT - After implementing table time tracking

---

## H.5 Financial summaries

**Feature ID:** H.5  
**Feature Name:** Financial summaries  
**Status:** FULLY IMPLEMENTED

**Backend Evidence:**
- Database tables / columns: Ticket totals, Payment amounts
- Domain entities: `SalesSummaryReportDto.cs`, `SalesBalanceReportDto.cs`
- Services: Report handlers calculate totals
- APIs / handlers: Summary endpoints
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): Summary panel in `SalesReportsPage.xaml`
- ViewModels: GrossSales, NetSales, TotalTax displayed
- Navigation path: Reports → Sales Summary
- User-visible workflow: Dashboard-style summary

**Notes:**
- Aggregate financial data displayed
- Gross, Net, Tax breakdowns

**Risks / Gaps:**
- None significant

**Recommendation:** COMPLETE - Works as expected

---

## H.6 Member activity reports

**Feature ID:** H.6  
**Feature Name:** Member activity reports  
**Status:** NOT IMPLEMENTED

**Backend Evidence:**
- Database tables / columns: No Member entity
- Domain entities: NO EVIDENCE FOUND
- Services: NO EVIDENCE FOUND
- APIs / handlers: NO EVIDENCE FOUND
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): NO EVIDENCE FOUND
- ViewModels: NO EVIDENCE FOUND
- Navigation path: NO EVIDENCE FOUND
- User-visible workflow: NO EVIDENCE FOUND

**Notes:**
- Requires Member system first
- Would show member spending, visits

**Risks / Gaps:**
- No member analytics

**Recommendation:** IMPLEMENT - After Member module

---

## H.7 Inventory reports

**Feature ID:** H.7  
**Feature Name:** Inventory reports  
**Status:** NOT IMPLEMENTED

**Backend Evidence:**
- Database tables / columns: InventoryItem exists
- Domain entities: NO EVIDENCE FOUND for reports
- Services: NO EVIDENCE FOUND
- APIs / handlers: NO EVIDENCE FOUND
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): NO EVIDENCE FOUND
- ViewModels: NO EVIDENCE FOUND
- Navigation path: NO EVIDENCE FOUND
- User-visible workflow: NO EVIDENCE FOUND

**Notes:**
- Inventory data exists but no reports
- Stock levels, movement, variances

**Risks / Gaps:**
- Cannot monitor inventory status

**Recommendation:** IMPLEMENT - Add inventory report UI

---

## H.8 X reports (mid-shift)

**Feature ID:** H.8  
**Feature Name:** X reports (mid-shift)  
**Status:** PARTIALLY IMPLEMENTED

**Backend Evidence:**
- Database tables / columns: CashSession data
- Domain entities: `GetShiftReportQuery.cs`
- Services: `GetShiftReportQueryHandler.cs`
- APIs / handlers: Shift report endpoint
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): Shift report dialog exists
- ViewModels: ShiftReportViewModel
- Navigation path: Manager → Shift Report
- User-visible workflow: Current session summary

**Notes:**
- Shift report = X report concept
- Shows current session data without closing

**Risks / Gaps:**
- May need explicit "X Report" naming

**Recommendation:** VERIFY - Ensure data completeness

---

## H.9 Z reports (end-of-day)

**Feature ID:** H.9  
**Feature Name:** Z reports (end-of-day)  
**Status:** PARTIALLY IMPLEMENTED

**Backend Evidence:**
- Database tables / columns: CashSession with closing
- Domain entities: `GetDrawerPullReportQuery.cs`
- Services: `GetDrawerPullReportQueryHandler.cs`
- APIs / handlers: Drawer pull report
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): `DrawerPullReportDialog.xaml`
- ViewModels: DrawerPullReportViewModel
- Navigation path: Manager → Close Drawer → Report
- User-visible workflow: End-of-shift settlement

**Notes:**
- Drawer pull = Z report concept
- Closes session and generates totals

**Risks / Gaps:**
- May not reset counters properly

**Recommendation:** VERIFY - Confirm EOD reset behavior

---

## H.10 Export to PDF

**Feature ID:** H.10  
**Feature Name:** Export to PDF  
**Status:** NOT IMPLEMENTED

**Backend Evidence:**
- Database tables / columns: N/A
- Domain entities: N/A
- Services: NO EVIDENCE FOUND
- APIs / handlers: NO EVIDENCE FOUND
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): NO EVIDENCE FOUND
- ViewModels: NO EVIDENCE FOUND
- Navigation path: NO EVIDENCE FOUND
- User-visible workflow: NO EVIDENCE FOUND

**Notes:**
- Would require PDF generation library
- Common requirement for archiving

**Risks / Gaps:**
- Cannot save reports for records

**Recommendation:** IMPLEMENT - Add PDF export capability

---

## H.11 Export to Excel

**Feature ID:** H.11  
**Feature Name:** Export to Excel  
**Status:** NOT IMPLEMENTED

**Backend Evidence:**
- Database tables / columns: N/A
- Domain entities: N/A
- Services: NO EVIDENCE FOUND
- APIs / handlers: NO EVIDENCE FOUND
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): NO EVIDENCE FOUND
- ViewModels: NO EVIDENCE FOUND
- Navigation path: NO EVIDENCE FOUND
- User-visible workflow: NO EVIDENCE FOUND

**Notes:**
- Most requested export format
- EPPlus or similar library needed

**Risks / Gaps:**
- Cannot analyze data in spreadsheets

**Recommendation:** IMPLEMENT - Priority export format

---

## H.12 Export to Word

**Feature ID:** H.12  
**Feature Name:** Export to Word  
**Status:** NOT IMPLEMENTED

**Backend Evidence:**
- Database tables / columns: N/A
- Domain entities: N/A
- Services: NO EVIDENCE FOUND
- APIs / handlers: NO EVIDENCE FOUND
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): NO EVIDENCE FOUND
- ViewModels: NO EVIDENCE FOUND
- Navigation path: NO EVIDENCE FOUND
- User-visible workflow: NO EVIDENCE FOUND

**Notes:**
- Lower priority than PDF/Excel
- Document generation needed

**Risks / Gaps:**
- Limited use case

**Recommendation:** DEFER - Lower priority

---

## H.13 Export to HTML

**Feature ID:** H.13  
**Feature Name:** Export to HTML  
**Status:** NOT IMPLEMENTED

**Backend Evidence:**
- Database tables / columns: N/A
- Domain entities: N/A
- Services: NO EVIDENCE FOUND
- APIs / handlers: NO EVIDENCE FOUND
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): NO EVIDENCE FOUND
- ViewModels: NO EVIDENCE FOUND
- Navigation path: NO EVIDENCE FOUND
- User-visible workflow: NO EVIDENCE FOUND

**Notes:**
- Easy to implement from data
- Good for email attachments

**Risks / Gaps:**
- None critical

**Recommendation:** CONSIDER - Easy implementation

---

## H.14 Export to TXT

**Feature ID:** H.14  
**Feature Name:** Export to TXT  
**Status:** NOT IMPLEMENTED

**Backend Evidence:**
- Database tables / columns: N/A
- Domain entities: N/A
- Services: NO EVIDENCE FOUND
- APIs / handlers: NO EVIDENCE FOUND
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): NO EVIDENCE FOUND
- ViewModels: NO EVIDENCE FOUND
- Navigation path: NO EVIDENCE FOUND
- User-visible workflow: NO EVIDENCE FOUND

**Notes:**
- Simplest export format
- Useful for legacy systems

**Risks / Gaps:**
- None critical

**Recommendation:** CONSIDER - Easy implementation

---

## H.15 Export to JPG

**Feature ID:** H.15  
**Feature Name:** Export to JPG  
**Status:** NOT IMPLEMENTED

**Backend Evidence:**
- Database tables / columns: N/A
- Domain entities: N/A
- Services: NO EVIDENCE FOUND
- APIs / handlers: NO EVIDENCE FOUND
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): NO EVIDENCE FOUND
- ViewModels: NO EVIDENCE FOUND
- Navigation path: NO EVIDENCE FOUND
- User-visible workflow: NO EVIDENCE FOUND

**Notes:**
- Screenshot/render capability
- Lower priority

**Risks / Gaps:**
- Limited use case

**Recommendation:** DEFER - Low priority

---

## Category H COMPLETE

- Features audited: 15
- Fully implemented: 2
- Partially implemented: 4
- Not implemented: 9

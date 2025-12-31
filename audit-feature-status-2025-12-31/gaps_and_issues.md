# Gaps & Issues
**Date:** 2025-12-31

## Backend Gaps
1.  **Reporting Engine**: No generalized Reporting Service exists yet. Reports (F-0092 to F-0104) are largely stubbed or missing backend logic to aggregate data efficiently.
2.  **Configuration**: APIs for Printers, Taxes, Coupons (F-0117, F-0118, F-0127) are missing or rudimentary.
3.  **Role Logic**: `DeleteRoleCommand` validation is basic. Floreant might have deeper checks.

## Frontend Gaps
1.  **Report Viewers**: Missing detailed report viewers (JasperReports equivalent).
2.  **Configuration Dialogs**: Many configuration screens (Terminal, Card, Tax) are stubbed in `SystemConfigPage`.
3.  **Explorers**: Missing Coupon, Tax, and Printer Group explorers.

## Known Issues
1.  **Source Generators**: MVVM Toolkit Source Generators occasionally fail to generate properties (e.g., in `RoleManagementViewModel`), requiring manual property implementation.
2.  **Navigation**: Some administrative screens are not yet linked in the main navigation or require enablement in `BackOfficeViewModel`.

## Limitations
1.  **Monolith**: The system runs as a monolith, unlike the planned distributed API architecture.
2.  **WinUI 3**: Some legacy Swing components (like intricate tables) are simplified in XAML.

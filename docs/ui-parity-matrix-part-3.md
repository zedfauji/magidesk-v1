# UI Parity Audit: Part 3 (F-0101 - F-0132)

| ID | Feature | FloreantPOS UI Behavior | Forensic UI Expectation | Current Magidesk Status | Parity Level |
| :--- | :--- | :--- | :--- | :--- | :--- |
| **F-0101** | Tip Report | Grid: Server / Amount. | Financial Report. | **NOT IMPLEMENTED** | **LOW** |
| **F-0102** | Attendance | Grid: User / Time In-Out. | Labor Audit. | **NOT IMPLEMENTED** | **LOW** |
| **F-0103** | Journal | Log View. | Audit Trail. | **NOT IMPLEMENTED** | **MEDIUM** |
| **F-0104** | Cash Out | Detailed Receipt Preview. | Server Checkout. | **NOT IMPLEMENTED** | **BLOCKER** |
| **F-0105** | Rest Config | Form: Name, Addr, Ticket Msg. | Settings Form. | **PARTIAL** (`SettingsPage` placeholder) | **MEDIUM** |
| **F-0106** | Term Config | Form: ID, Device Rules. | Hardware Setup. | **NOT IMPLEMENTED** | **HIGH RISK** |
| **F-0107** | Card Config | Form: Gateway Creds. | Secure Config. | **NOT IMPLEMENTED** | **HIGH RISK** |
| **F-0108** | Print Config | List: Physical Printers. | Routing Setup. | **PARTIAL** (`PrintPage` dev stub) | **CRITICAL** |
| **F-0109** | Tax Config | List: Rates. | Financial Setup. | **PARTIAL** (`DiscountTaxPage` dev stub) | **HIGH RISK** |
| **F-0110** | Language | Dropdown. | Locale Switcher. | **NOT IMPLEMENTED** | **LOW** |
| **F-0111** | Back Office | Tabbed Admin Window. | Admin Dashboard. | **PARTIAL** (`BackOfficePage` shell) | **HIGH RISK** |
| **F-0112** | Cat Explorer | Tree/Grid CRUD. | Menu Structure. | **PARTIAL** (Covered by `MenuEditorPage`) | **MEDIUM** |
| **F-0113** | Grp Explorer | Tree/Grid CRUD. | Menu Structure. | **PARTIAL** (Covered by `MenuEditorPage`) | **MEDIUM** |
| **F-0114** | Item Explorer | Form: Price, Tax, Printer. | Menu Detail. | **PARTIAL** (`MenuEditorPage` exists) | **MEDIUM** |
| **F-0115** | Mod Explorer | List: Modifiers. | Modifier CRUD. | **PARTIAL** (`ModifierEditorPage` exists) | **MEDIUM** |
| **F-0116** | ModGrp Exp | Form: Min/Max Rules. | Group Rules. | **PARTIAL** (`ModifierEditorPage` exists) | **MEDIUM** |
| **F-0117** | Coupon Exp | List: Rules. | Promo Config. | **NOT IMPLEMENTED** | **MEDIUM** |
| **F-0118** | Tax Explorer | List: Rates. | Tax CRUD. | **NOT IMPLEMENTED** | **HIGH RISK** |
| **F-0119** | Shift Exp | List: Definitions. | Shift CRUD. | **NOT IMPLEMENTED** | **LOW** |
| **F-0120** | User Exp | List: Users/Roles. | HR Config. | **PARTIAL** (`UserManagementPage` stub) | **BLOCKER** |
| **F-0121** | Order Type | List: Flags (Deliv/Dine). | Behavior Config. | **NOT IMPLEMENTED** | **MEDIUM** |
| **F-0122** | Cpn Dialog | Input: Code. | Discount Trigger. | **NOT IMPLEMENTED** | **MEDIUM** |
| **F-0123** | Disc Dialog | List: Manual Reasons. | Manager Override. | **NOT IMPLEMENTED** | **MEDIUM** |
| **F-0124** | Floor Config | Designer/List. | Zone Setup. | **NOT IMPLEMENTED** | **LOW** |
| **F-0125** | Notes | Text Area. | Special Req. | **NOT IMPLEMENTED** | **LOW** |
| **F-0126** | Deliv Zone | Map/List Zip Codes. | Logistics Config. | **NOT IMPLEMENTED** | **MEDIUM** |
| **F-0127** | Print Grp | List: Logical Printers. | Routing Logic. | **NOT IMPLEMENTED** | **CRITICAL** |
| **F-0128** | DB Backup | Dialog: Path. | Maintenance. | **NOT IMPLEMENTED** | **HIGH RISK** |
| **F-0129** | Banner | Footer Marquee. | Alerts. | **NOT IMPLEMENTED** | **LOW** |
| **F-0130** | About | Dialog: Ver #. | Info. | **NOT IMPLEMENTED** | **LOW** |
| **F-0131** | Confirm Dlg | "Are you sure?". | Safety. | **PARTIAL** (Ad-hoc usage) | **LOW** |
| **F-0132** | Progress Dlg | Blocking Spinner. | UX Feedback. | **PARTIAL** (Non-blocking rings) | **MEDIUM** |

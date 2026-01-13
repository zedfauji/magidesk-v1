# Frontend Tickets: Categories B-D, G-M (Consolidated)

> [!NOTE]
> This file consolidates frontend tickets for categories with partial implementation. P0/P1 tickets are detailed; P2 tickets are summarized.

---

## Category B - Floor & Layout Management

| Ticket ID | Feature ID | Title | Priority | Status |
|-----------|------------|-------|----------|--------|
| FE-B.3-01 | B.3 | Complete Property Editor Panel | P1 | NOT_STARTED |
| FE-B.6-01 | B.6 | Add Unsaved Changes Warning | P2 | NOT_STARTED |
| FE-B.9-01 | B.9 | Add Undo/Redo Toolbar | P2 | NOT_STARTED |

### FE-B.3-01: Complete Property Editor Panel

**Priority:** P1

**Scope:**
- Extend existing property panel to include all table properties
- Add color picker
- Add rotation slider
- Add icon selector

**Acceptance Criteria:**
- [ ] All properties editable
- [ ] Changes preview in real-time
- [ ] Save persists changes

---

## Category C - Billing, Payments & Pricing

| Ticket ID | Feature ID | Title | Priority | Status |
|-----------|------------|-------|----------|--------|
| FE-C.2-01 | C.2 | Display Time Charges on Ticket | P0 | NOT_STARTED |
| FE-C.5-01 | C.5 | Create Split Payment Dialog | P1 | NOT_STARTED |
| FE-C.6-01 | C.6 | Add Gratuity Selection Panel | P1 | NOT_STARTED |
| FE-C.7-01 | C.7 | Improve Discount Application UI | P1 | NOT_STARTED |

### FE-C.2-01: Display Time Charges on Ticket

**Priority:** P0

**Scope:**
- Show time charge line items distinctly
- Display duration and rate
- Show time icon

**Implementation:**
```xml
<!-- Time charge line item template -->
<DataTemplate x:DataType="vm:TimeChargeLineViewModel">
    <Grid>
        <FontIcon Glyph="&#xE121;" /> <!-- Clock icon -->
        <StackPanel>
            <TextBlock Text="Table Time" FontWeight="SemiBold" />
            <TextBlock Text="{x:Bind Duration, StringFormat='Duration: {0}'}" />
            <TextBlock Text="{x:Bind RateInfo}" Opacity="0.7" />
        </StackPanel>
        <TextBlock Text="{x:Bind Total}" HorizontalAlignment="Right" />
    </Grid>
</DataTemplate>
```

**Acceptance Criteria:**
- [ ] Time charges display distinctly
- [ ] Duration shown
- [ ] Rate breakdown available

### FE-C.5-01: Create Split Payment Dialog

**Priority:** P1

**Scope:**
- Multiple payment method selection
- Amount entry per method
- Running total display
- Change calculation

**Acceptance Criteria:**
- [ ] Add multiple payments
- [ ] Track remaining balance
- [ ] Calculate change
- [ ] Process all payments

---

## Category G - Inventory & Products

| Ticket ID | Feature ID | Title | Priority | Status |
|-----------|------------|-------|----------|--------|
| FE-G.2-01 | G.2 | Add Stock Level Display | P1 | NOT_STARTED |
| FE-G.3-01 | G.3 | Create Low Stock Alert Badge | P1 | NOT_STARTED |
| FE-G.5-01 | G.5 | Improve Modifier Selection UI | P1 | NOT_STARTED |

### FE-G.2-01: Add Stock Level Display

**Priority:** P1

**Scope:**
- Show stock on product cards
- Low stock warning color
- Out of stock indicator

**Acceptance Criteria:**
- [ ] Stock count visible
- [ ] Color coding for low stock
- [ ] Out of stock cannot be ordered

---

## Category H - Reporting & Export

| Ticket ID | Feature ID | Title | Priority | Status |
|-----------|------------|-------|----------|--------|
| FE-H.1-01 | H.1 | Create Daily Sales Report Page | P1 | NOT_STARTED |
| FE-H.4-01 | H.4 | Create Table Utilization Dashboard | P1 | NOT_STARTED |
| FE-H.5-01 | H.5 | Create Time Revenue Analytics Page | P1 | NOT_STARTED |
| FE-H.6-01 | H.6 | Create Member Analytics Dashboard | P2 | NOT_STARTED |

### FE-H.4-01: Create Table Utilization Dashboard

**Priority:** P1

**Scope:**
- Heat map of table usage
- Peak hours chart
- Occupancy statistics
- Revenue per table

**Acceptance Criteria:**
- [ ] Visual heat map
- [ ] Date range selector
- [ ] Export capability

---

## Category I - Hardware & Peripherals

| Ticket ID | Feature ID | Title | Priority | Status |
|-----------|------------|-------|----------|--------|
| FE-I.4-01 | I.4 | Create Lamp Control Panel | P1 | NOT_STARTED |
| FE-I.11-01 | I.11 | Kitchen Display System UI | P2 | COMPLETED |

### FE-I.4-01: Create Lamp Control Panel

**Priority:** P1

**Scope:**
- Manual lamp on/off buttons
- Status indicators per table
- All on/off button

**Acceptance Criteria:**
- [ ] Individual lamp control
- [ ] Status reflects actual state
- [ ] Master control works

### FE-I.11-01: Kitchen Display System UI

**Priority:** P2  
**Status:** COMPLETED

**Scope:**
- ✅ Kitchen display page implementation
- ✅ Order status tracking UI
- ✅ Order ready notifications
- ✅ Integration with KitchenDisplayViewModel
- ✅ Enhanced status service integration

**Acceptance Criteria:**
- [x] Kitchen display shows active orders
- [x] Order status updates in real-time
- [x] Order ready notifications work
- [x] Integration with enhanced status service
- [x] OrderEntryViewModel subscribes to notifications

**Completed:** 2026-01-12

---

## Category J - Security, Users & Staff

| Ticket ID | Feature ID | Title | Priority | Status |
|-----------|------------|-------|----------|--------|
| FE-J.1-01 | J.1 | Create ManagerPinDialog | P0 | NOT_STARTED |
| FE-J.1-02 | J.1 | Create LoginPage | P0 | NOT_STARTED |
| FE-J.9-01 | J.9 | Create ClockInOutPanel | P1 | NOT_STARTED |

### FE-J.1-01: Create ManagerPinDialog

**Priority:** P0

**Scope:**
- PIN entry numeric keypad
- Secure PIN masking
- Error feedback
- Timeout handling

**Implementation:**
```xml
<ContentDialog Title="Manager Authorization" IsPrimaryButtonEnabled="{x:Bind ViewModel.CanSubmit}">
    <StackPanel>
        <TextBlock Text="Enter Manager PIN" />
        <PasswordBox 
            Password="{x:Bind ViewModel.Pin, Mode=TwoWay}"
            PasswordRevealMode="Hidden"
            MaxLength="4" />
        
        <!-- Numeric Keypad -->
        <Grid>
            <!-- 3x4 grid of number buttons -->
        </Grid>
        
        <InfoBar 
            IsOpen="{x:Bind ViewModel.HasError}"
            Severity="Error"
            Message="{x:Bind ViewModel.ErrorMessage}" />
    </StackPanel>
</ContentDialog>
```

**Acceptance Criteria:**
- [ ] PIN entry works
- [ ] PIN masked
- [ ] Error shown for invalid
- [ ] Clears after failed attempt
- [ ] Returns authorization result

### FE-J.1-02: Create LoginPage

**Priority:** P0

**Scope:**
- User selection grid
- PIN entry
- Secure session start
- Logout on inactivity

> [!IMPORTANT]
> This is a **critical security gap**. Currently there is NO login page - the app opens directly to POS.

**Acceptance Criteria:**
- [ ] User selection works
- [ ] PIN validates
- [ ] Session started on success
- [ ] Failed attempts limited
- [ ] Logout works

---

## Category K - Localization

| Ticket ID | Feature ID | Title | Priority | Status |
|-----------|------------|-------|----------|--------|
| FE-K.3-01 | K.3 | Currency Format Display | P2 | NOT_STARTED |
| FE-K.4-01 | K.4 | Date/Time Format Display | P2 | NOT_STARTED |

---

## Category L - Operations

| Ticket ID | Feature ID | Title | Priority | Status |
|-----------|------------|-------|----------|--------|
| FE-L.3-01 | L.3 | Create Backup Management Page | P1 | NOT_STARTED |

---

## FE-G.4-01: Create Category Hierarchy Tree View

**Ticket ID:** FE-G.4-01  
**Feature ID:** G.4  
**Type:** Frontend  
**Title:** Create Category Hierarchy Tree View  
**Priority:** P2

### Outcome
Hierarchical tree view for nested product categories.

### Scope
- Create `CategoryTreeView` control
- Support drag-drop reordering
- Show parent-child relationships
- Expand/collapse nodes

### Quality & Guardrails
- **mvvm-pattern.md:** ViewModel pattern
- **G13:** Accessibility compliant

### Dependencies
| Type | Dependency | Ticket ID |
|------|------------|-----------|
| HARD | Category hierarchy BE | BE-G.4-01 |

### Acceptance Criteria
- [ ] Tree displays nested categories
- [ ] Drag-drop works
- [ ] Expand/collapse functional
- [ ] Add subcategory works
- [ ] Delete maintains integrity

---

## FE-C.6-01: Add Gratuity Selection Panel

**Ticket ID:** FE-C.6-01  
**Feature ID:** C.6  
**Type:** Frontend  
**Title:** Add Gratuity Selection Panel  
**Priority:** P1

### Outcome
Quick gratuity selection during payment.

### Scope
- Create gratuity selection control
- Preset percentage buttons
- Custom amount input
- Auto-calculate based on subtotal

### Quality & Guardrails
- **mvvm-pattern.md:** ViewModel pattern
- **G13:** Accessible buttons

### Dependencies
| Type | Dependency | Ticket ID |
|------|------------|-----------|
| SOFT | Payment processing | Exists |

### Acceptance Criteria
- [ ] Preset buttons work
- [ ] Custom input works
- [ ] Calculation correct
- [ ] Selection persists through payment

---

## FE-C.7-01: Improve Discount Application UI

**Ticket ID:** FE-C.7-01  
**Feature ID:** C.7  
**Type:** Frontend  
**Title:** Improve Discount Application UI  
**Priority:** P1

### Outcome
Enhanced discount selection interface.

### Scope
- Improve discount selector dialog
- Show available discounts
- Highlight member discounts
- Preview total with discount

### Quality & Guardrails
- **mvvm-pattern.md:** ViewModel pattern

### Dependencies
| Type | Dependency | Ticket ID |
|------|------------|-----------|
| SOFT | Discount BE | BE-C.7-01 |

### Acceptance Criteria
- [ ] Available discounts shown
- [ ] Member discounts highlighted
- [ ] Preview displays correctly
- [ ] Apply discount works

---

## FE-H.1-01: Create Daily Sales Report Page

**Ticket ID:** FE-H.1-01  
**Feature ID:** H.1  
**Type:** Frontend  
**Title:** Create Daily Sales Report Page  
**Priority:** P1

### Outcome
Page displaying daily sales analytics.

### Scope
- Create `DailySalesReportPage.xaml`
- Display revenue charts
- Breakdown tables
- Export functionality

### Quality & Guardrails
- **mvvm-pattern.md:** ViewModel pattern

### Dependencies
| Type | Dependency | Ticket ID |
|------|------------|-----------|
| HARD | Daily sales query | BE-H.1-01 |

### Acceptance Criteria
- [ ] Report displays correctly
- [ ] Charts render
- [ ] Data accurate
- [ ] Export works

---

## FE-H.5-01: Create Time Revenue Analytics Page

**Ticket ID:** FE-H.5-01  
**Feature ID:** H.5  
**Type:** Frontend  
**Title:** Create Time Revenue Analytics Page  
**Priority:** P1

### Outcome
Analytics page for time-based billing revenue.

### Scope
- Create `TimeRevenueAnalyticsPage.xaml`
- Revenue charts by table type
- Peak hours visualization
- Weekday vs weekend comparison

### Quality & Guardrails
- **mvvm-pattern.md:** ViewModel pattern

### Dependencies
| Type | Dependency | Ticket ID |
|------|------------|-----------|
| HARD | Time revenue query | BE-H.5-01 |

### Acceptance Criteria
- [ ] Page loads data
- [ ] Charts display correctly
- [ ] Filtering works
- [ ] Export functional

---

## Summary - All Consolidated Frontend Tickets

| Category | Tickets | Priority Distribution |
|----------|---------|----------------------|
| B | 3 | P1: 1, P2: 2 |
| C | 6 | P0: 1, P1: 5 |
| G | 4 | P1: 3, P2: 1 |
| H | 6 | P1: 5, P2: 1 |
| I | 1 | P1: 1 |
| J | 3 | P0: 2, P1: 1 |
| K | 2 | P2: 2 |
| L | 1 | P1: 1 |
| **Total** | **26** | **P0: 3, P1: 17, P2: 6** |

---

*Last Updated: 2026-01-10*


**Ticket ID:** FE-C.6-01  
**Feature ID:** C.6  
**Type:** Frontend  
**Title:** Add Gratuity Selection Panel  
**Priority:** P1

### Outcome
Quick gratuity selection during payment.

### Scope
- Create gratuity selection control
- Preset percentage buttons
- Custom amount input
- Auto-calculate based on subtotal

### Quality & Guardrails
- **mvvm-pattern.md:** ViewModel pattern
- **G13:** Accessible buttons

### Dependencies
| Type | Dependency | Ticket ID |
|------|------------|-----------|
| SOFT | Payment processing | Exists |

### Acceptance Criteria
- [ ] Preset buttons work
- [ ] Custom input works
- [ ] Calculation correct
- [ ] Selection persists through payment

---

## FE-C.7-01: Improve Discount Application UI

**Ticket ID:** FE-C.7-01  
**Feature ID:** C.7  
**Type:** Frontend  
**Title:** Improve Discount Application UI  
**Priority:** P1

### Outcome
Enhanced discount selection interface.

### Scope
- Improve discount selector dialog
- Show available discounts
- Highlight member discounts
- Preview total with discount

### Quality & Guardrails
- **mvvm-pattern.md:** ViewModel pattern

### Dependencies
| Type | Dependency | Ticket ID |
|------|------------|-----------|
| SOFT | Discount BE | BE-C.7-01 |

### Acceptance Criteria
- [ ] Available discounts shown
- [ ] Member discounts highlighted
- [ ] Preview displays correctly
- [ ] Apply discount works

---

## FE-H.1-01: Create Daily Sales Report Page

**Ticket ID:** FE-H.1-01  
**Feature ID:** H.1  
**Type:** Frontend  
**Title:** Create Daily Sales Report Page  
**Priority:** P1

### Outcome
Page displaying daily sales analytics.

### Scope
- Create `DailySalesReportPage.xaml`
- Display revenue charts
- Breakdown tables
- Export functionality

### Quality & Guardrails
- **mvvm-pattern.md:** ViewModel pattern

### Dependencies
| Type | Dependency | Ticket ID |
|------|------------|-----------|
| HARD | Daily sales query | BE-H.1-01 |

### Acceptance Criteria
- [ ] Report displays correctly
- [ ] Charts render
- [ ] Data accurate
- [ ] Export works

---

## FE-H.5-01: Create Time Revenue Analytics Page

**Ticket ID:** FE-H.5-01  
**Feature ID:** H.5  
**Type:** Frontend  
**Title:** Create Time Revenue Analytics Page  
**Priority:** P1

### Outcome
Analytics page for time-based billing revenue.

### Scope
- Create `TimeRevenueAnalyticsPage.xaml`
- Revenue charts by table type
- Peak hours visualization
- Weekday vs weekend comparison

### Quality & Guardrails
- **mvvm-pattern.md:** ViewModel pattern

### Dependencies
| Type | Dependency | Ticket ID |
|------|------------|-----------|
| HARD | Time revenue query | BE-H.5-01 |

### Acceptance Criteria
- [ ] Page loads data
- [ ] Charts display correctly
- [ ] Filtering works
- [ ] Export functional

---

## Category M - System Safety, Diagnostics & Recovery

| Ticket ID | Feature ID | Title | Priority | Status |
|-----------|------------|-------|----------|--------|
| FE-M.1-01 | M.1 | Error Management Dashboard | P2 | DONE |
| FE-M.9-01 | M.9 | Enhanced Error Dialog System | P2 | DONE |

### FE-M.1-01: Error Management Dashboard

**Priority:** P2  
**Status:** DONE

**Scope:**
- ✅ ErrorManagementPage.xaml implementation
- ✅ ErrorManagementViewModel with full functionality
- ✅ Error filtering and categorization UI
- ✅ Error resolution workflow
- ✅ Export functionality

**Acceptance Criteria:**
- [x] Dashboard displays recent errors
- [x] Error filtering by category and severity works
- [x] Error details panel shows comprehensive information
- [x] Mark errors as resolved functionality
- [x] Export errors to reports

**Completed:** 2026-01-12

### FE-M.9-01: Enhanced Error Dialog System

**Priority:** P2  
**Status:** DONE

**Scope:**
- ✅ EnhancedDialogService implementation
- ✅ Categorized error dialogs with recovery suggestions
- ✅ Hardware-specific error handling
- ✅ Network error handling with retry options
- ✅ Integration with error reporting service

**Acceptance Criteria:**
- [x] Enhanced error dialogs with categorization
- [x] Recovery suggestions displayed
- [x] Hardware and network specific error handling
- [x] Automatic error reporting to management
- [x] User-friendly error messages

**Completed:** 2026-01-12

---

## Summary - All Consolidated Frontend Tickets

| Category | Tickets | Priority Distribution |
|----------|---------|----------------------|
| B | 3 | P1: 1, P2: 2 |
| C | 6 | P0: 1, P1: 5 |
| G | 3 | P1: 3 |
| H | 6 | P1: 5, P2: 1 |
| I | 2 | P1: 1, P2: 1 |
| J | 3 | P0: 2, P1: 1 |
| K | 2 | P2: 2 |
| L | 1 | P1: 1 |
| M | 2 | P2: 2 |
| **Total** | **28** | **P0: 3, P1: 17, P2: 8** |

---

*Last Updated: 2026-01-12*

---

## Missing Tickets: Category B (Floor & Layout)

### FE-B.1-01: Floor Definition UI
**Feature ID:** B.1  
**Priority:** P2  
**Action:** Configure room names, dimensions, and types.  
**Criteria:** CRUD for Floors.

### FE-B.4-01: Background Image Manager
**Feature ID:** B.4  
**Priority:** P2  
**Action:** Upload and scale floor plan images.  
**Criteria:** Image renders behind tables; opacity control.

### FE-B.10-01: Alignment Guides & Snapping
**Feature ID:** B.10  
**Priority:** P2  
**Action:** Visual guidelines when moving tables.  
**Criteria:** Smart snap to adjacent tables.

### FE-B.11-01: Layout Zoom & Pan
**Feature ID:** B.11  
**Priority:** P2  
**Action:** Interactive canvas controls.  
**Criteria:** Mouse wheel zoom, drag pan.

### FE-B.12-01: Multi-Select Tables
**Feature ID:** B.12  
**Priority:** P2  
**Action:** Box select or Shift+Click multiple tables.  
**Criteria:** Move/Delete group of tables.

### FE-B.13-01: Layout Version History
**Feature ID:** B.13  
**Priority:** P3  
**Action:** List past layout saves; restore option.  
**Criteria:** Rollback to previous state.

### FE-B.14-01: Clone Layout
**Feature ID:** B.14  
**Priority:** P3  
**Action:** Copy layout from one floor to another.  
**Criteria:** Duplicates all tables and settings.

### FE-B.16-01: Layout Revert
**Feature ID:** B.16  
**Priority:** P2  
**Action:** Discard unsaved changes button.  
**Criteria:** Resets to last saved state.

---

## Missing Tickets: Category D (Tax & Currency)

### FE-D.1-01: Tax Configuration Page
**Feature ID:** D.1  
**Priority:** P1  
**Action:** Admin page to check/uncheck applicable taxes (VAT, GST, Sales Tax).  
**Criteria:** Toggle taxes globally.

### FE-D.2-01: Multiple Tax Rate UI
**Feature ID:** D.2  
**Priority:** P1  
**Action:** Define tax rates (Alcohol 10%, Food 5%).  
**Criteria:** CRUD for Tax Rates.

### FE-D.3-01: Tax Inclusive/Exclusive Toggle
**Feature ID:** D.3  
**Priority:** P1  
**Action:** Setting to switch price display mode.  
**Criteria:** Updates UI price display immediately.

### FE-D.4-01: Currency Formatting Settings
**Feature ID:** D.4  
**Priority:** P2  
**Action:** Configure symbol ($, €, £) and placement.  
**Criteria:** Updates all currency textblocks.

### FE-D.6-01: Service Charge Configuration
**Feature ID:** D.6  
**Priority:** P1  
**Action:** Configure auto-gratuity rules (e.g., Party > 6).  
**Criteria:** Set percentage and triggers.

### FE-D.7-01: Surcharge/Fee Management
**Feature ID:** D.7  
**Priority:** P2  
**Action:** Define credit card fees or holiday surcharges.  
**Criteria:** Apply rules to payments.

### FE-D.8-01: Rounding Rule Editor
**Feature ID:** D.8  
**Priority:** P2  
**Action:** Configure cash rounding (Nearest 0.05, etc.).  
**Criteria:** Visual example of rounding logic.

### FE-D.10-01: Tax Exemption UI
**Feature ID:** D.10  
**Priority:** P1  
**Action:** Button to remove tax from ticket (with Auth).  
**Criteria:** Requires Manager PIN; captures tax exempt ID.

---

## Missing Tickets: Category G (Inventory)

### FE-G.1-01: Product Management UI
**Feature ID:** G.1  
**Priority:** P1  
**Action:** Create/Edit products, categories, and ingredients.  
**Criteria:** Complete CRUD for catalog.

### FE-G.6-01: Stock Transfer UI
**Feature ID:** G.6  
**Priority:** P2  
**Action:** Move stock between locations (Bar -> Kitchen).  
**Criteria:** Select Source/Dest and Items.

### FE-G.7-01: Wastage Report/Entry
**Feature ID:** G.7  
**Priority:** P2  
**Action:** Log spilled/spoiled items.  
**Criteria:** Updates stock; asks for Reason.

### FE-G.8-01: Stock Take/Audit Interface
**Feature ID:** G.8  
**Priority:** P1  
**Action:** Digital countsheet for physical inventory.  
**Criteria:** Input actual counts; showing variance.

### FE-G.9-01: Vendor Management Page
**Feature ID:** G.9  
**Priority:** P2  
**Action:** Manage supplier details.  
**Criteria:** CRUD for Vendors.

### FE-G.10-01: Purchase Order Creator
**Feature ID:** G.10  
**Priority:** P2  
**Action:** Build orders based on low stock.  
**Criteria:** Auto-fill from low stock; email to vendor.

---

## Missing Tickets: Category H (Reporting)

### FE-H.2-01: End of Day Report UI
**Feature ID:** H.2  **Priority:** P0  
**Action:** Generate Z-Report.  **Criteria:** Visualize Sales, Taxes, Payments.

### FE-H.3-01: X-Report (Mid-Shift) UI
**Feature ID:** H.3  **Priority:** P1  
**Action:** View current shift totals without closing.

### FE-H.7-01: Sales by Category Chart
**Feature ID:** H.7  **Priority:** P2  **Action:** Pie chart of sales mix.

### FE-H.8-01: Void/Refund Report
**Feature ID:** H.8  **Priority:** P2  **Action:** List security exceptions.

### FE-H.9-01: Labor Cost Report
**Feature ID:** H.9  **Priority:** P2  **Action:** Labor % vs Sales.

### FE-H.10-01: Inventory Velocity Report
**Feature ID:** H.10 **Priority:** P3  **Action:** Fast/Slow movers list.

### FE-H.11-01: Customer Analytics Dashboard
**Feature ID:** H.11 **Priority:** P3  **Action:** Top customers, frequency.

### FE-H.12-01: Peak Hour Analysis
**Feature ID:** H.12 **Priority:** P3  **Action:** Heatmap of busy times.

### FE-H.13-01: Export Manager UI
**Feature ID:** H.13 **Priority:** P2  **Action:** Configurable PDF/Excel exports.

### FE-H.14-01: Automated Email Reports
**Feature ID:** H.14 **Priority:** P3  **Action:** Schedule report delivery.

### FE-H.15-01: Dashboard Customization
**Feature ID:** H.15 **Priority:** P3  **Action:** Widgets layout editor.

---

## Missing Tickets: Category I (Hardware)

### FE-I.1-01: Terminal Configuration
**Feature ID:** I.1  **Priority:** P1  **Action:** Identify terminal ID/Name.

### FE-I.2-01: Printer Routing UI
**Feature ID:** I.2  **Priority:** P1  **Action:** Map categories to printers.

### FE-I.3-01: Cash Drawer Settings
**Feature ID:** I.3  **Priority:** P1  **Action:** Trigger drawer kick test.

### FE-I.5-01: Customer Display Config
**Feature ID:** I.5  **Priority:** P2  **Action:** Customize secondary screen content.

### FE-I.6-01: Card Reader Setup
**Feature ID:** I.6  **Priority:** P1  **Action:** Pair/Test payment terminal.

### FE-I.7-01: Barcode Scanner Test
**Feature ID:** I.7  **Priority:** P2  **Action:** Verify scanner input mode.

### FE-I.8-01: Scale Integration UI
**Feature ID:** I.8  **Priority:** P3  **Action:** Calibrate/Test weighted items.

### FE-I.9-01: Kitchen Display System (KDS) View
**Feature ID:** I.9  **Priority:** P2  **Action:** Digital order board view.

### FE-I.10-01: Hardware Status Dashboard
**Feature ID:** I.10 **Priority:** P2  **Action:** Green/Red status for all devices.

### FE-I.11-01: Offline Mode Indicator
**Feature ID:** I.11 **Priority:** P1  **Action:** Warning banner when disconnected.

---

## Missing Tickets: Category K (Localization)

### FE-K.1-01: Language Support UI
**Feature ID:** K.1  **Priority:** P2  **Action:** Language picker and resource loading.

### FE-K.2-01: Regional Format Settings
**Feature ID:** K.2  **Priority:** P2  **Action:** Date/Time/Number format override.

### FE-K.5-01: RTL Layout Support
**Feature ID:** K.5  **Priority:** P3  **Action:** Test/Enable Right-To-Left layout.

### FE-K.6-01: Legal Disclaimer Config
**Feature ID:** K.6  **Priority:** P2  **Action:** Footer text editor for receipts.

---

## Missing Tickets: Category L (Operations)

### FE-L.1-01: Installation Wizard
**Feature ID:** L.1  **Priority:** P1  **Action:** First-run setup guide.

### FE-L.2-01: Licensing/Activation UI
**Feature ID:** L.2  **Priority:** P0  **Action:** Input license key and validate.

### FE-L.4-01: Auto-Update Settings
**Feature ID:** L.4  **Priority:** P2  **Action:** Configure update checks/times.

### FE-L.5-01: Database Connection Manager
**Feature ID:** L.5  **Priority:** P1  **Action:** Edit connection string/test db.

### FE-L.6-01: Backup & Restore UI
**Feature ID:** L.6  **Priority:** P1  **Action:** Button to backup/restore DB.

### FE-L.7-01: Log Viewer (System)
**Feature ID:** L.7  **Priority:** P2  **Action:** View application error logs.

### FE-L.8-01: Remote Support Access
**Feature ID:** L.8  **Priority:** P3  **Action:** Enable remote tunnel (optional).

### FE-L.9-01: Training Mode Toggle
**Feature ID:** L.9  **Priority:** P2  **Action:** Sandbox mode visual indicator.

### FE-L.10-01: Feedback Submission
**Feature ID:** L.10 **Priority:** P3  **Action:** Send bug report to dev.

### FE-L.11-01: Theme Editor
**Feature ID:** L.11 **Priority:** P3  **Action:** Customize accent colors.

### FE-L.12-01: Tablet/Mobile Layout
**Feature ID:** L.12 **Priority:** P2  **Action:** Responsive layout variants.

---

## Missing Tickets: Category M (System Safety)

### FE-M.1-01: Data Integrity Check UI
**Feature ID:** M.1  **Priority:** P2  **Action:** Run DB consistency check.

### FE-M.2-01: Crash Recovery Dialog
**Feature ID:** M.2  **Priority:** P1  **Action:** "Restore previous session?" prompt.

### FE-M.3-01: Network Diagnostics
**Feature ID:** M.3  **Priority:** P2  **Action:** Ping test/Latency graph.

### FE-M.4-01: Local Cache Management
**Feature ID:** M.4  **Priority:** P2  **Action:** Clear/View local storage.

### FE-M.5-01: Peripheral Health Check
**Feature ID:** M.5  **Priority:** P2  **Action:** Self-test all hardware.

### FE-M.6-01: Emergency Mode Toggle
**Feature ID:** M.6  **Priority:** P3  **Action:** Simplified UI for degraded state.

### FE-M.7-01: Secure Wipe Utility
**Feature ID:** M.7  **Priority:** P3  **Action:** Reset factory defaults.

### FE-M.8-01: Automated Testing Suite UI
**Feature ID:** M.8  **Priority:** P3  **Action:** Run built-in diagnostics.

### FE-M.9-01: Performance Monitor
**Feature ID:** M.9  **Priority:** P3  **Action:** CPU/Memory usage overlay.

### FE-M.10-01: Configuration Export/Import
**Feature ID:** M.10 **Priority:** P2  **Action:** Transfer settings between tills.

### FE-M.11-01: Version Compatibility Check
**Feature ID:** M.11 **Priority:** P2  **Action:** Verify DB vs App version.

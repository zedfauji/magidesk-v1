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

## Summary - All Consolidated Frontend Tickets

| Category | Tickets | Priority Distribution |
|----------|---------|----------------------|
| B | 3 | P1: 1, P2: 2 |
| C | 6 | P0: 1, P1: 5 |
| G | 3 | P1: 3 |
| H | 6 | P1: 5, P2: 1 |
| I | 1 | P1: 1 |
| J | 3 | P0: 2, P1: 1 |
| K | 2 | P2: 2 |
| L | 1 | P1: 1 |
| **Total** | **25** | **P0: 3, P1: 17, P2: 5** |

---

*Last Updated: 2026-01-10*

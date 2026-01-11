# Frontend Tickets: Category A - Table & Game Management

> [!CAUTION]
> **CRITICAL P0**: These UI components are essential for the core billing workflow. Without them, time-based billing is unusable.

## Ticket Index

| Ticket ID | Feature ID | Title | Priority | Status |
|-----------|------------|-------|----------|--------|
| FE-A.1-01 | A.1 | Create StartSessionDialog | P0 | NOT_STARTED |
| FE-A.1-02 | A.1 | Add Session Timer to Table Card | P0 | NOT_STARTED |
| FE-A.2-01 | A.2 | Create EndSessionDialog | P0 | NOT_STARTED |
| FE-A.3-01 | A.3 | Create Active Sessions Panel | P0 | NOT_STARTED |
| FE-A.4-01 | A.4 | Add Status Indicators to Table Cards | P0 | NOT_STARTED |
| FE-A.5-01 | A.5 | Create TableType Management Page | P1 | NOT_STARTED |
| FE-A.9-01 | A.9 | Create Pricing Configuration Dialog | P1 | NOT_STARTED |
| FE-A.16-01 | A.16 | Add Pause/Resume Controls | P0 | NOT_STARTED |
| FE-A.17-01 | A.17 | Create Time Adjustment Dialog | P1 | NOT_STARTED |
| FE-A.19-01 | A.19 | Add Guest Count Input | P1 | NOT_STARTED |

---

## FE-A.1-01: Create StartSessionDialog

**Ticket ID:** FE-A.1-01  
**Feature ID:** A.1  
**Type:** Frontend  
**Title:** Create StartSessionDialog  
**Priority:** P0

### Outcome (measurable, testable)
A ContentDialog that allows operators to start a table session with customer and guest count input.

### Scope
- Create `StartSessionDialog.xaml`
- Create `StartSessionDialogViewModel.cs`
- Customer search/selection (optional)
- Guest count input
- Submit calls `StartTableSessionCommand`

### Explicitly Out of Scope
- Reservation conversion (separate dialog)
- Pre-payment collection

### Implementation Notes
```xml
<!-- StartSessionDialog.xaml -->
<ContentDialog Title="Start Table Session">
    <StackPanel>
        <!-- Table Info (read-only) -->
        <TextBlock Text="{x:Bind ViewModel.TableName}" />
        <TextBlock Text="{x:Bind ViewModel.TableType}" />
        <TextBlock Text="{x:Bind ViewModel.HourlyRate, Converter={StaticResource CurrencyConverter}}" />
        
        <!-- Customer Search (optional) -->
        <AutoSuggestBox 
            PlaceholderText="Search customer (optional)"
            QuerySubmitted="{x:Bind ViewModel.SearchCustomerCommand}" />
        
        <!-- Guest Count -->
        <NumberBox
            Header="Number of Guests"
            Value="{x:Bind ViewModel.GuestCount, Mode=TwoWay}"
            Minimum="1" Maximum="20" />
    </StackPanel>
</ContentDialog>
```

### Quality & Guardrails
- **mvvm-pattern.md:** ViewModel calls Application layer, no business logic
- **no-silent-failure.md:** All errors shown to user via dialog
- **exception-handling-contract.md:** Use Result pattern from commands

### Dependencies
| Type | Dependency | Ticket ID |
|------|------------|-----------|
| HARD | StartTableSessionCommand | BE-A.1-02 |
| SOFT | Customer search | BE-F.2-01 |

### Acceptance Criteria
- [ ] Dialog opens from table context menu
- [ ] Shows table info (name, type, rate)
- [ ] Customer search works (if F module ready)
- [ ] Guest count validates (1-20)
- [ ] Start button creates session
- [ ] Error displayed if table already in use
- [ ] Dialog closes on success
- [ ] Localized strings used

---

## FE-A.1-02: Add Session Timer to Table Card

**Ticket ID:** FE-A.1-02  
**Feature ID:** A.1  
**Type:** Frontend  
**Title:** Add Session Timer to Table Card  
**Priority:** P0

### Outcome (measurable, testable)
Live-updating timer displayed on occupied table cards showing elapsed session time.

### Scope
- Add timer display to `TableCard` control
- Show elapsed time in HH:MM:SS format
- Show running charge total
- Update every second

### Implementation Notes
```xml
<!-- Inside TableCard.xaml when Status == InUse -->
<StackPanel Visibility="{x:Bind IsSessionActive}">
    <TextBlock 
        Text="{x:Bind SessionElapsedTime}"
        Style="{StaticResource TimerStyle}"
        FontSize="24" />
    <TextBlock 
        Text="{x:Bind RunningCharge, Converter={StaticResource CurrencyConverter}}"
        Style="{StaticResource RunningChargeStyle}" />
</StackPanel>
```

```csharp
// ViewModel updates via DispatcherTimer
private DispatcherTimer _timer;
public TimeSpan SessionElapsedTime => DateTime.UtcNow - Session.StartTime - Session.TotalPausedDuration;
public Money RunningCharge => _pricingService.CalculateTimeCharge(SessionElapsedTime, TableType);
```

### Acceptance Criteria
- [ ] Timer shows on occupied tables
- [ ] Updates every second
- [ ] Shows elapsed time (HH:MM:SS)
- [ ] Shows running charge
- [ ] Excludes paused time
- [ ] Timer style is prominent and readable
- [ ] Paused indicator shown when paused

---

## FE-A.2-01: Create EndSessionDialog

**Ticket ID:** FE-A.2-01  
**Feature ID:** A.2  
**Type:** Frontend  
**Title:** Create EndSessionDialog  
**Priority:** P0

### Outcome (measurable, testable)
A ContentDialog that ends a session, shows time charges, and proceeds to payment.

### Scope
- Create `EndSessionDialog.xaml`
- Create `EndSessionDialogViewModel.cs`
- Display session summary (time, rate, charge)
- Option to add to existing ticket or create new
- Submit calls `EndTableSessionCommand`

### Implementation Notes
```xml
<ContentDialog Title="End Session">
    <StackPanel>
        <!-- Session Summary -->
        <TextBlock Text="Duration:" />
        <TextBlock Text="{x:Bind ViewModel.Duration}" FontWeight="Bold" />
        
        <TextBlock Text="Hourly Rate:" />
        <TextBlock Text="{x:Bind ViewModel.HourlyRate, Converter={StaticResource CurrencyConverter}}" />
        
        <TextBlock Text="Total Charge:" FontSize="24" />
        <TextBlock Text="{x:Bind ViewModel.TotalCharge, Converter={StaticResource CurrencyConverter}}" 
                   FontSize="32" FontWeight="Bold" />
        
        <!-- Action Options -->
        <RadioButtons>
            <RadioButton Content="Create New Ticket" IsChecked="True" />
            <RadioButton Content="Add to Existing Ticket" />
        </RadioButtons>
    </StackPanel>
</ContentDialog>
```

### Dependencies
| Type | Dependency | Ticket ID |
|------|------------|-----------|
| HARD | EndTableSessionCommand | BE-A.2-01 |
| HARD | PricingService | BE-A.9-01 |

### Acceptance Criteria
- [ ] Shows duration correctly
- [ ] Shows calculated charge
- [ ] Create ticket option works
- [ ] Add to existing ticket works
- [ ] Proceeds to payment on confirm
- [ ] Error handling for failures

---

## FE-A.3-01: Create Active Sessions Panel

**Ticket ID:** FE-A.3-01  
**Feature ID:** A.3  
**Type:** Frontend  
**Title:** Create Active Sessions Panel  
**Priority:** P0

### Outcome (measurable, testable)
A sidebar or panel showing all currently active table sessions across all floors.

### Scope
- Create `ActiveSessionsPanel.xaml`
- List all active sessions
- Show table, time, customer, charge
- Quick actions (end, pause)

### Implementation Notes
```xml
<ListView ItemsSource="{x:Bind ViewModel.ActiveSessions}">
    <ListView.ItemTemplate>
        <DataTemplate x:DataType="local:ActiveSessionItemViewModel">
            <Grid>
                <TextBlock Text="{x:Bind TableName}" />
                <TextBlock Text="{x:Bind ElapsedTime}" />
                <TextBlock Text="{x:Bind CustomerName}" />
                <TextBlock Text="{x:Bind RunningCharge}" />
                <Button Command="{x:Bind EndCommand}" Content="End" />
                <Button Command="{x:Bind PauseCommand}" Content="Pause" />
            </Grid>
        </DataTemplate>
    </ListView.ItemTemplate>
</ListView>
```

### Acceptance Criteria
- [ ] Shows all active sessions
- [ ] Updates in real-time
- [ ] End button works
- [ ] Pause button works
- [ ] Sorted by start time
- [ ] Clicking navigates to table

---

## FE-A.4-01: Add Status Indicators to Table Cards

**Ticket ID:** FE-A.4-01  
**Feature ID:** A.4  
**Type:** Frontend  
**Title:** Add Status Indicators to Table Cards  
**Priority:** P0

### Outcome (measurable, testable)
Clear visual indicators on table cards showing current status.

### Scope
- Update `TableCard` with status-based styling
- Color coding: Available (green), In Use (blue), Reserved (orange), Paused (yellow)
- Status icon overlay
- Tooltip with details

### Implementation Notes
```xml
<Border Background="{x:Bind StatusColor}">
    <Grid>
        <!-- Existing table content -->
        
        <!-- Status Icon Overlay -->
        <FontIcon 
            Glyph="{x:Bind StatusIcon}"
            HorizontalAlignment="Right"
            VerticalAlignment="Top" />
    </Grid>
</Border>
```

### Acceptance Criteria
- [ ] Colors clearly distinguish states
- [ ] Icons match status
- [ ] Tooltip shows status details
- [ ] Accessible contrast ratios
- [ ] Consistent with design system

---

## FE-A.16-01: Add Pause/Resume Controls

**Ticket ID:** FE-A.16-01  
**Feature ID:** A.16  
**Type:** Frontend  
**Title:** Add Pause/Resume Controls  
**Priority:** P0

### Outcome (measurable, testable)
UI controls to pause and resume active sessions.

### Scope
- Add pause button to table context menu
- Add pause button to session panel
- Resume button when paused
- Confirmation dialog for pause

### Implementation Notes
```xml
<!-- Context Menu Item -->
<MenuFlyoutItem 
    Text="Pause Session"
    Icon="Pause"
    Command="{x:Bind ViewModel.PauseSessionCommand}"
    Visibility="{x:Bind ViewModel.CanPause}" />

<MenuFlyoutItem 
    Text="Resume Session"
    Icon="Play"
    Command="{x:Bind ViewModel.ResumeSessionCommand}"
    Visibility="{x:Bind ViewModel.CanResume}" />
```

### Dependencies
| Type | Dependency | Ticket ID |
|------|------------|-----------|
| HARD | PauseTableSessionCommand | BE-A.16-01 |

### Acceptance Criteria
- [ ] Pause button available for active sessions
- [ ] Resume button available for paused sessions
- [ ] Timer stops updating when paused
- [ ] Visual indicator for paused state
- [ ] Confirmation before pause

---

## FE-A.5-01: Create TableType Management Page

**Ticket ID:** FE-A.5-01  
**Feature ID:** A.5  
**Type:** Frontend  
**Title:** Create TableType Management Page  
**Priority:** P1

### Outcome  
Admin page for managing table types and pricing.

### Scope
- Create `TableTypeManagementPage.xaml`
- List all table types
- Add/Edit dialog
- Set pricing rules per type

### Quality & Guardrails
- **mvvm-pattern.md:** No business logic in ViewModel
- **no-silent-failure.md:** All errors shown
- **G13:** Accessibility compliant

### Dependencies
| Type | Dependency | Ticket ID |
|------|------------|-----------|
| HARD | TableType entity | BE-A.5-01 |

### Acceptance Criteria
- [ ] Page loads table types
- [ ] Add dialog works
- [ ] Edit dialog works
- [ ] Pricing rules saved
- [ ] Validation enforced
- [ ] Localized strings used

---

## FE-A.9-01: Create Pricing Configuration Dialog

**Ticket ID:** FE-A.9-01  
**Feature ID:** A.9  
**Type:** Frontend  
**Title:** Create Pricing Configuration Dialog  
**Priority:** P1

### Outcome
Dialog for configuring time-based pricing rules.

### Scope
- Create `PricingConfigDialog.xaml`
- Configure hourly rate
- Configure first-hour rate
- Configure rounding rules
- Configure minimum charges

### Quality & Guardrails
- **mvvm-pattern.md:** ViewModel calls Application layer
- **G13:** Proper form validation

### Dependencies
| Type | Dependency | Ticket ID |
|------|------------|-----------|
| HARD | TableType entity | BE-A.5-01 |

### Acceptance Criteria
- [ ] Dialog opens correctly
- [ ] All pricing fields editable
- [ ] Validation enforced
- [ ] Save persists changes
- [ ] Cancel discards changes

---

## FE-A.17-01: Create Time Adjustment Dialog

**Ticket ID:** FE-A.17-01  
**Feature ID:** A.17  
**Type:** Frontend  
**Title:** Create Time Adjustment Dialog  
**Priority:** P1

### Outcome
Manager dialog for adjusting session time.

### Scope
- Create `TimeAdjustmentDialog.xaml`
- Show current session time
- Allow time adjustment (+ or -)
- Require manager PIN
- Capture adjustment reason

### Quality & Guardrails
- **mvvm-pattern.md:** No business logic in ViewModel
- **G08:** Security - manager auth required

### Dependencies
| Type | Dependency | Ticket ID |
|------|------------|-----------|
| HARD | AdjustSessionTimeCommand | BE-A.17-01 |
| HARD | ManagerPinDialog | FE-J.1-01 |

### Acceptance Criteria
- [ ] Dialog shows current time
- [ ] Adjustment controls work
- [ ] Manager PIN required
- [ ] Reason required
- [ ] Adjustment logged
- [ ] Session time updated

---

## FE-A.19-01: Add Guest Count Input

**Ticket ID:** FE-A.19-01  
**Feature ID:** A.19  
**Type:** Frontend  
**Title:** Add Guest Count Input  
**Priority:** P1

### Outcome
Guest count input in session dialogs.

### Scope
- Add guest count to StartSessionDialog
- Validate range (1-20)
- Display in session panels
- Update guest count control

### Quality & Guardrails
- **G13:** Accessibility - keyboard input

### Dependencies
| Type | Dependency | Ticket ID |
|------|------------|-----------|
| HARD | GuestCount backend | BE-A.19-01 |

### Acceptance Criteria
- [ ] Input shown in start dialog
- [ ] Validation enforces 1-20
- [ ] Default value is 1
- [ ] Guest count displayed in panels
- [ ] Update functionality works

---

## Summary

| Priority | Count | Status |
|----------|-------|--------|
| P0 | 6 | NOT_STARTED |
| P1 | 4 | NOT_STARTED |
| **Total** | **10** | **NOT_STARTED** |

---

*Last Updated: 2026-01-10*

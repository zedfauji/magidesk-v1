# Frontend Tickets: Category E - Reservations & Scheduling

> [!CAUTION]
> **CRITICAL P0**: Entire reservation UI module needs to be created from scratch.

## Ticket Index

| Ticket ID | Feature ID | Title | Priority | Status |
|-----------|------------|-------|----------|--------|
| FE-E.1-01 | E.1 | Create ReservationDialog | P0 | NOT_STARTED |
| FE-E.2-01 | E.2 | Create ReservationCalendarPage | P0 | NOT_STARTED |
| FE-E.2-02 | E.2 | Create ReservationListView | P0 | NOT_STARTED |
| FE-E.3-01 | E.3 | Create EditReservationDialog | P1 | NOT_STARTED |
| FE-E.5-01 | E.5 | Create AvailabilityCheckPanel | P0 | NOT_STARTED |
| FE-E.6-01 | E.6 | Create CheckInReservationDialog | P0 | NOT_STARTED |
| FE-E.9-01 | E.9 | Create ClubScheduleSettingsPage | P2 | NOT_STARTED |

---

## FE-E.2-01: Create ReservationCalendarPage

**Ticket ID:** FE-E.2-01  
**Feature ID:** E.2  
**Type:** Frontend  
**Title:** Create ReservationCalendarPage  
**Priority:** P0

### Outcome (measurable, testable)
A full-page calendar view for managing reservations with day/week/month views.

### Scope
- Create `ReservationCalendarPage.xaml`
- Create `ReservationCalendarViewModel.cs`
- Day, Week, Month view toggles
- Drag-and-drop reservation creation
- Click to view/edit reservation

### Explicitly Out of Scope
- External calendar sync (Google, etc.)
- Online booking integration

### Implementation Notes
```xml
<Page>
    <Grid>
        <!-- Header with Date Navigation and View Toggle -->
        <Grid Row="0">
            <Button Content="&lt;" Command="{x:Bind ViewModel.PreviousCommand}" />
            <TextBlock Text="{x:Bind ViewModel.CurrentDateRange}" />
            <Button Content="&gt;" Command="{x:Bind ViewModel.NextCommand}" />
            
            <StackPanel Orientation="Horizontal">
                <RadioButton Content="Day" IsChecked="{x:Bind ViewModel.IsDayView, Mode=TwoWay}" />
                <RadioButton Content="Week" IsChecked="{x:Bind ViewModel.IsWeekView, Mode=TwoWay}" />
                <RadioButton Content="Month" IsChecked="{x:Bind ViewModel.IsMonthView, Mode=TwoWay}" />
            </StackPanel>
            
            <Button Content="+ New Reservation" Command="{x:Bind ViewModel.NewReservationCommand}" />
        </Grid>
        
        <!-- Calendar Grid -->
        <controls:CalendarGrid 
            Row="1"
            Reservations="{x:Bind ViewModel.Reservations}"
            Tables="{x:Bind ViewModel.Tables}"
            SelectedDate="{x:Bind ViewModel.SelectedDate, Mode=TwoWay}"
            ReservationClicked="{x:Bind ViewModel.OnReservationClicked}" />
    </Grid>
</Page>
```

### Quality & Guardrails
- **mvvm-pattern.md:** All data binding through ViewModel
- **no-silent-failure.md:** Loading errors surfaced
- **code-quality.md:** Custom control for calendar grid

### Dependencies
| Type | Dependency | Ticket ID |
|------|------------|-----------|
| HARD | GetReservationsQuery | BE-E.2-01 |

### Acceptance Criteria
- [ ] Day view shows hourly slots
- [ ] Week view shows all 7 days
- [ ] Month view shows monthly overview
- [ ] Reservations display correctly
- [ ] Click opens reservation details
- [ ] New reservation button works
- [ ] Date navigation works
- [ ] Responsive to window size
- [ ] Localized date formats

---

## FE-E.1-01: Create ReservationDialog

**Ticket ID:** FE-E.1-01  
**Feature ID:** E.1  
**Type:** Frontend  
**Title:** Create ReservationDialog  
**Priority:** P0

### Outcome (measurable, testable)
A ContentDialog for creating new reservations with availability checking.

### Scope
- Create `ReservationDialog.xaml`
- Create `ReservationDialogViewModel.cs`
- Date and time pickers
- Table selection with availability
- Customer info input
- Party size selection

### Implementation Notes
```xml
<ContentDialog Title="New Reservation" PrimaryButtonText="Create" SecondaryButtonText="Cancel">
    <StackPanel Spacing="12">
        <!-- Date Selection -->
        <CalendarDatePicker 
            Header="Date"
            Date="{x:Bind ViewModel.SelectedDate, Mode=TwoWay}" />
        
        <!-- Time Selection -->
        <Grid ColumnDefinitions="*,*">
            <TimePicker Header="Start Time" Time="{x:Bind ViewModel.StartTime, Mode=TwoWay}" />
            <TimePicker Header="End Time" Time="{x:Bind ViewModel.EndTime, Mode=TwoWay}" Grid.Column="1" />
        </Grid>
        
        <!-- Table Selection -->
        <ComboBox 
            Header="Table"
            ItemsSource="{x:Bind ViewModel.AvailableTables}"
            SelectedItem="{x:Bind ViewModel.SelectedTable, Mode=TwoWay}">
            <ComboBox.ItemTemplate>
                <DataTemplate>
                    <StackPanel Orientation="Horizontal">
                        <TextBlock Text="{Binding Name}" />
                        <TextBlock Text="{Binding TypeName}" Opacity="0.6" Margin="8,0,0,0" />
                    </StackPanel>
                </DataTemplate>
            </ComboBox.ItemTemplate>
        </ComboBox>
        
        <!-- Customer Info -->
        <TextBox Header="Customer Name" Text="{x:Bind ViewModel.CustomerName, Mode=TwoWay}" />
        <TextBox Header="Phone" Text="{x:Bind ViewModel.CustomerPhone, Mode=TwoWay}" />
        
        <!-- Party Size -->
        <NumberBox Header="Party Size" Value="{x:Bind ViewModel.PartySize, Mode=TwoWay}" Minimum="1" Maximum="20" />
        
        <!-- Notes -->
        <TextBox Header="Notes" Text="{x:Bind ViewModel.Notes, Mode=TwoWay}" AcceptsReturn="True" />
        
        <!-- Availability Indicator -->
        <InfoBar 
            IsOpen="{x:Bind ViewModel.HasConflict}"
            Severity="Warning"
            Title="Conflict"
            Message="{x:Bind ViewModel.ConflictMessage}" />
    </StackPanel>
</ContentDialog>
```

### Dependencies
| Type | Dependency | Ticket ID |
|------|------------|-----------|
| HARD | CreateReservationCommand | BE-E.1-02 |
| HARD | AvailabilityService | BE-E.5-01 |

### Acceptance Criteria
- [ ] All fields capture input correctly
- [ ] Date/time pickers work
- [ ] Table dropdown shows available tables
- [ ] Conflict warning shows when overlap
- [ ] Create succeeds with valid data
- [ ] Error shown on failure
- [ ] Localized labels

---

## FE-E.5-01: Create AvailabilityCheckPanel

**Ticket ID:** FE-E.5-01  
**Feature ID:** E.5  
**Type:** Frontend  
**Title:** Create AvailabilityCheckPanel  
**Priority:** P0

### Outcome (measurable, testable)
A panel/control that shows table availability for a given date/time.

### Scope
- Create `AvailabilityPanel.xaml` UserControl
- Query availability on date/time change
- Show available/unavailable tables
- Filter by table type

### Implementation Notes
```xml
<UserControl>
    <Grid>
        <!-- Date/Time Selection -->
        <StackPanel Orientation="Horizontal">
            <CalendarDatePicker Date="{x:Bind ViewModel.Date, Mode=TwoWay}" />
            <TimePicker Time="{x:Bind ViewModel.StartTime, Mode=TwoWay}" />
            <TimePicker Time="{x:Bind ViewModel.EndTime, Mode=TwoWay}" />
            <Button Content="Check" Command="{x:Bind ViewModel.CheckAvailabilityCommand}" />
        </StackPanel>
        
        <!-- Results -->
        <GridView ItemsSource="{x:Bind ViewModel.Tables}">
            <GridView.ItemTemplate>
                <DataTemplate>
                    <Border 
                        Background="{Binding IsAvailable, Converter={StaticResource AvailabilityColorConverter}}"
                        Padding="12">
                        <StackPanel>
                            <TextBlock Text="{Binding Name}" FontWeight="Bold" />
                            <TextBlock Text="{Binding Status}" />
                        </StackPanel>
                    </Border>
                </DataTemplate>
            </GridView.ItemTemplate>
        </GridView>
    </Grid>
</UserControl>
```

### Dependencies
| Type | Dependency | Ticket ID |
|------|------------|-----------|
| HARD | AvailabilityService | BE-E.5-01 |

### Acceptance Criteria
- [ ] Date/time selection works
- [ ] Available tables shown green
- [ ] Unavailable tables shown red/grey
- [ ] Shows reason for unavailability
- [ ] Click on available table selects it

---

## FE-E.6-01: Create CheckInReservationDialog

**Ticket ID:** FE-E.6-01  
**Feature ID:** E.6  
**Type:** Frontend  
**Title:** Create CheckInReservationDialog  
**Priority:** P0

### Outcome (measurable, testable)
A dialog for checking in a reservation and starting a table session.

### Scope
- Create `CheckInReservationDialog.xaml`
- Show reservation details
- Confirm check-in action
- Start session automatically

### Implementation Notes
```xml
<ContentDialog Title="Check In Reservation">
    <StackPanel>
        <TextBlock Text="{x:Bind ViewModel.CustomerName}" FontSize="20" FontWeight="Bold" />
        <TextBlock Text="{x:Bind ViewModel.TableName}" />
        <TextBlock Text="{x:Bind ViewModel.ReservationTime}" />
        <TextBlock Text="{x:Bind ViewModel.PartySize} guests" />
        
        <InfoBar 
            IsOpen="{x:Bind ViewModel.IsEarly}"
            Severity="Informational"
            Message="Customer is arriving early" />
            
        <InfoBar 
            IsOpen="{x:Bind ViewModel.IsLate}"
            Severity="Warning"
            Message="Customer is arriving late" />
    </StackPanel>
</ContentDialog>
```

### Dependencies
| Type | Dependency | Ticket ID |
|------|------------|-----------|
| HARD | ConvertReservationToSessionCommand | BE-E.6-01 |

### Acceptance Criteria
- [ ] Shows reservation details
- [ ] Indicates early/on-time/late arrival
- [ ] Check-in starts session
- [ ] Session linked to reservation
- [ ] Customer linked to session

---

## FE-E.2-02: Create ReservationListView

**Ticket ID:** FE-E.2-02  
**Feature ID:** E.2  
**Type:** Frontend  
**Title:** Create ReservationListView  
**Priority:** P0

### Outcome
List view of reservations for quick search and filtering.

### Scope
- Create `ReservationListView` control
- Search and filter functionality
- Sort by date, customer, table
- Quick actions (edit, cancel, check-in)

### Quality & Guardrails
- **mvvm-pattern.md:** ViewModel only
- **G13:** Accessibility compliant

### Dependencies
| Type | Dependency | Ticket ID |
|------|------------|-----------|
| HARD | GetReservationsQuery | BE-E.2-01 |

### Acceptance Criteria
- [ ] List displays all reservations
- [ ] Search works
- [ ] Filtering functional
- [ ] Sorting works
- [ ] Quick actions available
- [ ] Performance acceptable

---

## FE-E.3-01: Create EditReservationDialog

**Ticket ID:** FE-E.3-01  
**Feature ID:** E.3  
**Type:** Frontend  
**Title:** Create EditReservationDialog  
**Priority:** P1

### Outcome
Dialog for editing existing reservations.

### Scope
- Create `EditReservationDialog.xaml`
- Pre-populate existing data
- Support all fields from create
- Conflict detection on save

### Quality & Guardrails
- **mvvm-pattern.md:** No business logic in ViewModel
- **G01:** No silent failures

### Dependencies
| Type | Dependency | Ticket ID |
|------|------------|-----------|
| HARD | UpdateReservationCommand | BE-E.3-01 |
| SOFT | ReservationDialog | FE-E.1-01 |

### Acceptance Criteria
- [ ] Dialog loads reservation data
- [ ] All fields editable
- [ ] Conflict detection works
- [ ] Save updates reservation
- [ ] Error handling complete

---

## FE-E.9-01: Create ClubScheduleSettingsPage

**Ticket ID:** FE-E.9-01  
**Feature ID:** E.9  
**Type:** Frontend  
**Title:** Create ClubScheduleSettingsPage  
**Priority:** P2

### Outcome
Admin page for setting club operating hours.

### Scope
- Create `ClubScheduleSettingsPage.xaml`
- Set daily operating hours
- Set holiday closures
- Blackout periods configuration

### Quality & Guardrails
- **mvvm-pattern.md:** ViewModel pattern
- **G13:** Form validation

### Dependencies
| Type | Dependency | Ticket ID |
|------|------------|-----------|
| HARD | ClubSchedule entity | BE-E.9-01 |

### Acceptance Criteria
- [ ] Page loads schedule
- [ ] Hours editable per day
- [ ] Holiday dates work
- [ ] Save persists changes
- [ ] Validation enforced

---

## FE-E.10-01: Create RecurringReservationDialog

**Ticket ID:** FE-E.10-01  
**Feature ID:** E.10  
**Type:** Frontend  
**Title:** Create RecurringReservationDialog  
**Priority:** P2

### Outcome
Dialog for creating recurring reservations.

### Scope
- Extend ReservationDialog for recurrence
- Weekly pattern selection
- End date configuration
- Preview before creation

### Quality & Guardrails
- **mvvm-pattern.md:** ViewModel pattern
- **G01:** Show conflicts clearly

### Dependencies
| Type | Dependency | Ticket ID |
|------|------------|-----------|
| HARD | Recurring reservations BE | BE-E.10-01 |

### Acceptance Criteria
- [ ] Recurrence pattern configurable
- [ ] Preview shown
- [ ] Conflicts detected
- [ ] Batch creation works
- [ ] Error handling complete

---

## FE-E.11-01: Create ReminderSettingsDialog

**Ticket ID:** FE-E.11-01  
**Feature ID:** E.11  
**Type:** Frontend  
**Title:** Create ReminderSettingsDialog  
**Priority:** P2

### Outcome
Configure automated reservation reminders.

### Scope
- Create reminder settings dialog
- Configure reminder timing
- Message template editing
- Test reminder functionality

### Quality & Guardrails
- **mvvm-pattern.md:** ViewModel pattern

### Dependencies
| Type | Dependency | Ticket ID |
|------|------------|-----------|
| HARD | Reminder service | BE-E.11-01 |

### Acceptance Criteria
- [ ] Settings configurable
- [ ] Template editable
- [ ] Test send works
- [ ] Save persists settings

---

## FE-E.12-01: Create WaitingListPanel

**Ticket ID:** FE-E.12-01  
**Feature ID:** E.12  
**Type:** Frontend  
**Title:** Create WaitingListPanel  
**Priority:** P2

### Outcome
Panel for managing waiting list for tables.

### Scope
- Create `WaitingListPanel` control
- Add customer to waiting list
- Notify when table available
- Remove from list

### Quality & Guardrails
- **mvvm-pattern.md:** ViewModel pattern
- **G13:** Accessible notifications

### Dependencies
| Type | Dependency | Ticket ID |
|------|------------|-----------|
| HARD | Waiting list BE | BE-E.12-01 |

### Acceptance Criteria
- [ ] Add to list works
- [ ] Notify functionality works
- [ ] Remove from list works
- [ ] Priority ordering functional

---

## Summary

| Priority | Count | Status |
|----------|-------|--------|
| P0 | 5 | NOT_STARTED |
| P1 | 1 | NOT_STARTED |
| P2 | 4 | NOT_STARTED |
| **Total** | **10** | **NOT_STARTED** |

---

*Last Updated: 2026-01-10*

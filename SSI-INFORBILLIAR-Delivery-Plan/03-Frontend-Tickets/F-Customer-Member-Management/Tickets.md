# Frontend Tickets: Category F - Customer & Member Management

> [!NOTE]
> **Implementation Started**: Features F.1 and F.2 are complete. Customer list and search functionality are fully implemented.

## Ticket Index

| Ticket ID | Feature ID | Title | Priority | Status |
|-----------|------------|-------|----------|--------|
| FE-F.1-01 | F.1 | Create CustomerListPage | P0 | COMPLETE |
| FE-F.1-02 | F.1 | Create AddCustomerDialog | P0 | COMPLETE |
| FE-F.2-01 | F.2 | Create CustomerSearchControl | P0 | COMPLETE |
| FE-F.3-01 | F.3 | Create MemberProfilePage | P0 | NOT_STARTED |
| FE-F.4-01 | F.4 | Create MembershipTierManagementPage | P1 | NOT_STARTED |
| FE-F.6-01 | F.6 | Create PrepaidBalancePanel | P1 | NOT_STARTED |
| FE-F.7-01 | F.7 | Create CustomerHistoryTab | P1 | NOT_STARTED |
| FE-F.10-01 | F.10 | Create MemberCheckInDialog | P1 | NOT_STARTED |

---

## FE-F.1-01: Create CustomerListPage

**Ticket ID:** FE-F.1-01  
**Feature ID:** F.1  
**Type:** Frontend  
**Title:** Create CustomerListPage  
**Priority:** P0

### Outcome (measurable, testable)
A full page for viewing, searching, and managing all customers.

### Scope
- Create `CustomerListPage.xaml`
- Create `CustomerListViewModel.cs`
- Search/filter functionality
- Add new customer button
- View customer details
- Member indicator

### Implementation Notes
```xml
<Page>
    <Grid>
        <!-- Header -->
        <Grid Row="0">
            <TextBlock Text="Customers" Style="{StaticResource TitleStyle}" />
            <AutoSuggestBox 
                PlaceholderText="Search by name, phone, email..."
                QuerySubmitted="{x:Bind ViewModel.SearchCommand}" />
            <Button Content="+ Add Customer" Command="{x:Bind ViewModel.AddCustomerCommand}" />
        </Grid>
        
        <!-- Filter Tabs -->
        <NavigationView Row="1" PaneDisplayMode="Top">
            <NavigationViewItem Content="All" Tag="All" />
            <NavigationViewItem Content="Members" Tag="Members" />
            <NavigationViewItem Content="Non-Members" Tag="NonMembers" />
        </NavigationView>
        
        <!-- Customer List -->
        <ListView Row="2" ItemsSource="{x:Bind ViewModel.Customers}">
            <ListView.ItemTemplate>
                <DataTemplate x:DataType="vm:CustomerListItemViewModel">
                    <Grid Padding="12">
                        <Grid.ColumnDefinitions>
                            <ColumnDefinition Width="Auto" />
                            <ColumnDefinition Width="*" />
                            <ColumnDefinition Width="Auto" />
                        </Grid.ColumnDefinitions>
                        
                        <!-- Avatar -->
                        <PersonPicture DisplayName="{x:Bind FullName}" Width="48" />
                        
                        <!-- Info -->
                        <StackPanel Grid.Column="1" Margin="12,0">
                            <TextBlock Text="{x:Bind FullName}" FontWeight="SemiBold" />
                            <TextBlock Text="{x:Bind Phone}" Opacity="0.7" />
                            <TextBlock Text="{x:Bind Email}" Opacity="0.7" />
                        </StackPanel>
                        
                        <!-- Member Badge -->
                        <Border Grid.Column="2" 
                                Background="{x:Bind TierColor}"
                                CornerRadius="4"
                                Padding="8,4"
                                Visibility="{x:Bind IsMember}">
                            <TextBlock Text="{x:Bind TierName}" />
                        </Border>
                    </Grid>
                </DataTemplate>
            </ListView.ItemTemplate>
        </ListView>
    </Grid>
</Page>
```

### Acceptance Criteria
- [ ] Lists all customers
- [ ] Search works on name, phone, email
- [ ] Filter tabs work
- [ ] Member badge visible
- [ ] Click opens customer details
- [ ] Add button opens dialog
- [ ] Pagination for large lists

---

## FE-F.2-01: Create CustomerSearchControl

**Ticket ID:** FE-F.2-01  
**Feature ID:** F.2  
**Type:** Frontend  
**Title:** Create CustomerSearchControl  
**Priority:** P0

### Outcome (measurable, testable)
A reusable search control for quick customer lookup in dialogs and POS screens.

### Scope
- Create `CustomerSearchControl.xaml` UserControl
- Fast fuzzy search
- Display customer + member info
- Selection event for parent use

### Implementation Notes
```xml
<UserControl>
    <StackPanel>
        <AutoSuggestBox
            PlaceholderText="Search customer..."
            Text="{x:Bind ViewModel.SearchText, Mode=TwoWay}"
            TextChanged="{x:Bind ViewModel.OnSearchTextChanged}"
            SuggestionChosen="{x:Bind ViewModel.OnSuggestionChosen}">
            <AutoSuggestBox.ItemTemplate>
                <DataTemplate x:DataType="vm:CustomerSearchResultViewModel">
                    <Grid>
                        <StackPanel>
                            <TextBlock Text="{x:Bind FullName}" FontWeight="SemiBold" />
                            <TextBlock Text="{x:Bind Phone}" Opacity="0.7" />
                        </StackPanel>
                        <Border Visibility="{x:Bind IsMember}" HorizontalAlignment="Right">
                            <TextBlock Text="{x:Bind TierName}" Foreground="{x:Bind TierColor}" />
                        </Border>
                    </Grid>
                </DataTemplate>
            </AutoSuggestBox.ItemTemplate>
        </AutoSuggestBox>
        
        <!-- Selected Customer Display -->
        <Border 
            Visibility="{x:Bind ViewModel.HasSelectedCustomer}"
            Background="{ThemeResource ControlFillColorDefaultBrush}"
            CornerRadius="4" Padding="12">
            <Grid>
                <StackPanel>
                    <TextBlock Text="{x:Bind ViewModel.SelectedCustomer.FullName}" FontWeight="Bold" />
                    <TextBlock Text="{x:Bind ViewModel.SelectedCustomer.Phone}" />
                </StackPanel>
                <Button Content="Clear" Command="{x:Bind ViewModel.ClearSelectionCommand}" HorizontalAlignment="Right" />
            </Grid>
        </Border>
    </StackPanel>
</UserControl>
```

### Dependencies
| Type | Dependency | Ticket ID |
|------|------------|-----------|
| HARD | SearchCustomersQuery | BE-F.2-01 |

### Acceptance Criteria
- [ ] Search triggers after 2+ characters
- [ ] Results appear quickly (<100ms)
- [ ] Shows name, phone, member status
- [ ] Selection populates control
- [ ] Clear button works
- [ ] Reusable in dialogs

---

## FE-F.3-01: Create MemberProfilePage

**Ticket ID:** FE-F.3-01  
**Feature ID:** F.3  
**Type:** Frontend  
**Title:** Create MemberProfilePage  
**Priority:** P0

### Outcome (measurable, testable)
A detailed profile page for member management.

### Scope
- Create `MemberProfilePage.xaml`
- Member info display
- Tier and benefits display
- Prepaid balance panel
- Visit history tab
- Membership status management

### Implementation Notes
```xml
<Page>
    <Grid>
        <!-- Header Section -->
        <Grid Row="0" Padding="24">
            <PersonPicture DisplayName="{x:Bind ViewModel.FullName}" Width="80" />
            <StackPanel Margin="16,0,0,0">
                <TextBlock Text="{x:Bind ViewModel.FullName}" Style="{StaticResource TitleStyle}" />
                <TextBlock Text="{x:Bind ViewModel.MemberNumber}" Opacity="0.7" />
                <Border Background="{x:Bind ViewModel.TierColor}" CornerRadius="4" Padding="12,4">
                    <TextBlock Text="{x:Bind ViewModel.TierName}" />
                </Border>
            </StackPanel>
            
            <!-- Quick Actions -->
            <StackPanel Orientation="Horizontal" HorizontalAlignment="Right">
                <Button Content="Check In" Command="{x:Bind ViewModel.CheckInCommand}" />
                <Button Content="Add Credit" Command="{x:Bind ViewModel.AddCreditCommand}" />
                <Button Content="Edit" Command="{x:Bind ViewModel.EditCommand}" />
            </StackPanel>
        </Grid>
        
        <!-- Content Tabs -->
        <Pivot Row="1">
            <PivotItem Header="Overview">
                <!-- Contact info, stats, benefits -->
            </PivotItem>
            <PivotItem Header="History">
                <!-- Visit and purchase history -->
            </PivotItem>
            <PivotItem Header="Balance">
                <!-- Prepaid balance and transactions -->
            </PivotItem>
            <PivotItem Header="Reservations">
                <!-- Upcoming and past reservations -->
            </PivotItem>
        </Pivot>
    </Grid>
</Page>
```

### Acceptance Criteria
- [ ] Shows all member info
- [ ] Tier benefits displayed
- [ ] Prepaid balance visible
- [ ] History tab works
- [ ] Quick actions work
- [ ] Edit opens dialog

---

## FE-F.10-01: Create MemberCheckInDialog

**Ticket ID:** FE-F.10-01  
**Feature ID:** F.10  
**Type:** Frontend  
**Title:** Create MemberCheckInDialog  
**Priority:** P1

### Outcome (measurable, testable)
A quick check-in dialog for members arriving at the club.

### Scope
- Create `MemberCheckInDialog.xaml`
- Member search/scan
- Display member info and balance
- Show today's reservation if exists
- Proceed to start session

### Implementation Notes
```xml
<ContentDialog Title="Member Check-In">
    <StackPanel>
        <!-- Search -->
        <local:CustomerSearchControl 
            SelectedCustomer="{x:Bind ViewModel.Member, Mode=TwoWay}" />
        
        <!-- Member Info Display (after selection) -->
        <StackPanel Visibility="{x:Bind ViewModel.HasMember}">
            <TextBlock Text="{x:Bind ViewModel.MemberName}" FontSize="24" FontWeight="Bold" />
            <Border Background="{x:Bind ViewModel.TierColor}" CornerRadius="4" Padding="8,4">
                <TextBlock Text="{x:Bind ViewModel.TierName}" />
            </Border>
            
            <Grid>
                <TextBlock Text="Prepaid Balance:" />
                <TextBlock Text="{x:Bind ViewModel.PrepaidBalance}" HorizontalAlignment="Right" FontWeight="Bold" />
            </Grid>
            
            <Grid>
                <TextBlock Text="Discount:" />
                <TextBlock Text="{x:Bind ViewModel.DiscountPercent, StringFormat='{0}%'}" HorizontalAlignment="Right" />
            </Grid>
            
            <!-- Today's Reservation -->
            <InfoBar 
                IsOpen="{x:Bind ViewModel.HasReservation}"
                Severity="Success"
                Title="Reservation Found"
                Message="{x:Bind ViewModel.ReservationDetails}" />
        </StackPanel>
    </StackPanel>
</ContentDialog>
```

### Dependencies
| Type | Dependency | Ticket ID |
|------|------------|-----------|
| HARD | CheckInMemberCommand | BE-F.10-01 |
| SOFT | CustomerSearchControl | FE-F.2-01 |

### Acceptance Criteria
- [ ] Search finds member
- [ ] Member info displayed
- [ ] Balance shown
- [ ] Reservation detected
- [ ] Check-in records visit
- [ ] Proceeds to start session

---

## FE-F.4-01: Create MembershipTierManagementPage

**Ticket ID:** FE-F.4-01  
**Feature ID:** F.4  
**Type:** Frontend  
**Title:** Create MembershipTierManagementPage  
**Priority:** P1

### Outcome
Admin page for managing membership tiers and benefits.

### Scope
- Create `MembershipTierManagementPage.xaml`
- List all membership tiers
- Add/Edit tier dialog
- Configure discount percentages
- Define tier benefits

### Quality & Guardrails
- **mvvm-pattern.md:** No logic in ViewModel
- **G13:** Accessibility compliant

### Dependencies
| Type | Dependency | Ticket ID |
|------|------------|-----------|
| HARD | MembershipTier entity | BE-F.4-01 |

### Acceptance Criteria
- [ ] Page lists all tiers
- [ ] Add tier works
- [ ] Edit tier works
- [ ] Discount configuration works
- [ ] Benefits defined
- [ ] Validation enforced

---

## FE-F.6-01: Create PrepaidBalancePanel

**Ticket ID:** FE-F.6-01  
**Feature ID:** F.6  
**Type:** Frontend  
**Title:** Create PrepaidBalancePanel  
**Priority:** P1

### Outcome
Panel for managing member prepaid credits.

### Scope
- Create `PrepaidBalancePanel` control
- Display current balance
- Add credit dialog
- Transaction history
- Balance alerts

### Quality & Guardrails
- **mvvm-pattern.md:** ViewModel pattern
- **G13:** Form validation

### Dependencies
| Type | Dependency | Ticket ID |
|------|------------|-----------|
| HARD | Prepaid balance BE | BE-F.6-01 |

### Acceptance Criteria
- [ ] Balance displayed correctly
- [ ] Add credit works
- [ ] Transaction history shown
- [ ] Low balance alerts work

---

## FE-F.7-01: Create CustomerHistoryTab

**Ticket ID:** FE-F.7-01  
**Feature ID:** F.7  
**Type:** Frontend  
**Title:** Create CustomerHistoryTab  
**Priority:** P1

### Outcome
Tab showing customer visit and purchase history.

### Scope
- Create `CustomerHistoryTab` control
- List visit history
- Show purchase history
- Revenue statistics
- Filter by date range

### Quality & Guardrails
- **mvvm-pattern.md:** ViewModel pattern

### Dependencies
| Type | Dependency | Ticket ID |
|------|------------|-----------|
| HARD | Customer history BE | BE-F.7-01 |

### Acceptance Criteria
- [ ] Visit history displayed
- [ ] Purchase history displayed
- [ ] Stats calculated
- [ ] Filtering works
- [ ] Performance acceptable

---

## Summary

| Priority | Count | Status |
|----------|-------|--------|
| P0 | 1 | NOT_STARTED |
| P0 | 3 | COMPLETE |
| P1 | 4 | NOT_STARTED |
| **Total** | **8** | **3 COMPLETE** |

---

*Last Updated: 2026-01-10*

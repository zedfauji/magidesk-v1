# Frontend Tickets: UI Polish and Optimization

> **Category**: UI/UX Enhancement  
> **Priority**: P1 (High Priority - Competitive Parity)  
> **Total Tickets**: 26

---

## Overview

These tickets implement premium-grade UI polish and optimization for the Magidesk POS system. The work transforms the current functional but basic UI into a professional, touch-optimized, accessible interface that meets modern POS standards.

**Key Deliverables**:
- Redesigned Switchboard as proper navigation hub
- Complete toast notification and feedback system
- Session timer controls for live time tracking
- Manager PIN and confirmation dialogs
- Enhanced table map with interactive features
- Missing critical pages (Login, Reservation Calendar, Customer List, etc.)
- Touch optimization and accessibility compliance
- Visual consistency and performance optimization

---

## Ticket List

### Core UI Components (P1)

#### FE-UI-01: Toast Notification System
**Priority**: P1  
**Effort**: 3 days  
**Dependencies**: None

**Description**: Implement comprehensive toast notification system for user feedback.

**Acceptance Criteria**:
- [ ] ToastNotificationService created with Success/Error/Warning/Info methods
- [ ] ToastNotificationHost UserControl displays notifications in top-right corner
- [ ] Auto-dismiss after 4-8 seconds based on type
- [ ] Manual dismiss with X button
- [ ] Maximum 3 visible toasts with stacking
- [ ] Color-coded by type (green/red/yellow/blue)
- [ ] Property test: Toast auto-dismissal works correctly
- [ ] Property test: Toast stack limit enforced

**Files to Create/Modify**:
- `Services/ToastNotificationService.cs` (new)
- `Controls/ToastNotificationHost.xaml` (new)
- `Controls/ToastNotificationHost.xaml.cs` (new)
- `Models/ToastNotification.cs` (new)

---

#### FE-UI-02: Session Timer Control
**Priority**: P1  
**Effort**: 2 days  
**Dependencies**: None

**Description**: Create live-updating session timer control with threshold-based visual indicators.

**Acceptance Criteria**:
- [ ] SessionTimerControl UserControl created
- [ ] Displays time in HH:MM:SS format (or days for 24+ hours)
- [ ] Updates every second via DispatcherTimer
- [ ] Color changes based on thresholds (green/yellow/red)
- [ ] Shows "PAUSED" state when session paused
- [ ] Property test: Timer accuracy within 1 second tolerance
- [ ] Unit tests for time formatting edge cases

**Files to Create/Modify**:
- `Controls/SessionTimerControl.xaml` (new)
- `Controls/SessionTimerControl.xaml.cs` (new)

---

#### FE-UI-03: Loading Overlay Component
**Priority**: P1  
**Effort**: 2 days  
**Dependencies**: None

**Description**: Implement loading overlay for async operations with optional cancellation.

**Acceptance Criteria**:
- [ ] LoadingOverlay UserControl created
- [ ] Semi-transparent backdrop blocks interaction
- [ ] ProgressRing with descriptive message
- [ ] Optional Cancel button
- [ ] Auto-dismiss on operation complete
- [ ] Timeout handling (max 30 seconds)
- [ ] Property test: Overlay blocks all interactive elements

**Files to Create/Modify**:
- `Controls/LoadingOverlay.xaml` (new)
- `Controls/LoadingOverlay.xaml.cs` (new)
- `Services/LoadingOverlayService.cs` (new)

---

#### FE-UI-04: Manager PIN Dialog
**Priority**: P1  
**Effort**: 3 days  
**Dependencies**: SecurityService, EncryptionService

**Description**: Create manager authentication dialog for privileged operations.

**Acceptance Criteria**:
- [ ] ManagerPinDialog ContentDialog created
- [ ] Numeric keypad (0-9, backspace, confirm)
- [ ] Masked PIN entry (PasswordBox)
- [ ] Operation description display
- [ ] Error message display for invalid PIN
- [ ] Integration with SecurityService for validation
- [ ] Audit logging for all authentication attempts
- [ ] Property test: Valid PIN authorizes, invalid PIN blocks

**Files to Create/Modify**:
- `Views/Dialogs/ManagerPinDialog.xaml` (new)
- `Views/Dialogs/ManagerPinDialog.xaml.cs` (new)
- `Models/ManagerAuthResult.cs` (new)

---

#### FE-UI-05: Confirmation Dialog
**Priority**: P1  
**Effort**: 2 days  
**Dependencies**: None

**Description**: Create confirmation dialog for destructive actions.

**Acceptance Criteria**:
- [ ] ConfirmationDialog ContentDialog created
- [ ] Warning InfoBar at top
- [ ] Detail message and information card
- [ ] Confirm/Cancel buttons with distinct styling
- [ ] Static ShowAsync method for easy invocation
- [ ] Property test: Destructive actions require confirmation

**Files to Create/Modify**:
- `Views/Dialogs/ConfirmationDialog.xaml` (new)
- `Views/Dialogs/ConfirmationDialog.xaml.cs` (new)

---

### Switchboard and Navigation (P1)

#### FE-UI-06: Switchboard Redesign
**Priority**: P1  
**Effort**: 5 days  
**Dependencies**: FE-UI-01 (Toast), KeyboardShortcutService

**Description**: Redesign Switchboard as proper navigation hub with large touch-optimized buttons.

**Acceptance Criteria**:
- [ ] NavigationButton model created
- [ ] SwitchboardViewModel updated with button generation logic
- [ ] Permission-based button visibility
- [ ] Header shows user, terminal, shift status, live counts
- [ ] Large 120x120px buttons grouped by category
- [ ] Segoe Fluent Icons for all buttons
- [ ] Keyboard shortcuts displayed on buttons (F1-F12)
- [ ] Property test: Buttons only enabled with required permissions

**Files to Create/Modify**:
- `ViewModels/SwitchboardViewModel.cs` (modify)
- `Views/SwitchboardPage.xaml` (redesign)
- `Models/NavigationButton.cs` (new)

---

#### FE-UI-07: Keyboard Shortcut Service
**Priority**: P1  
**Effort**: 3 days  
**Dependencies**: None

**Description**: Implement keyboard shortcut registration and handling service.

**Acceptance Criteria**:
- [ ] KeyboardShortcutService created
- [ ] Shortcut registration with conflict detection
- [ ] F1-F12 shortcuts registered for common operations
- [ ] Ctrl+P for Print, Esc for Close Dialog
- [ ] Shortcuts displayed in tooltips
- [ ] Property test: No duplicate key combinations

**Files to Create/Modify**:
- `Services/KeyboardShortcutService.cs` (new)
- `Models/KeyboardShortcut.cs` (new)

---

### Critical Pages (P0)

#### FE-UI-08: Login Page
**Priority**: P0  
**Effort**: 4 days  
**Dependencies**: SecurityService, EncryptionService

**Description**: Create login page with user selection and PIN authentication.

**Acceptance Criteria**:
- [ ] LoginViewModel created
- [ ] LoginPage XAML with user selection grid
- [ ] Large user tiles with photos/avatars
- [ ] Role display under each user
- [ ] PIN entry with numeric keypad
- [ ] Masked PIN display
- [ ] Settings access button
- [ ] Version number display

**Files to Create/Modify**:
- `ViewModels/LoginViewModel.cs` (new)
- `Views/LoginPage.xaml` (new)
- `Views/LoginPage.xaml.cs` (new)

---

#### FE-UI-09: Enhanced Table Map
**Priority**: P1  
**Effort**: 5 days  
**Dependencies**: FE-UI-02 (Session Timer)

**Description**: Add interactive features to table map with session timers and context menus.

**Acceptance Criteria**:
- [ ] EnhancedTableControl UserControl created
- [ ] Context menu on right-click (Start Session, View Details, End Session)
- [ ] SessionTimerControl overlay on occupied tables
- [ ] Color-coded status indicators (green/red/yellow/gray)
- [ ] Hover tooltips with session details
- [ ] Drag-and-drop for server assignment
- [ ] Property test: Touch feedback within 100ms

**Files to Create/Modify**:
- `Controls/EnhancedTableControl.xaml` (new)
- `Controls/EnhancedTableControl.xaml.cs` (new)
- `ViewModels/TableMapViewModel.cs` (modify)
- `Views/TableMapPage.xaml` (modify)

---

#### FE-UI-10: Reservation Calendar Page
**Priority**: P0  
**Effort**: 8 days  
**Dependencies**: Reservation backend (BE-E.*)

**Description**: Create reservation calendar with day/week/month views and drag-to-create functionality.

**Acceptance Criteria**:
- [ ] ReservationCalendarViewModel created
- [ ] ReservationCalendarPage XAML with time-slot grid
- [ ] Day/Week/Month view toggle
- [ ] Reservation blocks with customer name and party size
- [ ] Drag-to-create new reservations
- [ ] Drag-to-resize reservation duration
- [ ] Color-coded status (available/reserved/occupied/conflict)
- [ ] Conflict detection and highlighting

**Files to Create/Modify**:
- `ViewModels/ReservationCalendarViewModel.cs` (new)
- `Views/ReservationCalendarPage.xaml` (new)
- `Views/ReservationCalendarPage.xaml.cs` (new)

---

#### FE-UI-11: Customer List Page
**Priority**: P0  
**Effort**: 5 days  
**Dependencies**: Customer backend (BE-F.1, BE-F.2)

**Description**: Create customer list page with search, filter, and CRUD operations.

**Acceptance Criteria**:
- [ ] CustomerListViewModel created
- [ ] CustomerListPage XAML with DataGrid/ListView
- [ ] Search bar with live filtering
- [ ] Filter dropdowns (status, membership, etc.)
- [ ] Action buttons (New, Edit, Delete)
- [ ] Customer detail panel
- [ ] Pagination support

**Files to Create/Modify**:
- `ViewModels/CustomerListViewModel.cs` (new)
- `Views/CustomerListPage.xaml` (new)
- `Views/CustomerListPage.xaml.cs` (new)

---

#### FE-UI-12: Member Management Page
**Priority**: P1  
**Effort**: 6 days  
**Dependencies**: Member backend (BE-F.3, BE-F.4, BE-F.5)

**Description**: Create member management page for membership administration.

**Acceptance Criteria**:
- [ ] MemberManagementViewModel created
- [ ] MemberManagementPage XAML
- [ ] Member list with membership tier indicators
- [ ] Membership tier configuration section
- [ ] Discount configuration section
- [ ] Prepaid account management section
- [ ] Member detail panel with history

**Files to Create/Modify**:
- `ViewModels/MemberManagementViewModel.cs` (new)
- `Views/MemberManagementPage.xaml` (new)
- `Views/MemberManagementPage.xaml.cs` (new)

---

#### FE-UI-13: Table Session Page
**Priority**: P1  
**Effort**: 5 days  
**Dependencies**: FE-UI-02 (Session Timer), Session backend (BE-A.*)

**Description**: Create table session page for active session monitoring and control.

**Acceptance Criteria**:
- [ ] TableSessionViewModel created
- [ ] TableSessionPage XAML
- [ ] Session list with SessionTimerControl for each
- [ ] Session control buttons (Pause, Resume, End)
- [ ] Current charges display
- [ ] Server assignment display
- [ ] Session detail panel
- [ ] Real-time session monitoring

**Files to Create/Modify**:
- `ViewModels/TableSessionViewModel.cs` (new)
- `Views/TableSessionPage.xaml` (new)
- `Views/TableSessionPage.xaml.cs` (new)

---

#### FE-UI-14: Inventory Management Page
**Priority**: P1  
**Effort**: 5 days  
**Dependencies**: Inventory backend (BE-G.*)

**Description**: Create inventory management page for stock control.

**Acceptance Criteria**:
- [ ] InventoryManagementViewModel created
- [ ] InventoryManagementPage XAML
- [ ] Inventory list with stock level indicators
- [ ] Low stock alert configuration
- [ ] Stock adjustment dialog
- [ ] Physical count mode UI
- [ ] Inventory reports section

**Files to Create/Modify**:
- `ViewModels/InventoryManagementViewModel.cs` (new)
- `Views/InventoryManagementPage.xaml` (new)
- `Views/InventoryManagementPage.xaml.cs` (new)

---

#### FE-UI-15: Audit Log Page
**Priority**: P2  
**Effort**: 4 days  
**Dependencies**: Audit backend (BE-J.6)

**Description**: Create audit log page for viewing system activity history.

**Acceptance Criteria**:
- [ ] AuditLogViewModel created
- [ ] AuditLogPage XAML with DataGrid
- [ ] Filter controls (user, action type, date range)
- [ ] Search bar
- [ ] Audit entry detail panel
- [ ] Export button
- [ ] Pagination support

**Files to Create/Modify**:
- `ViewModels/AuditLogViewModel.cs` (new)
- `Views/AuditLogPage.xaml` (new)
- `Views/AuditLogPage.xaml.cs` (new)

---

### Dialog Patterns (P1)

#### FE-UI-16: Convert Settle to Modal Dialog
**Priority**: P1  
**Effort**: 3 days  
**Dependencies**: FE-UI-03 (Loading Overlay)

**Description**: Convert SettlePage to modal dialog to preserve ticket context.

**Acceptance Criteria**:
- [ ] SettleDialog ContentDialog created
- [ ] SettleViewModel logic migrated
- [ ] OrderEntryPage shows SettleDialog as overlay
- [ ] Ticket context preserved during settlement
- [ ] Property test: Dialog preserves context on close

**Files to Create/Modify**:
- `Views/Dialogs/SettleDialog.xaml` (new)
- `Views/Dialogs/SettleDialog.xaml.cs` (new)
- `ViewModels/SettleViewModel.cs` (modify)
- `Views/OrderEntryPage.xaml.cs` (modify)

---

#### FE-UI-17: Customer Search Dialog
**Priority**: P1  
**Effort**: 3 days  
**Dependencies**: Customer backend (BE-F.2)

**Description**: Create customer search dialog for order entry context.

**Acceptance Criteria**:
- [ ] CustomerSearchDialog ContentDialog created
- [ ] Live search with results
- [ ] Quick-add new customer functionality
- [ ] Customer history preview
- [ ] Integration with OrderEntryPage
- [ ] Property test: Dialog preserves order entry context

**Files to Create/Modify**:
- `Views/Dialogs/CustomerSearchDialog.xaml` (new)
- `Views/Dialogs/CustomerSearchDialog.xaml.cs` (new)
- `ViewModels/CustomerSearchViewModel.cs` (new)

---

### Touch and Accessibility (P1)

#### FE-UI-18: Touch Optimization
**Priority**: P1  
**Effort**: 4 days  
**Dependencies**: None

**Description**: Implement touch-optimized styles and gestures.

**Acceptance Criteria**:
- [ ] TouchOptimizedStyles.xaml created
- [ ] Minimum 44x44px button styles
- [ ] Minimum 8px spacing between elements
- [ ] Large font styles (minimum 14pt)
- [ ] Touch ripple effect styles
- [ ] Swipe-to-delete gesture
- [ ] Swipe-to-refresh gesture
- [ ] Property test: All touch targets meet minimum size

**Files to Create/Modify**:
- `Styles/TouchOptimizedStyles.xaml` (new)
- Apply styles to all pages

---

#### FE-UI-19: Accessibility Features
**Priority**: P1  
**Effort**: 5 days  
**Dependencies**: None

**Description**: Implement accessibility features for screen readers and keyboard navigation.

**Acceptance Criteria**:
- [ ] AutomationProperties.Name set for all interactive elements
- [ ] Explicit TabIndex on all form fields
- [ ] Focus visual indicators
- [ ] AccessibilityStyles.xaml created
- [ ] High contrast theme support
- [ ] Relative font sizes
- [ ] Windows Narrator tested
- [ ] Property test: All interactive elements have accessibility names

**Files to Create/Modify**:
- `Styles/AccessibilityStyles.xaml` (new)
- Update all pages with AutomationProperties

---

### Visual Consistency (P1)

#### FE-UI-20: Visual Consistency Audit
**Priority**: P1  
**Effort**: 4 days  
**Dependencies**: All previous UI tickets

**Description**: Audit and update all pages for visual consistency.

**Acceptance Criteria**:
- [ ] ConsistentSpacing.xaml created with 8px grid system
- [ ] Consistent color schemes applied
- [ ] Consistent spacing applied
- [ ] Consistent typography applied
- [ ] Consistent iconography applied
- [ ] Consistent button styles applied
- [ ] Consistent card layouts applied
- [ ] Consistent animation durations (200ms/400ms)

**Files to Create/Modify**:
- `Styles/ConsistentSpacing.xaml` (new)
- Update all pages for consistency

---

### Error Handling (P1)

#### FE-UI-21: Error State Handling
**Priority**: P1  
**Effort**: 3 days  
**Dependencies**: FE-UI-01 (Toast)

**Description**: Implement user-friendly error handling with recovery options.

**Acceptance Criteria**:
- [ ] User-friendly error message templates created
- [ ] Retry buttons for recoverable errors
- [ ] Copy Error Details buttons for support scenarios
- [ ] Connection status indicators
- [ ] Error message localization keys
- [ ] All error handling updated to use new patterns

**Files to Create/Modify**:
- Update all ViewModels with new error handling patterns
- Update ToastNotificationService error messages

---

### Performance (P1)

#### FE-UI-22: Performance Optimization
**Priority**: P1  
**Effort**: 4 days  
**Dependencies**: All previous UI tickets

**Description**: Optimize UI performance for responsiveness.

**Acceptance Criteria**:
- [ ] Button clicks respond within 100ms
- [ ] Pages load within 500ms
- [ ] Virtualization for lists with 50+ items
- [ ] Data caching for frequently accessed data
- [ ] Async operations for all I/O-bound tasks
- [ ] 60 FPS for all animations
- [ ] Performance monitoring and logging

**Files to Create/Modify**:
- Update all pages with performance optimizations
- Add performance logging

---

### Integration Testing (P1)

#### FE-UI-23: Integration Testing
**Priority**: P1  
**Effort**: 5 days  
**Dependencies**: All previous UI tickets

**Description**: Comprehensive integration testing of all UI components.

**Acceptance Criteria**:
- [ ] Switchboard navigation tested to all pages
- [ ] Manager authentication flow tested
- [ ] Table Map interactions tested
- [ ] Dialog workflows tested
- [ ] Keyboard shortcuts tested across pages
- [ ] All property tests passing
- [ ] All unit tests passing

**Files to Create/Modify**:
- `Magidesk.Presentation.Tests/Integration/` (new test files)

---

### Manual Testing (P1)

#### FE-UI-24: Manual Accessibility Testing
**Priority**: P1  
**Effort**: 2 days  
**Dependencies**: FE-UI-19

**Description**: Manual testing with assistive technologies.

**Test Checklist**:
- [ ] Windows Narrator testing
- [ ] Keyboard-only navigation testing
- [ ] High contrast theme testing
- [ ] 200% font scaling testing

---

#### FE-UI-25: Manual Touch Testing
**Priority**: P1  
**Effort**: 2 days  
**Dependencies**: FE-UI-18

**Description**: Manual testing on touchscreen devices.

**Test Checklist**:
- [ ] All buttons tested on touchscreen
- [ ] Swipe gestures tested
- [ ] On-screen keyboard triggers tested

---

#### FE-UI-26: Visual Consistency Testing
**Priority**: P1  
**Effort**: 2 days  
**Dependencies**: FE-UI-20

**Description**: Manual visual consistency audit.

**Test Checklist**:
- [ ] Consistent spacing verified across all pages
- [ ] Consistent color usage verified
- [ ] Consistent typography verified

---

## Summary

| Priority | Count | Estimated Effort |
|----------|-------|------------------|
| P0 | 3 | 17 days |
| P1 | 22 | 91 days |
| P2 | 1 | 4 days |
| **Total** | **26** | **112 days** |

**Note**: Effort estimates assume 1 developer. With 2 developers working in parallel, total calendar time would be approximately 8-10 weeks.

---

*Last Updated: 2026-01-13*


# Task 2.3.11: RefundWizard View Implementation

**Date**: 2026-01-19  
**Task**: Create RefundWizard view (4-step wizard)  
**Status**: ✅ COMPLETE

## Overview
Implemented the complete RefundWizard XAML view as a 4-step wizard dialog that guides users through the refund process with proper validation, preview, and authorization.

## Files Created

### `Views/Dialogs/RefundWizard.xaml`
Complete ContentDialog implementation with 4-step wizard UI.

### `Views/Dialogs/RefundWizard.xaml.cs`
Code-behind file (minimal, logic in ViewModel).

## Implementation Details

### Step 1: Select Refund Mode
- **UI Elements**: Three radio buttons with descriptions
  - Full Refund: "Refund the entire ticket amount"
  - Partial Refund: "Refund a specific amount"
  - Specific Payments: "Select individual payments to refund"
- **Bindings**: 
  - `IsFullMode`, `IsPartialMode`, `IsSpecificMode` (two-way)
  - Radio buttons grouped by `GroupName="RefundMode"`

### Step 2: Enter Amount or Select Payments
- **Conditional UI** based on refund mode:
  - **Partial Mode**: NumberBox for amount input
    - Bound to `PartialAmountInput`
    - Minimum value: 0.01
    - Spin buttons enabled
  - **Specific Mode**: ListView with checkboxes
    - Bound to `SpecificPayments` collection
    - Each item shows payment description and amount
    - Checkbox bound to `IsSelected` property

### Step 3: Preview and Reason
- **Refund Preview Card**:
  - Refund Amount (bold)
  - Original Amount
  - Remaining Balance (semi-bold)
  - All amounts formatted as currency
  - Styled with card background and border
- **Refund Reason**:
  - Multi-line TextBox
  - Accepts return (for multi-line input)
  - Max length: 500 characters
  - Placeholder: "Enter reason for refund..."

### Step 4: Manager Authorization
- **Authorization UI**:
  - Informational InfoBar explaining authorization requirement
  - PasswordBox for manager PIN (max 6 characters)
  - "Process Refund" button (accent style)
  - Button bound to `ConfirmRefundCommand`

## Key Features

### Navigation
- **Primary Button**: "Next" (hidden on Step 4)
  - Bound to `NextCommand`
  - Disabled when `IsBusy`
- **Secondary Button**: "Back"
  - Bound to `BackCommand`
  - Disabled when `IsBusy`
- **Close Button**: "Cancel" (always available)

### Error Handling
- Error InfoBar at top of dialog
- Bound to `ErrorMessage` and `HasError`
- Closable by user
- Severity: Error

### Loading State
- ProgressRing overlay
- Activated when `IsBusy` is true
- Centered in dialog
- Prevents interaction during processing

### Visibility Management
- Steps shown/hidden based on ViewModel properties:
  - `IsStep1Visible`
  - `IsStep2Visible`
  - `IsStep3Visible`
  - `IsStep4Visible`
- Uses `BooleanToVisibilityConverter`

### Styling
- **LabelStyle**: Semi-bold labels with bottom margin
- **SectionHeaderStyle**: 16pt semi-bold section headers
- **Card styling**: Border, corner radius, padding, themed background
- Consistent spacing (16px between sections, 8-12px within sections)

## Requirements Satisfied

### REQ-5.4: Full Refund Processing
✅ Step 1 allows selection of Full Refund mode

### REQ-5.5: Partial Refund Processing
✅ Step 2 provides NumberBox for partial amount entry

### REQ-5.6: Manager Authorization
✅ Step 4 requires manager PIN before processing

### REQ-11.1: User-Friendly UI
✅ Clear 4-step wizard with descriptions and validation

## Technical Implementation

### Bindings
- All properties use proper two-way or one-way bindings
- Commands bound to ViewModel AsyncRelayCommand/RelayCommand
- Converters used for visibility and currency formatting

### Converters Used
- `BooleanToVisibilityConverter`: Show/hide steps
- `StringFormatConverter`: Currency formatting

### Layout
- Grid with 3 rows: Error bar, Content, Loading indicator
- ScrollViewer for step content (handles overflow)
- Fixed width: 500px
- Minimum height: 400px

## Build Status
✅ Build succeeded with 0 errors

## Integration Points

### ViewModel Integration
- Binds to `RefundWizardViewModel` properties and commands
- ViewModel handles all business logic
- View is purely presentational

### Dialog Lifecycle
- Opened from SettlePage (Task 2.3.12)
- Closed via `_closeAction` callback in ViewModel
- Result communicated through command execution

## Testing Considerations
- UI can be tested with different refund modes
- Validation messages display correctly
- Step navigation works as expected
- Authorization flow triggers properly

## Next Steps
Task 2.3.11 is complete. Next task is:
- **Task 2.3.12**: Add void/refund buttons to SettlePage

## Notes
- View follows WinUI 3 best practices
- Consistent with existing dialog patterns in the application
- Accessibility-friendly with proper labels and keyboard navigation
- Responsive layout handles different content sizes

# Implementation Plan: Table Session Management

## Overview

**AUDIT COMPLETE**: After reviewing the codebase, most core Table Session Management functionality is already implemented! The delivery plan tickets marked as "NOT_STARTED" are actually complete. This updated plan focuses only on genuinely missing components.

## Already Implemented ✅

### Backend Layer
- `StartTableSessionCommand` & Handler ✅
- `EndTableSessionCommand` & Handler ✅  
- `PauseTableSessionCommand` & Handler ✅
- `ResumeTableSessionCommand` & Handler ✅
- `AdjustSessionTimeCommand` & Handler ✅
- `GetActiveSessionsQuery` & Handler ✅
- `TableSession` domain entity ✅
- `TableType` domain entity ✅

### UI Layer
- `StartSessionDialogViewModel` ✅
- `EndSessionDialogViewModel` ✅
- `AdjustSessionTimeDialogViewModel` ✅
- `ActiveSessionsPanelViewModel` ✅
- Session controls in `TableMapViewModel` ✅
- Session controls in `OrderEntryViewModel` ✅

## Tasks - Only Missing Components

- [ ] 1. Implement PricingService (Backend Priority)
  - Create `IPricingService` interface and implementation
  - Handle time-based charge calculations with rounding rules
  - Support first-hour pricing and minimum charges
  - _Requirements: 4.1, 4.2, 4.3, 4.4_
  - _Status: Referenced in EndTableSessionCommandHandler but not implemented_

- [ ] 2. Complete Table Status Management Methods
  - Add `MarkInUse()` and `MarkAvailable()` methods to Table entity
  - Update session command handlers to use these methods
  - _Requirements: 5.2, 5.3_
  - _Status: Referenced in StartTableSessionCommandHandler but methods missing_

- [ ] 3. Implement Missing UI Views
  - Create actual XAML views for existing ViewModels:
    - `Views/Dialogs/StartSessionDialog.xaml`
    - `Views/Dialogs/EndSessionDialog.xaml` 
    - `Views/Dialogs/TableSessions/AdjustSessionTimeDialog.xaml`
    - `Views/ActiveSessionsPanel.xaml`
  - _Requirements: 3.1, 3.2, 3.3_
  - _Status: ViewModels exist but XAML views may be missing_

- [ ] 4. Add Time-Based Line Items to Ticket System
  - Implement `TimeChargeOrderLine` creation in `EndTableSessionCommand`
  - Add time duration and rate display to ticket line items
  - Ensure time charges integrate with tax and discount rules
  - _Requirements: 9.1, 9.2, 9.3_
  - _Status: EndTableSessionCommand exists but time line item creation unclear_

- [ ] 5. Implement Advanced Features (Lower Priority)
  - Create `TransferSessionCommand` for moving sessions between tables
  - Add session history queries for reporting
  - Implement guest count tracking and updates
  - _Requirements: 8.1, 8.2, 6.4_
  - _Status: Not implemented, marked P2 priority in delivery plan_

- [ ] 6. Add Validation and Error Handling
  - Create FluentValidation validators for session commands
  - Improve error handling in existing command handlers
  - Add user-friendly error messages in UI dialogs
  - _Requirements: 10.2, 10.4, 10.5_

- [ ] 7. Final Integration Testing
  - Test complete session workflow end-to-end
  - Verify session recovery after application restart
  - Test concurrent session management
  - _Requirements: All requirements validation_

## Notes

- Tasks marked with `*` are optional and can be skipped for faster MVP
- Each task references specific requirements for traceability
- Property tests validate universal correctness properties from the design
- Unit tests validate specific examples and edge cases
- Integration tests validate complete workflows
- The implementation leverages existing TableSession and TableType entities that are already implemented
- Focus on completing the application layer and UI components to enable the full session management workflow
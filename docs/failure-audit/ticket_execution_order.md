# Ticket Execution Order

## Phase 4.1: Foundation & Blockers (The "Safety Net")

1.  **FEH-006**: Implement `IDialogService`. (Prerequisite for all others)
2.  **FEH-002**: Harden `App.UnhandledException`. (Last line of defense)
3.  **SYS-001**: Fatal Startup Dialog. (First line of defense)

## Phase 4.2: Critical Runtime Crashes

4.  **FEH-001**: Fix `MainWindow.OnItemInvoked`. (**BLOCKER**)
5.  **NAV-001**: Fix `NavigationService.ShowDialogAsync`. (**BLOCKER**)

## Phase 4.3: High Risk Cleanups

6.  **FEH-003**: Fix `ShiftStartDialog` Async Void.
7.  **FEH-004**: Fix `TableDesignerPage` Async Void.
8.  **FEH-005**: Fix `MainWindow` Dispatcher.

## Phase 4.4: Backend Audits (Lower Priority)

9.  **BEH-001**: Audit Printing.
10. **BEH-002**: Audit Payment.

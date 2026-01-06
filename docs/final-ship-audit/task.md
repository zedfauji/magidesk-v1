# Final Pre-Ship Forensic Audit & Hardening

**Objective**: Guarantee 100% Operator Visibility for all errors. Zero silent failures.

## Phase 1: Line-by-Line Code Review [COMPLETED]
- [x] Audit Core Infrastructure
    - [x] `App.xaml.cs` (Startup/Global Handlers)
    - [x] `NavigationService.cs` (Dialogs/Auth)
    - [x] `PrintingService.cs` (Hardware)
- [x] Audit ViewModels (User Interaction)
    - [x] `LoginViewModel.cs`
    - [x] `DashboardViewModel.cs`
    - [x] `OrderEntryViewModel.cs` (Re-verify)
    - [x] `SettleViewModel.cs` (Re-verify)
    - [x] `TicketHistoryViewModel.cs`
    - [x] `ReportsViewModel.cs`
    - [x] `SystemConfigViewModel.cs`
    - [x] `ModifierSelectionViewModel.cs`
- [x] Audit Services & Data (Backend)
    - [x] `UserService.cs`
    - [x] `TerminalService.cs`
    - [x] `KitchenPrintService.cs`
    - [x] `Repositories` (Check for swallowed DB exceptions)

## Phase 2: Visibility Classification [COMPLETED]
- [x] Generate `operator_visible_failures.md`
- [x] Generate `silent_failure_eliminations.md`

## Phase 3: Documentation [COMPLETED]
- [x] Create `operator_visible_failures.md`
- [x] Create `silent_failure_eliminations.md`
- [x] Create `service_health_visibility.md` (Merged into silent eliminations)
- [x] Create `async_background_visibility.md` (Merged into silent eliminations)
- [x] Create `startup_runtime_visibility.md` (Merged into silent eliminations)

## Phase 4: Granular Ticketing [COMPLETED]
- [x] Create `tickets.md`
- [x] Create `ticket_execution_order.md`

## Phase 5: Implementation [COMPLETED]
- [x] Execute Audit Tickets (T-001 to T-012)

## Phase 6: Verification [COMPLETED]
- [x] Create `verification.md`

## Phase 7: Certification [COMPLETED]
- [x] Create `ship_certification.md`

# Navigation Findings & Blockers (Magidesk)

**Positioning**: You asked for navigation “like FloreantPOS”. The findings below are based on concrete deltas between:
- FloreantPOS RootView + action-based navigation + separate BackOfficeWindow
- Magidesk MainWindow sidebar + page/dialog navigation graph

## High-impact findings
- **[FND-001] No RootView-style default routing / terminal default view selection**
  - **Impact**: High
  - **Floreant baseline**: `RootView.showDefaultView()` routes based on `TerminalConfig.getDefaultView()` and order-type rules.
  - **Magidesk evidence**: Startup always navigates to `LoginPage`; after login, navigates to `SwitchboardPage`. No terminal-config default view routing evidenced.

- **[FND-002] Root navigation is a broad sidebar with many admin/utility surfaces**
  - **Impact**: High
  - **Floreant baseline**: primary operator flow is centered on RootView + switchboard/home; back office is separated and permission-driven.
  - **Magidesk evidence**: MainWindow sidebar exposes pages like `TicketPage`, `PaymentPage`, `TicketManagementPage`, `UserManagementPage`, `SalesReportsPage`, `SettingsPage`, etc.
  - **Risk**: operator can reach non-primary/administrative surfaces without a Floreant-like guardrail.

- **[FND-003] Back office model diverges (page-in-frame vs separate window + permission menus)**
  - **Impact**: High
  - **Floreant baseline**: `ShowBackofficeAction` opens/reuses `BackOfficeWindow` and builds menus by permission.
  - **Magidesk evidence**: `ManagerFunctionsDialog` navigates to `BackOfficePage` inside the main frame; no permission gating evidenced for entry or menu composition.

- **[FND-004] OpenTicketsListViewModel has a Split action with no UI binding**
  - **Impact**: Medium
  - **Evidence**: `OpenTicketsListViewModel.SplitAsync()` opens `SplitTicketDialog`, but `OpenTicketsListDialog.xaml` has only Resume/Transfer/Void buttons.
  - **Result**: feature may exist in code but is unreachable by current UI.

- **[FND-005] Orphan and variant UI assets exist**
  - **Impact**: Medium
  - **Evidence**: `MainPage` has no inbound navigation; `PasswordEntryDialog` has no call sites; multiple `SplitTicketDialog_*` XAML variants exist with unclear inclusion.

## Blockers to declaring “Floreant-like” parity
- **[BLK-001] Missing evidence of permission gating around Back Office and admin navigation**
  - Floreant has explicit permission checks (e.g., Back Office).
  - Magidesk parity cannot be confirmed without code evidence for similar guards.

- **[BLK-002] Unknown mapping for many root sidebar items**
  - Several Magidesk root surfaces appear utility/debug (`TicketPage`, `PaymentPage`). Their intended role vs operator workflow is not provable.

## Lower-impact observations
- **[OBS-001] Back navigation is implicit**
  - Many flows return using `GoBack()`; this is history-dependent and can differ from Floreant’s explicit view switching semantics.

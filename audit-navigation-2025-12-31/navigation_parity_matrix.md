# Navigation Parity Matrix (Magidesk vs FloreantPOS)

**Goal**: Identify whether Magidesk navigation behavior matches the FloreantPOS baseline.

**Sources**:
- **Magidesk**: `navigation_entries.md`, `navigation_graph.mmd`
- **FloreantPOS**: `floreant_navigation.md` (RootView card-layout, actions, BackOfficeWindow)

Legend:
- **YES**: explicit, code-evidenced equivalent exists
- **NO**: not present in current evidence
- **PARTIAL**: concept exists but differs materially
- **UNKNOWN**: intent/behavior not provable from available code evidence

## Core flows
| Flow | FloreantPOS baseline | Magidesk current | Parity | Evidence notes |
|---|---|---|---|---|
| Startup → Login | RootView hides header for `LoginView`; uses default view routing after auth | `App.xaml.cs` navigates to `Views.LoginPage` | PARTIAL | Magidesk has startup → Login, but no RootView-style default view routing captured |
| Login success → Home/Switchboard | `SwithboardViewAction` / `HomeScreenViewAction` show switchboard/home | `LoginViewModel.LoginAsync()` navigates to `SwitchboardPage` | YES | Direct navigation exists |
| Home screen concept | `RootView.showHomeScreen()` | `MainWindow` “Home” → `SwitchboardPage` | PARTIAL | Home exists but is one of many sidebar items; semantics differ |
| Switchboard → New ticket flow | RootView can create ticket for `OrderView` based on default view / order type | `SwitchboardPage` New Ticket: `OrderTypeSelectionDialog` → (GuestCount/ShiftStart) → `OrderEntryPage` | PARTIAL | Ticket creation exists; RootView-like default view logic not present |
| Order type requires table selection | RootView routes to `TableMapView` when table required | Magidesk has `TableMapPage` and can create/resume ticket on select | PARTIAL | Similar screens exist; routing is user-driven vs RootView automatic routing |
| Order type requires customer | RootView routes to `CustomerView` / delivery dispatch plugin | Magidesk has `CustomerSelectionDialog` from `OrderEntryViewModel.AssignCustomerAsync()` | PARTIAL | Customer selection exists but not as startup/default routing; no delivery dispatch view found |
| Kitchen display routing | RootView default can show `KitchenDisplayView` and hide header | Magidesk `SwitchboardPage` → `KitchenDisplayPage` | PARTIAL | Screen exists; header/show/hide behavior not comparable from evidence |
| Other Functions view | `SwitchboardOtherFunctionsView` via `ShowOtherFunctionsAction` | `ManagerFunctionsDialog` (modal) | PARTIAL | Similar intent (extra functions) but different UX primitive (modal dialog vs view) |
| Back Office access | `ShowBackofficeAction` opens/reuses `BackOfficeWindow` with permission gating | `ManagerFunctionsDialog` → `BackOfficePage` (in-frame) | PARTIAL | Back office exists but is not a separate window; no permission gating evidenced |
| Back Office navigation | BackOfficeWindow menu bar built by permissions | `BackOfficePage` internal `NavigationView` → sub-pages | PARTIAL | Internal nav exists; permission-based menu composition not evidenced |
| Logout | `LogoutAction` closes extra windows and `doLogout()` | `SwitchboardViewModel.LogoutCommand` → `LoginPage`; `SettleViewModel.OnLogoutAsync` → `LoginPage` | PARTIAL | Navigates to login, but window disposal / session teardown parity is UNKNOWN |

## Magidesk-only surfaces (no known FloreantPOS baseline match)
| Magidesk surface | Entry point | FloreantPOS equivalent | Parity |
|---|---|---|---|
| `TicketPage` (Ticket Entry) | MainWindow sidebar | UNKNOWN | UNKNOWN (likely debug/utility) |
| `PaymentPage` | MainWindow sidebar | UNKNOWN | UNKNOWN (likely debug/utility) |
| `TicketManagementPage` | MainWindow sidebar / Switchboard fallback | UNKNOWN | UNKNOWN |
| `PrintPage` | MainWindow sidebar | UNKNOWN | UNKNOWN |

## FloreantPOS baseline flows not evidenced in Magidesk
| FloreantPOS flow | Magidesk equivalent | Parity | Notes |
|---|---|---|---|
| RootView `showDefaultView()` (TerminalConfig-driven default view selection) | No equivalent found | NO | No evidence of terminal-config-based routing |
| BackOfficeWindow reuse/bring-to-front | `BackOfficePage` (frame page) | PARTIAL | Different windowing model |
| Permission-guarded Back Office entry (`UserPermission.VIEW_BACK_OFFICE`) | Unknown | UNKNOWN | No guard found in ManagerFunctions navigation |
| Delivery dispatch / plugin view routing | Unknown | NO/UNKNOWN | No evidence in current navigation graph |

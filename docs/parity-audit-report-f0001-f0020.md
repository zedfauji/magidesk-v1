# Parity Audit & Gap Analysis Report (F-0001 - F-0020)

**Date:** Dec 27, 2025
**Auditor:** Cascade (Principal Architect)
**Reference System:** FloreantPOS v1.4 (Build 706)
**Target System:** Magidesk (Phase 4 In-Progress)

---

## 1. Feature Parity Matrix

| ID | Feature Name | FloreantPOS Behavior (Ref) | Backend Parity | UI Readiness | Risk Level |
| :--- | :--- | :--- | :--- | :--- | :--- |
| **F-0001** | App Bootstrap & Init | Checks DB, config, peripherals. Blocks UI if failed. | **PARTIAL** (DB connection exists, but no health checks/blocking logic) | **BLOCKED** (UI assumes happy path) | **CRITICAL** |
| **F-0002** | POS Main Window | Shell with Status Bar (User, Terminal, Clock). Handles Shutdown. | **PARTIAL** (Window exists, missing status heartbeat/time sync) | **PARTIAL** (Nav frame exists, missing Status Bar) | **MEDIUM** |
| **F-0003** | Login Screen | Gates all access. Validates User/PIN. Enforces Role. | **FULL** (UserService/AuthService exists) | **NOT IMPLEMENTED** (Bypassed to Switchboard) | **CRITICAL** |
| **F-0004** | Switchboard View | Navigation Hub + "My Open Tickets" List + Clock Info. | **PARTIAL** (Query exists, missing Concurrency/Locking) | **PARTIAL** (Buttons exist, Data/List missing) | **HIGH** |
| **F-0005** | Order Entry Container | Host for Ticket + Menu + Actions. State Machine (Created->Active). | **FULL** (Ticket Entity & State Machine defined) | **NOT IMPLEMENTED** (Developer Stub only) | **HIGH** |
| **F-0006** | Ticket Panel | Displays items, totals. Actions: Search, Qty, Pay. | **PARTIAL** (Search/Pay backend logic exists) | **PARTIAL** (ListView exists, missing integrated actions) | **HIGH** |
| **F-0007** | Payment Keypad | Numeric pad for tender entry. Validates amounts. | **FULL** (Logic validated in SettleViewModel) | **PARTIAL** (Exists in SettlePage, but Password/PIN entry missing) | **MEDIUM** |
| **F-0008** | Settle Ticket Dialog | Modal payment flow. Handles partials, change, drawer assignment. | **FULL** (SettleTicketCommand & Logic implemented) | **PARTIAL** (Page implemented, not Dialog. Drawer logic unverified) | **MEDIUM** |
| **F-0009** | Manager Functions | Admin dialog (Reset Drawer, etc.). RBAC enforced. | **PARTIAL** (Missing granular permissions & logging) | **NOT IMPLEMENTED** (Placeholder only) | **HIGH** |
| **F-0010** | Cash Drops / Bleeds | Record cash out from drawer. Updates Drawer Balance. | **PARTIAL** (Missing DrawerDomainService/Logic) | **NOT IMPLEMENTED** | **HIGH** |
| **F-0011** | Open Tickets List | Dialog to list/filter/resume tickets. "Cashier Mode". | **PARTIAL** (Query exists, missing "Resume" validation) | **PARTIAL** (Page exists, missing "Cashier Mode" & Dialog) | **MEDIUM** |
| **F-0012** | Drawer Pull Report | Snapshot of expected vs actual cash. Reset drawer option. | **PARTIAL** (Missing Snapshot/Calculation logic) | **PARTIAL** (Page exists, missing HTML preview/Finish action) | **HIGH** |
| **F-0013** | Void Ticket | Reverses transaction, restores inventory. Requires Reason. | **FULL** (VoidTicketCommand exists) | **PARTIAL** (Command exists, Dialog/Flow missing) | **HIGH** |
| **F-0014** | Split Ticket | Move items to new ticket. Maintain value. | **FULL** (SplitTicketCommand verified) | **NOT IMPLEMENTED** (Stub only) | **MEDIUM** |
| **F-0015** | Payment Wait Dialog | Blocking modal during gateway processing. | **FULL** (Async/Await pattern replaces Wait Dialog) | **PARTIAL** (ProgressRing used, not blocking modal) | **LOW** |
| **F-0016** | Swipe Card | Capture Track data (encrypted). Tokenize. | **MISSING** (Hardware event handling missing) | **NOT IMPLEMENTED** | **CRITICAL** |
| **F-0017** | Auth Code Dialog | Manual entry of Voice Auth code. | **FULL** (Supported in Transaction model) | **NOT IMPLEMENTED** | **MEDIUM** |
| **F-0018** | Batch Capture | EOD Batch processing for Pre-Auths. | **MISSING** (No Batch Service) | **NOT IMPLEMENTED** | **HIGH** |
| **F-0019** | New Ticket Action | Initialize Ticket with defaults (User, Terminal, Tax). | **FULL** (TicketFactory implemented) | **PARTIAL** (Command exists, flow incomplete) | **LOW** |
| **F-0020** | Order Type Selection | Dialog to pick DineIn/TakeOut. Triggers Tax recalc. | **PARTIAL** (Entity exists, missing auto-recalc trigger) | **PARTIAL** (Dialog exists, missing workflow enforcement) | **MEDIUM** |

---

## 2. Gap & Drift Report

### Critical Backend Gaps
1.  **Bootstrapping Safety (F-0001)**: The application starts without verifying the Database Connection or Terminal Identity. This allows "Zombie" states where the UI looks ready but operations will fail.
    *   *Required Fix*: Implement `StartupHealthCheck` service that gates the `LoginScreen`.
2.  **Hardware/Peripheral Layer (F-0016)**: No abstraction exists for listening to Card Swipes or Scanner events. The domain model assumes data just "appears".
    *   *Required Fix*: Create `IPeripheralService` with event aggregation for hardware inputs.
3.  **Drawer Management (F-0010, F-0012)**: The concept of a "Drawer Session" is loosely defined. We need strict accounting for Opening Balance + Sales + In - Out = Expected.
    *   *Required Fix*: Implement `DrawerDomainService` with rigorous double-entry accounting logic.
4.  **Batch Processing (F-0018)**: No mechanism for End-of-Day Credit Card batch capture.

### UI Alignment Issues
1.  **Dialog vs Page**: FloreantPOS heavily uses Modal Dialogs (Settle, Open Tickets, Split). Magidesk has implemented these as Full Pages.
    *   *Impact*: Loss of context (background view hidden). Slower navigation.
    *   *Recommendation*: Refactor critical "sub-tasks" (Settle, Split, Auth) to `ContentDialog` or keep as Pages but ensure "Cancel/Back" restores exact state.
2.  **Missing Interactivity**:
    *   **Ticket View**: Is currently a read-only list in many places. Needs to be a command center (Swipe to Delete, Tap to Edit).
    *   **Login**: The crucial gatekeeper is missing.

### Contract Mismatches
1.  **Void Reasons**: Backend requires a `ReasonCode` for voids. UI currently has no mechanism to capture this (just a button).
2.  **Order Type Logic**: UI selects Order Type, but Backend `TicketFactory` doesn't strictly enforce the *consequences* of that type (e.g., prompting for Customer if Delivery).

---

## 3. Critical Blockers List

| Feature | Reason | Remediation Plan |
| :--- | :--- | :--- |
| **F-0003 Login** | Cannot enforce User Context (Permissions, Drawer Assignment) without it. | **P0**: Wire up `LoginView` to `AuthService`. Stop auto-nav to Switchboard. |
| **F-0001 Bootstrap** | Risk of operating on invalid configuration/DB. | **P0**: Add `InitializationService` check in `App.xaml.cs`. |
| **F-0016 Swipe** | Cannot process Card Present transactions. | **P1**: Define Hardware Interface. Implement Keyboard Shim for testing. |
| **F-0012 Drawer** | Cannot close shifts or track cash. | **P1**: Port `DrawerPull` logic from Floreant to `DrawerService`. |

---

## 4. Execution Timeline Proposal

### Phase 4.1: Foundation Repair (1 Week)
*   **Goal**: Secure the runtime environment.
*   Tasks:
    1.  Implement **F-0001** (Startup Checks).
    2.  Implement **F-0003** (Login Screen & Session Management).
    3.  Implement **F-0019** & **F-0020** (Robust Ticket Creation with Tax Recalc).

### Phase 4.2: Critical Workflows (2 Weeks)
*   **Goal**: Enable end-to-end "Happy Path" order.
*   Tasks:
    1.  **F-0005** & **F-0006**: Build proper Order Entry UI (Ticket View + Menu Grid).
    2.  **F-0008** & **F-0015**: Polish Settle Workflow (ensure payments align with Drawer).
    3.  **F-0011**: Functional Open Tickets List (Resume/Edit).

### Phase 4.3: Exception Handling (2 Weeks)
*   **Goal**: Handle reality (Voids, Splits, Refunds).
*   Tasks:
    1.  **F-0013**: Void Workflow (with Reason capture).
    2.  **F-0014**: Split Ticket UI.
    3.  **F-0012**: Drawer Management UI.

---

## 5. Recommendations

1.  **Stop New UI Development**: Do not build F-0021+ until F-0001/F-0003 are fixed. The app foundation is too shaky without User/Terminal context.
2.  **Adopt "Dialog-First"**: For F-0013 (Void), F-0014 (Split), F-0017 (Auth), implement as `ContentDialogs` immediately to match Floreant's modal workflow.
3.  **Harden the Backend**: The `DrawerDomainService` gap is significant. Prioritize this over "Manager Functions" (F-0009), as you can't manage what you don't track.

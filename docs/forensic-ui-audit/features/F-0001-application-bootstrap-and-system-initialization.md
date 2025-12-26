# Feature: Application bootstrap & system initialization

## Classification
- **Parity classification**: PARITY REQUIRED
- **MagiDesk status**: UNCERTAIN (not assessed yet)

## Problem / Why this exists (grounded)
- **Operational need**: Ensure the POS cannot be used until the local environment is coherent (DB connection, terminal identity, order types, printers, global config) and surface a remediation UI when it is not.
- **Evidence**: `Application.initializeSystem()` performs DB init/check, terminal init, printing setup, plugin init, then view init; on DB failure it offers to open `DatabaseConfigurationDialog`.

## User-facing surfaces
- **Surface type**: Auto-triggered workflow + error confirmation dialogs + optional configuration dialog
- **UI entry points**:
  - App startup: `com.floreantpos.main.Main.main()` → `Application.start()`
  - Auto-logoff: `Application.doAutoLogout()` (invoked elsewhere; trigger source not proven in this file)
- **Exit paths**:
  - Startup proceeds to RootView and initializes views, or halts with DB config prompt

## Preconditions & protections
- **State checks**:
  - `initializeSystem()` returns early if already initialized
  - Uses glass pane during initialization (`posWindow.setGlassPaneVisible(true)`) to block input
- **Failure protections**:
  - DB connection failure routes to a confirm prompt and optionally opens `DatabaseConfigurationDialog`

## Step-by-step behavior (forensic)
1. App boot: `Main.main()` loads default locale from `TerminalConfig.getDefaultLocale()` then `Application.start()`.
2. UI shell appears: `Application.start()` configures Look & Feel, creates `PosWindow`, enters fullscreen if configured, attaches `RootView`.
3. System initialization: `Application.initializeSystem()` blocks UI (glass pane), then:
   - `DatabaseUtil.checkConnection(DatabaseUtil.initialize())`
   - `DatabaseUtil.updateLegacyDatabase()`
   - Terminal identity (`initTerminal()`; `TerminalUtil.getSystemUID()` and/or configured terminal id)
   - Load order types (`OrderTypeDAO.findEnabledOrderTypes()`)
   - Print config, restaurant config, currency, global config, printer sets, length unit, plugins
   - Initializes root views: `RootView.getInstance().initializeViews()`, then login view order buttons and terminal id
4. On DB connection failure: shows a `JOptionPane.showConfirmDialog(...)` and (if YES) opens `DatabaseConfigurationDialog.show(...)`.
5. On any other exception: shows `POSMessageDialog.showError(...)`.
6. Unblocks UI by hiding glass pane in `finally`.

## Edge cases & failure paths
- **DB connection failure**: explicit remediation path to `DatabaseConfigurationDialog`.
- **Plugin init**: `ExtensionManager.getInstance().initialize(...)` and `FloreantPlugin.initUI()` are called; plugin behavior is not enumerated here (requires per-plugin inspection).

## Data / audit / financial impact
- **Writes/updates**:
  - Terminal record may be created/updated (`TerminalDAO.saveOrUpdate`) if not found by key/id.
  - Restaurant unique id may be assigned and persisted (`RestaurantDAO.saveOrUpdate`).
- **Operational risk**: Mis-identifying terminal or allowing usage without config can corrupt drawer/shift attribution and printer routing.

## Code traceability (REQUIRED)
- **Primary class**: `com.floreantpos.main.Application` → `/projects/Code/Redesign-POS/floreantpos/src/com/floreantpos/main/Application.java`
- **Entry point**: `com.floreantpos.main.Main` → `/projects/Code/Redesign-POS/floreantpos/src/com/floreantpos/main/Main.java`
- **Key methods**: `start()`, `initializeSystem()`, `initTerminal()`, `initOrderTypes()`, `shutdownPOS()`, `doLogin()`, `doLogout()`, `doAutoLogout()`
- **Dialogs referenced**: `DatabaseConfigurationDialog`, `UpdateDialog`, `PasswordEntryDialog`, `POSMessageDialog`

## Uncertainties (STOP; do not guess)
- **Auto-logoff trigger**: `PosWindow` defines an `autoLogoffTimer` but does not initialize it in the portion inspected; `Application.doAutoLogout()` exists, but the scheduling/trigger conditions are not proven here.

## MagiDesk parity notes
- **What exists today**: Not yet assessed.
- **What differs / missing**: Not yet assessed.

## Porting strategy (PLAN ONLY)
- **Backend requirements**: Equivalent “system initialization gate” with idempotency and a clear failure mode when DB/config are invalid.
- **API/DTO requirements**: None (local app), unless MagiDesk persists config remotely.
- **UI requirements**: A blocking “initializing” overlay; a DB configuration surface reachable from fatal startup errors.
- **Constraints for implementers**:
  - Do not allow order flow unless terminal identity and required config are loaded.
  - Preserve the explicit remediation path on DB failure (prompt → open configuration UI).

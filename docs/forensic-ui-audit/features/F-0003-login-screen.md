# Feature: Login screen (order-type buttons, switchboard/backoffice/kitchen entry)

## Classification
- **Parity classification**: PARITY REQUIRED
- **MagiDesk status**: UNCERTAIN (not assessed yet)

## Problem / Why this exists (grounded)
- **Operational need**: Gate access to operational views behind credential entry, while still offering environment actions (DB config, shutdown) depending on configuration.
- **Evidence**: `LoginView.doLogin()` prompts via `PasswordEntryDialog.getUser(...)`; buttons select the default view before login.

## User-facing surfaces
- **Surface type**: Screen
- **UI entry points**: default root view after app initialization
- **Controls observed**:
  - Order type buttons (dynamically from enabled order types where `isShowInLoginScreen()`)
  - “Orders” (Switchboard), “Back Office”, “Kitchen Display” (optional), “Driver View” (plugin-provided)
  - “Configure Database” (hidden in fullscreen and/or if config disallows)
  - “Shutdown” (hidden in fullscreen)
  - “Clock Out” (`ClockInOutAction(false, true)`)

## Preconditions & protections
- **Config gating**:
  - Fullscreen mode hides DB configure + shutdown buttons.
  - `TerminalConfig.isShowDbConfigureButton()` governs DB configure visibility when not fullscreen.
- **Plugin gating**:
  - Driver view is only available if `OrderServiceExtension` plugin exists.

## Step-by-step behavior (forensic)
1. Screen renders restaurant name from `Application.getInstance().getRestaurant().getName()`.
2. Order-type buttons are built in `initializeOrderButtonPanel()` from `Application.getInstance().getOrderTypes()` filtered by `orderType.isShowInLoginScreen()`.
3. Clicking “Orders” or “Back Office” or “Kitchen Display” sets `TerminalConfig.setDefaultView(...)` then calls `doLogin()`.
4. `doLogin()` prompts: `PasswordEntryDialog.getUser(Application.getPosWindow(), <title>, <prompt>)`.
   - If user is null: clears back-office login flag and returns.
   - Else calls `Application.doLogin(user)`.
5. Error handling:
   - `UserNotFoundException` → `POSMessageDialog.showError(..., "LoginView.3")`
   - `ShiftException` → `MessageDialog.showError(e.getMessage())`
   - Generic exception: if message contains “Cannot open connection” shows error then opens `DatabaseConfigurationDialog`.

## Edge cases & failure paths
- **DB connection failure during login**: explicit fallback to database configuration dialog.

## Data / audit / financial impact
- **Audit**: Login precedes shift/ticket attribution.

## Code traceability (REQUIRED)
- **Primary UI class**: `com.floreantpos.ui.views.LoginView` → `/projects/Code/Redesign-POS/floreantpos/src/com/floreantpos/ui/views/LoginView.java`
- **Credential UI**: `com.floreantpos.ui.dialog.PasswordEntryDialog` (not inspected here)
- **Navigation**: `TerminalConfig.setDefaultView(...)`, `RootView` usage

## Uncertainties (STOP; do not guess)
- **Meaning of `backOfficeLogin` flag**: Set true when Back Office button used, but downstream consumption not proven in this file.

## MagiDesk parity notes
- **What exists today**: Not yet assessed.
- **What differs / missing**: Not yet assessed.

## Porting strategy (PLAN ONLY)
- **Backend requirements**: Authenticate user; enforce shift/clock-in behavior after login (see `Application.initCurrentUser`).
- **UI requirements**: Dynamic order-type button panel and conditional environment buttons.
- **Constraints for implementers**:
  - Preserve explicit configuration gating rules (fullscreen vs non-fullscreen, config toggles).
  - Preserve the "default view" selection prior to credential entry.

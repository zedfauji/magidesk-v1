# Feature: POS main window shell (status bar, timers, shutdown flow)

## Classification
- **Parity classification**: PARITY REQUIRED
- **MagiDesk status**: UNCERTAIN (not assessed yet)

## Problem / Why this exists (grounded)
- **Operational need**: Provide a stable top-level window hosting all POS views, expose key operational status (terminal id, user, DB, tax-included mode, clock), and ensure controlled shutdown behavior.
- **Evidence**: `PosWindow` constructs status bar labels and a 1-second timer to display time; `windowClosing` behavior forks on login state.

## User-facing surfaces
- **Surface type**: Main window
- **UI entry points**: App startup (`Application.start()` creates `new PosWindow()`)
- **Exit paths**:
  - Window close request triggers shutdown workflow (either `ShutDownAction` or `Application.shutdownPOS()`)

## Preconditions & protections
- **State checks**:
  - `windowClosing`: if user is logged in (`Application.getCurrentUser() != null`) it triggers `new ShutDownAction().actionPerformed(null)`; else calls `Application.shutdownPOS()`.

## Step-by-step behavior (forensic)
1. Constructor builds a footer/status bar with labels: status, terminal, user, DB, tax-included, time.
2. Starts `clockTimer` (Swing `Timer(1000, ...)`) to update `lblTime` with `hh:mm:ss aaa`.
3. `updateView()` populates:
   - Terminal id from `TerminalConfig.getTerminalId()`
   - User full name + type or “Not Logged In”
   - DB host/name from `AppConfig`
   - Tax included YES/NO from `Application.isPriceIncludesTax()`
4. `setupSizeAndLocation()` loads persisted dimensions/position and minimum size; close op is `DO_NOTHING_ON_CLOSE`.
5. Fullscreen mode: `enterFullScreenMode()` sets undecorated + maximized.
6. Visibility toggles start/stop timers.

## Edge cases & failure paths
- **Window close while logged in**: shutdown path is delegated to `ShutDownAction` (behavior not proven here; requires action inspection).

## Data / audit / financial impact
- **Audit/financial**: Indirect. Controlled shutdown prevents state loss and may ensure drawer/shift close flows are respected.

## Code traceability (REQUIRED)
- **Primary UI class**: `com.floreantpos.main.PosWindow` → `/projects/Code/Redesign-POS/floreantpos/src/com/floreantpos/main/PosWindow.java`
- **Entry**: created in `Application.start()`
- **Shutdown linkage**: `com.floreantpos.actions.ShutDownAction` (not inspected yet)

## Uncertainties (STOP; do not guess)
- **Auto-logoff timer**: field `autoLogoffTimer` exists, but initialization and behavior are not present in the inspected code section.

## MagiDesk parity notes
- **What exists today**: Not yet assessed.
- **What differs / missing**: Not yet assessed.

## Porting strategy (PLAN ONLY)
- **UI requirements**: Main shell must surface terminal/user/db/tax-mode/time consistently and support a blocking overlay.
- **Constraints for implementers**:
  - Preserve the split shutdown behavior (logged-in vs logged-out close).
  - Preserve periodic time display behavior (used operationally for reconciliation).

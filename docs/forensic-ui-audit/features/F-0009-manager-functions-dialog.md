# Feature: Manager functions dialog

## Classification
- **Parity classification**: PARITY REQUIRED
- **MagiDesk status**: UNCERTAIN (not assessed yet)

## Problem / Why this exists (grounded)
- **Operational need**: Provide a privileged, consolidated manager entry point for cash-control and oversight operations (drawer pull, cash drops/bleed, open tickets, server tips report, drawer kick).
- **Evidence**: `ManagerDialog` buttons launch cash drop dialog, open tickets list dialog, drawer pull report dialog, tips cashout report dialog; also has a drawer kick action.

## User-facing surfaces
- **Surface type**: Modal dialog
- **UI entry points**: Not proven in this file; likely invoked by a “Manager” action elsewhere.

## Preconditions & protections
- **Modal behavior**: Constructed as modal (`super(Application.getPosWindow(), true)`).
- **Glass pane**: Uses a `GlassPane` while launching sub-dialogs.

## Step-by-step behavior (forensic)
- **Cash drops (drawer bleed)**:
  1. Shows glass pane.
  2. Opens `CashDropDialog`, calls `initDate()`, then `open()`.
  3. Hides glass pane.
- **Open tickets**:
  1. Shows glass pane.
  2. Opens `OpenTicketsListDialog.open()`.
  3. Hides glass pane.
- **Drawer pull report**:
  1. Shows glass pane.
  2. Opens `DrawerPullReportDialog` sized ~470x500, non-resizable.
  3. Hides glass pane.
- **Server tips**:
  1. Shows glass pane.
  2. Presents a criteria panel: user selection (`UserDAO.findAll`) + from/to date pickers.
  3. Generates report via `GratuityDAO.createReport(from,to,user)`.
  4. Opens `TipsCashoutReportDialog`.
  5. Hides glass pane.
- **Drawer kick**:
  - Executes `drawer-kick.bat` from `Application.getInstance().getLocation()` if present.

## Edge cases & failure paths
- Sub-dialog exceptions show `POSMessageDialog.showError(..., ERROR_MESSAGE, e)`.
- Drawer kick fails are logged but do not show an error dialog in this method.

## Data / audit / financial impact
- **Financial impact**:
  - Cash drops, drawer pull reports, and drawer kicks affect cash-control operations.
- **Risk**: Missing/incorrect manager controls create reconciliation gaps and shrink risk.

## Code traceability (REQUIRED)
- **Primary UI**: `com.floreantpos.ui.dialog.ManagerDialog` → `/projects/Code/Redesign-POS/floreantpos/src/com/floreantpos/ui/dialog/ManagerDialog.java`
- **Sub-dialogs**: `CashDropDialog`, `OpenTicketsListDialog`, `DrawerPullReportDialog`, `TipsCashoutReportDialog`
- **Data**: `UserDAO`, `GratuityDAO.createReport(...)`

## Uncertainties (STOP; do not guess)
- **Access control**: This dialog does not enforce permission checks itself; where/when it is protected (manager override) is not proven here.

## MagiDesk parity notes
- **What exists today**: Not yet assessed.
- **What differs / missing**: Not yet assessed.

## Porting strategy (PLAN ONLY)
- **Backend requirements**:
  - Cash drop recording and reporting
  - Drawer pull reporting and shift/drawer reconciliation
  - Tips cashout reporting by user/date range
- **UI requirements**:
  - Single manager entry with sub-dialog launches and blocking overlay
- **Constraints for implementers**:
  - Preserve the exact criteria capture for tips report (user + from/to date).
  - Preserve drawer kick as a distinct, quickly reachable function.

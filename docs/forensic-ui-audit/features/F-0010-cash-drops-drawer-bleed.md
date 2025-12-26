# Feature: Cash Drops (Drawer Bleed) management

## Classification
- **Parity classification**: PARITY REQUIRED
- **MagiDesk status**: UNCERTAIN (not assessed yet)

## Problem / Why this exists (grounded)
- **Operational need**: Record mid-shift cash removals (“drawer bleed”) per terminal to reduce drawer exposure and maintain reconciliation.
- **Evidence**: Dialog title/border reference “Cash Drops” for terminal; persists `CashDropTransaction` with terminal/user/time/amount.

## User-facing surfaces
- **Surface type**: Dialog (modal)
- **UI entry points**: Via `ManagerDialog.doShowCashDrops()`.
- **Controls observed**:
  - New Cash Drop (prompts amount)
  - Delete Selected
  - Close
  - Scroll up/down
  - Table list of unsettled cash drops

## Preconditions & protections
- **Data scope**: Loads only “unsettled” cash drops for the current terminal: `CashDropTransactionDAO.findUnsettled(terminal)`.
- **Selection**: Single selection in the list.

## Step-by-step behavior (forensic)
1. On open, `initDate()` loads unsettled cash drops for the current terminal.
2. **New Cash Drop**:
   - Opens `NumberSelectionDialog2` with floating point enabled.
   - Creates `CashDropTransaction` with:
     - `drawerResetted=false`
     - `terminal=Application.getTerminal()`
     - `user=Application.getCurrentUser()`
     - `transactionTime=now`
     - `amount=<entered>`
     - `paymentType=PaymentType.CASH.toString()`
   - Persists via `CashDropTransactionDAO.saveNewCashDrop(transaction, terminal)`.
3. **Delete Selected**:
   - Deletes selected transaction via `CashDropTransactionDAO.deleteCashDrop(transaction, Application.refreshAndGetTerminal())`.
4. Scroll up/down rotates selection through list and scrolls to visible.

## Edge cases & failure paths
- Any exception on create/delete shows `POSMessageDialog.showError(..., Messages.getString("CashDropDialog.16"/"18"), e)`.

## Data / audit / financial impact
- **Writes/updates**: Creates/deletes `CashDropTransaction` records tied to terminal/user.
- **Financial risk**: Incorrect handling causes cash-in-drawer reconciliation errors and shrink exposure.

## Code traceability (REQUIRED)
- **Primary UI**: `com.floreantpos.ui.dialog.CashDropDialog` → `/projects/Code/Redesign-POS/floreantpos/src/com/floreantpos/ui/dialog/CashDropDialog.java`
- **DAO**: `CashDropTransactionDAO.findUnsettled`, `.saveNewCashDrop`, `.deleteCashDrop`
- **Entry**: `com.floreantpos.ui.dialog.ManagerDialog.doShowCashDrops()`

## Uncertainties (STOP; do not guess)
- **Meaning of `drawerResetted` / “unsettled”**: Settlement rules are in DAO/services not inspected here.

## MagiDesk parity notes
- **What exists today**: Not yet assessed.

## Porting strategy (PLAN ONLY)
- **Backend requirements**: Cash drop transaction entity with terminal/user/time/amount; ability to query “unsettled” and delete.
- **UI requirements**: A list dialog scoped to terminal with quick add (numeric prompt) and delete.
- **Constraints for implementers**: Preserve scoping to current terminal and default payment type CASH for cash drops.

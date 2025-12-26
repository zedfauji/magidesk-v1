# Feature: Drawer Assignment Action (F-0065)

## Classification
- **Parity classification**: PARITY REQUIRED
- **MagiDesk status**: PARTIAL (Cash session exists but assignment dialog may differ)

## Problem / Why this exists (grounded)
- **Operational need**: Before processing cash transactions, a cash drawer must be assigned to the terminal with a starting balance. This enables cash accountability at end of shift.
- **Evidence**: `DrawerAssignmentAction.java` + `MultiCurrencyAssignDrawerDialog.java` - assigns drawer to user/terminal; sets initial balance; supports multi-currency.

## User-facing surfaces
- **Surface type**: Action + dialog
- **UI entry points**: Login screen (if drawer required); Manager functions; prompted when cash payment attempted
- **Exit paths**: Assign / Cancel

## Preconditions & protections
- **User/role/permission checks**: Drawer assignment permission
- **State checks**: No existing open drawer for terminal; terminal configured
- **Manager override**: Not typically required

## Step-by-step behavior (forensic)
1. System detects drawer not assigned (or user initiates)
2. DrawerAssignmentAction opens
3. Dialog prompts for:
   - Initial balance (counted cash in drawer)
   - User assignment (who is responsible)
   - Multi-currency amounts (if enabled)
4. On confirm:
   - CashDrawer record created
   - Terminal.assignedDrawer set
   - Terminal.currentBalance set
   - User now able to process cash
5. Drawer can be pulled at shift end

## Edge cases & failure paths
- **Drawer already assigned**: Show existing, offer to close first
- **Zero balance**: Allowed (some start empty)
- **Multi-currency mismatch**: Validate all configured currencies

## Data / audit / financial impact
- **Writes/updates**: CashDrawer entity; Terminal.assignedDrawer; Terminal.currentBalance
- **Audit events**: Drawer assignment logged
- **Financial risk**: Incorrect starting balance affects reconciliation; accountability gap

## Code traceability (REQUIRED)
- **Primary UI class(es)**: 
  - `DrawerAssignmentAction` → `actions/DrawerAssignmentAction.java`
  - `MultiCurrencyAssignDrawerDialog` → `ui/dialog/MultiCurrencyAssignDrawerDialog.java`
- **Entry action(s)**: Called from login flow, payment view, manager dialog
- **Workflow/service enforcement**: CashDrawerDAO; Terminal updates
- **Messages/labels**: Balance prompts, currency labels

## Uncertainties (STOP; do not guess)
- Blind balance entry vs. counted entry
- Manager verification of starting count

## MagiDesk parity notes
- **What exists today**: Cash session concept exists in MagiDesk
- **What differs / missing**: Multi-currency drawer assignment; explicit assignment dialog

## Porting strategy (PLAN ONLY)
- **Backend requirements**: 
  - AssignDrawerCommand with terminalId, userId, balances
  - CashSession creation with opening balance
- **API/DTO requirements**: POST /cash-sessions/open
- **UI requirements**: DrawerAssignmentDialog with balance entry
- **Constraints for implementers**: Must have open drawer for cash payments

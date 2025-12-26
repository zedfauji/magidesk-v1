# Feature: Payout Dialog (F-0062)

## Classification
- **Parity classification**: PARITY REQUIRED
- **MagiDesk status**: MISSING

## Problem / Why this exists (grounded)
- **Operational need**: Managers need to record cash payouts from the drawer for legitimate business expenses (deliveries, supplies, vendor payments). This must be tracked for drawer reconciliation.
- **Evidence**: `PayoutDialog.java` - records payout with reason, recipient, amount, note. Creates PayOutTransaction with DEBIT type. Updates terminal balance. Logs to ActionHistory.

## User-facing surfaces
- **Surface type**: Modal dialog
- **UI entry points**: ManagerDialog → Payout button; also via PayoutAction from switchboard (permission-gated)
- **Exit paths**: OK/Finish (records payout) / Cancel (no change)

## Preconditions & protections
- **User/role/permission checks**: Requires payout permission (checked via UserPermission in PayoutAction)
- **State checks**: Drawer must be assigned; sufficient cash balance assumed
- **Manager override**: Action itself requires manager-level permission

## Step-by-step behavior (forensic)
1. User with permission clicks Payout button
2. PayoutDialog opens with PayOutView form
3. User enters:
   - Payout amount (required, numeric)
   - Reason (selected from PayoutReason list)
   - Recipient (selected from PayoutRecepient list)
   - Note (optional text)
4. On OK/Finish:
   - Terminal balance reduced by payout amount
   - PayOutTransaction created with:
     - paymentType = CASH
     - transactionType = DEBIT
     - reason, recipient, note, amount
     - user, terminal, timestamp
   - Transaction saved via PayOutTransactionDAO
   - ActionHistory logged with PAY_OUT action
5. Dialog closes

## Edge cases & failure paths
- **Zero/negative amount**: Should be validated (not visible in code excerpt)
- **Insufficient drawer cash**: Not explicitly validated (drawer can go negative)
- **No reason/recipient selected**: May proceed with null values
- **Database error**: Exception shown to user, transaction not saved

## Data / audit / financial impact
- **Writes/updates**: 
  - Terminal.currentBalance reduced
  - PayOutTransaction created
  - ActionHistory created
- **Audit events**: ActionHistory.PAY_OUT with amount logged
- **Financial risk**: Unauthorized payouts; no approval workflow; drawer shortage

## Code traceability (REQUIRED)
- **Primary UI class(es)**: `PayoutDialog` → `ui/dialog/PayoutDialog.java`; `PayOutView` → `ui/views/PayOutView.java`
- **Entry action(s)**: `PayoutAction` → `actions/PayoutAction.java`
- **Workflow/service enforcement**: `PayOutTransactionDAO.saveTransaction()`
- **Messages/labels**: `PayoutDialog.2` (amount label)

## Uncertainties (STOP; do not guess)
- PayoutReason and PayoutRecepient sources (likely DB-driven lists)
- Whether payout can exceed drawer balance

## MagiDesk parity notes
- **What exists today**: No payout functionality
- **What differs / missing**: Entire payout workflow missing - dialog, transaction type, reason/recipient entities

## Porting strategy (PLAN ONLY)
- **Backend requirements**: 
  - PayoutReason, PayoutRecipient entities
  - CreatePayoutCommand with amount, reasonId, recipientId, note
  - Debit Cash Session balance
  - Audit event for payout
- **API/DTO requirements**: POST /payouts endpoint
- **UI requirements**: PayoutDialog with amount input, reason dropdown, recipient dropdown, notes field
- **Constraints for implementers**: Must affect drawer accountable calculation; must be included in drawer pull report

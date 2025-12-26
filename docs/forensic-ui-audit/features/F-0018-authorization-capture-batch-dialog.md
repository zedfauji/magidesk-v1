# Feature: Authorization capture batch dialog (background thread)

## Classification
- **Parity classification**: PARITY REQUIRED
- **MagiDesk status**: UNCERTAIN (not assessed yet)

## Problem / Why this exists (grounded)
- **Operational need**: Provide an operator-visible batch capture process for previously authorized transactions, with status logging and a safe “finish” only once work completes.
- **Evidence**: `AuthorizationDialog` starts a background thread on show, iterates transactions, captures auth amounts via gateway processor, and appends per-transaction status to a text area.

## User-facing surfaces
- **Surface type**: Dialog
- **UI entry points**: Not proven here; likely called from “transactions/authorizations” workflows.

## Preconditions & protections
- **Non-closeable**: `DO_NOTHING_ON_CLOSE`.
- **Finish button hidden until done**.

## Step-by-step behavior (forensic)
1. When set visible true, starts a new thread running `run()`.
2. For each transaction:
   - Reads `transaction.getCardReader()` → `CardReader.fromString(...)`.
   - If `CardReader.EXTERNAL_TERMINAL`:
     - Marks `transaction.captured=true` and saves.
     - Writes success line to status area.
     - Continues without calling gateway.
   - Else:
     - Uses `CardConfig.getPaymentGateway().getProcessor()` and calls `captureAuthAmount(transaction)`.
     - Marks captured true, saves, appends success.
     - Sleeps 6 seconds between transactions when there is a next transaction.
   - On exception: appends failure line with exception message.
3. On completion: label changes to completion message and Finish button becomes visible.

## Data / audit / financial impact
- **Writes/updates**: Marks transactions captured and persists via `PosTransactionDAO.saveOrUpdate`.
- **Financial risk**: Incorrect capture logic can leave authorizations uncaptured or double-captured.

## Code traceability (REQUIRED)
- **Primary UI**: `com.floreantpos.ui.views.payment.AuthorizationDialog` → `/projects/Code/Redesign-POS/floreantpos/src/com/floreantpos/ui/views/payment/AuthorizationDialog.java`
- **DAO**: `PosTransactionDAO.saveOrUpdate`
- **Gateway**: `CardConfig.getPaymentGateway().getProcessor().captureAuthAmount(...)`

## Uncertainties (STOP; do not guess)
- **Where transactions list comes from**: Call site not traced here; determining which transactions qualify requires inspecting that workflow.

## MagiDesk parity notes
- **What exists today**: Not yet assessed.

## Porting strategy (PLAN ONLY)
- **Backend requirements**: Authorization transaction lifecycle with explicit “captured” state and capture operation.
- **UI requirements**: Non-closeable progress dialog with per-transaction status log.
- **Constraints for implementers**: Preserve external-terminal short-circuit behavior and 6-second pacing between gateway captures.

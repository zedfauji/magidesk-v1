# Feature: Progress/Loading Dialog (F-0132)

## Classification
- **Parity classification**: PARITY REQUIRED
- **MagiDesk status**: PARTIAL (loading indicators may exist)

## Problem / Why this exists (grounded)
- **Operational need**: Visual feedback during long operations (processing payment, generating report). Prevent double-clicks.
- **Evidence**: `WaitDialog.java` + `PaymentProcessWaitDialog.java` - blocking wait with progress indicator.

## User-facing surfaces
- **Surface type**: Modal dialog (blocking)
- **UI entry points**: Triggered by long operations
- **Exit paths**: Auto-close on complete; Cancel if allowed

## Preconditions & protections
- **User/role/permission checks**: Inherited
- **State checks**: Operation in progress
- **Manager override**: Not applicable

## Step-by-step behavior (forensic)
1. User initiates long operation
2. ProgressDialog opens:
   - Spinner or progress bar
   - Status message
   - Cancel button (if applicable)
3. Background operation runs
4. Status updates (if applicable)
5. On complete:
   - Dialog closes
   - Result shown
6. On error:
   - Dialog closes
   - Error message shown

## Edge cases & failure paths
- **Timeout**: Cancel option; error message
- **Cancel requested**: Attempt cancellation
- **Operation fails**: Error handling

## Data / audit / financial impact
- **Writes/updates**: None (display only)
- **Audit events**: None
- **Financial risk**: Prevents duplicate transactions

## Code traceability (REQUIRED)
- **Primary UI class(es)**: 
  - `WaitDialog` → `ui/dialog/WaitDialog.java`
  - `PaymentProcessWaitDialog` → `ui/views/payment/PaymentProcessWaitDialog.java`
- **Entry action(s)**: Long operations
- **Workflow/service enforcement**: Async operation wrapper
- **Messages/labels**: Progress messages

## MagiDesk parity notes
- **What exists today**: Some loading indicators
- **What differs / missing**: Consistent progress dialog

## Porting strategy (PLAN ONLY)
- **Backend requirements**: Async operation support
- **API/DTO requirements**: None
- **UI requirements**: ProgressDialog component
- **Constraints for implementers**: Prevents UI interaction during operation

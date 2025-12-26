# Feature: Confirmation Dialog Pattern (F-0131)

## Classification
- **Parity classification**: PARITY REQUIRED
- **MagiDesk status**: EXISTS (confirmation dialogs implemented)

## Problem / Why this exists (grounded)
- **Operational need**: Confirm destructive/important actions before execution. Prevent accidents.
- **Evidence**: `POSMessageDialog.java` + `ConfirmDeleteDialog.java` - confirmation pattern used throughout.

## User-facing surfaces
- **Surface type**: Modal dialog (reusable pattern)
- **UI entry points**: Various destructive actions (void, delete, refund)
- **Exit paths**: Confirm / Cancel

## Preconditions & protections
- **User/role/permission checks**: Inherited from parent action
- **State checks**: Inherited from parent action
- **Manager override**: If parent action requires

## Step-by-step behavior (forensic)
1. User initiates sensitive action
2. ConfirmDialog opens:
   - Question/warning text
   - Yes/No or Confirm/Cancel buttons
   - Clear indication of consequence
3. User decides:
   - Confirm: Action proceeds
   - Cancel: Return without action

## Edge cases & failure paths
- **Timeout**: May auto-cancel
- **Multiple confirmations**: May chain

## Data / audit / financial impact
- **Writes/updates**: None (dialog only)
- **Audit events**: None directly (parent action handles)
- **Financial risk**: Prevention mechanism

## Code traceability (REQUIRED)
- **Primary UI class(es)**: 
  - `POSMessageDialog` → `ui/dialog/POSMessageDialog.java`
  - `ConfirmDeleteDialog` → `ui/dialog/ConfirmDeleteDialog.java`
- **Entry action(s)**: Called by parent actions
- **Workflow/service enforcement**: Action flow
- **Messages/labels**: Confirmation text

## MagiDesk parity notes
- **What exists today**: ContentDialog and confirmation patterns
- **What differs / missing**: May need consistent styling

## Porting strategy (PLAN ONLY)
- **Backend requirements**: None (UI pattern)
- **API/DTO requirements**: None
- **UI requirements**: Reusable ConfirmDialog component
- **Constraints for implementers**: Consistent styling; clear messaging

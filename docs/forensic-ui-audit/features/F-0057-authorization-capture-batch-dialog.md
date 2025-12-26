# Feature: Authorization Capture Batch Dialog (F-0057)

## Classification
- **Parity classification**: PARITY REQUIRED
- **MagiDesk status**: MISSING

## Problem / Why this exists (grounded)
- **Operational need**: Credit card pre-authorizations (bar tabs, hotels) must be captured/settled at end of day. Batch processing of card transactions.
- **Evidence**: `AuthorizeBatchDialog.java` + settlement actions - batch card transaction settlement; capture pre-auths.

## User-facing surfaces
- **Surface type**: Modal dialog
- **UI entry points**: ManagerDialog → Batch Close; End of Day process
- **Exit paths**: Batch submitted / Cancel

## Preconditions & protections
- **User/role/permission checks**: Batch close permission (manager)
- **State checks**: Open pre-auths exist; time window for capture
- **Manager override**: Permission required

## Step-by-step behavior (forensic)
1. Manager initiates batch close
2. AuthorizeBatchDialog opens
3. Shows pending transactions:
   - Pre-authorizations to capture
   - Tip adjustments to finalize
   - Voids to submit
4. Review totals
5. On submit:
   - Transactions sent to processor
   - Capture commands executed
   - Confirmations received
   - Batch report generated
6. Results displayed

## Edge cases & failure paths
- **Expired pre-auth**: May fail capture
- **Processor timeout**: Retry or manual
- **Partial batch failure**: Some succeed, some fail
- **Duplicate batch**: Prevented

## Data / audit / financial impact
- **Writes/updates**: Transaction status; settlement records
- **Audit events**: Batch closure logged
- **Financial risk**: Missed captures; duplicate settlements

## Code traceability (REQUIRED)
- **Primary UI class(es)**: `AuthorizeBatchDialog` → `ui/views/payment/AuthorizeBatchDialog.java`
- **Entry action(s)**: Manager actions
- **Workflow/service enforcement**: Card processor batch API
- **Messages/labels**: Batch labels

## MagiDesk parity notes
- **What exists today**: No card batch processing
- **What differs / missing**: Entire batch capture system

## Porting strategy (PLAN ONLY)
- **Backend requirements**: 
  - Batch settlement command
  - Pre-auth tracking
  - Processor integration
- **API/DTO requirements**: POST /card-batch/close
- **UI requirements**: BatchCloseDialog
- **Constraints for implementers**: Must capture within processor timeframe

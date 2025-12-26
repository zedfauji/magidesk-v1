# Feature: Card Signature Capture (F-0059)

## Classification
- **Parity classification**: DEFER (hardware dependent)
- **MagiDesk status**: MISSING

## Problem / Why this exists (grounded)
- **Operational need**: Capture customer signature for card transactions above threshold. Legal requirement for chargebacks.
- **Evidence**: Signature capture integration + `SignaturePanel.java` - capture and store signatures.

## User-facing surfaces
- **Surface type**: Modal dialog with signature pad
- **UI entry points**: After card payment above threshold
- **Exit paths**: Signature captured / Skip (if allowed)

## Preconditions & protections
- **User/role/permission checks**: Payment permission
- **State checks**: Card payment completed; above signature threshold
- **Manager override**: Skip may require approval

## Step-by-step behavior (forensic)
1. Card payment processed
2. If above signature threshold:
   - SignatureCapture dialog opens
   - Shows signature pad (hardware or touch)
   - "Sign here" prompt
3. Customer signs
4. Signature captured
5. Signature stored with transaction
6. Receipt prints with signature (optional)

## Edge cases & failure paths
- **Signature hardware failure**: Skip or retry
- **Below threshold**: Skip signature
- **Skip requested**: Manager approval

## Data / audit / financial impact
- **Writes/updates**: Signature image with transaction
- **Audit events**: Signature capture logged
- **Financial risk**: Chargeback defense

## Code traceability (REQUIRED)
- **Primary UI class(es)**: `SignaturePanel` → signature capture
- **Entry action(s)**: Card payment flow
- **Workflow/service enforcement**: Signature storage
- **Messages/labels**: Signature prompts

## MagiDesk parity notes
- **What exists today**: No signature capture
- **What differs / missing**: Entire signature system

## Porting strategy (PLAN ONLY)
- **Backend requirements**: Signature image storage
- **API/DTO requirements**: Signature with transaction
- **UI requirements**: Signature capture panel
- **Constraints for implementers**: Hardware integration; image storage

# Feature: <Feature Name>

## Classification
- **Parity classification**: PARITY REQUIRED | PARITY WITH MODERNIZATION | DEFER | REJECT
- **MagiDesk status**: EXISTS | PARTIAL | MISSING

## Problem / Why this exists (grounded)
- **Operational need**: <what real-world operator problem it solves>
- **Evidence**: <FloreantPOS references that imply the need (messages/actions/labels/workflow constraints)>

## User-facing surfaces
- **Surface type**: Screen | Dialog | Modal | Popup | Context menu | Shortcut | Auto-triggered UI
- **UI entry points**: <menus/buttons/actions/shortcuts>
- **Exit paths**: <close/cancel/commit>

## Preconditions & protections
- **User/role/permission checks**: <who can access>
- **State checks**: <ticket state, payment state, shift/drawer requirements>
- **Manager override**: <required? when?>

## Step-by-step behavior (forensic)
1. <step>
2. <step>

## Edge cases & failure paths
- <case>: <what happens>

## Data / audit / financial impact
- **Writes/updates**: <models affected>
- **Audit events**: <ActionHistory, transactions, etc>
- **Financial risk**: <how money can be miscounted if wrong>

## Code traceability (REQUIRED)
- **Primary UI class(es)**: <class → file>
- **Entry action(s)**: <action class → file>
- **Workflow/service enforcement**: <service/util/DAO classes>
- **Messages/labels**: <i18n keys if used>

## Uncertainties (STOP; do not guess)
- <uncertainty> (with exact code pointers)

## MagiDesk parity notes
- **What exists today**: <view/viewmodel/command references>
- **What differs / missing**: <precise deltas>

## Porting strategy (PLAN ONLY)
- **Backend requirements**: <commands, invariants, rules>
- **API/DTO requirements**: <new endpoints/contracts>
- **UI requirements**: <new screens/dialogs; nav impact>
- **Constraints for implementers**: <non-negotiable behavior constraints>

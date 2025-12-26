# Backend Forensic Audit: F-0022 Modifier Group Selection Dialog

## Feature Context
- **Feature**: Modifier Group Selection Dialog
- **Trace from**: `F-0022-modifier-group-selection-dialog.md`
- **Reference**: `ModifierGroupSelectionDialog.java`

## Backend Invariants
1.  **Rule Enforcement**: The backend MUST reject an item addition if required modifiers (Min > 0) are missing.
2.  **Max Constraint**: The system cannot persist more than `MaxQuantity` modifiers for a group.
3.  **Pricing**: Modifiers can add price (or be zero). The calculation strategy (Add, Highest, etc.) must be consistent.

## Forbidden States
-   **Incomplete Item**: A "Steak" ordered without a "Temperature" (if Temp is a required modifier group).
-   **Excess Selection**: 3 Sides selected when Max=2.

## Audit Requirements
-   **Event**: `MODIFIER_ADDED` (Implicit in Ticket Update).

## Concurrency Semantics
-   **Menu Change**: If a modifier goes Out of Stock while the dialog is open, the Save attempt must fail.

## MagiDesk Backend Parity
-   **Validation**: ⚠️ Need to ensure `TicketDomainService` validates Modifier constraints on Save, not just the UI.

## Alignment Strategy
1.  **Implement** `ModifierConstraintValidator` in the Domain Layer.

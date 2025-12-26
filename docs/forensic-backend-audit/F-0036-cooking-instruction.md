# Backend Forensic Audit: F-0036 Cooking Instruction Dialog

## Feature Context
- **Feature**: Cooking Instruction Dialog
- **Trace from**: `F-0036-cooking-instruction-dialog.md`
- **Reference**: `CookingInstructionDialog.java`

## Backend Invariants
1.  **Item Association**: Instructions are not global; they bind to specific line items.
2.  **Persistence**: Notes MUST serve as "Kitchen Instructions" and print on the kitchen ticket.

## Forbidden States
-   **Truncation**: Silently truncating long instructions without warning.

## Audit Requirements
-   **Event**: None.

## Concurrency Semantics
-   **None**.

## MagiDesk Backend Parity
-   **Logic**: ✅ `TicketItem` entity supports notes.

## Alignment Strategy
1.  **Consolidate** with F-0023. This appears to be the "Free Text" version vs "Predefined Selection".

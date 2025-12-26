# Backend Forensic Audit: F-0023 Cooking Instruction Selection Dialog

## Feature Context
- **Feature**: Cooking Instruction Selection Dialog
- **Trace from**: `F-0023-cooking-instruction-selection-dialog.md`
- **Reference**: `CookingInstructionSelectionView.java`

## Backend Invariants
1.  **Association**: Instructions must directly link to a specific `TicketItem`.
2.  **Persistence**: Instructions are text (or ref IDs) and must persist through to the Kitchen Ticket.

## Forbidden States
-   **Lost Instruction**: Saving a ticket but dropping the "No Onion" note.

## Audit Requirements
-   **Event**: None specific, part of Item Update.

## Concurrency Semantics
-   **None**.

## MagiDesk Backend Parity
-   **Model**: ✅ `TicketItem` has `Notes` or `Instructions` collection.

## Alignment Strategy
1.  **Standardize** difference between "Free Text Note" and "Predefined Instruction" (one might be a string, other a ref).

# Backend Forensic Audit: F-0050 Void Button

## Feature Context
- **Feature**: Void Button
- **Trace from**: `F-0050-void-button.md`
- **Reference**: `VoidTicketAction.java`

## Backend Invariants
1.  **Trigger**: Invokes F-0013 (Void Ticket Dialog).
2.  **Availability**: Only visible/active if Ticket is Open/Saved. Forbidden if Closed.

## Forbidden States
-   **None** (Action wrapper).

## Audit Requirements
-   **Event**: None (Handled by F-0013).

## Concurrency Semantics
-   **None**.

## MagiDesk Backend Parity
-   **Logic**: Shortcut to F-0013 workflow.

## Alignment Strategy
1.  **None**.

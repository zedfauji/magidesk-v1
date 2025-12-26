# Backend Forensic Audit: F-0041 Quick Pay Action

## Feature Context
- **Feature**: Quick Pay Action
- **Trace from**: `F-0041-quick-pay-action.md`
- **Reference**: `QuickPayAction.java`

## Backend Invariants
1.  **Atomicity**: Orchestrates `CreateTicket` -> `Settle` -> `Close` (if implicit) or `Settle` -> `Close` (if ticket exists).
2.  **Default Tender**: Usually implies CASH unless configured otherwise.

## Forbidden States
-   **Partial**: If "Quick Pay" is clicked, it usually means "Pay Full Balance".

## Audit Requirements
-   **Event**: `QUICK_PAY` (Mapped to Payment Received).

## Concurrency Semantics
-   **Locking**: High priority lock.

## MagiDesk Backend Parity
-   **Command**: ✅ Reuse `ProcessPaymentCommand`.

## Alignment Strategy
1.  **Treat** as UI Shortcut only.

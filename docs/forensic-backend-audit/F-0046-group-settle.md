# Backend Forensic Audit: F-0046 Group Settle Ticket Dialog

## Feature Context
- **Feature**: Group Settle Ticket Dialog
- **Trace from**: `F-0046-group-settle-ticket-dialog.md`
- **Reference**: `GroupSettleTicketDialog.java`

## Backend Invariants
1.  **Batch Semantic**: Paying N tickets with 1 transaction (e.g., "Pay for Table 1 and Table 2 together").
2.  **Distributability**: The payment must be distributed to each ticket. The tickets are NOT merged.
3.  **Atomicity**: All tickets settle, or none.

## Forbidden States
-   **Partial Success**: Table 1 paid, Table 2 still open (after a single card swipe).

## Audit Requirements
-   **Event**: `GROUP_SETTLE`
    -   Payload: TicketIds[], TotalAmount.
    -   Severity: INFO.

## Concurrency Semantics
-   **Multi-Lock**: Must require lock on ALL target tickets.

## MagiDesk Backend Parity
-   **Command**: ⚠️ `GroupSettleCommand` likely missing.

## Alignment Strategy
1.  **Implement** `BatchPaymentDomainService`.

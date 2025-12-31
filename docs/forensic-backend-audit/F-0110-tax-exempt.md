# Backend Forensic Audit: F-0110 Tax Exempt Button

## Feature Context
- **Feature**: Tax Exempt Button
- **Trace from**: `F-0047-tax-exempt-button.md` (moved from F-0047 due to ID conflict)
- **Reference**: `TaxExemptAction.java`

## Backend Invariants
1.  **Recalculation**: Setting `TaxExempt = true` MUST immediately zero out all auto-calculated taxes.
2.  **Persistence**: The flag must be stored on the Ticket.
3.  **Identifier**: Ideally, a "Tax ID" or "Reason" should be captured.

## Forbidden States
-   **Partial Exempt**: Generally, "Tax Exempt" is all-or-nothing on the Ticket. Line-item tax changes are done via Tax Groups.

## Audit Requirements
-   **Event**: `TAX_EXEMPT_SET`
    -   Payload: TicketId, AuthorizedBy.
    -   Severity: WARN.

## Concurrency Semantics
-   **None**.

## MagiDesk Backend Parity
-   **Logic**: ✅ `TaxDomainService` needs to respect the flag.

## Alignment Strategy
1.  **Ensure** `TaxCalculator` checks `Ticket.IsTaxExempt`.

# Backend Forensic Audit: F-0096 Credit Card Report

## Feature Context
- **Feature**: Credit Card Report
- **Trace from**: `F-0096-credit-card-report.md`
- **Reference**: `CreditCardReport.java`

## Backend Invariants
1.  **Source**: Auth/Capture Transactions (F-0018).
2.  **Fields**: Card Type (Visa/MC), Last4, Amount, AuthCode.

## Forbidden States
-   **PCI Violation**: Storing/Printing Full PAN.

## Audit Requirements
-   **Event**: None.

## Concurrency Semantics
-   **None**.

## MagiDesk Backend Parity
-   **Query**: ✅ Exists.

## Alignment Strategy
1.  **Ensure** Masking.

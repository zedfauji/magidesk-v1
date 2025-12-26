# Backend Forensic Audit: F-0107 Card Configuration View

## Feature Context
- **Feature**: Card Configuration View
- **Trace from**: `F-0107-card-configuration-view.md`
- **Reference**: `MerchantGatewayConfig.java`

## Backend Invariants
1.  **Security**: API Keys/Secrets MUST be encrypted at rest.
2.  **PCI**: Never store "Test Mode" credentials in Production environment.

## Forbidden States
-   **Cleartext**: Storing Gateway Secret in plain text.

## Audit Requirements
-   **Event**: `GATEWAY_CONFIG_UPDATED`.
    -   Payload: redacted.

## Concurrency Semantics
-   **None**.

## MagiDesk Backend Parity
-   **Model**: ⚠️ `MerchantProfile` needed.

## Alignment Strategy
1.  **Encrypt** secrets using DPAPI or similar.

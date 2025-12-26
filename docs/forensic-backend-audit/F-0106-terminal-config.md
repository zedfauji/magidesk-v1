# Backend Forensic Audit: F-0106 Terminal Configuration View

## Feature Context
- **Feature**: Terminal Configuration View
- **Trace from**: `F-0106-terminal-configuration-view.md`
- **Reference**: `TerminalConfig.java`

## Backend Invariants
1.  **Identity**: Bound to `TerminalKey` or Hostname.
2.  **Role**: Defines if this node is "Kitchen Display", "Server Station", "Cashier".

## Forbidden States
-   **Duplicate ID**: Two active terminals claiming ID "TERM-01".

## Audit Requirements
-   **Event**: `TERMINAL_CONFIG_UPDATED`.

## Concurrency Semantics
-   **None**.

## MagiDesk Backend Parity
-   **Model**: ✅ `Terminal` entity.

## Alignment Strategy
1.  **None**.

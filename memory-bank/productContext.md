# Product Context

## Operational Environment
Magidesk is a Point-of-Sale (POS) system designed for restaurants and retail environments. It operates in a high-paced, high-stress environment where speed, reliability, and accuracy are paramount.

## Critical Success Factors
1.  **Financial Integrity**: calculation of taxes, discounts, and totals must be mathematically precise and auditable.
2.  **Performance**: UI interactions must be instantaneous (sub-100ms) to support rapid order entry.
3.  **Reliability**: The system must be robust against network failures and improper shutdowns.
4.  **Security**: Strict role-based access control and audit trails for all sensitive actions (voids, comps, payouts).

## User Personas
-   **Server/Cashier**: Needs speed and minimal friction.
-   **Manager**: Needs control, override capabilities, and reporting.
-   **Admin**: Needs configuration and system health monitoring.
-   **Forensic Auditor**: Needs to verify that the system creates an immutable trail of events.

## Key Workflows
-   **Order Entry**: Creating and modifying tickets.
-   **Payment Processing**: Accepting various tender types, splitting tickets, handling tips.
-   **Shift Management**: Clock-in/out, drawer pulls, end-of-day closing.
-   **Back Office**: 
    -   Menu management (Complex Modifiers, Pizza, Combos).
    -   Reporting (Sales, Labor, Delivery, Productivity, Logs).
    -   Configuration (Backup/Restore, Fiscal Logging).

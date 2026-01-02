# Consolidated Master Forensic Table
**Audit Date:** 2026-01-01

| Feature | Domain | Backend | Frontend | Workflow | Navigation | Parity Status | Gap Type | Known Gaps | Risk |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| **Ticket Creation** | Orders | FULL | FULL | FULL | FULL | **FULL** | - | None | LOW |
| **Simple Modifiers** | Orders | FULL | FULL | FULL | FULL | **FULL** | - | None | LOW |
| **Pizza Builder** | Orders | PARTIAL | MISSING | MISSING | MISSING | **PARTIAL** | PARITY | Complex fraction pricing, UIs | HIGH |
| **Split Ticket** | Orders | FULL | FULL | FULL | FULL | **FULL** | - | _Backup files present_ | LOW |
| **Void Ticket** | Orders | FULL | FULL | FULL | FULL | **FULL** | - | Audit log visibility? | MED |
| **Delivery Dispatch** | Orders | PARTIAL | MISSING | PARTIAL | MISSING | **PARTIAL** | WORKFLOW | No Dispatch UI, Zone Logic | HIGH |
| **Cash Payment** | Payments | FULL | FULL | FULL | FULL | **FULL** | - | None | LOW |
| **Card (Swipe)** | Payments | FULL | FULL | FULL | FULL | **FULL** | - | Gateway specific logic | LOW |
| **Refunds** | Payments | FULL | FULL | FULL | FULL | **FULL** | - | Manager Approval Gate? | MED |
| **Multi-Currency** | Finance | MISSING | MISSING | MISSING | MISSING | **MISSING** | PARITY | Entire Domain Missing | CRIT |
| **Cash Drawer** | Finance | FULL | FULL | FULL | FULL | **FULL** | - | Strict Assignment? | MED |
| **Tips/Gratuity** | Finance | FULL | FULL | FULL | FULL | **FULL** | - | Auto-Gratuity Logic? | LOW |
| **KDS Routing** | Kitchen | FULL | FULL | FULL | FULL | **FULL** | - | Printer Group mapping | LOW |
| **Inventory Items** | Inventory | FULL | FULL | FULL | FULL | **FULL** | - | None | LOW |
| **Purchase Orders** | Inventory | MISSING | MISSING | MISSING | MISSING | **MISSING** | WORKFLOW | No PO Lifecycle | HIGH |
| **Table Map** | Tables | FULL | FULL | FULL | FULL | **FULL** | - | "Back to Map" Nav flow | MED |
| **Reservations** | Tables | MISSING | MISSING | MISSING | MISSING | **MISSING** | PARITY | No Booking System | HIGH |
| **Sales Reports** | Reports | FULL | FULL | FULL | FULL | **FULL** | - | Export formats | LOW |
| **User Mgmt** | System | FULL | FULL | FULL | FULL | **FULL** | - | Role granularity | LOW |
| **DB Backup** | System | MISSING | MISSING | MISSING | MISSING | **MISSING** | PARITY | No Tooling | CRIT |

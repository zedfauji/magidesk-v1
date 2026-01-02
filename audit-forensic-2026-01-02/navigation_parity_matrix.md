# Navigation Parity Analysis
**Audit Date:** 2026-01-01

## 1. Navigation Parity Matrix

| Flow | FloreantPOS | Magidesk | Parity | Notes |
| :--- | :--- | :--- | :--- | :--- |
| **Startup** | Login -> Switchboard | Login -> Switchboard | **FULL** | Identical high-level flow. |
| **Order Entry** | Switchboard -> Ticket Detail | Switchboard -> Order Entry | **FULL** | Identical. |
| **Payment** | Order -> Settle View | Order -> Settle Pay/View | **FULL** | Identical. |
| **Backoffice** | Switchboard -> Backoffice Tabs | Switchboard -> Backoffice Tabs | **FULL** | Structure preserved. |
| **Split Ticket** | Dialog on Order Screen | Dialog on Order Screen | **FULL** | Identical modal interaction. |
| **Reservation** | Table Map -> Assign Booking | **N/A** | **MISSING** | Path does not exist. |

## 2. Orphan Pages
*Pages found in codebase but potentially unreachable or disconnected:*
- `AuthorizationCaptureBatchDialog`: Is this wired to a button? Reference count check needed.
- `AuditConsole`: (Hypothetical) - Floreant has extensive logs. Magidesk has `AuditEvent` entity but no "Audit Viewer" page found.

## 3. Miswired / Divergent Navigation
- **Manager Functions:**
  - **Floreant:** Clicking "Manager" prompt password -> Opens Full Backoffice Window.
  - **Magidesk:** Clicking "Manager" -> Opens `ManagerFunctionsDialog` (Small list of ops like 'Open Drawer', 'Z-Report') OR `BackOfficePage`.
  - **Analysis:** Magidesk actually *improves* this by separating "Manager Operational Actions" (Dialog) from "Back Office Admin" (Page). However, for parity, we must ensure all Floreant "Manager" button functions are available in one of those two paths. Currently `Open Drawer` is in the dialog, but `Database Config` is missing.

# Feature-Level Parity Matrix
**Audit Date:** 2026-01-01
**Baseline:** FloreantPOS
**Target:** Magidesk

| Feature Domain | Feature Name | Backend Parity | Frontend Parity | Workflow Parity | Overall Status | Notes / Gaps |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| **Orders** | Ticket Creation | FULL | FULL | FULL | **FULL** | Basic creation logic matches. |
| | Modifiers | FULL | FULL | FULL | **FULL** | Both have Modifier selection & Groups. |
| | Pizza Builder | PARTIAL | MISSING | MISSING | **PARTIAL** | Magidesk lacks specialized "Pizza" pricing/UI logic. |
| | Cooking Instructions | FULL | FULL | FULL | **FULL** | `CookingInstruction` vs `KitchenOrder`. |
| | Ticket Splitting | FULL | FULL | FULL | **FULL** | `SplitTicketDialog` exists in both. |
| | Ticket Voiding | FULL | FULL | FULL | **FULL** | `VoidTicket` commands exist. |
| | Delivery | PARTIAL | MISSING | PARTIAL | **PARTIAL** | Magidesk has commands but no dedicated `DeliveryDispatch` UI found. |
| **Payments** | Cash Transaction | FULL | FULL | FULL | **FULL** | `CashPayment`, `CashEntryDialog`. |
| | Card Transaction | FULL | FULL | FULL | **FULL** | `SwipeCardDialog`, `MerchantGateway`. |
| | Split Payment | PARTIAL | PARTIAL | PARTIAL | **PARTIAL** | Magidesk allows partials but workflow differs from Floreant's rigid flow. |
| | Refunds | FULL | FULL | FULL | **FULL** | `RefundPaymentCommand`. |
| | Payouts | FULL | FULL | FULL | **FULL** | `Payout` entity and command exist. |
| | Coupons/Discounts | FULL | FULL | FULL | **FULL** | `Discount` entity and commands. |
| | Tax Exemption | FULL | PARTIAL | FULL | **PARTIAL** | Command exists, UI integration unclear. |
| **Finance** | Cash Drawer/Session | FULL | FULL | FULL | **FULL** | `CashDrawer` (Floreant) vs `CashSession` (Magidesk). |
| | Tips/Gratuity | FULL | FULL | FULL | **FULL** | `Gratuity` entity exists. |
| | Multi-Currency | MISSING | MISSING | MISSING | **MISSING** | No `Currency` logic found in Magidesk. |
| **Kitchen** | KDS Routing | FULL | FULL | FULL | **FULL** | `KitchenRoutingService` matches Floreant logic. |
| | Printer Groups | FULL | PARTIAL | FULL | **PARTIAL** | `PrinterGroup` exists, config UI simplified. |
| **Inventory** | Menu Management | FULL | FULL | FULL | **FULL** | `MenuEditorPage` vs Backoffice Menu. |
| | Inventory Tracking | FULL | FULL | FULL | **FULL** | `InventoryItem`, `InventoryTransaction`. |
| | Recipes | FULL | FULL | FULL | **FULL** | `RecipeLine` vs `Recepie`. |
| | Purchase Orders | MISSING | MISSING | MISSING | **MISSING** | `PurchaseOrder` entity absent in Magidesk. |
| | Vendors | PARTIAL | PARTIAL | PARTIAL | **PARTIAL** | Basic vendor fields likely, but full CRM missing. |
| **Tables** | Floor Plan | FULL | FULL | FULL | **FULL** | `TableMapPage` matches `TableMapView`. |
| | Table Booking | MISSING | MISSING | MISSING | **MISSING** | `TableBookingInfo` (Reservation) logic missing. |
| **Reports** | Sales Reports | FULL | FULL | FULL | **FULL** | `SalesReportsPage` covers key needs. |
| | Payroll/Labor | FULL | PARTIAL | PARTIAL | **PARTIAL** | `Shift` exists, but comprehensive `PayrollReport` less visible. |
| | Inventory Reports | PARTIAL | PARTIAL | PARTIAL | **PARTIAL** | `InventoryPage` exists, deep reporting? |
| **System** | User/Role Mgmt | FULL | FULL | FULL | **FULL** | `UserManagement`, `RoleManagement`. |
| | Terminal Config | FULL | FULL | FULL | **FULL** | `SystemConfigPage`, `Terminal` entity. |
| | Database Backup | MISSING | MISSING | MISSING | **MISSING** | `DataUpdateInfo` / Backup tools missing. |

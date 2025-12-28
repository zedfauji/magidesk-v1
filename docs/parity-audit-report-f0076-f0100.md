# Parity Audit & Gap Analysis Report (F-0076 - F-0100)

**Date:** Dec 27, 2025
**Auditor:** Cascade (Principal Architect)
**Reference System:** FloreantPOS v1.4 (Build 706)
**Target System:** Magidesk (Phase 4 In-Progress)

> **⚠️ CRITICAL DRIFT WARNING**: The misalignment between UI Audit definitions and Backend Audit definitions identified in the previous batch continues significantly in the F-0070s range. This report uses the **UI Audit** definitions as the primary key.

---

## 1. Feature Parity Matrix

| ID | Feature Name (UI Log) | FloreantPOS Behavior | Backend Parity | UI Readiness | Risk Level |
| :--- | :--- | :--- | :--- | :--- | :--- |
| **F-0076** | Multi-Ticket Payment | Pay for Ticket A + Ticket B in one txn. | **MISSING** (No Batch Payment Service) | **NOT IMPLEMENTED** | **LOW** |
| **F-0077** | Customer Selector | Search/Assign Customer to Ticket. | **FULL** (CustomerService exists) | **NOT IMPLEMENTED** | **HIGH** |
| **F-0078** | Customer Form | Create/Edit Customer Details. | **FULL** (Customer Entity/Repo exists) | **NOT IMPLEMENTED** | **HIGH** |
| **F-0079** | Customer Explorer | Browse/Filter Customers. | **FULL** (Query exists) | **NOT IMPLEMENTED** | **MEDIUM** |
| **F-0080** | Change Table | Move Ticket to different Table. | **PARTIAL** (Command missing, Logic implies Table Swap) | **PARTIAL** (Command present, UX broken) | **HIGH** |
| **F-0081** | Floor Explorer | Manage Floor Plans. | **FULL** (TableSection entity exists) | **NOT IMPLEMENTED** | **LOW** |
| **F-0082** | Table Map View | Visual Table Status (Free/Occupied). | **FULL** (Model supports status) | **PARTIAL** (Canvas exists, no interaction) | **HIGH** |
| **F-0083** | Home Delivery View | Dispatch Driver, Track Status. | **PARTIAL** (Ticket Type exists, Workflow missing) | **NOT IMPLEMENTED** | **MEDIUM** |
| **F-0084** | Pickup Order View | Monitor Pickup times. | **PARTIAL** (Ticket Type exists, Workflow missing) | **NOT IMPLEMENTED** | **MEDIUM** |
| **F-0085** | Bar Tab View | Manage named tabs (no tables). | **FULL** (TicketType.BarTab supported) | **NOT IMPLEMENTED** | **LOW** |
| **F-0086** | Seat Selection | Assign Items to Seat 1, 2, 3... | **FULL** (TicketItem.SeatNumber exists) | **NOT IMPLEMENTED** | **HIGH** |
| **F-0087** | Table Browser | Admin List of Tables. | **FULL** (Repo exists) | **NOT IMPLEMENTED** | **LOW** |
| **F-0088** | Kitchen Display (KDS) | Digital Kitchen Screen. | **PARTIAL** (Missing dedicated Query/Service) | **NOT IMPLEMENTED** | **CRITICAL** |
| **F-0089** | Kitchen Ticket | Individual Order visualization. | **PARTIAL** (Visual Logic only) | **NOT IMPLEMENTED** | **MEDIUM** |
| **F-0090** | Kitchen Status | Bump/Recall/Park orders. | **MISSING** (No KDS State Machine) | **NOT IMPLEMENTED** | **HIGH** |
| **F-0091** | Kitchen List | Grid of active orders. | **PARTIAL** (Visual Logic only) | **NOT IMPLEMENTED** | **MEDIUM** |
| **F-0092** | Sales Summary Report | Net Sales, Tax, Tips. | **PARTIAL** (Query Missing/Incomplete) | **PARTIAL** (Basic View) | **HIGH** |
| **F-0093** | Sales Detail Report | Line-item granularity. | **FULL** (Query Exists) | **NOT IMPLEMENTED** | **MEDIUM** |
| **F-0094** | Sales Balance Report | Cash Recon (Expected vs Actual). | **MISSING** (No Reconciliation Service) | **NOT IMPLEMENTED** | **HIGH** |
| **F-0095** | Exception Report | High Voids/Discounts. | **MISSING** (No Fraud Job) | **NOT IMPLEMENTED** | **LOW** |
| **F-0096** | Credit Card Report | Batch Auth Summary. | **FULL** (Query Exists) | **NOT IMPLEMENTED** | **MEDIUM** |
| **F-0097** | Payment Report | Tender type breakdown. | **FULL** (Query Exists) | **NOT IMPLEMENTED** | **MEDIUM** |
| **F-0098** | Menu Usage Report | Product Mix / Velocity. | **FULL** (Query Exists) | **NOT IMPLEMENTED** | **LOW** |
| **F-0099** | Server Productivity | Sales/Tips per Server. | **MISSING** (Query needed) | **NOT IMPLEMENTED** | **LOW** |
| **F-0100** | Hourly Labor Report | Labor Cost vs Sales %. | **MISSING** (Query needed) | **PARTIAL** (Basic View) | **MEDIUM** |

---

## 2. Gap & Drift Report

### Major Functional Gaps
1.  **Kitchen Display System (F-0088 - F-0091)**:
    *   The backend treats "Send to Kitchen" as a fire-and-forget event (Printing).
    *   Magidesk aims for a digital KDS, but the **State Machine** (`New` -> `Cooking` -> `Done` -> `Bumped`) is missing.
    *   *Risk*: Without this, the "Kitchen Display Page" is just a static list that desynchronizes immediately.
2.  **Customer Management (F-0077 - F-0079)**:
    *   The backend has the entities (`Customer`, `Address`), but the UI has **Zero** integration.
    *   *Impact*: Cannot take Delivery orders (which require Customer) or track Loyalty.
3.  **Reporting Engine (F-0092 - F-0100)**:
    *   While some Queries exist, the **Aggregation Service** is fragmented.
    *   The "Sales Balance" (F-0094) is the most critical missing report, as it is the primary tool for End-of-Day reconciliation.

### Documentation Drift
*   **F-0076**: UI="Multi-Ticket Payment" vs Backend="Drawer Balance".
*   **F-0077**: UI="Customer Selector" vs Backend="Table Selection".
*   **Implication**: Backend Audit files F-0076 through F-0081 are analyzing features that do not match the UI Feature ID.
    *   *Example*: Backend F-0077 analysis confirms `TableSelectionView` parity, but the UI is looking for `CustomerSelector`.
    *   *Correction*: I have manually mapped the backend capabilities to the correct UI feature request in the Matrix above (e.g., using Backend F-0079 findings to satisfy UI F-0079).

---

## 3. Critical Blockers List

| Feature | Reason | Remediation Plan |
| :--- | :--- | :--- |
| **F-0082 Table Map** | Primary interface for Dine-In. | **P1**: Wire up `TableStatus` events to the UI Canvas. |
| **F-0088 KDS** | Essential for non-printer kitchens. | **P1**: Implement `KitchenStateService`. |
| **F-0077 Customer** | Essential for Delivery/Takeout. | **P2**: Build `CustomerSelectionDialog`. |

---

## 4. Recommendations

1.  **Reporting Strategy**: Instead of building 10 separate Report Pages, implement a generic **Report Viewer** that accepts a `DataSet` from the backend. The backend already has many of these queries (`GetProductMix`, `GetPayments`).
2.  **KDS Architecture**: Decide NOW if KDS is "Printer Emulation" (stateless) or "Smart Display" (stateful).
    *   *Recommendation*: **Smart Display**. Add `KitchenOrder` entity that tracks start/finish times. This enables `AverageTicketTime` metrics later.
3.  **Table Map**: The current "Canvas" approach is fragile. Ensure the backend exposes `TableCoordinates` via API so the layout is data-driven, not hardcoded XAML.

# Parity Audit & Gap Analysis Report (F-0101 - F-0132)

**Date:** Dec 27, 2025
**Auditor:** Cascade (Principal Architect)
**Reference System:** FloreantPOS v1.4 (Build 706)
**Target System:** Magidesk (Phase 4 In-Progress)

---

## 1. Feature Parity Matrix

| ID | Feature Name | FloreantPOS Behavior | Backend Parity | UI Readiness | Risk Level |
| :--- | :--- | :--- | :--- | :--- | :--- |
| **F-0101** | Tip Report | Tips collected per server. | **FULL** (Query exists) | **NOT IMPLEMENTED** | **LOW** |
| **F-0102** | Attendance Report | Clock In/Out history. | **FULL** (Query exists) | **NOT IMPLEMENTED** | **LOW** |
| **F-0103** | Journal Report | Audit Log chronological view. | **PARTIAL** (Audit Query weak) | **NOT IMPLEMENTED** | **MEDIUM** |
| **F-0104** | Cash Out Report | End-of-shift Server Checkout. | **FULL** (Query exists) | **NOT IMPLEMENTED** | **HIGH** |
| **F-0105** | Restaurant Config | Global settings (Name, Address). | **FULL** (Service exists) | **PARTIAL** (SettingsPage placeholder) | **LOW** |
| **F-0106** | Terminal Config | Local settings (Printer, Device). | **FULL** (Model exists) | **NOT IMPLEMENTED** | **HIGH** |
| **F-0107** | Card Config | Gateway API Keys/Secrets. | **MISSING** (No MerchantProfile) | **NOT IMPLEMENTED** | **HIGH** |
| **F-0108** | Print Config | Map Virtual -> Physical Printers. | **FULL** (Model exists) | **NOT IMPLEMENTED** | **CRITICAL** |
| **F-0109** | Tax Config | Tax Rates & Rules. | **FULL** (Model exists) | **NOT IMPLEMENTED** | **HIGH** |
| **F-0110** | Language Selection | Locale picker. | **FULL** (Logic exists) | **NOT IMPLEMENTED** | **LOW** |
| **F-0111** | Back Office Window | Shell for Admin functions. | **FULL** (Logic exists) | **NOT IMPLEMENTED** | **MEDIUM** |
| **F-0112** | Menu Category Exp | CRUD Menu Categories. | **FULL** (Model exists) | **NOT IMPLEMENTED** | **HIGH** |
| **F-0113** | Menu Group Exp | CRUD Menu Groups. | **FULL** (Model exists) | **NOT IMPLEMENTED** | **HIGH** |
| **F-0114** | Menu Item Exp | CRUD Menu Items (Price, Tax). | **FULL** (Model exists) | **NOT IMPLEMENTED** | **CRITICAL** |
| **F-0115** | Modifier Exp | CRUD Modifiers. | **FULL** (Model exists) | **NOT IMPLEMENTED** | **HIGH** |
| **F-0116** | Modifier Group Exp | CRUD Modifier Groups (Rules). | **FULL** (Model exists) | **NOT IMPLEMENTED** | **HIGH** |
| **F-0117** | Coupon Explorer | CRUD Coupons. | **MISSING** (No Coupon Def model) | **NOT IMPLEMENTED** | **MEDIUM** |
| **F-0118** | Tax Explorer | CRUD Tax Rates. | **FULL** (Model exists) | **NOT IMPLEMENTED** | **HIGH** |
| **F-0119** | Shift Explorer | CRUD Shift Definitions. | **FULL** (Model exists) | **NOT IMPLEMENTED** | **LOW** |
| **F-0120** | User Explorer | CRUD Users & Roles. | **FULL** (Model exists) | **PARTIAL** (Placeholder) | **HIGH** |
| **F-0121** | Order Type Exp | CRUD Order Types (Behavior). | **FULL** (Model exists) | **NOT IMPLEMENTED** | **MEDIUM** |
| **F-0122** | Coupon/Disc Dialog | Apply to Ticket. | **FULL** (Logic exists) | **NOT IMPLEMENTED** | **MEDIUM** |
| **F-0123** | Discount App Dialog | Apply Manual Discount. | **FULL** (Logic exists) | **NOT IMPLEMENTED** | **MEDIUM** |
| **F-0124** | Table Section Config| CRUD Floor Zones. | **FULL** (Model exists) | **NOT IMPLEMENTED** | **LOW** |
| **F-0125** | Notes Dialog | Add Text to Ticket/Item. | **FULL** (Logic exists) | **NOT IMPLEMENTED** | **LOW** |
| **F-0126** | Delivery Zone Config| Zip Codes/Fees. | **MISSING** (No Zone Model) | **NOT IMPLEMENTED** | **MEDIUM** |
| **F-0127** | Printer Group Config| CRUD Printer Routing. | **FULL** (Model exists) | **NOT IMPLEMENTED** | **HIGH** |
| **F-0128** | DB Backup Dialog | Trigger Backup. | **MISSING** (No Service) | **PARTIAL** (Dialog only) | **HIGH** |
| **F-0129** | Message Banner | Footer Ticker. | **FULL** (Logic exists) | **NOT IMPLEMENTED** | **LOW** |
| **F-0130** | About Dialog | Version Info. | **FULL** (Logic exists) | **NOT IMPLEMENTED** | **LOW** |
| **F-0131** | Confirmation Dialog | Reusable "Are you sure?". | **N/A** (UI Pattern) | **PARTIAL** | **LOW** |
| **F-0132** | Progress Dialog | Reusable "Please wait...". | **N/A** (UI Pattern) | **PARTIAL** | **LOW** |

---

## 2. Gap & Drift Report

### Backend Gaps
1.  **Backup Service (F-0128)**: No mechanism to backup the database from the UI. This is critical for POS systems which often run on unreliable hardware.
2.  **Delivery Infrastructure (F-0126)**: Missing the domain modeling for "Delivery Zones" and "Driver Assignment".
3.  **Coupon Definitions (F-0117)**: The discount system is rudimentary. Complex rules (BOGO, Date-Time restrictions) are not modeled.

### UI Alignment Issues
1.  **Missing "Back Office"**: Magidesk has no centralized Admin UI. The backend has all the CRUD models (Menu, Tax, User), but there are no Views to manage them.
    *   *Impact*: The system is currently "ReadOnly" regarding configuration. A user cannot add a new menu item without SQL access.
2.  **Configuration Fragmentation**: Settings are scattered or non-existent. F-0105 through F-0109 (Config Views) are essential for deployment.

### Drift
*   **F-0122/F-0123**: Redundant "Discount" dialogs. Magidesk should consolidate these into a single "Adjustments" workflow.

---

## 3. Critical Blockers List

| Feature | Reason | Remediation Plan |
| :--- | :--- | :--- |
| **F-0114 Menu Editor** | Cannot manage the primary business asset (Menu) without DB access. | **P1**: Build `MenuEditorViewModel` and associated Views. |
| **F-0108 Printer Config** | Cannot route kitchen orders without configuring printers. | **P1**: Build `PrinterSetupView`. |
| **F-0120 User Admin** | Cannot create employees or reset passwords. | **P1**: Finish `UserManagementPage`. |

---

## 4. Execution Timeline Proposal

### Phase 4.1: (No Change)

### Phase 4.2: (No Change)

### Phase 4.3: Back Office (Admin)
*   **Goal**: Enable self-service configuration.
*   Tasks:
    1.  **F-0111**: Build "Back Office" Shell (Tabbed Interface).
    2.  **F-0114**: Menu/Modifier Editor.
    3.  **F-0108**: Hardware Configuration (Printers/Terminals).
    4.  **F-0120**: User Management.

---

## 5. Recommendations

1.  **De-prioritize "View" Reporting**: Focus on **Transactional Reporting** (Cash Out, Sales Summary) first. Analytical reports (Hourly Labor, Menu Usage) can wait or be offloaded to a separate BI tool.
2.  **Consolidate Config**: Instead of 10 separate pages for config, build a unified **Settings Dashboard** with sections for Restaurant, Hardware, and Financials.
3.  **Menu Editor is Key**: The complexity of Menu/Modifier/Group relationships (F-0112 to F-0116) requires a sophisticated UI (Drag & Drop or TreeView). Do not underestimate the effort here.

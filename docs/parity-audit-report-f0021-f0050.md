# Parity Audit & Gap Analysis Report (F-0021 - F-0050)

**Date:** Dec 27, 2025
**Auditor:** Cascade (Principal Architect)
**Reference System:** FloreantPOS v1.4 (Build 706)
**Target System:** Magidesk (Phase 4 In-Progress)

> **⚠️ CRITICAL DRIFT WARNING**: Significant ID mismatches detected between Forensic UI Audit and Forensic Backend Audit files starting at F-0022 and F-0047. This report aligns with the **UI Audit Log** (Parity-Analysis-Log) feature definitions, but notes backend coverage gaps where IDs diverge.

---

## 1. Feature Parity Matrix

| ID | Feature Name (UI Log) | FloreantPOS Behavior | Backend Parity | UI Readiness | Risk Level |
| :--- | :--- | :--- | :--- | :--- | :--- |
| **F-0021** | Ticket View Panel | Read-only list of items/prices. Hierarchy aware. | **PARTIAL** (Model exists, View logic incomplete) | **PARTIAL** (Basic List, poor hierarchy) | **HIGH** |
| **F-0022** | Order View Container | *Duplicate of F-0005*. Main shell for ordering. | **See F-0005** | **NOT IMPLEMENTED** | **HIGH** |
| **F-0023** | Guest Count Entry | Dialog to set number of guests on ticket. | **MISSING** (No backend file found for Guest Count specifically) | **NOT IMPLEMENTED** | **MEDIUM** |
| **F-0024** | Quantity Entry | Pre-entry dialog for quantity > 1. | **FULL** (Command logic exists) | **PARTIAL** (TextBox only, no Dialog) | **LOW** |
| **F-0025** | Print Ticket Action | Prints Customer Receipt. Updates 'Printed' status. | **MISSING** (PrintingService stubbed) | **NOT IMPLEMENTED** | **HIGH** |
| **F-0026** | Inc/Dec Quantity | +/- Buttons. Validates bounds (1). | **FULL** (Logic validated) | **NOT IMPLEMENTED** | **LOW** |
| **F-0027** | Send to Kitchen | Commits items. Status -> SENT. Prints to Kitchen. | **PARTIAL** (Command exists, KDS logic weak) | **PARTIAL** (Button exists, no visual feedback) | **CRITICAL** |
| **F-0028** | Delete Item | Removes item. Forbidden if Sent (must Void). | **FULL** (Validation logic exists) | **NOT IMPLEMENTED** | **MEDIUM** |
| **F-0029** | Misc Item Dialog | Ad-hoc item (Name, Price). | **MISSING** (No MiscItem entity support) | **NOT IMPLEMENTED** | **MEDIUM** |
| **F-0030** | Ticket Fee Dialog | Adds Surcharge/Fee to ticket. | **FULL** (TicketAdjustment entity exists) | **NOT IMPLEMENTED** | **MEDIUM** |
| **F-0031** | Menu Item Button | Grid button. Reflects Stock/Color. | **FULL** (DTOs support this) | **NOT IMPLEMENTED** | **HIGH** |
| **F-0032** | Size Selection | Dialog for Small/Med/Large. Updates Price. | **PARTIAL** (Model needs variant support) | **NOT IMPLEMENTED** | **HIGH** |
| **F-0033** | Beverage Quick Add | Shortcut panel for high-velocity items. | **FULL** (Supported by Category logic) | **NOT IMPLEMENTED** | **LOW** |
| **F-0034** | Item Search | Search by Name/Barcode. | **FULL** (Query exists) | **NOT IMPLEMENTED** | **MEDIUM** |
| **F-0035** | Price Entry | Set price for Open-Price items. | **FULL** (Supported in model) | **NOT IMPLEMENTED** | **MEDIUM** |
| **F-0036** | Cooking Instructions | Free-text notes or predefined. | **FULL** (TicketItem notes supported) | **NOT IMPLEMENTED** | **MEDIUM** |
| **F-0037** | Pizza Modifiers | 1st Half / 2nd Half / Whole selection logic. | **MISSING** (No "Portion" concept in model) | **NOT IMPLEMENTED** | **HIGH** |
| **F-0038** | Modifier Selection | Group constraints (Min/Max). | **FULL** (Logic exists) | **PARTIAL** (Dialog exists, not wired) | **HIGH** |
| **F-0039** | Add-On Selection | Upsell prompts. | **MISSING** (No explicit Upsell logic) | **NOT IMPLEMENTED** | **LOW** |
| **F-0040** | Combo Selection | Wizard for complex meals. | **MISSING** (No Combo entity logic) | **NOT IMPLEMENTED** | **MEDIUM** |
| **F-0041** | Quick Pay Action | One-tap cash settle. | **FULL** (Command composition) | **NOT IMPLEMENTED** | **LOW** |
| **F-0042** | Exact Due Button | Fills tender with exact due. | **FULL** (UI logic) | **PARTIAL** (Auto-fill exists, button missing) | **LOW** |
| **F-0043** | Quick Cash Buttons | $5, $10, $20 presets. | **FULL** (Configurable) | **NOT IMPLEMENTED** | **LOW** |
| **F-0044** | Cash Payment Button | Initiates Cash Settle. | **FULL** | **IMPLEMENTED** | **NONE** |
| **F-0045** | Credit Card Button | Initiates Card Settle. | **FULL** | **PARTIAL** (Button only) | **MEDIUM** |
| **F-0046** | Group Settle | Pay multiple tickets at once. | **MISSING** (No Batch Payment Service) | **NOT IMPLEMENTED** | **LOW** |
| **F-0047** | Split by Seat | Split ticket based on Seat assignment. | **FULL** (Split logic exists, Seat awareness needed) | **NOT IMPLEMENTED** | **HIGH** |
| **F-0048** | Split Even | Split total / N. | **FULL** (Math logic only) | **NOT IMPLEMENTED** | **MEDIUM** |
| **F-0049** | Split by Amount | Move specific $ to new ticket. | **PARTIAL** (Complex accounting needed) | **NOT IMPLEMENTED** | **MEDIUM** |
| **F-0050** | Swipe Card (Action) | *Duplicate of F-0016*. | **MISSING** | **NOT IMPLEMENTED** | **CRITICAL** |

---

## 2. Gap & Drift Report

### Documentation Drift
*   **ID Mismatch**:
    *   F-0022 is "Order View" in UI Audit, but "Modifier Group Selection" in Backend Audit.
    *   F-0023 is "Guest Count" in UI Audit, but "Cooking Instructions" in Backend Audit.
    *   F-0047 to F-0050 completely diverge (Split vs Tax/Coupon).
*   **Impact**: It is difficult to verify if "Guest Count" (F-0023) or "Pizza Modifiers" (F-0037) have backend support because the backend audit files are misaligned.
*   **Resolution**: **Must re-index backend audit files** or rely on code search to confirm backend capabilities for the "Missing" items.

### Functional Gaps
1.  **Pizza/Portion Logic (F-0037)**: The domain model lacks the granularity for "Left Half / Right Half" modifier placement. This is a structural gap in `TicketItemModifier`.
2.  **Misc Items (F-0029)**: No specific entity or flag found for "Ad-hoc" items. Using standard items with variable price is a workaround but loses reporting granularity ("Misc Category").
3.  **Group Settle (F-0046)**: The `ProcessPaymentCommand` operates on a single `TicketId`. No command exists to handle `List<TicketId>`.
4.  **Guest Count (F-0023)**: `Ticket` entity usually has `NumberOfGuests`, but no explicit command/validation was found to enforce it.

---

## 3. Critical Blockers List

| Feature | Reason | Remediation Plan |
| :--- | :--- | :--- |
| **F-0027 Send to Kitchen** | Core POS function. Without KDS/Print integration, the POS is just a calculator. | **P0**: Implement `KitchenService` and `PrinterService`. |
| **F-0038 Modifiers** | Cannot sell complex items (Steak, Pizza). | **P0**: Wire up `ModifierSelectionDialog` to `TicketViewModel` flow. |
| **F-0025 Printing** | Legal requirement (Receipts). | **P1**: Implement Receipt Template engine (ESC/POS or HTML). |

---

## 4. Recommendations

1.  **Prioritize F-0027 (Kitchen)**: This is the "Point of No Return" in the workflow. It needs the most robust testing (what if printer fails? what if network down?).
2.  **Standardize Modifiers**: F-0032 (Size), F-0037 (Pizza), F-0038 (Standard), F-0039 (Add-on) are all variations of the same "Child Item" problem. Solve this with a flexible **Modifier Architecture** rather than 4 separate features.
    *   *Suggestion*: Refactor `TicketItemModifier` to include `ModifierType` (Standard, Size, Upsell) and `Portion` (None, Left, Right).
3.  **Fix ID Drift**: Rename backend audit files to match the UI Log IDs to prevent future confusion.

---

## 5. Execution Timeline Update

*   **Phase 4.1**: (Unchanged)
*   **Phase 4.2**: Add F-0027 (Send to Kitchen) and F-0038 (Modifiers) to the critical path.
*   **Phase 4.3**: Add F-0046 (Group Settle) and F-0037 (Pizza Logic) as secondary targets.

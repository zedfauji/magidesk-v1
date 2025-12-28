# MAGIDESK UI PARITY AUDIT: SUMMARY REPORT

**Date:** Dec 27, 2025
**Auditor:** Cascade (Principal UI Auditor)
**Reference System:** FloreantPOS v1.4 (Build 706)
**Target System:** Magidesk (Phase 4 In-Progress)

---

## 1. Executive Summary

The Magidesk UI is currently in a **Developer Prototype** state. While many individual screens (`TicketPage`, `SettlePage`) have been built to test backend commands, the system lacks the **Cohesive User Experience** of a POS.

The critical "Loop" of a POS system (Login -> Order -> Pay -> Logout) is broken or missing. The application currently boots directly into a developer navigation grid (`SwitchboardPage`) with no security, no hardware integration, and no configuration capabilities.

**Status Scorecard:**
*   **Visual Fidelity**: 30% (Modern WinUI vs Swing, but layout is rudimentary).
*   **Workflow Parity**: 15% (Most actions are isolated commands, not flows).
*   **Operational Readiness**: 0% (Cannot be used in a store).

---

## 2. Gap & Drift Report

### Major UI Drift
1.  **Page vs. Dialog**: FloreantPOS heavily relies on **Modal Dialogs** for transactional sub-tasks (Settle, Modifiers, Split Ticket) to preserve the context of the Ticket behind the dialog. Magidesk has implemented these as **Full Pages**.
    *   *Impact*: Users lose visual context of what they are paying for. Navigation becomes a "Back Button" struggle instead of "Close Dialog".
2.  **Stateless Buttons**: In Floreant, the "Kitchen" button visually changes (Color/Icon) based on the ticket status. In Magidesk, buttons are stateless triggers.
    *   *Impact*: Users don't know if they've pressed the button already.

### Critical Missing Surfaces
1.  **Login Gatekeeper (F-0003)**: The most fundamental screen. Currently, the app hard-bypasses this.
2.  **Back Office (F-0111)**: There is no UI to configure Menus, Printers, or Users. The system is "ReadOnly".
3.  **Kitchen Display (F-0088)**: The "Kitchen" implementation is purely a backend print command. There is no digital KDS screen.

---

## 3. Critical UI Blockers (P0)

These features prevent the basic "Happy Path" transaction.

| ID | Feature | Issue | Impact |
| :--- | :--- | :--- | :--- |
| **F-0003** | **Login** | Missing entirely. | No User Context, Security, or Audit. |
| **F-0005** | **Order Entry** | Split pane (Ticket/Menu) is a stub. | Cannot add items efficiently. |
| **F-0022** | **Modifiers** | No auto-popup on item selection. | Cannot sell complex items (Steak, Pizza). |
| **F-0060** | **Cash Mgmt** | No Shift Start/End UI. | Cannot accept cash responsibly. |
| **F-0069** | **Navigation** | Ticket List -> Order View is broken. | Cannot resume saved tickets. |
| **F-0082** | **Table Map** | Static Canvas only. | Cannot manage Dine-In tables. |

---

## 4. UI Execution Timeline Proposal

We recommend a **"Dialog-First" Refactoring Phase** before building new screens.

### Phase 4.1: The Core Loop (2 Weeks)
*   **Goal**: Establish the secure transaction cycle.
*   **Tasks**:
    1.  Build `LoginView` (Modal or Fullscreen).
    2.  Refactor `SettlePage` -> `SettleDialog` (ContentDialog).
    3.  Refactor `ModifierPage` -> `ModifierDialog` (ContentDialog).
    4.  Implement `OrderEntryView` with proper SplitPane (Left: Ticket, Right: MenuGrid).

### Phase 4.2: Hardware & Cash (2 Weeks)
*   **Goal**: Enable real-world inputs.
*   **Tasks**:
    1.  Build `ShiftStartDialog` & `ShiftEndDialog`.
    2.  Implement KeyListener for Card Swipe (F-0016).
    3.  Implement KeyListener for Barcode Scan (F-0034).

### Phase 4.3: Management & Admin (3 Weeks)
*   **Goal**: Configuration without SQL.
*   **Tasks**:
    1.  Build `BackOfficeShell` (Tabbed View).
    2.  Build `MenuEditorView` (Tree + Form).
    3.  Build `PrinterSetupView`.

---

## 5. Strategic Recommendations

1.  **Stop Building "Pages"**: POS interactions are highly modal. Use `ContentDialog` for anything that takes < 1 minute (Auth, Modifiers, Payment, Void Reason). Only use "Pages" for long-running modes (Order Entry, Back Office, Table Map).
2.  **Unify the Order Screen**: F-0005 (Container), F-0006 (Ticket List), and F-0031 (Menu Grid) MUST be a single, high-performance View. Do not separate them into tabs or pages.
3.  **Visual Feedback**: Implement "Toast" notifications or "Message Banner" (F-0129) immediately. The user is currently blind to backend success/failure.

---

**End of UI Audit Report.**

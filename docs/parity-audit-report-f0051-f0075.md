# Parity Audit & Gap Analysis Report (F-0051 - F-0075)

**Date:** Dec 27, 2025
**Auditor:** Cascade (Principal Architect)
**Reference System:** FloreantPOS v1.4 (Build 706)
**Target System:** Magidesk (Phase 4 In-Progress)

---

## 1. Feature Parity Matrix

| ID | Feature Name | FloreantPOS Behavior | Backend Parity | UI Readiness | Risk Level |
| :--- | :--- | :--- | :--- | :--- | :--- |
| **F-0051** | Refund Button | Initiates Refund Workflow for Paid Tickets. | **FULL** (RefundCommand exists) | **PARTIAL** (Dev button only) | **HIGH** |
| **F-0052** | Check Payment Action | Accept Paper Check. Record Number. | **MISSING** (No Check Payment Type logic) | **NOT IMPLEMENTED** | **LOW** |
| **F-0053** | House Account Action | Charge to Customer Account. | **MISSING** (No Account/Credit logic) | **NOT IMPLEMENTED** | **MEDIUM** |
| **F-0054** | Gift Cert Entry | Accept Gift Card/Cert. Validate. | **PARTIAL** (Button exists, Validation logic weak) | **PARTIAL** | **MEDIUM** |
| **F-0055** | Gratuity Input | Add Tip to Ticket (Pre/Post Settle). | **FULL** (Supported in SettleViewModel) | **PARTIAL** (Field exists, Dialog missing) | **MEDIUM** |
| **F-0056** | Tip Adjustment | Adjust tip after card auth. | **MISSING** (No Tip Adjust workflow) | **NOT IMPLEMENTED** | **HIGH** |
| **F-0057** | Batch Capture | *Duplicate of F-0018*. | **MISSING** | **NOT IMPLEMENTED** | **HIGH** |
| **F-0058** | Multi-Currency | Accept foreign currency (e.g. CAD/USD). | **MISSING** (No Currency Conversion service) | **NOT IMPLEMENTED** | **LOW** |
| **F-0059** | Card Signature | Capture digital signature. | **MISSING** (No Blob storage for sigs) | **NOT IMPLEMENTED** | **LOW** |
| **F-0060** | Shift Start Dialog | Count Opening Cash. Assign Drawer. | **PARTIAL** (CashSession Open exists, Count missing) | **PARTIAL** (Dev Page only) | **HIGH** |
| **F-0061** | Shift End Dialog | Count Closing Cash. Reconcile. | **PARTIAL** (CashSession Close exists, Recon missing) | **PARTIAL** (Dev Page only) | **HIGH** |
| **F-0062** | Payout Dialog | Pay vendor/expense from drawer. | **FULL** (TerminalTransaction model supports this) | **NOT IMPLEMENTED** | **MEDIUM** |
| **F-0063** | No Sale Action | Open drawer without sale. Log event. | **FULL** (Hardware Interface exists) | **NOT IMPLEMENTED** | **HIGH** |
| **F-0064** | Cash Drop Action | Remove excess cash to safe. | **FULL** (TerminalTransaction model supports this) | **NOT IMPLEMENTED** | **HIGH** |
| **F-0065** | Drawer Assignment | Bind Drawer to User/Terminal. | **MISSING** (Explicit Assignment entity needed) | **NOT IMPLEMENTED** | **HIGH** |
| **F-0066** | Tip Declare Action | Server declares cash tips for tax. | **MISSING** (No Tip Declaration entity) | **NOT IMPLEMENTED** | **MEDIUM** |
| **F-0067** | Drawer Count Dialog | Denomination counting (Pennies, Nickels...). | **MISSING** (Only Total Amount supported) | **NOT IMPLEMENTED** | **MEDIUM** |
| **F-0068** | Ticket Explorer | Browse All Tickets (Open/Closed/Void). | **PARTIAL** (Open Only) | **PARTIAL** | **MEDIUM** |
| **F-0069** | Edit Ticket Action | Resume ticket in Order View. | **FULL** (Nav logic exists) | **PARTIAL** (Workflow broken) | **HIGH** |
| **F-0070** | View Receipt Action | Show/Reprint Receipt. | **MISSING** (Printing Service stubbed) | **NOT IMPLEMENTED** | **MEDIUM** |
| **F-0071** | Hold Ticket Action | Save & Exit. | **FULL** (Implicit Save) | **NOT IMPLEMENTED** (Explicit button) | **LOW** |
| **F-0072** | Reopen Ticket Action | Reverse Settle -> Open. | **MISSING** (Complex state transition missing) | **NOT IMPLEMENTED** | **HIGH** |
| **F-0073** | Refund Action | *Duplicate of F-0051*. | **FULL** | **PARTIAL** | **HIGH** |
| **F-0074** | User Transfer | Transfer Ticket to new Owner. | **MISSING** (No Transfer Command) | **NOT IMPLEMENTED** | **MEDIUM** |
| **F-0075** | Merge Tickets | Combine 2 tickets. | **MISSING** (No Merge Command) | **NOT IMPLEMENTED** | **LOW** |

---

## 2. Gap & Drift Report

### Backend Gaps
1.  **Financial Operations**:
    *   **Tip Adjust (F-0056)**: Critical for US markets with Table Service. The backend assumes "Pay & Close" is final. Needs a "Pre-Auth" -> "Capture" or "Adjust" state.
    *   **House Accounts (F-0053)**: No infrastructure for Customer Credit/Accounts Receivable.
2.  **Drawer Precision**:
    *   **Denomination Counting (F-0067)**: Backend only accepts a `decimal` Total. Real POS needs `5x$20, 10x$1`.
    *   **Assignment (F-0065)**: The link between a Physical Drawer and a User Session is weak.

### UI Alignment Issues
1.  **Dev vs Prod**: Almost all Drawer functions (Start Shift, End Shift, Drop, Payout) exist only as buttons on a "DrawerPullReportPage" or "CashSessionPage" designed for debugging. They need a dedicated **Cash Management Module**.
2.  **Ticket Explorer**: The current `TicketManagementPage` is an "Open Tickets" list. It lacks the ability to query historical/closed tickets (F-0068) which is required for Refunds/Reprints.

### Feature Drift
*   **F-0061 (Shift End)**: UI Audit calls it "Shift End", Backend Audit had "Customer Selection" at this ID range in previous batches, but UI Audit definition prevails.
*   **F-0052/F-0053**: Check/House Account payments are standard Floreant features but appear completely missing from Magidesk's domain model.

---

## 3. Critical Blockers List

| Feature | Reason | Remediation Plan |
| :--- | :--- | :--- |
| **F-0060 Shift Start** | Cannot operate a cash register without an Opening Count. | **P0**: Build `ShiftStartDialog` and wire to `OpenCashSessionCommand`. |
| **F-0061 Shift End** | Cannot reconcile cash. Theft risk. | **P0**: Build `ShiftEndDialog` with expected vs actual variance. |
| **F-0069 Edit Ticket** | Core workflow. "Resume" must load the Order View. | **P0**: Fix Navigation from Ticket List -> Order View. |

---

## 4. Execution Timeline Proposal

### Phase 4.1 (Immediate)
*   **Fix Navigation**: Ensure `TicketManagementPage` can successfully navigate to `OrderEntryPage` with a loaded Ticket ID (F-0069).

### Phase 4.2 (Cash Mgmt)
*   **Drawer UI**: Build the `ContentDialogs` for Shift Start/End.
*   **Denominations**: Update `CashSession` entity to support `List<CashCount>` for detailed reconciliation.

### Phase 4.3 (Financials)
*   **Refinement**: Implement `TipAdjustment` workflow (requires changing `TicketStatus` flow).
*   **Printing**: Implement Receipt History viewer (F-0070).

# Frontend Tickets: Category C - Billing, Payments & Pricing

| Ticket ID | Feature ID | Title | Priority | Status |
|-----------|------------|-------|----------|--------|
| FE-C.1-01 | C.1 | Automate Ticket Creation on Session Start | P0 | IN_PROGRESS |
| FE-C.2-01 | C.2 | Hold Ticket UI | P0 | READY_FOR_IMPLEMENTATION |
| FE-C.2-02 | C.2 | Display Time Charges and Duration | P0 | COMPLETED |
| FE-C.15-01 | C.15 | Implement Ticket Void UI | P1 | COMPLETED |
| FE-C.15-02 | C.15 | Implement Refund UI | P1 | COMPLETED |
| FE-C.15-03 | C.15 | Implement Ticket Reprint UI | P2 | COMPLETED |

---

## FE-C.1-01: Automate Ticket Creation on Session Start

**Ticket ID:** FE-C.1-01  
**Feature ID:** C.1  
**Title:** Automate Ticket Creation on Session Start  
**Priority:** P0

### Scope
- Update `StartSessionDialogViewModel` to handle ticket creation context.
- Ensure `StartTableSessionCommand` triggers ticket creation if needed.
- Update `TableMapViewModel` to support seamless transition.

---

## FE-C.2-01: Hold Ticket UI

**Ticket ID:** FE-C.2-01  
**Feature ID:** C.2  
**Title:** Hold Ticket UI  
**Priority:** P0  
**Status:** READY_FOR_IMPLEMENTATION  
**Dependencies:** BE-C.2-01

### Scope
- Create `HoldTicketDialog` to capture hold reason
- Create `HeldTicketsPage` to display all held tickets
- Add "Hold Ticket" button to `SettlePage`
- Implement release ticket functionality
- Update navigation for held tickets view

### Detailed Implementation
See [FE-C.2-01-DETAILED.md](./FE-C.2-01-DETAILED.md) for comprehensive implementation guide including:
- Complete ViewModels (HoldTicketDialogViewModel, HeldTicketsViewModel)
- XAML views and layouts
- Integration with SettlePage
- Navigation updates
- Testing checklist

### Acceptance Criteria
- [ ] "Hold Ticket" button available on SettlePage
- [ ] Hold Ticket dialog captures reason
- [ ] Held tickets page displays all held tickets
- [ ] Can release held ticket from list
- [ ] Table status updates when ticket held
- [ ] Visual feedback for success/error

---

## FE-C.2-02: Display Time Charges and Duration

**Ticket ID:** FE-C.2-02  
**Feature ID:** C.2  
**Title:** Display Time Charges and Duration  
**Priority:** P0

### Scope
- Update `OrderLineDto` to include duration and rate.
- Update `OrderEntryPage` to display these details clearly.

---

## FE-C.14-01: Refund Wizard Shell

**Ticket ID:** FE-C.14-01
**Feature ID:** C.14
**Title:** Refund Wizard Shell
**Priority:** P2

### Scope
- Create `RefundWizardDialog`
- Implement 4-step navigation structure (Mode -> Scope -> Preview -> Commit)
- Integrate into `TicketManagementPage` (Replacing simple `RefundTicketDialog`)
- State management for wizard context

### Acceptance Criteria
- [ ] Dialog opens on Refund click
- [ ] Step navigation works (Next/Back)
- [ ] Cancel closes dialog

---

## FE-C.14-02: Refund Scope Selection UI

**Ticket ID:** FE-C.14-02
**Feature ID:** C.14
**Title:** Refund Scope Selection UI
**Priority:** P2

### Scope
- **Step 1:** Mode selection (Full / Partial / Specific)
- **Step 2 (Partial):** Numeric input for amount, validation <= Paid
- **Step 2 (Specific):** DataGrid of payments with Checkboxes and Amount editing

### Acceptance Criteria
- [ ] Can select Refund Mode
- [ ] Partial amount validated against Total Paid
- [ ] Specific payments can be selected/deselected

---

## FE-C.14-03: Preview Screen UI

**Ticket ID:** FE-C.14-03
**Feature ID:** C.14
**Title:** Preview Screen UI
**Priority:** P2

### Scope
- **Step 3:** Preview
- Display Before/After table (Total, Paid, Due, Status)
- List operations to be performed (e.g., "Create Debit Payment $50")
- Visual warning for "Destructive Action"

### Acceptance Criteria
- [ ] Shows current vs projected values
- [ ] Clearly lists actions to take
- [ ] Read-only view

---

## FE-C.14-04: Authorization & Commit Step

**Ticket ID:** FE-C.14-04
**Feature ID:** C.14
**Title:** Authorization & Commit Step
**Priority:** P2

### Scope
- **Step 4:** Auth
- Embed `ManagerPinDialog` or PIN input logic
- Capture Refund Reason
- Execute Refund Command
- Handle Success/Error results

### Acceptance Criteria
- [ ] Manager PIN required
- [ ] Reason required
- [ ] "Confirm" button disabled until valid
- [ ] Success closes wizard and refreshes parent

---

## FE-C.15-01: Implement Ticket Void UI

**Ticket ID:** FE-C.15-01  
**Feature ID:** C.15  
**Title:** Implement Ticket Void UI  
**Priority:** P1

### Scope
- Add "Void Ticket" button to `TicketManagementPage` or `SettlePage`.
- Create `VoidTicketDialog` to capture Reason.
- Integrate `ManagerPinDialog` for authorization.
- Handle success/failure responses.

### Acceptance Criteria
- [ ] Void button available for open/unpaid tickets
- [ ] Pin prompt appears
- [ ] Void Reason required
- [ ] UI refreshes on success (ticket status updated)

---

## FE-C.15-02: Implement Refund UI

**Ticket ID:** FE-C.15-02  
**Feature ID:** C.15  
**Title:** Implement Refund UI  
**Priority:** P1

### Scope
- Add "Refund" button to `TicketManagementPage` for PAID tickets.
- Create `RefundTicketDialog`.
- Support Full Refund selection.
- Capture Refund Method and Reason.
- Require Manager Auth.

### Acceptance Criteria
- [ ] Refund available for paid tickets
- [ ] Dialog allows selecting amount/method
- [ ] Pin prompt appears
- [ ] UI reflects refunded status

---

## FE-C.15-03: Implement Ticket Reprint UI

**Ticket ID:** FE-C.15-03  
**Feature ID:** C.15  
**Title:** Implement Ticket Reprint UI  
**Priority:** P2

### Scope
- Add "Reprint Receipt" button to `SettlePage` and `TicketManagementPage`.
- Invoke `IReceiptPrintService.PrintTicket` or similar.
- Show "Printing..." feedback.

### Acceptance Criteria
- [ ] Reprint button accessible
- [ ] Triggers backend print job
- [ ] Visual feedback provided


---

## FE-C.3-01: Multiple Payment Method Selection

**Ticket ID:** FE-C.3-01  
**Feature ID:** C.3  
**Type:** Frontend  
**Title:** Multiple Payment Method Selection  
**Priority:** P0

### Scope
- Allow selecting multiple payment methods for a single ticket
- UI to add/remove payment lines
- Validate total matches ticket amount

### Acceptance Criteria
- [ ] User can mix Cash, Card, Gift Card
- [ ] Payments sum correctly
- [ ] Change calculated only on Cash portion

---

## FE-C.4-01: Split Payment UI

**Ticket ID:** FE-C.4-01  
**Feature ID:** C.4  
**Type:** Frontend  
**Title:** Split Payment UI  
**Priority:** P0

### Scope
- Extend SettlePage to support splitting by amount or item
- "Split by N" quick action
- Drag-and-drop items to separate sub-tickets (if full bill split)

### Acceptance Criteria
- [ ] Split by Amount working
- [ ] Split by Item working
- [ ] Each split can be paid separately

---

## FE-C.8-01: Bill-Level Discount UI

**Ticket ID:** FE-C.8-01  
**Feature ID:** C.8  
**Type:** Frontend  
**Title:** Bill-Level Discount UI  
**Priority:** P1

### Scope
- UI to apply percentage or fixed amount discount to entire bill
- Permission check for high discounts
- Reason capture

### Acceptance Criteria
- [ ] Apply 10%, 20%, Custom
- [ ] Recalculates tax/total
- [ ] Discount reason persisted

---

## FE-C.9-01: Happy Hour Indicator

**Ticket ID:** FE-C.9-01  
**Feature ID:** C.9  
**Type:** Frontend  
**Title:** Happy Hour Indicator  
**Priority:** P1

### Scope
- Visual indicator when Happy Hour pricing is active
- Show original vs discounted price on order lines
- Banner on main screen during active times

### Acceptance Criteria
- [ ] Clear "Happy Hour" badge
- [ ] Price strikethrough logic (Regular -> Promo)

---

## FE-C.10-01: Promotion Schedule UI

**Ticket ID:** FE-C.10-01  
**Feature ID:** C.10  
**Type:** Frontend  
**Title:** Promotion Schedule UI  
**Priority:** P2

### Scope
- Admin page to schedule price changes
- Set recurring rules (e.g., Every Friday 5-7 PM)
- Select applicable items/categories

### Acceptance Criteria
- [ ] Calendar view of promotions
- [ ] Conflict detection
- [ ] CRUD operations for promotions

---

## FE-C.11-01: Manual Promotion Override

**Ticket ID:** FE-C.11-01  
**Feature ID:** C.11  
**Type:** Frontend  
**Title:** Manual Promotion Override  
**Priority:** P2

### Scope
- Toggle to disable automatic promotion for a specific ticket
- Manager authorization required
- Reason capture

### Acceptance Criteria
- [ ] "Remove Promo" button
- [ ] Reverts to standard pricing
- [ ] Audited action

---

## FE-C.12-01: Price Override UI

**Ticket ID:** FE-C.12-01  
**Feature ID:** C.12  
**Type:** Frontend  
**Title:** Price Override UI  
**Priority:** P1

### Scope
- Tap price on Order Line to edit
- Numpad for new price
- Hard manager PIN requirement

### Acceptance Criteria
- [ ] Opens edit price dialog
- [ ] Enforces manager auth
- [ ] Visual indicator of "Manual Price" on line item

---

## FE-C.13-01: Price Override Audit Log

**Ticket ID:** FE-C.13-01  
**Feature ID:** C.13  
**Type:** Frontend  
**Title:** Price Override Audit Log  
**Priority:** P2

### Scope
- Admin report showing all manual price changes
- Filter by user, date, amount variance

### Acceptance Criteria
- [ ] List view of overrides
- [ ] Highlights large variances
- [ ] Searchable

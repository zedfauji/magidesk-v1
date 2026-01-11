# Frontend Tickets: Category C - Billing, Payments & Pricing

| Ticket ID | Feature ID | Title | Priority | Status |
|-----------|------------|-------|----------|--------|
| FE-C.1-01 | C.1 | Automate Ticket Creation on Session Start | P0 | IN_PROGRESS |
| FE-C.2-01 | C.2 | Display Time Charges and Duration | P0 | COMPLETED |
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

## FE-C.2-01: Display Time Charges and Duration

**Ticket ID:** FE-C.2-01  
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


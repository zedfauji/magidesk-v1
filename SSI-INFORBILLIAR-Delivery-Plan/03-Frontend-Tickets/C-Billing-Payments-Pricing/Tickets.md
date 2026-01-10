# Frontend Tickets: Category C - Billing, Payments & Pricing

| Ticket ID | Feature ID | Title | Priority | Status |
|-----------|------------|-------|----------|--------|
| FE-C.1-01 | C.1 | Automate Ticket Creation on Session Start | P0 | IN_PROGRESS |
| FE-C.2-01 | C.2 | Display Time Charges and Duration | P0 | COMPLETED |

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

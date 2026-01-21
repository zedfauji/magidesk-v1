# Category C: Billing, Payments & Pricing - Implementation Plan

## Executive Summary

**Goal**: Complete the missing features in Category C (Billing, Payments & Pricing) building on top of the existing working payment flow.

**Current Status**: 
- ✅ Basic payment flow working (C.1, C.3, C.6, C.8, C.14, C.16)
- ⚠️ Partial implementations (C.4 Split Payments, C.7 Discounts, C.12 Price Override, C.15 Void/Refund)
- ❌ Missing features (C.2 Hold Ticket, C.5 Group Billing, C.9 Happy Hour, C.10-C.11 Promotions, C.13 Audit Trail)

**Priority Focus**: P0 and P1 features that enhance the existing payment experience

---

## Phase 1: Critical Payment Features (P0)

### 1.1 Hold Ticket / Charge Later (C.2)
**Status**: 📋 READY FOR IMPLEMENTATION  
**Priority**: P0  
**Dependencies**: None  
**Detailed Ticket**: [BE-C.2-01-DETAILED.md](SSI-INFORBILLIAR-Delivery-Plan/02-Backend-Tickets/C-Billing-Payments-Pricing/BE-C.2-01-DETAILED.md), [FE-C.2-01-DETAILED.md](SSI-INFORBILLIAR-Delivery-Plan/03-Frontend-Tickets/C-Billing-Payments-Pricing/FE-C.2-01-DETAILED.md)

**Backend Tasks**:
- [ ] Add `Held` status to `TicketStatus` enum
- [ ] Create `HoldTicketCommand` with reason tracking
- [ ] Create `ReleaseHeldTicketCommand`
- [ ] Implement `HoldTicketCommandHandler`
- [ ] Add `GetHeldTicketsQuery`
- [ ] Update table status logic (release table when ticket held)
- [ ] Create database migration for new fields

**Frontend Tasks**:
- [ ] Add "Hold Ticket" button to SettlePage
- [ ] Create `HoldTicketDialog` (capture reason)
- [ ] Create "Held Tickets" view/page
- [ ] Add "Resume Ticket" action from held tickets list
- [ ] Update ticket status indicators
- [ ] Add navigation route for Held Tickets page

**Testing**:
- [ ] Unit tests for hold/release commands
- [ ] Integration test: Hold ticket → Table released
- [ ] Integration test: Release ticket → Resume payment
- [ ] UI/UX testing for dialog and page flows

---

## Phase 2: Enhanced Payment Features (P1)

### 2.1 Complete Split Payment Processing (C.4)
**Status**: ⚠️ Partial  
**Priority**: P1  
**Dependencies**: None

**Backend Tasks**:
- [ ] Enhance `ProcessPaymentCommand` to support multiple payments
- [ ] Create `ProcessSplitPaymentCommand`
- [ ] Implement split payment validation (sum = total)
- [ ] Handle partial payment tracking
- [ ] Support overpayment/change calculation

**Frontend Tasks**:
- [ ] Enhance SettlePage split payment UI
- [ ] Add "Split by Amount" quick actions (2-way, 3-way, 4-way, custom)
- [ ] Add "Split by Item" functionality
- [ ] Visual payment allocation display
- [ ] Change calculation for cash portions

**Testing**:
- [ ] Property test: Sum of splits = ticket total
- [ ] Unit test: Overpayment handling
- [ ] Integration test: Mixed payment methods

---

### 2.2 Complete Discount Application (C.7)
**Status**: ⚠️ Partial  
**Priority**: P1  
**Dependencies**: None

**Backend Tasks**:
- [ ] Extend `Discount` entity with `DiscountType` enum
- [ ] Implement member discount auto-application
- [ ] Create `ApplyDiscountCommand` with authorization
- [ ] Implement discount stacking rules
- [ ] Add discount audit trail

**Frontend Tasks**:
- [ ] Add discount selection UI to SettlePage
- [ ] Create discount quick buttons (10%, 20%, 50%, Custom)
- [ ] Implement member discount indicator
- [ ] Add manager override for high discounts
- [ ] Show discount breakdown in ticket summary

**Testing**:
- [ ] Property test: Discount never exceeds total
- [ ] Unit test: Member discount auto-application
- [ ] Integration test: Manager override required for >50%

---

### 2.3 Price Override with Permission (C.12)
**Status**: ⚠️ Partial  
**Priority**: P1  
**Dependencies**: None

**Backend Tasks**:
- [ ] Create `OverrideLinePriceCommand`
- [ ] Implement manager permission check
- [ ] Add price override audit event
- [ ] Update order line price calculation
- [ ] Track original vs override price

**Frontend Tasks**:
- [ ] Add "Edit Price" action to order line items
- [ ] Create price override dialog with numpad
- [ ] Integrate manager PIN authorization
- [ ] Visual indicator for overridden prices
- [ ] Show original price (strikethrough)

**Testing**:
- [ ] Unit test: Manager permission required
- [ ] Integration test: Audit trail created
- [ ] Property test: Override price >= 0

---

### 2.4 Complete Void/Refund Processing (C.15)
**Status**: ⚠️ Partial  
**Priority**: P1  
**Dependencies**: None

**Backend Tasks**:
- [ ] Enhance `VoidTicketCommand` with authorization
- [ ] Complete `RefundTicketCommand` implementation
- [ ] Support full and partial refunds
- [ ] Implement refund method tracking
- [ ] Generate refund receipts
- [ ] Update payment records

**Frontend Tasks**:
- [ ] Complete void ticket UI with reason capture
- [ ] Implement advanced refund wizard (4 steps)
- [ ] Add refund preview calculation
- [ ] Support specific payment refunds
- [ ] Add reprint receipt functionality

**Testing**:
- [ ] Unit test: Void requires manager auth
- [ ] Unit test: Paid tickets cannot be voided
- [ ] Property test: Refund amount <= paid amount
- [ ] Integration test: Refund updates payment records

---

## Phase 3: Promotional Pricing (P1)

### 3.1 Happy Hour / Promotional Pricing (C.9)
**Status**: ❌ Not Started  
**Priority**: P1  
**Dependencies**: C.7 (Discount system)

**Backend Tasks**:
- [ ] Create `PromotionSchedule` entity
- [ ] Implement time-based promotion rules
- [ ] Create `ApplyScheduledPromotionsService`
- [ ] Add promotion validation logic
- [ ] Integrate with discount application

**Frontend Tasks**:
- [ ] Add "Happy Hour" indicator banner
- [ ] Show promotional pricing on order lines
- [ ] Display original vs promo price
- [ ] Add promotion schedule management page

**Testing**:
- [ ] Unit test: Promotion applies during window
- [ ] Unit test: Promotion does not apply outside window
- [ ] Integration test: Automatic promotion application

---

### 3.2 Automatic Promotion Scheduling (C.10)
**Status**: ❌ Not Started  
**Priority**: P2  
**Dependencies**: C.9

**Backend Tasks**:
- [ ] Extend `PromotionSchedule` with recurrence rules
- [ ] Implement conflict detection
- [ ] Create promotion CRUD commands
- [ ] Add promotion activation/deactivation

**Frontend Tasks**:
- [ ] Create promotion schedule management UI
- [ ] Add calendar view for promotions
- [ ] Implement conflict detection UI
- [ ] Add promotion preview

**Testing**:
- [ ] Unit test: Recurring promotion rules
- [ ] Integration test: Conflict detection

---

### 3.3 Manual Promotion Override (C.11)
**Status**: ❌ Not Started  
**Priority**: P2  
**Dependencies**: C.9

**Backend Tasks**:
- [ ] Create `DisablePromotionCommand`
- [ ] Implement manager authorization
- [ ] Track override reason
- [ ] Revert to standard pricing

**Frontend Tasks**:
- [ ] Add "Remove Promotion" button
- [ ] Capture override reason
- [ ] Manager PIN authorization
- [ ] Visual indicator for manual override

**Testing**:
- [ ] Unit test: Manager auth required
- [ ] Integration test: Audit trail created

---

## Phase 4: Advanced Features (P1-P2)

### 4.1 Group Billing (C.5)
**Status**: ❌ Not Started  
**Priority**: P1  
**Dependencies**: C.4 (Split payments)

**Backend Tasks**:
- [ ] Create `GroupSettlement` entity
- [ ] Implement group billing strategies (equal split, by item, custom)
- [ ] Create `CreateGroupSettlementCommand`
- [ ] Link multiple tickets to master payment
- [ ] Handle group payment distribution

**Frontend Tasks**:
- [ ] Create group billing UI
- [ ] Add table selection for group
- [ ] Implement split strategy selection
- [ ] Show group payment summary
- [ ] Handle individual vs group payment

**Testing**:
- [ ] Property test: Group total = sum of tickets
- [ ] Integration test: Group payment distribution

---

### 4.2 Price Override Audit Trail (C.13)
**Status**: ❌ Not Started  
**Priority**: P2  
**Dependencies**: C.12

**Backend Tasks**:
- [ ] Create `GetPriceOverridesQuery`
- [ ] Implement filtering (user, date, variance)
- [ ] Calculate variance metrics
- [ ] Export audit report

**Frontend Tasks**:
- [ ] Create price override audit page
- [ ] Add filtering controls
- [ ] Highlight large variances
- [ ] Implement search functionality
- [ ] Add export to Excel

**Testing**:
- [ ] Unit test: Variance calculation
- [ ] Integration test: Audit report generation

---

## Implementation Order

### Sprint 1: Critical Payment Features (Week 1-2)
1. Hold Ticket / Charge Later (C.2) - 3 days
2. Complete Split Payment (C.4) - 4 days
3. Testing & Integration - 3 days

### Sprint 2: Enhanced Payment Features (Week 3-4)
1. Complete Discount Application (C.7) - 4 days
2. Price Override (C.12) - 3 days
3. Complete Void/Refund (C.15) - 3 days

### Sprint 3: Promotional Pricing (Week 5-6)
1. Happy Hour Pricing (C.9) - 4 days
2. Promotion Scheduling (C.10) - 3 days
3. Manual Override (C.11) - 2 days
4. Testing & Integration - 1 day

### Sprint 4: Advanced Features (Week 7-8)
1. Group Billing (C.5) - 5 days
2. Price Override Audit (C.13) - 3 days
3. Final Testing & Polish - 2 days

---

## Success Criteria

### Phase 1 Complete
- [ ] Tickets can be held and resumed
- [ ] Split payments work correctly
- [ ] All P0 features implemented

### Phase 2 Complete
- [ ] Discounts apply correctly with authorization
- [ ] Price overrides tracked and audited
- [ ] Void/refund flow complete

### Phase 3 Complete
- [ ] Happy hour pricing automatic
- [ ] Promotion scheduling working
- [ ] Manual overrides tracked

### Phase 4 Complete
- [ ] Group billing functional
- [ ] Audit trails comprehensive
- [ ] All P1-P2 features complete

---

## Risk Mitigation

### Technical Risks
1. **Payment calculation complexity**: Mitigate with comprehensive property-based tests
2. **Authorization flow**: Reuse existing manager PIN system
3. **Audit trail completeness**: Use existing audit event infrastructure

### Business Risks
1. **Feature scope creep**: Stick to defined tickets, defer P3 features
2. **Testing coverage**: Implement property tests for all payment calculations
3. **User experience**: Iterate on UI with user feedback

---

## Next Steps

1. **Review and Approve Plan**: Get stakeholder sign-off
2. **Create Missing Tickets**: Generate detailed tickets for each feature
3. **Set Up Development Environment**: Ensure all dependencies ready
4. **Begin Sprint 1**: Start with Hold Ticket feature

---

*Last Updated: January 14, 2026*

# FloreantPOS Feature Implementation Roadmap

> **Based on Phase 2-3 Forensic Audit**  
> **Last Updated**: 2025-12-25

---

## Roadmap Overview

```
┌─────────────────────────────────────────────────────────────────────────┐
│ PHASE 0: FOUNDATION (Complete)                                          │
│ ✅ Core POS shell, login, order entry, basic payments, table sessions  │
├─────────────────────────────────────────────────────────────────────────┤
│ PHASE 1: PRODUCTION-READY (Target: MVP Launch)                          │
│ 🔲 Cash management, refunds, split tickets, reports, shift management  │
├─────────────────────────────────────────────────────────────────────────┤
│ PHASE 2: ENHANCED OPERATIONS                                            │
│ 🔲 Customer management, advanced splits, cooking instructions, etc.    │
├─────────────────────────────────────────────────────────────────────────┤
│ PHASE 3: SPECIALTY FEATURES                                             │
│ 🔲 KDS, delivery, multi-currency, gift certificates, etc.              │
└─────────────────────────────────────────────────────────────────────────┘
```

---

## Phase 1: Production-Ready (56 features) 🎯

### Sprint 1.1: Cash Management Critical Path
**Duration**: 1-2 weeks | **Features**: 6

| ID | Feature | Effort | Dependencies |
|----|---------|--------|--------------|
| F-0012 | Drawer Pull Report | M | Shift entity |
| F-0062 | Payout Dialog | M | None |
| F-0063 | No Sale Action | S | Drawer assigned |
| F-0064 | Cash Drop Action | M | Drawer assigned |
| F-0067 | Drawer Count Dialog | M | Denomination config |
| F-0104 | Cash Out Report | L | F-0012 complete |

**Acceptance Criteria**:
- [ ] Can start shift with opening count
- [ ] Can perform cash drops throughout day
- [ ] Can end shift with drawer pull and variance report

---

### Sprint 1.2: Payment Completeness
**Duration**: 2 weeks | **Features**: 8

| ID | Feature | Effort | Dependencies |
|----|---------|--------|--------------|
| F-0014 | Split Ticket Dialog | L | None |
| F-0048 | Split Even | M | F-0014 |
| F-0049 | Split by Amount | M | F-0014 |
| F-0073 | Refund Action | L | Void ticket exists |
| F-0055 | Gratuity Input | M | Payment complete |
| F-0056 | Tip Adjustment | M | F-0055 |
| F-0043 | Quick Cash Buttons | S | None |
| F-0042 | Exact Due Button | S | None |

**Acceptance Criteria**:
- [ ] Can split ticket evenly and by amount
- [ ] Can process full and partial refunds with reason
- [ ] Can add/adjust tips on card payments

---

### Sprint 1.3: Ticket Operations
**Duration**: 1-2 weeks | **Features**: 6

| ID | Feature | Effort | Dependencies |
|----|---------|--------|--------------|
| F-0074 | User Transfer | M | Ticket entity |
| F-0072 | Reopen Ticket | M | Payment reversal |
| F-0070 | View Receipt | S | Print template |
| F-0029 | Misc Item Dialog | M | Order entry |
| F-0030 | Ticket Fee Dialog | M | Ticket calc |
| F-0034 | Item Search | M | Menu indexed |

**Acceptance Criteria**:
- [ ] Can transfer ticket between servers
- [ ] Can reopen paid ticket (with reversal)
- [ ] Can add misc items and fees

---

### Sprint 1.4: Reports Foundation
**Duration**: 2 weeks | **Features**: 10

| ID | Feature | Effort | Dependencies |
|----|---------|--------|--------------|
| F-0092 | Sales Summary | M | Ticket data |
| F-0093 | Sales Detail | M | F-0092 |
| F-0097 | Payment Report | M | Payment data |
| F-0101 | Tip Report | M | Gratuity data |
| F-0102 | Attendance Report | M | Clock in/out |
| F-0103 | Journal Report | M | Audit events |
| F-0096 | Credit Card Report | M | Card payments |
| F-0094 | Sales Balance | M | Sales data |
| F-0060 | Shift Start Dialog | M | Existing shift |
| F-0061 | Shift End Dialog | M | F-0060 |

**Acceptance Criteria**:
- [ ] Can generate daily sales summary
- [ ] Can view payment breakdown by type
- [ ] Can see tip totals by server
- [ ] Can track attendance and labor

---

### Sprint 1.5: Customer & Menu
**Duration**: 2 weeks | **Features**: 8

| ID | Feature | Effort | Dependencies |
|----|---------|--------|--------------|
| F-0077 | Customer Selector | M | Customer entity |
| F-0078 | Customer Form | M | Customer entity |
| F-0079 | Customer Explorer | M | F-0078 |
| F-0115 | Modifier Explorer | M | Modifier entity |
| F-0116 | Modifier Group Explorer | M | ModifierGroup |
| F-0117 | Coupon Explorer | M | Coupon entity |
| F-0122 | Coupon/Discount Dialog | L | F-0117 |
| F-0123 | Discount Application | M | Discount limits |

**Acceptance Criteria**:
- [ ] Can attach customer to order
- [ ] Can manage modifiers and groups
- [ ] Can apply coupons and discounts

---

## Phase 2: Enhanced Operations (30 features)

### Sprint 2.1: Advanced Splits & Payments
| ID | Feature | Effort |
|----|---------|--------|
| F-0047 | Split by Seat | L |
| F-0076 | Multi-Ticket Payment | M |
| F-0046 | Group Settle | M |
| F-0052 | Check Payment | S |

### Sprint 2.2: Enhanced Order Entry
| ID | Feature | Effort |
|----|---------|--------|
| F-0036 | Cooking Instructions | M |
| F-0033 | Beverage Quick Add | S |
| F-0039 | Add-On Selection | M |
| F-0035 | Price Entry Dialog | S |
| F-0032 | Size Selection | S |

### Sprint 2.3: Tables & Seating
| ID | Feature | Effort |
|----|---------|--------|
| F-0081 | Floor Explorer | M |
| F-0086 | Seat Selection | M |
| F-0085 | Bar Tab Selection | M |
| F-0124 | Table Section Config | M |

### Sprint 2.4: Advanced Reports
| ID | Feature | Effort |
|----|---------|--------|
| F-0095 | Sales Exception | M |
| F-0098 | Menu Usage | M |
| F-0099 | Server Productivity | M |
| F-0100 | Hourly Labor | M |

### Sprint 2.5: Configuration & UX
| ID | Feature | Effort |
|----|---------|--------|
| F-0110 | Language Selection | M |
| F-0128 | Database Backup | M |
| F-0129 | Message Banner | M |
| F-0066 | Tip Declare | S |

---

## Phase 3: Specialty Features (25 features)

### Kitchen Display System
| ID | Feature | Effort |
|----|---------|--------|
| F-0088 | Kitchen Display Window | L |
| F-0089 | Kitchen Ticket View | M |
| F-0090 | Kitchen Status Selector | S |
| F-0091 | Kitchen Ticket List | M |

### Delivery & Pickup
| ID | Feature | Effort |
|----|---------|--------|
| F-0083 | Home Delivery View | L |
| F-0084 | Pickup Order View | M |
| F-0126 | Delivery Zone Config | M |

### Advanced Payment
| ID | Feature | Effort |
|----|---------|--------|
| F-0053 | House Account | L |
| F-0054 | Gift Certificate | L |
| F-0058 | Multi-Currency | L |
| F-0059 | Signature Capture | M |

### Specialty Menu
| ID | Feature | Effort |
|----|---------|--------|
| F-0037 | Pizza Modifiers | L |
| F-0040 | Combo Selection | L |

---

## Effort Estimates

| Size | Time Estimate | Examples |
|------|--------------|----------|
| S (Small) | 0.5-1 day | Button, simple dialog, minor edit |
| M (Medium) | 2-3 days | Dialog with logic, CRUD view, report |
| L (Large) | 4-5 days | Complex workflow, multi-step, integration |
| XL (Extra Large) | 1-2 weeks | New subsystem, KDS, delivery module |

---

## Dependencies Graph (Critical Path)

```
Login ─► Order Entry ─► Payment ─► Settle
                │
                ▼
        ┌── Drawer Pull ◄── Cash Drop
        │       │
        ▼       ▼
    Shift End   Reports
```

**Critical Path for MVP**:
1. Cash management (drawer pull, cash drop, payout)
2. Refund flow
3. Basic split ticket
4. Core reports (sales, payments)
5. Shift management complete

---

## Success Metrics

| Metric | Target | Measurement |
|--------|--------|-------------|
| Phase 1 Feature Coverage | 100% | All P0+P1 features |
| Cash Reconciliation | Working | Can complete shift with variance report |
| Refund Capability | Working | Can process full refund |
| Split Capability | Working | Can split evenly |
| Daily Reports | Available | Sales summary, payments, tips |


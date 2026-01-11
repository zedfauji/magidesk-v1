# Feature to Ticket Matrix

> **Master Index**: Maps all 164 features to their backend and frontend tickets with priority and status.

---

## Legend

| Priority | Meaning |
|----------|---------|
| P0 | Critical blocker - Must have for MVP |
| P1 | High priority - Competitive parity |
| P2 | Medium priority - Differentiation |
| P3 | Low priority - Polish |

| Status | Meaning |
|--------|---------|
| ❌ | Not Started |
| 🔄 | In Progress |
| ✅ | Complete |
| ⚠️ | Partial |

---

## Category A: Table & Game Management (19 Features)

| ID | Feature | Audit | Priority | Backend Ticket | BE Status | Frontend Ticket | FE Status |
|----|---------|-------|----------|----------------|-----------|-----------------|-----------|
| A.1 | Start/timer session | 5/5 (100%) | P0 | BE-A.1-01 ✅, BE-A.1-02 ✅, BE-A.1-03 ✅ | ✅ | FE-A.1-01 ✅, FE-A.1-02 ✅ | ✅ |
| A.2 | End session | 2/2 (100%) | P0 | BE-A.2-01 ✅ | ✅ | FE-A.2-01 ✅ | ✅ |
| A.3 | List active sessions | 2/2 (100%) | P0 | BE-A.3-01 ✅ | ✅ | FE-A.3-01 ✅ | ✅ |
| A.4 | Real-time status | ✅ FULL | P0 | BE-A.4-01 ✅ | ✅ | FE-A.4-01 ✅ | ✅ |
| A.5 | Table types | ✅ FULL | P0 | BE-A.5-01 ✅ | ✅ | FE-A.5-01 | ❌ |
| A.6 | Type per table | ✅ FULL | P0 | BE-A.6-01 ✅ | ✅ | - | - |
| A.7 | Link equipment | ❌ NOT | P2 | BE-A.7-01 | ❌ | - | ❌ |
| A.8 | Game history | ❌ NOT | P2 | BE-A.8-01 | ❌ | - | ❌ |
| A.9 | Time-based pricing | ✅ FULL | P0 | BE-A.9-01 ✅ | ✅ | FE-A.9-01 | ❌ |
| A.10 | First-hour pricing | ❌ NOT | P1 | BE-A.10-01 | ❌ | - | - |
| A.11 | Time rounding | ❌ NOT | P1 | BE-A.11-01 | ❌ | - | - |
| A.12 | Minimum charge | ❌ NOT | P1 | BE-A.12-01 | ❌ | - | - |
| A.13 | Server assignment | ⚠️ PART | P2 | BE-A.13-01 | ❌ | - | ❌ |
| A.14 | Merge tables | ❌ NOT | P2 | BE-A.14-01 | ❌ | - | ❌ |
| A.15 | Split tables | ❌ NOT | P2 | BE-A.15-01 | ❌ | - | ❌ |
| A.16 | Pause/resume | ❌ NOT | P0 | BE-A.16-01 | ❌ | FE-A.16-01 | ❌ |
| A.17 | Manager override | ❌ NOT | P0 | BE-A.17-01 | ❌ | FE-A.17-01 | ❌ |
| A.18 | Transfer session | ❌ NOT | P2 | BE-A.18-01 | ❌ | - | ❌ |
| A.19 | Guest count | ⚠️ PART | P1 | BE-A.19-01 | ❌ | FE-A.19-01 | ❌ |

---

## Category B: Floor & Layout Management (18 Features)

| ID | Feature | Audit | Priority | Backend Ticket | BE Status | Frontend Ticket | FE Status |
|----|---------|-------|----------|----------------|-----------|-----------------|-----------|
| B.1 | Floor / room definitions | ✅ FULL | - | - | ✅ | - | ✅ |
| B.2 | Multiple floors per venue | ✅ FULL | - | - | ✅ | - | ✅ |
| B.3 | Floor dimensions | ✅ FULL | - | - | ✅ | - | ✅ |
| B.4 | Background configuration | ⚠️ PART | P2 | BE-B.4-01 | ❌ | FE-B.4-01 | ❌ |
| B.5 | Table layout designer | ✅ FULL | - | - | ✅ | - | ✅ |
| B.6 | Drag-and-drop placement | ✅ FULL | - | - | ✅ | - | ✅ |
| B.7 | Resize tables | ✅ FULL | - | - | ✅ | - | ✅ |
| B.8 | Table shape configuration | ✅ FULL | - | - | ✅ | - | ✅ |
| B.9 | Snap-to-grid alignment | ❌ NOT | P2 | BE-B.9-01 | ❌ | FE-B.9-01 | ❌ |
| B.10 | Alignment guides | ❌ NOT | P2 | BE-B.10-01 | ❌ | FE-B.10-01 | ❌ |
| B.11 | Zoom and pan | ⚠️ PART | P2 | BE-B.11-01 | ❌ | FE-B.11-01 | ❌ |
| B.12 | Multi-select and group move | ❌ NOT | P2 | BE-B.12-01 | ❌ | FE-B.12-01 | ❌ |
| B.13 | Layout versions per floor | ⚠️ PART | P2 | BE-B.13-01 | ❌ | - | ⚠️ |
| B.14 | Clone layout | ❌ NOT | P2 | BE-B.14-01 | ❌ | - | ❌ |
| B.15 | Draft vs published layout | ✅ FULL | - | - | ✅ | - | ✅ |
| B.16 | Layout rollback / revert | ❌ NOT | P2 | BE-B.16-01 | ❌ | - | ❌ |
| B.17 | Visual occupancy map | ✅ FULL | - | - | ✅ | - | ✅ |
| B.18 | Layout persistence and reload safety | ✅ FULL | - | - | ✅ | - | ✅ |

---

## Category C: Billing, Payments & Pricing (16 Features)

| ID | Feature | Audit | Priority | Backend Ticket | BE Status | Frontend Ticket | FE Status |
|----|---------|-------|----------|----------------|-----------|-----------------|-----------|
| C.1 | Real-time billing per table | ❌ NOT | P0 | BE-C.1-01 | ❌ | FE-C.1-01 | ❌ |
| C.2 | Close now / charge later | ❌ NOT | P0 | BE-C.2-01 | ❌ | - | ❌ |
| C.3 | Multiple payment methods | ✅ FULL | - | - | ✅ | - | ✅ |
| C.4 | Split payments | ✅ FULL | - | - | ✅ | - | ✅ |
| C.5 | Group billing | ⚠️ PART | P1 | BE-C.5-01 | ❌ | FE-C.5-01 | ❌ |
| C.6 | Tips handling | ⚠️ PART | P1 | BE-C.6-01 | ✅ | FE-C.6-01 | ✅ |
| C.7 | Discounts (time-only) | ❌ NOT | P1 | BE-C.7-01 | ❌ | - | ❌ |
| C.8 | Discounts (full bill) | ✅ FULL | - | - | ✅ | - | ✅ |
| C.9 | Happy Hour / promotional pricing | ⚠️ PART | P1 | BE-C.9-01 | ❌ | FE-C.9-01 | ❌ |
| C.10 | Automatic promotion scheduling | ❌ NOT | P2 | BE-C.10-01 | ❌ | - | ❌ |
| C.11 | Manual promotion override | ⚠️ PART | P2 | BE-C.11-01 | ❌ | - | ⚠️ |
| C.12 | Price override with permission | ⚠️ PART | P1 | BE-C.12-01 | ❌ | FE-C.12-01 | ❌ |
| C.13 | Price override audit trail | ❌ NOT | P2 | BE-C.13-01 | ❌ | - | ❌ |
| C.14 | Advanced Refund Management | ❌ NOT | P2 | BE-C.14-01, BE-C.14-02, BE-C.14-03, BE-C.14-04 | ❌ | FE-C.14-01, FE-C.14-02, FE-C.14-03, FE-C.14-04 | ❌ |
| C.15 | Reprint / void ticket | ⚠️ PART | P1 | BE-C.15-01 | ❌ | FE-C.15-01 | ❌ |
| C.16 | Cashbox visibility | ⚠️ PART | P2 | BE-C.16-01 | ❌ | - | ⚠️ |

---

## Category D: Tax, Currency & Financial Rules (9 Features)

| ID | Feature | Audit | Priority | Backend Ticket | BE Status | Frontend Ticket | FE Status |
|----|---------|-------|----------|----------------|-----------|-----------------|-----------|
| D.1 | Tax calculation | ✅ FULL | - | - | ✅ | - | ✅ |
| D.2 | Multi-tax rates | ⚠️ PART | P1 | BE-D.2-01 | ❌ | - | ⚠️ |
| D.3 | Currency format | ✅ FULL | - | - | ✅ | - | ✅ |
| D.4 | Tax exemption | ⚠️ PART | P1 | BE-D.4-01 | ❌ | - | ⚠️ |
| D.5 | Tax breakdown | ⚠️ PART | P2 | BE-D.5-01 | ❌ | - | ⚠️ |
| D.6 | Rounding rules | ✅ FULL | - | - | ✅ | - | ✅ |
| D.7 | Service charge configuration | ❌ NOT | P2 | BE-D.7-01 | ❌ | - | ❌ |
| D.8 | Auto-gratuity rules | ❌ NOT | P2 | BE-D.8-01 | ❌ | - | ❌ |
| D.9 | Multi-currency support | ❌ NOT | P2 | BE-D.9-01 | ❌ | - | ❌ |

---

## Category E: Reservations & Scheduling (12 Features)

| ID | Feature | Audit | Priority | Backend Ticket | BE Status | Frontend Ticket | FE Status |
|----|---------|-------|----------|----------------|-----------|-----------------|-----------|
| E.1 | Create reservations | ❌ NOT | P0 | BE-E.1-01, BE-E.1-02 | ❌ | FE-E.1-01 | ❌ |
| E.2 | Calendar view | ❌ NOT | P0 | BE-E.2-01 | ❌ | FE-E.2-01, FE-E.2-02 | ❌ |
| E.3 | Edit reservations | ❌ NOT | P0 | BE-E.3-01 | ❌ | FE-E.3-01 | ❌ |
| E.4 | Cancel reservations | ❌ NOT | P0 | BE-E.4-01 | ❌ | - | ❌ |
| E.5 | Availability check | ❌ NOT | P0 | BE-E.5-01 | ❌ | FE-E.5-01 | ❌ |
| E.6 | Convert to session | ❌ NOT | P0 | BE-E.6-01 | ❌ | FE-E.6-01 | ❌ |
| E.7 | Conflict detection | ❌ NOT | P1 | BE-E.7-01 | ❌ | - | - |
| E.8 | Customer association | ❌ NOT | P1 | BE-E.8-01 | ❌ | - | - |
| E.9 | Club schedule | ❌ NOT | P1 | BE-E.9-01 | ❌ | FE-E.9-01 | ❌ |
| E.10 | Recurring reservations | ❌ NOT | P2 | BE-E.10-01 | ❌ | - | ❌ |
| E.11 | Reminders | ❌ NOT | P2 | BE-E.11-01 | ❌ | - | ❌ |
| E.12 | Waiting list | ❌ NOT | P2 | BE-E.12-01 | ❌ | - | ❌ |

---

## Category F: Customer & Member Management (13 Features)

| ID | Feature | Audit | Priority | Backend Ticket | BE Status | Frontend Ticket | FE Status |
|----|---------|-------|----------|----------------|-----------|-----------------|-----------|
| F.1 | Customer records | ✅ FULL | P0 | [walkthrough](file:///c:/Users/giris/.gemini/antigravity/brain/06fc728d-db77-4c8a-9dd0-f1e3ce101956/walkthrough.md) | ✅ | FE-F.1-01 ✅, FE-F.1-02 ✅ | ✅ |
| F.2 | Customer search | ✅ FULL | P0 | [walkthrough](file:///c:/Users/giris/.gemini/antigravity/brain/06fc728d-db77-4c8a-9dd0-f1e3ce101956/f2-walkthrough.md) | ✅ | FE-F.2-01 ✅ | ✅ |
| F.3 | Memberships | ❌ NOT | P2 | BE-F.3-01 | ❌ | FE-F.3-01 | ❌ |
| F.4 | Membership tiers | ❌ NOT | P2 | BE-F.4-01 | ❌ | FE-F.4-01 | ❌ |
| F.5 | Member discounts | ❌ NOT | P2 | BE-F.5-01 | ❌ | - | - |
| F.6 | Prepaid accounts | ❌ NOT | P2 | BE-F.6-01 | ❌ | FE-F.6-01 | ❌ |
| F.7 | Customer history | ❌ NOT | P2 | BE-F.7-01 | ❌ | FE-F.7-01 | ❌ |
| F.8 | Renewal | ❌ NOT | P2 | BE-F.8-01 | ❌ | - | ❌ |
| F.9 | Guest passes | ❌ NOT | P2 | BE-F.9-01 | ❌ | - | ❌ |
| F.10 | Member check-in | ❌ NOT | P2 | BE-F.10-01 | ❌ | FE-F.10-01 | ❌ |
| F.11 | Customer notes | ❌ NOT | P2 | BE-F.11-01 | ❌ | - | ❌ |
| F.12 | Customer merge | ❌ NOT | P2 | BE-F.12-01 | ❌ | - | ❌ |
| F.13 | Member analytics | ❌ NOT | P2 | BE-F.13-01 | ❌ | - | ❌ |

---

## Category G: Inventory & Products (12 Features)

| ID | Feature | Audit | Priority | Backend Ticket | BE Status | Frontend Ticket | FE Status |
|----|---------|-------|----------|----------------|-----------|-----------------|-----------|
| G.1 | Menu item CRUD | ✅ FULL | - | - | ✅ | - | ✅ |
| G.2 | Stock level tracking | ✅ FULL | P1 | [walkthrough](file:///c:/Users/giris/.gemini/antigravity/brain/0a093ae8-a242-4323-aadf-1bfeabde64b4/walkthrough.md) | ✅ | FE-G.2-01 | ✅ |
| G.3 | Low stock alerts | ✅ FULL | P1 | BE-G.3-01 | ✅ | FE-G.3-01 | ✅ |
| G.4 | Category hierarchy | ✅ FULL | P2 | [walkthrough](file:///C:/Users/giris/.gemini/antigravity/brain/d81c90a1-d025-4732-8393-ef5f6799e279/walkthrough.md) | ✅ | FE-G.4-01 | ✅ |
| G.5 | Modifier groups | ✅ FULL | P1 | [walkthrough](file:///C:/Users/giris/.gemini/antigravity/brain/d81c90a1-d025-4732-8393-ef5f6799e279/walkthrough.md) | ✅ | FE-G.5-01 | ✅ |
| G.6 | Product images | ✅ FULL | - | - | ✅ | - | ✅ |
| G.7 | SKU/barcode | ❌ NOT | P2 | BE-G.7-01 | ❌ | - | ❌ |
| G.8 | Pricing tiers | ⚠️ PART | P2 | - | ⚠️ | - | ⚠️ |
| G.9 | Product import | ❌ NOT | P2 | BE-G.9-01 | ❌ | - | ❌ |
| G.10 | Product export | ❌ NOT | P2 | BE-G.10-01 | ❌ | - | ❌ |
| G.11 | Recipe / ingredient tracking | ❌ NOT | P2 | BE-G.11-01 | ❌ | - | ❌ |
| G.12 | Waste tracking | ❌ NOT | P2 | BE-G.12-01 | ❌ | - | ❌ |

---

## Category H: Reporting & Export (15 Features)

| ID | Feature | Audit | Priority | Backend Ticket | BE Status | Frontend Ticket | FE Status |
|----|---------|-------|----------|----------------|-----------|-----------------|-----------|
| H.1 | Daily sales report | ⚠️ PART | P1 | BE-H.1-01 | ❌ | FE-H.1-01 | ❌ |
| H.2 | Shift summary | ⚠️ PART | P1 | BE-H.2-01 | ❌ | - | ⚠️ |
| H.3 | Server performance | ❌ NOT | P2 | BE-H.3-01 | ❌ | - | ❌ |
| H.4 | Table utilization | ❌ NOT | P1 | BE-H.4-01 | ❌ | FE-H.4-01 | ❌ |
| H.5 | Time-based revenue | ❌ NOT | P1 | BE-H.5-01 | ❌ | FE-H.5-01 | ❌ |
| H.6 | Member activity | ❌ NOT | P1 | BE-H.6-01 | ❌ | FE-H.6-01 | ❌ |
| H.7 | Inventory report | ⚠️ PART | P2 | BE-H.7-01 | ❌ | - | ⚠️ |
| H.8 | Tax report | ⚠️ PART | P2 | BE-H.8-01 | ❌ | - | ⚠️ |
| H.9 | Custom date range | ✅ FULL | - | - | ✅ | - | ✅ |
| H.10 | PDF export | ⚠️ PART | P2 | BE-H.10-01 | ❌ | - | ⚠️ |
| H.11 | Excel export | ⚠️ PART | P2 | BE-H.11-01 | ❌ | - | ⚠️ |
| H.12 | Payment method breakdown | ❌ NOT | P2 | BE-H.12-01 | ❌ | - | ❌ |
| H.13 | Discount usage report | ❌ NOT | P2 | BE-H.13-01 | ❌ | - | ❌ |
| H.14 | Hourly sales trend | ❌ NOT | P2 | BE-H.14-01 | ❌ | - | ❌ |
| H.15 | Product popularity report | ❌ NOT | P2 | BE-H.15-01 | ❌ | - | ❌ |

---

## Category I: Hardware & Peripherals (11 Features)

| ID | Feature | Audit | Priority | Backend Ticket | BE Status | Frontend Ticket | FE Status |
|----|---------|-------|----------|----------------|-----------|-----------------|-----------|
| I.1 | Receipt printer | ✅ FULL | - | - | ✅ | - | ✅ |
| I.2 | Cash drawer auto-open | ⚠️ PART | P1 | BE-I.2-01 | ❌ | - | ⚠️ |
| I.3 | Kitchen printer | ✅ FULL | - | - | ✅ | - | ✅ |
| I.4 | Lamp control | ❌ NOT | P1 | BE-I.4-01 | ❌ | FE-I.4-01 | ❌ |
| I.5 | Barcode scanner | ⚠️ PART | P2 | BE-I.5-01 | ❌ | - | ⚠️ |
| I.6 | Customer display | ⚠️ PART | P2 | BE-I.6-01 | ❌ | - | ⚠️ |
| I.7 | Scale integration | ✅ FULL | - | - | ✅ | - | ✅ |
| I.8 | Card reader | ⚠️ PART | P2 | BE-I.8-01 | ❌ | - | ⚠️ |
| I.9 | Multi-terminal | ✅ FULL | - | - | ✅ | - | ✅ |
| I.10 | Caller ID integration | ❌ NOT | P2 | BE-I.10-01 | ❌ | - | ❌ |
| I.11 | Kitchen display system | ❌ NOT | P2 | BE-I.11-01 | ❌ | - | ❌ |

---

## Category J: Security, Users & Staff (10 Features)

| ID | Feature | Audit | Priority | Backend Ticket | BE Status | Frontend Ticket | FE Status |
|----|---------|-------|----------|----------------|-----------|-----------------|-----------|
| J.1 | User login/auth | ✅ FULL | P0 | BE-J.1-01 | ✅ | FE-J.1-01, FE-J.1-02 | ✅ |
| J.2 | Role-based permissions | ✅ FULL | - | - | ✅ | - | ✅ |
| J.3 | User management | ✅ FULL | - | - | ✅ | - | ✅ |
| J.4 | PIN security | ✅ FULL | - | - | ✅ | - | ✅ |
| J.5 | Permission groups | ✅ FULL | - | - | ✅ | - | ✅ |
| J.6 | Audit logging | ⚠️ PART | P1 | - | ⚠️ | - | ⚠️ |
| J.7 | Server assignment | ⚠️ PART | P1 | BE-J.7-01 | ❌ | - | ⚠️ |
| J.8 | User activity log | ⚠️ PART | P2 | - | ⚠️ | - | ⚠️ |
| J.9 | Clock in/out | ❌ NOT | P1 | BE-J.9-01 | ❌ | FE-J.9-01 | ❌ |
| J.10 | Break tracking | ⚠️ PART | P2 | BE-J.10-01 | ❌ | - | ⚠️ |

---

## Category K: Localization & Regionalization (6 Features)

| ID | Feature | Audit | Priority | Backend Ticket | BE Status | Frontend Ticket | FE Status |
|----|---------|-------|----------|----------------|-----------|-----------------|-----------|
| K.1 | Multi-language UI | ✅ FULL | - | - | ✅ | - | ✅ |
| K.2 | User-level language | ✅ FULL | - | - | ✅ | - | ✅ |
| K.3 | Currency formatting | ⚠️ PART | P2 | BE-K.3-01 | ❌ | FE-K.3-01 | ❌ |
| K.4 | Date/time formatting | ⚠️ PART | P2 | BE-K.4-01 | ❌ | FE-K.4-01 | ❌ |
| K.5 | Number formatting | ❌ NOT | P2 | BE-K.5-01 | ❌ | - | ❌ |
| K.6 | Translation management | ❌ NOT | P2 | BE-K.6-01 | ❌ | FE-K.6-01 | ❌ |

---

## Category L: Operations, Deployment & Configuration (12 Features)

| ID | Feature | Audit | Priority | Backend Ticket | BE Status | Frontend Ticket | FE Status |
|----|---------|-------|----------|----------------|-----------|-----------------|-----------|
| L.1 | MSIX deployment | ✅ FULL | - | - | ✅ | - | ✅ |
| L.2 | Auto-update | ✅ FULL | - | - | ✅ | - | ✅ |
| L.3 | Database backup | ⚠️ PART | P1 | BE-L.3-01 | ❌ | FE-L.3-01 | ❌ |
| L.4 | Database restore | ⚠️ PART | P1 | BE-L.4-01 | ❌ | - | ⚠️ |
| L.5 | Auto-backup schedule | ❌ NOT | P2 | BE-L.5-01 | ❌ | - | ❌ |
| L.6 | System config UI | ✅ FULL | - | - | ✅ | - | ✅ |
| L.7 | Terminal config | ✅ FULL | - | - | ✅ | - | ✅ |
| L.8 | Offline operation | ✅ FULL | - | - | ✅ | - | ✅ |
| L.9 | Data sync | ⚠️ PART | P2 | - | ⚠️ | - | ⚠️ |
| L.10 | Network resilience | ✅ FULL | - | - | ✅ | - | ✅ |
| L.11 | Config export | ✅ FULL | - | - | ✅ | - | ✅ |
| L.12 | Config import | ❌ NOT | P2 | - | ❌ | - | ❌ |

---

## Category M: System Safety, Diagnostics & Recovery (11 Features)

| ID | Feature | Audit | Priority | Backend Ticket | BE Status | Frontend Ticket | FE Status |
|----|---------|-------|----------|----------------|-----------|-----------------|-----------|
| M.1 | Error logging | ✅ FULL | - | - | ✅ | - | ✅ |
| M.2 | Crash recovery | ✅ FULL | - | - | ✅ | - | ✅ |
| M.3 | Transaction journal | ⚠️ PART | P1 | BE-M.3-01 | ❌ | - | ⚠️ |
| M.4 | Health monitoring | ⚠️ PART | P2 | - | ⚠️ | - | ⚠️ |
| M.5 | Diagnostic tools | ❌ NOT | P2 | - | ❌ | - | ❌ |
| M.6 | Database integrity check | ❌ NOT | P2 | BE-M.6-01 | ❌ | - | ❌ |
| M.7 | Performance monitoring | ❌ NOT | P2 | BE-M.7-01 | ❌ | - | ❌ |
| M.8 | Memory leak detection | ❌ NOT | P2 | BE-M.8-01 | ❌ | - | ❌ |
| M.9 | Automatic error reporting | ❌ NOT | P2 | BE-M.9-01 | ❌ | - | ❌ |
| M.10 | System health dashboard | ❌ NOT | P2 | BE-M.10-01 | ❌ | FE-M.10-01 | ❌ |
| M.11 | Rollback capability | ❌ NOT | P2 | BE-M.11-01 | ❌ | - | ❌ |

---

## Category J: Security & Access Control (Integration Tickets)

| ID | Feature | Audit | Priority | Backend Ticket | BE Status | Frontend Ticket | FE Status |
|----|---------|-------|----------|----------------|-----------|-----------------|-----------|
| J.SEC-01 | Manager PIN Authorization (Refund Wizard) | N/A | P1 | - | - | FE-J-SEC-01 | ❌ |

---

## Summary Statistics

| Priority | Backend | Frontend | Cross-Cutting | Total |
|----------|---------|----------|---------------|-------|
| P0 | 25 | 16 | 8 | 49 |
| P1 | 35 | 15 | 8 | 58 |
| P2 | 60 | 25 | 1 | 86 |
| **Total** | **120** | **56** | **17** | **193** |

> **Note**: Total tickets (193) exceeds total features (164) because some features require multiple tickets (backend + frontend).

---

*Last Updated: 2026-01-10*

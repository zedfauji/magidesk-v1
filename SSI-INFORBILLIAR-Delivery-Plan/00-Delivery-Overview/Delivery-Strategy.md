# Delivery Strategy

## 1. Overview

This document defines the delivery strategy for achieving full feature parity with the SSI-INFORBILLIAR specification based on the completed audit findings.

### 1.1 Audit Reference

| Audit Artifact | Location |
|----------------|----------|
| Executive Summary | `SSI-INFORBILLIAR-Audit-2026-08-01-FULL/00-Executive-Summary/` |
| Feature Parity Reports | `SSI-INFORBILLIAR-Audit-2026-08-01-FULL/01-Feature-Parity/` |
| Backend Analysis | `SSI-INFORBILLIAR-Audit-2026-08-01-FULL/02-Backend-Analysis/` |
| Frontend/UX Analysis | `SSI-INFORBILLIAR-Audit-2026-08-01-FULL/03-Frontend-UX-Analysis/` |
| Competitive Gap Analysis | `SSI-INFORBILLIAR-Audit-2026-08-01-FULL/04-Competitive-Gap-Analysis/` |
| Roadmap Recommendations | `SSI-INFORBILLIAR-Audit-2026-08-01-FULL/05-Roadmap-Recommendations/` |

### 1.2 Guardrail Reference

All development MUST comply with rules defined in `.agent/rules/*`:
- `guardrails.md` - Architecture and layer constraints
- `domain-model.md` - Entity design and invariants
- `testing-requirements.md` - Coverage and quality requirements
- `exception-handling-contract.md` - Error handling patterns
- `no-silent-failure.md` - UI surfacing requirements
- `mvvm-pattern.md` - ViewModel constraints
- `code-quality.md` - Code standards

---

## 2. Feature Summary from Audit

| Status | Count | Percentage |
|--------|-------|------------|
| **Fully Implemented** | 37 | 29.4% |
| **Partially Implemented** | 44 | 34.9% |
| **Not Implemented** | 45 | 35.7% |
| **Total** | 126 | 100% |

### 2.1 Category Breakdown

| Category | Total | Full | Partial | Not | Priority |
|----------|-------|------|---------|-----|----------|
| A. Table & Game Management | 19 | 0 | 9 | 10 | **P0** |
| B. Floor & Layout Management | 18 | 10 | 4 | 4 | P2 |
| C. Billing, Payments & Pricing | 16 | 4 | 7 | 5 | **P0** |
| D. Tax, Currency & Financial Rules | 9 | 1 | 4 | 4 | P1 |
| E. Reservations & Scheduling | 12 | 0 | 0 | 12 | **P0** |
| F. Customer & Member Management | 13 | 0 | 0 | 13 | **P0** |
| G. Inventory & Products | 12 | 4 | 5 | 3 | P2 |
| H. Reporting & Export | 15 | 2 | 4 | 9 | P1 |
| I. Hardware & Peripherals | 11 | 5 | 2 | 4 | P1 |
| J. Security, Users & Staff | 10 | 5 | 3 | 2 | **P0** |
| K. Localization & Regionalization | 6 | 3 | 3 | 0 | P2 |
| L. Operations, Deployment & Config | 12 | 7 | 3 | 2 | P2 |
| M. System Safety & Recovery | 11 | 1 | 5 | 5 | P1 |

---

## 3. Delivery Phases

> **Note**: All feature IDs reference [Feature-to-Ticket-Matrix.md](../01-Feature-Index/Feature-to-Ticket-Matrix.md)

### Phase 1: Parity Blockers (Weeks 1-4)
**Goal:** Enable basic billiard club operation

**Features (P0 Priority):**

#### Category J: Security (Login Gating)
- **J.1** - User login/auth (BE-J.1-01, FE-J.1-01, FE-J.1-02)
  - Backend: Manager authorization service
  - Frontend: Login page + Manager PIN dialog

#### Category A: Table & Game Management (Core Sessions)
- **A.1** - Start/timer session (BE-A.1-01, BE-A.1-02, FE-A.1-01, FE-A.1-02)
- **A.2** - End session (BE-A.2-01, FE-A.2-01)
- **A.3** - List active sessions (BE-A.3-01, FE-A.3-01)
- **A.4** - Real-time status display (BE-A.4-01, FE-A.4-01)
- **A.5** - Table types (BE-A.5-01, FE-A.5-01)
- **A.6** - Type per table (BE-A.6-01)
- **A.9** - Time-based pricing (BE-A.9-01, FE-A.9-01)
- **A.16** - Pause/resume billing (BE-A.16-01, FE-A.16-01)
- **A.17** - Manager time override (BE-A.17-01, FE-A.17-01)

#### Category F: Customer & Member Management (Basic)
- **F.1** - Customer records (BE-F.1-01, FE-F.1-01, FE-F.1-02)
- **F.2** - Customer search (BE-F.2-01, FE-F.2-01)
- **F.3** - Memberships (BE-F.3-01, FE-F.3-01)
- **F.4** - Membership tiers (BE-F.4-01, FE-F.4-01)
- **F.5** - Member discounts (BE-F.5-01)

#### Category C: Billing (Time Charges)
- **C.1** - Create ticket with session link (BE-C.1-01)
- **C.2** - Time charges on ticket (BE-C.2-01, FE-C.2-01)

**Exit Criteria:**
- [ ] User must login before operations (J.1)
- [ ] Can start/stop/pause table sessions (A.1, A.2, A.16)
- [ ] Time-based billing functional (A.9, C.2)
- [ ] Basic customer tracking (F.1, F.2)
- [ ] Member discounts apply (F.5)

**Ticket Count:** 25 Backend + 16 Frontend + 8 Cross-Cutting = **49 tickets**

---

### Phase 2: Competitive Parity (Weeks 5-10)
**Goal:** Match competitor capabilities

**Features (P0 + P1 Priority):**

#### Category E: Reservations & Scheduling (Complete Module)
- **E.1** - Create reservations (BE-E.1-01, BE-E.1-02, FE-E.1-01)
- **E.2** - Calendar view (BE-E.2-01, FE-E.2-01, FE-E.2-02)
- **E.3** - Edit reservations (BE-E.3-01, FE-E.3-01)
- **E.4** - Cancel reservations (BE-E.4-01)
- **E.5** - Availability check (BE-E.5-01, FE-E.5-01)
- **E.6** - Convert to session (BE-E.6-01, FE-E.6-01)
- **E.7** - Conflict detection (BE-E.7-01)
- **E.8** - Customer association (BE-E.8-01)
- **E.9** - Club schedule (BE-E.9-01, FE-E.9-01)

#### Category F: Customer & Member (Advanced Features)
- **F.6** - Prepaid accounts (BE-F.6-01, FE-F.6-01)
- **F.7** - Customer history (BE-F.7-01, FE-F.7-01)
- **F.8** - Membership renewal (BE-F.8-01)
- **F.10** - Member check-in (BE-F.10-01, FE-F.10-01)

#### Category A: Table & Game (Pricing Rules)
- **A.10** - First-hour pricing (BE-A.10-01)
- **A.11** - Time rounding rules (BE-A.11-01)
- **A.12** - Minimum charge rules (BE-A.12-01)
- **A.19** - Guest count tracking (BE-A.19-01, FE-A.19-01)

#### Category C: Billing (Complete Features)
- **C.3** - Add products (BE-C.3-01)
- **C.5** - Split payments (BE-C.5-01, FE-C.5-01)
- **C.6** - Gratuity/tips (BE-C.6-01, FE-C.6-01)
- **C.7** - Discounts (BE-C.7-01, FE-C.7-01)
- **C.9** - Refunds (BE-C.9-01)
- **C.10** - Void tickets (BE-C.10-01)
- **C.14** - Customer on ticket (BE-C.14-01)

#### Category D: Tax & Currency
- **D.2** - Multi-tax rates (BE-D.2-01)
- **D.4** - Tax exemption (BE-D.4-01)

#### Category G: Inventory
- **G.2** - Stock level tracking (BE-G.2-01, FE-G.2-01)
- **G.3** - Low stock alerts (BE-G.3-01, FE-G.3-01)
- **G.5** - Modifier groups (BE-G.5-01, FE-G.5-01)

#### Category H: Reporting (Core Reports)
- **H.1** - Daily sales report (BE-H.1-01, FE-H.1-01)
- **H.2** - Shift summary (BE-H.2-01)
- **H.4** - Table utilization (BE-H.4-01, FE-H.4-01)
- **H.5** - Time-based revenue (BE-H.5-01, FE-H.5-01)
- **H.6** - Member activity (BE-H.6-01, FE-H.6-01)

#### Category I: Hardware
- **I.2** - Cash drawer auto-open (BE-I.2-01)
- **I.4** - Lamp control (BE-I.4-01, FE-I.4-01)

#### Category J: Security & Staff
- **J.7** - Server assignment (BE-J.7-01)
- **J.9** - Clock in/out (BE-J.9-01, FE-J.9-01)

#### Category L: Operations
- **L.3** - Database backup (BE-L.3-01, FE-L.3-01)
- **L.4** - Database restore (BE-L.4-01)

#### Category M: System Safety
- **M.3** - Transaction journal (BE-M.3-01)

**Exit Criteria:**
- [ ] Reservation calendar functional (E.1-E.6)
- [ ] Membership plans operational (F.6-F.10)
- [ ] All pricing rules implemented (A.10-A.12)
- [ ] Split payments working (C.5)
- [ ] Core reports available (H.1, H.4, H.5)
- [ ] Lamp control integrated (I.4)

**Ticket Count:** 30 Backend + 14 Frontend + 8 Cross-Cutting = **52 tickets**

---

### Phase 3: Differentiation (Weeks 11-14)
**Goal:** Surpass competitors with modern features

**Features (P2 Priority):**

#### Category A: Table & Game (Advanced)
- **A.7** - Link game equipment (BE-A.7-01)
- **A.8** - Game history logs (BE-A.8-01)
- **A.13** - Server assignment (BE-A.13-01)
- **A.14** - Merge tables (BE-A.14-01)
- **A.15** - Split tables (BE-A.15-01)
- **A.18** - Transfer session (BE-A.18-01)

#### Category B: Floor & Layout (Gaps)
- **B.6** - Floor switching validation (BE-B.6-01, FE-B.6-01)
- **B.9** - Undo/redo (BE-B.9-01, FE-B.9-01)

#### Category C: Billing (Advanced)
- **C.11** - Hold tickets (BE-C.11-01)
- **C.12** - Transfer tickets (BE-C.12-01)

#### Category D: Tax
- **D.5** - Tax breakdown (BE-D.5-01)

#### Category E: Reservations (Advanced)
- **E.10** - Recurring reservations (BE-E.10-01)
- **E.11** - Reminder system (BE-E.11-01)
- **E.12** - Waiting list (BE-E.12-01)

#### Category F: Customer (Advanced)
- **F.9** - Guest passes (BE-F.9-01)
- **F.11** - Customer notes (BE-F.11-01)
- **F.12** - Customer merge (BE-F.12-01)
- **F.13** - Member analytics (BE-F.13-01)

#### Category G: Inventory (Advanced)
- **G.4** - Category hierarchy (BE-G.4-01)
- **G.7** - SKU/barcode (BE-G.7-01)
- **G.9** - Product import (BE-G.9-01)
- **G.10** - Product export (BE-G.10-01)

#### Category H: Reporting (Advanced)
- **H.3** - Server performance (BE-H.3-01)
- **H.7** - Inventory report (BE-H.7-01)
- **H.8** - Tax report (BE-H.8-01)
- **H.10** - PDF export (BE-H.10-01)
- **H.11** - Excel export (BE-H.11-01)

#### Category I: Hardware (Advanced)
- **I.5** - Barcode scanner (BE-I.5-01)
- **I.6** - Customer display (BE-I.6-01)
- **I.8** - Card reader (BE-I.8-01)

#### Category J: Security (Advanced)
- **J.10** - Break tracking (BE-J.10-01)

#### Category K: Localization
- **K.3** - Currency formatting (BE-K.3-01, FE-K.3-01)
- **K.4** - Date/time formatting (BE-K.4-01, FE-K.4-01)

#### Category L: Operations (Advanced)
- **L.5** - Auto-backup schedule (BE-L.5-01)

**Exit Criteria:**
- [ ] PDF/Excel export functional (H.10, H.11)
- [ ] Advanced reservation features (E.10-E.12)
- [ ] Member analytics available (F.13)
- [ ] All hardware integrations complete (I.5, I.6, I.8)

**Ticket Count:** 28 Backend + 10 Frontend + 1 Cross-Cutting = **39 tickets**

---

### Phase 4: Polish (Weeks 15-17)
**Goal:** Production readiness - Complete all PARTIAL features

**Features (Completing Partial Implementations):**

#### Category B: Floor & Layout
- **B.2** - Complete multi-floor support (BE-B.2-01)
- **B.3** - Complete object properties (BE-B.3-01, FE-B.3-01)

#### Testing & Quality
- Integration test coverage
- UI automation tests
- Performance optimization
- Documentation completion

**Exit Criteria:**
- [ ] All PARTIAL features completed to FULL
- [ ] All tests passing (≥90% Domain, ≥80% App)
- [ ] Documentation complete
- [ ] Zero guardrail violations
- [ ] Zero silent failures
- [ ] Production deployment successful

**Ticket Count:** Remaining tickets + quality/testing work

---

---

## 4. Ticket Classification

### 4.1 Ticket Types

| Type | Description | Ticket ID Prefix |
|------|-------------|------------------|
| **BE** | Backend ticket | `BE-{Category}.{Feature}-{Seq}` |
| **FE** | Frontend ticket | `FE-{Category}.{Feature}-{Seq}` |
| **CC** | Cross-cutting ticket | `CC-{Area}-{Seq}` |

### 4.2 Priority Levels

| Priority | Description | SLA |
|----------|-------------|-----|
| **P0** | Parity Blocker - Cannot operate without | Must complete in Phase 1 |
| **P1** | Competitive Parity - Core functionality | Must complete in Phase 2 |
| **P2** | Differentiation - Advantage features | Must complete in Phase 3 |
| **P3** | Polish - Nice-to-have | Complete in Phase 4 |

### 4.3 Feature Status Mapping

| Audit Status | Ticket Priority | Action |
|--------------|-----------------|--------|
| NOT IMPLEMENTED | P0/P1 | Full implementation |
| PARTIALLY IMPLEMENTED | P1/P2 | Complete missing pieces |
| FULLY IMPLEMENTED | None | No ticket needed |

---

## 5. Ticket Granularity Rules

### 5.1 Backend Ticket Boundaries

One ticket per:
- New domain entity
- New aggregate root
- New command handler
- New query handler
- New service

Split if ticket spans:
- Multiple aggregates
- Multiple bounded contexts
- More than 5 use cases

### 5.2 Frontend Ticket Boundaries

One ticket per:
- New page
- New dialog
- New control (reusable)
- Complex feature area

Split if ticket spans:
- Multiple pages
- Multiple unrelated dialogs
- More than 3 ViewModels

### 5.3 Maximum Ticket Size

- **Backend:** ≤40 story points / ≤3 days effort
- **Frontend:** ≤40 story points / ≤3 days effort
- If larger, MUST split

---

## 6. Dependency Management

### 6.1 Dependency Types

| Type | Description | Handling |
|------|-------------|----------|
| **HARD** | Cannot start without predecessor | Strict sequencing |
| **SOFT** | Preferred but not blocking | Parallel if needed |
| **EXTERNAL** | Third-party dependency | Early validation |

### 6.2 Dependency Chains

```
J.1-J.2-J.3 (Login/RBAC) 
    └── A.1-A.2 (Table Sessions) 
        ├── A.3-A.12 (Time Tracking/Pricing)
        └── C.1-C.8 (Billing)
            └── E.1-E.12 (Reservations)

F.1-F.3 (Customer/Member)
    └── F.4-F.13 (Member Features)
        └── E.8 (Reservation linking)
```

---

## 7. Quality Gates

### 7.1 Per-Ticket Quality Criteria

Every ticket MUST satisfy:

| Criterion | Verification |
|-----------|--------------|
| Guardrail compliance | Code review checklist |
| Test coverage | Domain ≥90%, App ≥80% |
| No silent failures | Exception handling audit |
| MVVM compliance | Architecture review |
| Documentation | Code comments + docs |

### 7.2 Phase Gate Criteria

Before phase exit:

| Criterion | Verification |
|-----------|--------------|
| All phase tickets complete | Status tracking |
| All tests passing | CI/CD pipeline |
| No P0/P1 bugs open | Bug tracker |
| Demo successful | Stakeholder sign-off |

---

## 8. Progress Tracking

### 8.1 Tracking Artifacts

| Artifact | Purpose | Update Frequency |
|----------|---------|------------------|
| `Ticket-Status.md` | Individual ticket status | Per ticket completion |
| `Feature-Completion.md` | Feature-level rollup | Daily |
| Feature-to-Ticket Matrix | Traceability | Per ticket creation |

### 8.2 Status Values

| Status | Description |
|--------|-------------|
| `NOT_STARTED` | Ticket created, not begun |
| `IN_PROGRESS` | Development active |
| `BLOCKED` | Cannot proceed |
| `IN_REVIEW` | Code review pending |
| `TESTING` | QA validation |
| `DONE` | Complete and verified |

### 8.3 Machine-Updateable Format

All tracking files use machine-parseable formats:
- Markdown tables with consistent columns
- Status values from fixed enum
- Date stamps in ISO 8601
- Ticket IDs as stable references

---

## 9. Risk Mitigation

### 9.1 Identified Risks

| Risk | Probability | Impact | Mitigation |
|------|-------------|--------|------------|
| Scope creep | High | High | Strict ticket scope |
| Dependency delays | Medium | High | Early parallel work |
| Quality regression | Medium | High | Automated tests |
| Guardrail violations | Medium | Medium | Code review gates |

### 9.2 Escalation Path

| Severity | Escalation |
|----------|------------|
| Blocked >1 day | Tech Lead notification |
| Blocked >3 days | PM escalation |
| Quality issue | Architecture review |
| Guardrail violation | Immediate fix required |

---

## 10. Success Metrics

### 10.1 Velocity Metrics

| Metric | Target |
|--------|--------|
| Tickets completed/week | ≥10 |
| Backend/Frontend ratio | ~50/50 |
| Bug escape rate | <5% |

### 10.2 Quality Metrics

| Metric | Target |
|--------|--------|
| Test coverage (Domain) | ≥90% |
| Test coverage (Application) | ≥80% |
| Guardrail violations | 0 |
| Silent failure instances | 0 |

### 10.3 Delivery Metrics

| Metric | Target |
|--------|--------|
| Phase 1 on-time | 100% |
| Features complete | 100% |
| Documentation complete | 100% |

---

*Document Version: 1.0*  
*Created: 2026-01-08*  
*Based on: SSI-INFORBILLIAR-Audit-2026-08-01-FULL*

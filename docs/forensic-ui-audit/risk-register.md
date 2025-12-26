# FloreantPOS Feature Implementation Risk Register

> **Based on Phase 2-3 Forensic Audit**  
> **Last Updated**: 2025-12-25

---

## Risk Severity Matrix

| Probability / Impact | LOW | MEDIUM | HIGH |
|---------------------|-----|--------|------|
| **HIGH** | Monitor | Mitigate | Critical |
| **MEDIUM** | Accept | Monitor | Mitigate |
| **LOW** | Accept | Accept | Monitor |

---

## Critical Risks (Immediate Action Required)

### RISK-001: Missing Cash Drawer Reconciliation
- **Impact**: HIGH
- **Probability**: HIGH
- **Feature IDs**: F-0012, F-0062, F-0063, F-0064, F-0067
- **Description**: No drawer pull report, cash drop, or payout functionality. Shifts cannot reconcile.
- **Business Impact**: Cannot end shifts properly; no accountability for cash
- **Mitigation**: Implement drawer pull flow before production deployment
- **Status**: 🔴 NOT STARTED

### RISK-002: Incomplete Refund Flow
- **Impact**: HIGH  
- **Probability**: HIGH
- **Feature IDs**: F-0073
- **Description**: Refund action missing. Cannot process customer refunds.
- **Business Impact**: Customer service failure; chargeback risk
- **Mitigation**: Implement full refund flow with manager approval
- **Status**: 🔴 NOT STARTED

### RISK-003: No Ticket Split Capability
- **Impact**: MEDIUM
- **Probability**: HIGH
- **Feature IDs**: F-0014, F-0047, F-0048, F-0049
- **Description**: Cannot split ticket by item, seat, or evenly.
- **Business Impact**: Very common customer request; operational friction
- **Mitigation**: Implement at least split-by-item for MVP
- **Status**: 🔴 NOT STARTED

---

## High Risks (Action Required Before Production)

### RISK-004: Missing Void Ticket Workflow
- **Impact**: HIGH
- **Probability**: MEDIUM
- **Feature IDs**: F-0013
- **Description**: Void ticket exists but workflow may be incomplete.
- **Business Impact**: Cannot cancel orders; inventory discrepancy
- **Mitigation**: Verify complete void flow with audit trail
- **Status**: 🟡 PARTIAL

### RISK-005: No Transfer Ticket Between Users
- **Impact**: MEDIUM
- **Probability**: HIGH
- **Feature IDs**: F-0074
- **Description**: Cannot transfer ticket ownership between servers.
- **Business Impact**: Shift change problems; tip allocation issues
- **Mitigation**: Implement ticket transfer with reason logging
- **Status**: 🔴 NOT STARTED

### RISK-006: Limited Report Suite
- **Impact**: MEDIUM
- **Probability**: HIGH
- **Feature IDs**: F-0092 to F-0104
- **Description**: No sales summary, payment, or cash out reports.
- **Business Impact**: No visibility into operations; accounting difficulty
- **Mitigation**: Implement core reports (sales, payments, tips)
- **Status**: 🔴 NOT STARTED

---

## Medium Risks (Monitor & Plan)

### RISK-007: No Customer Management
- **Impact**: MEDIUM
- **Probability**: MEDIUM
- **Feature IDs**: F-0077, F-0078, F-0079
- **Description**: Cannot attach customer to order; no customer database.
- **Business Impact**: No loyalty; no delivery address; limited marketing
- **Mitigation**: Implement basic customer CRUD; defer loyalty
- **Status**: 🔴 NOT STARTED

### RISK-008: Modifier Groups Incomplete
- **Impact**: MEDIUM
- **Probability**: MEDIUM
- **Feature IDs**: F-0115, F-0116, F-0038
- **Description**: Modifier selection exists but group management lacking.
- **Business Impact**: Menu flexibility limited; pricing errors possible
- **Mitigation**: Complete modifier explorer; test modifier pricing
- **Status**: 🟡 PARTIAL

### RISK-009: No Misc Item Entry
- **Impact**: MEDIUM
- **Probability**: LOW
- **Feature IDs**: F-0029
- **Description**: Cannot add ad-hoc items not in menu.
- **Business Impact**: Special requests cannot be accommodated
- **Mitigation**: Add misc item dialog with name/price entry
- **Status**: 🔴 NOT STARTED

---

## Low Risks (Acceptable for MVP)

### RISK-010: No Kitchen Display System
- **Impact**: LOW (for MVP)
- **Probability**: LOW
- **Feature IDs**: F-0088 to F-0091
- **Description**: Entire KDS functionality missing.
- **Business Impact**: Can use printed kitchen tickets; KDS is enhancement
- **Mitigation**: Defer to Phase 2; use print-based kitchen flow
- **Status**: 🟢 DEFERRED

### RISK-011: No Delivery/Pickup Workflow
- **Impact**: LOW (context-dependent)
- **Probability**: LOW
- **Feature IDs**: F-0083, F-0084, F-0126
- **Description**: Specialized order types not implemented.
- **Business Impact**: Limited if dine-in focused; critical if delivery business
- **Mitigation**: Assess client needs; defer or prioritize accordingly
- **Status**: 🟢 DEFERRED

### RISK-012: No Multi-Currency Support
- **Impact**: LOW
- **Probability**: LOW
- **Feature IDs**: F-0058
- **Description**: Cannot accept multiple currencies.
- **Business Impact**: Border locations or tourist areas only
- **Mitigation**: Defer unless specific client requirement
- **Status**: 🟢 DEFERRED

### RISK-013: No Gift Certificate System
- **Impact**: LOW
- **Probability**: LOW
- **Feature IDs**: F-0054
- **Description**: Cannot sell or redeem gift certificates.
- **Business Impact**: Revenue opportunity; not critical for MVP
- **Mitigation**: Defer to Phase 2+
- **Status**: 🟢 DEFERRED

---

## Risk Trend Dashboard

| Category | Critical | High | Medium | Low | Total |
|----------|---------|------|--------|-----|-------|
| Payments | 2 | 1 | 1 | 1 | 5 |
| Cash Mgmt | 1 | 1 | 0 | 0 | 2 |
| Orders | 1 | 1 | 1 | 0 | 3 |
| Reports | 0 | 1 | 0 | 0 | 1 |
| Kitchen | 0 | 0 | 0 | 1 | 1 |
| Delivery | 0 | 0 | 0 | 1 | 1 |
| **TOTAL** | **4** | **4** | **2** | **3** | **13** |

---

## Immediate Action Items

1. ⚠️ **Cash Management**: Implement F-0012, F-0062, F-0064 before any production use
2. ⚠️ **Refund Flow**: Implement F-0073 with full audit trail
3. ⚠️ **Split Ticket**: Implement at least basic split (F-0014)
4. 📋 **Transfer Ticket**: Implement F-0074 for shift changes
5. 📋 **Core Reports**: Implement F-0092, F-0097, F-0101

---

## Deferred Items (Phase 2+)

- Kitchen Display System (F-0088-0091)
- Delivery/Pickup workflows (F-0083-0084)
- Advanced split options (F-0047 seat-based)
- Multi-currency (F-0058)
- Gift certificates (F-0054)
- House accounts (F-0053)
- Pizza modifiers (F-0037)
- Combo items (F-0040)


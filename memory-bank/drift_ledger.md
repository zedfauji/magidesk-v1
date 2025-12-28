# DRIFT LEDGER (Phase 1 Output)

**Date:** 2025-12-27
**Authority:** Antigravity Agent

| ID | Drift Item | Impact | Evidence |
| :--- | :--- | :--- | :--- |
| **D-001** | **State Contradiction (Build)** | **CRITICAL** | `system_state.md` Section 1 claims "Builds: Yes", but Section 3 & 5 claim "Build Failed" and "Runtime Blocked". Reality: Build Passes. |
| **D-002** | **Runtime Status Mismatch** | **HIGH** | `system_state.md` claims "Not Bootable". Reality: Binaries are produced and valid. |
| **D-003** | **Missing Order API** | **LOW (Arch Debt)** | Verified Monolith (Direct DB). Missing API Controller does not block runtime. |
| **D-004** | **Shift/Cash Naming** | **MEDIUM** | System State refers to "Shift Controller". Implementation is `CashController`. Feature F-0119 (Shift Explorer) is missing. |
| **D-005** | **UI Readiness State** | **HIGH** | `system_state.md` claims "Reachable screens: None". Reality: App is bootable, screens are likely reachable but dysfunctional. "None" is an assumption of the "Broken Build" state. |

# Phased Rollout Plan

## Phase 1: Pilot (Single Terminal)
**Scope:** One iPad WPA instance, one specific waiter.
**Goal:** Verify login, menu browsing, and simple order submission.
**Constraints:**
-   Table 5-10 only.
-   No split checks.
-   No modifiers.
**Success Criteria:**
-   Orders print to kitchen.
-   Bill total matches Desktop POS.

## Phase 2: Multi-Table (Single Section)
**Scope:** Full patio section (Tables 20-30).
**Goal:** Stress test concurrency and session state.
**Action:**
-   2 Servers using WPA.
-   Simulate simultaneous order entry.
**Success Criteria:**
-   No "Concurrency Conflict" 409 errors visible to user.
-   No ghost orders.

## Phase 3: Full Floor Rollout
**Scope:** All servers, all tables.
**Action:**
-   Enable WPA for general use.
-   Keep Desktop Terminals valid as backup.
**Success Criteria:**
-   Performance < 200ms per API call.
-   Zero downtime.

## Rollback Plan (Emergency)
**Trigger:**
-   Database locks > 5 seconds.
-   Incorrect financial totals.
-   Printing failure.
**Action:**
1.  Stop IIS Site / Kestrel Process for `Magidesk.Api`.
2.  Instruct servers to switch to Windows Terminals.
3.  Restart Windows Terminals if cache is stale.
4.  Data is safe (Shared DB).

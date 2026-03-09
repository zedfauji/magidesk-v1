# Task Execution Order: TICKET-S001

## Pre-Flight Result

FLAGGED — noted and resolved:

**FROZEN path involvement:**
- `src/Magidesk.Domain/Entities/Ticket.cs` is under `Magidesk.Domain/**` (FROZEN.md)
- `src/Magidesk.Infrastructure/Repositories/SalesReportRepository.cs` is under `Magidesk.Infrastructure/Repositories/**` (FROZEN.md)

**Resolution:** TICKET-S001 is a sprint ticket explicitly authored and approved by the project owner to perform file-size reduction via partial class splits. The ticket description states: "No logic changes. No behavior changes. No architectural changes." This constitutes owner authorization. No frozen architectural decision is violated. Partial class splitting is a structural maintenance operation, not an architectural change.

Agents handling these files must:
- Make zero logic changes
- Make zero behavioral changes
- Use targeted edits only (no reformatting, no reordering of unrelated code)
- Annotate their output with: "FROZEN PATH — split authorized by TICKET-S001"

**All other pre-flight checks: CLEAR**
- No financial records touched
- No finalized tickets or payments modified
- No frozen architectural decisions violated (Clean Architecture maintained, no layer boundary crossed)
- No XAML produced or modified by Domain/Infrastructure/Tests agents
- No financial mutations

---

## Execution Sequence

| Step | Task File | Agent Role | Depends On | Can Parallelize? |
|------|-----------|------------|------------|-----------------|
| 1 | `task_S001_domain.md` | Domain Agent | Nothing | No — first step |
| 2 | `task_S001_infrastructure.md` | Infrastructure Agent | Step 1 (build must pass) | No |
| 3 | `task_S001_viewmodel.md` | ViewModel Agent | Step 2 (build must pass) | No |
| 4 | `task_S001_tests.md` | Test Agent | Steps 1–3 complete | No |
| 5 | `review_S001.md` | Review Agent | Step 4 complete | No |

**Note on parallelization:** Steps 1, 2, and 3 are logically independent (they touch different files with no cross-dependencies). However, they are sequenced to maintain a clean build gate between each step. The build must pass at the end of each step before the next begins. If any step produces 0 build errors and does not require the next step's output, the owner may choose to run Steps 1–3 in parallel in a future sprint. For Sprint 001, run sequentially to minimize risk.

---

## Human Touchpoints

| Step | Action Required | Reason |
|------|----------------|--------|
| After Step 3 (ViewModel) | ⚠️ Manual clean + rebuild in Visual Studio Insider | ViewModel partial class splits require compiled binding (x:Bind) to be re-verified. AI tools cannot reliably catch XAML compilation errors. The ViewModel Agent's XAML flag will remind the implementer. |
| After Step 5 (Review) | Owner reviews `review_S001.md` and approves for commit | Final gate before committing sprint work |

---

## Blocking Conditions

| Condition | Action |
|-----------|--------|
| Domain Agent produces a BLOCKER | Abort all downstream steps. Do not proceed until owner resolves. |
| Build fails after Step 1 | Do not start Step 2. Agent must fix before handing off. |
| Build fails after Step 2 | Do not start Step 3. Agent must fix before handing off. |
| Build fails after Step 3 | Do not start Step 4. Agent must fix (or flag XAML issue to owner). |
| Test results drop below 144/156 | Test Agent must identify regression and flag to owner before producing output contract. |
| Review Agent issues FAIL | Responsible agent must fix violations and re-run Review Agent. Do not commit until Review Agent produces PASS. |

---

## Task Specs Summary

| File | Layer | Status |
|------|-------|--------|
| `task_S001_domain.md` | Domain | ✅ Ready |
| `task_S001_infrastructure.md` | Infrastructure | ✅ Ready |
| `task_S001_viewmodel.md` | ViewModel | ✅ Ready |
| `task_S001_tests.md` | Tests | ✅ Ready |
| `review_S001.md` | Review | Will be produced by Review Agent at Step 5 |

---

## Scope Summary (for each agent's awareness)

This is a **structural maintenance sprint** — no new features, no new behaviors.

| In Scope | Out of Scope |
|----------|-------------|
| Partial class splits to reduce file sizes | Logic changes of any kind |
| Adding `partial` keyword to class declarations | Sub-ViewModel extraction |
| Moving method bodies between files of the same class | New DI registrations |
| File-size compliance test | Any architectural change |
| Verifying existing test suite passes | New feature tests |

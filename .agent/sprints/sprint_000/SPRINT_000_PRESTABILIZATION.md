# Sprint 000 — Pre-Pipeline Stabilization

**Created:** 2026-03-03  
**Purpose:** These 4 fixes must be completed before the multi-agent pipeline runs any feature sprint. They eliminate known blockers that would cause agents to fail or produce incorrect output.  
**Run order:** Do these sequentially. Each is a single-agent task.

---

## Ticket S000-01: Fix E2E Database Reset Script

**Layer:** Infrastructure (Tests)  
**Effort:** ~30 min  
**Blocking:** 17 of 28 test failures

### Task
Update the `DatabaseResetEngine` SQL reset script to include the `KitchenOrderItems` table that currently exists in the schema but is missing from the reset script.

Also verify and create the PostgreSQL role `giris` if it does not exist as a superuser or with appropriate permissions on `magidesk_pos`.

### Files to Modify
- `src/Magidesk.Tests.E2E/Infrastructure/DatabaseReset/` — find the reset SQL and add `KitchenOrderItems`

### Acceptance Criteria
- [ ] `dotnet test src/Magidesk.Tests.E2E` no longer fails with `relation "KitchenOrderItems" does not exist`
- [ ] `dotnet test src/Magidesk.Tests.E2E` no longer fails with `role "giris" does not exist`
- [ ] E2E infrastructure failures drop from 17 to 0 (remaining E2E failures should be behavioral only)

### Agent Prompt
```
You are the Infrastructure Agent for Magidesk POS.

Read: .agent/knowledge/08_ai_assistant_rules.md

Task: Fix the E2E database reset script.

1. Find the DatabaseResetEngine reset SQL in src/Magidesk.Tests.E2E/Infrastructure/DatabaseReset/
2. Add the missing KitchenOrderItems table to the reset/recreate script
3. Check if there are any other tables in the production schema (check EF Core configurations in src/Magidesk.Infrastructure/Persistence/Configurations/) that are missing from the reset script
4. Provide the SQL needed to create the giris PostgreSQL role if missing

Do not modify production code. Tests only.
Do not modify any of the 28 existing failing tests.
```

---

## Ticket S000-02: Fix Workflow Test DI Gap

**Layer:** Tests  
**Effort:** ~15 min  
**Blocking:** 3 test failures

### Task
Add `IUserContextService` mock registration to `WorkflowTestBase.cs` DI setup.

### Files to Modify
- `src/Magidesk.Tests.Workflows/WorkflowTestBase.cs`

### Acceptance Criteria
- [ ] `OpenCashSession_ShouldInvokeCommand_WhenInputIsValid` passes
- [ ] `CloseCashSession_ShouldInvokeCommand_WhenActiveSessionExists` passes
- [ ] `OpenCashSession_ShouldFail_WhenInputIsInvalid` passes
- [ ] No other tests broken

### Agent Prompt
```
You are the Test Agent for Magidesk POS.

Read: .agent/knowledge/08_ai_assistant_rules.md

Task: Fix the DI gap in WorkflowTestBase.cs.

File: src/Magidesk.Tests.Workflows/WorkflowTestBase.cs

The 3 failing tests fail with: Unable to resolve service for type 'IUserContextService'

Add a mock or stub implementation of IUserContextService to the test DI container in WorkflowTestBase.
The mock should return a consistent non-empty Guid for GetCurrentUserId().
Do not use Guid.Empty.
Do not modify any of the 3 failing tests themselves — fix only the DI setup.
Do not modify any other test infrastructure.
```

---

## Ticket S000-03: Fix Guid.Empty in Application Handlers

**Layer:** Application  
**Effort:** ~1 hour  
**Blocking:** Audit trail integrity for all future features

### Task
Replace `Guid.Empty` used as `performedBy` in 4 application handlers. Inject `IUserContextService` into each handler and use `GetCurrentUserId()`.

### Files to Modify
- `src/Magidesk.Application/Services/ModifyOrderLineCommandHandler.cs` — line 82
- `src/Magidesk.Application/Services/RemoveOrderLineCommandHandler.cs` — line 79
- `src/Magidesk.Application/Services/ApplyDiscountCommandHandler.cs` — lines 281, 315
- `src/Magidesk.Application/Services/SessionControlService.cs` — lines 67, 119, 182, 293

### Acceptance Criteria
- [ ] Zero `Guid.Empty` used as `performedBy` in the 4 files above
- [ ] Each handler injects `IUserContextService` via constructor
- [ ] `IUserContextService` is already registered — no new DI registration needed
- [ ] Clean build maintained
- [ ] No domain layer changes

### Agent Prompt
```
You are the Application Agent for Magidesk POS.

Read: .agent/knowledge/08_ai_assistant_rules.md

Task: Replace Guid.Empty identity in 4 application handlers.

For each of these files:
- src/Magidesk.Application/Services/ModifyOrderLineCommandHandler.cs (line 82)
- src/Magidesk.Application/Services/RemoveOrderLineCommandHandler.cs (line 79)
- src/Magidesk.Application/Services/ApplyDiscountCommandHandler.cs (lines 281, 315)
- src/Magidesk.Application/Services/SessionControlService.cs (lines 67, 119, 182, 293)

Do the following:
1. Add IUserContextService to the constructor (it is already registered in DI)
2. Replace Guid.Empty with _userContextService.GetCurrentUserId() at each marked line
3. Do not change any other logic in these files
4. Do not modify Domain layer
5. Verify the change compiles (no new interface needed — IUserContextService already exists at Magidesk.Application/Interfaces/IUserContextService.cs)

Output: modified files only. No other changes.
```

---

## Ticket S000-04: Fix KDS AutoRoute Notification Gap

**Layer:** Application  
**Effort:** ~1 hour  
**Blocking:** KDS production blocker (partial)

### Task
In `AddOrderLineCommandHandler.cs`, after `IKitchenRoutingService.AutoRouteOrderLinesAsync()` succeeds, call `IOrderNotificationService.NotifyAsync()` with the station context.

This closes the second remaining KDS notification gap (the first gap — legacy `PrintingService` wrapper — is addressed by the agent rule forbidding new callers).

### Files to Modify
- `src/Magidesk.Application/Services/AddOrderLineCommandHandler.cs` (or wherever AutoRoute is called)

### Acceptance Criteria
- [ ] After successful auto-route, `IOrderNotificationService.NotifyAsync()` is called
- [ ] Notification fires only after persistence succeeds — not before
- [ ] If notification fails, it logs but does NOT roll back the order line addition
- [ ] Clean build maintained
- [ ] No domain layer changes

### Agent Prompt
```
You are the Application Agent for Magidesk POS.

Read: .agent/knowledge/08_ai_assistant_rules.md

Task: Close the KDS auto-route notification gap.

File: src/Magidesk.Application/Services/AddOrderLineCommandHandler.cs
(or find where IKitchenRoutingService.AutoRouteOrderLinesAsync is called when ShouldPrintToKitchen=true)

Current flow:
AddOrderLine → AutoRouteOrderLinesAsync → [MISSING] → KDS display not updated

Required fix:
After AutoRouteOrderLinesAsync succeeds and persistence is committed:
1. Inject IOrderNotificationService (already registered in DI as Scoped)
2. Call NotifyAsync() with appropriate station/order context
3. Wrap the notification call in try/catch — if it fails, log the error but DO NOT throw and DO NOT roll back the order line
4. The order line addition must always succeed even if notification fails

Constraints:
- Do not modify Domain layer
- Do not call the legacy PrintingService wrapper
- Notification must fire AFTER persistence succeeds, not before
- Failure of notification must not affect the order line result returned to UI
```

---

## Sprint 000 Completion Checklist

Before starting Sprint 001:

- [ ] S000-01 complete — E2E DB failures resolved
- [ ] S000-02 complete — Workflow DI failures resolved  
- [ ] S000-03 complete — Guid.Empty audit identity fixed
- [ ] S000-04 complete — KDS AutoRoute notification gap closed
- [ ] `dotnet build` still clean (0 errors)
- [ ] Test failure count reduced from 28 to ≤8 (only real behavioral failures remain)
- [ ] `07_current_state_and_open_work.md` updated with Sprint 000 results
- [ ] Git commit: `fix(sprint-000): pre-pipeline stabilization complete`

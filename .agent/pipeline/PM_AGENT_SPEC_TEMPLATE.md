# PM Agent Spec Template — Clean Architecture .NET Pipeline

**Version:** 2.0  
**Purpose:** Instructions for the PM Agent on how to decompose a sprint ticket into layer-specific task specs. Project-specific context is loaded from `PROJECT_CONTEXT.md` at runtime — this template contains only the universal decomposition process.

---

## Step 1: Run Pre-Flight Checklist

Before writing any task spec, answer every question below. If any answer triggers a STOP, do not proceed — output a BLOCKER.

- [ ] Does this ticket touch a finalized financial record? → If yes, verify the project's immutability rules. Flag to owner before proceeding.
- [ ] Does this ticket require changing a frozen architectural decision? → Read `PROJECT_CONTEXT.md` Section: Frozen Decisions. If yes, STOP and flag.
- [ ] Does this ticket touch files listed in `PROJECT_CONTEXT.md` Section: Problem Files? → If yes, note the constraint in the relevant task spec.
- [ ] Does this ticket produce or modify XAML? → If yes, flag the human touchpoint in the order file.
- [ ] Does this ticket affect financial mutations, payments, or audit trails? → If yes, verify the identity/audit rules in `AI_ASSISTANT_RULES.md`.

---

## Step 2: Identify Affected Layers

For each layer below, mark whether this ticket touches it:

| Layer | Touched? | Reason |
|-------|----------|--------|
| Domain | YES / NO | New or modified entity, value object, domain event, or invariant |
| Application | YES / NO | New or modified command, query, handler, or service interface |
| Infrastructure | YES / NO | New or modified repository, ORM config, migration, or external service |
| ViewModel | YES / NO | New or modified ViewModel |
| View | YES / NO | New or modified XAML |
| Tests | ALWAYS YES | Minimum: one Domain test + one Application test |

---

## Step 3: Produce One Task Spec Per Touched Layer

Use the Task Spec Format below. Save each as: `task_[TICKET_ID]_[layer].md`

### Task Spec Format

```
# Task Spec: [TICKET_ID] — [LAYER] Layer

## Ticket Summary
[One sentence: what the feature or fix achieves]

## This Task's Responsibility
[What specifically this layer agent must implement. Reference method names, class names, and file paths from PROJECT_CONTEXT.md where relevant.]

## Input Contract
[What this agent receives from the previous layer's output contract. List class names, interface names, DTO shapes.]

## Output Contract (Required)
[What this agent must produce for the next agent. List class names, method signatures, property names.]

## Files to Create
[Each new file: full path + one-line description]

## Files to Modify
[Each existing file: full path + specific change needed]
[If a file is listed in PROJECT_CONTEXT.md Problem Files: note the constraint here]

## Constraints
- Follow all rules in AI_ASSISTANT_RULES.md
- Maximum file line limit: [from PROJECT_CONTEXT.md]
- One class per file
- No silent failures
- [Any ticket-specific constraints]

## Acceptance Criteria
[Bullet list — each item must be independently verifiable by the Review Agent]

## Do NOT
[Explicit list of things this agent must not do for this specific task]

## XAML Flag
NO — this task does not produce or modify XAML
OR
YES ⚠️ — this task produces or modifies XAML. Agent must end output with:
"XAML CHANGE — requires manual clean + rebuild before marking complete."
```

---

## Step 4: Produce the Dependency Order File

Save as: `task_[TICKET_ID]_order.md`

```
# Task Execution Order: [TICKET_ID]

## Execution Sequence

| Step | Task File | Agent Role | Depends On | Can Parallelize? |
|------|-----------|------------|------------|-----------------|
| 1 | task_[ID]_domain.md | Domain Agent | Nothing | No |
| 2 | task_[ID]_application.md | Application Agent | Step 1 | No |
| 3 | task_[ID]_infrastructure.md | Infrastructure Agent | Step 2 | No |
| 4 | task_[ID]_viewmodel.md | ViewModel Agent | Step 2 | Yes (with Step 3) |
| 5 | task_[ID]_view.md | View Agent | Step 4 | No |
| 6 | task_[ID]_tests.md | Test Agent | Steps 1–5 | No |
| 7 | review_[ID].md | Review Agent | Step 6 | No |

## Human Touchpoints
[List steps requiring human action — e.g. "Step 5: XAML change — requires VS build verify"]

## Blocking Conditions
[e.g. "If Domain step fails or outputs a BLOCKER, abort all downstream steps."]

## Pre-Flight Result
CLEAR — no frozen decisions affected, no financial immutability risk
OR
FLAGGED — [describe what was flagged and what owner decision is needed]
```

---

## Layer Responsibility Reference

Use this to correctly assign work to the right layer task spec.

### Domain Layer
- Creates or modifies: entities, value objects, domain events, domain services, invariants, repository interfaces
- Does NOT: touch persistence, HTTP, or UI

### Application Layer
- Creates or modifies: commands, queries, handlers, application service interfaces, DTOs
- Always injects the user context service — never uses a null/empty identity
- Does NOT: touch ORM or UI directly

### Infrastructure Layer
- Creates or modifies: repository implementations, ORM configurations, migrations, external service implementations
- Always implements interfaces defined elsewhere — never defines them
- Does NOT: contain business logic

### ViewModel Layer
- Creates or modifies: ViewModels
- Always works from DTOs — never exposes domain entities
- Does NOT: call repositories or write business logic

### View Layer
- Creates or modifies: XAML files
- Always uses compiled binding
- Does NOT: contain logic in code-behind

### Tests Layer
- Creates: unit tests (Domain), handler tests (Application), integration tests (Infrastructure) as needed
- Does NOT: modify existing failing tests unless that is the task's explicit purpose

---

## Worked Example

**Ticket:** "Add manager override to apply a manual price adjustment to an order line"

### Pre-Flight
- Touches financial data (order line price) → verify immutability rules — adjustment creates audit entry, does not modify history ✅
- No frozen decisions affected ✅
- No XAML (manager override uses existing dialog pattern) — check PROJECT_CONTEXT.md ✅

### Layer Identification
- Domain: YES — new `PriceAdjustment` value object, new method on `OrderLine`
- Application: YES — new `ApplyPriceAdjustmentCommand` + Handler
- Infrastructure: YES — ORM config for new value object if needed
- ViewModel: YES — new command in the relevant ViewModel
- View: NO — uses existing manager override dialog
- Tests: YES — always

### Task Specs Produced
- `task_PRICE001_domain.md`
- `task_PRICE001_application.md`
- `task_PRICE001_infrastructure.md`
- `task_PRICE001_viewmodel.md`
- `task_PRICE001_tests.md`
- `task_PRICE001_order.md`

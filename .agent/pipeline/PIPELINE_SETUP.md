# Pipeline Setup Guide — Multi-Agent Development

**Version:** 2.0  
**Purpose:** How to configure and operate the semi-autonomous multi-agent development pipeline. Project-neutral process — project-specific state lives in `PROJECT_CONTEXT.md`.

---

## Part 1: File Structure

```
project-root/
├── .agent/
│   ├── pipeline/
│   │   ├── AGENT_ROLES.md              ← Permanent. Never changes.
│   │   ├── PM_AGENT_SPEC_TEMPLATE.md   ← Permanent. Never changes.
│   │   ├── PIPELINE_SETUP.md           ← This file. Never changes.
│   │   └── AI_ASSISTANT_RULES.md       ← Project rules. Update rarely.
│   └── context/
│       └── PROJECT_CONTEXT.md          ← Updated every sprint.
└── sprints/
    └── sprint_NNN/
        ├── tickets/
        │   └── TICKET-NNN_description.md
        └── tasks/
            ├── task_TICKETNNN_domain.md
            ├── task_TICKETNNN_application.md
            ├── task_TICKETNNN_infrastructure.md
            ├── task_TICKETNNN_viewmodel.md
            ├── task_TICKETNNN_view.md
            ├── task_TICKETNNN_tests.md
            ├── task_TICKETNNN_order.md
            └── review_TICKETNNN.md
```

**Key principle:** `AGENT_ROLES.md` and `PM_AGENT_SPEC_TEMPLATE.md` are permanent and project-neutral. All project-specific state that agents need is in `PROJECT_CONTEXT.md`. Only `PROJECT_CONTEXT.md` needs updating between sprints.

---

## Part 2: Agent Configuration

### Option A — Manual (Antigravity / Claude chat)
Run each agent by pasting the agent prompt template below into a new session. Load the required files as context.

### Option B — Cowork (Parallel / Automated)
Configure 4 persistent agent instances. Each loads its role definition once and receives task specs as work items.

### Cowork Agent Setup

| Agent Instance | Roles It Covers | When It Runs |
|----------------|-----------------|--------------|
| PM Agent | Role 1 (PM) | Start of every ticket |
| Code Agent | Roles 2–4 (Domain, Application, Infrastructure) | Sequential, after PM |
| UI Agent | Roles 5–6 (ViewModel, View) | After Application output contract |
| Quality Agent | Roles 7–8 (Tests, Review) | After all code is complete |

### System Prompt for Each Cowork Agent

Load this as the persistent system prompt for each agent instance, substituting the role name:

```
You are the [AGENT NAME] for this project.

Before every task, read:
1. .agent/pipeline/AGENT_ROLES.md — your role section defines your identity, responsibilities, and what you are forbidden from doing.
2. .agent/context/PROJECT_CONTEXT.md — the current state of the project, frozen decisions, problem files, and active constraints.
3. .agent/pipeline/AI_ASSISTANT_RULES.md — non-negotiable project rules.

When you receive a task spec file, implement it exactly.
End every output with the OUTPUT CONTRACT block defined in AGENT_ROLES.md.
If you encounter a blocker, output the BLOCKER block and stop immediately.
```

---

## Part 3: Running a Sprint

### Step 1: Write the Ticket

Create `sprints/sprint_NNN/tickets/TICKET-NNN_description.md`:

```markdown
# Ticket: TICKET-NNN

## Title
[Feature or fix name]

## Description
[What this achieves from the user's perspective. One paragraph.]

## Acceptance Criteria
- [ ] [Verifiable outcome 1]
- [ ] [Verifiable outcome 2]

## Out of Scope
[Anything explicitly not included in this ticket]

## Notes
[Any known constraints, related files, or context the PM Agent should know]
```

### Step 2: Run PM Agent

**Prompt:**
```
You are the PM Agent.

Read these files before doing anything:
- .agent/pipeline/PM_AGENT_SPEC_TEMPLATE.md
- .agent/pipeline/AGENT_ROLES.md
- .agent/context/PROJECT_CONTEXT.md
- .agent/pipeline/AI_ASSISTANT_RULES.md
- sprints/sprint_NNN/tickets/TICKET-NNN_description.md

Run the pre-flight checklist from PM_AGENT_SPEC_TEMPLATE.md.
Decompose this ticket into layer task specs.
Save all task spec files to: sprints/sprint_NNN/tasks/
Save the dependency order file to: sprints/sprint_NNN/tasks/task_TICKETNNN_order.md
Do not write any implementation code.
```

### Step 3: Human Review of PM Output (Required)

Before running any specialist agent:
- Read `task_TICKETNNN_order.md`
- If PM Agent output a BLOCKER → resolve it before continuing
- If pre-flight was FLAGGED → make the required decision
- If task breakdown looks wrong → correct it now, not after agents have run

This review takes 2–5 minutes and prevents wasted agent runs.

### Step 4: Run Specialist Agents

Run agents in the order defined in `task_TICKETNNN_order.md`. For sequential agents, wait for the output contract before starting the next.

**Prompt template for each specialist agent:**
```
You are the [ROLE] Agent.

Read these files before doing anything:
- .agent/pipeline/AGENT_ROLES.md — your role section
- .agent/context/PROJECT_CONTEXT.md
- .agent/pipeline/AI_ASSISTANT_RULES.md
- sprints/sprint_NNN/tasks/task_TICKETNNN_[layer].md
- [Previous agent's output contract, if applicable]

Implement your task spec exactly.
End your output with the OUTPUT CONTRACT block.
If you encounter a blocker, output the BLOCKER block and stop.
```

### Step 5: XAML Build Verify (Human — When Required)

If any agent flagged `XAML build required: YES`:
1. Open Visual Studio Insider
2. Clean solution
3. Rebuild
4. If errors: paste the error into the View Agent and let it fix
5. Repeat until clean
6. Only then proceed to Step 6

This is the primary human touchpoint in the pipeline. It cannot be automated.

### Step 6: Run Quality Agent

```
You are the Quality Agent covering both the Test Agent and Review Agent roles.

Read these files:
- .agent/pipeline/AGENT_ROLES.md — Test Agent (Role 7) and Review Agent (Role 8) sections
- .agent/context/PROJECT_CONTEXT.md
- .agent/pipeline/AI_ASSISTANT_RULES.md
- All output files from agents for TICKET-NNN
- sprints/sprint_NNN/tasks/task_TICKETNNN_tests.md

First: write the required tests (Test Agent role).
Then: run the full review checklist (Review Agent role).
Output review_TICKETNNN.md to sprints/sprint_NNN/tasks/
```

### Step 7: Commit or Fix

If `review_TICKETNNN.md` shows `Approved for Commit: YES`:
```
git add -A
git commit -m "feat(TICKET-NNN): [title] — pipeline complete, review passed"
```

If `Approved for Commit: NO`:
- Send violations back to the agent responsible for each violation
- Re-run Review Agent after fixes
- Do not commit until review passes

---

## Part 4: Human Touchpoints Summary

These cannot be removed from the loop regardless of automation level:

| Touchpoint | When | Action |
|------------|------|--------|
| PM output review | After PM Agent runs | Read order file, confirm no blockers |
| BLOCKER response | Any agent outputs BLOCKER | Owner makes architectural decision |
| XAML build verify | Any task with XAML changes | Visual Studio Insider: clean + rebuild |
| Review FAIL fix | Review Agent outputs FAIL | Route violations back to responsible agent |

---

## Part 5: Updating PROJECT_CONTEXT.md After Each Sprint

After every sprint completes:

1. Update **Section 9** (Build State) — re-run the diagnostic prompt if significant changes were made
2. Update **Section 10** (Test State) — update failure counts and categories
3. Update **Section 11** (Active Blockers) — close resolved blockers, add newly discovered ones
4. Update **Section 13** (Sprint History) — mark sprint complete with a one-line summary

Commit the updated `PROJECT_CONTEXT.md` with the sprint commit:
```
git add .agent/context/PROJECT_CONTEXT.md
git commit -m "context: update PROJECT_CONTEXT.md after sprint NNN"
```

---

## Part 6: When the Pipeline Breaks

### Agent produces output outside its layer
→ Stop. Discard output. Add a "Do NOT" constraint to the task spec and re-run.

### Agent ignores a constraint from PROJECT_CONTEXT.md
→ Verify PROJECT_CONTEXT.md was included in the agent's context. Re-run with explicit file load.

### XAML errors persist after multiple fix attempts
→ Switch to Visual Studio Insider for manual resolution. Paste the error to the View Agent for a targeted fix.

### Review Agent fails the same violation repeatedly
→ The task spec did not make the constraint clear enough. Update the task spec's "Do NOT" section and re-run from the responsible agent.

### Build breaks after a commit
→ Revert the commit. Run the diagnostic prompt to identify the regression. Assign a targeted fix ticket.

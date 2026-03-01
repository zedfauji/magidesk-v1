You are Ralph Loop running inside Antigravity.

OBJECTIVE:
Implement UserContextService + Manager Override for Magidesk POS.

SOURCE OF TRUTH:
Read and obey PRD.md strictly.

EXECUTION RULES:
- Execute ONE task per iteration
- Tasks are defined in PRD.md under "TASKS"
- Start every iteration with a fresh context
- Do NOT anticipate future tasks
- Do NOT refactor unrelated code
- Do NOT modify Magidesk.Domain
- Fix in place, no redesigns, no parallel implementations

VERIFICATION (after every task):
- dotnet build must succeed

STOP CONDITIONS:
- All TASKS in PRD.md are completed
- Build passes with zero errors

FAILURE CONDITIONS:
- Build failure
- Violation of PRD constraints
- Introduction of new TODOs or Guid.Empty

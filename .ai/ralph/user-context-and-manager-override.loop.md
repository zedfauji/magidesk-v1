RALPH LOOP CONFIGURATION

Objective:
Implement UserContextService + Manager Override strictly according to:
ai/specs/user-context-and-manager-override.gsd.md

Task Source:
ai/tasks/user-context-and-manager-override.tasks.md

Rules:
- Execute ONE task per loop iteration
- Start each iteration with a fresh context
- Do NOT anticipate future tasks
- Do NOT refactor unrelated code
- Do NOT modify Magidesk.Domain

Verification after each task:
- dotnet build must succeed

Stop conditions:
- All tasks completed
- Build passes with zero errors

Failure conditions:
- Build failure
- Violation of GSD constraints
- Introduction of new TODOs

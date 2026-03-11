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
- Run .\build-xaml.ps1 — NOT dotnet build
- Read diagnostics\build-logs\build_summary_LATEST.txt for result
- Task is only complete when build_summary_LATEST.txt shows BUILD RESULT: SUCCESS
- dotnet build is BANNED — it misses WinUI 3 XAML compiler errors (MC*, WMC*, XBF*)
- If the same build error appears 3 times in a row, STOP and report to owner

STOP CONDITIONS:
- All TASKS in PRD.md are completed
- .\build-xaml.ps1 passes with zero errors

FAILURE CONDITIONS:
- Build failure (as reported by .\build-xaml.ps1)
- Violation of PRD constraints
- Introduction of new TODOs or Guid.Empty
- Using dotnet build instead of .\build-xaml.ps1

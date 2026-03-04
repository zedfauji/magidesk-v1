---
name: error-handling-guardian
description: Prevents silent failures and ensures robust exception use.
---
# Error Handling Guardian

Trigger when generating try/catch or logging code.

Rules:
- Avoid catch blocks that swallow exceptions.
- Ensure log includes context and actionable state.
- If meaningful action is needed, bubble up error.

Assist by generating annotated catch + logging patterns.

---
name: coding-standards-enforcer
description: Apply advanced coding standards for large .NET codebases.
---
# Coding Standards Enforcer

**Trigger:** When generating or refactoring code.

**Goal**
Maintain code structure and readability, enforcing:
- Limit methods to single responsibility
- Appropriate naming conventions
- Restrict file size
- Avoid deep nesting
- Use expression-bodied members where appropriate
- Encapsulate mutable state

**Instructions**
1) Analyze existing code pattern.
2) If method > 40 lines: propose extraction.
3) If file > 300 lines: propose splitting.
4) Suggest naming changes based on conventions.

**Constraints**
- Do not suggest changes that alter behavior.

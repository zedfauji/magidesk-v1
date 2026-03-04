---
name: file-decomposition-advisor
description: Assist splitting large files into smaller, focused units.
---
# File Decomposition Advisor

**Trigger:** When a file exceeds standards or the user asks for simplification.

**Goal**
Break up large modules:
- Extract services
- Split ViewModels into partials/modules
- Move helper logic to separate classes

**Instructions**
- Inspect file structure
- Identify clusters of responsibility
- Propose extraction points (methods, classes)

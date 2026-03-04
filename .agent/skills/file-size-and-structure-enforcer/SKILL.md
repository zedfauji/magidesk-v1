---
name: file-size-and-structure-enforcer
description: Enforces maximum file length and decomposition standards.
---
# File Size and Structure Enforcement

Trigger when creating or modifying source files.

Rules:
1) If file length > 300 lines:
   - Propose splitting into focused units (services, helpers).
   - Suggest extracting sub-ViewModels for UI.
2) Generate narrative and draft code for decomposition.

Fail the operation if the file remains > 300 lines after proposal.

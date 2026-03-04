# File Size and Structural Limits

## Hard Limits
- Maximum file length: 300 lines (including whitespace).
- If logic exceeds this limit, it must be split.

## Splitting Rules
- Split by responsibility, not convenience.
- Shared logic must move into services or helpers.
- ViewModels must not grow indefinitely; extract sub-viewmodels if needed.

## Enforcement Principle
Large files increase agent error rate and human review cost.
Agents must proactively decompose code.

Any change that pushes a file beyond limits is invalid.


This directly addresses your AI slob root cause.

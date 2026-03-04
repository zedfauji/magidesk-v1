---
trigger: always_on
---

# Testing Scope and Boundaries

## Rules
- Business logic must be testable without UI.
- ViewModels must be testable in isolation.
- UI tests should validate behavior, not implementation.

## Constraints
- Do not add brittle tests.
- Do not mock .
- Tests must fail meaningfully.

Tests exist to protect behavior, not to satisfy metrics.

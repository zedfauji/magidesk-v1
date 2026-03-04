---
trigger: always_on
---

# Library Usage and Dependencies

## Rules
- Prefer existing project libraries over introducing new ones.
- Avoid overlapping libraries that solve the same problem.

## Constraints
- No experimental or unmaintained libraries.
- No dynamic runtime dependency loading.
- No hidden transitive dependencies without justification.

## .NET Specific
- Use standard .NET libraries where possible.
- Third-party UI helpers must not bypass MVVM boundaries.

Library sprawl is prohibited.

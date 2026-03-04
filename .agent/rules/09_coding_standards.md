# Coding Standards (Authoritative)

## Structure
- One file = one primary responsibility.
- One public class per file.
- Nested classes are prohibited unless explicitly justified.

## Methods
- Methods must not exceed 40 logical lines.
- Methods must do one thing.
- Early returns are preferred over deep nesting.

## Naming
- Names must reflect intent, not implementation.
- No abbreviations unless industry-standard.
- Boolean names must read as predicates (Is*, Has*, Can*).

## Mutability
- Prefer immutable data.
- State mutation must be explicit and localized.
- No hidden side effects in getters.

Violations are considered incorrect code generation.

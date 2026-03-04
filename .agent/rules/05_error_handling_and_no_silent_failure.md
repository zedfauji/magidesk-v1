# Error Handling and No Silent Failure

This project mandates explicit error propagation and handling.

## Rules
- Agents must not generate catch blocks that swallow exceptions.
- Errors must be logged or surfaced to the caller.
- Avoid patterns where failure state collapses silently.

## Behavioral Guarantees
Generated edits must include error handling consistent with project conventions.

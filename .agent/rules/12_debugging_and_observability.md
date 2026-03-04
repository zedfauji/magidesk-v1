# Debugging and Observability Standards

## Error Handling
- Every failure path must be observable.
- Exceptions must include actionable context.
- Stack traces must not be suppressed.

## Logging
- Logs must describe *state*, not just events.
- Avoid excessive logging in hot paths.
- Logging must not alter control flow.

## Debuggability
- Code must be readable under a debugger.
- Avoid clever constructs that obscure execution flow.

If a bug cannot be diagnosed from code + logs, the code is incorrect.

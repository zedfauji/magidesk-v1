Using the following GSD spec as the single source of truth:

(ai/specs/user-context-and-manager-override.gsd.md)

Generate a task list suitable for Ralph Loop execution.

Rules:
- Each task must be small, atomic, and verifiable
- Tasks must be ordered
- Each task must have a clear done condition
- Tasks must avoid overlap

Task categories must include:
1. Interface definition
2. Service implementation
3. DI wiring
4. ViewModel integration
5. Manager override integration
6. Cleanup of Guid.Empty / TODO usage
7. Build verification

Output ONLY the markdown content of:
ai/tasks/user-context-and-manager-override.tasks.md

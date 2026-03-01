Create a GSD specification for the following feature.

Project:
Magidesk POS (WinUI 3, Clean Architecture)

Feature:
UserContextService + Manager Override

The spec must define:
1. Purpose
2. Non-goals
3. Architecture boundaries
4. Required interfaces
5. Required behaviors
6. Explicit constraints
7. Acceptance criteria (verifiable)

System facts:
- Authentication UI already exists
- Domain layer must NOT be modified
- Manager override dialog already exists
- Current code uses Guid.Empty / TODOs for user identity
- DI is already stabilized

Constraints (NON-NEGOTIABLE):
- No new authentication UI
- No changes to Magidesk.Domain
- No parallel implementations
- No redesign of login flow
- Singleton lifetime for UserContextService

Acceptance criteria must include:
- Zero remaining Guid.Empty assignments
- All previous TODOs related to user identity resolved
- Application builds successfully

Output ONLY the markdown content of:
ai/specs/user-context-and-manager-override.gsd.md

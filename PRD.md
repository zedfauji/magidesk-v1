# UserContextService + Manager Override

## Purpose
Provide a single, authoritative source of truth for:
- Current logged-in user identity
- User roles
- Manager override enforcement

Eliminate all Guid.Empty and identity TODO usage.

## In Scope
- IUserContextService interface
- Concrete UserContextService implementation
- Integration with existing Login flow
- Manager override using existing dialog
- ViewModel updates to consume UserContextService

## Out of Scope
- New authentication UI
- Domain layer changes
- Redesign of login or authorization flows
- Payment or hardware features

## Architecture Constraints
- Clean Architecture must be respected
- Magidesk.Domain MUST NOT be modified
- UserContextService must be Singleton
- No parallel implementations allowed

## Manager Override Rules
- Manager override required only where already enforced conceptually
- Existing ManagerOverrideDialog must be reused
- Override returns true only on successful authorization

## Acceptance Criteria
- Zero remaining Guid.Empty usage for user identity
- All identity-related TODOs resolved
- Manager override works where required
- dotnet build succeeds with zero errors

## TASKS

1. Define IUserContextService interface in Application layer
   - Methods: GetCurrentUserId, IsInRole, RequireManagerOverrideAsync
   - Build must pass

2. Implement UserContextService in Presentation or Infrastructure layer
   - Track logged-in user after successful login
   - Build must pass

3. Register UserContextService in DI as Singleton
   - Remove any temporary identity hacks
   - Build must pass

4. Integrate UserContextService with LoginViewModel
   - Set current user on successful login
   - Build must pass

5. Replace all Guid.Empty / identity TODO usage in ViewModels
   - Use IUserContextService.GetCurrentUserId()
   - Build must pass

6. Wire manager override logic using existing ManagerOverrideDialog
   - Require override where already implied
   - Build must pass

7. Final cleanup and verification
   - Ensure no new TODOs added
   - dotnet build succeeds

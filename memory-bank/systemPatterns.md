# System Patterns

## Architecture Style
The project follows **Clean Architecture** with **Domain-Driven Design (DDD)** principles and **CQRS**.

### Layers
1.  **Domain (Core)**:
    -   Contains Enterprise Business Logic, Entities, Value Objects, and Domain Services.
    -   **Rule**: Must be pure .NET Standard/Core. NO dependencies on EF Core, UI, or external libraries.
    -   **Invariant**: All financial calculations happen here.
2.  **Application**:
    -   Contains Use Cases, CQRS Interfaces (Commands/Queries), and DTOs.
    -   Orchestrates logic but delegates business rules to the Domain.
3.  **Infrastructure**:
    -   Implements Interfaces (Repositories, Services).
    -   Contains EF Core DbContext, Migrations, and External Service integrations.
4.  **Presentation (UI)**:
    -   **WinUI 3** Desktop Application.
    -   **MVVM** Pattern (CommunityToolkit.Mvvm).
    -   **Rule**: ViewModels depend on Application Layer (MediatR), NEVER directly on DbContext.

## Key Patterns
-   **CQRS**: Separation of Read (Queries) and Write (Commands) operations.
-   **Repository Pattern**: Abstract data access.
-   **Result Pattern**: Use a Result<T> wrapper for unified error handling.
-   **Factory Pattern**: Use factories for complex entity creation (e.g., TicketFactory).
-   **Value Objects**: Use for immutable concepts like `Money`, `TaxRate`.

## Development Guardrails
-   **Forensic Verification**: Every PR/Commit must reference the specific F-XXXX feature it implements or verifies.
-   **Testing**:
    -   Domain Logic: 100% Unit Test coverage required.
    -   Infrastructure: Integration tests for Database constraints.
-   **Database**:
    -   Use `decimal(18,4)` for money columns.
    -   Invariants enforced at Database level (Check Constraints) where possible for critical safety.

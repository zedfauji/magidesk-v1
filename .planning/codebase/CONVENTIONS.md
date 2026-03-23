# Coding Conventions

**Analysis Date:** 2026-03-23

## Naming Patterns

**Files:**
- Command/Query handler files: `{CommandName}Handler.cs`, `{QueryName}Handler.cs`
  - Example: `ApplyUpdateCommandHandler.cs`, `CheckForUpdatesQueryHandler.cs`
- Command/Query definition files: `{CommandName}.cs`, `{QueryName}.cs`
  - Example: `CreateCategoryCommand.cs`, `CheckForUpdatesQuery.cs`
- Repository files: `I{EntityName}Repository.cs` (interface), `{EntityName}Repository.cs` (implementation)
  - Example: `ITicketRepository.cs`, `IInventoryItemRepository.cs`
- Test files: `{SubjectUnderTest}Tests.cs` or split into partial classes
  - Example: `UpdateInventoryItemCommandHandlerTests.cs`, `UpdateInventoryItemCommandHandlerTests.SkuValidation.cs`
- Domain entities: `{EntityName}.cs`
  - Example: `InventoryItem.cs`, `Money.cs`
- Value objects: `{ValueObjectName}.cs`
  - Example: `Money.cs` (with proper namespacing in `ValueObjects/`)
- DTOs: `{EntityName}Dto.cs` or grouped in files like `AuthDtos.cs`
  - Example: `UpdateAvailableDto.cs`, `OrderDtos.cs`

**Functions/Methods:**
- Handlers: `Handle()` or `HandleAsync()` - public interface for command/query handlers
  - Returns appropriate type (often records like `ApplyUpdateResult`, or domain entity Guid)
  - Async handlers use `async Task<T>` pattern
- Repository methods: `GetByIdAsync()`, `GetByNameAsync()`, `GetBySkuCodeAsync()`, `AddAsync()`, `UpdateAsync()`
  - All repository operations are async
- Factory/Creation: `Create()` static method on domain entities
  - Example: `InventoryItem.Create(name, unit, stockQuantity, ...)`
- State change methods: Descriptive verbs - `Activate()`, `Deactivate()`, `AdjustStock()`, `UpdateName()`
  - Used on domain entities to enforce invariants
- Background services: `GetNextTicketNumberAsync()`, `RouteToKitchenAsync()`

**Variables:**
- Private fields: `_camelCase` (with leading underscore)
  - Example: `_updateService`, `_logger`, `_tickets`, `_mockItemRepository`
- Parameters: `camelCase`
  - Example: `downloadUrl`, `installerPath`, `ticketNumber`
- Local variables: `camelCase`
- Constants: `UPPER_CASE` (private), or `UpperCase` for public constants
  - Example: `private const int DecimalPlaces = 2`, `private const string DefaultCurrency = "USD"`

**Types:**
- Commands: `{ActionName}Command` - record types inheriting from `IRequest<T>`
  - Example: `ApplyUpdateCommand(string DownloadUrl, string AssetName) : IRequest<ApplyUpdateResult>`
- Queries: `{QuestionName}Query` - record types inheriting from `IRequest<T>`
  - Example: `CheckForUpdatesQuery(string CurrentVersion) : IRequest<UpdateAvailableDto?>`
- Results/Responses: `{CommandName}Result` or `{DataName}Dto`
  - Example: `ApplyUpdateResult(bool Success, string? ErrorMessage)`
- Domain entities: PascalCase, typically in `Domain/Entities/`
  - Example: `InventoryItem`, `Ticket`, `InventoryCategory`
- Value objects: PascalCase with `record` keyword, in `Domain/ValueObjects/`
  - Example: `Money`, `UserId`, `MoneyAmount`
- Repositories: `I{EntityName}Repository` (interface), `{EntityName}Repository` (implementation)
  - Example: `IInventoryItemRepository`, `InventoryItemRepository`
- Exceptions: `{ContextName}Exception` or inherit from domain `BusinessRuleViolationException`
  - Example: `BusinessRuleViolationException`, `InvalidOperationException` (standard .NET)
- Interfaces: `I{InterfaceName}` or `I{ServiceName}Service`
  - Example: `IUpdateService`, `ISecurityService`, `IEncryptionService`, `ITicketRepository`
- Test doubles: `Stub{ServiceName}` (simple stubs) or `InMemory{RepositoryName}` (in-memory implementations)
  - Example: `StubKitchenRoutingService`, `InMemoryTicketRepository`

## Code Style

**Formatting:**
- C# 8.0+ language features enabled
- Nullable reference types enabled: `<Nullable>enable</Nullable>`
- ImplicitUsings enabled: `<ImplicitUsings>enable</ImplicitUsings>` (auto-imports common namespaces)
- Target framework: .NET 8.0

**Linting:**
- No explicit ESLint or StyleCop configuration detected in project root
- SonarQube rulesets present in `.sonarqube/conf/` for static analysis
- Follow standard C# conventions and .NET Core guidelines

## Import Organization

**Order:**
1. System namespaces (`using System;`, `using System.Threading;`)
2. System.Collections and core framework namespaces
3. Microsoft namespaces (`using Microsoft.AspNetCore`, `using Microsoft.Extensions`)
4. Third-party packages (`using Moq`, `using Xunit`, `using FluentAssertions`, `using MediatR`)
5. Project-specific namespaces (`using Magidesk.Domain`, `using Magidesk.Application`)

**Example from handlers:**
```csharp
using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Magidesk.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Magidesk.Application.Commands;
```

**Path Aliases:**
- No explicit path aliases used (standard .NET conventions)
- Namespaces correspond to folder structure: `src/Magidesk.Application/Commands/` → `namespace Magidesk.Application.Commands`

## Error Handling

**Patterns:**
- Throw `InvalidOperationException` for domain rule violations with descriptive messages
  - Example: `throw new InvalidOperationException("Item not found");`
  - Example: `throw new InvalidOperationException("SKU code already exists");`
- Use custom `BusinessRuleViolationException` for complex domain rule failures
  - Caught and mapped to HTTP 400 Bad Request in `GlobalExceptionHandler`
- Use `ArgumentException` for invalid method parameters
  - Example: `throw new ArgumentException("Name cannot be empty");`
- Use `ArgumentException` with named parameter for null checks and validation
  - Example: `throw new ArgumentException("Money amount cannot be negative.", nameof(amount));`
- Let standard exceptions propagate (`DbUpdateConcurrencyException`, `UnauthorizedAccessException`)

**Exception Mapping in API Layer:**
- `GlobalExceptionHandler` (located at `src/Magidesk.Api/Infrastructure/GlobalExceptionHandler.cs`) maps exceptions to HTTP status codes:
  - `BusinessRuleViolationException` → 400 Bad Request
  - `ArgumentException` → 400 Bad Request
  - `KeyNotFoundException` → 404 Not Found
  - `UnauthorizedAccessException` → 403 Forbidden
  - `DbUpdateConcurrencyException` → 409 Conflict
  - `InvalidOperationException` → 409 Conflict (state conflicts)
  - Unhandled exceptions → 500 Internal Server Error

**Handlers/Queries pattern:**
- Try-catch at handler level for commands with result objects (like `ApplyUpdateResult`)
  - Example: `ApplyUpdateCommandHandler.cs` catches exceptions and returns failure result
- Let exceptions propagate from query handlers; exceptions handled at API controller level

## Logging

**Framework:** `Microsoft.Extensions.Logging`

**Patterns:**
- Injected as `ILogger<T>` where T is the handler/service class
  - Example: `_logger = logger;` in constructor
- Log level `LogError` for exceptions with exception object and formatted message
  - Example: `_logger.LogError(ex, "Failed to apply update from {Url}", request.DownloadUrl);`
- Use structured logging with named placeholders: `{FieldName}` instead of string interpolation
- Logged at handler level, especially for cross-cutting concerns (exceptions, important state changes)

## Comments

**When to Comment:**
- Private field/method purpose if not obvious from name
- Complex business logic that isn't self-explanatory
- Rationale for non-obvious design decisions

**JSDoc/TSDoc:**
- Use XML documentation comments (`/// <summary>`) for public APIs (commands, queries, handlers, interfaces)
- Include `<summary>`, `<param>`, `<returns>`, `<exception>` tags
- Example from `CreateCategoryCommandHandler`:
  ```csharp
  /// <summary>
  /// Handles the CreateCategoryCommand by validating inputs and creating the category.
  /// </summary>
  /// <param name="request">The command containing category creation data.</param>
  /// <param name="cancellationToken">Cancellation token for the async operation.</param>
  /// <returns>The unique identifier of the created category.</returns>
  /// <exception cref="InvalidOperationException">
  /// Thrown when category name already exists or parent category is not found/inactive.
  /// </exception>
  ```

**Domain Entity Comments:**
- Use summary comments on properties and methods
- Include constraints and invariants in property documentation
- Example from `Money`:
  ```csharp
  /// <summary>
  /// Gets the monetary amount (rounded to 2 decimal places).
  /// </summary>
  public decimal Amount { get; }
  ```

## Function Design

**Size:**
- Handler methods typically 20-50 lines (averaging ~30)
- Domain entity factory methods use static `Create()` pattern
- Validation logic often extracted into separate `if` blocks with labeled comments (Step 1, Step 2, etc.)

**Parameters:**
- Commands/Queries passed as single `request` parameter + `CancellationToken`
- Domain entity methods operate on `this` with value parameters
- Repositories use standard async pattern: `async Task<T>` with `CancellationToken` parameter

**Return Values:**
- Commands return result objects (records): `ApplyUpdateResult`, `ApplyUpdateCommand` → `IRequest<ApplyUpdateResult>`
- Queries return DTOs or domain entities: `CheckForUpdatesQuery` → `IRequest<UpdateAvailableDto?>`
- Handlers return nullable types when result may not exist: `IRequest<UpdateAvailableDto?>` (note the `?`)
- Repository methods return `Task<T?>` for single entity lookups (nullable)
- Domain entity static factories return the entity type: `static InventoryItem Create(...)`

## Module Design

**Exports:**
- Commands/Queries: Defined as records inheriting from `IRequest<T>` in dedicated files
  - Located in `src/Magidesk.Application/Commands/` or `src/Magidesk.Application/Queries/`
- Handlers: Classes implementing `IRequestHandler<TRequest, TResponse>` or `IRequestHandler<TRequest>` (MediatR pattern)
  - Located in same namespace as related command/query or in `Handlers/` subdirectory
- Interfaces: Defined in `src/Magidesk.Application/Interfaces/`
  - Each interface typically in its own file or grouped logically
- Domain entities: Entities and value objects exported from `src/Magidesk.Domain/Entities/` and `src/Magidesk.Domain/ValueObjects/`

**Barrel Files:**
- No barrel files (index.ts/index.cs style exports) used
- Direct namespace-based imports preferred
- Services registered via dependency injection in `DependencyInjection/ServiceCollectionExtensions.cs`

**Namespacing:**
- Commands under `Magidesk.Application.Commands` or `Magidesk.Application.Commands.{Feature}`
  - Example: `Magidesk.Application.Commands.Inventory.CreateCategoryCommand`
  - Handlers in same namespace or `Handlers` subdirectory
- Queries under `Magidesk.Application.Queries` or `Magidesk.Application.Queries.{Feature}`
- DTOs under `Magidesk.Application.DTOs` or `Magidesk.Application.DTOs.{Feature}`
- Domain logic under `Magidesk.Domain.Entities` or `Magidesk.Domain.ValueObjects`
- Infrastructure under `Magidesk.Infrastructure.Services` or specific feature namespaces

---

*Convention analysis: 2026-03-23*

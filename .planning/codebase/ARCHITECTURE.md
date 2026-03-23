# Architecture

**Analysis Date:** 2026-03-23

## Pattern Overview

**Overall:** Clean Architecture with Domain-Driven Design (DDD) and CQRS (Command Query Responsibility Segregation)

**Key Characteristics:**
- Layered architecture: Domain → Application → Infrastructure → Presentation
- Dependency injection (Microsoft.Extensions.DependencyInjection) for loose coupling
- MediatR for command/query dispatching with handler pattern
- Entity Framework Core for ORM with code-first migrations
- Multiple presentation tiers: WinUI 3 desktop client and ASP.NET Core API backend

## Layers

**Domain Layer (`Magidesk.Domain`):**
- Purpose: Core business logic, entities, value objects, and domain services
- Location: `/src/Magidesk.Domain/`
- Contains: Aggregate roots (Ticket, Order, Payment), Value Objects (Money, UserId), Domain Entities (Shift, CashSession, Table, MenuItem)
- Depends on: Nothing (pure domain logic)
- Used by: Application, Infrastructure for validation and business rules

**Application Layer (`Magidesk.Application`):**
- Purpose: Use cases, commands, queries, DTOs, and application services
- Location: `/src/Magidesk.Application/`
- Contains: Command handlers (CreateTicketCommandHandler, ProcessPaymentCommandHandler), Query handlers (GetTicketQueryHandler, GetOpenTicketsQueryHandler), Application Services (TicketCreationService, CashSessionService, KitchenRoutingService), DTOs
- Depends on: Domain layer for business rules and entities
- Used by: Presentation (WinUI), API controllers

**Infrastructure Layer (`Magidesk.Infrastructure`):**
- Purpose: Data persistence, external integrations, printing, payment processing
- Location: `/src/Magidesk.Infrastructure/`
- Contains:
  - Data: ApplicationDbContext, DbContextFactory, entity configurations
  - Repositories: EfRepository (generic), specific repos (TicketRepository, UserRepository, InventoryItemRepository)
  - Services: DatabaseConfigurationService, DatabaseSeedingService, SystemInitializationService, PrintService implementations
  - PaymentGateways: MockPaymentGateway, payment processing
  - Printing: Print drivers (EscPosDriver, PlainTextDriver), layout engines, receipt/kitchen print services
- Depends on: Domain and Application layers
- Used by: Application and Presentation layers

**Presentation Layer - Desktop (`Magidesk.Presentation`):**
- Purpose: WinUI 3 desktop UI for POS operations
- Location: `/src/Magidesk.Presentation/`
- Contains: Views (XAML), ViewModels, Services (NavigationService, DialogService, UserService), Controls, Converters
- Depends on: Application and Infrastructure layers
- Used by: End users through WinUI app

**Presentation Layer - API (`Magidesk.Api`):**
- Purpose: ASP.NET Core REST API backend, Kitchen Display System (KDS) hub
- Location: `/src/Magidesk.Api/`
- Contains: Controllers (AuthController, OrdersController, KitchenController, ReportsController), DTOs, SignalR Hubs (KitchenHub)
- Depends on: Application and Infrastructure layers
- Used by: Frontend clients (web, mobile), KDS displays

**Testing Layers:**
- `Magidesk.Domain.Tests`: Unit tests for domain logic and entities
- `Magidesk.Application.Tests`: Unit tests for command/query handlers, application services
- `Magidesk.Infrastructure.Tests`: Integration tests for repositories and services
- `Magidesk.Tests.E2E`: End-to-end tests for workflows
- `Magidesk.Tests.Workflows`: Business workflow tests

## Data Flow

**Order Creation Flow:**

1. **Presentation** (OrderPageViewModel) collects order data
2. **Command Dispatch** (MediatR): Sends `CreateTicketCommand`
3. **Handler Execution** (`CreateTicketCommandHandler`): Validates, creates Ticket aggregate
4. **Domain Logic** (TicketCreationService): Enforces business rules (pricing, taxes, guards)
5. **Persistence** (TicketRepository): Saves Ticket via ApplicationDbContext
6. **Result** (CreateTicketResult): Returns TicketId and TicketNumber
7. **UI Update** (OrderPageViewModel): Updates order lines, totals, and kitchen printing

**Payment Processing Flow:**

1. **UI** (PaymentViewModel): Initiates `ProcessPaymentCommand`
2. **Handler** (ProcessPaymentCommandHandler): Validates payment amount
3. **Gateway** (IPaymentGateway): Processes card/cash payment (MockPaymentGateway in dev)
4. **Domain** (PaymentDomainService): Applies payment to Ticket aggregate
5. **Persistence** (PaymentRepository): Saves Payment entity
6. **Notification** (IKitchenNotificationPublisher): Notifies KDS if ticket ready
7. **Result**: Payment confirmation or error

**Report Query Flow:**

1. **UI** (SalesReportsViewModel): Requests `GetSalesBalanceQuery`
2. **Handler** (GetSalesBalanceQueryHandler): Reads from repositories (no mutation)
3. **Optimization** (ReportCacheService): Checks cache before database hit
4. **Data** (SalesReportRepository + joins): Aggregates data from Tickets, Payments
5. **DTO** (SalesBalanceReportDto): Returns formatted report data
6. **UI** (SalesReportsViewModel): Renders report with charts/grids

**State Management:**

- **Command State**: Aggregates (Ticket, CashSession) maintain state; Handlers apply commands
- **Query State**: Read repositories return DTOs; no direct state modification
- **Context State**: ITerminalContext (singleton in desktop), IUserService track current user/terminal
- **UI State**: ViewModels (transient) manage page-specific state; NavigationService routes pages

## Key Abstractions

**Command Pattern:**
- Purpose: Encapsulates operations as objects with explicit intent
- Examples: `CreateTicketCommand`, `ProcessPaymentCommand`, `CloseTicketCommand`
- Pattern: Commands implement `IRequest<TResponse>`, handlers implement `IRequestHandler<TCommand, TResponse>`
- Location: `/src/Magidesk.Application/Commands/`

**Query Pattern:**
- Purpose: Fetch data without side effects
- Examples: `GetTicketQuery`, `GetOpenTicketsQuery`, `GetSalesBalanceQuery`
- Pattern: Queries implement `IRequest<TResponse>`, handlers implement `IRequestHandler<TQuery, TResponse>`
- Location: `/src/Magidesk.Application/Queries/`

**Repository Pattern:**
- Purpose: Abstract data access, allow swapping implementations
- Implementations: `EfRepository<T>` (generic), `TicketRepository`, `UserRepository`, `PaymentRepository`
- Pattern: Repositories implement `IRepository<T>` interface
- Location: `/src/Magidesk.Infrastructure/Repositories/`

**Aggregate Root:**
- Purpose: Enforce domain consistency boundaries
- Examples: Ticket (root for OrderLine, Payment, TicketDiscount), CashSession (root for Payout, CashDrop)
- Pattern: Aggregates encapsulate collections, expose read-only interfaces
- Location: `/src/Magidesk.Domain/Entities/`

**Value Object:**
- Purpose: Represent immutable domain concepts without identity
- Examples: Money (amount + currency), UserId (scalar identifier), TableNumbers (list)
- Pattern: Equality by value, no entity identity
- Location: `/src/Magidesk.Domain/ValueObjects/`

**Domain Service:**
- Purpose: Encapsulate business logic that doesn't fit single entity
- Examples: TaxDomainService, PaymentDomainService, DiscountDomainService, PriceCalculator
- Pattern: Stateless services with dependency injection
- Location: `/src/Magidesk.Domain/Services/`, `/src/Magidesk.Domain/DomainServices/`

**Application Service:**
- Purpose: Orchestrate operations across multiple aggregates
- Examples: CashSessionService, KitchenRoutingService, TicketCreationService
- Pattern: Injected with repositories and domain services
- Location: `/src/Magidesk.Application/Services/`

**DTO (Data Transfer Object):**
- Purpose: Transfer data between layers without exposing domain model
- Examples: TicketDto, OrderLineDto, PaymentDto, SalesBalanceReportDto
- Pattern: Flat structures optimized for presentation
- Location: `/src/Magidesk.Application/DTOs/`

## Entry Points

**Desktop Application:**
- Location: `/src/Magidesk.Presentation/App.xaml.cs`
- Triggers: Windows app launch
- Responsibilities: DI setup (AddApplication, AddInfrastructure, AddPresentation), database configuration check, system initialization, navigation to LoginPage

**API Server:**
- Location: `/src/Magidesk.Api/Program.cs`
- Triggers: ASP.NET Core startup
- Responsibilities: DI configuration (AddInfrastructure, AddApplication), middleware setup, controller routing, SignalR hub mapping

**Kitchen Display System Hub:**
- Location: `/src/Magidesk.Api/Hubs/KitchenHub.cs`
- Triggers: Client connections via SignalR
- Responsibilities: Real-time kitchen order updates via IKitchenNotificationPublisher

## Error Handling

**Strategy:** Layered exception handling with domain-specific exceptions and global handlers

**Patterns:**

- **Domain Exceptions** (`/src/Magidesk.Domain/Exceptions/`):
  - InvalidOperationException, InvalidTicketStateException, InsufficientFundsException
  - Thrown from domain entities and domain services to prevent invalid state

- **Command Validation**:
  - FluentValidation validators on commands
  - Example: `BulkUpdateInventoryItemsCommandValidator` checks rules before handler execution
  - Location: `/src/Magidesk.Application/Commands/` (inline or separate)

- **Global Exception Handler** (API):
  - `GlobalExceptionHandler` middleware in `/src/Magidesk.Api/Infrastructure/`
  - Catches all unhandled exceptions, logs, returns ProblemDetails
  - Handles: validation errors, domain exceptions, unexpected errors

- **Startup Error Handling** (Desktop):
  - App.xaml.cs: Fatal error handlers for initialization, database setup failures
  - AppDomain.UnhandledException, TaskScheduler.UnobservedTaskException handlers
  - Fallback: Logs to crash_log.txt in AppData\Local\Magidesk\Logs

## Cross-Cutting Concerns

**Logging:**
- Desktop: StartupLogger to crash_log.txt in AppData
- API: Microsoft.Extensions.Logging with configuration filters
- Pattern: Logged at startup, errors, critical operations (ISystemInitializationService, handlers)

**Validation:**
- Domain: Aggregate root guards (e.g., Ticket.SetServiceCharge validates percentage)
- Application: FluentValidation on commands before handler
- Pattern: Fail-fast, return validation errors to caller

**Authentication:**
- Desktop: IUserService (singleton) holds CurrentUser after login
- API: Bearer token (JWT assumed, not fully visible)
- Terminal context: ITerminalContext tracks current terminal identity
- Pattern: Set at login, checked in commands that require authorization

**Authorization:**
- Pattern: Manager override (ManagerOverrideService) for sensitive operations
- Examples: Void ticket, refund, drawer bleed
- Implementation: `AuthorizeManagerCommand` validates manager PIN before operation

**Database Transactions:**
- EF Core implicit transactions per SaveChanges
- Pattern: Single aggregate modification per command
- Interceptor: VersionIncrementInterceptor handles optimistic concurrency (Version property)

**Caching:**
- Services.EnhancedCachingService (IMemoryCache-based, singleton)
- ReportCacheService: Caches expensive report queries
- Location: `/src/Magidesk.Infrastructure/Services/`

**Printing:**
- Abstraction: IPrintLayoutEngine, IPrintDriver, IRawPrintService
- Drivers: EscPosDriver (thermal), PlainTextDriver (standard)
- Adapters: Thermal58mmAdapter, Thermal80mmAdapter, StandardPageAdapter
- Template Engine: LiquidTemplateEngine for dynamic receipt layouts
- Location: `/src/Magidesk.Infrastructure/Printing/`

---

*Architecture analysis: 2026-03-23*

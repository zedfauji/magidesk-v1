# Codebase Structure

**Analysis Date:** 2026-03-23

## Directory Layout

```
Magidesk/
├── .planning/                          # GSD planning documents (generated)
├── .gsd/                               # GSD tool configuration
├── src/
│   ├── Magidesk.sln                    # Main solution file (Desktop + API)
│   ├── Magidesk.Installer.sln          # Separate installer solution
│   │
│   ├── Magidesk.Domain/                # Core domain layer (DDD)
│   ├── Magidesk.Application/           # Use cases, commands, queries
│   ├── Magidesk.Infrastructure/        # Data access, services, external integrations
│   ├── Magidesk.Migrations/            # EF Core migrations and seeding
│   │
│   ├── Magidesk.Presentation/          # WinUI 3 desktop application
│   ├── Magidesk.Api/                   # ASP.NET Core REST API + SignalR hubs
│   │
│   ├── Magidesk.Domain.Tests/          # Domain entity & service tests
│   ├── Magidesk.Application.Tests/     # Command/query handler tests
│   ├── Magidesk.Infrastructure.Tests/  # Repository & service tests
│   ├── Magidesk.Tests.E2E/             # End-to-end workflow tests
│   ├── Magidesk.Tests.Workflows/       # Business process tests
│   │
│   ├── Magidesk.Installer/             # WiX installer UI & components
│   ├── Magidesk.Installer.CustomActions/
│   └── Magidesk.Installer.PropertyTests/
├── build/                              # Build artifacts, staging, deployment
├── Scripts/                            # Build and deployment scripts
└── [config files]                      # .gitignore, README, PRD, etc.
```

## Directory Purposes

**Magidesk.Domain:**
- Purpose: Pure business domain with no external dependencies
- Contains: Entities (Ticket, OrderLine, Payment, CashSession, Table, MenuItem, User), Value Objects (Money, UserId), Domain Services (TaxDomainService, PaymentDomainService), Exceptions, Enumerations
- Key files:
  - `Entities/Ticket.cs`: Aggregate root for orders
  - `Entities/CashSession.cs`: Aggregate root for cash management
  - `Entities/MenuItem.cs`: Menu item definition
  - `ValueObjects/Money.cs`: Currency-aware monetary value
  - `Services/`: TaxDomainService, PaymentDomainService, DiscountDomainService, PriceCalculator
  - `Exceptions/`: InvalidOperationException, InvalidTicketStateException

**Magidesk.Application:**
- Purpose: Application logic layer implementing CQRS
- Contains:
  - `Commands/`: Command classes (CreateTicketCommand, ProcessPaymentCommand, etc.)
  - `Queries/`: Query classes (GetTicketQuery, GetOpenTicketsQuery, etc.)
  - `Commands/[CommandName]Handler.cs`: Command implementation handlers
  - `Queries/[QueryName]Handler.cs`: Query implementation handlers
  - `DTOs/`: Data transfer objects for cross-layer communication
  - `Interfaces/`: Service contracts (ITicketRepository, IKitchenRoutingService, etc.)
  - `Services/`: Application services (TicketCreationService, CashSessionService, KitchenRoutingService)
  - `Mapping/`: AutoMapper profiles for DTO mapping
  - `DependencyInjection/ServiceCollectionExtensions.cs`: MediatR and handler registration
- Key files:
  - `Commands/CreateTicketCommand.cs` & handler: Core ticket creation logic
  - `Commands/ProcessPaymentCommand.cs` & handler: Payment processing
  - `Queries/GetOpenTicketsQuery.cs` & handler: Fetch open tickets
  - `Services/TicketCreationService.cs`: Business rule enforcement for ticket creation
  - `DTOs/TicketDto.cs`: Ticket data transfer format

**Magidesk.Infrastructure:**
- Purpose: External dependencies, data persistence, integrations
- Contains:
  - `Data/ApplicationDbContext.cs`: EF Core DbContext (80+ DbSet properties)
  - `Data/DbContextFactory.cs`: Factory pattern to delay DB connection
  - `Data/Configurations/`: Entity type configurations for EF Core
  - `Repositories/`: IRepository implementations (EfRepository generic + 40+ specific repos)
  - `Services/`: Domain/Application service implementations (DatabaseConfigurationService, SystemInitializationService, CashBalanceTrackingService, UpdateService)
  - `PaymentGateways/`: Payment gateway implementations (MockPaymentGateway)
  - `Printing/`: Printing infrastructure (KitchenPrintService, ReceiptPrintService, drivers, adapters)
  - `Security/`: AesEncryptionService, SecurityService
  - `DependencyInjection/ServiceCollectionExtensions.cs`: Infrastructure service registration
- Key files:
  - `Data/ApplicationDbContext.cs`: 80+ entities mapped
  - `Repositories/TicketRepository.cs`: Ticket data access
  - `Repositories/EfRepository.cs`: Generic repository implementation
  - `Services/SystemInitializationService.cs`: System startup logic
  - `Services/GithubUpdateService.cs`: Auto-update mechanism
  - `Printing/KitchenPrintService.cs`: Kitchen printer integration

**Magidesk.Migrations:**
- Purpose: Database migration scripts and seeding
- Contains:
  - `Migrations/`: EF Core migration files (timestamps + Up/Down)
  - `Seeding/`: Initial data population (MenuItems, Discounts, OrderTypes, etc.)
- Key files:
  - `Migrations/[timestamp]_[description].cs`: Schema changes
  - `Seeding/`: Restaurant configuration, tax rates, default items

**Magidesk.Presentation:**
- Purpose: WinUI 3 desktop UI for POS operations
- Contains:
  - `Views/`: XAML page definitions (OrderPage.xaml, LoginPage.xaml, CashSessionPage.xaml, etc.)
  - `Views/Dialogs/`: Modal dialogs (ConfirmationDialog.xaml, ManagerPinDialog.xaml, etc.)
  - `ViewModels/`: MVVM view models (OrderPageViewModel, PaymentViewModel, etc.)
  - `ViewModels/Dialogs/`: Dialog view models (ManagerPinDialogViewModel, etc.)
  - `Services/`: NavigationService, DialogService, UserService, UpdateNotificationService
  - `Controls/`: Custom XAML controls
  - `Converters/`: Value converters for XAML binding
  - `Configuration/appsettings.json`: App configuration and defaults
  - `Configuration/appsettings.defaults.json`: Template for defaults
  - `Styles/`: XAML resource dictionaries (brushes, fonts, templates)
  - `Strings/`: Localization resources
  - `Assets/`: Images, icons
  - `DependencyInjection/ServiceCollectionExtensions.cs`: Presentation service registration
- Key files:
  - `App.xaml.cs`: Application startup, DI composition, initialization
  - `MainWindow.xaml`: Root window shell
  - `Views/LoginPage.xaml`: Authentication entry point
  - `Views/OrderPage.xaml`: Main POS order entry UI
  - `Views/PaymentPage.xaml`: Payment processing
  - `ViewModels/OrderPageViewModel.cs`: Order page logic

**Magidesk.Api:**
- Purpose: ASP.NET Core REST API and real-time communication
- Contains:
  - `Program.cs`: Startup configuration (DI, middleware, controllers, SignalR)
  - `Controllers/`: API endpoints (AuthController, OrdersController, KitchenController, ReportsController, etc.)
  - `Hubs/KitchenHub.cs`: SignalR hub for KDS real-time updates
  - `Services/`: HTTP-specific services (HttpUserService, HttpTerminalContext, SignalRKitchenNotificationPublisher)
  - `Infrastructure/GlobalExceptionHandler.cs`: Global error handling middleware
  - `Dtos/`: API transfer objects
  - `deployment/`: Deployment configuration files
- Key files:
  - `Program.cs`: API configuration entry point
  - `Controllers/OrdersController.cs`: Order API endpoints
  - `Controllers/KitchenController.cs`: Kitchen management endpoints
  - `Hubs/KitchenHub.cs`: Kitchen display system real-time hub
  - `Infrastructure/GlobalExceptionHandler.cs`: Exception handling

**Magidesk.Domain.Tests:**
- Purpose: Unit test domain entities and services
- Contains:
  - `Entities/`: Tests for domain entities (TicketTests, CashSessionTests, etc.)
  - `DomainServices/`: Tests for domain services (TaxDomainServiceTests, PaymentDomainServiceTests)
  - `Compliance/`: Compliance and validation tests
  - `ValueObjects/`: Tests for value objects
- Test organization: One test file per entity/service

**Magidesk.Application.Tests:**
- Purpose: Unit test commands, queries, and application services
- Contains:
  - `Commands/`: Command handler tests
  - `Queries/`: Query handler tests
  - `Handlers/`: Handler-specific tests
  - `Services/`: Application service tests
  - `TestDoubles/`: Mocks and fakes for dependencies
- Test organization: [CommandName]HandlerTests, [QueryName]HandlerTests

**Magidesk.Infrastructure.Tests:**
- Purpose: Integration tests for repositories and infrastructure services
- Contains:
  - `Repositories/`: Repository tests
  - `Services/`: Infrastructure service tests (DatabaseConfigurationServiceTests, etc.)
  - `Compliance/`: Database compliance tests
- Test organization: [RepositoryName]Tests, [ServiceName]Tests

**build/:**
- Purpose: Build outputs and installer artifacts
- Contains:
  - `installer/staging/app/`: Staged application binaries (DLLs, EXE)
  - `installer/staging/tools/`: Tools like efbundle.exe
- Generated: By build process, not committed (in .gitignore)

## Key File Locations

**Entry Points:**
- `src/Magidesk.Presentation/App.xaml.cs`: Desktop app startup, DI composition, database initialization
- `src/Magidesk.Api/Program.cs`: API server configuration, middleware, routing
- `src/Magidesk.Presentation/Views/LoginPage.xaml`: Authentication UI entry point
- `src/Magidesk.Presentation/Views/OrderPage.xaml`: Main POS order entry UI
- `src/Magidesk.Presentation/MainWindow.xaml.cs`: Root window shell hosting frames

**Configuration:**
- `src/Magidesk.Presentation/Configuration/appsettings.defaults.json`: Default app settings
- `src/Magidesk.Infrastructure/DependencyInjection/ServiceCollectionExtensions.cs`: Infrastructure service wiring
- `src/Magidesk.Application/DependencyInjection/ServiceCollectionExtensions.cs`: MediatR and handler wiring
- `src/Magidesk.Presentation/DependencyInjection/ServiceCollectionExtensions.cs`: Presentation service wiring
- `src/Magidesk.Api/Program.cs`: API service wiring

**Core Logic:**
- `src/Magidesk.Domain/Entities/Ticket.cs`: Order aggregate root
- `src/Magidesk.Domain/Entities/CashSession.cs`: Cash management aggregate
- `src/Magidesk.Domain/Services/TaxDomainService.cs`: Tax calculation business logic
- `src/Magidesk.Domain/Services/PaymentDomainService.cs`: Payment processing logic
- `src/Magidesk.Application/Services/TicketCreationService.cs`: Ticket creation orchestration
- `src/Magidesk.Application/Services/CashSessionService.cs`: Cash session management
- `src/Magidesk.Application/Services/KitchenRoutingService.cs`: Kitchen order routing

**Testing:**
- `src/Magidesk.Domain.Tests/Entities/TicketTests.cs`: Ticket entity tests
- `src/Magidesk.Application.Tests/Commands/CreateTicketCommandHandlerTests.cs`: Handler tests
- `src/Magidesk.Infrastructure.Tests/Repositories/TicketRepositoryTests.cs`: Persistence tests
- `src/Magidesk.Tests.E2E/Tests/`: End-to-end workflow tests
- `src/Magidesk.Tests.Workflows/Workflows/`: Business process tests

## Naming Conventions

**Files:**
- Entities: `[EntityName].cs` (Ticket.cs, OrderLine.cs, CashSession.cs)
- Commands: `[ActionName]Command.cs` and `[ActionName]CommandHandler.cs` (CreateTicketCommand.cs, CreateTicketCommandHandler.cs)
- Queries: `[FetchName]Query.cs` and `[FetchName]QueryHandler.cs` (GetTicketQuery.cs, GetTicketQueryHandler.cs)
- Repositories: `[EntityName]Repository.cs` (TicketRepository.cs, UserRepository.cs)
- Services: `[FunctionName]Service.cs` (TicketCreationService.cs, CashSessionService.cs)
- DTOs: `[EntityName]Dto.cs` (TicketDto.cs, PaymentDto.cs)
- Views: `[PageName]Page.xaml` (OrderPage.xaml, LoginPage.xaml)
- ViewModels: `[PageName]ViewModel.cs` (OrderPageViewModel.cs, LoginViewModel.cs)
- Tests: `[TargetClass]Tests.cs` (TicketTests.cs, CreateTicketCommandHandlerTests.cs)

**Directories:**
- Features/domains: Plural for collections (Entities, Commands, Queries, Services, Repositories)
- UI pages: `Views/[PageName].xaml` (Views/OrderPage.xaml, Views/LoginPage.xaml)
- Dialogs: `Views/Dialogs/[DialogName].xaml` (Views/Dialogs/ConfirmationDialog.xaml)
- View models: `ViewModels/[ClassName].cs` (ViewModels/OrderPageViewModel.cs)
- Custom controls: `Controls/[ControlName].xaml` (Controls/MenuItemControl.xaml)
- Helpers/Utilities: `Services/[ServiceName].cs` (Services/NavigationService.cs)

## Where to Add New Code

**New Feature (e.g., loyalty points system):**

1. **Domain Entity** → `src/Magidesk.Domain/Entities/LoyaltyAccount.cs`
   - Define aggregate root with business rules
   - Add exceptions if needed: `src/Magidesk.Domain/Exceptions/InvalidLoyaltyPointsException.cs`

2. **Domain Service** → `src/Magidesk.Domain/Services/LoyaltyPointsService.cs`
   - Implement business logic spanning aggregates

3. **Commands & Handlers** → `src/Magidesk.Application/Commands/RedeemPointsCommand.cs` + handler
   - One file per command (command + result classes together)
   - Handler: `RedeemPointsCommandHandler.cs`

4. **Queries & Handlers** → `src/Magidesk.Application/Queries/GetLoyaltyAccountQuery.cs` + handler
   - Query class and handler in separate files or same folder

5. **DTOs** → `src/Magidesk.Application/DTOs/LoyaltyAccountDto.cs`
   - Transfer objects for cross-layer communication

6. **Repository** → `src/Magidesk.Infrastructure/Repositories/LoyaltyAccountRepository.cs`
   - Implement `IRepository<LoyaltyAccount>`
   - Register in `/src/Magidesk.Infrastructure/DependencyInjection/ServiceCollectionExtensions.cs`

7. **Migration** → `src/Magidesk.Migrations/Migrations/[timestamp]_AddLoyaltyTables.cs`
   - Use `dotnet ef migrations add AddLoyaltyTables`
   - Create Up/Down methods

8. **UI** → `src/Magidesk.Presentation/Views/LoyaltyPage.xaml`
   - Create page view
   - Create `src/Magidesk.Presentation/ViewModels/LoyaltyPageViewModel.cs`
   - Register ViewModel in `/src/Magidesk.Presentation/DependencyInjection/ServiceCollectionExtensions.cs`

9. **API Endpoints** → `src/Magidesk.Api/Controllers/LoyaltyController.cs`
   - Create controller with endpoints
   - Use injected IMediator to dispatch commands/queries

10. **Tests**:
    - Domain tests: `src/Magidesk.Domain.Tests/Entities/LoyaltyAccountTests.cs`
    - Command tests: `src/Magidesk.Application.Tests/Commands/RedeemPointsCommandHandlerTests.cs`
    - Repo tests: `src/Magidesk.Infrastructure.Tests/Repositories/LoyaltyAccountRepositoryTests.cs`

**New Page/Screen (e.g., Loyalty Management):**

1. **View** → `src/Magidesk.Presentation/Views/LoyaltyPage.xaml`
   - XAML UI definition
   - Code-behind: minimal (just Navigate wiring)

2. **ViewModel** → `src/Magidesk.Presentation/ViewModels/LoyaltyPageViewModel.cs`
   - MVVM pattern: properties bound to View, commands for actions
   - Inject: IMediator, INavigationService, IDialogService

3. **Register ViewModel** → `src/Magidesk.Presentation/DependencyInjection/ServiceCollectionExtensions.cs`
   - Add: `services.AddTransient<LoyaltyPageViewModel>();`

4. **Navigation** → Update `NavigationService` if custom routing needed

**New Dialog/Modal:**

1. **Dialog View** → `src/Magidesk.Presentation/Views/Dialogs/LoyaltySelectionDialog.xaml`

2. **Dialog ViewModel** → `src/Magidesk.Presentation/ViewModels/Dialogs/LoyaltySelectionViewModel.cs`

3. **Register** → `src/Magidesk.Presentation/DependencyInjection/ServiceCollectionExtensions.cs`
   - Add transient registration for ViewModel and View

4. **Launch** → From parent ViewModel:
   ```csharp
   var result = await _dialogService.ShowDialogAsync<LoyaltySelectionViewModel>();
   ```

**New Repository:**

1. Create: `src/Magidesk.Infrastructure/Repositories/[EntityName]Repository.cs`
   - Implement `IRepository<TEntity>` or inherit `EfRepository<TEntity>`
   - Add specific query methods if needed

2. Register: In `src/Magidesk.Infrastructure/DependencyInjection/ServiceCollectionExtensions.cs`
   ```csharp
   services.AddScoped<ILoyaltyAccountRepository, LoyaltyAccountRepository>();
   ```

3. DbSet: Add to `src/Magidesk.Infrastructure/Data/ApplicationDbContext.cs`
   ```csharp
   public DbSet<LoyaltyAccount> LoyaltyAccounts { get; set; } = null!;
   ```

**New Service:**

1. **Interface** → `src/Magidesk.Application/Interfaces/[IServiceName].cs` (optional, for abstraction)

2. **Implementation** → `src/Magidesk.Application/Services/[ServiceName].cs` or `src/Magidesk.Infrastructure/Services/[ServiceName].cs`

3. **Register** → `DependencyInjection/ServiceCollectionExtensions.cs`
   - Application services: `AddApplication()`
   - Infrastructure services: `AddInfrastructure()`

## Special Directories

**Magidesk.Migrations:**
- Purpose: Database schema management
- Generated: by EF Core migrations
- Committed: Yes, contains schema history
- Convention: Timestamp_Description.cs format

**.planning/codebase/**
- Purpose: Architecture and structure documentation (generated by GSD)
- Generated: By `/gsd:map-codebase` command
- Committed: Yes, consumed by other GSD commands

**build/installer/staging/**
- Purpose: Build output staging for installer packaging
- Generated: By build process
- Committed: No (in .gitignore except app/**/bin)

**.vscode/, .agent/, .gsd/, .claude/**, etc.
- Purpose: Tool-specific configuration
- Committed: Selectively (most in .gitignore)

---

*Structure analysis: 2026-03-23*

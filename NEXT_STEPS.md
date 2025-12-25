# Magidesk POS - Execution Plan

## Phase 1: Foundation (Weeks 1-2)

### Week 1: Solution Structure & Domain Model

#### Day 1-2: Solution Setup
- [ ] Create .NET solution with proper structure
  - [ ] Magidesk.Domain project
  - [ ] Magidesk.Application project
  - [ ] Magidesk.Infrastructure project
  - [ ] Magidesk.Presentation project (WinUI 3)
  - [ ] Test projects (Domain.Tests, Application.Tests)
- [ ] Configure project references (dependencies point inward)
- [ ] Set up NuGet packages (EF Core, CommunityToolkit.Mvvm, etc.)
- [ ] Configure solution-wide settings (.editorconfig, etc.)

#### Day 3-4: Domain Model Implementation
- [ ] Implement Money value object
  - [ ] Properties, invariants, operations
  - [ ] Unit tests
- [ ] Implement UserId value object
- [ ] Implement Ticket entity
  - [ ] Properties, constructors
  - [ ] Status transitions
  - [ ] Invariant enforcement
  - [ ] Unit tests
- [ ] Implement OrderLine entity
  - [ ] Properties, constructors
  - [ ] Invariant enforcement
  - [ ] Unit tests
- [ ] Implement Payment entity
  - [ ] Properties, constructors
  - [ ] Payment type handling
  - [ ] Invariant enforcement
  - [ ] Unit tests
- [ ] Implement CashSession entity
  - [ ] Properties, constructors
  - [ ] Invariant enforcement
  - [ ] Unit tests
- [ ] Implement AuditEvent entity
  - [ ] Immutable design
  - [ ] Unit tests

#### Day 5: Domain Services & Events
- [ ] Implement TicketDomainService
  - [ ] CalculateTotals
  - [ ] CanAddPayment
  - [ ] CanCloseTicket
  - [ ] CanVoidTicket
  - [ ] Unit tests
- [ ] Implement PaymentDomainService
  - [ ] CalculateChange
  - [ ] CanRefundPayment
  - [ ] Unit tests
- [ ] Implement CashSessionDomainService
  - [ ] CalculateExpectedCash
  - [ ] CanCloseSession
  - [ ] Unit tests
- [ ] Define domain events (interfaces/base classes)
- [ ] Implement domain exceptions

### Week 2: Application Layer

#### Day 1-2: Application Interfaces & DTOs
- [ ] Define repository interfaces (ITicketRepository, IPaymentRepository, etc.)
- [ ] Define application service interfaces
- [ ] Create DTOs for Presentation layer
  - [ ] TicketDto
  - [ ] OrderLineDto
  - [ ] PaymentDto
  - [ ] CashSessionDto
- [ ] Create mapping profiles (AutoMapper or manual)

#### Day 3-4: Use Cases - Commands
- [ ] Implement OpenCashSessionCommand
  - [ ] Validation
  - [ ] Domain service calls
  - [ ] Repository persistence
  - [ ] Audit event creation
  - [ ] Unit tests
- [ ] Implement CloseCashSessionCommand
  - [ ] Validation
  - [ ] Domain service calls
  - [ ] Repository persistence
  - [ ] Audit event creation
  - [ ] Unit tests
- [ ] Implement CreateTicketCommand
  - [ ] Validation
  - [ ] Domain entity creation
  - [ ] Repository persistence
  - [ ] Audit event creation
  - [ ] Unit tests
- [ ] Implement AddOrderLineCommand
  - [ ] Validation
  - [ ] Domain entity modification
  - [ ] Repository persistence
  - [ ] Audit event creation
  - [ ] Unit tests
- [ ] Implement RemoveOrderLineCommand
- [ ] Implement ModifyOrderLineCommand
- [ ] Implement ProcessPaymentCommand
- [ ] Implement CloseTicketCommand
- [ ] Implement VoidTicketCommand

#### Day 5: Use Cases - Queries
- [ ] Implement GetCurrentCashSessionQuery
- [ ] Implement GetTicketQuery
- [ ] Implement GetMenuItemsQuery
- [ ] Implement GetTicketListQuery (for future use)

## Phase 2: Infrastructure (Week 3)

#### Day 1-2: EF Core Setup
- [ ] Create DbContext
- [ ] Configure PostgreSQL connection (local passwordless server)
- [ ] Configure entity mappings
  - [ ] Ticket configuration
  - [ ] OrderLine configuration
  - [ ] Payment configuration
  - [ ] CashSession configuration
  - [ ] AuditEvent configuration
  - [ ] Discount configurations
  - [ ] Gratuity configuration
- [ ] Configure value object mappings (Money)
- [ ] Set up optimistic concurrency (Version fields)
- [ ] Create database migrations
- [ ] Seed initial data (MenuItems, OrderTypes, Shifts)

#### Day 3-4: Repository Implementations
- [ ] Implement TicketRepository
  - [ ] Add, Update, GetById, GetByNumber
  - [ ] Integration tests
- [ ] Implement PaymentRepository
- [ ] Implement CashSessionRepository
- [ ] Implement AuditEventRepository
- [ ] Implement MenuItemRepository (read-only for MVP)

#### Day 5: Infrastructure Services
- [ ] Implement audit service (creates AuditEvents)
- [ ] Implement date/time service (for testability)
- [ ] Set up dependency injection container
- [ ] Integration tests for repositories

## Phase 3: Presentation Layer (Weeks 4-6)

#### Week 4: ViewModels & Basic UI
- [ ] Set up WinUI 3 project structure
- [ ] Configure dependency injection in WinUI app
- [ ] Create MainWindow and navigation
- [ ] Implement CashSessionViewModel
  - [ ] Properties, commands
  - [ ] Integration with OpenCashSessionCommand
  - [ ] Integration with CloseCashSessionCommand
- [ ] Create CashSessionView
- [ ] Implement TicketViewModel
  - [ ] Properties, commands
  - [ ] Integration with ticket commands
- [ ] Create TicketView (basic layout)

#### Week 5: Ticket Entry UI
- [ ] Complete TicketView
  - [ ] Menu item grid/list
  - [ ] Order lines display
  - [ ] Totals display
  - [ ] Add/remove/modify item functionality
- [ ] Implement MenuItemViewModel
- [ ] Create MenuItemView/UserControl
- [ ] Implement OrderLineViewModel
- [ ] Create OrderLineView/UserControl
- [ ] Add keyboard shortcuts
- [ ] Input validation and error display

#### Week 6: Payment & Polish
- [ ] Implement PaymentViewModel
- [ ] Create PaymentView
  - [ ] Payment type selection (Cash only)
  - [ ] Amount input
  - [ ] Change calculation display
- [ ] Complete ticket closing flow
- [ ] Error handling UI
- [ ] Loading states
- [ ] Basic styling/theming
- [ ] End-to-end testing

## Phase 4: Testing & Refinement (Weeks 7-8)

#### Week 7: Comprehensive Testing
- [ ] Domain layer unit tests (target: >90% coverage)
- [ ] Application layer unit tests (target: >80% coverage)
- [ ] Integration tests for repositories
- [ ] End-to-end tests for critical flows
- [ ] Performance testing (100+ items per ticket)
- [ ] Error scenario testing

#### Week 8: Bug Fixes & Documentation
- [ ] Fix identified bugs
- [ ] Performance optimizations
- [ ] Code review and refactoring
- [ ] Update architecture documentation
- [ ] Create user guide (basic)
- [ ] Prepare for MVP demo

## Immediate Next Steps (Today)

1. **Create Solution Structure**
   ```bash
   # Create solution and projects
   dotnet new sln -n Magidesk
   dotnet new classlib -n Magidesk.Domain -f net8.0
   dotnet new classlib -n Magidesk.Application -f net8.0
   dotnet new classlib -n Magidesk.Infrastructure -f net8.0
   dotnet new winui3 -n Magidesk.Presentation -f net8.0
   dotnet new xunit -n Magidesk.Domain.Tests -f net8.0
   dotnet new xunit -n Magidesk.Application.Tests -f net8.0
   ```

2. **Set Up Project References**
   - Application → Domain
   - Infrastructure → Application, Domain
   - Presentation → Application (only)
   - Tests → respective projects

3. **Add NuGet Packages**
   - Domain: None (pure)
   - Application: FluentValidation, AutoMapper (optional)
   - Infrastructure: Microsoft.EntityFrameworkCore, Microsoft.EntityFrameworkCore.Sqlite
   - Presentation: CommunityToolkit.Mvvm, Microsoft.Extensions.DependencyInjection
   - Tests: FluentAssertions, Moq, xunit

4. **Create First Domain Entity**
   - Start with Money value object (simplest)
   - Then Ticket entity
   - Build incrementally

## Success Metrics

### Code Quality
- [ ] All invariants have unit tests
- [ ] Domain layer has >90% test coverage
- [ ] Application layer has >80% test coverage
- [ ] Zero business logic in Presentation layer
- [ ] All dependencies point inward

### Functionality
- [ ] Can complete full transaction flow
- [ ] All invariants enforced
- [ ] All mutations create audit events
- [ ] Works offline
- [ ] Handles errors gracefully

### Performance
- [ ] UI operations <100ms
- [ ] Can handle 100+ items per ticket
- [ ] Database operations efficient

## Risk Mitigation

### Risk: Underestimating Complexity
- **Mitigation**: Start with simplest entities, build incrementally
- **Checkpoint**: Review after Week 1

### Risk: UI Complexity
- **Mitigation**: Keep UI simple, focus on functionality over polish in MVP
- **Checkpoint**: Review after Week 4

### Risk: Performance Issues
- **Mitigation**: Test with realistic data volumes early
- **Checkpoint**: Performance testing in Week 7

### Risk: Scope Creep
- **Mitigation**: Strictly adhere to MVP scope, defer features
- **Checkpoint**: Weekly scope review

## Communication Plan

- **Daily**: Stand-up notes (what done, what next, blockers)
- **Weekly**: Progress review against plan
- **Milestone**: Demo and feedback session

## Questions to Resolve Before Starting

1. Confirm MVP scope (especially payment types, receipts)
2. Confirm tax calculation approach
3. Confirm menu item structure
4. Confirm Windows version target (Windows 10/11 minimum)
5. Confirm development environment setup


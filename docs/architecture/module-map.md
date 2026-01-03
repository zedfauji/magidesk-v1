# Module Map

## Overview

This document provides a comprehensive mapping of all modules, components, and their relationships within the Magidesk POS system. It serves as a navigation guide for understanding the system's structure and dependencies.

## Module Organization

### High-Level Module Structure

```
Magidesk POS System
├── Presentation Layer
│   ├── Views (WinUI 3)
│   ├── ViewModels (MVVM)
│   └── Navigation Services
├── Application Layer
│   ├── Commands (CQRS)
│   ├── Queries (CQRS)
│   ├── DTOs (Data Transfer)
│   └── Application Services
├── Domain Layer
│   ├── Entities (Business Objects)
│   ├── Value Objects (Immutable)
│   ├── Domain Services (Business Logic)
│   └── Domain Events (State Changes)
└── Infrastructure Layer
    ├── Repositories (Data Access)
    ├── External Services (Integrations)
    └── Database (PostgreSQL)
```

## Presentation Layer Modules

### Views Module

#### Purpose
Provide visual interface for user interaction using WinUI 3 framework.

#### Components

**Main Views**
- `LoginPage.xaml` - User authentication interface
- `MainPage.xaml` - Main application shell
- `SwitchboardPage.xaml` - Primary navigation hub
- `OrderEntryPage.xaml` - Order taking interface

**Management Views**
- `ManagerFunctionsDialog.xaml` - Administrative functions
- `UserManagementPage.xaml` - Staff management
- `TableMapPage.xaml` - Table layout management
- `MenuEditorPage.xaml` - Menu item management

**Transaction Views**
- `PaymentPage.xaml` - Payment processing interface
- `SettlePage.xaml` - Ticket settlement
- `VoidTicketDialog.xaml` - Ticket voiding
- `SplitTicketDialog.xaml` - Order splitting

**Reporting Views**
- `SalesReportsPage.xaml` - Sales analytics
- `KitchenDisplayPage.xaml` - Kitchen order display
- `DrawerPullReportDialog.xaml` - Cash drawer reports

**Dialog Views**
- `TableSelectionDialog.xaml` - Table selection
- `ModifierSelectionDialog.xaml` - Order customization
- `QuantityDialog.xaml` - Quantity entry
- `NotesDialog.xaml` - Special instructions

#### Dependencies
- WinUI 3 framework
- CommunityToolkit.Mvvm
- Application layer DTOs
- System resources and styles

#### Key Characteristics
- **XAML-based**: Declarative UI definition
- **Data Binding**: Automatic UI updates
- **Event Routing**: User input handling
- **Responsive Design**: Adaptive to different screen sizes

### ViewModels Module

#### Purpose
Coordinate between Views and Application layer, manage UI state, and handle user interactions.

#### Components

**Core ViewModels**
- `LoginViewModel.cs` - Authentication coordination
- `MainPageViewModel.cs` - Application shell state
- `SwitchboardViewModel.cs` - Navigation management
- `OrderEntryViewModel.cs` - Order taking logic

**Management ViewModels**
- `ManagerFunctionsViewModel.cs` - Administrative operations
- `UserManagementViewModel.cs` - Staff management coordination
- `TableMapViewModel.cs` - Table layout state
- `MenuEditorViewModel.cs` - Menu editing coordination

**Transaction ViewModels**
- `PaymentViewModel.cs` - Payment processing coordination
- `SettleViewModel.cs` - Settlement logic
- `VoidTicketViewModel.cs` - Void operation coordination
- `SplitTicketViewModel.cs` - Order splitting logic

**Reporting ViewModels**
- `SalesReportsViewModel.cs` - Report data management
- `KitchenDisplayViewModel.cs` - Kitchen order coordination
- `DrawerPullReportViewModel.cs` - Cash report generation

**Dialog ViewModels**
- `TableSelectionViewModel.cs` - Table selection logic
- `ModifierSelectionViewModel.cs` - Modifier management
- `QuantityViewModel.cs` - Quantity validation
- `NotesDialogViewModel.cs` - Note handling

#### Dependencies
- Application layer commands and queries
- CommunityToolkit.Mvvm base classes
- System.ComponentModel for data binding
- Navigation services

#### Key Characteristics
- **Observable Properties**: Support data binding
- **Command Properties**: Handle user actions
- **State Management**: Maintain UI state
- **Validation**: Input validation and error handling

### Navigation Services Module

#### Purpose
Manage application navigation, dialog display, and window lifecycle.

#### Components
- `NavigationService.cs` - Screen navigation
- `DialogService.cs` - Dialog management
- `WindowService.cs` - Window lifecycle
- `DeepLinkService.cs` - Deep linking support

#### Dependencies
- WinUI 3 navigation framework
- ViewModels for navigation targets
- Application layer for navigation decisions

## Application Layer Modules

### Commands Module (CQRS Write Model)

#### Purpose
Handle write operations and state changes through the Command pattern.

#### Components

**Ticket Commands**
- `CreateTicketCommand.cs` - Initialize new ticket
- `AddOrderLineCommand.cs` - Add items to ticket
- `RemoveOrderLineCommand.cs` - Remove items from ticket
- `ModifyOrderLineCommand.cs` - Change order line
- `CloseTicketCommand.cs` - Finalize ticket
- `VoidTicketCommand.cs` - Cancel ticket
- `TransferTicketCommand.cs` - Change ticket ownership
- `SplitTicketCommand.cs` - Divide ticket
- `MergeTicketsCommand.cs` - Combine tickets

**Payment Commands**
- `ProcessPaymentCommand.cs` - Add payment to ticket
- `RefundPaymentCommand.cs` - Process refund
- `AuthorizeCardPaymentCommand.cs` - Authorize card payment
- `CaptureCardPaymentCommand.cs` - Capture authorized payment
- `VoidCardPaymentCommand.cs` - Void card payment
- `AddTipsToCardPaymentCommand.cs` - Add tips to card payment

**Management Commands**
- `CreateUserCommand.cs` - Create staff account
- `UpdateUserCommand.cs` - Modify staff account
- `DeleteUserCommand.cs` - Remove staff account
- `CreateShiftCommand.cs` - Define work shift
- `UpdateShiftCommand.cs` - Modify shift details
- `CreateTableCommand.cs` - Create dining table
- `UpdateTableCommand.cs` - Modify table details

**Cash Management Commands**
- `OpenCashSessionCommand.cs` - Start cash drawer
- `CloseCashSessionCommand.cs` - End cash drawer
- `AddCashDropCommand.cs` - Remove cash for security
- `AddDrawerBleedCommand.cs` - Remove cash for change
- `AddPayoutCommand.cs` - Disburse cash for expenses

**System Commands**
- `UpdateRestaurantConfigCommand.cs` - Modify restaurant settings
- `UpdateTerminalConfigCommand.cs` - Modify terminal settings
- `CreateSystemBackupCommand.cs` - Create system backup
- `RestoreSystemBackupCommand.cs` - Restore from backup

#### Dependencies
- Domain entities and services
- Repository interfaces
- Validation frameworks
- Event publishers

#### Key Characteristics
- **Immutable**: Commands are immutable once created
- **Validated**: Input validation before processing
- **Transactional**: Database transaction management
- **Event Publishing**: Domain events for state changes

### Queries Module (CQRS Read Model)

#### Purpose
Handle read operations and data retrieval through the Query pattern.

#### Components

**Ticket Queries**
- `GetTicketQuery.cs` - Retrieve specific ticket
- `GetTicketByNumberQuery.cs` - Find ticket by number
- `GetOpenTicketsQuery.cs` - List open tickets
- `GetTicketHistoryQuery.cs` - Ticket modification history

**Menu Queries**
- `GetMenuItemsQuery.cs` - Retrieve menu items
- `GetMenuCategoriesQuery.cs` - List menu categories
- `GetAvailableModifiersQuery.cs` - Get item modifiers
- `GetMenuItemQuery.cs` - Specific menu item details

**User Queries**
- `GetUsersQuery.cs` - List staff members
- `GetCurrentUserQuery.cs` - Current user details
- `GetUserPermissionsQuery.cs` - User permissions
- `GetActiveUsersQuery.cs` - Currently active staff

**Reporting Queries**
- `GetSalesSummaryQuery.cs` - Sales summary data
- `GetSalesDetailQuery.cs` - Detailed sales data
- `GetLaborReportQuery.cs` - Labor cost analysis
- `GetInventoryReportQuery.cs` - Inventory status
- `GetDrawerPullReportQuery.cs` - Cash drawer report

**System Queries**
- `GetRestaurantConfigQuery.cs` - Restaurant settings
- `GetTerminalConfigQuery.cs` - Terminal configuration
- `GetShiftQuery.cs` - Shift details
- `GetCashSessionQuery.cs` - Cash session status

#### Dependencies
- Repository interfaces
- Database contexts
- Mapping services
- Caching services

#### Key Characteristics
- **Read-Only**: Queries do not modify state
- **Optimized**: Tailored for specific read scenarios
- **Cached**: Frequently accessed data cached
- **Projected**: Return DTOs, not domain entities

### DTOs Module

#### Purpose
Provide data transfer objects for communication between layers.

#### Components

**Ticket DTOs**
- `TicketDto.cs` - Ticket summary data
- `OrderLineDto.cs` - Order line details
- `TicketDiscountDto.cs` - Discount information
- `TicketStatusDto.cs` - Status information

**Payment DTOs**
- `PaymentDto.cs` - Payment details
- `PaymentBatchDto.cs` - Batch payment data
- `GratuityDto.cs` - Gratuity information
- `CashSessionDto.cs` - Cash session summary

**Menu DTOs**
- `MenuItemDto.cs` - Menu item data
- `MenuCategoryDto.cs` - Category information
- `MenuGroupDto.cs` - Group organization
- `OrderLineModifierDto.cs` - Modifier details

**User DTOs**
- `UserDto.cs` - User profile data
- `RoleDto.cs` - Role information
- `PermissionDto.cs` - Permission details
- `ShiftDto.cs` - Shift information

**Reporting DTOs**
- `SalesSummaryDto.cs` - Sales summary
- `SalesDetailDto.cs` - Detailed sales data
- `LaborReportDto.cs` - Labor analysis
- `InventoryReportDto.cs` - Inventory status
- `AttendanceReportDto.cs` - Staff attendance

#### Dependencies
- Domain entities (for mapping)
- System libraries for serialization
- Validation attributes

#### Key Characteristics
- **Data Containers**: No business logic
- **Serializable**: Support JSON/XML serialization
- **Validatable**: Input validation attributes
- **Immutable**: Read-only where appropriate

### Application Services Module

#### Purpose
Provide coordination services for complex operations and cross-cutting concerns.

#### Components
- `EventPublisher.cs` - Domain event publishing
- `ValidationService.cs` - Cross-cutting validation
- `MappingService.cs` - Entity-DTO mapping
- `CachingService.cs` - Application-level caching
- `LoggingService.cs` - Application logging
- `SecurityService.cs` - Security coordination

#### Dependencies
- Domain events and handlers
- Validation frameworks
- Mapping libraries
- Caching frameworks
- Logging frameworks

## Domain Layer Modules

### Entities Module

#### Purpose
Define core business entities with rich behavior and invariant enforcement.

#### Components

**Core Entities (Aggregate Roots)**
- `Ticket.cs` - Order management aggregate
- `CashSession.cs` - Cash management aggregate
- `User.cs` - Staff management entity
- `Shift.cs` - Work period entity

**Menu Entities**
- `MenuItem.cs` - Menu item definition
- `MenuCategory.cs` - Menu organization
- `MenuGroup.cs` - Item grouping
- `ModifierGroup.cs` - Modifier organization

**Operational Entities**
- `Table.cs` - Dining table entity
- `OrderLine.cs` - Order line item
- `Payment.cs` - Payment transaction
- `KitchenOrder.cs` - Kitchen order entity

**Configuration Entities**
- `RestaurantConfiguration.cs` - Restaurant settings
- `Terminal.cs` - POS terminal entity
- `PrinterGroup.cs` - Printer organization
- `OrderType.cs` - Order type definition

#### Dependencies
- Value objects for complex attributes
- Domain services for complex logic
- Domain events for state changes
- Exceptions for error handling

#### Key Characteristics
- **Rich Behavior**: Contain business logic
- **Invariant Enforcement**: Protect business rules
- **Aggregate Roots**: Control consistency boundaries
- **Event Publishing**: Notify state changes

### Value Objects Module

#### Purpose
Define immutable value objects that represent domain concepts.

#### Components
- `Money.cs` - Monetary amounts with currency
- `UserId.cs` - User identification
- `TaxRate.cs` - Tax calculation parameters
- `Address.cs` - Location information
- `RecipeLine.cs` - Recipe ingredient line
- `TaxGroup.cs` - Tax grouping for calculations

#### Dependencies
- System libraries for validation
- Domain exceptions for errors

#### Key Characteristics
- **Immutable**: Cannot be modified after creation
- **Value Equality**: Equality based on values, not identity
- **Validation**: Ensure valid state on creation
- **Self-Validation**: Contain validation logic

### Domain Services Module

#### Purpose
Provide complex business logic that doesn't naturally fit in a single entity.

#### Components
- `TicketDomainService.cs` - Complex ticket operations
- `PaymentDomainService.cs` - Payment processing logic
- `DiscountDomainService.cs` - Discount calculations
- `TaxDomainService.cs` - Tax calculations
- `ServiceChargeDomainService.cs` - Service charge logic
- `CashSessionDomainService.cs` - Cash session operations

#### Dependencies
- Domain entities for operations
- Value objects for calculations
- Other domain services for coordination

#### Key Characteristics
- **Stateless**: No persistent state
- **Business Logic**: Complex business rules
- **Coordination**: Orchestrate multiple entities
- **Calculation**: Complex mathematical operations

### Domain Events Module

#### Purpose
Define events that represent important business state changes.

#### Components
- `TicketCreatedEvent.cs` - New ticket created
- `PaymentProcessedEvent.cs` - Payment completed
- `OrderModifiedEvent.cs` - Order changed
- `TicketClosedEvent.cs` - Ticket finalized
- `CashSessionOpenedEvent.cs` - Cash drawer opened
- `CashSessionClosedEvent.cs` - Cash drawer closed

#### Dependencies
- Domain entities that publish events
- Event handlers that process events

#### Key Characteristics
- **Immutable**: Event data cannot change
- **Serializable**: Support persistence and transmission
- **Timestamped**: Include occurrence time
- **Contextual**: Include relevant context information

## Infrastructure Layer Modules

### Repositories Module

#### Purpose
Implement data access patterns for domain entities.

#### Components

**Core Repositories**
- `TicketRepository.cs` - Ticket data access
- `CashSessionRepository.cs` - Cash session data access
- `UserRepository.cs` - User data access
- `ShiftRepository.cs` - Shift data access

**Menu Repositories**
- `MenuRepository.cs` - Menu data access
- `MenuCategoryRepository.cs` - Category data access
- `MenuGroupRepository.cs` - Group data access
- `ModifierGroupRepository.cs` - Modifier data access

**Operational Repositories**
- `TableRepository.cs` - Table data access
- `PaymentRepository.cs` - Payment data access
- `KitchenOrderRepository.cs` - Kitchen order data access
- `OrderTypeRepository.cs` - Order type data access

**Reporting Repositories**
- `SalesReportRepository.cs` - Sales data aggregation
- `LaborReportRepository.cs` - Labor data aggregation
- `InventoryRepository.cs` - Inventory data access
- `AuditEventRepository.cs` - Audit trail access

#### Dependencies
- Entity Framework Core
- PostgreSQL database
- Domain entities
- Application interfaces

#### Key Characteristics
- **Interface Implementation**: Implement repository interfaces
- **Entity Framework**: Use EF Core for data access
- **Optimization**: Query optimization and caching
- **Transaction Management**: Database transaction coordination

### External Services Module

#### Purpose
Implement integrations with external systems and services.

#### Components

**Payment Services**
- `MockPaymentGateway.cs` - Payment processing simulation
- `CreditCardProcessor.cs` - Credit card handling
- `GiftCertificateService.cs` - Gift certificate management

**Communication Services**
- `EmailService.cs` - Email notifications
- `SMSService.cs` - Text message notifications
- `PushNotificationService.cs` - Mobile notifications

**Printing Services**
- `ReceiptPrintService.cs` - Receipt printing
- `KitchenPrintService.cs` - Kitchen printing
- `ReportPrintService.cs` - Report printing

**Backup Services**
- `PostgresBackupService.cs` - Database backup
- `FileBackupService.cs` - File system backup
- `CloudBackupService.cs` - Cloud storage backup

#### Dependencies
- External service APIs
- HTTP clients
- File system access
- Configuration settings

#### Key Characteristics
- **Interface Implementation**: Implement service interfaces
- **Error Handling**: Robust error handling and retry logic
- **Configuration**: External service configuration
- **Logging**: Comprehensive operation logging

### Database Module

#### Purpose
Manage database schema, connections, and migrations.

#### Components

**Database Context**
- `ApplicationDbContext.cs` - EF Core database context
- `DbContextFactory.cs` - Context factory pattern
- `DatabaseConnection.cs` - Connection management

**Configurations**
- Entity configurations for all domain entities
- Relationship mappings
- Index definitions
- Constraint definitions

**Migrations**
- Database schema evolution
- Migration scripts
- Seed data management
- Version tracking

#### Dependencies
- Entity Framework Core
- PostgreSQL provider
- Domain entities
- Configuration settings

#### Key Characteristics
- **Code-First**: Schema defined in code
- **Migrations**: Automatic schema evolution
- **Optimization**: Performance-tuned configurations
- **Versioning**: Tracked schema versions

## Cross-Cutting Concerns

### Security Module

#### Purpose
Provide authentication, authorization, and data protection.

#### Components
- `AuthenticationService.cs` - User authentication
- `AuthorizationService.cs` - Permission checking
- `EncryptionService.cs` - Data encryption
- `AuditService.cs` - Security auditing

### Logging Module

#### Purpose
Provide comprehensive logging and monitoring.

#### Components
- `LoggingService.cs` - Application logging
- `PerformanceLogger.cs` - Performance monitoring
- `ErrorLogger.cs` - Error tracking
- `AuditLogger.cs` - Audit trail logging

### Configuration Module

#### Purpose
Manage application configuration and settings.

#### Components
- `ConfigurationService.cs` - Settings management
- `FeatureFlagService.cs` - Feature toggles
- `EnvironmentService.cs` - Environment detection
- `SecretService.cs` - Secret management

### Validation Module

#### Purpose
Provide input validation and business rule enforcement.

#### Components
- `ValidationService.cs` - Cross-cutting validation
- `BusinessRuleValidator.cs` - Business rule validation
- `InputSanitizer.cs` - Input sanitization
- `ModelValidator.cs` - Model validation

## Module Dependencies

### Dependency Graph

```
Presentation Layer
├── Views
│   ├── Depends on: ViewModels
│   └── Depends on: System Resources
├── ViewModels
│   ├── Depends on: Application Layer
│   └── Depends on: Navigation Services
└── Navigation Services
    ├── Depends on: ViewModels
    └── Depends on: Application Layer

Application Layer
├── Commands
│   ├── Depends on: Domain Layer
│   └── Depends on: Infrastructure Interfaces
├── Queries
│   ├── Depends on: Infrastructure Interfaces
│   └── Depends on: DTOs
├── DTOs
│   └── Depends on: Domain Layer (for mapping)
└── Application Services
    ├── Depends on: Domain Layer
    └── Depends on: Infrastructure Interfaces

Domain Layer
├── Entities
│   ├── Depends on: Value Objects
│   ├── Depends on: Domain Services
│   └── Depends on: Domain Events
├── Value Objects
│   └── Self-contained
├── Domain Services
│   ├── Depends on: Entities
│   └── Depends on: Value Objects
└── Domain Events
    └── Self-contained

Infrastructure Layer
├── Repositories
│   ├── Depends on: Domain Layer
│   └── Depends on: Database Module
├── External Services
│   └── Implements Application Interfaces
└── Database Module
    ├── Depends on: Domain Layer
    └── Depends on: Configuration
```

### Dependency Rules

#### Allowed Dependencies
1. **Presentation → Application**: UI can call application services
2. **Application → Domain**: Use cases can use domain logic
3. **Infrastructure → Application**: Data access implements application interfaces
4. **Infrastructure → Domain**: Repositories work with domain entities

#### Prohibited Dependencies
1. **Domain → Infrastructure**: Domain must not depend on technical details
2. **Domain → Application**: Domain must not depend on use cases
3. **Application → Presentation**: Application must not depend on UI
4. **Presentation → Domain**: UI must not directly access domain logic

## Module Communication Patterns

### Synchronous Communication

#### Command Pattern
```
ViewModel → Command → Handler → Domain → Repository → Database
```

#### Query Pattern
```
ViewModel → Query → Handler → Repository → Database → DTO → ViewModel
```

### Asynchronous Communication

#### Event Pattern
```
Domain Entity → Domain Event → Event Publisher → Event Handlers → External Services
```

#### Service Integration
```
Application Service → External Service → Async Response → Callback → Application Service
```

## Module Lifecycle

### Initialization Order

1. **Configuration**: Load application settings
2. **Database**: Initialize database connections
3. **Infrastructure**: Set up repositories and external services
4. **Application**: Initialize application services
5. **Domain**: Register domain events and services
6. **Presentation**: Initialize UI and navigation

### Shutdown Order

1. **Presentation**: Close UI and save user state
2. **Application**: Complete in-flight operations
3. **Domain**: Flush domain events
4. **Infrastructure**: Close database connections
5. **Database**: Ensure data consistency
6. **Configuration**: Save settings

## Module Testing Strategy

### Unit Testing

#### Domain Layer
- **Entity Tests**: Business logic and invariants
- **Value Object Tests**: Validation and equality
- **Domain Service Tests**: Complex business operations
- **Domain Event Tests**: Event creation and handling

#### Application Layer
- **Command Tests**: Use case validation and execution
- **Query Tests**: Data retrieval and mapping
- **DTO Tests**: Serialization and validation
- **Service Tests**: Coordination logic

#### Infrastructure Layer
- **Repository Tests**: Data access and mapping
- **Service Tests**: External integration
- **Database Tests**: Schema and migrations

### Integration Testing

#### Database Integration
- Repository implementations with real database
- Migration testing
- Performance testing
- Concurrency testing

#### External Service Integration
- Payment gateway integration
- Email service integration
- Print service integration
- Backup service integration

## Module Evolution

### Planned Enhancements

#### Presentation Layer
- **Mobile Views**: Touch-optimized interfaces
- **Accessibility**: Enhanced accessibility features
- **Themes**: Customizable UI themes
- **Internationalization**: Multi-language support

#### Application Layer
- **API Layer**: RESTful API for external access
- **Background Services**: Asynchronous processing
- **Caching**: Advanced caching strategies
- **Performance**: Query optimization

#### Domain Layer
- **Business Rules**: Advanced rule engine
- **Events**: Enhanced event sourcing
- **Validation**: Sophisticated validation framework
- **Calculations**: Complex business calculations

#### Infrastructure Layer
- **Microservices**: Service decomposition
- **Cloud Integration**: Cloud service integration
- **Monitoring**: Advanced monitoring
- **Security**: Enhanced security features

## Conclusion

The Magidesk POS system is organized into well-defined modules with clear responsibilities and dependencies. This modular architecture provides:

- **Maintainability**: Clear module boundaries make the system easier to maintain
- **Testability**: Each module can be tested in isolation
- **Scalability**: Modules can be scaled independently
- **Flexibility**: New features can be added without affecting existing modules
- **Understanding**: Clear organization makes the system easier to understand

The module map serves as a comprehensive guide for developers, architects, and maintainers to understand the system's structure and navigate its components effectively.

---

*This module map provides the complete structural overview of the Magidesk POS system architecture.*
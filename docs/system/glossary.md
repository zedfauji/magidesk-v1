# System Glossary

## Overview

This glossary defines the terminology, concepts, and language used throughout the Magidesk POS system. Understanding these terms is essential for developers, administrators, and users working with the system.

## Core Business Terms

### Ticket
**Definition**: A ticket represents a customer order or transaction from creation to final settlement. It is the central aggregate root in the domain model.

**Context**: The ticket entity contains all order lines, payments, discounts, and metadata related to a single customer transaction. A ticket progresses through a defined lifecycle: Draft → Open → Paid → Closed, with possible transitions to Voided or Refunded states.

**Examples**:
- Table 5's dinner order for 4 guests
- Takeout order #12345
- Delivery order to 123 Main Street

**Related Terms**: Order, Check, Bill, Transaction

### Order Line
**Definition**: An individual line item within a ticket representing a specific menu item, quantity, and associated modifiers.

**Context**: Order lines contain the menu item, quantity, unit price, modifiers, and calculated total. They are the building blocks of tickets and support complex configurations like combos and special instructions.

**Examples**:
- 2x Cheeseburgers with no pickles
- 1x Caesar Salad (large size)
- 3x Draft Beer (16oz) with extra lime

**Related Terms**: Menu Item, Modifier, Order Entry

### Modifier
**Definition**: An addition, removal, or change to a base menu item that affects its preparation, price, or composition.

**Context**: Modifiers can be ingredients, preparation methods, portion sizes, or special requests. They can be free or have additional charges and can be mandatory or optional.

**Examples**:
- Extra cheese (+$1.50)
- No onions (free)
- Well done (free)
- Large size (+$2.00)

**Related Terms**: Modifier Group, Ingredient, Customization

### Payment
**Definition**: A financial transaction that settles part or all of a ticket's balance.

**Context**: Payments can be cash, credit card, gift certificate, or other tender types. Each payment has a status (Pending, Completed, Voided) and is linked to a specific ticket.

**Examples**:
- $50.00 cash payment
- $25.00 credit card charge
- $10.00 gift certificate redemption

**Related Terms**: Tender, Transaction, Settlement

## System Architecture Terms

### Clean Architecture
**Definition**: An architectural pattern that separates concerns into distinct layers with dependency inversion, ensuring business logic remains independent of external concerns.

**Context**: In Magidesk POS, this means the Domain layer contains no external dependencies, the Application layer orchestrates use cases, Infrastructure handles external concerns, and Presentation manages user interface.

**Benefits**: Testability, maintainability, flexibility, and independence of technical concerns.

**Related Terms**: Domain Layer, Application Layer, Infrastructure Layer, Presentation Layer

### Domain Layer
**Definition**: The innermost layer containing business entities, value objects, domain services, and business rules.

**Context**: This layer contains the core business logic and has no dependencies on external frameworks or infrastructure. It defines the "what" of the system without concern for "how" it's implemented.

**Components**: Entities, Value Objects, Domain Services, Domain Events, Enumerations

**Related Terms**: Business Logic, Domain Model, Rich Domain Model

### Application Layer
**Definition**: The layer that contains use cases, application services, and coordinates domain objects to fulfill business requirements.

**Context**: This layer defines the application's use cases through commands and queries, orchestrates domain objects, manages transactions, and defines interfaces for infrastructure implementations.

**Components**: Commands, Queries, DTOs, Application Services, Interfaces

**Related Terms**: Use Cases, CQRS, Application Services

### Infrastructure Layer
**Definition**: The outermost layer that implements technical concerns and external integrations.

**Context**: This layer contains database access, external service integrations, file system operations, and other technical implementations. It depends on the Application and Domain layers but not vice versa.

**Components**: Repositories, External Services, Database, File System

**Related Terms**: Data Access, External Integrations, Technical Infrastructure

### Presentation Layer
**Definition**: The outermost layer responsible for user interface and interaction.

**Context**: This layer contains views, view models, and user interface logic. It depends only on the Application layer and never directly accesses Domain or Infrastructure layers.

**Components**: Views, ViewModels, User Interface, Navigation

**Related Terms**: UI Layer, MVVM, User Interface

## Data and Database Terms

### Entity
**Definition**: An object with identity that represents a core business concept and contains both data and behavior.

**Context**: Entities have a unique identifier, lifecycle, and business rules. They are the primary building blocks of the domain model and are persisted to the database.

**Examples**: Ticket, User, MenuItem, Table

**Related Terms**: Aggregate Root, Value Object, Domain Model

### Value Object
**Definition**: An immutable object that represents a concept without identity, defined by its attributes rather than ID.

**Context**: Value objects are used to represent concepts like Money, Address, or DateRange. They are immutable and can be shared between entities.

**Examples**: Money, UserId, Address, TaxRate

**Related Terms**: Immutable Object, Domain Concept, Data Type

### Aggregate Root
**Definition**: An entity that serves as the entry point to an aggregate and controls access to all objects within the aggregate boundary.

**Context**: Aggregate roots ensure consistency and enforce invariants within their boundary. Only aggregate roots can be directly retrieved from the repository.

**Examples**: Ticket (controls OrderLines, Payments, Discounts), CashSession (controls CashDrops, Payouts)

**Related Terms**: Aggregate, Consistency Boundary, Invariants

### Repository
**Definition**: A design pattern that mediates between the domain and data mapping layers using a collection-like interface for accessing domain objects.

**Context**: Repositories encapsulate data access logic, provide a domain-oriented interface to persistence, and support unit of work patterns.

**Examples**: ITicketRepository, IUserRepository, IMenuItemRepository

**Related Terms**: Data Access, Persistence, Unit of Work

### Migration
**Definition**: A controlled, versioned change to the database schema that evolves the database structure over time.

**Context**: Migrations are automatically generated by Entity Framework Core and applied in sequence to ensure consistent database schemas across environments.

**Examples**: Adding a new column, creating a new table, adding an index

**Related Terms**: Schema Evolution, Database Versioning, EF Core Migrations

## User Interface Terms

### MVVM (Model-View-ViewModel)
**Definition**: A design pattern that separates user interface (View), data and business logic (Model), and presentation logic (ViewModel).

**Context**: In Magidesk POS, Views are XAML files, ViewModels coordinate with Application layer, and Models are DTOs from the Application layer.

**Benefits**: Testability, separation of concerns, designer-developer workflow

**Related Terms**: Data Binding, Command Pattern, Observable Pattern

### View
**Definition**: The visual representation of user interface elements, typically implemented in XAML for WinUI 3.

**Context**: Views contain only visual layout and event handlers, with no business logic. They bind to ViewModels for data and behavior.

**Examples**: OrderEntryPage.xaml, PaymentPage.xaml, LoginPage.xaml

**Related Terms**: XAML, User Interface, Visual Tree

### ViewModel
**Definition**: A class that acts as an intermediary between the View and the Model, containing presentation logic and state.

**Context**: ViewModels expose data for binding, handle user interactions, and coordinate with Application layer services. They contain no business logic.

**Examples**: OrderEntryViewModel, PaymentViewModel, LoginViewModel

**Related Terms**: MVVM, Data Binding, Presentation Logic

### Command
**Definition**: An object that represents an action that can be executed, typically triggered by user interface elements.

**Context**: Commands encapsulate the logic for user actions and support enable/disable states. In MVVM, they bind to UI elements like buttons.

**Examples**: SaveTicketCommand, ProcessPaymentCommand, CancelOrderCommand

**Related Terms**: ICommand, Command Pattern, User Action

## Business Operations Terms

### Shift
**Definition**: A defined work period for staff members, typically with start and end times and associated responsibilities.

**Context**: Shifts organize work schedules, track attendance, and provide context for sales and labor reporting.

**Examples**: Morning shift (8 AM - 4 PM), Evening shift (4 PM - 12 AM)

**Related Terms**: Work Period, Schedule, Staff Management

### Cash Session
**Definition**: A period of cash drawer management from opening to closing, tracking all cash transactions and reconciliations.

**Context**: Cash sessions track cash flow, manage drawer drops and bleeds, and provide accountability for cash handling.

**Examples**: Day shift cash session, Night shift cash session

**Related Terms**: Cash Drawer, Cash Management, Cash Reconciliation

### Cash Drop
**Definition**: The process of removing excess cash from the drawer during a shift for security purposes.

**Context**: Cash drops reduce the amount of cash in the drawer, are documented with amounts and reasons, and maintain cash accountability.

**Examples**: $200 cash drop to safe at 2 PM

**Related Terms**: Cash Management, Drawer Bleed, Cash Security

### Drawer Bleed
**Definition**: The process of removing small amounts of cash from the drawer to make change for customers.

**Context**: Drawer bleeds maintain adequate change availability, are tracked for accountability, and support smooth cash operations.

**Examples**: $20 bleed for quarters and dimes

**Related Terms**: Cash Drop, Change Making, Cash Management

### Payout
**Definition**: A disbursement of cash from the drawer for authorized expenses or reimbursements.

**Context**: Payouts require authorization, are documented with reasons and receipts, and maintain cash accountability.

**Examples**: $50 payout for office supplies, $25 payout for delivery driver gas

**Related Terms**: Cash Disbursement, Expense, Reimbursement

## Payment Processing Terms

### Tender
**Definition**: The form of payment used to settle a transaction.

**Context**: Tender types include cash, credit card, debit card, gift certificate, and other payment methods.

**Examples**: Cash, Credit Card, Gift Certificate

**Related Terms**: Payment Type, Payment Method, Tender Type

### Authorization
**Definition**: The process of verifying that a payment method has sufficient funds or credit available for a transaction.

**Context**: Authorizations reserve funds on credit cards but don't complete the transaction until capture.

**Examples**: Credit card authorization for $50.00

**Related Terms**: Capture, Settlement, Card Processing

### Capture
**Definition**: The process of completing a previously authorized payment transaction and transferring funds.

**Context**: Capture converts an authorization into a completed transaction, initiating the actual fund transfer.

**Examples**: Capturing authorized credit card payment

**Related Terms**: Authorization, Settlement, Payment Completion

### Settlement
**Definition**: The process of batching and submitting captured transactions for actual fund transfer.

**Context**: Settlement typically occurs daily and moves funds from card issuers to merchant accounts.

**Examples**: Daily batch settlement of credit card transactions

**Related Terms**: Batch Processing, Fund Transfer, Merchant Account

### Gratuity
**Definition**: A voluntary additional payment given to service staff, typically calculated as a percentage of the bill.

**Context**: Gratuities can be added by customers, calculated automatically, or distributed among staff according to house policies.

**Examples**: 18% gratuity on $100 bill = $18.00

**Related Terms**: Tip, Service Charge, Staff Compensation

## Kitchen Operations Terms

### Kitchen Display System (KDS)
**Definition**: A digital system that displays orders to kitchen staff in real-time, replacing paper tickets.

**Context**: KDS improves order accuracy, tracks preparation times, and provides real-time order status updates.

**Examples**: Digital screen showing order queue and status

**Related Terms**: Kitchen Display, Order Board, Digital Tickets

### Bumping
**Definition**: The process of marking a kitchen order item as completed and removing it from the active order queue.

**Context**: Bumping indicates that an item is ready for service and updates the order status in the system.

**Examples**: Bumping completed burger from kitchen display

**Related Terms**: Order Completion, Item Ready, Kitchen Status

### Expeditor
**Definition**: A staff member responsible for coordinating order completion, quality checking, and organizing orders for service.

**Context**: Expeditors ensure order accuracy, manage timing, and serve as the final checkpoint before orders reach customers.

**Examples**: Expo station staff coordinating multiple orders

**Related Terms**: Food Runner, Order Coordinator, Quality Control

### Fire
**Definition**: The action of starting preparation on a kitchen order, especially for items that need precise timing.

**Context**: Firing an order begins the cooking process and is typically done when the order is ready to be prepared.

**Examples**: Firing the steaks for table 12's order

**Related Terms**: Start Cooking, Order Initiation, Kitchen Timing

## Reporting and Analytics Terms

### Sales Mix
**Definition**: The proportion of different menu items or categories in total sales volume.

**Context**: Sales mix analysis helps identify popular items, optimize menu pricing, and manage inventory.

**Examples**: 30% burgers, 20% appetizers, 15% beverages

**Related Terms**: Menu Analysis, Item Performance, Sales Breakdown

### Labor Cost Percentage
**Definition**: The ratio of labor costs to total sales, expressed as a percentage.

**Context**: This key metric helps evaluate staffing efficiency and profitability.

**Examples**: Labor cost of $500 on $2000 sales = 25% labor cost

**Related Terms**: Labor Efficiency, Staff Cost, Profitability Metric

### Table Turnover
**Definition**: The number of times a table is occupied by different customers during a service period.

**Context**: Higher table turnover indicates better efficiency and revenue generation per table.

**Examples**: 3 table turns per dinner service

**Related Terms**: Table Efficiency, Revenue per Table, Seating Capacity

### Check Average
**Definition**: The average amount spent per customer or per ticket.

**Context**: Check average is a key performance indicator for business health and pricing strategy.

**Examples**: $45 average check per customer

**Related Terms**: Average Ticket Size, Revenue per Customer, Spend per Guest

## Technical Implementation Terms

### CQRS (Command Query Responsibility Segregation)
**Definition**: A pattern that separates read (query) and write (command) operations into different models.

**Context**: In Magidesk POS, Commands modify state (CreateTicket, AddPayment) while Queries read state (GetTicket, GetOpenTickets).

**Benefits**: Optimized read/write models, clear separation of concerns, better scalability

**Related Terms**: Command Pattern, Query Pattern, Read Model, Write Model

### Domain Event
**Definition**: An event that represents something that happened in the domain that domain experts care about.

**Context**: Domain events are published when important business events occur, allowing loose coupling and eventual consistency.

**Examples**: TicketCreated, PaymentProcessed, OrderModified

**Related Terms**: Event Sourcing, Event Handler, Loose Coupling

### Invariant
**Definition**: A rule that must always be true for a domain object or aggregate.

**Context**: Invariants ensure data integrity and business rule compliance. They are enforced in domain entities.

**Examples**: Ticket total cannot be negative, Payment amount cannot exceed ticket balance

**Related Terms**: Business Rule, Data Integrity, Domain Constraint

### Concurrency
**Definition**: The handling of simultaneous operations on the same data by multiple users or processes.

**Context**: Concurrency is managed through optimistic concurrency control using version numbers to prevent lost updates.

**Examples**: Two users modifying the same ticket simultaneously

**Related Terms**: Optimistic Concurrency, Version Control, Lost Update

### Dependency Injection
**Definition**: A technique for achieving Inversion of Control between classes and their dependencies.

**Context**: DI container manages object creation and lifetime, enabling loose coupling and improved testability.

**Examples**: Injecting ITicketRepository into TicketService

**Related Terms**: IoC Container, Service Locator, Loose Coupling

## Security and Compliance Terms

### PCI DSS (Payment Card Industry Data Security Standard)
**Definition**: A set of security standards designed to ensure that all companies that process, store, or transmit credit card information maintain a secure environment.

**Context**: Magidesk POS complies with PCI DSS requirements for secure card processing and data protection.

**Requirements**: Encryption, access control, network security, vulnerability management

**Related Terms**: Payment Security, Data Protection, Compliance

### Audit Trail
**Definition**: A chronological record of system activities that provides evidence of the sequence of events.

**Context**: Audit trails track all modifications to critical data, support forensic analysis, and ensure accountability.

**Examples**: Log of all ticket modifications, payment changes, user access

**Related Terms**: Audit Log, Activity Tracking, Compliance

### Role-Based Access Control (RBAC)
**Definition**: An approach to restricting system access to authorized users based on their roles within an organization.

**Context**: RBAC in Magidesk POS ensures users can only access features and data appropriate to their job function.

**Examples**: Servers can take orders but not access financial reports

**Related Terms**: Permissions, Access Control, User Roles

### Encryption
**Definition**: The process of converting information or data into a code to prevent unauthorized access.

**Context**: Sensitive data like payment information and user credentials are encrypted both at rest and in transit.

**Examples**: Credit card numbers encrypted in database, HTTPS for network communication

**Related Terms**: Cryptography, Data Protection, Security

## Development and Testing Terms

### Unit Test
**Definition**: A test that verifies the behavior of a single unit of code in isolation.

**Context**: Unit tests in Magidesk POS test domain entities, application services, and other individual components.

**Examples**: Testing Ticket.CalculateTotals() method

**Related Terms**: Test Coverage, Mock Object, Test Isolation

### Integration Test
**Definition**: A test that verifies the interaction between multiple components or systems.

**Context**: Integration tests verify database operations, external service calls, and component interactions.

**Examples**: Testing repository with actual database

**Related Terms**: End-to-End Test, System Test, Component Test

### Test Coverage
**Definition**: A measure of how much of the code is exercised by automated tests.

**Context**: Magidesk POS targets >90% coverage for Domain layer and >80% for Application layer.

**Examples**: 95% line coverage for domain entities

**Related Terms**: Code Coverage, Test Metrics, Quality Assurance

### Refactoring
**Definition**: The process of restructuring existing code without changing its external behavior.

**Context**: Refactoring improves code quality, maintainability, and performance while preserving functionality.

**Examples**: Extracting method, renaming variable, simplifying conditional

**Related Terms**: Code Quality, Technical Debt, Code Maintenance

## Acronyms and Abbreviations

| Acronym | Full Term | Context |
|---------|------------|---------|
| POS | Point of Sale | System type and industry |
| MVVM | Model-View-ViewModel | UI architecture pattern |
| CQRS | Command Query Responsibility Segregation | Application architecture pattern |
| DTO | Data Transfer Object | Data structure for layer communication |
| CRUD | Create, Read, Update, Delete | Basic data operations |
| API | Application Programming Interface | Service interface |
| UI | User Interface | Visual interaction layer |
| UX | User Experience | Overall user interaction design |
| KDS | Kitchen Display System | Kitchen operations technology |
| PCI | Payment Card Industry | Payment security standards |
| DSS | Data Security Standard | Compliance requirements |
| RBAC | Role-Based Access Control | Security permission model |
| DI | Dependency Injection | Design pattern for loose coupling |
| IoC | Inversion of Control | Design principle for dependency management |
| EF | Entity Framework | Object-relational mapping framework |
| ORM | Object-Relational Mapping | Database access technique |
| SQL | Structured Query Language | Database query language |
| JSON | JavaScript Object Notation | Data interchange format |
| XML | Extensible Markup Language | Data markup format |
| HTTP | Hypertext Transfer Protocol | Network communication protocol |
| HTTPS | HTTP Secure | Encrypted network communication |
| TLS | Transport Layer Security | Encryption protocol |
| CPU | Central Processing Unit | Computer processor |
| RAM | Random Access Memory | Computer memory |
| SSD | Solid State Drive | Storage device |
| LAN | Local Area Network | Local computer network |
| WAN | Wide Area Network | Broad network connection |
| VPN | Virtual Private Network | Secure network connection |
| SaaS | Software as a Service | Software delivery model |
| MVP | Minimum Viable Product | Product development strategy |
| KPI | Key Performance Indicator | Business metric |
| ROI | Return on Investment | Financial metric |
| TCO | Total Cost of Ownership | Economic metric |

## Conclusion

This glossary provides a comprehensive reference for the terminology used throughout the Magidesk POS system. Understanding these terms is essential for effective communication, development, and operation of the system.

The glossary will be updated as the system evolves and new concepts are introduced. Users and developers are encouraged to reference this document when encountering unfamiliar terminology.

---

*This glossary serves as the authoritative reference for all terminology used in the Magidesk POS system.*
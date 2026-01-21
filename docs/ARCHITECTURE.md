# Magidesk POS - Architecture Documentation

## Overview
Magidesk is a ground-up rebuild of a Windows-based Point of Sale system using modern .NET technologies and Clean Architecture principles. This system prioritizes correctness, auditability, and maintainability over convenience.

## Architecture Principles

### Core Tenets
1. **Domain-First**: All business logic lives in the Domain layer
2. **Audit-First**: Every financial mutation is immutable and auditable
3. **Offline-Capable**: System must function without network connectivity
4. **Strict MVVM**: UI contains zero business logic
5. **Clean Architecture**: Dependencies point inward toward Domain

### Layer Responsibilities

#### Domain Layer (Core)
- **Purpose**: Pure business logic, no dependencies on infrastructure
- **Contains**: Entities, Value Objects, Domain Events, Domain Services, Invariants
- **Rules**: 
  - No external dependencies (no EF, no HTTP, no file I/O)
  - Language-agnostic business rules
  - Immutable financial entities where possible

#### Application Layer
- **Purpose**: Orchestrates domain logic, defines use cases
- **Contains**: Use Cases (Commands/Queries), Application Services, DTOs
- **Rules**:
  - Depends only on Domain
  - Coordinates domain objects and services
  - Handles transaction boundaries
  - Validates application-level concerns

#### Infrastructure Layer
- **Purpose**: Technical implementations
- **Contains**: 
  - Data Persistence (EF Core, repositories)
  - External Services (payment processors, printers)
  - File System Access
  - Network Communication
- **Rules**:
  - Implements interfaces defined in Application/Domain
  - No business logic

#### Presentation Layer (WinUI 3)
- **Purpose**: User interface only
- **Contains**: Views, ViewModels, Converters, UI Helpers
- **Rules**:
  - Zero business logic
  - ViewModels delegate to Application layer
  - No direct database access
  - No domain entity exposure to UI

## Solution Structure

```
Magidesk/
├── Magidesk.Domain/              # Core business logic
│   ├── Entities/
│   ├── ValueObjects/
│   ├── DomainEvents/
│   ├── Services/
│   └── Exceptions/
├── Magidesk.Application/         # Use cases and orchestration
│   ├── Commands/
│   ├── Queries/
│   ├── DTOs/
│   ├── Interfaces/
│   └── Services/
├── Magidesk.Infrastructure/      # Technical implementations
│   ├── Persistence/
│   │   ├── Configurations/
│   │   ├── Repositories/
│   │   └── DbContext/
│   ├── Services/
│   └── External/
├── Magidesk.Presentation/        # WinUI 3 UI
│   ├── Views/
│   ├── ViewModels/
│   ├── Converters/
│   └── Resources/
└── Magidesk.Tests/               # Test projects
    ├── Domain.Tests/
    ├── Application.Tests/
    └── Integration.Tests/
```

## Technology Stack

- **UI Framework**: WinUI 3
- **MVVM Framework**: CommunityToolkit.Mvvm
- **ORM**: EF Core (Infrastructure only)
- **Database**: PostgreSQL (local server, passwordless)
- **Validation**: FluentValidation (Application layer)
- **Dependency Injection**: Microsoft.Extensions.DependencyInjection
- **Logging**: Microsoft.Extensions.Logging

## Key Design Decisions

### 1. Immutable Financial Records
- Tickets, Payments, and Cash Sessions are immutable once finalized
- Changes create new versions with audit trail
- Prevents data corruption and ensures auditability
- Refunds create new transactions (don't modify originals)

### 2. Event Sourcing for Critical Operations
- All financial mutations emit domain events
- Events are stored immutably
- Enables audit trail reconstruction
- Complete state snapshots (before/after)

### 3. Database Architecture
- PostgreSQL database for all operations
- Local PostgreSQL server (passwordless for development)
- Database: `magidesk_pos` (already exists)
- Can support multi-terminal scenarios with proper connection pooling
- Offline-capable design (can work with local PostgreSQL instance)
- EF Core migrations for schema management

### 4. Strict Invariant Enforcement
- Domain entities enforce invariants at construction
- Application layer validates use case preconditions
- Infrastructure layer enforces data integrity
- State machine pattern for ticket/payment states (prevents invalid states)

### 5. Split Payment Architecture
- Multiple payments per ticket supported from start
- Partial payments allowed
- Ticket closes when PaidAmount >= TotalAmount
- Each payment is independent transaction

### 6. Discount System
- Item-level and ticket-level discounts
- Only one discount applies (max discount selected)
- Discounts snapshotted when applied (immutable)
- Multiple discount types supported

### 7. Payment Type Extensibility
- Base Payment entity with type-specific fields
- Polymorphic payment types (Cash, Card, Gift Cert, Custom)
- Card payments support authorize/capture workflow
- Gift certificates support cash back

## Non-Negotiable Rules

1. **No Business Logic in UI**: ViewModels are thin coordinators
2. **No Database Access from UI**: All data access through Application layer
3. **Domain Logic Only in Domain**: No exceptions
4. **Auditable Financial Mutations**: Every change is logged
5. **Reference Systems are Reference Only**: No code copying


# Magidesk POS

A modern, ground-up rebuild of a Windows-based Point of Sale system using Clean Architecture principles, .NET 8, and WinUI 3.

## Project Status

**Current Phase**: ✅ Architecture & Design Complete, Ready for Implementation

All design documentation has been completed based on thorough analysis of the FloreantPOS reference system. See [PROJECT_STATUS.md](./PROJECT_STATUS.md) for details.

## Architecture Overview

This project follows **Clean Architecture** with strict layer separation:

- **Domain Layer**: Pure business logic, no dependencies
- **Application Layer**: Use cases and orchestration
- **Infrastructure Layer**: Technical implementations (EF Core, repositories)
- **Presentation Layer**: WinUI 3 UI with MVVM pattern

## Key Principles

1. **Domain-First**: All business logic lives in Domain layer
2. **Audit-First**: Every financial mutation is immutable and auditable
3. **Offline-Capable**: System functions without network connectivity
4. **Strict MVVM**: UI contains zero business logic
5. **Invariant Enforcement**: Business rules cannot be bypassed

## Documentation

- [ARCHITECTURE.md](./ARCHITECTURE.md) - Clean Architecture design
- [DOMAIN_MODEL_FULL.md](./DOMAIN_MODEL_FULL.md) - Complete domain model (full POS)
- [DOMAIN_MODEL.md](./DOMAIN_MODEL.md) - Domain model summary
- [INVARIANTS_FULL.md](./INVARIANTS_FULL.md) - Complete invariants (full POS)
- [INVARIANTS.md](./INVARIANTS.md) - Invariants summary
- [SCOPE.md](./SCOPE.md) - Full POS scope definition
- [FLOREANTPOS_ANALYSIS.md](./FLOREANTPOS_ANALYSIS.md) - Reference system analysis
- [ASSUMPTIONS.md](./ASSUMPTIONS.md) - Design assumptions and decisions
- [EXECUTION_PLAN.md](./EXECUTION_PLAN.md) - 24-week implementation plan
- [DATABASE_SETUP.md](./DATABASE_SETUP.md) - PostgreSQL configuration
- [PROJECT_STATUS.md](./PROJECT_STATUS.md) - Current project status

## Solution Structure

```
Magidesk/
├── Magidesk.Domain/              # Core business logic
├── Magidesk.Application/         # Use cases and orchestration
├── Magidesk.Infrastructure/      # Technical implementations
├── Magidesk.Presentation/        # WinUI 3 UI
└── Tests/
    ├── Magidesk.Domain.Tests/
    └── Magidesk.Application.Tests/
```

## Technology Stack

- **.NET**: 8.0
- **UI Framework**: WinUI 3
- **MVVM Framework**: CommunityToolkit.Mvvm (to be added)
- **ORM**: EF Core (to be added)
- **Database**: PostgreSQL (local, passwordless)
- **Testing**: xUnit

## Getting Started

### Prerequisites

- .NET 8 SDK
- Windows 10/11
- PostgreSQL (local server, passwordless)
- Visual Studio 2022 or VS Code with C# extension

### Building

```bash
dotnet build
```

### Running Tests

```bash
dotnet test
```

## Full POS Scope

This is a **complete POS system rebuild**, not an MVP. The architecture supports all features from the start, with phased implementation.

**Core Features**:
- ✅ Ticket management (create, edit, split, void, refund)
- ✅ Multiple payment types (Cash, Credit, Debit, Gift Cert, Custom)
- ✅ Split payments (multiple payments per ticket)
- ✅ Tips/Gratuity
- ✅ Item and ticket-level discounts
- ✅ Tax calculation (including tax-exempt)
- ✅ Service and delivery charges
- ✅ Cash drawer and session management
- ✅ Drawer pull reports
- ✅ Shifts and order types
- ✅ Table management (restaurant)
- ✅ Kitchen and receipt printing
- ✅ Complete audit trail

See [SCOPE.md](./SCOPE.md) for complete feature list.

## Development Guidelines

### Non-Negotiable Rules

1. **No Business Logic in UI**: ViewModels are thin coordinators
2. **No Database Access from UI**: All data access through Application layer
3. **Domain Logic Only in Domain**: No exceptions
4. **Auditable Financial Mutations**: Every change is logged
5. **Reference Systems are Reference Only**: No code copying

### Code Organization

- **Domain**: Entities, Value Objects, Domain Services, Domain Events
- **Application**: Commands, Queries, DTOs, Application Services
- **Infrastructure**: Repositories, EF Core configurations, external services
- **Presentation**: Views, ViewModels, Converters

## Next Steps

See [EXECUTION_PLAN.md](./EXECUTION_PLAN.md) for the detailed 24-week implementation plan.

**Immediate next steps** (Phase 1, Week 1):
1. Implement Money value object
2. Implement Ticket entity (full model)
3. Implement OrderLine entity
4. Implement Payment entity (base)
5. Implement domain services
6. Set up EF Core with PostgreSQL
7. Create initial database migration

## Database

- **Database**: `magidesk_pos` (exists)
- **Schema**: `magidesk` (created, empty)
- **Connection**: Local PostgreSQL (passwordless)
- **ORM**: EF Core (migrations for schema management)

## License

[To be determined]

## Contributing

[To be determined]


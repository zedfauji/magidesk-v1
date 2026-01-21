# Magidesk.Application

The application layer orchestrates domain logic and defines use cases. This layer depends only on the Domain layer.

## Structure

```
Magidesk.Application/
├── Commands/           # Write operations (CQRS-style)
│   ├── OpenCashSessionCommand.cs
│   ├── CloseCashSessionCommand.cs
│   ├── CreateTicketCommand.cs
│   └── ...
├── Queries/            # Read operations
│   ├── GetCurrentCashSessionQuery.cs
│   ├── GetTicketQuery.cs
│   └── ...
├── DTOs/               # Data transfer objects for Presentation
│   ├── TicketDto.cs
│   ├── OrderLineDto.cs
│   └── ...
├── Interfaces/         # Repository and service interfaces
│   ├── ITicketRepository.cs
│   ├── IPaymentRepository.cs
│   └── ...
└── Services/           # Application services
    └── AuditService.cs
```

## Rules

- **Depends only on Domain**: No infrastructure dependencies
- **Orchestrates domain logic**: Coordinates domain objects and services
- **Transaction boundaries**: Defines unit of work boundaries
- **DTOs for Presentation**: Never expose domain entities to UI

## Implementation Status

- [ ] Repository interfaces
- [ ] Application service interfaces
- [ ] DTOs
- [ ] Commands
- [ ] Queries
- [ ] Application services


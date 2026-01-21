# Magidesk.Infrastructure

The infrastructure layer provides technical implementations. This layer implements interfaces defined in Application/Domain layers.

## Structure

```
Magidesk.Infrastructure/
├── Persistence/
│   ├── DbContext/
│   │   └── MagideskDbContext.cs
│   ├── Configurations/
│   │   ├── TicketConfiguration.cs
│   │   └── ...
│   └── Repositories/
│       ├── TicketRepository.cs
│       └── ...
├── Services/
│   └── AuditService.cs
└── External/
    └── (Future: payment processors, printers)
```

## Rules

- **Implements interfaces**: All implementations of Application/Domain interfaces
- **No business logic**: Pure technical concerns
- **EF Core here**: Database access only in this layer
- **External services**: Payment processors, printers, etc.

## Implementation Status

- [ ] EF Core DbContext
- [ ] Entity configurations
- [ ] Repository implementations
- [ ] Audit service implementation
- [ ] Database migrations


# Magidesk.Domain

The core domain layer containing all business logic. This layer has **zero dependencies** on infrastructure, UI, or external libraries.

## Structure

```
Magidesk.Domain/
├── Entities/           # Aggregate roots and entities
│   ├── Ticket.cs
│   ├── OrderLine.cs
│   ├── Payment.cs
│   ├── CashSession.cs
│   └── AuditEvent.cs
├── ValueObjects/       # Immutable value objects
│   ├── Money.cs
│   └── UserId.cs
├── DomainServices/     # Domain services for complex operations
│   ├── TicketDomainService.cs
│   ├── PaymentDomainService.cs
│   └── CashSessionDomainService.cs
├── DomainEvents/       # Domain events
│   ├── TicketCreated.cs
│   ├── TicketClosed.cs
│   └── ...
├── Enumerations/       # Domain enums
│   ├── TicketStatus.cs
│   ├── PaymentType.cs
│   └── ...
└── Exceptions/         # Domain exceptions
    ├── DomainException.cs
    ├── BusinessRuleViolationException.cs
    └── InvalidOperationException.cs
```

## Rules

- **No external dependencies**: No EF Core, no HTTP, no file I/O
- **Pure business logic**: Language-agnostic rules
- **Invariant enforcement**: All business rules enforced here
- **Immutable where possible**: Financial records are immutable once finalized

## Implementation Status

- [ ] Money value object
- [ ] UserId value object
- [ ] Ticket entity
- [ ] OrderLine entity
- [ ] Payment entity
- [ ] CashSession entity
- [ ] AuditEvent entity
- [ ] Domain services
- [ ] Domain events
- [ ] Domain exceptions


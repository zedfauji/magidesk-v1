# Magidesk POS - Assumptions and Decisions

## Document Purpose
This document captures all assumptions made during the initial design phase. These assumptions should be validated with stakeholders and updated as the project evolves.

## Business Assumptions

### B1: Single-Location Operation
- **Assumption**: Full POS targets single-location businesses initially
- **Rationale**: Simplifies architecture, no multi-tenant concerns
- **Future Consideration**: May need multi-location support later

### B2: Cash Sessions
- **Assumption**: Cash sessions align with shifts (one session per shift per user)
- **Rationale**: Proper cash accountability per shift
- **Future Consideration**: May need multiple sessions per day for shift changes

### B3: Tax Calculation
- **Assumption**: Support for multiple tax rates, tax groups, tax-exempt items
- **Rationale**: Full POS requires flexible tax handling
- **Future Consideration**: Price-includes-tax mode, tax by order type

### B4: Currency
- **Assumption**: Single currency (USD) initially, architecture supports multi-currency
- **Rationale**: Most businesses use single currency, but design supports expansion
- **Future Consideration**: Multi-currency support if needed

### B5: Menu Structure
- **Assumption**: Full menu structure with categories, groups, modifiers, sizes
- **Rationale**: Full POS requires complete menu management
- **Future Consideration**: Advanced features like recipes, inventory integration

## Technical Assumptions

### T1: Database Choice
- **Assumption**: PostgreSQL (local server, passwordless)
- **Rationale**: Production-ready database, supports complex queries, ACID compliance, better for multi-user scenarios
- **Migration Path**: EF Core migrations for schema changes

### T2: Concurrency Model
- **Assumption**: Optimistic concurrency with version fields
- **Rationale**: Better for offline scenarios, simpler than pessimistic locking
- **Future Consideration**: May need distributed locking for multi-terminal

### T3: Event Sourcing Scope
- **Assumption**: Audit events stored alongside entities (not full event sourcing)
- **Rationale**: Simpler implementation, sufficient for audit trail
- **Future Consideration**: Full event sourcing if replay/reconstruction needed

### T4: Offline Strategy
- **Assumption**: Local PostgreSQL instance, can work offline if server is local
- **Rationale**: PostgreSQL provides robust data storage, local instance supports offline operation
- **Future Consideration**: Sync mechanism for multi-terminal scenarios

### T5: Error Handling
- **Assumption**: Exceptions for business rule violations, graceful UI error display
- **Rationale**: Clear error boundaries, prevents silent failures
- **Future Consideration**: Retry mechanisms for infrastructure failures

## Architecture Assumptions

### A1: Layer Strictness
- **Assumption**: Strict layer separation, no shortcuts
- **Rationale**: Long-term maintainability, testability
- **Trade-off**: More boilerplate, but clearer boundaries

### A2: Domain Model Richness
- **Assumption**: Rich domain model with behavior in entities
- **Rationale**: Encapsulation, invariant enforcement
- **Trade-off**: More complex entities, but better correctness

### A3: DTO Usage
- **Assumption**: DTOs for all Application → Presentation communication
- **Rationale**: Prevents domain entity exposure, allows API evolution
- **Trade-off**: Mapping overhead, but better separation

### A4: Repository Pattern
- **Assumption**: Repository pattern for data access abstraction
- **Rationale**: Testability, can swap implementations
- **Trade-off**: Additional abstraction layer, but worth it

### A5: Dependency Injection
- **Assumption**: Constructor injection throughout
- **Rationale**: Testability, explicit dependencies
- **Trade-off**: More setup, but better design

## UI/UX Assumptions

### U1: Input Method
- **Assumption**: Primary input via keyboard and mouse (not touch-optimized)
- **Rationale**: Desktop-first, Windows desktop environment
- **Future Consideration**: Touch support if needed for tablet mode

### U2: Screen Resolution
- **Assumption**: Minimum 1920x1080 resolution
- **Rationale**: Modern desktop standard
- **Future Consideration**: Responsive layout for different sizes

### U3: Navigation
- **Assumption**: Simple navigation, few screens in MVP
- **Rationale**: MVP scope is limited
- **Future Consideration**: More complex navigation as features grow

### U4: Accessibility
- **Assumption**: Basic keyboard navigation, not full WCAG compliance in MVP
- **Rationale**: MVP focus on core functionality
- **Future Consideration**: Full accessibility in later phases

## Security Assumptions

### S1: Authentication
- **Assumption**: No authentication in MVP (single user, trusted environment)
- **Rationale**: Simplifies MVP, can add later
- **Future Consideration**: User accounts, roles, permissions

### S2: Data Encryption
- **Assumption**: SQLite database not encrypted in MVP
- **Rationale**: Local trusted environment, simplifies setup
- **Future Consideration**: Encryption at rest if needed

### S3: Audit Trail
- **Assumption**: Audit events stored locally, not tamper-proof
- **Rationale**: MVP scope, local environment
- **Future Consideration**: Cryptographic signatures, remote audit log

## Performance Assumptions

### P1: Data Volume
- **Assumption**: MVP handles <10,000 tickets, <100,000 order lines
- **Rationale**: Single-location, reasonable volume
- **Future Consideration**: Pagination, archiving for larger volumes

### P2: Response Time
- **Assumption**: UI operations complete in <100ms
- **Rationale**: Good user experience
- **Future Consideration**: Async operations, background processing

### P3: Concurrent Users
- **Assumption**: Single user per terminal in MVP
- **Rationale**: MVP scope
- **Future Consideration**: Multi-user support in later phases

## Integration Assumptions

### I1: Payment Processing
- **Assumption**: No external payment processors in MVP (cash only)
- **Rationale**: Simplifies MVP, no hardware integration
- **Future Consideration**: Payment gateway integration in Phase 2

### I2: Receipt Printing
- **Assumption**: No receipt printing in MVP
- **Rationale**: Can add later, not core to transaction processing
- **Future Consideration**: Printer integration in Phase 2

### I3: Barcode Scanning
- **Assumption**: No barcode scanning in MVP
- **Rationale**: Manual entry sufficient for MVP
- **Future Consideration**: Scanner integration if needed

## Validation and Review

### When to Review
- Before starting each phase
- When requirements change
- When assumptions are proven wrong
- During architecture reviews

### How to Update
1. Document new assumptions as they arise
2. Mark outdated assumptions as "Superseded"
3. Add rationale for changes
4. Update affected design documents

## Known Risks from Assumptions

### R1: Single User Limitation
- **Risk**: May need multi-user support sooner than expected
- **Mitigation**: Design with user context in mind, easy to add later

### R2: Offline-Only
- **Risk**: May need sync capabilities earlier
- **Mitigation**: Design data model to support sync (timestamps, version fields)

### R3: Simple Menu Structure
- **Risk**: May need modifiers/options immediately
- **Mitigation**: Design OrderLine to be extensible

### R4: No Inventory
- **Risk**: May need inventory tracking
- **Mitigation**: Keep MenuItem simple, can add inventory later

## Decisions Made

### D1: Use EF Core (not Dapper)
- **Decision**: EF Core for ORM
- **Rationale**: Better integration with .NET, migrations, change tracking
- **Alternative Considered**: Dapper (rejected for less features)

### D2: Use CommunityToolkit.Mvvm
- **Decision**: CommunityToolkit.Mvvm for MVVM
- **Rationale**: Modern, maintained, good WinUI 3 support
- **Alternative Considered**: Prism, ReactiveUI (chosen for simplicity)

### D3: SQLite First
- **Decision**: Start with SQLite, migrate later if needed
- **Rationale**: Offline-first, no server setup
- **Alternative Considered**: SQL Server from start (rejected for complexity)

### D4: No CQRS Initially
- **Decision**: Simple command/query separation, not full CQRS
- **Rationale**: MVP doesn't need read/write separation complexity
- **Future Consideration**: CQRS if read performance becomes issue

### D5: Synchronous Domain Operations
- **Decision**: Domain layer is synchronous
- **Rationale**: Simpler, domain logic is fast
- **Future Consideration**: Async if needed for performance

## Questions for Stakeholders

1. **Tax Configuration**: Single rate or multiple? Tax-exempt items needed?
2. **Payment Types**: Which payment types are required for MVP? (assumed cash only)
3. **Receipts**: Are receipts required for MVP? (assumed no)
4. **Multi-User**: Is single-user acceptable for MVP? (assumed yes)
5. **Menu Management**: Is menu editing required in MVP? (assumed no, read-only)
6. **Reporting**: What reports are essential? (assumed none for MVP)
7. **Backup**: How should data be backed up? (not addressed in MVP)


# Development Policies

## Code Quality Standards

### Code Review Requirements
- All code must be reviewed before merge
- Review for architecture compliance
- Review for business logic placement
- Review for test coverage
- Review for documentation

### Code Style
- Follow C# coding conventions
- Use meaningful names
- Keep methods small (< 50 lines)
- Keep classes focused (single responsibility)
- Use comments for complex logic only

### Naming Conventions
- **Classes**: PascalCase (e.g., `Ticket`, `OrderLine`)
- **Methods**: PascalCase (e.g., `CalculateTotals`, `CanClose`)
- **Properties**: PascalCase (e.g., `TotalAmount`, `CreatedAt`)
- **Fields**: camelCase with underscore prefix for private (e.g., `_repository`)
- **Constants**: PascalCase (e.g., `MaxRetryAttempts`)
- **Enums**: PascalCase (e.g., `TicketStatus`, `PaymentType`)
- **Interfaces**: I prefix (e.g., `ITicketRepository`)

## Git Workflow

### Branch Strategy
- `main`: Production-ready code
- `develop`: Integration branch
- `feature/*`: Feature branches
- `bugfix/*`: Bug fix branches
- `hotfix/*`: Hotfix branches

### Commit Messages
- Use clear, descriptive messages
- Format: `[Type] Description`
- Types: `feat`, `fix`, `docs`, `refactor`, `test`, `chore`
- Reference issues if applicable

### Pull Requests
- One feature per PR
- Include tests
- Update documentation
- Get approval before merge

## Testing Policies

### Test Coverage Requirements
- Domain layer: >90% coverage
- Application layer: >80% coverage
- Infrastructure: >70% coverage
- Presentation: Focus on critical paths

### Test Types
- **Unit Tests**: Test individual components
- **Integration Tests**: Test component integration
- **End-to-End Tests**: Test complete workflows

### Test Organization
- One test class per class being tested
- Test methods: `[MethodName]_[Scenario]_[ExpectedResult]`
- Use Arrange-Act-Assert pattern
- Mock external dependencies

## Documentation Requirements

### Code Documentation
- XML comments on public APIs
- Document complex algorithms
- Document business rules
- Keep comments up to date

### Architecture Documentation
- Update ARCHITECTURE.md when architecture changes
- Document design decisions
- Document trade-offs

### API Documentation
- Document all public interfaces
- Document parameters and return values
- Document exceptions
- Provide usage examples

## Dependency Management

### NuGet Packages
- Use stable versions (avoid pre-release)
- Keep packages up to date
- Document why each package is needed
- Review package licenses
- **REQUIRE**: Use Context7 MCP tool to check latest documentation before using new library features

### Package Updates
- Test updates before applying
- Update one package at a time
- Review changelogs
- Test thoroughly after updates
- Use Context7 MCP tool to check for breaking changes

### Required Libraries
- **EF Core**: ORM for database access (Infrastructure only)
- **CommunityToolkit.Mvvm**: MVVM framework
- **FluentValidation**: Validation framework
- **Polly**: Resilience and transient-fault handling
- **xUnit**: Testing framework
- **Moq**: Mocking framework
- **FluentAssertions**: Assertion library (recommended)

### Optional Libraries
- **Dapper**: Lightweight ORM (if performance requires, alongside EF Core)
- **AutoMapper**: Object mapping (if needed)
- **MediatR**: Mediator pattern (if CQRS needed)

## Security Policies

### Data Protection
- Never log sensitive data (card numbers, passwords)
- Encrypt sensitive data at rest
- Use secure connections for external APIs
- Follow PCI-DSS if handling card data

### Authentication
- Implement proper authentication
- Use secure password storage
- Implement session management
- Handle permissions properly

### Input Validation
- Validate all user input
- Sanitize input for display
- Use parameterized queries
- Prevent SQL injection

## Performance Policies

### Database Queries
- Use appropriate indexes
- Avoid N+1 queries
- Use pagination for large datasets
- Monitor query performance

### Memory Management
- Dispose of resources properly
- Avoid memory leaks
- Use async/await for I/O
- Profile memory usage

### UI Performance
- Keep UI responsive
- Use async operations
- Virtualize large lists
- Optimize data binding

## Error Handling Policies

### Exception Handling
- Catch specific exceptions
- Log exceptions with context
- Don't swallow exceptions
- Provide user-friendly messages

### Logging
- Log all errors
- Log important business events
- Use appropriate log levels
- Don't log sensitive data

### Error Recovery
- Implement retry logic where appropriate
- Handle transient failures
- Provide fallback mechanisms
- Test error scenarios

## Refactoring Policies

### When to Refactor
- Code smells detected
- Performance issues
- Architecture violations
- Technical debt accumulation

### Refactoring Rules
- Refactor in separate commits
- Maintain test coverage
- Update documentation
- Get review before merge

## MCP Tools Usage

### Context7 MCP Tool
- **REQUIRE**: Use before implementing new library features
- **REQUIRE**: Check latest documentation for libraries
- **REQUIRE**: Verify compatibility and breaking changes
- Document library usage decisions

### PostgreSQL MCP Tool
- **ALLOW**: Database analysis and optimization
- **ALLOW**: Query performance analysis
- **ALLOW**: Health checks and schema analysis
- **BLOCK**: Direct schema modifications (use EF Core migrations)

### Sequential Thinking MCP Tool
- **REQUIRE**: Use for complex domain problems
- **REQUIRE**: Use for architecture decisions
- **REQUIRE**: Use for multi-step implementations
- Document thinking process for complex solutions

### Memory Bank MCP Tool
- **REQUIRE**: Store important architectural decisions
- **REQUIRE**: Document domain model decisions
- **REQUIRE**: Maintain project context and history
- **REQUIRE**: Update when decisions change
- Use for knowledge persistence and onboarding

## Code Review Checklist

### Architecture
- [ ] Follows Clean Architecture
- [ ] No layer violations
- [ ] Proper dependency direction
- [ ] No business logic in UI
- [ ] Used Context7 for library documentation (if new library features)
- [ ] Used Sequential Thinking for complex problems (if applicable)

### Domain
- [ ] Invariants enforced
- [ ] Rich domain model
- [ ] Proper state management
- [ ] Domain events used appropriately

### Application
- [ ] Use cases clear
- [ ] DTOs used correctly
- [ ] Validation in place
- [ ] Error handling appropriate

### Infrastructure
- [ ] Repository pattern followed
- [ ] EF Core configured correctly
- [ ] External services abstracted
- [ ] No business logic

### Presentation
- [ ] MVVM pattern followed
- [ ] No business logic
- [ ] Proper data binding
- [ ] Accessibility considered

### Testing
- [ ] Tests written
- [ ] Coverage adequate
- [ ] Tests pass
- [ ] Edge cases covered

## Prohibited Practices

### NEVER:
- Skip code reviews
- Commit without tests
- Ignore linter warnings
- Copy code from FloreantPOS
- Hard-code configuration
- Commit secrets/passwords
- Skip documentation updates
- Merge without approval

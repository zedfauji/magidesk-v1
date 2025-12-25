# Testing Requirements

## Test Coverage Requirements

### Domain Layer
- **Minimum**: 90% code coverage
- **Target**: 95%+ coverage
- **Focus**: All invariants, all domain services, all value objects
- **Critical**: Every invariant must have tests

### Application Layer
- **Minimum**: 80% code coverage
- **Target**: 85%+ coverage
- **Focus**: All use cases, all validators, all mappers
- **Critical**: All commands and queries tested

### Infrastructure Layer
- **Minimum**: 70% code coverage
- **Target**: 75%+ coverage
- **Focus**: Repository implementations, EF Core configurations
- **Critical**: Data access paths tested

### Presentation Layer
- **Focus**: Critical user flows
- **Coverage**: Not measured by percentage
- **Focus**: ViewModel logic, navigation, error handling

## Test Types

### Unit Tests
- Test individual components in isolation
- Mock external dependencies
- Fast execution
- Test behavior, not implementation

### Integration Tests
- Test component integration
- Use test database
- Test repository implementations
- Test EF Core configurations

### End-to-End Tests
- Test complete workflows
- Test user scenarios
- Test critical paths
- Slower but comprehensive

## Test Organization

### Project Structure
```
Tests/
├── Domain.Tests/
│   ├── Entities/
│   ├── ValueObjects/
│   ├── Services/
│   └── Invariants/
├── Application.Tests/
│   ├── Commands/
│   ├── Queries/
│   └── Services/
└── Integration.Tests/
    ├── Repositories/
    └── Workflows/
```

### Test Naming
- Format: `[Entity]_[Method]_[Scenario]_[ExpectedResult]`
- Example: `Ticket_CanClose_WhenPaid_ReturnsTrue`
- Example: `Ticket_AddOrderLine_WhenClosed_ThrowsInvalidOperationException`

### Test Structure
- Arrange: Set up test data
- Act: Execute operation
- Assert: Verify results
- One assertion per test (when possible)

## Domain Layer Testing

### Entity Tests
- Test construction
- Test invariants
- Test state transitions
- Test business logic methods
- Test edge cases

### Value Object Tests
- Test immutability
- Test equality
- Test validation
- Test operations

### Domain Service Tests
- Test all methods
- Test business rules
- Test edge cases
- Mock dependencies if needed

### Invariant Tests
- Every invariant must have unit test
- Test violation scenarios
- Test enforcement
- Verify exceptions thrown

## Application Layer Testing

### Command Tests
- Test successful execution
- Test validation failures
- Test business rule violations
- Test error handling
- Mock repositories

### Query Tests
- Test data retrieval
- Test filtering
- Test sorting
- Test pagination
- Mock repositories

### Validator Tests
- Test all validation rules
- Test error messages
- Test edge cases

## Infrastructure Layer Testing

### Repository Tests
- Test CRUD operations
- Test queries
- Test relationships
- Use test database
- Clean up after tests

### EF Core Configuration Tests
- Test entity mappings
- Test value object mappings
- Test relationships
- Test constraints

## Test Data

### Test Fixtures
- Create test data builders
- Use factories for test data
- Keep test data realistic
- Reuse test data where possible

### Test Database
- Use separate test database
- Reset database between tests (or use transactions)
- Seed test data as needed
- Clean up test data

## Mocking

### When to Mock
- External dependencies
- Infrastructure services
- Slow operations
- Non-deterministic operations

### Mocking Rules
- Mock repositories in Application tests
- Mock external services
- Don't mock domain entities
- Use appropriate mocking framework (Moq)

## Test Execution

### Test Execution
- All tests must pass before merge
- Run tests locally before commit
- Run tests in CI/CD
- Fix failing tests immediately

### Test Performance
- Unit tests: < 100ms each
- Integration tests: < 1s each
- E2E tests: < 10s each
- Total test suite: < 5 minutes

## Test Maintenance

### Keep Tests Updated
- Update tests when code changes
- Remove obsolete tests
- Refactor tests when needed
- Keep tests maintainable

### Test Quality
- Tests should be readable
- Tests should be maintainable
- Tests should be fast
- Tests should be reliable

## Prohibited Practices

### NEVER:
- Skip writing tests
- Comment out failing tests
- Ignore test failures
- Write tests that depend on each other
- Use production database for tests
- Hard-code test data in multiple places

### ALWAYS:
- Write tests for new code
- Keep tests passing
- Update tests when code changes
- Use appropriate test types
- Clean up test data

## Test Documentation

### Test Documentation
- Document complex test scenarios
- Document test data setup
- Document test assumptions
- Keep test documentation current

## Coverage Reports

### Coverage Tracking
- Track coverage per layer
- Set coverage goals
- Review coverage reports
- Improve coverage over time

### Coverage Tools
- Use code coverage tools
- Generate coverage reports
- Review coverage gaps
- Aim for high coverage

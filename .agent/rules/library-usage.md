# Library Usage Rules

## Preferred Libraries

### Core Libraries (Required)
- **EF Core**: ORM for database access (Infrastructure layer only)
- **CommunityToolkit.Mvvm**: MVVM framework for WinUI 3
- **Microsoft.Extensions.DependencyInjection**: Dependency injection
- **Microsoft.Extensions.Logging**: Logging framework
- **FluentValidation**: Validation framework (Application layer)

### Recommended Libraries
- **Polly**: Resilience and transient-fault handling
- **AutoMapper**: Object-to-object mapping (if needed)
- **xUnit**: Testing framework
- **Moq**: Mocking framework for tests
- **FluentAssertions**: Assertion library for tests

### Optional Libraries (Use When Needed)
- **Dapper**: Lightweight ORM (if performance requires, alongside EF Core)
- **MediatR**: Mediator pattern (if CQRS pattern needed)
- **Serilog**: Advanced logging (if needed)
- **Refit**: HTTP client library (if external APIs needed)

## Library Selection Principles

### Prefer Established Libraries
- **REQUIRE**: Use well-maintained, popular libraries
- **REQUIRE**: Prefer Microsoft/Asp.NET Foundation libraries
- **REQUIRE**: Check library maintenance status
- **REQUIRE**: Verify .NET 8 compatibility

### Library Evaluation
- Check NuGet download statistics
- Check GitHub stars and activity
- Check last update date
- Check .NET 8 compatibility
- Check license compatibility

### Avoid
- ❌ Unmaintained libraries
- ❌ Libraries with security vulnerabilities
- ❌ Libraries incompatible with .NET 8
- ❌ Reinventing the wheel

## EF Core Usage

### When to Use
- **REQUIRE**: All database access in Infrastructure layer
- **REQUIRE**: Entity configurations
- **REQUIRE**: Migrations for schema changes
- **REQUIRE**: Repository pattern implementation

### Configuration
- Use Fluent API for entity configuration
- Configure value objects properly
- Use optimistic concurrency (Version fields)
- Configure relationships properly
- Use appropriate query tracking

### Prohibited
- ❌ EF Core in Domain layer
- ❌ EF Core in Presentation layer
- ❌ Direct DbContext access from Application layer
- ❌ Skipping repository pattern

## Dapper Usage (If Needed)

### When to Use
- **OPTIONAL**: High-performance read queries
- **OPTIONAL**: Complex queries that EF Core struggles with
- **OPTIONAL**: Reporting queries
- **ALLOW**: Alongside EF Core (not replacement)

### Usage Rules
- Use in Infrastructure layer only
- Use for read-only queries primarily
- Use parameterized queries (prevent SQL injection)
- Use Dapper for specific performance-critical paths
- Keep EF Core for most operations

### Integration
- Can use Dapper in same project as EF Core
- Use Dapper for specific repositories if needed
- Document why Dapper is used for specific queries
- Maintain consistency with EF Core patterns

## Polly Usage

### When to Use
- **REQUIRE**: External API calls (payment gateways)
- **REQUIRE**: Database connection retries
- **REQUIRE**: Transient fault handling
- **REQUIRE**: Circuit breaker patterns

### Configuration
- Configure retry policies for transient failures
- Use circuit breaker for external services
- Configure timeout policies
- Use appropriate policies per service type

### Examples
- Payment gateway calls: Retry with exponential backoff
- Database connections: Retry transient failures
- External APIs: Circuit breaker pattern

## CommunityToolkit.Mvvm Usage

### When to Use
- **REQUIRE**: All ViewModels
- **REQUIRE**: Property change notifications
- **REQUIRE**: Commands
- **REQUIRE**: MVVM pattern implementation

### Features to Use
- `[ObservableProperty]`: Auto-generate INotifyPropertyChanged
- `[RelayCommand]`: Auto-generate commands
- `[AsyncRelayCommand]`: Auto-generate async commands
- `SetProperty`: Property change helper

### Prohibited
- ❌ Manual INotifyPropertyChanged implementation
- ❌ Manual command implementation (use attributes)
- ❌ Skipping MVVM framework

## FluentValidation Usage

### When to Use
- **REQUIRE**: Command validation in Application layer
- **REQUIRE**: Input validation
- **REQUIRE**: Business rule validation (Application level)

### Configuration
- Create validators for each command
- Use validation rules
- Provide clear error messages
- Integrate with Application layer

### Prohibited
- ❌ Validation in Domain layer (use domain services)
- ❌ Validation in Presentation layer (except UI-level)
- ❌ Skipping validation

## Testing Libraries

### xUnit
- **REQUIRE**: All test projects
- Use for unit tests, integration tests
- Use test fixtures appropriately
- Use theory tests for parameterized tests

### Moq
- **REQUIRE**: Mocking in tests
- Mock repositories in Application tests
- Mock external services
- Don't mock domain entities

### FluentAssertions
- **RECOMMEND**: Assertion library
- Use for readable assertions
- Use for collection assertions
- Use for exception assertions

## Library Updates

### Update Policy
- Keep libraries updated
- Review changelogs before updating
- Test updates in development
- Update one library at a time
- Review breaking changes

### Security Updates
- **REQUIRE**: Apply security patches immediately
- Monitor security advisories
- Use tools to check vulnerabilities
- Update vulnerable packages promptly

## Library Documentation

### Documentation Requirements
- **REQUIRE**: Use Context7 MCP tool for latest documentation
- **REQUIRE**: Check library documentation before use
- **REQUIRE**: Follow library best practices
- **REQUIRE**: Use latest patterns from documentation

### Documentation Sources
- Official library documentation
- Context7 MCP tool for latest docs
- GitHub repositories
- NuGet package pages

## Prohibited Practices

### NEVER:
- Use unmaintained libraries
- Skip library documentation
- Use libraries with known vulnerabilities
- Reinvent functionality available in libraries
- Use incompatible library versions

### ALWAYS:
- Use established, maintained libraries
- Check documentation before use
- Follow library best practices
- Keep libraries updated
- Use Context7 for latest documentation

## Library Integration

### Dependency Management
- Use NuGet for package management
- Keep packages updated
- Document why each library is used
- Review package licenses

### Version Management
- Use stable versions (avoid pre-release)
- Lock versions in production
- Test updates before applying
- Document version requirements

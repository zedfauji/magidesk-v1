# Development Guide

## Overview

This guide provides comprehensive information for developers working on the Magidesk POS system. It covers development setup, coding standards, architectural patterns, testing procedures, and contribution guidelines.

## Development Environment Setup

### Prerequisites

#### Required Software
- **Visual Studio 2022** (17.8 or later) with:
  - .NET desktop development workload
  - ASP.NET and web development workload
  - Individual component: .NET 8 SDK
- **Git** (latest version)
- **PostgreSQL** (15.x or later)
- **Docker Desktop** (optional, for containerized development)
- **Node.js** (18.x or later) for frontend development tools

#### Hardware Requirements
- **RAM**: 16 GB minimum, 32 GB recommended
- **Storage**: 100 GB free SSD space
- **CPU**: Multi-core processor (i7 or equivalent recommended)

### Repository Setup

#### Clone Repository
```bash
# Clone the main repository
git clone https://github.com/your-org/magidesk-pos.git
cd magidesk-pos

# Clone submodules if any
git submodule update --init --recursive
```

#### Branch Strategy
```
main                    # Production-ready code
develop                 # Integration branch for features
feature/feature-name    # Feature development branches
bugfix/bug-description # Bug fix branches
hotfix/emergency-fix    # Critical fixes for production
release/version-x.y     # Release preparation branches
```

### Local Development Setup

#### Database Setup
```bash
# Install PostgreSQL (if not already installed)
# Create development database
createdb magidesk_dev

# Create development user
createuser --interactive magidesk_dev

# Grant permissions
psql -d magidesk_dev -c "GRANT ALL PRIVILEGES ON DATABASE magidesk_dev TO magidesk_dev;"
```

#### Configuration Setup
```json
// appsettings.Development.json
{
  "Application": {
    "Environment": "Development",
    "InstanceName": "DEV-LOCAL"
  },
  "Database": {
    "ConnectionString": "Host=localhost;Database=magidesk_dev;Username=magidesk_dev;Password=dev_password;",
    "EnableSensitiveDataLogging": true,
    "EnableDetailedErrors": true
  },
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.EntityFrameworkCore": "Information"
    }
  }
}
```

#### Database Migration
```bash
# Navigate to Infrastructure project
cd src/Magidesk.Infrastructure

# Apply migrations
dotnet ef database update

# Seed initial data
dotnet run --project ../Magidesk.SeedData -- --environment Development
```

## Project Structure

### Solution Organization
```
Magidesk.sln
├── src/
│   ├── Magidesk.Domain/
│   ├── Magidesk.Application/
│   ├── Magidesk.Infrastructure/
│   ├── Magidesk.Presentation/
│   ├── Magidesk.Api/
│   └── Magidesk.Tests/
├── tests/
│   ├── Magidesk.Domain.Tests/
│   ├── Magidesk.Application.Tests/
│   ├── Magidesk.Infrastructure.Tests/
│   └── Magidesk.Presentation.Tests/
├── docs/
├── scripts/
└── tools/
```

### Layer Responsibilities

#### Domain Layer
```csharp
// Magidesk.Domain/
Entities/
├── Ticket.cs              // Aggregate root
├── OrderLine.cs           // Entity
├── Payment.cs             // Entity
├── User.cs                // Entity
└── ...

ValueObjects/
├── Money.cs               // Value object
├── UserId.cs              // Value object
├── TicketNumber.cs        // Value object
└── ...

Services/
├── ITicketService.cs      // Domain service interface
├── TicketService.cs       // Domain service implementation
└── ...

Events/
├── TicketCreated.cs       // Domain event
├── PaymentProcessed.cs    // Domain event
└── ...

Exceptions/
├── BusinessRuleViolationException.cs
├── DomainInvalidOperationException.cs
└── ...
```

#### Application Layer
```csharp
// Magidesk.Application/
Commands/
├── CreateTicketCommand.cs
├── CreateTicketCommandHandler.cs
├── ProcessPaymentCommand.cs
├── ProcessPaymentCommandHandler.cs
└── ...

Queries/
├── GetTicketQuery.cs
├── GetTicketQueryHandler.cs
├── GetOpenTicketsQuery.cs
├── GetOpenTicketsQueryHandler.cs
└── ...

DTOs/
├── TicketDto.cs
├── OrderLineDto.cs
├── PaymentDto.cs
└── ...

Interfaces/
├── ITicketRepository.cs
├── IPaymentRepository.cs
├── IUserRepository.cs
└── ...

Validators/
├── CreateTicketCommandValidator.cs
├── ProcessPaymentCommandValidator.cs
└── ...
```

#### Infrastructure Layer
```csharp
// Magidesk.Infrastructure/
Persistence/
├── MagideskDbContext.cs
├── Configurations/
│   ├── TicketConfiguration.cs
│   ├── OrderLineConfiguration.cs
│   └── ...
├── Repositories/
│   ├── TicketRepository.cs
│   ├── PaymentRepository.cs
│   └── ...
└── Migrations/
    ├── 20240101000000_InitialCreate.cs
    └── ...

ExternalServices/
├── PaymentGateway/
│   ├── IPaymentGateway.cs
│   ├── StripePaymentGateway.cs
│   └── ...
├── EmailService/
│   ├── IEmailService.cs
│   └── SmtpEmailService.cs
└── ...
```

#### Presentation Layer
```csharp
// Magidesk.Presentation/
Views/
├── MainWindow.xaml
├── OrderEntryView.xaml
├── PaymentView.xaml
└── ...

ViewModels/
├── MainWindowViewModel.cs
├── OrderEntryViewModel.cs
├── PaymentViewModel.cs
└── ...

Controls/
├── CustomButton.cs
├── NumericTextBox.cs
└── ...

Converters/
├── BoolToVisibilityConverter.cs
├── CurrencyConverter.cs
└── ...

Services/
├── NavigationService.cs
├── DialogService.cs
└── ...
```

## Coding Standards

### C# Coding Guidelines

#### Naming Conventions
```csharp
// Classes: PascalCase
public class TicketService
public class OrderLineDto
public class PaymentGatewayException

// Interfaces: PascalCase with I prefix
public interface ITicketRepository
public interface IPaymentService

// Methods: PascalCase
public void CreateTicket()
public async Task<TicketDto> GetTicketAsync(Guid id)

// Properties: PascalCase
public string TicketNumber { get; set; }
public decimal TotalAmount { get; private set; }

// Fields: camelCase with underscore prefix
private readonly ITicketRepository _ticketRepository;
private readonly ILogger<TicketService> _logger;

// Constants: PascalCase
public const int MaxGuestCount = 20;
public static readonly string DefaultCurrency = "USD";

// Private methods: camelCase
private void validateTicket()
private async Task sendNotificationAsync()
```

#### Code Organization
```csharp
// File organization
namespace Magidesk.Application.Commands
{
    public class CreateTicketCommand : ICommand<CreateTicketResult>
    {
        // Properties first
        public Guid TableId { get; set; }
        public int GuestCount { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();

        // Validation
        public ValidationResult Validate()
        {
            // Implementation
        }
    }

    public class CreateTicketCommandHandler : ICommandHandler<CreateTicketCommand, CreateTicketResult>
    {
        // Dependencies first
        private readonly ITicketRepository _ticketRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateTicketCommandHandler> _logger;

        // Constructor
        public CreateTicketCommandHandler(
            ITicketRepository ticketRepository,
            IUnitOfWork unitOfWork,
            ILogger<CreateTicketCommandHandler> logger)
        {
            _ticketRepository = ticketRepository ?? throw new ArgumentNullException(nameof(ticketRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // Public methods
        public async Task<CreateTicketResult> HandleAsync(CreateTicketCommand command, CancellationToken cancellationToken = default)
        {
            // Implementation
        }

        // Private methods
        private void validateCommand(CreateTicketCommand command)
        {
            // Implementation
        }
    }
}
```

#### Exception Handling
```csharp
// Domain exceptions
public class BusinessRuleViolationException : DomainException
{
    public BusinessRuleViolationException(string message) : base(message) { }
    public BusinessRuleViolationException(string message, Exception innerException) : base(message, innerException) { }
}

// Application exceptions
public class NotFoundException : ApplicationException
{
    public NotFoundException(string entityName, object id) : base($"{entityName} with id {id} not found") { }
}

// Proper exception handling
public async Task<TicketDto> GetTicketAsync(Guid id)
{
    try
    {
        var ticket = await _ticketRepository.GetByIdAsync(id);
        if (ticket == null)
        {
            throw new NotFoundException("Ticket", id);
        }

        return _mapper.Map<TicketDto>(ticket);
    }
    catch (NotFoundException)
    {
        throw; // Re-throw domain exceptions
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error retrieving ticket with id {TicketId}", id);
        throw new ApplicationException("Error retrieving ticket", ex);
    }
}
```

### XAML Guidelines

#### Naming Conventions
```xml
<!-- Controls: camelCase -->
<TextBox x:Name="ticketNumberTextBox" />
<Button x:Name="saveButton" />
<ListView x:Name="orderLinesListView" />

<!-- Resources: PascalCase -->
<Style x:Key="PrimaryButtonStyle" />
<DataTemplate x:Key="OrderLineTemplate" />
<Converter x:Key="CurrencyConverter" />
```

#### Code Organization
```xml
<Window x:Class="Magidesk.Presentation.Views.OrderEntryView"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
        xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
        mc:Ignorable="d"
        Title="Order Entry" Height="800" Width="1200">
    
    <!-- Resources section first -->
    <Window.Resources>
        <Style x:Key="PrimaryButtonStyle" TargetType="Button">
            <!-- Style definition -->
        </Style>
        
        <DataTemplate x:Key="OrderLineTemplate">
            <!-- Template definition -->
        </DataTemplate>
    </Window.Resources>
    
    <!-- Layout structure -->
    <Grid>
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto" />
            <RowDefinition Height="*" />
            <RowDefinition Height="Auto" />
        </Grid.RowDefinitions>
        
        <!-- Header -->
        <StackPanel Grid.Row="0" Orientation="Horizontal" Margin="10">
            <!-- Header content -->
        </StackPanel>
        
        <!-- Main content -->
        <Grid Grid.Row="1">
            <!-- Main content -->
        </Grid>
        
        <!-- Footer -->
        <StackPanel Grid.Row="2" Orientation="Horizontal" Margin="10">
            <!-- Footer content -->
        </StackPanel>
    </Grid>
</Window>
```

## Testing Guidelines

### Test Organization

#### Test Project Structure
```
Magidesk.Domain.Tests/
├── Entities/
│   ├── TicketTests.cs
│   ├── OrderLineTests.cs
│   └── ...
├── ValueObjects/
│   ├── MoneyTests.cs
│   ├── UserIdTests.cs
│   └── ...
├── Services/
│   ├── TicketServiceTests.cs
│   └── ...
└── TestHelpers/
    ├── TestDataBuilder.cs
    └── ...
```

#### Test Categories
```csharp
// Test categories for organization
[TestCategory("Unit")]
[TestCategory("Integration")]
[TestCategory("Performance")]
[TestCategory("Security")]
```

### Unit Testing

#### Domain Entity Tests
```csharp
[TestClass]
public class TicketTests
{
    [TestMethod]
    [TestCategory("Unit")]
    public void CreateTicket_WithValidData_ShouldCreateTicket()
    {
        // Arrange
        var tableId = Guid.NewGuid();
        var createdBy = new UserId(Guid.NewGuid());
        
        // Act
        var ticket = new Ticket(tableId, createdBy);
        
        // Assert
        Assert.IsNotNull(ticket);
        Assert.AreEqual(tableId, ticket.TableId);
        Assert.AreEqual(createdBy, ticket.CreatedBy);
        Assert.AreEqual(TicketStatus.Draft, ticket.Status);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [ExpectedException(typeof(BusinessRuleViolationException))]
    public void AddOrderLine_ToClosedTicket_ShouldThrowException()
    {
        // Arrange
        var ticket = TestDataBuilder.CreateClosedTicket();
        var orderLine = TestDataBuilder.CreateOrderLine();
        
        // Act
        ticket.AddOrderLine(orderLine);
        
        // Assert - Exception expected
    }

    [TestMethod]
    [TestCategory("Unit")]
    public void CalculateTotals_WithMultipleItems_ShouldCalculateCorrectly()
    {
        // Arrange
        var ticket = TestDataBuilder.CreateTicket();
        ticket.AddOrderLine(TestDataBuilder.CreateOrderLine(10.00m, 2));
        ticket.AddOrderLine(TestDataBuilder.CreateOrderLine(5.00m, 1));
        
        // Act
        ticket.CalculateTotals();
        
        // Assert
        Assert.AreEqual(25.00m, ticket.SubtotalAmount);
        Assert.AreEqual(2.00m, ticket.TaxAmount); // Assuming 8% tax
        Assert.AreEqual(27.00m, ticket.TotalAmount);
    }
}
```

#### Service Tests
```csharp
[TestClass]
public class TicketServiceTests
{
    private Mock<ITicketRepository> _mockTicketRepository;
    private Mock<IUnitOfWork> _mockUnitOfWork;
    private Mock<ILogger<TicketService>> _mockLogger;
    private TicketService _ticketService;

    [TestInitialize]
    public void Setup()
    {
        _mockTicketRepository = new Mock<ITicketRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockLogger = new Mock<ILogger<TicketService>>();
        
        _ticketService = new TicketService(
            _mockTicketRepository.Object,
            _mockUnitOfWork.Object,
            _mockLogger.Object);
    }

    [TestMethod]
    [TestCategory("Unit")]
    public async Task CreateTicketAsync_WithValidCommand_ShouldCreateTicket()
    {
        // Arrange
        var command = TestDataBuilder.CreateCreateTicketCommand();
        var expectedTicket = TestDataBuilder.CreateTicket();
        
        _mockTicketRepository.Setup(x => x.AddAsync(It.IsAny<Ticket>()))
                           .ReturnsAsync(expectedTicket);
        _mockUnitOfWork.Setup(x => x.SaveChangesAsync())
                     .ReturnsAsync(1);

        // Act
        var result = await _ticketService.CreateTicketAsync(command);

        // Assert
        Assert.IsNotNull(result);
        _mockTicketRepository.Verify(x => x.AddAsync(It.IsAny<Ticket>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
    }
}
```

### Integration Testing

#### Database Integration Tests
```csharp
[TestClass]
public class TicketRepositoryIntegrationTests
{
    private MagideskDbContext _context;
    private TicketRepository _repository;
    private string _connectionString;

    [TestInitialize]
    public void Setup()
    {
        _connectionString = "Host=localhost;Database=magidesk_test;Username=test_user;Password=test_password;";
        
        var options = new DbContextOptionsBuilder<MagideskDbContext>()
            .UseNpgsql(_connectionString)
            .Options;

        _context = new MagideskDbContext(options);
        _context.Database.EnsureCreated();
        
        _repository = new TicketRepository(_context);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [TestMethod]
    [TestCategory("Integration")]
    public async Task AddAsync_WithValidTicket_ShouldPersistTicket()
    {
        // Arrange
        var ticket = TestDataBuilder.CreateTicket();

        // Act
        await _repository.AddAsync(ticket);
        await _context.SaveChangesAsync();

        // Assert
        var savedTicket = await _context.Tickets.FindAsync(ticket.Id);
        Assert.IsNotNull(savedTicket);
        Assert.AreEqual(ticket.TicketNumber, savedTicket.TicketNumber);
    }
}
```

### Performance Testing

#### Load Testing
```csharp
[TestMethod]
[TestCategory("Performance")]
public async Task GetOpenTicketsAsync_WithLargeDataset_ShouldCompleteWithinTimeLimit()
{
    // Arrange
    var stopwatch = Stopwatch.StartNew();
    var timeLimit = TimeSpan.FromSeconds(5);

    // Act
    var result = await _ticketService.GetOpenTicketsAsync();
    stopwatch.Stop();

    // Assert
    Assert.IsTrue(stopwatch.Elapsed < timeLimit, $"Operation took {stopwatch.Elapsed} which exceeds limit of {timeLimit}");
}
```

## Build and Deployment

### Local Build

#### Build Commands
```bash
# Build entire solution
dotnet build Magidesk.sln --configuration Release

# Build specific project
dotnet build src/Magidesk.Presentation/Magidesk.Presentation.csproj --configuration Release

# Build with warnings as errors
dotnet build Magidesk.sln --configuration Release --warnaserror

# Build for specific runtime
dotnet build src/Magidesk.Presentation/Magidesk.Presentation.csproj --configuration Release --runtime win-x64
```

#### Run Tests
```bash
# Run all tests
dotnet test

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test project
dotnet test tests/Magidesk.Domain.Tests/Magidesk.Domain.Tests.csproj

# Run tests with specific category
dotnet test --filter "TestCategory=Unit"
```

### Code Quality

#### Static Analysis
```bash
# Install .NET analyzers
dotnet add package Microsoft.CodeAnalysis.NetAnalyzers

# Run analyzers
dotnet build --configuration Release

# Install StyleCop
dotnet add package StyleCop.Analyzers
```

#### Code Formatting
```bash
# Install dotnet-format
dotnet tool install -g dotnet-format

# Format code
dotnet format

# Verify formatting
dotnet format --verify-no-changes
```

## Git Workflow

### Branch Naming Conventions

```
feature/feature-name              # New features
bugfix/bug-description           # Bug fixes
hotfix/emergency-fix             # Critical fixes
refactor/refactor-description     # Code refactoring
docs/documentation-update        # Documentation changes
chore/maintenance-task           # Maintenance tasks
```

### Commit Message Format

```
<type>(<scope>): <description>

[optional body]

[optional footer]
```

#### Types
- `feat`: New feature
- `fix`: Bug fix
- `docs`: Documentation changes
- `style`: Code style changes (formatting, etc.)
- `refactor`: Code refactoring
- `test`: Adding or modifying tests
- `chore`: Maintenance tasks

#### Examples
```
feat(order-entry): Add split payment functionality

- Implement split payment dialog
- Add payment method selection
- Update payment processing logic

Closes #123

fix(payment): Resolve credit card processing timeout

- Increase timeout to 30 seconds
- Add retry logic for failed transactions
- Update error handling

Fixes #456

docs(api): Update payment gateway documentation

- Add new API endpoints
- Update request/response examples
- Fix typos in authentication section
```

### Pull Request Process

#### PR Template
```markdown
## Description
Brief description of changes made.

## Type of Change
- [ ] Bug fix
- [ ] New feature
- [ ] Breaking change
- [ ] Documentation update

## Testing
- [ ] Unit tests pass
- [ ] Integration tests pass
- [ ] Manual testing completed
- [ ] Performance testing completed

## Checklist
- [ ] Code follows project style guidelines
- [ ] Self-review completed
- [ ] Documentation updated
- [ ] Tests added/updated
- [ ] No breaking changes (or documented)
```

#### Review Process
1. **Self-Review**: Review your own changes
2. **Peer Review**: Request review from team members
3. **Automated Checks**: Ensure CI/CD passes
4. **Approval**: Get required approvals
5. **Merge**: Merge to target branch

## Debugging Guidelines

### Logging Standards

#### Log Levels
```csharp
// Trace: Detailed information for debugging
_logger.LogTrace("Entering method {MethodName} with parameters {@Parameters}", nameof(GetTicket), parameters);

// Debug: Information useful for development
_logger.LogDebug("Ticket {TicketId} found with {OrderLineCount} order lines", ticket.Id, ticket.OrderLines.Count);

// Information: General information about application flow
_logger.LogInformation("Ticket {TicketId} created successfully", ticket.Id);

// Warning: Potential issues that don't stop the application
_logger.LogWarning("Ticket {TicketId} has no order lines", ticket.Id);

// Error: Errors that occur but don't crash the application
_logger.LogError(ex, "Failed to create ticket for table {TableId}", tableId);

// Critical: Serious errors that crash the application
_logger.LogCritical(ex, "Database connection failed");
```

#### Structured Logging
```csharp
// Use structured logging with parameters
_logger.LogInformation("Payment processed for ticket {TicketId} with amount {Amount:C}", ticketId, amount);

// Include correlation IDs
using var activity = Activity.StartActivity("ProcessPayment");
activity?.SetTag("ticket.id", ticketId.ToString());
activity?.SetTag("payment.amount", amount.ToString());

_logger.LogInformation("Payment processing started for ticket {TicketId}", ticketId);
```

### Debugging Techniques

#### Breakpoint Usage
```csharp
// Conditional breakpoints
if (ticket.TotalAmount > 1000) // Break only for large amounts

// Hit count breakpoints
// Break after 5 hits

// Tracepoints for logging without breaking
// Output message: $"Ticket {ticket.Id} processed"
```

#### Exception Debugging
```csharp
// Enable detailed exceptions in development
if (_environment.IsDevelopment())
{
    // Include stack traces and inner exceptions
    throw new ApplicationException("Detailed error message", ex);
}

// Use exception filters
try
{
    // Code that might throw
}
catch (Exception ex) when (ex is not BusinessRuleViolationException)
{
    // Handle non-business rule exceptions
}
```

## Performance Optimization

### Database Optimization

#### Query Optimization
```csharp
// Use AsNoTracking for read-only queries
var tickets = await _context.Tickets
    .AsNoTracking()
    .Where(t => t.Status == TicketStatus.Open)
    .ToListAsync();

// Use projections to reduce data transfer
var ticketDtos = await _context.Tickets
    .Where(t => t.Status == TicketStatus.Open)
    .Select(t => new TicketDto
    {
        Id = t.Id,
        TicketNumber = t.TicketNumber,
        TotalAmount = t.TotalAmount
    })
    .ToListAsync();

// Use proper indexing
// Add indexes to frequently queried columns
[Index(nameof(Status))]
[Index(nameof(CreatedAt))]
public class Ticket
{
    // Entity definition
}
```

#### Caching Strategies
```csharp
// Memory caching
public class TicketService
{
    private readonly IMemoryCache _cache;
    private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(15);

    public async Task<TicketDto> GetTicketAsync(Guid id)
    {
        var cacheKey = $"ticket:{id}";
        
        if (_cache.TryGetValue(cacheKey, out TicketDto cachedTicket))
        {
            return cachedTicket;
        }

        var ticket = await _repository.GetByIdAsync(id);
        var ticketDto = _mapper.Map<TicketDto>(ticket);
        
        _cache.Set(cacheKey, ticketDto, _cacheDuration);
        
        return ticketDto;
    }
}
```

### Application Performance

#### Async Programming
```csharp
// Use async/await properly
public async Task<List<TicketDto>> GetOpenTicketsAsync()
{
    // ConfigureAwait(false) in library code
    var tickets = await _repository.GetOpenTicketsAsync().ConfigureAwait(false);
    return _mapper.Map<List<TicketDto>>(tickets);
}

// Avoid async void (use async Task instead)
public async Task HandleButtonClick()
{
    try
    {
        await _ticketService.CreateTicketAsync(command);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Failed to create ticket");
    }
}
```

#### Memory Management
```csharp
// Dispose resources properly
public class TicketRepository : ITicketRepository, IDisposable
{
    private readonly MagideskDbContext _context;
    private bool _disposed = false;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _context?.Dispose();
            }
            _disposed = true;
        }
    }
}
```

## Security Guidelines

### Input Validation

#### Command Validation
```csharp
public class CreateTicketCommandValidator : AbstractValidator<CreateTicketCommand>
{
    public CreateTicketCommandValidator()
    {
        RuleFor(x => x.TableId)
            .NotEmpty()
            .WithMessage("Table ID is required");

        RuleFor(x => x.GuestCount)
            .GreaterThan(0)
            .LessThanOrEqualTo(20)
            .WithMessage("Guest count must be between 1 and 20");

        RuleFor(x => x.Items)
            .NotEmpty()
            .WithMessage("At least one item is required");

        RuleForEach(x => x.Items)
            .ChildRules(item =>
            {
                item.RuleFor(x => x.MenuItemId)
                    .NotEmpty()
                    .WithMessage("Menu item ID is required");

                item.RuleFor(x => x.Quantity)
                    .GreaterThan(0)
                    .WithMessage("Quantity must be greater than 0");

                item.RuleFor(x => x.UnitPrice)
                    .GreaterThanOrEqualTo(0)
                    .WithMessage("Unit price cannot be negative");
            });
    }
}
```

#### SQL Injection Prevention
```csharp
// Use parameterized queries
public async Task<List<Ticket>> GetTicketsByStatusAsync(TicketStatus status)
{
    var sql = "SELECT * FROM magidesk.Tickets WHERE Status = @status";
    var parameters = new { status = status.ToString() };
    
    return await _context.Tickets
        .FromSqlRaw(sql, parameters)
        .ToListAsync();
}

// Avoid string concatenation in SQL
// BAD: $"SELECT * FROM Tickets WHERE Status = '{status}'"
// GOOD: Use parameters as shown above
```

### Authentication and Authorization

#### JWT Implementation
```csharp
public class JwtTokenService : ITokenService
{
    public string GenerateToken(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
```

## Documentation Standards

### Code Documentation

#### XML Documentation
```csharp
/// <summary>
/// Creates a new ticket with the specified parameters.
/// </summary>
/// <param name="command">The command containing ticket creation parameters.</param>
/// <param name="cancellationToken">Cancellation token for the operation.</param>
/// <returns>
/// A task that represents the asynchronous operation.
/// The task result contains the created ticket information.
/// </returns>
/// <exception cref="ArgumentNullException">Thrown when command is null.</exception>
/// <exception cref="BusinessRuleViolationException">Thrown when business rules are violated.</exception>
/// <example>
/// <code>
/// var command = new CreateTicketCommand { TableId = Guid.NewGuid(), GuestCount = 4 };
/// var result = await ticketService.CreateTicketAsync(command);
/// </code>
/// </example>
public async Task<CreateTicketResult> CreateTicketAsync(CreateTicketCommand command, CancellationToken cancellationToken = default)
{
    // Implementation
}
```

### API Documentation

#### Swagger/OpenAPI
```csharp
/// <summary>
/// API controller for ticket management.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class TicketsController : ControllerBase
{
    /// <summary>
    /// Gets a ticket by ID.
    /// </summary>
    /// <param name="id">The ticket ID.</param>
    /// <returns>The ticket information.</returns>
    /// <response code="200">Returns the ticket.</response>
    /// <response code="404">Ticket not found.</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(TicketDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TicketDto>> GetTicket(Guid id)
    {
        // Implementation
    }
}
```

## Conclusion

This development guide provides comprehensive standards and procedures for contributing to the Magidesk POS system. Following these guidelines ensures:

- **Code Quality**: Consistent, maintainable, and readable code
- **Performance**: Optimized applications that meet performance requirements
- **Security**: Secure applications that protect sensitive data
- **Testing**: Thoroughly tested code with high coverage
- **Documentation**: Well-documented code that is easy to understand

All developers should familiarize themselves with these guidelines and follow them consistently throughout the development process.

---

*This development guide serves as the definitive reference for coding standards and best practices in the Magidesk POS system.*
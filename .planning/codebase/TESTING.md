# Testing Patterns

**Analysis Date:** 2026-03-23

## Test Framework

**Runner:**
- xUnit 2.5.3
- Config: Project files use `<IsTestProject>true</IsTestProject>` marker
- Tests located in projects: `Magidesk.Application.Tests`, `Magidesk.Domain.Tests`, `Magidesk.Infrastructure.Tests`, `Magidesk.Tests.E2E`, `Magidesk.Tests.Workflows`

**Assertion Library:**
- FluentAssertions 6.12.0 (primary) - used for expressive assertions
  - Example: `.Should().Be()`, `.Should().ContainSingle()`, `.Should().ThrowAsync<>()`
- Xunit built-in assertions (used alongside FluentAssertions)
  - Example: `Assert.True()`, `Assert.False()`, `Assert.Equal()`

**Run Commands:**
```bash
dotnet test                                    # Run all tests
dotnet test --watch                           # Watch mode
dotnet test /p:CollectCoverageMetrics=true    # Coverage (requires coverlet.collector)
```

**Coverage Tool:**
- coverlet.collector 6.0.0 (integrated in test projects)
- Enabled via `/p:CollectCoverageMetrics=true` flag

## Test File Organization

**Location:**
- Co-located with source projects in parallel `*.Tests` projects
- Test project structure mirrors source structure

**Naming:**
- Pattern: `{SubjectUnderTest}Tests.cs`
  - Example: `ApplyUpdateCommandHandlerTests.cs`, `AddOrderLineCommandHandlerTests.cs`
- Large test files split into partial classes
  - Example: `UpdateInventoryItemCommandHandlerTests.cs` (base) + `UpdateInventoryItemCommandHandlerTests.SkuValidation.cs`, `UpdateInventoryItemCommandHandlerTests.CategoryValidation.cs`, `UpdateInventoryItemCommandHandlerTests.Activation.cs`, `UpdateInventoryItemCommandHandlerTests.StockAdjustment.cs`

**Structure:**
```
src/
├── Magidesk.Application.Tests/
│   ├── Commands/
│   │   ├── ApplyUpdateCommandHandlerTests.cs
│   │   └── Inventory/
│   │       └── Handlers/
│   │           ├── CreateCategoryCommandHandlerTests.cs
│   │           ├── UpdateInventoryItemCommandHandlerTests.cs
│   │           ├── UpdateInventoryItemCommandHandlerTests.SkuValidation.cs
│   │           └── ... (other partial classes)
│   ├── Handlers/
│   │   ├── AddOrderLineCommandHandlerTests.cs
│   │   └── ... (other command handlers)
│   ├── Queries/
│   │   ├── CheckForUpdatesQueryHandlerTests.cs
│   │   └── ...
│   └── TestDoubles/
│       ├── InMemoryTicketRepository.cs
│       ├── StubKitchenRoutingService.cs
│       └── ... (test doubles)
├── Magidesk.Domain.Tests/
├── Magidesk.Infrastructure.Tests/
```

## Test Structure

**Suite Organization:**
```csharp
public class ApplyUpdateCommandHandlerTests
{
    private readonly Mock<IUpdateService> _updateService;
    private readonly ApplyUpdateCommandHandler _sut;  // System Under Test

    public ApplyUpdateCommandHandlerTests()
    {
        _updateService = new Mock<IUpdateService>();
        _sut = new ApplyUpdateCommandHandler(
            _updateService.Object,
            NullLogger<ApplyUpdateCommandHandler>.Instance);
    }

    [Fact]
    public async Task Handle_WhenDownloadAndApplySucceed_ReturnsSuccess()
    {
        // Arrange
        const string downloadUrl = "https://example.com/Magidesk-Setup.msi";
        const string installerPath = @"C:\Temp\Magidesk-Setup.msi";

        _updateService
            .Setup(s => s.DownloadInstallerAsync(downloadUrl, It.IsAny<IProgress<double>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(installerPath);

        // Act
        var result = await _sut.Handle(
            new ApplyUpdateCommand(downloadUrl, "Magidesk-Setup.msi"),
            CancellationToken.None);

        // Assert
        Assert.True(result.Success);
    }
}
```

**Patterns:**
- **Fixture setup:** Constructor initializes mocks and system-under-test (`_sut`)
  - Mocks injected via constructor
  - Use `NullLogger<T>.Instance` for loggers when not testing logging
- **Test method naming:** `{MethodName}_{Condition}_{ExpectedResult}`
  - Example: `Handle_WhenDownloadAndApplySucceed_ReturnsSuccess`
  - Example: `Handle_WhenDownloadThrows_ReturnsFailureWithMessage`
- **Arrange-Act-Assert:** Explicitly separated with comments
  - `// Arrange` - set up test data and mocks
  - `// Act` - invoke the method under test
  - `// Assert` - verify results
- **Test method attribute:** `[Fact]` for simple tests, `[Property]` or `[Theory]` with data (via FsCheck when used)
- **Async tests:** Use `async Task` for async test methods
- **Teardown:** No explicit teardown needed; fixtures are created per test

## Mocking

**Framework:** Moq 4.20.72

**Patterns:**
```csharp
// Create mock
private readonly Mock<IUpdateService> _updateService = new Mock<IUpdateService>();

// Setup return value
_updateService
    .Setup(s => s.DownloadInstallerAsync(downloadUrl, It.IsAny<IProgress<double>>(), It.IsAny<CancellationToken>()))
    .ReturnsAsync(installerPath);

// Setup exception
_updateService
    .Setup(s => s.DownloadInstallerAsync(downloadUrl, It.IsAny<IProgress<double>>(), It.IsAny<CancellationToken>()))
    .ThrowsAsync(new InvalidOperationException(errorMessage));

// Capture arguments
string? capturedUrl = null;
_updateService
    .Setup(s => s.DownloadInstallerAsync(It.IsAny<string>(), It.IsAny<IProgress<double>>(), It.IsAny<CancellationToken>()))
    .Callback<string, IProgress<double>, CancellationToken>((url, _, _) => capturedUrl = url)
    .ReturnsAsync(@"C:\Temp\installer.msi");

// Verify invocation
_mockItemRepository.Verify(
    x => x.UpdateAsync(existingItem, It.IsAny<CancellationToken>()),
    Times.Once);

_mockItemRepository.Verify(
    x => x.UpdateAsync(It.IsAny<InventoryItem>(), It.IsAny<CancellationToken>()),
    Times.Never,
    "Item should not be updated when it does not exist");
```

**What to Mock:**
- External service interfaces (e.g., `IUpdateService`, `ISecurityService`, `IKitchenRoutingService`)
- Repository interfaces when testing handlers/services that depend on them
- Database-dependent operations (always mock repositories in unit tests)
- Services with external I/O (network, file system)

**What NOT to Mock:**
- Domain entities - create real instances or use test doubles
- Value objects - create real instances
- Repository implementations in integration tests - use test doubles instead
- Loggers - use `NullLogger<T>.Instance`
- In-memory repositories - use actual implementations from TestDoubles

## Fixtures and Factories

**Test Data:**
```csharp
// Example: Real domain entity in test
var userId = new UserId(Guid.NewGuid());
var ticketNumber = await tickets.GetNextTicketNumberAsync();
var ticket = Ticket.Create(ticketNumber, userId, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
await tickets.AddAsync(ticket);

// Example: Test DTO
var cmd = new AddOrderLineCommand
{
    TicketId = ticket.Id,
    MenuItemId = Guid.NewGuid(),
    MenuItemName = "Burger",
    Quantity = 2m,
    UnitPrice = new Money(5m),
    TaxRate = 0.1m
};

// Example: Inventory item with factory
var existingItem = InventoryItem.Create(
    "Original Item",
    "kg",
    100m,
    20m,
    "ORIG-001",
    null);
```

**Location:**
- Test doubles: `src/Magidesk.Application.Tests/TestDoubles/`
  - In-memory repositories: `InMemory{EntityName}Repository.cs`
  - Stub services: `Stub{ServiceName}.cs`

**In-Memory Repository Pattern:**
```csharp
// File: src/Magidesk.Application.Tests/TestDoubles/InMemoryTicketRepository.cs
internal sealed class InMemoryTicketRepository : ITicketRepository
{
    private readonly Dictionary<Guid, Ticket> _tickets = new();
    private int _nextTicketNumber = 1;

    private sealed class NoOpTransaction : ITransaction
    {
        public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task RollbackAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public void Dispose() { }
    }

    public Task AddAsync(Ticket ticket, CancellationToken cancellationToken = default)
    {
        _tickets[ticket.Id] = ticket;
        return Task.CompletedTask;
    }

    public Task<Ticket?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _tickets.TryGetValue(id, out var ticket);
        return Task.FromResult(ticket);
    }

    // ... other methods
}
```

**Stub Service Pattern:**
```csharp
// File: src/Magidesk.Application.Tests/TestDoubles/StubKitchenRoutingService.cs
public class StubKitchenRoutingService : IKitchenRoutingService
{
    public Task<List<Guid>> RouteToKitchenAsync(TicketDto ticket, List<Guid>? itemIds = null)
    {
        return Task.FromResult(new List<Guid>());
    }

    public Task<bool> AutoRouteOrderLinesAsync(Guid ticketId, List<Guid> orderLineIds)
    {
        return Task.FromResult(true);
    }

    public bool ShouldAutoRoute(OrderLineDto orderLine) => true;
}
```

## Coverage

**Requirements:** Not enforced via configuration, but tests are comprehensive
- Unit tests cover happy path and error conditions
- Integration tests use in-memory repositories to test full flow

**View Coverage:**
```bash
dotnet test /p:CollectCoverageMetrics=true
```

## Test Types

**Unit Tests:**
- Scope: Individual command handlers, query handlers, domain entities
- Approach: Mocked dependencies, test one behavior per test method
- Location: `src/Magidesk.Application.Tests/Commands/`, `src/Magidesk.Application.Tests/Handlers/`, `src/Magidesk.Domain.Tests/`
- Example: `ApplyUpdateCommandHandlerTests.cs` - tests handler with mocked `IUpdateService`
- Example: `UpdateInventoryItemCommandHandlerTests.cs` - tests handler with mocked repositories

**Integration Tests:**
- Scope: Full command/query flow with real domain entities and in-memory repositories
- Approach: Uses test doubles (in-memory repositories) instead of mocks
- Location: `src/Magidesk.Application.Tests/Handlers/` (some tests use in-memory repos for integration testing)
- Example: `AddOrderLineCommandHandlerTests.cs` - uses `InMemoryTicketRepository`, `InMemoryMenuRepository`, creates real `Ticket` entities
- Pattern: Create test doubles directly, inject into handler, invoke handler, assert on side effects

**E2E Tests:**
- Framework: Not detailed in analysis; exists in `src/Magidesk.Tests.E2E/` project
- Scope: Full application flow including API layer
- Note: Not analyzed in detail; present but not primary test focus

**Domain Tests:**
- Scope: Domain entity invariants and value objects
- Location: `src/Magidesk.Domain.Tests/`
- Uses FluentAssertions and Moq for value object comparisons
- Example: Test domain factories, state transitions, value object operations

## Common Patterns

**Async Testing:**
```csharp
[Fact]
public async Task Handle_WhenDownloadThrows_ReturnsFailureWithMessage()
{
    // Arrange
    const string downloadUrl = "https://example.com/Magidesk-Setup.msi";
    const string errorMessage = "Network unavailable";

    _updateService
        .Setup(s => s.DownloadInstallerAsync(downloadUrl, It.IsAny<IProgress<double>>(), It.IsAny<CancellationToken>()))
        .ThrowsAsync(new InvalidOperationException(errorMessage));

    // Act
    var result = await _sut.Handle(
        new ApplyUpdateCommand(downloadUrl, "Magidesk-Setup.msi"),
        CancellationToken.None);

    // Assert
    Assert.False(result.Success);
    Assert.Equal(errorMessage, result.ErrorMessage);
}
```

**Error Testing:**
```csharp
// With FluentAssertions
[Fact]
public async Task HandleAsync_WithMissingTicket_ShouldThrow()
{
    // Arrange
    var tickets = new InMemoryTicketRepository();
    var cmd = new AddOrderLineCommand { TicketId = Guid.NewGuid(), /* ... */ };
    var handler = new AddOrderLineCommandHandler(tickets, /* ... */);

    // Act
    var act = async () => await handler.HandleAsync(cmd);

    // Assert
    await act.Should().ThrowAsync<Magidesk.Domain.Exceptions.BusinessRuleViolationException>();
}

// Capturing arguments for assertion
[Fact]
public async Task Handle_PassesCorrectDownloadUrlToService()
{
    const string expectedUrl = "https://releases.example.com/v0.2.0/Magidesk-Setup-x64.msi";
    string? capturedUrl = null;

    _updateService
        .Setup(s => s.DownloadInstallerAsync(It.IsAny<string>(), It.IsAny<IProgress<double>>(), It.IsAny<CancellationToken>()))
        .Callback<string, IProgress<double>, CancellationToken>((url, _, _) => capturedUrl = url)
        .ReturnsAsync(@"C:\Temp\installer.msi");

    // Act
    await _sut.Handle(
        new ApplyUpdateCommand(expectedUrl, "Magidesk-Setup-x64.msi"),
        CancellationToken.None);

    // Assert
    Assert.Equal(expectedUrl, capturedUrl);
}
```

**State Verification (Integration Pattern):**
```csharp
[Fact]
public async Task HandleAsync_ShouldAddOrderLine_AndWriteAuditEvent()
{
    // Arrange
    var tickets = new InMemoryTicketRepository();
    var audits = new InMemoryAuditEventRepository();
    var handler = new AddOrderLineCommandHandler(tickets, /* ... */, audits, /* ... */);

    var ticket = Ticket.Create(/* ... */);
    await tickets.AddAsync(ticket);

    var cmd = new AddOrderLineCommand { /* ... */ };

    // Act
    var result = await handler.HandleAsync(cmd);

    // Assert
    result.OrderLineId.Should().NotBe(Guid.Empty);

    var updated = await tickets.GetByIdAsync(ticket.Id);
    updated!.OrderLines.Should().ContainSingle(ol => ol.Id == result.OrderLineId);

    audits.Events.Should().HaveCount(1);
}
```

**Version Comparison Testing (Simple Logic):**
```csharp
[Fact]
public void IsNewer_WhenCandidateIsHigherMinor_ReturnsTrue()
{
    Assert.True(GithubUpdateService.IsNewer("0.2.0", "0.1.0"));
}

[Fact]
public void IsNewer_WhenVersionsAreEqual_ReturnsFalse()
{
    Assert.False(GithubUpdateService.IsNewer("0.1.0", "0.1.0"));
}

[Fact]
public void IsNewer_WhenCandidateIsInvalidVersion_ReturnsFalse()
{
    Assert.False(GithubUpdateService.IsNewer("not-a-version", "0.1.0"));
}
```

---

*Testing analysis: 2026-03-23*

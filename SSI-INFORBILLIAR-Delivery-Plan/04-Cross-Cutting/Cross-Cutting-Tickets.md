# Cross-Cutting Tickets

> [!NOTE]
> These tickets span multiple features and establish foundational infrastructure.

---

## Infrastructure & Architecture

| Ticket ID | Title | Priority | Status | Description |
|-----------|-------|----------|--------|-------------|
| CC-INFRA-01 | Create Database Migration for Session Entities | P0 | NOT_STARTED | Add TableSession, TableType migrations |
| CC-INFRA-02 | Create Database Migration for Reservation Entities | P0 | NOT_STARTED | Add Reservation, ClubSchedule migrations |
| CC-INFRA-03 | Create Database Migration for Customer/Member | P0 | NOT_STARTED | Add Customer, Member, MembershipTier migrations |
| CC-INFRA-04 | Register New Services in DI Container | P0 | NOT_STARTED | Register all new services in ConfigureServices |
| CC-INFRA-05 | Create Seed Data for Master Records | P1 | NOT_STARTED | Seed TableTypes, MembershipTiers, default schedule |

### CC-INFRA-01: Create Database Migration for Session Entities

**Priority:** P0

**Scope:**
- Create EF Core migration for:
  - TableSession
  - TableType
- Include foreign keys
- Include indexes

**Commands:**
```powershell
dotnet ef migrations add AddTableSessionEntities
dotnet ef database update
```

**Acceptance Criteria:**
- [ ] Migration creates tables correctly
- [ ] Foreign keys valid
- [ ] Indexes on frequently queried columns
- [ ] Can rollback cleanly

---

## Security

| Ticket ID | Title | Priority | Status | Description |
|-----------|-------|----------|--------|-------------|
| CC-SEC-01 | Implement Session-Based Authentication | P0 | NOT_STARTED | User login creates app session |
| CC-SEC-02 | Implement Auto-Logout Timer | P1 | NOT_STARTED | Logout after inactivity |
| CC-SEC-03 | Add Manager Authorization Service | P0 | NOT_STARTED | Central auth service for privileged ops |
| CC-SEC-04 | Audit Log for Sensitive Operations | P1 | NOT_STARTED | Log all manager overrides |

### CC-SEC-01: Implement Session-Based Authentication

**Priority:** P0

**Scope:**
- Create `ISessionService` interface
- Implement user session management
- Store current user in memory
- Validate session on navigation

**Implementation:**
```csharp
public interface ISessionService
{
    Task<LoginResult> Login(Guid userId, string pin);
    void Logout();
    User? CurrentUser { get; }
    bool IsLoggedIn { get; }
    bool HasPermission(string permission);
    event EventHandler<SessionChangedEventArgs> SessionChanged;
}
```

**Acceptance Criteria:**
- [ ] Login validates credentials
- [ ] CurrentUser populated on login
- [ ] Logout clears session
- [ ] App requires login to proceed
- [ ] Session persists through navigation

---

## Localization

| Ticket ID | Title | Priority | Status | Description |
|-----------|-------|----------|--------|-------------|
| CC-LOC-01 | Add Resource Strings for New Features | P1 | NOT_STARTED | Add localized strings for A, E, F features |
| CC-LOC-02 | Complete Missing Translations | P2 | NOT_STARTED | Ensure all supported languages have translations |

---

## Testing

| Ticket ID | Title | Priority | Status | Description |
|-----------|-------|----------|--------|-------------|
| CC-TEST-01 | Create Unit Tests for PricingService | P0 | NOT_STARTED | All pricing calculation scenarios |
| CC-TEST-02 | Create Unit Tests for ReservationService | P0 | NOT_STARTED | Availability and conflict detection |
| CC-TEST-03 | Create Integration Tests for Session Flow | P1 | NOT_STARTED | Start → Pause → Resume → End flow |
| CC-TEST-04 | Create UI Automation Tests for Core Flows | P2 | NOT_STARTED | Browser automation for critical paths |

### CC-TEST-01: Create Unit Tests for PricingService

**Priority:** P0

**Scope:**
- Test all pricing rules
- Test edge cases
- Test member discounts
- Achieve ≥90% coverage

**Test Cases:**
```csharp
[Theory]
[InlineData(30, 10.00, 5.00)]    // 30 min at $10/hr = $5
[InlineData(60, 10.00, 10.00)]   // 60 min at $10/hr = $10
[InlineData(90, 10.00, 15.00)]   // 90 min at $10/hr = $15
public void CalculateTimeCharge_ReturnsCorrectAmount(int minutes, decimal hourlyRate, decimal expected)
{
    // Arrange
    var service = new PricingService();
    var duration = TimeSpan.FromMinutes(minutes);
    var tableType = new TableTypeBuilder().WithHourlyRate(hourlyRate).Build();
    
    // Act
    var result = service.CalculateTimeCharge(duration, tableType);
    
    // Assert
    result.Amount.Should().Be(expected);
}
```

**Acceptance Criteria:**
- [ ] Basic calculation tests pass
- [ ] First-hour rate tests pass
- [ ] Rounding tests pass
- [ ] Minimum charge tests pass
- [ ] Member discount tests pass
- [ ] Coverage ≥90%

---

## Performance

| Ticket ID | Title | Priority | Status | Description |
|-----------|-------|----------|--------|-------------|
| CC-PERF-01 | Optimize Active Sessions Query | P1 | NOT_STARTED | Add indexes, optimize joins |
| CC-PERF-02 | Implement Timer Update Throttling | P1 | NOT_STARTED | Prevent UI thread blocking with many timers |

---

## Summary

| Category | Tickets | Priority Distribution |
|----------|---------|----------------------|
| Infrastructure | 5 | P0: 4, P1: 1 |
| Security | 4 | P0: 2, P1: 2 |
| Localization | 2 | P1: 1, P2: 1 |
| Testing | 4 | P0: 2, P1: 1, P2: 1 |
| Performance | 2 | P1: 2 |
| **Total** | **17** | **P0: 8, P1: 7, P2: 2** |

---

*Last Updated: 2026-01-08*

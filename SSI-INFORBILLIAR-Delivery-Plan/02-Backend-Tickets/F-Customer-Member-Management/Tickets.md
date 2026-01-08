# Backend Tickets: Category F - Customer & Member Management

> [!CAUTION]
> **CRITICAL GAP**: This entire module is NOT IMPLEMENTED (0% parity). All 13 features require full implementation from scratch. This is a P0 blocker for loyalty, memberships, and recurring revenue.

## Ticket Index

| Ticket ID | Feature ID | Title | Priority | Status |
|-----------|------------|-------|----------|--------|
| BE-F.1-01 | F.1 | Create Customer Entity and Repository | P0 | NOT_STARTED |
| BE-F.2-01 | F.2 | Create Customer Search Query | P0 | NOT_STARTED |
| BE-F.3-01 | F.3 | Create Member Entity with Subscription Support | P0 | NOT_STARTED |
| BE-F.4-01 | F.4 | Create MembershipTier Entity | P0 | NOT_STARTED |
| BE-F.5-01 | F.5 | Implement Member Discount Application | P0 | NOT_STARTED |
| BE-F.6-01 | F.6 | Create Credit/Prepaid Account System | P1 | NOT_STARTED |
| BE-F.7-01 | F.7 | Create Customer History Query | P1 | NOT_STARTED |
| BE-F.8-01 | F.8 | Create MembershipRenewalCommand | P1 | NOT_STARTED |
| BE-F.9-01 | F.9 | Create Guest Pass System | P2 | NOT_STARTED |
| BE-F.10-01 | F.10 | Create Member Check-In Command | P1 | NOT_STARTED |
| BE-F.11-01 | F.11 | Create Customer Notes System | P2 | NOT_STARTED |
| BE-F.12-01 | F.12 | Create Customer Merge Command | P2 | NOT_STARTED |
| BE-F.13-01 | F.13 | Create Member Analytics Query | P2 | NOT_STARTED |

---

## BE-F.1-01: Create Customer Entity and Repository

**Ticket ID:** BE-F.1-01  
**Feature ID:** F.1  
**Type:** Backend  
**Title:** Create Customer Entity and Repository  
**Priority:** P0

### Outcome (measurable, testable)
A fully implemented `Customer` domain entity for tracking club patrons.

### Scope
- Create `Customer.cs` entity in `Magidesk.Domain/Entities/`
- Create `ICustomerRepository` interface
- Create `CustomerRepository` implementation
- Create EF Core configuration
- Add migration for Customers table

### Explicitly Out of Scope
- Member-specific fields (separate entity)
- UI components
- Import functionality

### Implementation Notes
```csharp
public class Customer
{
    public Guid Id { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string FullName => $"{FirstName} {LastName}";
    public string Email { get; private set; }
    public string Phone { get; private set; }
    public DateTime? DateOfBirth { get; private set; }
    public string Address { get; private set; }
    public string City { get; private set; }
    public string PostalCode { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? LastVisitAt { get; private set; }
    public int TotalVisits { get; private set; }
    public Money TotalSpent { get; private set; }
    public bool IsActive { get; private set; }
    
    // Navigation
    public Member Member { get; private set; }
    public IReadOnlyCollection<CustomerNote> Notes { get; }
    
    // Domain methods
    public static Customer Create(string firstName, string lastName, string phone);
    public void UpdateContactInfo(string email, string phone, string address);
    public void RecordVisit(DateTime visitTime, Money spent);
    public void Deactivate();
    public void Reactivate();
}
```

### Quality & Guardrails
- **guardrails.md:** Rich domain model with behavior
- **domain-model.md:** Entity with invariants
- **testing-requirements.md:** ≥90% coverage
- **security-policies.md:** PII handling compliance

### Dependencies
| Type | Dependency | Ticket ID |
|------|------------|-----------|
| SOFT | Money value object | Exists |

### Acceptance Criteria
- [ ] Entity created with all properties
- [ ] Phone format validated
- [ ] Email format validated (if provided)
- [ ] RecordVisit updates TotalVisits and TotalSpent
- [ ] Repository interface created
- [ ] Repository implementation created
- [ ] EF Core configuration with indexes
- [ ] Migration runs successfully
- [ ] Unit tests ≥90% coverage

### Failure Modes to Guard Against
- Invalid phone/email format accepted
- Duplicate customer creation
- PII exposure in logs

---

## BE-F.2-01: Create Customer Search Query

**Ticket ID:** BE-F.2-01  
**Feature ID:** F.2  
**Type:** Backend  
**Title:** Create Customer Search Query  
**Priority:** P0

### Outcome (measurable, testable)
A fast, fuzzy-matching customer search for quick lookup at POS.

### Scope
- Create `SearchCustomersQuery` with multi-field search
- Search by name, phone, email
- Support partial matching
- Return paginated results

### Implementation Notes
```csharp
public record SearchCustomersQuery(
    string SearchTerm,
    int PageNumber = 1,
    int PageSize = 20
);

public record CustomerSearchResultDto(
    Guid Id,
    string FullName,
    string Phone,
    string Email,
    bool IsMember,
    string MembershipTier,
    int TotalVisits
);

// Handler uses LIKE or Full-Text search
// Orders by relevance (exact match first)
```

### Dependencies
| Type | Dependency | Ticket ID |
|------|------------|-----------|
| HARD | Customer entity | BE-F.1-01 |

### Acceptance Criteria
- [ ] Search returns matching customers
- [ ] Partial name matching works
- [ ] Phone search works (with/without formatting)
- [ ] Results paginated
- [ ] Performance < 100ms for 10k customers
- [ ] Member status included in results

---

## BE-F.3-01: Create Member Entity with Subscription Support

**Ticket ID:** BE-F.3-01  
**Feature ID:** F.3  
**Type:** Backend  
**Title:** Create Member Entity with Subscription Support  
**Priority:** P0

### Outcome (measurable, testable)
A `Member` entity that extends Customer with subscription/membership capabilities.

### Scope
- Create `Member.cs` entity (links to Customer)
- Create `MembershipStatus` enumeration
- Track membership dates, tier, renewal
- Create repository
- Add migration

### Explicitly Out of Scope
- Automatic renewal processing
- Payment integration
- Tier upgrade logic (separate ticket)

### Implementation Notes
```csharp
public class Member
{
    public Guid Id { get; private set; }
    public Guid CustomerId { get; private set; }
    public Guid TierId { get; private set; }
    public string MemberNumber { get; private set; }
    public DateTime JoinDate { get; private set; }
    public DateTime? ExpirationDate { get; private set; }
    public MembershipStatus Status { get; private set; }
    public decimal DiscountPercent { get; private set; }
    public Money PrepaidBalance { get; private set; }
    
    // Navigation
    public Customer Customer { get; private set; }
    public MembershipTier Tier { get; private set; }
    
    // Domain methods
    public static Member Create(Guid customerId, Guid tierId, string memberNumber);
    public void Renew(DateTime newExpirationDate);
    public void Suspend(string reason);
    public void Reactivate();
    public void UpgradeTier(Guid newTierId, decimal newDiscountPercent);
    public bool IsActive => Status == MembershipStatus.Active && 
                           (ExpirationDate == null || ExpirationDate > DateTime.UtcNow);
}

public enum MembershipStatus
{
    Active = 0,
    Expired = 1,
    Suspended = 2,
    Cancelled = 3
}
```

### Quality & Guardrails
- **domain-model.md:** State machine for status
- **guardrails.md:** Business logic in entity

### Dependencies
| Type | Dependency | Ticket ID |
|------|------------|-----------|
| HARD | Customer entity | BE-F.1-01 |
| HARD | MembershipTier entity | BE-F.4-01 |

### Acceptance Criteria
- [ ] Entity links to Customer
- [ ] MemberNumber is unique
- [ ] Status transitions validated
- [ ] IsActive property correct
- [ ] PrepaidBalance tracked
- [ ] Repository created
- [ ] Migration successful
- [ ] Tests ≥90%

---

## BE-F.4-01: Create MembershipTier Entity

**Ticket ID:** BE-F.4-01  
**Feature ID:** F.4  
**Type:** Backend  
**Title:** Create MembershipTier Entity  
**Priority:** P0

### Outcome (measurable, testable)
A `MembershipTier` entity defining different membership levels with benefits.

### Scope
- Create `MembershipTier.cs` entity
- Define tier benefits (discount %, hourly rate discount)
- Create repository
- Seed default tiers

### Implementation Notes
```csharp
public class MembershipTier
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public decimal DiscountPercent { get; private set; }
    public decimal? HourlyRateDiscount { get; private set; }
    public bool IncludesFreeGuests { get; private set; }
    public int FreeGuestsPerVisit { get; private set; }
    public Money MonthlyFee { get; private set; }
    public Money AnnualFee { get; private set; }
    public int SortOrder { get; private set; }
    public bool IsActive { get; private set; }
    
    // Domain methods
    public static MembershipTier Create(string name, decimal discountPercent, Money monthlyFee);
    public Money CalculateMemberPrice(Money regularPrice);
    public decimal GetEffectiveHourlyRate(decimal baseRate);
}
```

### Dependencies
| Type | Dependency | Ticket ID |
|------|------------|-----------|
| SOFT | Money value object | Exists |

### Acceptance Criteria
- [ ] Entity with all benefit properties
- [ ] DiscountPercent 0-100 validated
- [ ] CalculateMemberPrice works correctly
- [ ] Default tiers seeded (Bronze, Silver, Gold)
- [ ] Repository created
- [ ] Tests ≥90%

---

## BE-F.5-01: Implement Member Discount Application

**Ticket ID:** BE-F.5-01  
**Feature ID:** F.5  
**Type:** Backend  
**Title:** Implement Member Discount Application  
**Priority:** P0

### Outcome (measurable, testable)
A service that applies member discounts to tickets and time charges.

### Scope
- Create `IMemberDiscountService` interface
- Implement automatic discount application
- Integrate with PricingService for time charges
- Integrate with Ticket for product discounts

### Implementation Notes
```csharp
public interface IMemberDiscountService
{
    Money ApplyMemberDiscount(Money price, Member member);
    decimal GetMemberHourlyRate(decimal baseRate, Member member);
    DiscountInfo GetMemberDiscountInfo(Member member);
}

public record DiscountInfo(
    decimal DiscountPercent,
    decimal? HourlyRateDiscount,
    string TierName
);

// Integration points:
// 1. PricingService calls GetMemberHourlyRate
// 2. Ticket calls ApplyMemberDiscount for products
```

### Dependencies
| Type | Dependency | Ticket ID |
|------|------------|-----------|
| HARD | Member entity | BE-F.3-01 |
| HARD | MembershipTier entity | BE-F.4-01 |
| SOFT | PricingService | BE-A.9-01 |

### Acceptance Criteria
- [ ] Discount applied correctly to price
- [ ] Hourly rate discount works
- [ ] Expired member gets no discount
- [ ] Suspended member gets no discount
- [ ] Discount info returned for UI display
- [ ] Integration with PricingService works

---

## BE-F.6-01: Create Credit/Prepaid Account System

**Ticket ID:** BE-F.6-01  
**Feature ID:** F.6  
**Type:** Backend  
**Title:** Create Credit/Prepaid Account System  
**Priority:** P1

### Outcome (measurable, testable)
A system for managing member prepaid balances and house accounts.

### Scope
- Create `PrepaidTransaction` entity
- Create `AddPrepaidCreditCommand`
- Create `DeductPrepaidCreditCommand`
- Create `GetPrepaidBalanceQuery`
- Integrate as payment method

### Implementation Notes
```csharp
public class PrepaidTransaction
{
    public Guid Id { get; private set; }
    public Guid MemberId { get; private set; }
    public Money Amount { get; private set; }
    public PrepaidTransactionType Type { get; private set; }
    public Guid? TicketId { get; private set; }
    public string Description { get; private set; }
    public DateTime TransactionDate { get; private set; }
    public Money BalanceAfter { get; private set; }
}

public enum PrepaidTransactionType
{
    Deposit,
    Charge,
    Refund,
    Adjustment
}
```

### Dependencies
| Type | Dependency | Ticket ID |
|------|------------|-----------|
| HARD | Member entity | BE-F.3-01 |

### Acceptance Criteria
- [ ] Can add credit to member
- [ ] Can deduct credit for payment
- [ ] Balance never goes negative (without override)
- [ ] Transaction history tracked
- [ ] Can integrate as payment type
- [ ] Tests ≥80%

---

## BE-F.7-01: Create Customer History Query

**Ticket ID:** BE-F.7-01  
**Feature ID:** F.7  
**Type:** Backend  
**Title:** Create Customer History Query  
**Priority:** P1

### Outcome (measurable, testable)
A query that retrieves complete customer visit and purchase history.

### Scope
- Create `GetCustomerHistoryQuery`
- Include visits, purchases, reservations
- Support date range filtering
- Return summary statistics

### Implementation Notes
```csharp
public record GetCustomerHistoryQuery(
    Guid CustomerId,
    DateTime? StartDate,
    DateTime? EndDate
);

public record CustomerHistoryDto(
    CustomerSummary Summary,
    IEnumerable<VisitRecord> Visits,
    IEnumerable<PurchaseRecord> Purchases,
    IEnumerable<ReservationRecord> Reservations
);

public record CustomerSummary(
    int TotalVisits,
    Money TotalSpent,
    TimeSpan TotalPlayTime,
    Money AverageSpendPerVisit,
    DateTime? LastVisit,
    string FavoriteTable
);
```

### Dependencies
| Type | Dependency | Ticket ID |
|------|------------|-----------|
| HARD | Customer entity | BE-F.1-01 |
| SOFT | Reservation entity | BE-E.1-01 |
| SOFT | TableSession entity | BE-A.1-01 |

### Acceptance Criteria
- [ ] Returns complete visit history
- [ ] Returns purchase history
- [ ] Returns reservation history
- [ ] Summary statistics correct
- [ ] Date filtering works
- [ ] Performance acceptable

---

## BE-F.10-01: Create Member Check-In Command

**Ticket ID:** BE-F.10-01  
**Feature ID:** F.10  
**Type:** Backend  
**Title:** Create Member Check-In Command  
**Priority:** P1

### Outcome (measurable, testable)
A quick check-in system for members arriving at the club.

### Scope
- Create `CheckInMemberCommand`
- Validate membership status
- Record visit
- Display member info and balance

### Implementation Notes
```csharp
public record CheckInMemberCommand(
    Guid MemberId
);

public record CheckInResult(
    bool Success,
    string MemberName,
    string TierName,
    decimal DiscountPercent,
    Money PrepaidBalance,
    bool HasReservation,
    Guid? ReservationId,
    string Message
);

// Handler:
// 1. Validate member is active
// 2. Record visit on Customer
// 3. Check for today's reservation
// 4. Return member info
```

### Dependencies
| Type | Dependency | Ticket ID |
|------|------------|-----------|
| HARD | Member entity | BE-F.3-01 |
| SOFT | Reservation entity | BE-E.1-01 |

### Acceptance Criteria
- [ ] Active members can check in
- [ ] Visit recorded
- [ ] Reservation detected if exists
- [ ] Expired members show warning
- [ ] Suspended members blocked
- [ ] PrepaidBalance displayed

---

## Summary

| Priority | Count | Status |
|----------|-------|--------|
| P0 | 5 | NOT_STARTED |
| P1 | 4 | NOT_STARTED |
| P2 | 4 | NOT_STARTED |
| **Total** | **13** | **NOT_STARTED** |

---

*Last Updated: 2026-01-08*

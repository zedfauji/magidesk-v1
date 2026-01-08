# Backend Architecture Analysis

## 1. Architecture Overview

The Magidesk backend follows **Clean Architecture** principles with clear separation of concerns:

```
┌─────────────────────────────────────────────────────────────┐
│                    Presentation Layer                        │
│              (WinUI 3 Views & ViewModels)                   │
├─────────────────────────────────────────────────────────────┤
│                    Application Layer                         │
│           (Commands, Queries, DTOs, Handlers)               │
├─────────────────────────────────────────────────────────────┤
│                      Domain Layer                            │
│        (Entities, Value Objects, Enumerations)              │
├─────────────────────────────────────────────────────────────┤
│                   Infrastructure Layer                       │
│      (Repositories, Services, Database, Printing)           │
└─────────────────────────────────────────────────────────────┘
```

### Projects Structure

| Project | Purpose | Status |
|---------|---------|--------|
| `Magidesk.Domain` | Entities, Value Objects, Business Rules | ✅ Solid |
| `Magidesk.Application` | Use Cases, CQRS Handlers | ✅ Solid |
| `Magidesk.Infrastructure` | Data Access, External Services | ✅ Solid |
| `Magidesk.Presentation` | WinUI 3 Views, ViewModels | ⚠️ Gaps |
| `Magidesk.Api` | REST API Controllers | ✅ Available |
| `Magidesk.Migrations` | EF Core Migrations | ✅ Solid |

---

## 2. Domain Layer Analysis

### 2.1 Core Entities Present

| Entity | Description | Completeness |
|--------|-------------|--------------|
| `Ticket` | Order aggregate root | ✅ Complete |
| `OrderLine` | Line items with modifiers | ✅ Complete |
| `Payment` | TPH hierarchy (Cash, Card, Gift) | ✅ Complete |
| `Table` | Physical table representation | ⚠️ Missing time tracking |
| `Floor` | Dining area/room | ✅ Complete |
| `TableLayout` | Table arrangement per floor | ✅ Complete |
| `MenuItem` | Product catalog entry | ✅ Complete |
| `MenuCategory` | Product categorization | ✅ Complete |
| `User` | Staff/operator accounts | ✅ Complete |
| `Role` | Permission groups | ✅ Complete |
| `Shift` | Work schedule periods | ✅ Complete |
| `CashSession` | Cash drawer sessions | ✅ Complete |
| `Discount` | Promotional discounts | ✅ Complete |
| `InventoryItem` | Stock tracking | ⚠️ Basic only |

### 2.2 Missing Critical Entities

> [!CAUTION]
> The following domain entities are **entirely missing** and represent critical architectural gaps:

| Missing Entity | Impact | Priority |
|----------------|--------|----------|
| `Customer` | Cannot track customer relationships | **P0** |
| `Member` | Cannot manage club memberships | **P0** |
| `MembershipPlan` | Cannot offer membership tiers | **P0** |
| `Reservation` | Cannot book tables in advance | **P0** |
| `TableSession` | Cannot track time-based billing | **P0** |
| `HourBank` | Cannot offer prepaid hours | **P1** |
| `Supplier` | Cannot manage vendors | **P2** |

### 2.3 Entity Gap Details

#### Customer Entity (Required)
```csharp
// PROPOSED: Customer.cs
public class Customer
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string? Phone { get; private set; }
    public string? Email { get; private set; }
    public string? BarcodeId { get; private set; }
    public byte[]? Photo { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
}
```

#### TableSession Entity (Required)
```csharp
// PROPOSED: TableSession.cs
public class TableSession
{
    public Guid Id { get; private set; }
    public Guid TableId { get; private set; }
    public Guid? CustomerId { get; private set; }
    public DateTime StartTime { get; private set; }
    public DateTime? EndTime { get; private set; }
    public DateTime? PausedAt { get; private set; }
    public TimeSpan TotalPausedDuration { get; private set; }
    public TableSessionStatus Status { get; private set; }
    public Money TotalCharge { get; private set; }
    public Guid? TicketId { get; private set; }
}
```

#### Reservation Entity (Required)
```csharp
// PROPOSED: Reservation.cs
public class Reservation
{
    public Guid Id { get; private set; }
    public Guid TableId { get; private set; }
    public Guid? CustomerId { get; private set; }
    public string GuestName { get; private set; }
    public string GuestPhone { get; private set; }
    public DateTime ReservedFrom { get; private set; }
    public DateTime ReservedTo { get; private set; }
    public string? Notes { get; private set; }
    public ReservationStatus Status { get; private set; }
}
```

---

## 3. Application Layer Analysis

### 3.1 Command/Query Coverage

| Domain Area | Commands | Queries | Status |
|-------------|----------|---------|--------|
| Tickets | ✅ Create, Update, Close, Void | ✅ Get, List | Complete |
| Payments | ✅ Process, Refund | ✅ Get | Complete |
| Menu | ✅ CRUD | ✅ Get, List | Complete |
| Tables | ⚠️ Seat/Release only | ✅ Get, List | Gaps |
| Users | ✅ CRUD, Auth | ✅ Get, List | Complete |
| Reports | ⚠️ Basic | ✅ Multiple | Needs export |
| Reservations | ❌ None | ❌ None | **Missing** |
| Customers | ❌ None | ❌ None | **Missing** |
| TableSessions | ❌ None | ❌ None | **Missing** |

### 3.2 Missing Commands (Critical)

```csharp
// Required for billiard club operation:
StartTableSessionCommand
PauseTableSessionCommand
ResumeTableSessionCommand
EndTableSessionCommand
AdjustTableSessionTimeCommand (manager override)

CreateReservationCommand
UpdateReservationCommand
CancelReservationCommand
CheckInReservationCommand
MarkNoShowCommand

CreateCustomerCommand
UpdateCustomerCommand
LinkCustomerToTicketCommand
ScanMemberBarcodeCommand
```

---

## 4. Infrastructure Layer Analysis

### 4.1 Services Present

| Service | Purpose | Status |
|---------|---------|--------|
| `PrintingService` | Receipt printing | ✅ Complete |
| `KitchenPrintService` | Kitchen ticket routing | ✅ Complete |
| `WindowsPrintingService` | Raw ESC/POS | ✅ Complete |
| `CashDrawerService` | Drawer control | ✅ Complete |
| `AuthenticationService` | User login | ✅ Complete |
| `EncryptionService` | PIN hashing | ✅ Complete |

### 4.2 Services Missing

| Missing Service | Purpose | Priority |
|-----------------|---------|----------|
| `PricingService` | Time-based billing calculation | **P0** |
| `ReservationService` | Conflict detection, reminders | **P0** |
| `RelayControlService` | Lamp/equipment control | **P1** |
| `ExportService` | PDF/Excel generation | **P1** |
| `BackupService` | Database backup/restore | **P1** |
| `NotificationService` | Low stock, reservation alerts | **P2** |

---

## 5. Database Schema Gaps

### 5.1 Current Tables
- ✅ Tickets, OrderLines, OrderLineModifiers
- ✅ Payments (TPH)
- ✅ Tables, Floors, TableLayouts
- ✅ MenuItems, MenuCategories, MenuGroups
- ✅ Users, Roles
- ✅ Shifts, CashSessions
- ✅ InventoryItems, InventoryAdjustments
- ✅ Discounts
- ✅ PrinterGroups, PrinterMappings
- ✅ Terminals

### 5.2 Tables to Add

```sql
-- Customer & Member Management
CREATE TABLE Customers (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Phone NVARCHAR(20),
    Email NVARCHAR(100),
    BarcodeId NVARCHAR(50),
    Photo VARBINARY(MAX),
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL
);

CREATE TABLE MembershipPlans (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500),
    MonthlyFee DECIMAL(18,2),
    DiscountPercent DECIMAL(5,2),
    HourlyRateDiscount DECIMAL(5,2),
    IsActive BIT DEFAULT 1
);

CREATE TABLE Memberships (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    CustomerId UNIQUEIDENTIFIER FOREIGN KEY,
    PlanId UNIQUEIDENTIFIER FOREIGN KEY,
    StartDate DATE NOT NULL,
    ExpiryDate DATE,
    Status NVARCHAR(20),
    HourBankBalance DECIMAL(10,2) DEFAULT 0
);

-- Reservations
CREATE TABLE Reservations (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    TableId UNIQUEIDENTIFIER FOREIGN KEY,
    CustomerId UNIQUEIDENTIFIER FOREIGN KEY,
    GuestName NVARCHAR(100),
    GuestPhone NVARCHAR(20),
    ReservedFrom DATETIME2 NOT NULL,
    ReservedTo DATETIME2 NOT NULL,
    Notes NVARCHAR(500),
    Status NVARCHAR(20) NOT NULL,
    CreatedAt DATETIME2 NOT NULL
);

-- Table Sessions (Time Tracking)
CREATE TABLE TableSessions (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    TableId UNIQUEIDENTIFIER FOREIGN KEY,
    CustomerId UNIQUEIDENTIFIER FOREIGN KEY,
    TicketId UNIQUEIDENTIFIER FOREIGN KEY,
    StartTime DATETIME2 NOT NULL,
    EndTime DATETIME2,
    PausedAt DATETIME2,
    TotalPausedMinutes INT DEFAULT 0,
    Status NVARCHAR(20) NOT NULL,
    HourlyRate DECIMAL(18,2),
    TotalCharge DECIMAL(18,2)
);

-- Table Type Pricing
CREATE TABLE TableTypes (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    Name NVARCHAR(50) NOT NULL,
    HourlyRate DECIMAL(18,2) NOT NULL,
    FirstHourRate DECIMAL(18,2),
    MinimumMinutes INT DEFAULT 30,
    RoundingMinutes INT DEFAULT 15
);
```

---

## 6. Recommendations

### 6.1 Immediate Actions (P0)

1. **Create Domain Entities**
   - Add `Customer`, `Member`, `MembershipPlan` entities
   - Add `Reservation` entity with conflict detection
   - Add `TableSession` entity with time tracking
   - Add `TableType` with pricing rules

2. **Extend Table Entity**
   - Add `TableTypeId` foreign key
   - Add `CurrentSessionId` for active session tracking

3. **Create Commands/Queries**
   - Full CQRS for TableSession lifecycle
   - Full CQRS for Reservation management
   - Customer/Member operations

### 6.2 Secondary Actions (P1)

4. **Create PricingService**
   - Calculate time-based charges
   - Apply first-hour rules
   - Handle rounding rules
   - Member discount application

5. **Create ExportService**
   - PDF generation (QuestPDF or similar)
   - Excel generation (EPPlus)
   - Report formatting

6. **Create BackupService**
   - Database backup to file
   - Restore capability
   - Scheduled backup support

### 6.3 Tertiary Actions (P2)

7. **Hardware Integration**
   - Relay control service
   - Serial communication for legacy hardware

8. **Notification Service**
   - Low stock alerts
   - Reservation reminders
   - Membership expiry warnings

---

## 7. Conclusion

The backend architecture is **well-designed and follows best practices**. The Clean Architecture pattern provides excellent separation of concerns and testability. The primary issue is **missing domain entities and services** for billiard-club-specific functionality rather than architectural flaws.

**Estimated effort to address gaps:**
- P0 Entity Creation: 1-2 weeks
- P0 Service Implementation: 2-3 weeks
- P1 Features: 2-3 weeks
- P2 Features: 1-2 weeks

**Total: 6-10 weeks of backend development**

---

*End of Backend Architecture Analysis*

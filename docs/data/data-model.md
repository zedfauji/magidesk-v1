# Data Model

## Overview

The Magidesk POS system uses a comprehensive data model designed around Clean Architecture principles with PostgreSQL as the primary database. This document details the complete data structure, relationships, and constraints that support all business operations.

## Database Architecture

### Schema Organization

The database uses a dedicated `magidesk` schema to isolate all POS data:

```sql
-- Primary schema for all POS data
CREATE SCHEMA magidesk;

-- Schema organization by functional area
magidesk.tickets          -- Order management
magidesk.payments         -- Financial transactions
magidesk.menu             -- Menu and inventory
magidesk.users            -- User management
magidesk.shifts           -- Shift and session management
magidesk.reports          -- Reporting and analytics
magidesk.audit            -- Audit trail and logging
```

### Naming Conventions

- **Tables**: PascalCase, plural nouns (e.g., `Tickets`, `OrderLines`)
- **Columns**: PascalCase, descriptive names (e.g., `TicketNumber`, `CreatedAt`)
- **Indexes**: `IX_TableName_ColumnName` format
- **Foreign Keys**: `FK_TableName_ColumnName_ReferenceTable` format
- **Constraints**: `CK_TableName_RuleName` format for check constraints

## Core Entities

### Ticket Aggregate

The `Ticket` entity is the central aggregate root representing customer orders:

```sql
CREATE TABLE magidesk.Tickets (
    Id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    TicketNumber BIGINT NOT NULL UNIQUE,
    Status VARCHAR(20) NOT NULL CHECK (Status IN ('Draft', 'Open', 'Paid', 'Closed', 'Refunded', 'Voided')),
    CreatedAt TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    OpenedAt TIMESTAMP WITH TIME ZONE,
    ClosedAt TIMESTAMP WITH TIME ZONE,
    CreatedBy UUID NOT NULL REFERENCES magidesk.Users(Id),
    ClosedBy UUID REFERENCES magidesk.Users(Id),
    VoidedBy UUID REFERENCES magidesk.Users(Id),
    ActiveDate DATE NOT NULL DEFAULT CURRENT_DATE,
    TableId UUID REFERENCES magidesk.Tables(Id),
    GuestCount INTEGER NOT NULL DEFAULT 1 CHECK (GuestCount > 0),
    OrderType VARCHAR(20) NOT NULL DEFAULT 'DineIn' CHECK (OrderType IN ('DineIn', 'Takeout', 'Delivery', 'BarTab')),
    CustomerId UUID REFERENCES magidesk.Customers(Id),
    SubtotalAmount DECIMAL(12,2) NOT NULL DEFAULT 0.00 CHECK (SubtotalAmount >= 0),
    DiscountAmount DECIMAL(12,2) NOT NULL DEFAULT 0.00 CHECK (DiscountAmount >= 0),
    TaxAmount DECIMAL(12,2) NOT NULL DEFAULT 0.00 CHECK (TaxAmount >= 0),
    ServiceChargeAmount DECIMAL(12,2) NOT NULL DEFAULT 0.00 CHECK (ServiceChargeAmount >= 0),
    DeliveryChargeAmount DECIMAL(12,2) NOT NULL DEFAULT 0.00 CHECK (DeliveryChargeAmount >= 0),
    AdjustmentAmount DECIMAL(12,2) NOT NULL DEFAULT 0.00,
    TotalAmount DECIMAL(12,2) NOT NULL DEFAULT 0.00 CHECK (TotalAmount >= 0),
    PaidAmount DECIMAL(12,2) NOT NULL DEFAULT 0.00 CHECK (PaidAmount >= 0),
    DueAmount DECIMAL(12,2) NOT NULL DEFAULT 0.00,
    AdvanceAmount DECIMAL(12,2) NOT NULL DEFAULT 0.00 CHECK (AdvanceAmount >= 0),
    IsTaxExempt BOOLEAN NOT NULL DEFAULT FALSE,
    PriceIncludesTax BOOLEAN NOT NULL DEFAULT FALSE,
    IsBarTab BOOLEAN NOT NULL DEFAULT FALSE,
    IsReOpened BOOLEAN NOT NULL DEFAULT FALSE,
    Notes TEXT,
    Version INTEGER NOT NULL DEFAULT 1,
    CreatedAt TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    UpdatedAt TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

-- Indexes for performance
CREATE INDEX IX_Tickets_TicketNumber ON magidesk.Tickets(TicketNumber);
CREATE INDEX IX_Tickets_Status ON magidesk.Tickets(Status);
CREATE INDEX IX_Tickets_ActiveDate ON magidesk.Tickets(ActiveDate);
CREATE INDEX IX_Tickets_CreatedBy ON magidesk.Tickets(CreatedBy);
CREATE INDEX IX_Tickets_TableId ON magidesk.Tickets(TableId);
```

### Order Lines

```sql
CREATE TABLE magidesk.OrderLines (
    Id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    TicketId UUID NOT NULL REFERENCES magidesk.Tickets(Id) ON DELETE CASCADE,
    MenuItemId UUID NOT NULL REFERENCES magidesk.MenuItems(Id),
    Quantity INTEGER NOT NULL CHECK (Quantity > 0),
    UnitPrice DECIMAL(10,2) NOT NULL CHECK (UnitPrice >= 0),
    TotalPrice DECIMAL(12,2) NOT NULL CHECK (TotalPrice >= 0),
    SeatNumber INTEGER,
    SortOrder INTEGER NOT NULL DEFAULT 0,
    Status VARCHAR(20) NOT NULL DEFAULT 'Ordered' CHECK (Status IN ('Ordered', 'Sent', 'Preparing', 'Ready', 'Completed', 'Voided')),
    SentToKitchenAt TIMESTAMP WITH TIME ZONE,
    CompletedAt TIMESTAMP WITH TIME ZONE,
    Notes TEXT,
    Version INTEGER NOT NULL DEFAULT 1,
    CreatedAt TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    UpdatedAt TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

CREATE INDEX IX_OrderLines_TicketId ON magidesk.OrderLines(TicketId);
CREATE INDEX IX_OrderLines_MenuItemId ON magidesk.OrderLines(MenuItemId);
CREATE INDEX IX_OrderLines_Status ON magidesk.OrderLines(Status);
```

### Order Line Modifiers

```sql
CREATE TABLE magidesk.OrderLineModifiers (
    Id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    OrderLineId UUID NOT NULL REFERENCES magidesk.OrderLines(Id) ON DELETE CASCADE,
    ModifierId UUID NOT NULL REFERENCES magidesk.Modifiers(Id),
    Quantity INTEGER NOT NULL DEFAULT 1 CHECK (Quantity > 0),
    UnitPrice DECIMAL(10,2) NOT NULL CHECK (UnitPrice >= 0),
    TotalPrice DECIMAL(12,2) NOT NULL CHECK (TotalPrice >= 0),
    CreatedAt TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

CREATE INDEX IX_OrderLineModifiers_OrderLineId ON magidesk.OrderLineModifiers(OrderLineId);
CREATE INDEX IX_OrderLineModifiers_ModifierId ON magidesk.OrderLineModifiers(ModifierId);
```

## Payment System

### Payments

```sql
CREATE TABLE magidesk.Payments (
    Id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    TicketId UUID NOT NULL REFERENCES magidesk.Tickets(Id) ON DELETE CASCADE,
    PaymentType VARCHAR(20) NOT NULL CHECK (PaymentType IN ('Cash', 'CreditCard', 'DebitCard', 'GiftCertificate', 'MobilePayment', 'Check')),
    Amount DECIMAL(12,2) NOT NULL CHECK (Amount > 0),
    Status VARCHAR(20) NOT NULL DEFAULT 'Pending' CHECK (Status IN ('Pending', 'Authorized', 'Captured', 'Completed', 'Declined', 'Refunded', 'Voided')),
    ReferenceNumber VARCHAR(100),
    AuthorizationCode VARCHAR(100),
    TransactionId VARCHAR(100),
    ProcessorResponse TEXT,
    ProcessedAt TIMESTAMP WITH TIME ZONE,
    RefundedAt TIMESTAMP WITH TIME ZONE,
    RefundAmount DECIMAL(12,2) CHECK (RefundAmount >= 0),
    RefundReason TEXT,
    CreatedBy UUID NOT NULL REFERENCES magidesk.Users(Id),
    ProcessedBy UUID REFERENCES magidesk.Users(Id),
    Version INTEGER NOT NULL DEFAULT 1,
    CreatedAt TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    UpdatedAt TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

CREATE INDEX IX_Payments_TicketId ON magidesk.Payments(TicketId);
CREATE INDEX IX_Payments_Status ON magidesk.Payments(Status);
CREATE INDEX IX_Payments_PaymentType ON magidesk.Payments(PaymentType);
CREATE INDEX IX_Payments_TransactionId ON magidesk.Payments(TransactionId);
```

### Credit Card Details

```sql
CREATE TABLE magidesk.CreditCardPayments (
    Id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    PaymentId UUID NOT NULL REFERENCES magidesk.Payments(Id) ON DELETE CASCADE,
    CardType VARCHAR(20) NOT NULL CHECK (CardType IN ('Visa', 'MasterCard', 'AmericanExpress', 'Discover', 'Other')),
    LastFourDigits VARCHAR(4) NOT NULL,
    ExpirationMonth INTEGER NOT NULL CHECK (ExpirationMonth BETWEEN 1 AND 12),
    ExpirationYear INTEGER NOT NULL CHECK (ExpirationYear >= EXTRACT(YEAR FROM CURRENT_DATE)),
    CardholderName VARCHAR(100),
    EntryMethod VARCHAR(20) NOT NULL CHECK (EntryMethod IN ('Swipe', 'Insert', 'Tap', 'Manual')),
    IsContactless BOOLEAN NOT NULL DEFAULT FALSE,
    CreatedAt TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);
```

### Gift Certificates

```sql
CREATE TABLE magidesk.GiftCertificates (
    Id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    CertificateNumber VARCHAR(50) NOT NULL UNIQUE,
    OriginalAmount DECIMAL(12,2) NOT NULL CHECK (OriginalAmount > 0),
    CurrentBalance DECIMAL(12,2) NOT NULL CHECK (CurrentBalance >= 0),
    IssuedAt TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    ExpiresAt TIMESTAMP WITH TIME ZONE,
    IssuedBy UUID NOT NULL REFERENCES magidesk.Users(Id),
    CustomerId UUID REFERENCES magidesk.Customers(Id),
    Status VARCHAR(20) NOT NULL DEFAULT 'Active' CHECK (Status IN ('Active', 'Expired', 'Depleted', 'Voided')),
    Notes TEXT,
    CreatedAt TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    UpdatedAt TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

CREATE INDEX IX_GiftCertificates_CertificateNumber ON magidesk.GiftCertificates(CertificateNumber);
CREATE INDEX IX_GiftCertificates_Status ON magidesk.GiftCertificates(Status);

CREATE TABLE magidesk.GiftCertificateTransactions (
    Id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    GiftCertificateId UUID NOT NULL REFERENCES magidesk.GiftCertificates(Id) ON DELETE CASCADE,
    PaymentId UUID REFERENCES magidesk.Payments(Id),
    TransactionType VARCHAR(20) NOT NULL CHECK (TransactionType IN ('Issued', 'Redeemed', 'Reloaded', 'Refunded')),
    Amount DECIMAL(12,2) NOT NULL,
    BalanceBefore DECIMAL(12,2) NOT NULL,
    BalanceAfter DECIMAL(12,2) NOT NULL,
    TicketId UUID REFERENCES magidesk.Tickets(Id),
    CreatedBy UUID NOT NULL REFERENCES magidesk.Users(Id),
    CreatedAt TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);
```

## Menu Management

### Menu Categories

```sql
CREATE TABLE magidesk.MenuCategories (
    Id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    Name VARCHAR(100) NOT NULL,
    Description TEXT,
    ParentCategoryId UUID REFERENCES magidesk.MenuCategories(Id),
    DisplayOrder INTEGER NOT NULL DEFAULT 0,
    IsAvailable BOOLEAN NOT NULL DEFAULT TRUE,
    Color VARCHAR(7), -- Hex color code
    Icon VARCHAR(50),
    CreatedAt TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    UpdatedAt TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

CREATE INDEX IX_MenuCategories_ParentCategoryId ON magidesk.MenuCategories(ParentCategoryId);
CREATE INDEX IX_MenuCategories_DisplayOrder ON magidesk.MenuCategories(DisplayOrder);
```

### Menu Items

```sql
CREATE TABLE magidesk.MenuItems (
    Id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    CategoryId UUID NOT NULL REFERENCES magidesk.MenuCategories(Id),
    Name VARCHAR(200) NOT NULL,
    Description TEXT,
    Price DECIMAL(10,2) NOT NULL CHECK (Price >= 0),
    Cost DECIMAL(10,2) CHECK (Cost >= 0),
    SKU VARCHAR(50),
    Barcode VARCHAR(50),
    ImageUrl VARCHAR(500),
    IsAvailable BOOLEAN NOT NULL DEFAULT TRUE,
    IsTaxable BOOLEAN NOT NULL DEFAULT TRUE,
    TaxRateId UUID REFERENCES magidesk.TaxRates(Id),
    PreparationTime INTEGER, -- in minutes
    SortOrder INTEGER NOT NULL DEFAULT 0,
    KitchenStationId UUID REFERENCES magidesk.KitchenStations(Id),
    IsCombo BOOLEAN NOT NULL DEFAULT FALSE,
    MaxQuantity INTEGER CHECK (MaxQuantity > 0),
    MinimumAge INTEGER CHECK (MinimumAge >= 0),
    Calories INTEGER CHECK (Calories >= 0),
    Allergens TEXT[], -- Array of allergen codes
    DietaryRestrictions TEXT[], -- Array of dietary restriction codes
    Version INTEGER NOT NULL DEFAULT 1,
    CreatedAt TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    UpdatedAt TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

CREATE INDEX IX_MenuItems_CategoryId ON magidesk.MenuItems(CategoryId);
CREATE INDEX IX_MenuItems_IsAvailable ON magidesk.MenuItems(IsAvailable);
CREATE INDEX IX_MenuItems_KitchenStationId ON magidesk.MenuItems(KitchenStationId);
CREATE INDEX IX_MenuItems_SKU ON magidesk.MenuItems(SKU);
```

### Modifiers

```sql
CREATE TABLE magidesk.ModifierGroups (
    Id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    Name VARCHAR(100) NOT NULL,
    Description TEXT,
    SelectionType VARCHAR(20) NOT NULL DEFAULT 'Optional' CHECK (SelectionType IN ('Required', 'Optional', 'Multiple')),
    MinSelections INTEGER NOT NULL DEFAULT 0 CHECK (MinSelections >= 0),
    MaxSelections INTEGER CHECK (MaxSelections > 0),
    DisplayOrder INTEGER NOT NULL DEFAULT 0,
    CreatedAt TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    UpdatedAt TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

CREATE TABLE magidesk.Modifiers (
    Id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    ModifierGroupId UUID NOT NULL REFERENCES magidesk.ModifierGroups(Id) ON DELETE CASCADE,
    Name VARCHAR(100) NOT NULL,
    Description TEXT,
    Price DECIMAL(10,2) NOT NULL DEFAULT 0.00 CHECK (Price >= 0),
    Cost DECIMAL(10,2) CHECK (Cost >= 0),
    IsAvailable BOOLEAN NOT NULL DEFAULT TRUE,
    DisplayOrder INTEGER NOT NULL DEFAULT 0,
    IsDefault BOOLEAN NOT NULL DEFAULT FALSE,
    CreatedAt TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    UpdatedAt TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

CREATE INDEX IX_Modifiers_ModifierGroupId ON magidesk.Modifiers(ModifierGroupId);
CREATE INDEX IX_Modifiers_IsAvailable ON magidesk.Modifiers(IsAvailable);

-- Link menu items to modifier groups
CREATE TABLE magidesk.MenuItemModifierGroups (
    Id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    MenuItemId UUID NOT NULL REFERENCES magidesk.MenuItems(Id) ON DELETE CASCADE,
    ModifierGroupId UUID NOT NULL REFERENCES magidesk.ModifierGroups(Id) ON DELETE CASCADE,
    DisplayOrder INTEGER NOT NULL DEFAULT 0,
    CreatedAt TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    UNIQUE(MenuItemId, ModifierGroupId)
);
```

## User Management

### Users

```sql
CREATE TABLE magidesk.Users (
    Id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    Username VARCHAR(50) NOT NULL UNIQUE,
    Email VARCHAR(255) UNIQUE,
    FirstName VARCHAR(100) NOT NULL,
    LastName VARCHAR(100) NOT NULL,
    PasswordHash VARCHAR(255) NOT NULL,
    Role VARCHAR(20) NOT NULL DEFAULT 'Server' CHECK (Role IN ('Admin', 'Manager', 'Server', 'Host', 'Kitchen', 'Cashier')),
    IsActive BOOLEAN NOT NULL DEFAULT TRUE,
    IsLocked BOOLEAN NOT NULL DEFAULT FALSE,
    FailedLoginAttempts INTEGER NOT NULL DEFAULT 0 CHECK (FailedLoginAttempts >= 0),
    LastLoginAt TIMESTAMP WITH TIME ZONE,
    PasswordChangedAt TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    Phone VARCHAR(20),
    Address TEXT,
    HireDate DATE,
    TerminationDate DATE,
    PayRate DECIMAL(10,2) CHECK (PayRate >= 0),
    Permissions TEXT[], -- Array of permission codes
    CreatedAt TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    UpdatedAt TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

CREATE INDEX IX_Users_Username ON magidesk.Users(Username);
CREATE INDEX IX_Users_Email ON magidesk.Users(Email);
CREATE INDEX IX_Users_Role ON magidesk.Users(Role);
CREATE INDEX IX_Users_IsActive ON magidesk.Users(IsActive);
```

### User Sessions

```sql
CREATE TABLE magidesk.UserSessions (
    Id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    UserId UUID NOT NULL REFERENCES magidesk.Users(Id),
    SessionToken VARCHAR(255) NOT NULL UNIQUE,
    StartedAt TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    ExpiresAt TIMESTAMP WITH TIME ZONE NOT NULL,
    LastActivityAt TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    IPAddress VARCHAR(45),
    UserAgent TEXT,
    IsActive BOOLEAN NOT NULL DEFAULT TRUE,
    CreatedAt TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

CREATE INDEX IX_UserSessions_UserId ON magidesk.UserSessions(UserId);
CREATE INDEX IX_UserSessions_SessionToken ON magidesk.UserSessions(SessionToken);
CREATE INDEX IX_UserSessions_ExpiresAt ON magidesk.UserSessions(ExpiresAt);
```

## Shift Management

### Shifts

```sql
CREATE TABLE magidesk.Shifts (
    Id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    Name VARCHAR(100) NOT NULL,
    Description TEXT,
    StartTime TIME NOT NULL,
    EndTime TIME NOT NULL,
    DaysOfWeek INTEGER[] NOT NULL, -- Array of day numbers (0=Sunday, 1=Monday, etc.)
    IsActive BOOLEAN NOT NULL DEFAULT TRUE,
    CreatedAt TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    UpdatedAt TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

CREATE INDEX IX_Shifts_IsActive ON magidesk.Shifts(IsActive);
```

### Cash Sessions

```sql
CREATE TABLE magidesk.CashSessions (
    Id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    ShiftId UUID REFERENCES magidesk.Shifts(Id),
    UserId UUID NOT NULL REFERENCES magidesk.Users(Id),
    TerminalId UUID REFERENCES magidesk.Terminals(Id),
    StartedAt TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    EndedAt TIMESTAMP WITH TIME ZONE,
    StartingCash DECIMAL(12,2) NOT NULL DEFAULT 0.00 CHECK (StartingCash >= 0),
    ExpectedCash DECIMAL(12,2) CHECK (ExpectedCash >= 0),
    ActualCash DECIMAL(12,2) CHECK (ActualCash >= 0),
    CashOverage DECIMAL(12,2),
    CashShortage DECIMAL(12,2),
    Status VARCHAR(20) NOT NULL DEFAULT 'Active' CHECK (Status IN ('Active', 'Closed', 'Balanced', 'Unbalanced')),
    Notes TEXT,
    CreatedAt TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    UpdatedAt TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

CREATE INDEX IX_CashSessions_UserId ON magidesk.CashSessions(UserId);
CREATE INDEX IX_CashSessions_TerminalId ON magidesk.CashSessions(TerminalId);
CREATE INDEX IX_CashSessions_Status ON magidesk.CashSessions(Status);
```

### Cash Drops and Bleeds

```sql
CREATE TABLE magidesk.CashTransactions (
    Id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    CashSessionId UUID NOT NULL REFERENCES magidesk.CashSessions(Id) ON DELETE CASCADE,
    TransactionType VARCHAR(20) NOT NULL CHECK (TransactionType IN ('Drop', 'Bleed', 'Pickup')),
    Amount DECIMAL(12,2) NOT NULL CHECK (Amount != 0),
    Reason TEXT,
    CreatedBy UUID NOT NULL REFERENCES magidesk.Users(Id),
    ApprovedBy UUID REFERENCES magidesk.Users(Id),
    CreatedAt TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

CREATE INDEX IX_CashTransactions_CashSessionId ON magidesk.CashTransactions(CashSessionId);
CREATE INDEX IX_CashTransactions_TransactionType ON magidesk.CashTransactions(TransactionType);
```

## Table Management

### Tables

```sql
CREATE TABLE magidesk.Tables (
    Id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    TableNumber VARCHAR(20) NOT NULL UNIQUE,
    Name VARCHAR(50),
    Capacity INTEGER NOT NULL CHECK (Capacity > 0),
    MinimumCapacity INTEGER NOT NULL DEFAULT 1 CHECK (MinimumCapacity > 0),
    Status VARCHAR(20) NOT NULL DEFAULT 'Available' CHECK (Status IN ('Available', 'Occupied', 'Reserved', 'NeedsCleaning', 'OutOfService')),
    SectionId UUID REFERENCES magidesk.TableSections(Id),
    Shape VARCHAR(20) DEFAULT 'Rectangle' CHECK (Shape IN ('Rectangle', 'Circle', 'Square', 'Oval')),
    XPosition INTEGER,
    YPosition INTEGER,
    Width INTEGER,
    Height INTEGER,
    Rotation INTEGER DEFAULT 0,
    CreatedAt TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    UpdatedAt TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

CREATE INDEX IX_Tables_TableNumber ON magidesk.Tables(TableNumber);
CREATE INDEX IX_Tables_Status ON magidesk.Tables(Status);
CREATE INDEX IX_Tables_SectionId ON magidesk.Tables(SectionId);
```

### Table Sections

```sql
CREATE TABLE magidesk.TableSections (
    Id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    Name VARCHAR(50) NOT NULL,
    Description TEXT,
    DisplayOrder INTEGER NOT NULL DEFAULT 0,
    CreatedAt TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    UpdatedAt TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);
```

## Kitchen Operations

### Kitchen Stations

```sql
CREATE TABLE magidesk.KitchenStations (
    Id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    Name VARCHAR(50) NOT NULL,
    Description TEXT,
    Type VARCHAR(30) NOT NULL DEFAULT 'Preparation' CHECK (Type IN ('Preparation', 'Cooking', 'Frying', 'Grilling', 'Baking', 'Beverage', 'Dessert')),
    PrinterId VARCHAR(50),
    DisplayOrder INTEGER NOT NULL DEFAULT 0,
    IsActive BOOLEAN NOT NULL DEFAULT TRUE,
    CreatedAt TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    UpdatedAt TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

CREATE INDEX IX_KitchenStations_Type ON magidesk.KitchenStations(Type);
CREATE INDEX IX_KitchenStations_IsActive ON magidesk.KitchenStations(IsActive);
```

### Kitchen Orders

```sql
CREATE TABLE magidesk.KitchenOrders (
    Id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    OrderLineId UUID NOT NULL REFERENCES magidesk.OrderLines(Id) ON DELETE CASCADE,
    StationId UUID NOT NULL REFERENCES magidesk.KitchenStations(Id),
    Status VARCHAR(20) NOT NULL DEFAULT 'Pending' CHECK (Status IN ('Pending', 'Acknowledged', 'Preparing', 'Ready', 'Completed', 'Cancelled')),
    Priority INTEGER NOT NULL DEFAULT 1 CHECK (Priority BETWEEN 1 AND 5),
    EstimatedTime INTEGER, -- in minutes
    ActualTime INTEGER, -- in minutes
    StartedAt TIMESTAMP WITH TIME ZONE,
    CompletedAt TIMESTAMP WITH TIME ZONE,
    AcknowledgedBy UUID REFERENCES magidesk.Users(Id),
    CompletedBy UUID REFERENCES magidesk.Users(Id),
    Notes TEXT,
    CreatedAt TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    UpdatedAt TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

CREATE INDEX IX_KitchenOrders_OrderLineId ON magidesk.KitchenOrders(OrderLineId);
CREATE INDEX IX_KitchenOrders_StationId ON magidesk.KitchenOrders(StationId);
CREATE INDEX IX_KitchenOrders_Status ON magidesk.KitchenOrders(Status);
CREATE INDEX IX_KitchenOrders_Priority ON magidesk.KitchenOrders(Priority);
```

## Tax and Discounts

### Tax Rates

```sql
CREATE TABLE magidesk.TaxRates (
    Id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    Name VARCHAR(100) NOT NULL,
    Rate DECIMAL(5,4) NOT NULL CHECK (Rate >= 0 AND Rate <= 1),
    Description TEXT,
    IsDefault BOOLEAN NOT NULL DEFAULT FALSE,
    IsActive BOOLEAN NOT NULL DEFAULT TRUE,
    CreatedAt TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    UpdatedAt TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

CREATE INDEX IX_TaxRates_IsDefault ON magidesk.TaxRates(IsDefault);
CREATE INDEX IX_TaxRates_IsActive ON magidesk.TaxRates(IsActive);
```

### Discounts

```sql
CREATE TABLE magidesk.Discounts (
    Id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    Name VARCHAR(100) NOT NULL,
    Description TEXT,
    Type VARCHAR(20) NOT NULL CHECK (Type IN ('Percentage', 'Amount', 'ItemPercentage', 'ItemAmount')),
    Value DECIMAL(12,4) NOT NULL CHECK (Value >= 0),
    MinimumAmount DECIMAL(12,2) CHECK (MinimumAmount >= 0),
    MaximumAmount DECIMAL(12,2) CHECK (MaximumAmount >= 0),
    IsAutoApply BOOLEAN NOT NULL DEFAULT FALSE,
    RequiresManagerApproval BOOLEAN NOT NULL DEFAULT FALSE,
    ValidFrom DATE,
    ValidTo DATE,
    UsageLimit INTEGER CHECK (UsageLimit > 0),
    TimesUsed INTEGER NOT NULL DEFAULT 0 CHECK (TimesUsed >= 0),
    MenuItemId UUID REFERENCES magidesk.MenuItems(Id),
    CategoryId UUID REFERENCES magidesk.MenuCategories(Id),
    IsActive BOOLEAN NOT NULL DEFAULT TRUE,
    CreatedAt TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    UpdatedAt TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

CREATE INDEX IX_Discounts_Type ON magidesk.Discounts(Type);
CREATE INDEX IX_Discounts_IsActive ON magidesk.Discounts(IsActive);
CREATE INDEX IX_Discounts_MenuItemId ON magidesk.Discounts(MenuItemId);
CREATE INDEX IX_Discounts_CategoryId ON magidesk.Discounts(CategoryId);

CREATE TABLE magidesk.TicketDiscounts (
    Id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    TicketId UUID NOT NULL REFERENCES magidesk.Tickets(Id) ON DELETE CASCADE,
    DiscountId UUID NOT NULL REFERENCES magidesk.Discounts(Id),
    Amount DECIMAL(12,2) NOT NULL CHECK (Amount >= 0),
    AppliedBy UUID NOT NULL REFERENCES magidesk.Users(Id),
    CreatedAt TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

CREATE INDEX IX_TicketDiscounts_TicketId ON magidesk.TicketDiscounts(TicketId);
CREATE INDEX IX_TicketDiscounts_DiscountId ON magidesk.TicketDiscounts(DiscountId);
```

## Reporting and Analytics

### Daily Sales Summary

```sql
CREATE TABLE magidesk.DailySalesSummaries (
    Id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    BusinessDate DATE NOT NULL UNIQUE,
    GrossSales DECIMAL(12,2) NOT NULL CHECK (GrossSales >= 0),
    NetSales DECIMAL(12,2) NOT NULL CHECK (NetSales >= 0),
    DiscountAmount DECIMAL(12,2) NOT NULL DEFAULT 0.00 CHECK (DiscountAmount >= 0),
    TaxAmount DECIMAL(12,2) NOT NULL DEFAULT 0.00 CHECK (TaxAmount >= 0),
    ServiceChargeAmount DECIMAL(12,2) NOT NULL DEFAULT 0.00 CHECK (ServiceChargeAmount >= 0),
    GratuityAmount DECIMAL(12,2) NOT NULL DEFAULT 0.00 CHECK (GratuityAmount >= 0),
    TicketCount INTEGER NOT NULL DEFAULT 0 CHECK (TicketCount >= 0),
    GuestCount INTEGER NOT NULL DEFAULT 0 CHECK (GuestCount >= 0),
    AverageTicketValue DECIMAL(12,2) CHECK (AverageTicketValue >= 0),
    CreatedAt TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    UpdatedAt TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

CREATE INDEX IX_DailySalesSummaries_BusinessDate ON magidesk.DailySalesSummaries(BusinessDate);
```

### Hourly Sales

```sql
CREATE TABLE magidesk.HourlySales (
    Id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    BusinessDate DATE NOT NULL,
    Hour INTEGER NOT NULL CHECK (Hour BETWEEN 0 AND 23),
    SalesAmount DECIMAL(12,2) NOT NULL DEFAULT 0.00 CHECK (SalesAmount >= 0),
    TicketCount INTEGER NOT NULL DEFAULT 0 CHECK (TicketCount >= 0),
    GuestCount INTEGER NOT NULL DEFAULT 0 CHECK (GuestCount >= 0),
    CreatedAt TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    UNIQUE(BusinessDate, Hour)
);

CREATE INDEX IX_HourlySales_BusinessDate ON magidesk.HourlySales(BusinessDate);
CREATE INDEX IX_HourlySales_Hour ON magidesk.HourlySales(Hour);
```

## Audit Trail

### Audit Logs

```sql
CREATE TABLE magidesk.AuditLogs (
    Id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    TableName VARCHAR(100) NOT NULL,
    RecordId UUID NOT NULL,
    Action VARCHAR(20) NOT NULL CHECK (Action IN ('INSERT', 'UPDATE', 'DELETE')),
    OldValues JSONB,
    NewValues JSONB,
    ChangedColumns TEXT[],
    UserId UUID REFERENCES magidesk.Users(Id),
    IPAddress VARCHAR(45),
    UserAgent TEXT,
    CreatedAt TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

CREATE INDEX IX_AuditLogs_TableName ON magidesk.AuditLogs(TableName);
CREATE INDEX IX_AuditLogs_RecordId ON magidesk.AuditLogs(RecordId);
CREATE INDEX IX_AuditLogs_Action ON magidesk.AuditLogs(Action);
CREATE INDEX IX_AuditLogs_UserId ON magidesk.AuditLogs(UserId);
CREATE INDEX IX_AuditLogs_CreatedAt ON magidesk.AuditLogs(CreatedAt);
```

### System Logs

```sql
CREATE TABLE magidesk.SystemLogs (
    Id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    LogLevel VARCHAR(10) NOT NULL CHECK (LogLevel IN ('DEBUG', 'INFO', 'WARN', 'ERROR', 'FATAL')),
    Category VARCHAR(50) NOT NULL,
    Message TEXT NOT NULL,
    Exception TEXT,
    UserId UUID REFERENCES magidesk.Users(Id),
    SessionId UUID,
    RequestId VARCHAR(50),
    AdditionalData JSONB,
    CreatedAt TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

CREATE INDEX IX_SystemLogs_LogLevel ON magidesk.SystemLogs(LogLevel);
CREATE INDEX IX_SystemLogs_Category ON magidesk.SystemLogs(Category);
CREATE INDEX IX_SystemLogs_UserId ON magidesk.SystemLogs(UserId);
CREATE INDEX IX_SystemLogs_CreatedAt ON magidesk.SystemLogs(CreatedAt);
```

## Data Integrity Constraints

### Check Constraints

```sql
-- Ticket constraints
ALTER TABLE magidesk.Tickets ADD CONSTRAINT CK_Tickets_DueAmount 
    CHECK (DueAmount >= 0);

ALTER TABLE magidesk.Tickets ADD CONSTRAINT CK_Tickets_PaymentLogic 
    CHECK (PaidAmount <= TotalAmount);

-- Order line constraints
ALTER TABLE magidesk.OrderLines ADD CONSTRAINT CK_OrderLines_TotalPrice 
    CHECK (TotalPrice = Quantity * UnitPrice);

-- Payment constraints
ALTER TABLE magidesk.Payments ADD CONSTRAINT CK_Payments_RefundLogic 
    CHECK (RefundAmount IS NULL OR RefundAmount <= Amount);
```

### Trigger Functions

```sql
-- Update timestamp trigger
CREATE OR REPLACE FUNCTION magidesk.update_updated_at_column()
RETURNS TRIGGER AS $$
BEGIN
    NEW.UpdatedAt = NOW();
    RETURN NEW;
END;
$$ language 'plpgsql';

-- Apply to tables with UpdatedAt column
CREATE TRIGGER update_tickets_updated_at BEFORE UPDATE ON magidesk.Tickets
    FOR EACH ROW EXECUTE FUNCTION magidesk.update_updated_at_column();

CREATE TRIGGER update_orderlines_updated_at BEFORE UPDATE ON magidesk.OrderLines
    FOR EACH ROW EXECUTE FUNCTION magidesk.update_updated_at_column();

-- Audit trigger
CREATE OR REPLACE FUNCTION magidesk.audit_trigger()
RETURNS TRIGGER AS $$
BEGIN
    IF TG_OP = 'INSERT' THEN
        INSERT INTO magidesk.AuditLogs (TableName, RecordId, Action, NewValues, UserId)
        VALUES (TG_TABLE_NAME, NEW.Id, 'INSERT', row_to_json(NEW), NEW.CreatedBy);
        RETURN NEW;
    ELSIF TG_OP = 'UPDATE' THEN
        INSERT INTO magidesk.AuditLogs (TableName, RecordId, Action, OldValues, NewValues, ChangedColumns, UserId)
        VALUES (TG_TABLE_NAME, NEW.Id, 'UPDATE', row_to_json(OLD), row_to_json(NEW), 
                array(SELECT key FROM jsonb_each_text(row_to_json(NEW)) 
                      EXCEPT SELECT key FROM jsonb_each_text(row_to_json(OLD))), NEW.UpdatedBy);
        RETURN NEW;
    ELSIF TG_OP = 'DELETE' THEN
        INSERT INTO magidesk.AuditLogs (TableName, RecordId, Action, OldValues, UserId)
        VALUES (TG_TABLE_NAME, OLD.Id, 'DELETE', row_to_json(OLD), OLD.CreatedBy);
        RETURN OLD;
    END IF;
    RETURN NULL;
END;
$$ LANGUAGE plpgsql;

-- Apply audit triggers
CREATE TRIGGER audit_tickets_trigger AFTER INSERT OR UPDATE OR DELETE ON magidesk.Tickets
    FOR EACH ROW EXECUTE FUNCTION magidesk.audit_trigger();
```

## Performance Optimization

### Partitioning Strategy

```sql
-- Partition audit logs by month
CREATE TABLE magidesk.AuditLogs_y2024m01 PARTITION OF magidesk.AuditLogs
    FOR VALUES FROM ('2024-01-01') TO ('2024-02-01');

CREATE TABLE magidesk.AuditLogs_y2024m02 PARTITION OF magidesk.AuditLogs
    FOR VALUES FROM ('2024-02-01') TO ('2024-03-01');

-- Partition system logs by month
CREATE TABLE magidesk.SystemLogs_y2024m01 PARTITION OF magidesk.SystemLogs
    FOR VALUES FROM ('2024-01-01') TO ('2024-02-01');
```

### Materialized Views

```sql
-- Daily sales summary materialized view
CREATE MATERIALIZED VIEW magidesk.DailySalesSummary AS
SELECT 
    DATE(CreatedAt) as BusinessDate,
    COUNT(*) as TicketCount,
    SUM(TotalAmount) as GrossSales,
    SUM(DueAmount) as NetSales,
    SUM(DiscountAmount) as DiscountAmount,
    SUM(TaxAmount) as TaxAmount,
    SUM(ServiceChargeAmount) as ServiceChargeAmount,
    COALESCE(SUM(p.Amount), 0) as GratuityAmount,
    SUM(GuestCount) as GuestCount,
    AVG(TotalAmount) as AverageTicketValue
FROM magidesk.Tickets t
LEFT JOIN magidesk.Payments p ON t.Id = p.TicketId AND p.PaymentType = 'Gratuity'
WHERE t.Status IN ('Paid', 'Closed')
GROUP BY DATE(CreatedAt);

CREATE UNIQUE INDEX IX_DailySalesSummary_BusinessDate ON magidesk.DailySalesSummary(BusinessDate);

-- Refresh function
CREATE OR REPLACE FUNCTION magidesk.refresh_daily_sales_summary(target_date DATE DEFAULT CURRENT_DATE)
RETURNS void AS $$
BEGIN
    REFRESH MATERIALIZED VIEW CONCURRENTLY magidesk.DailySalesSummary;
END;
$$ LANGUAGE plpgsql;
```

## Data Migration Strategy

### Version Control

```sql
-- Migration tracking table
CREATE TABLE magidesk.Migrations (
    Id SERIAL PRIMARY KEY,
    Version VARCHAR(20) NOT NULL UNIQUE,
    Description TEXT,
    ScriptName VARCHAR(255),
    AppliedAt TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    AppliedBy VARCHAR(100) NOT NULL DEFAULT 'system'
);

CREATE INDEX IX_Migrations_Version ON magidesk.Migrations(Version);
```

### Backup and Recovery

```sql
-- Create backup function
CREATE OR REPLACE FUNCTION magidesk.create_backup(backup_name VARCHAR(100))
RETURNS void AS $$
BEGIN
    EXECUTE format('CREATE TABLE magidesk.backup_%I AS SELECT * FROM magidesk.%I', backup_name, 'Tickets');
    -- Add other tables as needed
END;
$$ LANGUAGE plpgsql;
```

## Security Considerations

### Row Level Security

```sql
-- Enable RLS on sensitive tables
ALTER TABLE magidesk.Payments ENABLE ROW LEVEL SECURITY;
ALTER TABLE magidesk.Tickets ENABLE ROW LEVEL SECURITY;

-- Create policies
CREATE POLICY ticket_access_policy ON magidesk.Tickets
    FOR ALL TO application_role
    USING (ActiveDate >= CURRENT_DATE - INTERVAL '30 days');

CREATE POLICY payment_access_policy ON magidesk.Payments
    FOR ALL TO application_role
    USING (CreatedAt >= CURRENT_DATE - INTERVAL '30 days');
```

### Data Encryption

```sql
-- Encrypt sensitive data
CREATE EXTENSION IF NOT EXISTS pgcrypto;

-- Example of encrypted column (for future use)
ALTER TABLE magidesk.Users ADD COLUMN EncryptedSSN BYTEA;
UPDATE magidesk.Users SET EncryptedSSN = pgp_sym_encrypt(SSN, 'encryption_key');
```

## Conclusion

This comprehensive data model provides the foundation for all Magidesk POS operations. The design emphasizes:

- **Data Integrity**: Strong constraints and validation rules
- **Performance**: Optimized indexes and partitioning
- **Auditability**: Complete audit trail and logging
- **Security**: Row-level security and encryption capabilities
- **Scalability**: Partitioning and materialized views for large datasets
- **Maintainability**: Clear naming conventions and documentation

The model supports all business requirements while ensuring data consistency, performance, and security for restaurant operations.

---

*This data model documentation serves as the definitive reference for understanding the database structure and relationships in the Magidesk POS system.*
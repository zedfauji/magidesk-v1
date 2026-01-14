# Database Schema Fix: RoundingRule Column

## Issue
The application was failing when starting table sessions with the error:
```
PostgreSQL error 42703 - column t.RoundingRule does not exist
```

## Root Cause
The `TableType` entity in the domain model had a `RoundingRule` property (enum), but:
1. The EF Core configuration was missing the property mapping
2. No migration had been created to add the column to the database
3. The `MinimumCharge` owned entity columns were also missing

## Solution Applied

### 1. Updated EF Core Configuration
**File**: `Magidesk.Infrastructure/Data/Configurations/TableTypeConfiguration.cs`

Added the `RoundingRule` property configuration:
```csharp
builder.Property(t => t.RoundingRule)
    .IsRequired()
    .HasConversion<string>()
    .HasDefaultValue(Domain.Enumerations.TimeRoundingRule.None);
```

### 2. Applied Database Schema Changes
Used PostgreSQL MCP server to add missing columns directly to the database:

```sql
-- Add RoundingRule column
ALTER TABLE magidesk."TableTypes" 
ADD COLUMN IF NOT EXISTS "RoundingRule" text NOT NULL DEFAULT 'None';

-- Add MinimumCharge columns
ALTER TABLE magidesk."TableTypes" 
ADD COLUMN IF NOT EXISTS "MinimumChargeAmount" numeric(18,2) NOT NULL DEFAULT 0,
ADD COLUMN IF NOT EXISTS "MinimumChargeCurrency" character varying(3) NOT NULL DEFAULT 'USD';
```

### 3. Updated EF Core Model Snapshot
**File**: `Magidesk.Migrations/Migrations/ApplicationDbContextModelSnapshot.cs`

Updated the `TableType` entity configuration to include:
- `RoundingRule` property (text, default 'None')
- `MinimumCharge` owned entity with Amount and Currency properties

## Verification

### Database Schema (Verified via postgres-mcp)
The `magidesk.TableTypes` table now has all required columns:
- ✅ `Id` (uuid)
- ✅ `Name` (varchar(100))
- ✅ `Description` (varchar(500))
- ✅ `HourlyRate` (numeric(10,2))
- ✅ `FirstHourRate` (numeric(10,2), nullable)
- ✅ `MinimumMinutes` (integer, default 0)
- ✅ `RoundingMinutes` (integer, default 1)
- ✅ `RoundingRule` (text, default 'None') ← **ADDED**
- ✅ `MinimumChargeAmount` (numeric(18,2), default 0) ← **ADDED**
- ✅ `MinimumChargeCurrency` (varchar(3), default 'USD') ← **ADDED**
- ✅ `IsActive` (boolean, default true)
- ✅ `CreatedAt` (timestamp with time zone)
- ✅ `UpdatedAt` (timestamp with time zone)

### Build Status
- ✅ Build succeeded: 0 Errors, 582 Warnings (pre-existing)
- ✅ All projects compiled successfully

## RoundingRule Enum Values
The `TimeRoundingRule` enum supports:
- `None` - No rounding (default)
- `FifteenMinutes` - Round to 15-minute increments
- `ThirtyMinutes` - Round to 30-minute increments
- `SixtyMinutes` - Round to 60-minute increments

## Impact
- **Start Session** functionality should now work correctly
- Table pricing calculations will properly use the rounding rules
- Minimum charge enforcement is now supported

## Date
January 14, 2026

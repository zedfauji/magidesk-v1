# UserId Empty GUID Error - Root Cause and Fix

## Date
2026-01-16

## Issue
Application throws `ArgumentException: "UserId cannot be empty Guid"` during Entity Framework data materialization when loading `Gratuity` entities.

## Root Cause
The `GratuityConfiguration` was using `OwnsOne` pattern incorrectly for the `OwnerId` property (which is a `UserId` value object):

```csharp
builder.OwnsOne(g => g.OwnerId, o =>
{
    o.Property(ow => ow.Value)
        .HasColumnName("OwnerId")
        .IsRequired();
});
```

This configuration caused Entity Framework to attempt to materialize the `UserId` value object by calling its constructor with the GUID value from the database. The `UserId` constructor validates that the GUID is not empty and throws an exception if it is:

```csharp
public UserId(Guid value)
{
    if (value == Guid.Empty)
    {
        throw new ArgumentException("UserId cannot be empty Guid.", nameof(value));
    }
    Value = value;
}
```

## Why This Was Hard to Diagnose
1. **Database was clean**: Multiple forensic audits confirmed there were NO empty GUIDs in the database
2. **Error occurred during materialization**: The stack trace showed `PopulateIncludeCollection`, indicating EF was loading data from the database
3. **Inconsistent configuration**: Other UserId fields (like `ProcessedBy` in `Payment`, `Payout`, `CashDrop`, `DrawerBleed`) used `HasConversion` correctly, but `Gratuity.OwnerId` used `OwnsOne`

## The Fix
Changed `GratuityConfiguration` to use `HasConversion` instead of `OwnsOne`:

```csharp
builder.Property(g => g.OwnerId)
    .HasConversion(
        v => v.Value,
        v => new UserId(v))
    .HasColumnName("OwnerId")
    .IsRequired();
```

This ensures consistent behavior with other UserId fields in the codebase and prevents EF from attempting to materialize the value object incorrectly.

## Files Modified
- `Magidesk.Infrastructure/Data/Configurations/GratuityConfiguration.cs`

## Testing
After applying this fix:
1. Close the running application
2. Rebuild the solution
3. Run the application
4. The error should no longer occur when loading Gratuity entities

## Related Issues
- Previous fixes addressed empty GUIDs in the database (all cleaned up)
- Previous fixes addressed code logic issues in command handlers
- This fix addresses the EF configuration issue that was the actual root cause

## Lessons Learned
1. **Consistency matters**: When using value objects with EF Core, use the same configuration pattern throughout the codebase
2. **OwnsOne vs HasConversion**: `OwnsOne` is for complex value objects with multiple properties; `HasConversion` is for simple value objects that wrap a single value
3. **Trust the stack trace**: The error was happening during materialization, not during query construction or data insertion

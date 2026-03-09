# Task Spec: TICKET-S002 — Infrastructure Layer

## Ticket Summary
Deliver five inventory sidebar improvements by implementing the new repository methods, adding the InventoryCategory repository, updating EF configurations, and generating the required database migration.

## This Task's Responsibility
1. Implement `GetPagedAsync` on `InventoryItemRepository` — supports search term (name/SKU), filter type (LowStock, OutOfStock, RecentlyAdded, None), and category filter with skip/take pagination.
2. Create `InventoryCategoryRepository` implementing `IInventoryCategoryRepository`.
3. Update `InventoryItemConfiguration` to map the three new domain properties (`CategoryId`, `SkuCode`, `CreatedAt`).
4. Create `InventoryCategoryConfiguration` for the new entity.
5. Generate an EF Core migration: `AddInventoryCategory_ExtendInventoryItem`.
6. Register `InventoryCategoryRepository` in the Infrastructure DI extension.

### GetPagedAsync implementation on InventoryItemRepository
Build an `IQueryable<InventoryItem>` query:
- Always filter `IsActive == true`
- If `categoryId` is not null: add `.Where(x => x.CategoryId == categoryId)`
- If `searchTerm` is not null/whitespace: add `.Where(x => EF.Functions.ILike(x.Name, $"%{searchTerm}%") || (x.SkuCode != null && EF.Functions.ILike(x.SkuCode, $"%{searchTerm}%")))`
  - Use `EF.Functions.ILike` for case-insensitive PostgreSQL search — do NOT use `.ToLower()` string manipulation
- Apply `InventoryFilterType` switch:
  - `LowStock`: `.Where(x => x.StockQuantity <= x.ReorderPoint && x.StockQuantity > 0)`
  - `OutOfStock`: `.Where(x => x.StockQuantity == 0)`
  - `RecentlyAdded`: `.Where(x => x.CreatedAt >= DateTimeOffset.UtcNow.AddDays(-30))` — last 30 days
  - `None`: no additional filter
- Get total count via `CountAsync()` on the filtered (unpaged) query
- Apply `.OrderBy(x => x.Name).Skip(skip).Take(take)` for the paged result
- Return `(IReadOnlyList<InventoryItem> Items, int TotalCount)` tuple

### InventoryCategoryRepository
Simple CRUD repository:
- `GetAllActiveAsync` — `.Where(x => x.IsActive).OrderBy(x => x.SortOrder).ToListAsync()`
- `GetByIdAsync` — `.FirstOrDefaultAsync(x => x.Id == id)`
- `AddAsync` — `_context.InventoryCategories.Add(category); await _context.SaveChangesAsync()`
- `UpdateAsync` — `_context.InventoryCategories.Update(category); await _context.SaveChangesAsync()`
- Inject `MagideskDbContext` (or the project's concrete DbContext — check existing repositories for the actual context class name)

### InventoryItemConfiguration additions
Add to the `Configure` method:
```csharp
builder.Property(x => x.SkuCode).HasMaxLength(50);
builder.Property(x => x.CreatedAt).IsRequired();
builder.Property(x => x.CategoryId); // nullable FK — no required constraint
builder.HasOne<InventoryCategory>()
    .WithMany()
    .HasForeignKey(x => x.CategoryId)
    .IsRequired(false)
    .OnDelete(DeleteBehavior.SetNull);
```

### InventoryCategoryConfiguration
```csharp
builder.HasKey(x => x.Id);
builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
builder.Property(x => x.SortOrder).IsRequired();
builder.Property(x => x.IsActive).IsRequired();
builder.Property(x => x.ParentCategoryId); // nullable self-reference
builder.HasOne<InventoryCategory>()
    .WithMany()
    .HasForeignKey(x => x.ParentCategoryId)
    .IsRequired(false)
    .OnDelete(DeleteBehavior.Restrict);
```

### Migration
Run: `dotnet ef migrations add AddInventoryCategory_ExtendInventoryItem --project src/Magidesk.Migrations --startup-project src/Magidesk.Api` (adjust paths to match actual project layout). The migration must:
- Create `InventoryCategories` table with `Id`, `Name`, `SortOrder`, `ParentCategoryId`, `IsActive` columns
- Add `SkuCode` (varchar 50, nullable), `CreatedAt` (timestamptz, not null), `CategoryId` (uuid, nullable) columns to `InventoryItems`
- Add FK from `InventoryItems.CategoryId` to `InventoryCategories.Id` with `ON DELETE SET NULL`

**IMPORTANT:** Do not manually mutate any EF-managed concurrency or version fields. Do not add `Version` or `RowVersion` to the new entities.

## Input Contract
From Application Agent:
- `IInventoryItemRepository.GetPagedAsync(searchTerm, filter, categoryId, skip, take, ct)` signature
- `IInventoryCategoryRepository` interface with `GetAllActiveAsync`, `GetByIdAsync`, `AddAsync`, `UpdateAsync`
- `InventoryFilterType` enum values: `None`, `LowStock`, `OutOfStock`, `RecentlyAdded`
From Domain Agent:
- `InventoryCategory` entity with `Id`, `Name`, `SortOrder`, `ParentCategoryId`, `IsActive`
- `InventoryItem` new properties: `CategoryId` (Guid?), `SkuCode` (string?), `CreatedAt` (DateTimeOffset)

## Output Contract (Required)
- `InventoryItemRepository` implements new `GetPagedAsync` method
- `InventoryCategoryRepository` class implementing `IInventoryCategoryRepository`
- `InventoryCategoryConfiguration` class mapping `InventoryCategory` to `InventoryCategories` table
- `InventoryItemConfiguration` updated with `SkuCode`, `CreatedAt`, `CategoryId` FK mappings
- EF migration file `AddInventoryCategory_ExtendInventoryItem` applied cleanly to the `magidesk_pos` database
- Both repositories registered in `AddInfrastructureServices()` DI extension

## Files to Create
- `src/Magidesk.Infrastructure/Repositories/InventoryCategoryRepository.cs` — full CRUD implementation of `IInventoryCategoryRepository`
- `src/Magidesk.Infrastructure/Data/Configurations/InventoryCategoryConfiguration.cs` — EF config for `InventoryCategory`
- `src/Magidesk.Migrations/Migrations/[timestamp]_AddInventoryCategory_ExtendInventoryItem.cs` — EF migration (generated)
- `src/Magidesk.Migrations/Migrations/[timestamp]_AddInventoryCategory_ExtendInventoryItem.Designer.cs` — EF designer file (generated)

## Files to Modify
- `src/Magidesk.Infrastructure/Repositories/InventoryItemRepository.cs` — add `GetPagedAsync` implementation
  - Currently has CRUD methods only; add the new method at the bottom — do not reformat existing code
  - File is not in problem files; after addition must remain under 300 lines
- `src/Magidesk.Infrastructure/Data/Configurations/InventoryItemConfiguration.cs` — add three property mappings and FK
  - Currently 30 lines — targeted additions only; do not reformat
- `src/Magidesk.Infrastructure/DependencyInjection/ServiceCollectionExtensions.cs` — add `InventoryCategoryRepository` Scoped registration
- The DbContext file (check the actual context class, likely `MagideskDbContext.cs`) — add `DbSet<InventoryCategory> InventoryCategories { get; set; }`

## Constraints
- Follow all rules in AI_ASSISTANT_RULES.md
- Maximum file line limit: 300 lines per `.cs` file
- One class per file
- No business logic in repositories — pure persistence
- Never manually mutate EF-managed `Version` or `RowVersion` fields
- EF Core in Infrastructure only — no EF imports in other layers
- Implement interfaces — never define new ones here
- Use `EF.Functions.ILike` for case-insensitive search (PostgreSQL-specific) — not `.ToLower()`
- Register repositories as Scoped in the Infrastructure DI extension

## Acceptance Criteria
- `InventoryItemRepository.GetPagedAsync("sugar", None, null, 0, 20)` returns items whose Name or SkuCode contains "sugar" (case-insensitive), up to 20 results, with correct TotalCount
- `InventoryItemRepository.GetPagedAsync(null, LowStock, null, 0, 20)` returns only items where `StockQuantity <= ReorderPoint && StockQuantity > 0`
- `InventoryItemRepository.GetPagedAsync(null, OutOfStock, null, 0, 20)` returns only items where `StockQuantity == 0`
- `InventoryItemRepository.GetPagedAsync(null, RecentlyAdded, null, 0, 20)` returns only items created in the last 30 days
- `InventoryCategoryRepository.GetAllActiveAsync()` returns only active categories ordered by `SortOrder`
- EF migration applies without error: `dotnet ef database update`
- `InventoryCategories` table exists in `magidesk_pos` after migration
- `InventoryItems` table has new columns `SkuCode`, `CreatedAt`, `CategoryId` after migration
- dotnet build passes with 0 errors

## Do NOT
- Add business logic to the repository
- Touch `SalesReportRepository.cs` (problem file)
- Add a navigation property loading that causes N+1 queries — use explicit joins or projections
- Manually increment any `Version` field
- Define new interfaces — implement only

## XAML Flag
NO — this task does not produce or modify XAML

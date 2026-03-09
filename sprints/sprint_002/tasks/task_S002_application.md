# Task Spec: TICKET-S002 — Application Layer

## Ticket Summary
Deliver five inventory sidebar improvements by adding paged/filtered queries, a categories query, a bulk-update command, and the supporting DTOs and interface extensions that the ViewModel and Infrastructure layers depend on.

## This Task's Responsibility
Define all Application-layer contracts for the five inventory features:

1. Extend `IInventoryItemRepository` with pagination and filter support.
2. Add `IInventoryCategoryRepository` interface.
3. Add DTOs: `InventoryItemDto`, `InventoryCategoryDto`, `InventoryItemPagedResultDto`, `BulkUpdateInventoryItemEntryDto`.
4. Add `GetInventoryItemsPagedQuery` + Handler — paged, filterable, searchable.
5. Add `GetInventoryCategoriesQuery` + Handler — returns all active categories.
6. Add `BulkUpdateInventoryItemsCommand` + Handler — updates quantity and reorder point for a batch of items.
7. Register all new handlers in the Application DI extension.

### InventoryFilterType enum
Define `InventoryFilterType` enum in `Magidesk.Application.Queries` namespace:
```
None, LowStock, OutOfStock, RecentlyAdded
```

### IInventoryItemRepository extensions
Add to `IInventoryItemRepository`:
```csharp
Task<(IReadOnlyList<InventoryItem> Items, int TotalCount)> GetPagedAsync(
    string? searchTerm,
    InventoryFilterType filter,
    Guid? categoryId,
    int skip,
    int take,
    CancellationToken cancellationToken = default);
```

### IInventoryCategoryRepository
New interface:
```csharp
Task<IReadOnlyList<InventoryCategory>> GetAllActiveAsync(CancellationToken cancellationToken = default);
Task<InventoryCategory?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
Task AddAsync(InventoryCategory category, CancellationToken cancellationToken = default);
Task UpdateAsync(InventoryCategory category, CancellationToken cancellationToken = default);
```

### DTOs
- `InventoryItemDto` — flat DTO for paged results: `Id`, `Name`, `Unit`, `SkuCode`, `StockQuantity`, `ReorderPoint`, `CategoryId`, `CategoryName`, `CreatedAt`, `IsActive`
- `InventoryCategoryDto` — `Id`, `Name`, `SortOrder`, `ParentCategoryId`
- `InventoryItemPagedResultDto` — wraps `IReadOnlyList<InventoryItemDto> Items` + `int TotalCount` + `int Page` + `int PageSize`
- `BulkUpdateInventoryItemEntryDto` — `Guid Id`, `decimal NewStockQuantity`, `decimal NewReorderPoint`

### GetInventoryItemsPagedQuery
```csharp
public record GetInventoryItemsPagedQuery(
    string? SearchTerm,
    InventoryFilterType Filter,
    Guid? CategoryId,
    int Page,
    int PageSize) : IQuery<InventoryItemPagedResultDto>;
```
Handler: `GetInventoryItemsPagedQueryHandler` — calls `IInventoryItemRepository.GetPagedAsync`, maps results to `InventoryItemDto`, returns `InventoryItemPagedResultDto`.

### GetInventoryCategoriesQuery
```csharp
public record GetInventoryCategoriesQuery() : IQuery<IReadOnlyList<InventoryCategoryDto>>;
```
Handler: `GetInventoryCategoriesQueryHandler` — calls `IInventoryCategoryRepository.GetAllActiveAsync`, maps to `InventoryCategoryDto` list.

### BulkUpdateInventoryItemsCommand
```csharp
public record BulkUpdateInventoryItemsCommand(
    IReadOnlyList<BulkUpdateInventoryItemEntryDto> Items,
    string AdjustmentReason) : ICommand<Unit>;
```
Handler: `BulkUpdateInventoryItemsCommandHandler`
- Inject `IInventoryItemRepository`, `IInventoryAdjustmentRepository`, `IUserContextService`
- For each entry: load item by Id; call `AdjustStock(delta)` where `delta = entry.NewStockQuantity - item.StockQuantity`; call `SetReorderPoint(entry.NewReorderPoint)`; create `InventoryAdjustment.Create(item.Id, delta, adjustmentReason, userId)` if delta != 0; persist via `UpdateAsync`
- Never use `Guid.Empty` — resolve user identity via `IUserContextService.GetCurrentUserId()`
- All items in the batch are processed in a single operation scope; if any item is not found, throw `InvalidOperationException` with item Id
- Add FluentValidation validator: `BulkUpdateInventoryItemsCommandValidator` — must have at least 1 item; `NewStockQuantity` must be >= 0; `NewReorderPoint` must be >= 0

**Note:** `InventoryAdjustment.Create` currently takes `(Guid itemId, decimal delta, string reason)` — check existing signature and pass userId if the signature accepts it. If not, the Adjustment entity does not require userId in this ticket scope — skip the userId param for `InventoryAdjustment.Create`.

### IQuery / ICommand pattern
Use the project's custom `ICommandHandler<TCommand, TResult>` pattern. If the project uses `IQuery<TResult>` and `ICommand<TResult>` types, use those. If not, use the same record + handler pattern as `AdjustStockCommand` in `Magidesk.Application.Commands`.

## Input Contract
From Domain Agent:
- `InventoryCategory` entity with `Create()`, `Id`, `Name`, `SortOrder`, `ParentCategoryId`, `IsActive`
- `InventoryItem` with new properties: `CategoryId` (Guid?), `SkuCode` (string?), `CreatedAt` (DateTimeOffset)
- `InventoryItem` new methods: `AssignCategory`, `ClearCategory`, `UpdateSkuCode`
- `InventoryItem.Create(name, unit, stockQty, reorderPoint, skuCode?, categoryId?)` updated signature

## Output Contract (Required)
- `InventoryFilterType` enum in `Magidesk.Application.Queries` namespace
- `IInventoryItemRepository` extended with `GetPagedAsync(...)`
- `IInventoryCategoryRepository` in `Magidesk.Application.Interfaces`
- `InventoryItemDto` in `Magidesk.Application.DTOs`
- `InventoryCategoryDto` in `Magidesk.Application.DTOs`
- `InventoryItemPagedResultDto` in `Magidesk.Application.DTOs`
- `BulkUpdateInventoryItemEntryDto` in `Magidesk.Application.DTOs`
- `GetInventoryItemsPagedQuery` record + `GetInventoryItemsPagedQueryHandler` class
- `GetInventoryCategoriesQuery` record + `GetInventoryCategoriesQueryHandler` class
- `BulkUpdateInventoryItemsCommand` record + `BulkUpdateInventoryItemsCommandHandler` class + `BulkUpdateInventoryItemsCommandValidator` class
- All handlers registered in `AddApplicationServices()` DI extension

## Files to Create
- `src/Magidesk.Application/Interfaces/IInventoryCategoryRepository.cs` — repository interface for categories
- `src/Magidesk.Application/DTOs/InventoryItemDto.cs` — flat DTO for inventory items
- `src/Magidesk.Application/DTOs/InventoryCategoryDto.cs` — flat DTO for categories
- `src/Magidesk.Application/DTOs/InventoryItemPagedResultDto.cs` — paged result wrapper
- `src/Magidesk.Application/DTOs/BulkUpdateInventoryItemEntryDto.cs` — per-item bulk edit entry
- `src/Magidesk.Application/Queries/InventoryFilterType.cs` — filter enum
- `src/Magidesk.Application/Queries/GetInventoryItemsPagedQuery.cs` — query record
- `src/Magidesk.Application/Queries/GetInventoryItemsPagedQueryHandler.cs` — handler
- `src/Magidesk.Application/Queries/GetInventoryCategoriesQuery.cs` — query record
- `src/Magidesk.Application/Queries/GetInventoryCategoriesQueryHandler.cs` — handler
- `src/Magidesk.Application/Commands/BulkUpdateInventoryItemsCommand.cs` — command record
- `src/Magidesk.Application/Commands/BulkUpdateInventoryItemsCommandHandler.cs` — handler
- `src/Magidesk.Application/Commands/BulkUpdateInventoryItemsCommandValidator.cs` — FluentValidation validator

## Files to Modify
- `src/Magidesk.Application/Interfaces/IInventoryItemRepository.cs` — add `GetPagedAsync` overload; do not remove existing methods
- `src/Magidesk.Application/DependencyInjection/ServiceCollectionExtensions.cs` — register `GetInventoryItemsPagedQueryHandler`, `GetInventoryCategoriesQueryHandler`, `BulkUpdateInventoryItemsCommandHandler`

## Constraints
- Follow all rules in AI_ASSISTANT_RULES.md
- Maximum file line limit: 300 lines per `.cs` file
- One class per file
- No silent failures — never swallow exceptions
- `BulkUpdateInventoryItemsCommandHandler` must use `IUserContextService.GetCurrentUserId()` — never `Guid.Empty`
- Depend only on Domain — no Infrastructure imports
- DTOs cross the Application → Presentation boundary — never pass domain entities to callers
- FluentValidation only in Application layer
- If `BulkUpdateInventoryItemsCommandHandler` approaches 300 lines, extract mapping or validation logic to a private helper class in the same folder

## Acceptance Criteria
- `IInventoryItemRepository` has new `GetPagedAsync` method alongside existing methods (no breaking change)
- `IInventoryCategoryRepository` is defined with the four required methods
- All four DTOs are records or classes in `Magidesk.Application.DTOs` namespace
- `GetInventoryItemsPagedQueryHandler` maps domain entities to `InventoryItemDto` correctly (name, sku, category, stock, etc.)
- `BulkUpdateInventoryItemsCommandHandler` processes a batch of 2+ items, creates `InventoryAdjustment` for each non-zero delta
- `BulkUpdateInventoryItemsCommandValidator` rejects empty item list and negative quantities
- All three handlers are registered in `AddApplicationServices()`
- dotnet build passes with 0 errors in the Application project

## Do NOT
- Call EF Core directly — this layer defines interfaces only
- Import any Infrastructure namespace
- Use `Guid.Empty` as a user actor
- Remove existing methods from `IInventoryItemRepository`
- Add business logic to DTOs — they are pure data carriers

## XAML Flag
NO — this task does not produce or modify XAML

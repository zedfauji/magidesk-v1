# Task Spec: TICKET-S002 — Domain Layer

## Ticket Summary
Deliver five inventory sidebar improvements (categorisation, real-time search, UI virtualisation, bulk edit, quick filters) by extending the domain model with category and search-key support.

## This Task's Responsibility
Create the `InventoryCategory` entity and extend `InventoryItem` with three new properties required downstream: `CategoryId` (nullable FK), `SkuCode` (for search-by-code), and `CreatedAt` (for the "Recently Added" filter chip). Both mutations must enforce invariants and remain pure domain objects.

### InventoryCategory entity
- Properties: `Id` (Guid), `Name` (string), `SortOrder` (int), `ParentCategoryId` (nullable Guid), `IsActive` (bool)
- Factory: `InventoryCategory.Create(string name, int sortOrder, Guid? parentCategoryId = null)`
- Methods: `UpdateName(string name)`, `UpdateSortOrder(int order)`, `SetParent(Guid parentCategoryId)`, `ClearParent()`, `Deactivate()`, `Activate()`
- Invariant: `Name` must not be null or whitespace — throw `ArgumentException` on violation

### InventoryItem modifications
- Add `CategoryId` property: `public Guid? CategoryId { get; private set; }` — nullable, no invariant
- Add `SkuCode` property: `public string? SkuCode { get; private set; }` — nullable string, max 50 chars at domain level
- Add `CreatedAt` property: `public DateTimeOffset CreatedAt { get; private set; }` — set once in `Create()`, never mutated
- Update `Create()` factory signature: `Create(string name, string unit, decimal stockQuantity, decimal reorderPoint, string? skuCode = null, Guid? categoryId = null)` — set `CreatedAt = DateTimeOffset.UtcNow` internally
- Add method `AssignCategory(Guid categoryId)` — sets `CategoryId`
- Add method `ClearCategory()` — sets `CategoryId = null`
- Add method `UpdateSkuCode(string? skuCode)` — sets `SkuCode` (null allowed to clear)

## Input Contract
Nothing — Domain has no upstream agent.

## Output Contract (Required)
- `InventoryCategory` class in `Magidesk.Domain.Entities` namespace
  - `InventoryCategory.Create(string name, int sortOrder, Guid? parentCategoryId = null) : InventoryCategory`
  - `Id`, `Name`, `SortOrder`, `ParentCategoryId`, `IsActive` properties (all public get, private set)
- `InventoryItem` modifications:
  - New properties: `CategoryId` (Guid?), `SkuCode` (string?), `CreatedAt` (DateTimeOffset)
  - Updated factory: `InventoryItem.Create(string name, string unit, decimal stockQuantity, decimal reorderPoint, string? skuCode = null, Guid? categoryId = null)`
  - New methods: `AssignCategory(Guid)`, `ClearCategory()`, `UpdateSkuCode(string?)`

## Files to Create
- `src/Magidesk.Domain/Entities/InventoryCategory.cs` — new InventoryCategory entity with full factory and mutation methods

## Files to Modify
- `src/Magidesk.Domain/Entities/InventoryItem.cs` — add `CategoryId`, `SkuCode`, `CreatedAt`; update `Create()`; add `AssignCategory`, `ClearCategory`, `UpdateSkuCode`
  - **Not in problem files** — targeted additions only; do not reformat or remove existing methods

## Constraints
- Follow all rules in AI_ASSISTANT_RULES.md
- Maximum file line limit: 300 lines per `.cs` file
- One class per file
- No silent failures — throw `ArgumentException` on invariant violations
- Zero external dependencies — no ORM, no HTTP
- Do NOT add EF Core attributes or annotations to either entity
- `InventoryItem.cs` is currently 52 lines — after modification it must remain under 300 lines
- `InventoryCategory.cs` must be under 300 lines (it will be well under)

## Acceptance Criteria
- `InventoryCategory.Create("Beverages", 1)` returns an active category with correct properties
- `InventoryCategory.Create(null, 1)` throws `ArgumentException`
- `InventoryCategory.Create("", 1)` throws `ArgumentException`
- `InventoryItem.Create("Sugar", "kg", 10, 2)` still works with no `SkuCode` or `CategoryId` (backward compatible)
- `InventoryItem.Create("Sugar", "kg", 10, 2, "SKU-001", categoryId)` sets `SkuCode` and `CategoryId`
- `item.AssignCategory(categoryId)` sets `CategoryId`; `item.ClearCategory()` sets it back to null
- `item.UpdateSkuCode("NEW-SKU")` sets it; `item.UpdateSkuCode(null)` clears it
- `item.CreatedAt` is set at construction and is never null/default
- dotnet build passes with 0 errors in the Domain project

## Do NOT
- Add EF or any persistence annotations to the entities
- Add a navigation property (`InventoryCategory Category`) to `InventoryItem` — FK only, no navigation at domain level
- Modify any entity other than `InventoryItem` and the new `InventoryCategory`
- Touch `Ticket.cs`, `OrderPageViewModel.cs`, or `SalesReportRepository.cs`
- Add more than the three new properties to `InventoryItem` in this task

## XAML Flag
NO — this task does not produce or modify XAML

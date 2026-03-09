# Task Spec: TICKET-S002 — Tests Layer

## Ticket Summary
Deliver five inventory sidebar improvements with full test coverage for the new domain entity, the three Application handlers, and the repository paging contract.

## This Task's Responsibility
Write the minimum required tests covering:
1. **Domain unit tests** — `InventoryCategory` invariants and `InventoryItem` new property behaviour
2. **Application handler tests** — `GetInventoryItemsPagedQueryHandler`, `GetInventoryCategoriesQueryHandler`, `BulkUpdateInventoryItemsCommandHandler`
3. **Infrastructure integration test** — `InventoryItemRepository.GetPagedAsync` paging, search, and filter correctness

### Test naming convention
Follow the project convention observed in existing tests. If no convention file exists, use:
`MethodName_StateUnderTest_ExpectedBehaviour`

### Domain tests — Magidesk.Domain.Tests
File: `src/Magidesk.Domain.Tests/Entities/InventoryCategoryTests.cs`
- `Create_ValidName_ReturnsActiveCategory` — verify Id generated, Name set, IsActive = true, SortOrder set
- `Create_NullName_ThrowsArgumentException`
- `Create_EmptyName_ThrowsArgumentException`
- `Create_WithParentCategoryId_SetsParentCategoryId`
- `UpdateName_ValidName_UpdatesName`
- `UpdateName_WhitespaceName_ThrowsArgumentException`
- `Deactivate_ActiveCategory_SetsIsActiveFalse`
- `Activate_InactiveCategory_SetsIsActiveTrue`

File: `src/Magidesk.Domain.Tests/Entities/InventoryItemExtendedTests.cs` (new file — do not modify existing InventoryItem tests)
- `Create_WithSkuCode_SetsSkuCode`
- `Create_WithCategoryId_SetsCategoryId`
- `Create_NoSkuOrCategory_HasNullSkuAndNullCategory`
- `Create_SetsCreatedAt_ToUtcNow` — assert `CreatedAt` is within 1 second of `DateTimeOffset.UtcNow`
- `AssignCategory_SetsCategoryId`
- `ClearCategory_SetsCategoryIdToNull`
- `UpdateSkuCode_SetsNewCode`
- `UpdateSkuCode_Null_ClearsSkuCode`

### Application handler tests — Magidesk.Application.Tests
File: `src/Magidesk.Application.Tests/Queries/GetInventoryItemsPagedQueryHandlerTests.cs`
- `Handle_NoFilters_ReturnsPagedResult` — mock `IInventoryItemRepository.GetPagedAsync` returning 5 items, total 100; verify `TotalCount == 100`, `Items.Count == 5`, `Page == 0`
- `Handle_WithSearchTerm_PassesSearchTermToRepository` — verify `GetPagedAsync` was called with the search term
- `Handle_WithLowStockFilter_PassesLowStockFilterToRepository` — verify filter enum passed correctly
- `Handle_MapsEntityToDtoCorrectly` — verify `InventoryItemDto` fields match the mocked entity

File: `src/Magidesk.Application.Tests/Queries/GetInventoryCategoriesQueryHandlerTests.cs`
- `Handle_ReturnsAllActiveCategories` — mock repository returning 3 categories; verify `IReadOnlyList<InventoryCategoryDto>` with 3 items
- `Handle_MapsCategoryToDtoCorrectly` — verify `InventoryCategoryDto.Name` and `SortOrder` match mock data

File: `src/Magidesk.Application.Tests/Commands/BulkUpdateInventoryItemsCommandHandlerTests.cs`
- `Handle_ValidItems_UpdatesEachItemStockAndReorderPoint` — mock repository returning two items; verify `UpdateAsync` called for each; verify `InventoryAdjustmentRepository.AddAsync` called for items with non-zero delta
- `Handle_ZeroDelta_DoesNotCreateAdjustment` — if `NewStockQuantity == item.StockQuantity`, verify `AddAsync` NOT called
- `Handle_ItemNotFound_ThrowsInvalidOperationException` — mock repository returning null for a requested Id
- `Handle_UsesRealUserIdentity` — mock `IUserContextService.GetCurrentUserId()` returning a non-empty Guid; verify it is NOT `Guid.Empty`
- `Validator_EmptyItemList_ReturnsValidationFailure`
- `Validator_NegativeStockQuantity_ReturnsValidationFailure`
- `Validator_ValidItems_PassesValidation`

### Infrastructure integration test — Magidesk.Infrastructure.Tests
File: `src/Magidesk.Infrastructure.Tests/Repositories/InventoryItemRepositoryPagedTests.cs`
- `GetPagedAsync_NoFilters_ReturnsPaginatedResults` — seed 25 active items; call `GetPagedAsync(null, None, null, 0, 10)`; assert `Items.Count == 10` and `TotalCount == 25`
- `GetPagedAsync_SearchByName_ReturnsMatchingItems` — seed items including one named "TestSugar"; call with searchTerm "sugar"; assert it appears in results
- `GetPagedAsync_SearchBySkuCode_ReturnsMatchingItems` — seed item with SkuCode "SKU-FIND-ME"; call with searchTerm "FIND-ME"; assert it appears
- `GetPagedAsync_LowStockFilter_ReturnsOnlyLowStockItems` — seed mix of low-stock and normal items; assert only items where `StockQuantity <= ReorderPoint && StockQuantity > 0` are returned
- `GetPagedAsync_OutOfStockFilter_ReturnsOnlyZeroStockItems`
- `GetPagedAsync_CategoryFilter_ReturnsOnlyItemsInCategory` — seed items with and without category; assert only correct category items returned

**Note:** Infrastructure tests use a real PostgreSQL test database (per project conventions). Do not substitute SQLite or in-memory for these tests — the project frozen decisions require PostgreSQL only.

## Input Contract
From all preceding agents:
- `InventoryCategory.Create`, `Id`, `Name`, `SortOrder`, `ParentCategoryId`, `IsActive`
- `InventoryItem` new properties: `CategoryId`, `SkuCode`, `CreatedAt`; new methods: `AssignCategory`, `ClearCategory`, `UpdateSkuCode`
- `GetInventoryItemsPagedQueryHandler`, `GetInventoryCategoriesQueryHandler`, `BulkUpdateInventoryItemsCommandHandler`
- `IInventoryItemRepository.GetPagedAsync` signature
- `BulkUpdateInventoryItemsCommandValidator`
- `InventoryFilterType` enum values

## Output Contract (Required)
- All test files added to existing test projects — no new test projects created
- All new tests pass without modifying any existing tests
- Pre-existing 144/156 passing tests remain passing

## Files to Create
- `src/Magidesk.Domain.Tests/Entities/InventoryCategoryTests.cs` — 8 domain tests
- `src/Magidesk.Domain.Tests/Entities/InventoryItemExtendedTests.cs` — 8 domain tests
- `src/Magidesk.Application.Tests/Queries/GetInventoryItemsPagedQueryHandlerTests.cs` — 4 handler tests
- `src/Magidesk.Application.Tests/Queries/GetInventoryCategoriesQueryHandlerTests.cs` — 2 handler tests
- `src/Magidesk.Application.Tests/Commands/BulkUpdateInventoryItemsCommandHandlerTests.cs` — 7 tests (5 handler + 3 validator)
- `src/Magidesk.Infrastructure.Tests/Repositories/InventoryItemRepositoryPagedTests.cs` — 6 integration tests

## Files to Modify
None — add only; do not touch pre-existing test files.

## Constraints
- Follow all rules in AI_ASSISTANT_RULES.md
- Maximum file line limit: 300 lines per `.cs` file — split test classes if needed
- No `Thread.Sleep` — deterministic assertions only
- Do not modify existing failing tests
- Mock `IUserContextService.GetCurrentUserId()` with a real non-empty Guid — never `Guid.Empty`
- Infrastructure tests must use PostgreSQL — do not use SQLite or in-memory DbContext
- Each test must be independently runnable — no shared mutable state between tests
- One test class per file

## Acceptance Criteria
- All 8 `InventoryCategoryTests` pass
- All 8 `InventoryItemExtendedTests` pass
- All 4 `GetInventoryItemsPagedQueryHandlerTests` pass
- All 2 `GetInventoryCategoriesQueryHandlerTests` pass
- All 8 `BulkUpdateInventoryItemsCommandHandlerTests` pass (5 handler + 3 validator)
- All 6 `InventoryItemRepositoryPagedTests` pass
- Pre-existing 144 passing tests remain passing
- `dotnet test` on Domain.Tests and Application.Tests passes with 0 failures
- No test uses `Thread.Sleep`
- No test uses `Guid.Empty` as a mock user identity

## Do NOT
- Modify any existing test file
- Create a new test project — add to existing projects only
- Use `Thread.Sleep` or fixed delays
- Comment out assertions to make tests pass
- Mock infrastructure to fake PostgreSQL — Infrastructure tests need the real database
- Write tests for the View or ViewModel layers in this task (not in scope)

## XAML Flag
NO — this task does not produce or modify XAML

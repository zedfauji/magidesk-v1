# Task Execution Order: TICKET-S002

## Pre-Flight Result
CLEAR — no frozen decisions affected, no financial immutability risk.

Flags noted:
- ⚠️ XAML touchpoint: Step 5 (View Agent) modifies InventoryPage.xaml and creates InventoryBulkEditDialog.xaml — human must perform manual clean + rebuild in Visual Studio Insider before marking Step 5 complete.
- InventoryViewModel.cs is NOT in the problem files list. Full rewrite is permitted (currently 169 lines). The three partial files must each stay under 300 lines.
- Do not touch: OrderPageViewModel.cs, Ticket.cs, SalesReportRepository.cs, PrintingService.cs.

---

## Execution Sequence

| Step | Task File | Agent Role | Depends On | Can Parallelize? |
|------|-----------|------------|------------|-----------------|
| 1 | task_S002_domain.md | Domain Agent | Nothing | No |
| 2 | task_S002_application.md | Application Agent | Step 1 | No |
| 3 | task_S002_infrastructure.md | Infrastructure Agent | Step 2 | Yes (parallel with Step 4) |
| 4 | task_S002_viewmodel.md | ViewModel Agent | Step 2 | Yes (parallel with Step 3) |
| 5 | task_S002_view.md | View Agent | Step 4 | No |
| 6 | task_S002_tests.md | Test Agent | Steps 1–5 | No |
| 7 | review_S002.md | Review Agent | Step 6 | No |

---

## Human Touchpoints

| Step | Action Required |
|------|----------------|
| Step 3 | Run EF migration: `dotnet ef migrations add AddInventoryCategory_ExtendInventoryItem --project src/Magidesk.Migrations --startup-project src/Magidesk.Api` then `dotnet ef database update`. Agent generates migration code but human must verify the schema diff before applying to the production database. |
| Step 5 | ⚠️ XAML build verify — after View Agent completes, human must do a manual clean + rebuild in Visual Studio Insider. XAML compilation errors are not reliably caught by AI tooling. Do not mark Step 5 complete until the build passes with 0 errors. |

---

## Blocking Conditions

- If Domain Agent (Step 1) outputs a BLOCKER: abort all downstream steps immediately.
- If Application Agent (Step 2) outputs a BLOCKER: abort Steps 3, 4, 5, 6.
- If Infrastructure Agent (Step 3) fails migration: Steps 5 and 6 (infrastructure integration tests) cannot proceed — flag to owner.
- If ViewModel Agent (Step 4) outputs a BLOCKER: abort Step 5.
- If View Agent (Step 5) flags XAML errors after rebuild: do not proceed to Step 6 until resolved.
- If any step produces a file with more than 300 lines: that step is incomplete — agent must split before handing off.
- If `IUserContextService.GetCurrentUserId()` is not called in `BulkUpdateInventoryItemsCommandHandler`: Step 2 is incomplete — fix before proceeding to Step 3.

---

## Layer Summary

| Layer | New Files | Modified Files |
|-------|-----------|---------------|
| Domain | InventoryCategory.cs | InventoryItem.cs |
| Application | IInventoryCategoryRepository.cs, 4 DTOs, InventoryFilterType.cs, 2 query records + handlers, 1 command record + handler + validator | IInventoryItemRepository.cs, ServiceCollectionExtensions.cs |
| Infrastructure | InventoryCategoryRepository.cs, InventoryCategoryConfiguration.cs, EF migration | InventoryItemRepository.cs, InventoryItemConfiguration.cs, ServiceCollectionExtensions.cs, DbContext |
| ViewModel | InventoryViewModel.Search.cs, InventoryViewModel.BulkEdit.cs, InventoryBulkEditViewModel.cs, InventoryBulkEditRow.cs | InventoryViewModel.cs (rewrite to partial), PresentationServiceExtensions.cs |
| View | InventoryBulkEditDialog.xaml, InventoryBulkEditDialog.xaml.cs | InventoryPage.xaml, InventoryPage.xaml.cs |
| Tests | InventoryCategoryTests.cs, InventoryItemExtendedTests.cs, GetInventoryItemsPagedQueryHandlerTests.cs, GetInventoryCategoriesQueryHandlerTests.cs, BulkUpdateInventoryItemsCommandHandlerTests.cs, InventoryItemRepositoryPagedTests.cs | None |

---

## Acceptance Gate (all must be true before Review Agent runs)

- [ ] `dotnet build` passes with 0 errors across entire solution
- [ ] EF migration applied cleanly — `InventoryCategories` table exists and `InventoryItems` has new columns
- [ ] XAML clean + rebuild passes in Visual Studio Insider
- [ ] All new domain and application tests pass
- [ ] Pre-existing 144/156 passing tests still pass
- [ ] No file exceeds 300 lines
- [ ] No `{Binding}` reflection binding in new XAML
- [ ] No `Guid.Empty` used as user identity in any handler
- [ ] No repository or EF imports in any ViewModel file

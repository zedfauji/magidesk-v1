# Task Spec: TICKET-S002 — ViewModel Layer

## Ticket Summary
Deliver five inventory sidebar improvements by overhauling `InventoryViewModel` into three partial class files and creating a dedicated `InventoryBulkEditViewModel` for the inline DataGrid editing flow.

## This Task's Responsibility
The existing `InventoryViewModel.cs` (169 lines) performs direct repository calls and will not scale to the new feature set. Replace it with a clean, Application-layer-driven implementation split across three partial files, plus a new sub-ViewModel for bulk editing.

### File split plan
| File | Responsibility | Max lines |
|------|---------------|-----------|
| `InventoryViewModel.cs` | Constructor, DI, core state (IsBusy, StatusMessage, pagination), `LoadPageAsync`, `LoadCategoriesAsync`, category TreeView source | 250 |
| `InventoryViewModel.Search.cs` | `SearchText`, `ActiveFilter` (InventoryFilterType), filter chip commands, search-triggered reload | 150 |
| `InventoryViewModel.BulkEdit.cs` | Checkbox selection tracking, `SelectedItems` collection, `IsBulkEditBarVisible`, `OpenBulkEditCommand`, bulk-edit invocation | 150 |
| `InventoryBulkEditViewModel.cs` | Separate ViewModel for the bulk edit DataGrid dialog — `EditableItems` (ObservableCollection), `ConfirmCommand`, `CancelCommand` | 200 |

### InventoryViewModel.cs (core)
```csharp
public partial class InventoryViewModel : ViewModelBase
```
- Constructor injects: `ICommandHandler<GetInventoryItemsPagedQuery, InventoryItemPagedResultDto>`, `ICommandHandler<GetInventoryCategoriesQuery, IReadOnlyList<InventoryCategoryDto>>`, `ICommandHandler<BulkUpdateInventoryItemsCommand, Unit>` (or equivalent query/command handler interfaces — use the project's actual DI pattern)
- Properties: `[ObservableProperty] private bool _isBusy`, `[ObservableProperty] private string _statusMessage`, `[ObservableProperty] private int _currentPage`, `[ObservableProperty] private int _totalCount`, `[ObservableProperty] private int _pageSize = 50`
- `public ObservableCollection<InventoryItemDto> InventoryItems { get; } = new()`
- `public ObservableCollection<InventoryCategoryDto> Categories { get; } = new()`
- `[ObservableProperty] private InventoryCategoryDto? _selectedCategory` — when set, triggers a filtered reload
- `[RelayCommand] private async Task LoadPageAsync()` — calls `GetInventoryItemsPagedQuery` with current `SearchText`, `ActiveFilter`, `SelectedCategory?.Id`, `CurrentPage`, `PageSize`; replaces `InventoryItems` collection; updates `TotalCount`
- `[RelayCommand] private async Task LoadCategoriesAsync()` — calls `GetInventoryCategoriesQuery`; replaces `Categories`; called once at startup
- `[RelayCommand] private async Task NextPageAsync()` — increments `CurrentPage`, calls `LoadPageAsync`
- `[RelayCommand] private async Task PreviousPageAsync()` — decrements `CurrentPage` (min 0), calls `LoadPageAsync`
- `public bool HasNextPage => (CurrentPage + 1) * PageSize < TotalCount`
- `public bool HasPreviousPage => CurrentPage > 0`

### InventoryViewModel.Search.cs (partial)
```csharp
public partial class InventoryViewModel
```
- `[ObservableProperty] private string _searchText = string.Empty` — on `OnSearchTextChanged` partial, reset `CurrentPage = 0` and schedule a debounced reload (use `Task.Delay(300)` with a cancellation token pattern to debounce; store `CancellationTokenSource _searchCts`)
- `[ObservableProperty] private InventoryFilterType _activeFilter = InventoryFilterType.None`
- `[RelayCommand] private void SetFilter(InventoryFilterType filter)` — sets `ActiveFilter`, resets `CurrentPage = 0`, triggers `LoadPageAsync()`
- `public bool IsFilterAll => ActiveFilter == InventoryFilterType.None`
- `public bool IsFilterLowStock => ActiveFilter == InventoryFilterType.LowStock`
- `public bool IsFilterOutOfStock => ActiveFilter == InventoryFilterType.OutOfStock`
- `public bool IsFilterRecentlyAdded => ActiveFilter == InventoryFilterType.RecentlyAdded`

### InventoryViewModel.BulkEdit.cs (partial)
```csharp
public partial class InventoryViewModel
```
- `public ObservableCollection<InventoryItemDto> SelectedItems { get; } = new()`
- `public bool IsBulkEditBarVisible => SelectedItems.Count >= 2`
- `[RelayCommand] private void ToggleItemSelection(InventoryItemDto item)` — adds/removes item from `SelectedItems`; raises `OnPropertyChanged(nameof(IsBulkEditBarVisible))`
- `[RelayCommand] private void OpenBulkEdit()` — raises `BulkEditRequested` event (see below); View subscribes to this event to open the dialog
- `public event EventHandler<IReadOnlyList<InventoryItemDto>>? BulkEditRequested` — fired by `OpenBulkEdit`; passes the current `SelectedItems` snapshot
- `public async Task CommitBulkEditAsync(IReadOnlyList<BulkUpdateInventoryItemEntryDto> entries)` — calls `BulkUpdateInventoryItemsCommand`; on success, clears `SelectedItems`, reloads page, shows status message; on failure, sets `StatusMessage` with error

### InventoryBulkEditViewModel.cs
Standalone ViewModel (not partial) — created fresh and injected into the bulk edit dialog:
```csharp
public class InventoryBulkEditViewModel : ObservableObject
```
- Constructor: `InventoryBulkEditViewModel(IReadOnlyList<InventoryItemDto> selectedItems)`
- `public ObservableCollection<InventoryBulkEditRow> EditableItems { get; }` — one row per selected item
- `[RelayCommand] private void Confirm()` — raises `Confirmed` event with `IReadOnlyList<BulkUpdateInventoryItemEntryDto>`
- `[RelayCommand] private void Cancel()` — raises `Cancelled` event
- `public event EventHandler<IReadOnlyList<BulkUpdateInventoryItemEntryDto>>? Confirmed`
- `public event EventHandler? Cancelled`

### InventoryBulkEditRow.cs
Simple observable row model (one class per file):
```csharp
public class InventoryBulkEditRow : ObservableObject
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    [ObservableProperty] private decimal _newStockQuantity;
    [ObservableProperty] private decimal _newReorderPoint;
}
```

## Input Contract
From Application Agent:
- `GetInventoryItemsPagedQuery(SearchTerm, Filter, CategoryId, Page, PageSize)` record
- `GetInventoryCategoriesQuery()` record
- `BulkUpdateInventoryItemsCommand(Items, AdjustmentReason)` record
- `InventoryItemDto` with: `Id`, `Name`, `Unit`, `SkuCode`, `StockQuantity`, `ReorderPoint`, `CategoryId`, `CategoryName`, `CreatedAt`, `IsActive`
- `InventoryCategoryDto` with: `Id`, `Name`, `SortOrder`, `ParentCategoryId`
- `InventoryItemPagedResultDto` with: `Items`, `TotalCount`, `Page`, `PageSize`
- `BulkUpdateInventoryItemEntryDto` with: `Id`, `NewStockQuantity`, `NewReorderPoint`
- `InventoryFilterType` enum: `None`, `LowStock`, `OutOfStock`, `RecentlyAdded`

## Output Contract (Required)
Observable properties and commands for the View Agent:

**InventoryViewModel:**
- `ObservableCollection<InventoryItemDto> InventoryItems`
- `ObservableCollection<InventoryCategoryDto> Categories`
- `InventoryCategoryDto? SelectedCategory` (ObservableProperty)
- `string SearchText` (ObservableProperty)
- `InventoryFilterType ActiveFilter` (ObservableProperty)
- `bool IsFilterAll`, `bool IsFilterLowStock`, `bool IsFilterOutOfStock`, `bool IsFilterRecentlyAdded` (computed)
- `bool IsBusy` (ObservableProperty)
- `string StatusMessage` (ObservableProperty)
- `int TotalCount`, `int CurrentPage`, `int PageSize` (ObservableProperty)
- `bool HasNextPage`, `bool HasPreviousPage` (computed)
- `ObservableCollection<InventoryItemDto> SelectedItems`
- `bool IsBulkEditBarVisible` (computed)
- Commands: `LoadPageCommand`, `LoadCategoriesCommand`, `NextPageCommand`, `PreviousPageCommand`, `SetFilterCommand`, `ToggleItemSelectionCommand`, `OpenBulkEditCommand`
- Event: `BulkEditRequested`

**InventoryBulkEditViewModel:**
- `ObservableCollection<InventoryBulkEditRow> EditableItems`
- Commands: `ConfirmCommand`, `CancelCommand`
- Events: `Confirmed`, `Cancelled`

**InventoryBulkEditRow:**
- `Guid Id`, `string Name` (init-only)
- `decimal NewStockQuantity`, `decimal NewReorderPoint` (ObservableProperty)

## Files to Create
- `src/Magidesk.Presentation/ViewModels/InventoryViewModel.Search.cs` — partial: search + filter chip state
- `src/Magidesk.Presentation/ViewModels/InventoryViewModel.BulkEdit.cs` — partial: bulk selection and edit delegation
- `src/Magidesk.Presentation/ViewModels/InventoryBulkEditViewModel.cs` — sub-ViewModel for bulk edit DataGrid
- `src/Magidesk.Presentation/ViewModels/InventoryBulkEditRow.cs` — observable row model for DataGrid binding

## Files to Modify
- `src/Magidesk.Presentation/ViewModels/InventoryViewModel.cs` — rewrite to use Application-layer queries/commands; convert to `partial class`; remove direct repository calls; split code into three partial files as described above
  - Not in problem files — full rewrite is permitted
  - After rewrite, `InventoryViewModel.cs` must be under 250 lines
- `src/Magidesk.Presentation/DependencyInjection/PresentationServiceExtensions.cs` — register `InventoryBulkEditViewModel` as Transient (created fresh per dialog open)

## Constraints
- Follow all rules in AI_ASSISTANT_RULES.md
- Maximum file line limit: 300 lines per `.cs` file
- One class per file (partial classes count as one class across multiple files)
- No business logic — ViewModels are coordinators only
- No repository or DbContext imports — call Application layer only
- Use `[ObservableProperty]` and `[RelayCommand]` — never manual `ICommand`
- Debounce search with CancellationToken pattern — no `Thread.Sleep`
- Never expose domain entities to the View — DTOs only
- `InventoryBulkEditViewModel` is Transient (fresh per open) — do not register as Singleton

## Acceptance Criteria
- `InventoryViewModel.cs` is a partial class, under 250 lines, injects only Application-layer types
- `LoadPageAsync` fires `GetInventoryItemsPagedQuery` and replaces `InventoryItems` with the result
- Changing `SearchText` triggers a debounced reload with the new search term
- Calling `SetFilterCommand` with `LowStock` sets `ActiveFilter`, resets page, reloads
- `SelectedItems.Count` increases when `ToggleItemSelectionCommand` is called; `IsBulkEditBarVisible` becomes true at count >= 2
- `OpenBulkEditCommand` raises `BulkEditRequested` event with the current selection snapshot
- `CommitBulkEditAsync` calls `BulkUpdateInventoryItemsCommand` and clears `SelectedItems` on success
- `InventoryBulkEditViewModel` exposes `EditableItems` rows, each with editable `NewStockQuantity` and `NewReorderPoint`
- No repository or EF imports appear in any ViewModel file
- dotnet build passes with 0 errors in the Presentation project

## Do NOT
- Call `IInventoryItemRepository` directly from any ViewModel
- Import `Magidesk.Infrastructure.*` in any ViewModel
- Import `Magidesk.Domain.*` entities in any ViewModel (DTOs only)
- Combine more than one class in a single file
- Use reflection-based `{Binding}` patterns — compiled binding only (relevant for code-behind references)
- Add business logic for low-stock detection — that is handled via `InventoryFilterType.LowStock` passed to the query

## XAML Flag
NO — this task does not produce or modify XAML

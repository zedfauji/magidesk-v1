# Task Spec: TICKET-S002 — View Layer

## Ticket Summary
Deliver five inventory sidebar improvements by overhauling `InventoryPage.xaml` with a search bar, filter ribbon, category-grouped virtualised list with checkboxes, a bulk action bar, and a `InventoryBulkEditDialog` ContentDialog containing a CommunityToolkit.WinUI DataGrid.

## This Task's Responsibility
Produce two new/modified XAML files that bind exclusively to properties and commands defined in the ViewModel output contract. Zero logic in code-behind — event handlers delegate immediately to ViewModel.

### InventoryPage.xaml layout (top to bottom)
```
[ Search TextBox ]
[ Filter Ribbon: All | Low Stock | Out of Stock | Recently Added ]
[ Category TreeView / grouped panel (left column, ~30% width) ]
[ Item List with checkboxes (right column, 70% width)          ]
[ Bulk Action Bar (visible only when IsBulkEditBarVisible)      ]
[ Pagination: Previous | Page N of M | Next                     ]
```

#### Search TextBox
- `x:Bind ViewModel.SearchText, Mode=TwoWay` — no code-behind text-changed handler
- Placeholder: "Search by name or SKU…"
- Clear button (`x:Bind ViewModel.SearchText.Length > 0` or a ClearCommand) — keep simple

#### Filter Ribbon
Four `ToggleButton` controls in a horizontal `StackPanel` (or `CommandBarFlyout`-free row):
- "All" — IsChecked bound to `x:Bind ViewModel.IsFilterAll, Mode=OneWay`; click calls `ViewModel.SetFilterCommand` with parameter `0` (= `InventoryFilterType.None`)
- "Low Stock" — `x:Bind ViewModel.IsFilterLowStock, Mode=OneWay`; click passes `1`
- "Out of Stock" — `x:Bind ViewModel.IsFilterOutOfStock, Mode=OneWay`; click passes `2`
- "Recently Added" — `x:Bind ViewModel.IsFilterRecentlyAdded, Mode=OneWay`; click passes `3`
- Use integer CommandParameter values corresponding to `InventoryFilterType` int values (None=0, LowStock=1, OutOfStock=2, RecentlyAdded=3)

#### Category Panel (left column)
- `TreeView` (WinUI 3 `Microsoft.UI.Xaml.Controls.TreeView`) bound to `x:Bind ViewModel.Categories`
- Each `TreeViewItem` displays `CategoryName`
- Selecting a category sets `ViewModel.SelectedCategory` via `x:Bind ViewModel.SelectedCategory, Mode=TwoWay` on the TreeView's `SelectedItem` (or use `SelectionChanged` event that immediately delegates to ViewModel)
- "All Categories" root node (static, not data-bound) resets `SelectedCategory = null`

#### Item List (right column, virtualised)
- `ListView` with `ItemsPanel` using `ItemsStackPanel` (WinUI 3 virtualisation default) — do not disable virtualisation
- `ItemsSource x:Bind ViewModel.InventoryItems, Mode=OneWay`
- Each item row: `CheckBox` + `TextBlock` (Name) + `TextBlock` (SKU) + `TextBlock` (Stock) + `TextBlock` (Reorder)
- `CheckBox.Command x:Bind ViewModel.ToggleItemSelectionCommand` with `CommandParameter x:Bind` bound to the row's `InventoryItemDto`
- Use `DataTemplate` with `x:DataType="dto:InventoryItemDto"` for compiled binding in the template

#### Bulk Action Bar
- `StackPanel` with `Visibility x:Bind ViewModel.IsBulkEditBarVisible, Mode=OneWay, Converter={StaticResource BoolToVisibilityConverter}`
- Contents: `TextBlock` showing `x:Bind ViewModel.SelectedItems.Count, Mode=OneWay` + " items selected" + `Button` "Bulk Edit" calling `ViewModel.OpenBulkEditCommand`

#### Pagination Row
- `Button` "Previous" — `Command x:Bind ViewModel.PreviousPageCommand`; `IsEnabled x:Bind ViewModel.HasPreviousPage, Mode=OneWay`
- `TextBlock` — `x:Bind ViewModel.CurrentPage, Mode=OneWay` with simple text format (can use `x:Bind` string formatting or a converter)
- `Button` "Next" — `Command x:Bind ViewModel.NextPageCommand`; `IsEnabled x:Bind ViewModel.HasNextPage, Mode=OneWay`

### InventoryBulkEditDialog.xaml
A `ContentDialog` (not a Page) containing a CommunityToolkit.WinUI `DataGrid`:
- `x:DataType` set to `vm:InventoryBulkEditViewModel`
- `DataGrid` `ItemsSource x:Bind ViewModel.EditableItems, Mode=OneWay`
- Columns:
  - `DataGridTextColumn Header="Item" Binding="{x:Bind Name}"` (read-only)
  - `DataGridTextColumn Header="Stock Quantity" Binding="{x:Bind NewStockQuantity, Mode=TwoWay}"`
  - `DataGridTextColumn Header="Reorder Point" Binding="{x:Bind NewReorderPoint, Mode=TwoWay}"`
- `PrimaryButtonCommand x:Bind ViewModel.ConfirmCommand`
- `CloseButtonCommand x:Bind ViewModel.CancelCommand`
- `PrimaryButtonText="Apply Changes"`, `CloseButtonText="Cancel"`

### Code-behind: InventoryPage.xaml.cs
The code-behind must:
1. Get `InventoryViewModel` from DI and assign to `ViewModel` property
2. Subscribe to `ViewModel.BulkEditRequested` event
3. In the handler: create `InventoryBulkEditViewModel` (from DI or `new`), open `InventoryBulkEditDialog`, subscribe to `Confirmed`, on `Confirmed` call `await ViewModel.CommitBulkEditAsync(entries)`
4. On navigation (`OnNavigatedTo`): call `ViewModel.LoadCategoriesCommand.Execute(null)` and `ViewModel.LoadPageCommand.Execute(null)`
5. All of the above is delegation only — zero business logic

### Code-behind: InventoryBulkEditDialog.xaml.cs
- Expose `public InventoryBulkEditViewModel ViewModel { get; set; }` property set by the caller
- No logic

## Input Contract
From ViewModel Agent:

**InventoryViewModel:**
- `ObservableCollection<InventoryItemDto> InventoryItems`
- `ObservableCollection<InventoryCategoryDto> Categories`
- `InventoryCategoryDto? SelectedCategory` (ObservableProperty)
- `string SearchText` (ObservableProperty)
- `bool IsFilterAll`, `bool IsFilterLowStock`, `bool IsFilterOutOfStock`, `bool IsFilterRecentlyAdded`
- `bool IsBusy`, `string StatusMessage`
- `int TotalCount`, `int CurrentPage`, `int PageSize`
- `bool HasNextPage`, `bool HasPreviousPage`
- `ObservableCollection<InventoryItemDto> SelectedItems`
- `bool IsBulkEditBarVisible`
- Commands: `LoadPageCommand`, `LoadCategoriesCommand`, `NextPageCommand`, `PreviousPageCommand`, `SetFilterCommand` (param: int), `ToggleItemSelectionCommand` (param: InventoryItemDto), `OpenBulkEditCommand`
- Event: `BulkEditRequested`

**InventoryBulkEditViewModel:**
- `ObservableCollection<InventoryBulkEditRow> EditableItems`
- Commands: `ConfirmCommand`, `CancelCommand`
- Events: `Confirmed`, `Cancelled`

**InventoryBulkEditRow:**
- `Guid Id`, `string Name`
- `decimal NewStockQuantity`, `decimal NewReorderPoint` (TwoWay bindable)

**InventoryItemDto properties used in list template:**
- `Name`, `SkuCode`, `StockQuantity`, `ReorderPoint` (display only in list rows)

## Output Contract (Required)
- `InventoryPage.xaml` with all five feature areas present and bound
- `InventoryBulkEditDialog.xaml` ContentDialog with DataGrid
- `InventoryPage.xaml.cs` with DI wiring, navigation trigger, and bulk edit dialog delegation
- `InventoryBulkEditDialog.xaml.cs` with ViewModel property only

## Files to Create
- `src/Magidesk.Presentation/Views/InventoryBulkEditDialog.xaml` — ContentDialog with CommunityToolkit DataGrid
- `src/Magidesk.Presentation/Views/InventoryBulkEditDialog.xaml.cs` — minimal code-behind

## Files to Modify
- `src/Magidesk.Presentation/Views/InventoryPage.xaml` — complete overhaul; replace existing two-column editor layout with the five-feature layout described above
- `src/Magidesk.Presentation/Views/InventoryPage.xaml.cs` — add navigation trigger, BulkEditRequested subscription, dialog open/await logic

## Constraints
- Follow all rules in AI_ASSISTANT_RULES.md
- Compiled binding only — `x:Bind` everywhere; no `{Binding}` reflection binding
- No logic in code-behind beyond: DI wiring, navigation triggers, and immediate delegation to ViewModel
- Never invent ViewModel property names not in the output contract above
- All XAML changes require a manual clean + rebuild in Visual Studio Insider before marking complete
- ListView must use `ItemsStackPanel` to enable WinUI 3 virtualisation — do not explicitly set `VirtualizingStackPanel.IsVirtualizing="False"`
- `InventoryPage.xaml` must not exceed reasonable size — if it approaches 300 lines, split UI regions into `UserControl` components
- `x:DataType` must be set in all `DataTemplate` elements for compiled binding to work
- `EnhancedTableControl.xaml` uses a special code-behind pattern — do not use it as a reference for this task

## Acceptance Criteria
- `InventoryPage.xaml` contains a TextBox bound to `ViewModel.SearchText`
- Filter ribbon has four ToggleButtons bound to the four IsFilter* properties and calling `SetFilterCommand`
- Category TreeView is present and bound to `ViewModel.Categories`
- Item ListView rows have a CheckBox calling `ToggleItemSelectionCommand`
- Bulk action bar is present with `Visibility` driven by `IsBulkEditBarVisible`
- Pagination Previous/Next buttons are present and bound
- `InventoryBulkEditDialog.xaml` contains a DataGrid with three columns (Name read-only, Stock TwoWay, Reorder TwoWay)
- Code-behind subscribes to `BulkEditRequested` and opens the dialog
- Code-behind calls `LoadCategoriesCommand` and `LoadPageCommand` on navigation
- No `{Binding}` expressions anywhere in the new/modified XAML
- ⚠️ XAML CHANGE — requires manual clean + rebuild before marking complete.

## Do NOT
- Add any C# logic to code-behind beyond DI wiring, navigation triggers, and delegation
- Use `{Binding}` reflection binding anywhere
- Invent ViewModel properties not listed in the Input Contract
- Disable WinUI ListView virtualisation
- Reference `EnhancedTableControl.xaml` pattern for this control

## XAML Flag
YES ⚠️ — this task produces or modifies XAML. Agent must end output with:
"XAML CHANGE — requires manual clean + rebuild before marking complete."

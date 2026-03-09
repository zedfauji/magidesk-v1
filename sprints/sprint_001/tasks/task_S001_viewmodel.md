# Task Spec: TICKET-S001 — ViewModel Layer

## Ticket Summary
Split four oversized ViewModel files into partial classes to bring every file under 300 lines. No logic changes, no MVVM behavior changes, no binding surface changes.

## This Task's Responsibility
Split four ViewModel files using the C# `partial` keyword. For each file:
1. Add the `partial` keyword to the existing class declaration in the original file
2. Extract method groups into named partial files in the same folder
3. The original file retains: class declaration, all field declarations, all `[ObservableProperty]` / `[RelayCommand]` attribute declarations, constructor, and `Initialize` method
4. Extracted partial files contain method implementations only — no new fields, no new commands, no new observable properties

### ⚠️ Problem Files — Targeted Edits Only
All four ViewModels are listed in PROJECT_CONTEXT.md Problem Files:
- `OrderPageViewModel.cs` (2,293 lines) — Extract additions to partial: `OrderPageViewModel.[Feature].cs`
- `TableMapViewModel.cs` (1,105 lines) — Extract additions to partial: `TableMapViewModel.[Feature].cs`
- `SettlePageViewModel.cs` (919 lines) — Targeted edits only
- `TableDesignerViewModel.cs` (908 lines) — Targeted edits only

**Targeted edits means:** Do not reformat, reorder, or rename any member. Do not change indentation, whitespace style, or any other code you did not introduce. Make only the minimum edits required to split the file.

---

## Per-File Split Plan

### A. OrderPageViewModel.cs (2,293 lines)
Target: 10 partial files, each ≤ 300 lines

| Partial File | Feature Area |
|---|---|
| `OrderPageViewModel.cs` (retain) | Using directives, class declaration, all `[ObservableProperty]` / `[RelayCommand]` / field declarations, constructor, `InitializeAsync` |
| `OrderPageViewModel.DataLoading.cs` | Ticket load, table load, category load, product load methods |
| `OrderPageViewModel.ProductManagement.cs` | Product filtering, product selection, category filtering |
| `OrderPageViewModel.OrderItems.cs` | Add/edit/remove order item methods, modifier dialogs |
| `OrderPageViewModel.TicketOperations.cs` | Split ticket, merge ticket, hold ticket, transfer ticket |
| `OrderPageViewModel.PayNavigation.cs` | Navigate to settle, payment-related navigation helpers |
| `OrderPageViewModel.SessionManagement.cs` | Table session start/end, session state, real-time refresh |
| `OrderPageViewModel.Administration.cs` | Reprint, void, fire to kitchen, manager override |
| `OrderPageViewModel.Discounts.cs` | Apply/remove discount, gratuity, fee operations |
| `OrderPageViewModel.HelperModels.cs` | Inner helper classes or nested ViewModels declared inside the file, if any |

**Note:** If any resulting partial file exceeds 299 lines, split it further before completing the task. Adjust feature-area boundaries as needed.

---

### B. TableMapViewModel.cs (1,105 lines)
Target: 6 partial files, each ≤ 300 lines

| Partial File | Feature Area |
|---|---|
| `TableMapViewModel.cs` (retain) | Class declaration, fields, constructor, `InitializeAsync` |
| `TableMapViewModel.DataRefresh.cs` | LoadTables, RefreshTables, real-time polling |
| `TableMapViewModel.TableActions.cs` | Select, start session, end session, view details commands |
| `TableMapViewModel.ServerManagement.cs` | Server assignment, context menu operations |
| `TableMapViewModel.SessionDialogs.cs` | Start/End/Pause/Resume/TimeAdjust dialog helpers |
| `TableMapViewModel.Permissions.cs` | Permission checks, cleanup, navigation guards |

---

### C. SettlePageViewModel.cs (919 lines)
Target: 6 partial files, each ≤ 300 lines

| Partial File | Feature Area |
|---|---|
| `SettlePageViewModel.cs` (retain) | Class declaration, fields, constructor, `InitializeAsync` |
| `SettlePageViewModel.TenderEntry.cs` | Keypad input, clear, quick cash, amount calculation |
| `SettlePageViewModel.PaymentProcessing.cs` | Process payment, payment method selection, payment validation |
| `SettlePageViewModel.TicketModifications.cs` | Apply tip, hold ticket, split ticket, discount operations |
| `SettlePageViewModel.AdditionalOperations.cs` | Print receipt, tax exempt, navigation |
| `SettlePageViewModel.HelperModels.cs` | Inner helper classes (e.g. `PaymentMethodViewModel`) declared inside the file, if any |

---

### D. TableDesignerViewModel.cs (908 lines)
Target: 6 partial files, each ≤ 300 lines

| Partial File | Feature Area |
|---|---|
| `TableDesignerViewModel.cs` (retain) | Class declaration, fields, constructor, `InitializeAsync` |
| `TableDesignerViewModel.DataLoading.cs` | Load floors, load tables |
| `TableDesignerViewModel.TableOperations.cs` | Add table, delete table, update position |
| `TableDesignerViewModel.LayoutManagement.cs` | Save/load/new/clone/delete/publish layout |
| `TableDesignerViewModel.UIInteractions.cs` | Drag, select, mode toggle, validation |
| `TableDesignerViewModel.Performance.cs` | Visible tables, virtualization, performance metrics |

---

## Input Contract
- Output contract from Infrastructure Agent confirming SalesReportRepository split and build passing

## Output Contract (Required)
The ViewModel Agent must end its response with the standard OUTPUT CONTRACT block listing:
- All new partial files created (paths) for each of the 4 ViewModels
- Confirmation that each original ViewModel file has the `partial` keyword added
- Confirmation that every resulting file is under 300 lines
- Confirmation that zero logic, observable property, or command was changed
- Observable property names and command names: UNCHANGED (no new members added, no members removed)
- XAML build required: YES (partial class splits require a VS clean + rebuild to verify compiled binding still resolves)
- Handoff to: Test Agent

## Files to Create

### OrderPageViewModel partials:
- `src/Magidesk.Presentation/ViewModels/OrderPageViewModel.DataLoading.cs`
- `src/Magidesk.Presentation/ViewModels/OrderPageViewModel.ProductManagement.cs`
- `src/Magidesk.Presentation/ViewModels/OrderPageViewModel.OrderItems.cs`
- `src/Magidesk.Presentation/ViewModels/OrderPageViewModel.TicketOperations.cs`
- `src/Magidesk.Presentation/ViewModels/OrderPageViewModel.PayNavigation.cs`
- `src/Magidesk.Presentation/ViewModels/OrderPageViewModel.SessionManagement.cs`
- `src/Magidesk.Presentation/ViewModels/OrderPageViewModel.Administration.cs`
- `src/Magidesk.Presentation/ViewModels/OrderPageViewModel.Discounts.cs`
- `src/Magidesk.Presentation/ViewModels/OrderPageViewModel.HelperModels.cs` (only if inner classes exist)

### TableMapViewModel partials:
- `src/Magidesk.Presentation/ViewModels/TableMapViewModel.DataRefresh.cs`
- `src/Magidesk.Presentation/ViewModels/TableMapViewModel.TableActions.cs`
- `src/Magidesk.Presentation/ViewModels/TableMapViewModel.ServerManagement.cs`
- `src/Magidesk.Presentation/ViewModels/TableMapViewModel.SessionDialogs.cs`
- `src/Magidesk.Presentation/ViewModels/TableMapViewModel.Permissions.cs`

### SettlePageViewModel partials:
- `src/Magidesk.Presentation/ViewModels/SettlePageViewModel.TenderEntry.cs`
- `src/Magidesk.Presentation/ViewModels/SettlePageViewModel.PaymentProcessing.cs`
- `src/Magidesk.Presentation/ViewModels/SettlePageViewModel.TicketModifications.cs`
- `src/Magidesk.Presentation/ViewModels/SettlePageViewModel.AdditionalOperations.cs`
- `src/Magidesk.Presentation/ViewModels/SettlePageViewModel.HelperModels.cs` (only if inner classes exist)

### TableDesignerViewModel partials:
- `src/Magidesk.Presentation/ViewModels/TableDesignerViewModel.DataLoading.cs`
- `src/Magidesk.Presentation/ViewModels/TableDesignerViewModel.TableOperations.cs`
- `src/Magidesk.Presentation/ViewModels/TableDesignerViewModel.LayoutManagement.cs`
- `src/Magidesk.Presentation/ViewModels/TableDesignerViewModel.UIInteractions.cs`
- `src/Magidesk.Presentation/ViewModels/TableDesignerViewModel.Performance.cs`

## Files to Modify
- `src/Magidesk.Presentation/ViewModels/OrderPageViewModel.cs` — Add `partial` keyword; remove methods that move to partials. Retain all field, property, and command declarations.
- `src/Magidesk.Presentation/ViewModels/TableMapViewModel.cs` — Add `partial` keyword; remove methods that move to partials.
- `src/Magidesk.Presentation/ViewModels/SettlePageViewModel.cs` — Add `partial` keyword; remove methods that move to partials.
- `src/Magidesk.Presentation/ViewModels/TableDesignerViewModel.cs` — Add `partial` keyword; remove methods that move to partials.

## Constraints
- Follow all rules in `.agent/rules/`
- Maximum file line limit: 300 lines per `.cs` file
- One class per file (partial keyword allowed — same class, same namespace)
- No silent failures
- NO logic changes of any kind
- NO new observable properties or relay commands
- NO new DI registrations (ticket explicitly excludes this)
- All `[ObservableProperty]` and `[RelayCommand]` attribute-decorated fields/methods remain in the primary file (`ClassName.cs`), not in partials — this ensures the CommunityToolkit.Mvvm source generator finds them reliably
- Namespace: `Magidesk.Presentation.ViewModels`
- Partial files must declare `internal partial class [ClassName]` matching the original access modifier

## Acceptance Criteria
- [ ] All 4 original ViewModel files have `partial` keyword added
- [ ] Every resulting file (original + all partials) is under 300 lines
- [ ] No observable property or command was added, removed, or renamed
- [ ] No method signature was changed
- [ ] `dotnet build` passes with 0 errors after split
- [ ] XAML bindings remain valid (no ViewModel member was removed from the class surface)

## Do NOT
- Change any business logic or ViewModel behavior
- Move `[ObservableProperty]` or `[RelayCommand]` decorated declarations to partial files
- Add or remove DI registrations
- Rewrite or reformat code you did not introduce
- Rename any ViewModel class, method, property, or field
- Touch any XAML files
- Create sub-ViewModels (explicitly out of scope per ticket)

## XAML Flag
YES ⚠️ — partial class splits for ViewModels require manual clean + rebuild in Visual Studio to ensure compiled x:Bind still resolves. Agent must end output with:
"XAML CHANGE — requires manual clean + rebuild before marking complete."

# Orphan UI Surfaces (Current App)

**Definition**: A surface with **no inbound navigation** discovered from explicit code-level evidence.

**Important**: This list is based on the current extraction in `navigation_entries.md` / `navigation_graph.mmd`. Any additional navigation triggers discovered later may remove items from this list.

## Pages
- **MainPage**
  - **Evidence**: No `Navigate(typeof(Views.MainPage))` found.
  - **Status**: ORPHAN (by current evidence)

- **OrderTypeExplorerPage / ShiftExplorerPage / MenuEditorPage / ModifierEditorPage / InventoryPage**
  - **Inbound**: only via `BackOfficePage` internal navigation.
  - **Status**: NOT orphan (reachable via BackOfficePage)

## Dialogs
- **PasswordEntryDialog**
  - **Evidence**: No `new PasswordEntryDialog()` usage found in scanned code.
  - **Status**: ORPHAN (by current evidence)

- **SplitTicketDialog_Backup / SplitTicketDialog_Fixed / SplitTicketDialog_Minimal**
  - **Evidence**: No navigation call sites reference these files by filename; call sites reference `SplitTicketDialog` type.
  - **Status**: ORPHAN AS FILE VARIANTS (by current evidence)
  - **Note**: These are variant XAML files in the repo; runtime usage is **UNKNOWN** without build/project inclusion inspection.

- **Views/Dialogs/CashEntryDialog** (`Magidesk.Presentation.Views.Dialogs.CashEntryDialog`)
  - **Evidence**: No call sites found for this type; Cash entry appears to use `Views/CashEntryDialog`.
  - **Status**: ORPHAN (by current evidence)

## UNKNOWN / requires additional verification
- **MainWindow sidebar items**: confirm any additional tags beyond those in `MainWindow.xaml` (current scan shows fixed set).

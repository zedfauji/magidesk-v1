# Drift Report

## Scope
Drift items are grouped as:
- Backend vs Frontend drift
- Docs vs Code drift
- FloreantPOS vs Current App drift
- Runtime safety drift

Each item includes **evidence** and **impact**.

## Backend vs Frontend drift

### 1) Commands exist without confirmed UI surfaces
- **What is drifting**: Application-layer commands exist (e.g., transfer/refund) but no corresponding UI surface was confirmed in this audit pass.
- **Evidence**:
  - Commands exist: `Magidesk.Application/Commands/TransferTicketCommand.cs`, `RefundTicketCommand.cs`, `RefundPaymentCommand.cs`.
  - No directly observed view/dialog names for these flows in `Views/**/*.xaml` list.
- **Impact**: Medium

### 2) Table-required order types block ticket creation
- **What is drifting**: Backend model supports table assignment commands, but the primary ticket creation flow blocks with TODO when table is required.
- **Evidence**: `ViewModels/SwitchboardViewModel.cs` contains TODO for table selection and returns early.
- **Impact**: High

## Docs vs Code drift

### 3) Dialog vs Page mismatch for settlement
- **What is drifting**: Docs describe a settle dialog pattern; current app uses a page.
- **Evidence**:
  - Docs: `docs/forensic-ui-audit/drift-list.md` states settle implemented as `SettlePage.xaml` instead of modal dialog.
  - Code: `Views/SettlePage.xaml` exists.
- **Impact**: Medium

### 4) “Tables Not Implemented” stub contradicts docs that expect a table workflow
- **What is drifting**: Docs define table selection and table map behaviors; switchboard tables entry is stubbed.
- **Evidence**:
  - Code: `SwitchboardViewModel.TablesCommand = Debug.WriteLine("Tables Not Implemented")`.
  - Docs: `/docs/forensic-ui-audit/features/F-0082-table-map-view.md` (exists in docs corpus).
- **Impact**: High

### 5) No Sale behavior is simulated in settle viewmodel
- **What is drifting**: Docs expect drawer kick behavior; current code uses a simulated delay + message.
- **Evidence**: `SettleViewModel.OnNoSaleAsync()` contains TODO and sets `Error = "Drawer Opened (No Sale)"`.
- **Impact**: High

## FloreantPOS vs Current App drift

### 6) Floreant uses modal dialogs to preserve ticket context
- **What is drifting**: Floreant commonly uses dialogs for settlement/modifiers/split; current app uses a mix (some dialogs exist, settle is a page).
- **Evidence**:
  - Floreant: `ui/views/payment/SettleTicketDialog.java`, `ui/views/order/modifier/ModifierSelectionDialog.java`, `ui/views/SplitTicketDialog.java`.
  - Current app: `Views/SettlePage.xaml` (page), dialogs exist for modifiers and split.
- **Impact**: Medium

### 7) Floreant boot-time DB configuration prompt
- **What is drifting**: Floreant prompts DB config on connection failure; current app shows loading overlay error string (behavior depends on `ISystemInitializationService`).
- **Evidence**:
  - Floreant: `Application.initializeSystem()` catches `DatabaseConnectionException` and offers to open `DatabaseConfigurationDialog`.
  - Current app: `App.OnLaunched` displays `mainWindow.ShowLoading("Startup Failed: ...")`.
- **Impact**: Medium

## Runtime safety drift

### 8) Hardcoded identifiers used in critical workflows
- **What is drifting**: Ticket creation and cash operations fall back to hardcoded GUIDs.
- **Evidence**: `SwitchboardViewModel.cs` uses hardcoded user/terminal/shift IDs; also used in drawer operations.
- **Impact**: Critical

### 9) Dialog XamlRoot requirements can throw at runtime
- **What is drifting**: Showing a dialog without a loaded `Frame.XamlRoot` throws.
- **Evidence**: `NavigationService.ShowDialogAsync` throws `InvalidOperationException("Frame's XamlRoot is null")`.
- **Impact**: High

### 10) Error handling is often debug-print only
- **What is drifting**: Many exception paths are logged to debug output without user-visible feedback.
- **Evidence**: `SwitchboardViewModel.NewTicketAsync()` catch prints `Create Ticket Failed: ...` without UI.
- **Impact**: Medium

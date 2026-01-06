# Settle UI (SUI) Audit Tickets

| ID | Title | Severity | Status | Owner |
| :--- | :--- | :--- | :--- | :--- |
| **SUI-001** | Fix "No Sale" Semantic Error | **HIGH** | DONE | Antigravity |
| **SUI-002** | Fix Action Button Row Layout | **MEDIUM** | DONE | Antigravity |
| **SUI-003** | Add Ticket & Table Context to Summary | **MEDIUM** | DONE | Antigravity |
| **SUI-004** | Apply Currency Formatting to Financials | **LOW** | DONE | Antigravity |
| **SUI-005** | Standardize Settle Button Styles (Cancel) | **LOW** | DONE | Antigravity |

## Details

### SUI-001: Fix "No Sale" Semantic Error
*   **Location**: `SettleViewModel.cs`, Method `OnNoSaleAsync`
*   **Failure**: Success message "Drawer Opened (No Sale)" is mapped to the `Error` property, causing it to display as a red failure in UI.
*   **Requirement**: Introduce a `StatusMessage` property or use a multi-severity InfoBar. Decouple successful operations from the error channel.

### SUI-002: Fix Action Button Row Layout
*   **Location**: `SettlePage.xaml`
*   **Failure**: `TEST WAIT`, `Next $`, `No Sale`, and `Exact` buttons are currently overlapping or stacked in a 2-column grid.
*   **Requirement**: Refactor into a 4-column row to match the intended design screenshot.

### SUI-003: Add Ticket & Table Context to Summary
*   **Location**: `SettlePage.xaml`, Summary Panel
*   **Failure**: Panel only shows financial amounts; cashier cannot see which Ticket # or Table Name is active.
*   **Requirement**: Add `TicketNumber` and `TableName` readouts to the summary panel header.

### SUI-004: Apply Currency Formatting to Financials
*   **Location**: `SettlePage.xaml`
*   **Failure**: Amounts show as raw numbers (e.g., `0`) instead of localized currency (e.g., `$0.00`).
*   **Requirement**: Register `CurrencyConverter` and apply it to all financial `TextBlock` bindings.

### SUI-005: Standardize Settle Button Styles
*   **Location**: `SettlePage.xaml`
*   **Failure**: "Cancel" button is oversized and uses non-standard "Gray" background.
*   **Requirement**: Align button heights and use standard POS action styles.

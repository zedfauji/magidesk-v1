# Printing System Feature Inventory

## 1. Core Services

### IPrintingService (Generic)
- **Status**: Exists, Implemented as `PrintingService`.
- **State**: **STUB**. methods contain `Task.Delay` and `Debug.WriteLine` only.
- **Reference**: `Magidesk.Infrastructure.Services.PrintingService`
- **Gap**: Not connected to mocked interfaces below.

### IKitchenPrintService (Domain Specific)
- **Status**: Exists, Implemented as `MockKitchenPrintService`.
- **State**: **MOCK**. Logs output to console.
- **Features**:
  - Print Ordered Items (OrderLines)
  - Filter by `ShouldPrintToKitchen`
  - Track `PrintedToKitchen` status
- **Gap**: Needs real implementation using `IPrintingService` (or direct printer access).

### IReceiptPrintService (Domain Specific)
- **Status**: Exists, Implemented as `MockReceiptPrintService`.
- **State**: **MOCK**. Logs output + Creates Audit Event.
- **Features**:
  - Print Ticket Receipt
  - Print Payment Receipt
  - Print Refund Receipt
- **Gap**: Needs real implementation.

## 2. Printer Configuration

### Domain Entities
- **PrinterGroup**: Represents virtual destination (e.g., "Kitchen", "Bar").
- **PrinterMapping**: Maps Group + Terminal -> Physical Printer Name.
- **Status**: Entities exist.
- **Gap**: No CRUD Commands found for these entities (except maybe migration seeds). UI likely missing.

## 3. Operations

### Kitchen Printing
- **Command**: `PrintToKitchenCommand`
- **Handler**: `PrintToKitchenCommandHandler` (Uses `IKitchenPrintService`)
- **Status**: Wired up in backend, using Mocks.

### Receipt Printing
- **Command**: `PrintReceiptCommand`
- **Handler**: `PrintReceiptCommandHandler` (Uses `IReceiptPrintService`)
- **Status**: Wired up in backend, using Mocks.

### Cash Drawer
- **Command**: NONE.
- **Logic**: NONE using "Open" or "Kick" keywords.
- **Gap**: **CRITICAL**. No way to open cash drawer programmatically.

### Reports
- **Queries**: Extensive (`GetShiftReport`, `GetDailySales`, etc.)
- **Printing**: NONE.
- **Gap**: **CRITICAL**. Reports are data-only. No print formatting or command exists.

## 4. UI Layer
- **Views**: `PrintPage.xaml` (Likely a test/debug page).
- **ViewModels**: `PrintViewModel.cs`.
- **Gap**: No dedicated "Printer Setup" UI found in search results.

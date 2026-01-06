# Printing System TODO Map

## 1. Infrastructure Layer
- [x] **Real Implementation**: Replaced Mocks with `WindowsPrintingService` and `EscPosDriver`.
- [x] **Unification**: Architecture defined (`IReceiptPrintService`, `IRawPrintService`).
- [x] **Physical Implementation**: ESC/POS logic implemented via `EscPosHelper` and `EscPosDriver`.

## 2. Configuration
- [x] **CRUD API**: `PrinterGroupRepository` and `PrinterMappingRepository` active.
- [x] **UI**: "System & Printers" page active.
  - [x] List detected Windows printers.
  - [x] Create/Edit Printer Groups.
  - [x] Map Terminal -> Group -> Physical Printer.
  
## 3. Cash Drawer
- [x] **Service**: `ICashDrawerService` implemented.
- [x] **Command**: `OpenCashDrawerCommand` implemented.
- [x] **Trigger**: Auto-fires on Cash Payment completion (in `ReceiptPrintService`).

## 4. Reports
- [ ] **Formatting**: Create text-based (ESC/POS style) or Graphic-based report generators for:
  - Shift Report
  - Daily Sales
- [ ] **Command**: Create `PrintReportCommand` accepting Report Type and Data/ID.

## 5. Workflow Integration
- [x] **Payment**: Receipt + Drawer Kick fires on success.
- [x] **Kitchen**: Items route to correct groups (e.g. Bar vs Kitchen).

## 6. Template System (NEW)
- [x] **Backend**: Template Engine and Schema.
- [x] **Frontend**: Editor and Preview UI.
- [x] **Integration**: Hybrid Service Service.

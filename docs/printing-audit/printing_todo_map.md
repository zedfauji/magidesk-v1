# Printing System TODO Map

## 1. Infrastructure Layer
- [ ] **Real Implementation**: Replace `MockKitchenPrintService` and `MockReceiptPrintService` with real implementations that use `System.Drawing.Printing`.
- [ ] **Unification**: Decide if `PrintingService` should implement the specific interfaces or be a low-level driver used by them. (Recommendation: Low-level driver).
- [ ] **Physical Implementation**: Implement ESC/POS logic or Windows Printer logic.

## 2. Configuration
- [ ] **CRUD API**: Create Commands/Queries for `PrinterGroup` and `PrinterMapping`.
- [ ] **UI**: Create "Printers & Devices" settings page.
  - List detected Windows printers.
  - Create/Edit Printer Groups.
  - Map Terminal -> Group -> Physical Printer.

## 3. Cash Drawer
- [ ] **Service**: Add `IOpenDrawerService` or method on `IPrintingService`.
- [ ] **Command**: Create `OpenCashDrawerCommand`.
- [ ] **Trigger**: Auto-fire on Cash Payment completion.

## 4. Reports
- [ ] **Formatting**: Create text-based (ESC/POS style) or Graphic-based report generators for:
  - Shift Report
  - Daily Sales
- [ ] **Command**: Create `PrintReportCommand` accepting Report Type and Data/ID.

## 5. Workflow Integration
- [ ] **Payment**: Ensure Receipt + Drawer Kick fires on success.
- [ ] **Kitchen**: Ensure correct items route to correct groups (e.g. Bar vs Kitchen).

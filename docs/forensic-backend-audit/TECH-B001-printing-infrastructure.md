# TECH-B001: Printing Infrastructure Implementation

## Description
Implement the core printing services and physical printer adapters which are currently stubs.

## Scope
-   **Target Component**: `Magidesk.Infrastructure.Services.PrintingService`
-   **Adapters**:
    -   `Thermal80mmAdapter` (ESC/POS)
    -   `Thermal58mmAdapter` (ESC/POS)
    -   `StandardPageAdapter` (PDF/HTML)

## Implementation Tasks
- [ ] Replace `PrintingService` stubs with real `System.Drawing.Printing` or `RawPrinterHelper` calls.
- [ ] Implement ESC/POS command generation in `Thermal80mmAdapter`.
- [ ] Implement ESC/POS command generation in `Thermal58mmAdapter`.
- [ ] Implement PDF/HTML generation in `StandardPageAdapter`.
- [ ] Integrate with `PrinterMapping` repository to resolve physical printers.

## Acceptance Criteria
-   `PrintingService.PrintTicket()` sends actual bytes to a physical printer.
-   `PrintingService.PrintReceipt()` supports both 80mm and 58mm formats.

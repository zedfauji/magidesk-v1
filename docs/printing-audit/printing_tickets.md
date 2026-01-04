# Printing System Tickets

## Backend Infrastructure (PRT-BE)

### PRT-BE-001: Implement Low-Level Printing Service
*   **Goal**: Create a service that wraps `System.Drawing.Printing` to handle raw printing.
*   **Requirements**:
    *   List installed printers.
    *   Send raw string data (or graphics) to a specific printer.
    *   Handle `Win32Exception` and wrapper in `PrintingException`.
    *   Support ESC/POS byte commands (for drawer kick).

### PRT-BE-002: Implement Real Kitchen Print Service
*   **Goal**: Replace `MockKitchenPrintService` with `KitchenPrintService`.
*   **Requirements**:
    *   Inject `IPrintingService`.
    *   Format Ticket for Kitchen (Large Text, Table Name, Server, Date).
    *   Group items by PrinterGroup (if logic requires splitting ticket).
    *   Format Modifiers (indented, red if possible/supported, or bold).
    *   Verify `ShouldPrintToKitchen` logic is respected.

### PRT-BE-003: Implement Real Receipt Print Service
*   **Goal**: Replace `MockReceiptPrintService` with `ReceiptPrintService`.
*   **Requirements**:
    *   Format Customer Receipt (Restaurant Name, Address, Ticket #).
    *   List Items, Quantities, Prices.
    *   Show Subtotal, Tax, Total, Payments, Change.
    *   Support Refunds/Voids formatting.

### PRT-BE-004: Implement Cash Drawer Logic
*   **Goal**: Programmatic control of Cash Drawer.
*   **Requirements**:
    *   Method `OpenDrawer(string printerName)`.
    *   Send ESC/POS Pulse Command (`<1B>p<00><19><FA>`).
    *   Create `OpenCashDrawerCommand`.

### PRT-BE-005: Implement Report Printing
*   **Goal**: Print Shift/Day Reports.
*   **Requirements**:
    *   Create `PrintReportCommand`.
    *   Create `IReportFormatter` interface and implementations for Shift/Sales reports.
    *   Format DTO data into printable text/layout.

## Frontend Configuration (PRT-CFG)

### PRT-CFG-001: Printer Configuration UI
*   **Goal**: UI to map Virtual Groups to Physical Printers.
*   **Requirements**:
    *   View: `PrinterConfigurationPage`.
    *   List `PrinterMapping`s for current Terminal.
    *   Dropdown of discovered Windows Printers.
    *   Test Print button.

## Workflow Integration (PRT-FLW)

### PRT-FLW-001: Wire Payment Workflows
*   **Goal**: specific triggers.
*   **Requirements**:
    *   `PayCashCommand` -> Success -> Kick Drawer + Print Receipt.
    *   `PayCardCommand` -> Success -> Print Receipt (optional/prompt).

### PRT-FLW-002: Wire Report Workflows
*   **Goal**: Print buttons on Report Screens.
*   **Requirements**:
    *   Add "Print" button to Report Viewer.
    *   Invoke `PrintReportCommand`.

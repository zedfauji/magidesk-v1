# Printing Subsystem Audit

## Executive Summary
The Magidesk Printing Subsystem relies on a **Raw Printing** architecture, utilizing ESC/POS command sequences sent directly to the printer via the Windows Spooler. It explicitly bypasses GDI+ (System.Drawing) for receipts and kitchen tickets, favoring speed and control over graphical layout.

**Critical Finding**: The **Receipt Printing Trigger is MISSING** from the automated payment workflow. While the code for printing receipts exists (`ReceiptPrintService`), it is not invoked by `ProcessPaymentCommandHandler` or `SettleViewModel` after a successful transaction.

## 1. Architecture Overview

### 1.1 Core Components
*   **Virtual Layer**: `PrinterGroup` (Kitchen, Bar, Receipt) defines *logical* destinations.
*   **Hardware Layer**: `PrinterMapping` maps a Virtual Group to a Physical Windows Printer Name per Terminal.
*   **Service Layer**:
    *   `KitchenPrintService`: Explicitly handles ticket order lines.
    *   `ReceiptPrintService`: Handles full ticket receipts.
    *   `WindowsPrintingService`: The low-level implementation of `IRawPrintService`, likely using `RawPrinterHelper` or P/Invoke to send bytes to the spooler.
    *   `KitchenRoutingService`: Handles KDS (Kitchen Display System) routing via database.

### 1.2 Data Flow
1.  **Command**: `PrintToKitchenCommand` or `PrintReceiptCommand`.
2.  **Handler**: Resolves `Ticket` and `OrderLines`.
3.  **Service**: `KitchenPrintService` determines the `PrinterGroup` for each line.
4.  **Mapping**: Looks up `PrinterMapping` for the current Terminal + PrinterGroup.
5.  **Generation**:
    *   **Template**: If a Liquid template is defined, it renders to an intermediate object model, then to ESC/POS.
    *   **Fallback**: Generates hardcoded ESC/POS byte arrays (`EscPosHelper`).
6.  **Transport**: `WindowsPrintingService` sends the raw bytes to the Windows Spooler.

## 2. Workflow Audit

### 2.1 Kitchen Printing (Implemented)
*   **Trigger**: Explicit button press in `OrderEntryViewModel` (`PrintTicketCommand`).
*   **Logic**:
    *   Checks `ShouldPrintToKitchen` flag on OrderLines.
    *   Filters out already printed lines (`!PrintedToKitchen`).
    *   Groups by `PrinterGroup`.
    *   Updates `PrintedToKitchen` flag on success.
    *   **Status**: Healthy, but relies on manual trigger.

### 2.2 Receipt Printing (Incomplete)
*   **Trigger**:
    *   **Manual**: `SettleViewModel` -> `ReprintReceiptCommand` -> `PrintReceiptCommandHandler` (Exists).
    *   **Automated**: **MISSING**. `SettleViewModel.ProcessPaymentAsync` processes the payment and closes the screen without printing. `ProcessPaymentCommandHandler` records the payment but initiates no printing side-effect.
*   **Status**: **Broken/Incomplete**. Customers will not receive receipts automatically.

## 3. Risk Assessment

### 3.1 Hardcoded ESC/POS Dependency
*   **Risk**: High.
*   **Description**: The fallback generation logic (`EscPosHelper`) produces raw ESC/POS bytes.
*   **Impact**: This will **fail** if the user attempts to use a standard Inkjet/Laser printer or a non-ESC/POS thermal printer (e.g., Star Micronics uses StarPRNT, though often has emulation).
*   **Mitigation**: Implement a GDI+ / XPS path (e.g., `StandardPage` format in `PrinterMapping`) which draws text to a `PrintDocument`. The code hints at `PrinterFormat.StandardPage` support but the implementation in `KitchenPrintService` creates a plain text ticket, which might not print correctly on all Windows drivers without formatting.

### 3.2 Missing Receipt Trigger
*   **Risk**: Critical (Functional).
*   **Impact**: Operational friction; staff must manually reprint receipts.
*   **Fix**: Inject `IReceiptPrintService` into `SettleViewModel` or `ProcessPaymentCommandHandler` and invoke `PrintTicketReceiptAsync` upon `ProcessPaymentResult.Success`.

### 3.3 Concurrency & State
*   **Risk**: Low/Medium.
*   **Description**: `PrintedToKitchen` flag is updated in memory and persisted.
*   **Mitigation**: Ensure `SaveChanges` is called after marking lines as printed to prevent duplicate printing on crash/restart. (Verified: `KitchenPrintService` updates flag, caller usually handles persistence, need to double-check `PrintToKitchenCommandHandler`).

## 4. Recommendations

1.  **Implement Auto-Print**: Update `SettleViewModel` to call `PrintReceiptCommand` immediately after a successful payment.
2.  **Verify Raw Printer Support**: Confirm `WindowsPrintingService` robustly handles driver names and errors.
3.  **Standardize Template Usage**: Move away from hardcoded ESC/POS fallback to Liquid Templates as the primary mechanism to allow field-customization.

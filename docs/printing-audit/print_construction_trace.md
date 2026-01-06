# Print Construction Trace

## Overview
This document traces the end-to-end execution path for all printing operations in the application, identifying trigger points, data aggregation, formatting, and final output generation.

## 1. Customer Receipt
**Status**: Implemented (Hardcoded ESC/POS)

*   **Trigger**:
    *   **UI**: `PaymentHubViewModel` (Auto-print options) or `TicketViewModel` (Print Button).
    *   **Command**: `PrintReceiptCommand`.
*   **Entry Point**: `PrintReceiptCommandHandler.HandleAsync`.
*   **Service**: `ReceiptPrintService.PrintTicketReceiptAsync`.
*   **Data Aggregation**:
    *   Uses `Ticket` entity (loaded via `ITicketRepository`).
    *   Resolves `User` (Server Name) via `IUserRepository`.
    *   Resolves `PrinterMapping` via `IPrinterMappingRepository`.
*   **Formatting Layer**:
    *   **Logic**: Mixed inside `ReceiptPrintService.GenerateReceiptBytes`.
    *   **Engine**: `EscPosHelper` (Manual byte generation).
    *   **Layout**: Hardcoded inline (Header, Address, Item Loop, Totals).
*   **Output**:
    *   Sends raw bytes to `IRawPrintService` -> `WindowsPrintingService` (WinSpool).

## 2. Kitchen Ticket
**Status**: Implemented (Hardcoded ESC/POS, Supports Plain Text via switch)

*   **Trigger**:
    *   **UI**: `OrderEntryViewModel` ("Send" Button).
    *   **Command**: `PrintToKitchenCommand`.
*   **Entry Point**: `PrintToKitchenCommandHandler.HandleAsync`.
*   **Service**: `KitchenPrintService.PrintTicketAsync`.
*   **Data Aggregation**:
    *   Uses `Ticket` entity.
    *   Filters `OrderLines` by `ShouldPrintToKitchen` and `!PrintedToKitchen`.
    *   Groups lines by `PrinterGroupId`.
*   **Formatting Layer**:
    *   **Logic**: `KitchenPrintService.GenerateTicketBytes` (Thermal) or `GeneratePlainTextTicket` (StandardPage).
    *   **Engine**: `EscPosHelper` (Thermal) or `StringBuilder` (Text).
    *   **Layout**: Hardcoded inline.
*   **Output**:
    *   Iterates via `PrinterGroup`, resolves Mapping, sends bytes to `IRawPrintService`.

## 3. Reports (Drawer, Shift, Sales)
**Status**: **NOT IMPLEMENTED** (Gap)

*   **Findings**:
    *   `DrawerPullReportViewModel` contains `PrintAsync` with `// TODO`.
    *   No `ReportPrintService` exists.
    *   `IRawPrintService` is not injected into any Report ViewModels.
    *   Reports are currently display-only in the Back Office.

## 4. Bar Ticket
**Status**: Implemented (Shared with Kitchen Logic)

*   Executed via `KitchenPrintService` using the same logic as Kitchen Tickets.
*   Differentiated only by `PrinterGroupId` configuration (e.g., "Bar" group vs "Kitchen" group).

## Key Observations
1.  **Direct Coupling**: Formatting logic is tightly coupled with the Service layer (`GenerateReceiptBytes` inside `ReceiptPrintService`).
2.  **Hardcoded Strings**: Headers ("MAGIDESK POS"), Addresses, and Labels ("Check:", "Date:") are string literals.
3.  **No Template Engine**: All layout is procedural C# code using `EscPosHelper`.
4.  **Missing Report Printing**: A major functional gap exists for physical report printing.

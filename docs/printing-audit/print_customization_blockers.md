# Print Customization Blockers

## 1. Hardcoded Business Identity
*   **Location**: `ReceiptPrintService.cs` (Line ~244, ~249)
*   **Blocker**: Restaurant Name ("MAGIDESK POS") and Address ("123 Main Street") are string literals.
*   **Impact**: Critical. Users cannot configure their own business name or legal address on receipts.

## 2. Hardcoded Layout Structure
*   **Location**: `ReceiptPrintService.cs`, `KitchenPrintService.cs`
*   **Blocker**: The sequence of printing (Header -> Info -> Items -> Totals) is defined by the procedural order of code execution.
*   **Impact**: Cannot reorder sections (e.g., put "Server Name" at the bottom).

## 3. Formatting & Localization
*   **Location**: Inline String Interpolation (e.g., `$"Chk: {ticket.TicketNumber}"`)
*   **Blocker**: Labels are baked into the logic.
*   **Impact**:
    *   Cannot translate labels (En/Es/Fr).
    *   Cannot change terminology (e.g., "Table" vs "Seat").
    *   Cannot change date format (US vs EU).

## 4. Printer Width Assumptions
*   **Location**: `int width = mapping.PrintableWidthChars > 0 ? mapping.PrintableWidthChars : 32;`
*   **Blocker**: While configurable via DB, the *rendering strategy* (filling lines with `-`) is primitive text manipulation.
*   **Impact**: Complex layouts (columns) will break on different widths (58mm vs 80mm) without a smarter layout engine.

## 5. Missing Template Engine
*   **Location**: Entire Architecture
*   **Blocker**: There is no abstraction for a "Document" or "Template". The Service *is* the Template.
*   **Impact**: Introducing customization requires refactoring the Service to accept an external definition (Template) and a data source (Ticket) to produce output.

## 6. Image / Logo Support
*   **Location**: `EscPosHelper` exists but is not wired to dynamic image sources.
*   **Blocker**: No logic to load a user-defined image blob and convert it to ESC/POS bitmap commands.
*   **Impact**: Receipts look generic/unprofessional without branding.

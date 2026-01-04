# Printer Configuration Model

## Overview
Printer configuration is managed per **Terminal** via the `PrinterMapping` entity. We do not configure "System Printers" globally; we configure "How a Terminal uses a System Printer".

## Configuration Elements

### 1. Printer Format
Defined in `PrinterMapping`.
*   **Type:** Enum `PrinterFormat`
*   **Values:** `Thermal58mm`, `Thermal80mm`, `StandardPage`
*   **Default:** `Thermal80mm`
*   **Purpose:** Determines which `ILayoutAdapter` is used by the `PrintLayoutEngine`.

### 2. Auto-Cut Support
Defined in `PrinterMapping` (Proposed).
*   **Type:** `bool`
*   **Default:** `true`
*   **Purpose:** Whether to send the cut command.

### 3. Cash Drawer Trigger
Defined in `PrinterMapping` (Existing logic utilizes Printer Group properties, but trigger might be per-mapping).
*   *Clarification:* Currently `CashDrawerService` sends to "Receipt" printer. We might explicitly enable/disable this per mapping if needed, but for now, implicit association with "Receipt" Group is sufficient.

## Database Schema (Proposed)

```sql
ALTER TABLE "UseCases"."PrinterMappings"
ADD COLUMN "Format" integer NOT NULL DEFAULT 1; -- 1 = Thermal80mm
ADD COLUMN "CutEnabled" boolean NOT NULL DEFAULT TRUE;
```

## Entity Model

```csharp
public class PrinterMapping
{
    // ... Existing
    public PrinterFormat Format { get; private set; }
    public bool CutEnabled { get; private set; }

    public void UpdateConfiguration(string physicalName, PrinterFormat format, bool cutEnabled) { ... }
}
```

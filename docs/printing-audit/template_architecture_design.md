# Template Architecture Design

## 1. Template Format
*   **Choice**: **Liquid** (via `Fluid` library) or **Handlebars.Net**.
*   **Rationale**: Logic-less, safe, widely understood, text-based.
*   **Storage**: Stored as strings in the `PrintTemplates` database table.

## 2. Abstract Document Model (ADM)
Instead of templates generating raw bytes directly, they will generate an **Intermediate Representation** (Abstract Document Model). This decouples the "Design" from the "Hardware".

**Structure**:
```json
{
  "Elements": [
    { "Type": "Image", "Source": "Logo" },
    { "Type": "Text", "Content": "Magidesk CRM", "Align": "Center", "Style": "Bold_DoubleHeight" },
    { "Type": "LineBreak" },
    { "Type": "KeyVal", "Key": "Date:", "Val": "01/01/2026", "Align": "Spread" },
    { "Type": "Separator", "Char": "-" },
    ...
  ]
}
```
*Note: The Template Engine renders this JSON/Object structure.*

## 3. Rendering Pipeline

1.  **Context Preparation**:
    *   `TicketService` loads Entity.
    *   `PrintContextBuilder` converts Entity -> Safe Dictionary/DTO (localizes dates, formats currency).
2.  **Template Resolution**:
    *   `PrinterService` looks up assigned `TemplateId` for the target `PrinterGroup`.
    *   If null, use `DefaultSystemTemplate`.
3.  **Engine Rendering**:
    *   Input: `TemplateString` + `PrintContext`.
    *   Output: `RenderedADM` (Abstract Document Model).
4.  **Driver Translation**:
    *   Input: `RenderedADM` + `PrinterMapping` (Width, Format).
    *   **EscPosDriver**: Converts ADM -> Byte Arrays (ESC/POS).
    *   **TextDriver**: Converts ADM -> ASCII String (Standard Page).
    *   **HtmlDriver**: Converts ADM -> HTML (Verification Preview).

## 4. Data Safety
*   **Read-Only Context**: The context passed to Liquid is a specialized DTO (e.g., `TicketPrintModel`).
*   **No Math**: The Template Engine is discouraged from doing complex math. Totals are pre-calculated in the Context.

## 5. Storage Schema
**Table: `PrintTemplates`**
*   `Id` (Guid)
*   `Name` (Varchar): "Standard Receipt"
*   `Type` (Enum): Receipt, Kitchen, Report
*   `Content` (Text): The Liquid source code.
*   `IsSystem` (Bool): If true, cannot be deleted/modified (User must Clone).

**Table: `PrinterGroups` (Extension)**
*   `ReceiptTemplateId` (FK) - Nullable.
*   `KitchenTemplateId` (FK) - Nullable.

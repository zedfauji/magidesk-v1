# Template System Implementation Tickets

## Group 1: Foundation (Backend)

### TKT-P001: Template Schema & Repository [COMPLETED]
*   **Scope**: Create `PrintTemplates` table and Entity. Add `TemplateId` to `PrinterGroups`. Create `IPrintTemplateRepository`.
*   **Target**: `Magidesk.Domain`, `Magidesk.Infrastructure`.
*   **Risk**: Low.

### TKT-P002: Liquid Engine Integration [COMPLETED]
*   **Scope**: Import `Fluid` (or similar). Create `ITemplateEngine` service that takes string + model and returns string/JSON.
*   **Verification**: Unit tests with sample Liquid strings.
*   **Target**: `Magidesk.Application`.
*   **Risk**: Medium (Dependency management).

### TKT-P003: Abstract Document Model (ADM) & Drivers [COMPLETED]
*   **Scope**: Define the ADM C# classes. Implement `EscPosDriver` (converts ADM to Bytes).
*   **Verification**: Ensure ADM output matches current hardcoded byte output.
*   **Target**: `Magidesk.Infrastructure`.
*   **Risk**: High (Core printing correctness).

## Group 2: Context Building

### TKT-P004: Ticket Print Model Builder [COMPLETED]
*   **Scope**: Create `PrintContextBuilder`. Map `Ticket` -> `TicketPrintModel` (Safe DTO). Ensure all fields (Tax, Subtotal, Mods) are present and formatted.
*   **Target**: `Magidesk.Application`.

## Group 3: Frontend (Management)

### TKT-P005: Template Editor UI [COMPLETED]
*   **Scope**: Create `PrintTemplatesPage`. List View + Edit View.
*   **Features**: Syntax highlighting (if possible, or just text area), Save/Cancel.
*   **Target**: `Magidesk.Presentation`.

### TKT-P006: Template Previewer [COMPLETED]
*   **Scope**: Implement "Dry Run" logic in Editor. Render ADM to HTML for visual preview.
*   **Target**: `Magidesk.Presentation`.

## Group 4: Integration & Migration

### TKT-P007: Service Integration (Hybrid Mode) [COMPLETED]
*   **Scope**: Update `ReceiptPrintService` to attempt Template Render. Fallback to Legacy.
*   **Target**: `Magidesk.Infrastructure`.
*   **Risk**: High.

### TKT-P008: Seeding & Cleanup [COMPLETED]
*   **Scope**: Create Default Templates. Remove Hardcoded Logic (Phase 2, after stability confirmed).

# Template Migration & Safety Plan

## 1. Phase 1: Hybrid Mode (Safety Net)
*   **Strategy**: Keep the existing `GenerateReceiptBytes` (Hardcoded C#) as a fallback mechanism.
*   **Implementation**:
    *   Try to load `PrintTemplate` from DB.
    *   Try to Render.
    *   **Catch Exception**: If rendering fails, log error and execute `GenerateReceiptBytes` (Legacy).

## 2. Seeding Defaults
*   **Migration Step**:
    *   Create `PrintTemplates` table.
    *   Insert 2 System Templates: `Standard_Receipt_v1` and `Standard_Kitchen_v1`.
    *   Update all `PrinterGroups` to verify they link to these defaults.

## 3. Validation Gates
*   **Edit Time**: When saving a template in the Editor, run a "Dry Run" render with Mock Data. If it throws (Syntax Error), **block the save**.
*   **Runtime**: Wrap rendering in `try/catch`.

## 4. Rollback Mechanism
*   **UI Feature**: "Reset to Factory Default".
*   **Action**: Resets the `PrinterGroup.TemplateId` to the immutable `System_...` template ID.

## 5. Prevents "Broken Ship"
*   By mandating the "Dry Run" on save, users cannot save a template that has invalid Liquid syntax.
*   By keeping the Legacy Code as a hard-fallback for Version 1.0, business continuity is guaranteed even if the Template Engine crashes.

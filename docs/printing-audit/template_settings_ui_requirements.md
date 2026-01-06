# Template Settings UI Requirements

## 1. Template Management Screen
*   **Location**: Admin > System Configuration > Print Templates.
*   **List View**:
    *   Show all templates.
    *   Columns: Name, Type (Receipt/Kitchen/Report), Status (System/Custom).
    *   Actions: Create New, Edit, Clone, Delete (Custom only).

## 2. Template Editor
*   **Layout**: Split Screen.
    *   **Left**: Code Editor (Syntax Highlighting for syntax like Liquid/Handlebars).
    *   **Right**: Live Preview.
*   **Preview Controls**:
    *   **Mock Data Selector**: "Small Ticket", "Big Ticket", "Refund", "Void".
    *   **Format Selector**: "80mm Receipt", "58mm Receipt", "A4 Text".
    *   *The preview renders the HTML-equivalent of the ADM.*

## 3. Assignment UI
*   **Location**: System Configuration > Printer Groups (Existing).
*   **Interaction**:
    *   Add `ComboBox` for "Primary Template".
    *   (Future) "Secondary Template" (e.g., Customer Copy vs Merchant Copy).

## 4. Import / Export
*   **Feature**: Allow users to share templates.
*   **Action**: "Export to JSON" and "Import JSON".
*   **Validation**: On import, validate syntax integrity.

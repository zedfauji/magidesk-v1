# Template Capability Requirements

To address the hardcoupling and enable user customization, the new Printing System must support the following capabilities:

## 1. Data Binding & Variables
*   **Must Support**:
    *   Entity Bindings: `{{Ticket.Number}}`, `{{Table.Name}}`, `{{User.FirstName}}`.
    *   Business Info: `{{Restaurant.Name}}`, `{{Restaurant.Address}}`, `{{Restaurant.Phone}}`.
    *   Formatting: `{{Price | "C2"}}`, `{{Date | "MM/dd/yy"}}`.
*   **Restriction**: Access should be limited to a safe "ViewModel" or "Context" object, not raw Entities.

## 2. Structural Logic
*   **Must Support**:
    *   **Iteration**: `{{#each OrderLines}} ... {{/each}}` for printing item lists.
    *   **Conditional Visibility**: `{{#if IsRefund}} **REFUND** {{/if}}` or `{{#if ShowPrices}} ... {{/if}}`.
    *   **Section Ordering**: Ability to define the order of Header, Body, Footer blocks.

## 3. Layout Awareness
*   **Must Support**:
    *   **Alignment**: Left, Center, Right (e.g., `<center>Text</center>` or similar abstract commands).
    *   **Emphasis**: Bold, DoubleHeight, DoubleWidth.
    *   **Dynamic Separators**: `<hr/>` or `{{FillLine char="-"}}` that respects the target printer width (32/42/48 chars).
    *   **Images**: `<image id="logo" />` support.

## 4. Format Independence
*   **Must Support**:
    *   rendering the *same* template driver to different outputs:
        *   **Thermal (ESC/POS)**: Mapped to bytes.
        *   **Text (Standard)**: Mapped to ASCII.
        *   **HTML/Preview**: Mapped to visual preview in UI.

## 5. Safety & Integrity
*   **Must Enforce**:
    *   **Sandboxing**: Templates cannot execute arbitrary C# code.
    *   **Totals Integrity**: Users cannot define custom math (e.g., `{{Total * 0.5}}`). Totals must be computed by the Backend and passed as read-only values.
    *   **Fallback**: If a template fails to render, a hardcoded "Safe Mode" receipt must print.

## 6. Management
*   **Must Support**:
    *   **Per-Printer-Group Assignment**: "Kitchen" uses Template A, "Bar" uses Template B.
    *   **Default Templates**: System ships with "Standard Receipt" and "Standard Kitchen" that cannot be deleted, only cloned.

# Data Model vs Presentation Map

## 1. Kitchen Ticket

### Data Fields (Pure)
*   `Ticket.TicketNumber`
*   `Ticket.TableNumbers` (List<string>)
*   `User.FirstName` / `LastName` (Server Name)
*   `OrderLine.Quantity`
*   `OrderLine.MenuItemName`
*   `OrderLine.Instructions` (String)
*   `Modifier.Name`

### Derived Fields
*   `DateTime.Now` (Date printed, not necessarily Order Date)
*   `GroupBy(PrinterGroupId)` (Logic derivation)

### Presentation Concerns (Hardcoded)
*   **Formatting**: "ORDER #{0}" (Ticket Prefix)
*   **Formatting**: "Table: {0}" (Label)
*   **Formatting**: "Server: {0}" (Label)
*   **Layout**: Separator lines (`-` x Width)
*   **Layout**: Indentation for Modifiers ("   + ")
*   **Layout**: Emphasis (`**`) for Instructions.
*   **Styling**: Bold/Doublesize commands (`EscPosHelper`) attached to specific lines.

## 2. Customer Receipt

### Data Fields (Pure)
*   `Ticket.TicketNumber`
*   `Ticket.TotalAmount`
*   `Ticket.TaxAmount`
*   `Ticket.SubtotalAmount`
*   `Ticket.Payments` (Type, Amount)
*   `OrderLine.Quantity`
*   `OrderLine.MenuItemName`
*   `OrderLine.TotalAmount` (Price)

### Derived Fields
*   `DateTime.Now`
*   `Balance Due` (Calculated for display, though present in entity)
*   `ShowPrices` (Boolean flag from PrinterGroup config)

### Presentation Concerns (Hardcoded)
*   **Static Text**: "MAGIDESK POS" (Header), Address Lines
*   **Static Text**: "Thank You!" (Footer)
*   **Labels**: "Chk:", "Date:", "Svr:", "Subtotal:", "Tax:", "TOTAL:"
*   **Layout**: Right-alignment logic (manual spacing or ESC commands)
*   **Layout**: Separator lines
*   **Styling**: Double Height for Header and Total.

## Classification Summary

| Field Type | Status | Issue |
| :--- | :--- | :--- |
| **Header Identity** | **HARDCODED** | Cannot change Restaurant Name/Address without code edit. |
| **Labels** | **HARDCODED** | Cannot localize or customize labels ("Chk" vs "Order"). |
| **Date Format** | **HARDCODED** | Fixed to `MM/dd/yyyy HH:mm`. |
| **Item Layout** | **MIXED** | Logic (`"{Qty} {Name}"`) is embedded in C# loops. |
| **Prices** | **CONFIGURABLE** | Can be toggled via `ShowPrices`. |
| **Footer** | **HARDCODED** | Fixed "Thank You!" message. |

# Required Printing Flows

## 1. Kitchen Order Printing
*   **Trigger**: User clicks "Send Order" or completes Payment.
*   **Condition**: Ticket has unprinted items (`ShouldPrintToKitchen == true` AND `PrintedToKitchen == false`).
*   **Routing**:
    *   Iterate OrderLines.
    *   Resolve `Product.PrinterGroup` (e.g., "Kitchen", "Bar").
    *   Resolve `PrinterMapping` (Terminal -> Group -> Physical Printer).
    *   Group items by Physical Printer.
*   **Output**: Print "Kitchen Ticket" to each target printer.
*   **Post-Action**: Mark items as `PrintedToKitchen = true`.
*   **Failure**: Show Dialog "Failed to print to Kitchen/Bar". **DO NOT** mark as printed.

## 2. Customer Receipt
*   **Trigger**: Payment Successful OR User clicks "Print Receipt".
*   **Routing**: Default "Receipt" printer for the Terminal.
*   **Output**: Standard Receipt format (Header, Items, Tax, Total, Payments, Footer).
*   **Failure**: Show Dialog "Failed to print receipt".

## 3. Cash Drawer Kick
*   **Trigger**: 
    1.  Cash Payment Successful.
    2.  User clicks "No Sale" (Open Drawer) - requires Manager permissions.
*   **Routing**: "Receipt" printer (usually connected to drawer via RJ11).
*   **Output**: ESC/POS Pulse command.
*   **Failure**: Log warning (user might not notice).

## 4. Shift Reports
*   **Trigger**: Shift Close / End of Day.
*   **Routing**: "Receipt" printer.
*   **Output**: Shift Summary (Sales, Payments, Tips, Cash Expected).
*   **Failure**: Show Dialog. Allow retry.

## 5. Refunds / Voids
*   **Trigger**: Ticket Voided or Refund Processed.
*   **Output**: "Credit Slip" or "Void Slip" for signature.
*   **Routing**: Receipt Printer.

# Parity Analysis Log (Part 1: F-0001 to F-0050)

This log details the analysis for each feature to determine its parity status.

---

### F-0001: Application Bootstrap & System Initialization

*   **Forensic Requirement**: Ensure system is coherent (DB connection, terminal ID, config) *before* showing the main UI. Must show a blocking "initializing" overlay and provide a remediation path (e.g., DB config dialog) on failure.
*   **MagiDesk Implementation**: `App.xaml.cs` initializes DI and immediately creates `MainWindow`, which navigates directly to `SwitchboardPage`.
*   **Parity Gap**: No pre-startup health check or initialization gate exists. The app proceeds to the main UI regardless of DB or configuration state. No blocking "initializing" UI is present. No remediation path for DB connection failure is implemented.
*   **Assessed Status**: **PARTIAL**. The app bootstraps, but misses the critical safety checks and initialization workflow.

---

### F-0002: POS Main Window Shell

*   **Forensic Requirement**: A main window shell that hosts all views and displays a status bar with critical information (terminal ID, user, DB status, clock). Must also handle shutdown logic differently based on login state.
*   **MagiDesk Implementation**: `MainWindow.xaml` provides a `NavigationView` as the shell, which is functional for navigation. However, it completely lacks the status bar, the live clock timer, and any custom shutdown logic.
*   **Parity Gap**: The entire status bar feature is missing. The controlled shutdown workflow is not implemented.
*   **Assessed Status**: **PARTIAL**. A window exists, but it's missing key informational and behavioral components.

---

### F-0003: Login Screen

*   **Forensic Requirement**: A login screen to gate access to all operational views, featuring order-type buttons and primary navigation that triggers a password prompt.
*   **MagiDesk Implementation**: The application navigates directly to `SwitchboardPage`, completely bypassing any authentication step.
*   **Parity Gap**: The entire login screen, user authentication flow, and role-based view gating are non-existent.
*   **Assessed Status**: **NOT IMPLEMENTED**.

---

### F-0004: Switchboard View

*   **Forensic Requirement**: A central navigation hub that also displays dynamic operational data, specifically a list of the current user's open tickets and quick sales statistics.
*   **MagiDesk Implementation**: `SwitchboardPage.xaml` is a static grid of navigation buttons. It does not fetch or display any dynamic data.
*   **Parity Gap**: The open tickets list and the quick stats display are both missing. The view is purely for navigation, not for operational awareness.
*   **Assessed Status**: **PARTIAL**. The navigation hub aspect exists, but the crucial data display components are missing.

---

### F-0005: Order Entry View Container

*   **Forensic Requirement**: A main order entry screen combining a ticket view, menu selection, and action buttons.
*   **MagiDesk Implementation**: `TicketPage.xaml` is a developer-oriented test page with manual GUID inputs for creating and loading tickets. It is not a functional, user-facing order entry screen.
*   **Parity Gap**: The page completely lacks the integrated menu selection view, which is a core component of the order entry container. The workflow is not user-driven.
*   **Assessed Status**: **NOT IMPLEMENTED**.

---

### F-0006: Ticket Panel (Search, Qty, Pay Now)

*   **Forensic Requirement**: A panel within the order view that displays ticket items and totals, and includes integrated actions like Search, Quantity, and Pay Now.
*   **MagiDesk Implementation**: The `ListView` and totals `TextBlock` within the developer-focused `TicketPage.xaml` are a rudimentary representation of the ticket item display.
*   **Parity Gap**: It is missing the integrated action buttons specified (Search, Qty, Pay Now) and is not part of a proper order entry view.
*   **Assessed Status**: **PARTIAL**. The most basic elements exist but are not in a functional or user-facing context.

---

### F-0007: Password Entry Dialog & Payment Keypad

*   **Forensic Requirement**: A reusable password/PIN entry dialog for login and overrides. A numeric keypad for tender amount entry in the payment view.
*   **MagiDesk Implementation**:
    *   **Password Dialog**: No such dialog exists. Authentication is not implemented.
    *   **Payment Keypad**: `SettlePage.xaml` includes a numeric keypad for amount entry.
*   **Parity Gap**:
    *   The entire password entry and manager override UI is missing.
    *   A payment keypad exists but is part of a larger, specific `SettlePage` rather than a general payment view.
*   **Assessed Status**: **PARTIAL**. The keypad component exists, but the critical password dialog is **NOT IMPLEMENTED**.

---

### F-0008: Logout Action & Settle Ticket Dialog

*   **Forensic Requirement**: A `LogoutAction` to end the user session. A `SettleTicketDialog` for handling payments.
*   **MagiDesk Implementation**:
    *   **Logout Action**: No logout action or command is visible in `SwitchboardViewModel` or `MainWindow`.
    *   **Settle Ticket Dialog**: The `SettlePage.xaml` and its `SettleViewModel` provide a comprehensive UI for settling a ticket, including payment type selection and a numpad. It is a full page, not a dialog, but fulfills the functional requirement.
*   **Parity Gap**: The logout functionality is missing. The settlement UI is a page, not a dialog, which is a minor architectural deviation but functionally similar.
*   **Assessed Status**: **PARTIAL**. Settle screen exists, but Logout action is **NOT IMPLEMENTED**.

---

### F-0009: Clock In/Out Action & Manager Functions Dialog

*   **Forensic Requirement**: A `ClockInOutAction` for time tracking. A `ManagerDialog` to access administrative functions.
*   **MagiDesk Implementation**:
    *   **Clock In/Out**: No clock-in or clock-out functionality was found in any ViewModel.
    *   **Manager Functions**: The `SwitchboardViewModel` has a `ManagerFunctionsCommand` which navigates to `UserManagementPage`, a placeholder page with no actual functions.
*   **Parity Gap**: Both the clock-in/out action and the manager functions dialog are completely missing.
*   **Assessed Status**: **NOT IMPLEMENTED**.

---

### F-0010: Cash Drops/Drawer Bleed & Switchboard Panel

*   **Forensic Requirement**: A UI for managers to record cash drops/drawer bleeds. A central navigation panel.
*   **MagiDesk Implementation**:
    *   **Cash Drops/Bleeds**: No UI exists to initiate a cash drop. The `DrawerPullReportViewModel` can display these transactions, but there is no way to create them from the UI.
    *   **Switchboard Panel**: This is a duplicate analysis of `F-0004`. The `SwitchboardPage` exists as a static navigation grid.
*   **Parity Gap**: The cash drop/drawer bleed feature is entirely missing from the UI. The switchboard panel lacks dynamic data.
*   **Assessed Status**: **PARTIAL**. The panel exists, but the cash management action is **NOT IMPLEMENTED**.

---

### F-0011: Open Tickets List Dialog

*   **Forensic Requirement**: A dialog to list open tickets with actions like Void and Transfer Server. Should have a special "cashier mode" where selecting a ticket immediately resumes it.
*   **MagiDesk Implementation**: `TicketManagementPage.xaml` provides a list of open tickets and some actions (Void, Refund, basic Split). It is a full page, not a dialog.
*   **Parity Gap**: The dedicated "Transfer Server" action is missing. The "cashier mode" behavior (select to resume) is not implemented. The UI is a page, not a modal dialog as specified.
*   **Assessed Status**: **PARTIAL**. A list of open tickets with some actions exists, but key workflows and the dialog-based presentation are missing.

---

### F-0012: Drawer Pull Report Dialog

*   **Forensic Requirement**: A modal dialog that shows a detailed drawer pull report in an HTML preview, with Print and Finish actions.
*   **MagiDesk Implementation**: `DrawerPullReportPage.xaml` is a full page that displays the drawer pull data. It loads the report for the current user's active cash session.
*   **Parity Gap**: The implementation is a page, not a dialog. It lacks the HTML preview and the "Finish" action. The print action is present but not fully implemented.
*   **Assessed Status**: **PARTIAL**. The report data is displayed, but the specific dialog-based workflow and HTML preview are missing.

---

### F-0013: Void Ticket Dialog

*   **Forensic Requirement**: A modal dialog that prompts for a void reason, allows flagging items as wasted, and handles the refund workflow for paid tickets.
*   **MagiDesk Implementation**: The `TicketManagementPage` has a 'Void Selected' button that directly calls a `VoidTicketCommand`. There is no dialog, no reason selection, no 'wasted' flag, and no logic to handle refunds for paid tickets.
*   **Parity Gap**: The entire user-facing dialog, data capture (reason, wasted), and conditional logic (paid vs. unpaid) are missing. The current implementation is a direct, unprotected command.
*   **Assessed Status**: **PARTIAL**. The backend command exists, but the UI workflow is **NOT IMPLEMENTED**.

---

### F-0014: Split Ticket Dialog

*   **Forensic Requirement**: A dialog showing two ticket views side-by-side, allowing users to move items between them. Must also support splitting by seat or splitting evenly.
*   **MagiDesk Implementation**: The `TicketManagementPage` has a 'Split Selected' button with a hardcoded test implementation that moves only the first item of a ticket to a new one. A `SplitTicketDialog.xaml` exists but is not wired into a user-facing workflow.
*   **Parity Gap**: The entire interactive split dialog is missing from the user flow. There is no UI for item selection, splitting by seat, or splitting evenly.
*   **Assessed Status**: **NOT IMPLEMENTED**. The existing functionality is a developer test stub, not a feature.

---

### F-0015: Payment Process Wait Dialog

*   **Forensic Requirement**: A blocking modal dialog with a progress indicator that is shown during long operations like processing a payment.
*   **MagiDesk Implementation**: The `SettlePage.xaml` and other views use a `ProgressRing` overlay controlled by an `IsBusy` property on the ViewModel. This provides visual feedback but does not use a separate, dedicated `WaitDialog`.
*   **Parity Gap**: The pattern is implemented as a non-blocking overlay within a page, not as a separate, blocking modal `ContentDialog` as specified.
*   **Assessed Status**: **PARTIAL**. A progress indicator exists, but it does not match the specific modal dialog pattern.

---

### F-0016: Swipe Card Dialog

*   **Forensic Requirement**: A dialog to prompt for a card swipe and handle the card data capture.
*   **MagiDesk Implementation**: No UI for swiping cards, manual entry, or handling external terminals was found.
*   **Parity Gap**: The entire feature is missing.
*   **Assessed Status**: **NOT IMPLEMENTED**.

---

### F-0017: Authorization Code Dialog

*   **Forensic Requirement**: A dialog for manually entering a credit card authorization code.
*   **MagiDesk Implementation**: No such dialog or input mechanism was found.
*   **Parity Gap**: The entire feature is missing.
*   **Assessed Status**: **NOT IMPLEMENTED**.

---

### F-0018: Authorization Capture Batch Dialog

*   **Forensic Requirement**: A dialog to process a batch of previously authorized credit card transactions.
*   **MagiDesk Implementation**: No UI for batch capturing authorizations was found.
*   **Parity Gap**: The entire feature is missing.
*   **Assessed Status**: **NOT IMPLEMENTED**.

---

### F-0019: New Ticket Action

*   **Forensic Requirement**: An action that initiates the new ticket workflow, including order type selection.
*   **MagiDesk Implementation**: The `SwitchboardViewModel` contains a `NewTicketCommand` that correctly launches the `OrderTypeSelectionDialog`.
*   **Parity Gap**: The action does not lead to a functional order entry view; it navigates to the developer-focused `TicketPage`.
*   **Assessed Status**: **PARTIAL**. The initial action and dialog are implemented, but the subsequent workflow is incomplete.

---

### F-0020: Order Type Selection Dialog

*   **Forensic Requirement**: A dialog to select an order type, which then drives different workflows (e.g., table selection for Dine-In).
*   **MagiDesk Implementation**: `OrderTypeSelectionDialog.xaml` exists and allows for selection. It returns the selected type.
*   **Parity Gap**: The dialog itself does not enforce or trigger the different workflows required by each order type (e.g., prompting for a table).
*   **Assessed Status**: **PARTIAL**. The dialog UI exists, but the critical workflow logic is missing.

---

### F-0021: Ticket View Panel

*   **Forensic Requirement**: A panel to display current ticket items with quantities, modifiers, and prices, and allow for item selection.
*   **MagiDesk Implementation**: `TicketPage.xaml` contains a `ListView` that displays some order line details.
*   **Parity Gap**: The view is part of a developer test page, not a real order entry screen. It lacks proper formatting, modifier display, and robust item selection for actions.
*   **Assessed Status**: **PARTIAL**. A basic list exists, but it's not a functional UI component.

---

### F-0022: Order View Container

*   **Forensic Requirement**: A main order entry screen combining a ticket view, menu selection, and action buttons.
*   **MagiDesk Implementation**: `TicketPage.xaml` is a developer-oriented test page with manual GUID inputs for creating and loading tickets. It is not a functional, user-facing order entry screen.
*   **Parity Gap**: The page completely lacks the integrated menu selection view, which is a core component of the order entry container. The workflow is not user-driven.
*   **Assessed Status**: **NOT IMPLEMENTED**.

---

### F-0023: Guest Count Entry Dialog

*   **Forensic Requirement**: A dialog or input to set the number of guests for a ticket.
*   **MagiDesk Implementation**: No UI for entering guest count was found.
*   **Parity Gap**: The feature is entirely missing.
*   **Assessed Status**: **NOT IMPLEMENTED**.

---

### F-0024: Quantity Entry Dialog

*   **Forensic Requirement**: A dialog to pre-enter a quantity before selecting a menu item.
*   **MagiDesk Implementation**: The developer-focused `TicketPage` has a `TextBox` for quantity, but it's for manual testing and not part of a user-facing pre-entry workflow.
*   **Parity Gap**: The pre-entry quantity dialog workflow is missing.
*   **Assessed Status**: **PARTIAL**. A quantity field exists for testing, but the feature is **NOT IMPLEMENTED**.

---

### F-0025: Print Ticket Action

*   **Forensic Requirement**: An action to print a ticket for a customer (guest check/receipt).
*   **MagiDesk Implementation**: A `SendToKitchenCommand` exists, but no specific action for printing a customer-facing receipt was found.
*   **Parity Gap**: The customer receipt printing action is missing.
*   **Assessed Status**: **PARTIAL**. Kitchen printing is partially implemented, but customer receipt printing is **NOT IMPLEMENTED**.

---

### F-0026: Increase/Decrease Quantity Action

*   **Forensic Requirement**: UI controls (like +/- buttons) to incrementally change the quantity of a selected order line.
*   **MagiDesk Implementation**: No such controls were found. Quantity can only be changed via a direct `TextBox` input on the test page.
*   **Parity Gap**: The interactive +/- quantity controls are missing.
*   **Assessed Status**: **NOT IMPLEMENTED**.

---

### F-0027: Send to Kitchen Action

*   **Forensic Requirement**: An action to send new items on a ticket to the kitchen printers or KDS.
*   **MagiDesk Implementation**: `TicketPage.xaml` has a "Send to Kitchen" button bound to a `SendToKitchenCommand` in `TicketViewModel`.
*   **Parity Gap**: The visual feedback for sent items (e.g., graying out) is present but tied to the developer test page. The full workflow needs to be integrated into a proper order entry screen.
*   **Assessed Status**: **PARTIAL**. The core command and button exist but are not part of a functional user workflow.

---

### F-0028: Delete Ticket Item Action

*   **Forensic Requirement**: An action to remove an item from a ticket, with different rules for sent vs. unsent items.
*   **MagiDesk Implementation**: No UI for deleting a specific item from the ticket view was found. `TicketManagementPage` has a `VoidTicketCommand`, which acts on the entire ticket.
*   **Parity Gap**: The ability to delete a single item is missing.
*   **Assessed Status**: **NOT IMPLEMENTED**.

---

### F-0029: Misc Ticket Item Dialog

*   **Forensic Requirement**: A dialog to add an item with a custom name and price to a ticket.
*   **MagiDesk Implementation**: No UI for adding miscellaneous items was found.
*   **Parity Gap**: The entire feature is missing.
*   **Assessed Status**: **NOT IMPLEMENTED**.

---

### F-0030: Ticket Fee Dialog

*   **Forensic Requirement**: A dialog to add a fee (e.g., service charge, delivery fee) to a ticket.
*   **MagiDesk Implementation**: No UI for adding fees was found.
*   **Parity Gap**: The entire feature is missing.
*   **Assessed Status**: **NOT IMPLEMENTED**.

---

### F-0031: Menu Item Button View

*   **Forensic Requirement**: A touch-friendly grid of buttons to select menu items, organized by categories and groups.
*   **MagiDesk Implementation**: No such view exists. The `TicketPage` provides only manual text entry for a `MenuItemId`.
*   **Parity Gap**: The entire visual menu navigation and selection feature is missing.
*   **Assessed Status**: **NOT IMPLEMENTED**.

---

### F-0032: Size Selection Dialog

*   **Forensic Requirement**: A dialog to select an item's size (e.g., Small, Medium, Large), which affects its price.
*   **MagiDesk Implementation**: No dedicated size selection dialog was found. This functionality is expected to be handled by the modifier system, which is itself only partially implemented.
*   **Parity Gap**: The specific, streamlined UI for size selection is missing.
*   **Assessed Status**: **NOT IMPLEMENTED**.

---

### F-0033: Beverage Quick Add

*   **Forensic Requirement**: A quick-add panel for common beverages to speed up bar service.
*   **MagiDesk Implementation**: No such quick-add panel or specialized view was found.
*   **Parity Gap**: The entire feature is missing.
*   **Assessed Status**: **NOT IMPLEMENTED**.

---

### F-0034: Item Search Dialog

*   **Forensic Requirement**: A dialog to search for menu items by name or barcode.
*   **MagiDesk Implementation**: No item search dialog was found.
*   **Parity Gap**: The entire feature is missing.
*   **Assessed Status**: **NOT IMPLEMENTED**.

---

### F-0035: Price Entry Dialog

*   **Forensic Requirement**: A dialog for manually entering the price of an open-price item.
*   **MagiDesk Implementation**: The `TicketPage` has a `UnitPriceText` `TextBox` for manual price entry during testing, but no dedicated dialog exists for a user-facing workflow.
*   **Parity Gap**: The dedicated dialog for open-price items is missing.
*   **Assessed Status**: **NOT IMPLEMENTED**.

---

### F-0036: Cooking Instruction Dialog

*   **Forensic Requirement**: A dialog for adding free-text or pre-defined cooking instructions to an item.
*   **MagiDesk Implementation**: The `ModifierSelectionDialog` has a `TextBlock` for displaying group descriptions/instructions, but no interactive UI for adding them.
*   **Parity Gap**: The feature for adding custom or pre-defined instructions is missing.
*   **Assessed Status**: **NOT IMPLEMENTED**.

---

### F-0037: Pizza Modifiers View

*   **Forensic Requirement**: A specialized UI for configuring pizza toppings, including placement on halves or the whole pizza.
*   **MagiDesk Implementation**: No pizza-specific modifier UI was found.
*   **Parity Gap**: The entire feature is missing.
*   **Assessed Status**: **NOT IMPLEMENTED**.

---

### F-0038: Modifier Selection Dialog

*   **Forensic Requirement**: A dialog to select modifiers for a menu item, organized by groups, with validation for required selections.
*   **MagiDesk Implementation**: `ModifierSelectionDialog.xaml` exists and provides a UI for selecting modifiers from groups. It includes logic for validating selections.
*   **Parity Gap**: The dialog is not integrated into a functional order entry workflow. It is called from the developer test page.
*   **Assessed Status**: **PARTIAL**. The dialog and its core logic are implemented, but it is not part of a user-facing feature.

---

### F-0039: Add-On Selection View

*   **Forensic Requirement**: A view or prompt to suggest and add upsell items (add-ons).
*   **MagiDesk Implementation**: No UI for suggesting or adding add-ons was found.
*   **Parity Gap**: The entire feature is missing.
*   **Assessed Status**: **NOT IMPLEMENTED**.

---

### F-0040: Combo Item Selection Dialog

*   **Forensic Requirement**: A dialog to guide the user through selecting the components of a combo meal.
*   **MagiDesk Implementation**: No UI for combo item selection was found.
*   **Parity Gap**: The entire feature is missing.
*   **Assessed Status**: **NOT IMPLEMENTED**.

---

### F-0041: Quick Pay Action

*   **Forensic Requirement**: A one-button action to settle a ticket directly, especially for exact cash, bypassing the main payment screen.
*   **MagiDesk Implementation**: No such direct action exists. The `SettleUiCommand` on the `TicketPage` navigates to the full `SettlePage`.
*   **Parity Gap**: The streamlined "quick pay" workflow is missing.
*   **Assessed Status**: **NOT IMPLEMENTED**.

---

### F-0042: Exact Due Button

*   **Forensic Requirement**: A button in the payment view to automatically tender the exact due amount.
*   **MagiDesk Implementation**: The `SettleViewModel` automatically populates the tender amount with the due amount. There is no explicit "Exact Due" button.
*   **Parity Gap**: The explicit button is missing, though the default behavior is similar.
*   **Assessed Status**: **PARTIAL**.

---

### F-0043: Quick Cash Buttons

*   **Forensic Requirement**: Buttons for common cash denominations ($5, $10, $20) to speed up cash payment.
*   **MagiDesk Implementation**: No such buttons exist on the `SettlePage` or `PaymentPage`.
*   **Parity Gap**: The entire feature is missing.
*   **Assessed Status**: **NOT IMPLEMENTED**.

---

### F-0044: Cash Payment Button

*   **Forensic Requirement**: A button to initiate a cash payment.
*   **MagiDesk Implementation**: The `SettlePage` has a "CASH" button that triggers the `ProcessPaymentCommand` with the 'Cash' parameter.
*   **Parity Gap**: None. The button and command exist.
*   **Assessed Status**: **IMPLEMENTED**.

---

### F-0045: Credit Card Payment Button

*   **Forensic Requirement**: A button to initiate a credit card payment.
*   **MagiDesk Implementation**: The `SettlePage` has a "CREDIT CARD" button that triggers the `ProcessPaymentCommand` with the 'CreditCard' parameter.
*   **Parity Gap**: The backend simulation is basic, but the UI component exists.
*   **Assessed Status**: **PARTIAL**. The button exists, but the full workflow (swipe, etc.) is missing.

---

### F-0046: Group Settle Ticket Dialog

*   **Forensic Requirement**: A dialog to pay for multiple tickets with a single transaction.
*   **MagiDesk Implementation**: No UI for selecting or settling multiple tickets at once was found.
*   **Parity Gap**: The entire feature is missing.
*   **Assessed Status**: **NOT IMPLEMENTED**.

---

### F-0047: Split by Seat Dialog

*   **Forensic Requirement**: A dialog to split a ticket based on items assigned to different seats.
*   **MagiDesk Implementation**: No UI for splitting by seat was found.
*   **Parity Gap**: The entire feature is missing.
*   **Assessed Status**: **NOT IMPLEMENTED**.

---

### F-0048: Split Even Dialog

*   **Forensic Requirement**: A dialog to split a ticket's total evenly among a specified number of guests.
*   **MagiDesk Implementation**: No UI for splitting a ticket evenly was found.
*   **Parity Gap**: The entire feature is missing.
*   **Assessed Status**: **NOT IMPLEMENTED**.

---

### F-0049: Split by Amount Dialog

*   **Forensic Requirement**: A dialog to split a specific dollar amount off a ticket.
*   **MagiDesk Implementation**: No UI for splitting by amount was found.
*   **Parity Gap**: The entire feature is missing.
*   **Assessed Status**: **NOT IMPLEMENTED**.

---

### F-0050: Swipe Card Dialog

*   **Forensic Requirement**: A dialog to prompt for a card swipe and handle card data capture.
*   **MagiDesk Implementation**: No UI for swiping cards, manual entry, or handling external terminals was found.
*   **Parity Gap**: The entire feature is missing.
*   **Assessed Status**: **NOT IMPLEMENTED**.
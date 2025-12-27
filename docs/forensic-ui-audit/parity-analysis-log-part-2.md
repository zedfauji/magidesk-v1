# Parity Analysis Log (Part 2: F-0051 to F-0100)

This log details the analysis for each feature to determine its parity status.

---

### F-0051: Refund Button

*   **Forensic Requirement**: An action to trigger the refund workflow for a paid ticket.
*   **MagiDesk Implementation**: `TicketManagementPage.xaml` has a 'Refund Selected' button and associated command.
*   **Parity Gap**: The UI is on a developer test page, not integrated into a user-facing workflow like a ticket explorer. The full refund dialog with amount validation is missing.
*   **Assessed Status**: **PARTIAL**. The basic command exists, but the user workflow is not implemented.

---

### F-0052: Check Payment Action

*   **Forensic Requirement**: A dialog to accept a paper check as payment, recording the check number.
*   **MagiDesk Implementation**: No UI for accepting check payments was found.
*   **Parity Gap**: The entire feature is missing.
*   **Assessed Status**: **NOT IMPLEMENTED**.

---

### F-0053: House Account Payment Action

*   **Forensic Requirement**: A dialog to charge a payment to a customer's house account.
*   **MagiDesk Implementation**: No UI for house account payments was found.
*   **Parity Gap**: The entire feature is missing.
*   **Assessed Status**: **NOT IMPLEMENTED**.

---

### F-0054: Gift Certificate Entry

*   **Forensic Requirement**: A dialog to enter a gift certificate number and value for payment.
*   **MagiDesk Implementation**: The `SettlePage` has a 'GIFT CARD' button, but it is not fully wired and no dedicated entry dialog exists.
*   **Parity Gap**: The gift certificate entry dialog and validation workflow are missing.
*   **Assessed Status**: **PARTIAL**. A button exists, but the feature is **NOT IMPLEMENTED**.

---

### F-0055: Gratuity Input

*   **Forensic Requirement**: A dialog to add tips/gratuity to a ticket.
*   **MagiDesk Implementation**: `SettleViewModel` and `PaymentViewModel` have properties for `TipsText`, but there is no dedicated dialog for gratuity entry as described.
*   **Parity Gap**: The specific `GratuityInputDialog` is missing.
*   **Assessed Status**: **PARTIAL**. The data fields exist, but the UI workflow is not implemented as specified.

---

### F-0056: Tip Adjustment Action

*   **Forensic Requirement**: An action to adjust tips on a previously closed (credit card) ticket.
*   **MagiDesk Implementation**: No UI for post-settlement tip adjustment was found.
*   **Parity Gap**: The entire feature is missing.
*   **Assessed Status**: **NOT IMPLEMENTED**.

---

### F-0057: Authorization Capture Batch Dialog

*   **Forensic Requirement**: A dialog to process a batch of previously authorized credit card transactions.
*   **MagiDesk Implementation**: No UI for batch capturing authorizations was found.
*   **Parity Gap**: The entire feature is missing.
*   **Assessed Status**: **NOT IMPLEMENTED**.

---

### F-0058: Multi-Currency Tender Dialog

*   **Forensic Requirement**: A dialog to accept payments in multiple currencies.
*   **MagiDesk Implementation**: No multi-currency support was found.
*   **Parity Gap**: The entire feature is missing.
*   **Assessed Status**: **NOT IMPLEMENTED**.

---

### F-0059: Card Signature Capture

*   **Forensic Requirement**: A dialog or panel to capture a customer's signature for a card transaction.
*   **MagiDesk Implementation**: No UI for signature capture was found.
*   **Parity Gap**: The entire feature is missing.
*   **Assessed Status**: **NOT IMPLEMENTED**.

---

### F-0060: Shift Start Dialog

*   **Forensic Requirement**: A dialog to start a new shift, including counting an opening cash balance.
*   **MagiDesk Implementation**: `CashSessionViewModel` has an `OpenCommand`, but it is part of a developer test page and lacks a dedicated shift start dialog.
*   **Parity Gap**: The user-facing dialog and workflow for starting a shift are missing.
*   **Assessed Status**: **PARTIAL**. The backend command exists, but the UI workflow is **NOT IMPLEMENTED**.

---

### F-0082: Table Map View

*   **Forensic Requirement**: A visual floor layout showing table status with color-coding. Clicking a table should start or resume an order.
*   **MagiDesk Implementation**: `TableMapPage.xaml` uses a `Canvas` to position tables and a converter to color-code them by status. The `TableMapViewModel` has logic to handle table selection for starting new orders or resuming existing ones.
*   **Parity Gap**: The navigation from the table map leads to the developer-focused `TicketPage`, not a functional user-facing order entry screen. The layout is a static canvas, not a configurable floor plan.
*   **Assessed Status**: **PARTIAL**. The view and its core logic exist, but the subsequent workflow is incomplete.

---

### F-0083: Home Delivery View

*   **Forensic Requirement**: A dedicated view for managing delivery orders.
*   **MagiDesk Implementation**: No UI for home delivery management was found.
*   **Parity Gap**: The entire feature is missing.
*   **Assessed Status**: **NOT IMPLEMENTED**.

---

### F-0084: Pickup Order View

*   **Forensic Requirement**: A dedicated view for managing pickup orders.
*   **MagiDesk Implementation**: No UI for pickup order management was found.
*   **Parity Gap**: The entire feature is missing.
*   **Assessed Status**: **NOT IMPLEMENTED**.

---

### F-0085: Bar Tab Selection View

*   **Forensic Requirement**: A view for creating and managing bar tabs.
*   **MagiDesk Implementation**: No UI for bar tab management was found.
*   **Parity Gap**: The entire feature is missing.
*   **Assessed Status**: **NOT IMPLEMENTED**.

---

### F-0086: Seat Selection Dialog

*   **Forensic Requirement**: A dialog to assign items to specific seats.
*   **MagiDesk Implementation**: No UI for seat selection was found.
*   **Parity Gap**: The entire feature is missing.
*   **Assessed Status**: **NOT IMPLEMENTED**.

---

### F-0087: Table Browser

*   **Forensic Requirement**: A back-office view for CRUD operations on tables.
*   **MagiDesk Implementation**: No dedicated table browser UI was found. `TableMapPage` displays tables but does not manage them.
*   **Parity Gap**: The entire table management feature is missing.
*   **Assessed Status**: **NOT IMPLEMENTED**.

---

### F-0088: Kitchen Display Window

*   **Forensic Requirement**: A standalone window for kitchen staff to view and manage orders.
*   **MagiDesk Implementation**: No Kitchen Display System (KDS) UI was found.
*   **Parity Gap**: The entire feature is missing.
*   **Assessed Status**: **NOT IMPLEMENTED**.

---

### F-0089: Kitchen Ticket View

*   **Forensic Requirement**: A component within the KDS to display a single ticket's details.
*   **MagiDesk Implementation**: No KDS UI was found.
*   **Parity Gap**: The entire feature is missing.
*   **Assessed Status**: **NOT IMPLEMENTED**.

---

### F-0090: Kitchen Status Selector

*   **Forensic Requirement**: A filter bar in the KDS to sort tickets by status.
*   **MagiDesk Implementation**: No KDS UI was found.
*   **Parity Gap**: The entire feature is missing.
*   **Assessed Status**: **NOT IMPLEMENTED**.

---

### F-0091: Kitchen Ticket List Panel

*   **Forensic Requirement**: A panel within the KDS to display a grid of all active kitchen tickets.
*   **MagiDesk Implementation**: No KDS UI was found.
*   **Parity Gap**: The entire feature is missing.
*   **Assessed Status**: **NOT IMPLEMENTED**.

---

### F-0092: Sales Summary Report

*   **Forensic Requirement**: A back-office report showing sales performance summaries.
*   **MagiDesk Implementation**: `SalesReportsPage.xaml` has a section for the Sales Summary report and the `SalesReportsViewModel` can fetch the data.
*   **Parity Gap**: The UI is a basic representation and may lack detailed grouping and export options.
*   **Assessed Status**: **PARTIAL**.

---

### F-0093: Sales Detail Report

*   **Forensic Requirement**: A report showing an itemized breakdown of all sales.
*   **MagiDesk Implementation**: The `SalesReportsPage` framework exists, but it does not currently render a sales detail report.
*   **Parity Gap**: The specific report view is missing.
*   **Assessed Status**: **NOT IMPLEMENTED**.

---

### F-0094: Sales Balance Report

*   **Forensic Requirement**: A report to reconcile sales, payments, and cash.
*   **MagiDesk Implementation**: No specific sales balance report UI was found.
*   **Parity Gap**: The entire feature is missing.
*   **Assessed Status**: **NOT IMPLEMENTED**.

---

### F-0095: Sales Exception Report

*   **Forensic Requirement**: A report to highlight high-risk transactions like voids and discounts.
*   **MagiDesk Implementation**: No exception report UI was found.
*   **Parity Gap**: The entire feature is missing.
*   **Assessed Status**: **NOT IMPLEMENTED**.

---

### F-0096: Credit Card Report

*   **Forensic Requirement**: A report summarizing credit card transactions.
*   **MagiDesk Implementation**: No credit card report UI was found.
*   **Parity Gap**: The entire feature is missing.
*   **Assessed Status**: **NOT IMPLEMENTED**.

---

### F-0097: Payment Report

*   **Forensic Requirement**: A report breaking down payments by tender type.
*   **MagiDesk Implementation**: No payment report UI was found.
*   **Parity Gap**: The entire feature is missing.
*   **Assessed Status**: **NOT IMPLEMENTED**.

---

### F-0098: Menu Usage Report

*   **Forensic Requirement**: A report analyzing menu item sales performance.
*   **MagiDesk Implementation**: No menu usage report UI was found.
*   **Parity Gap**: The entire feature is missing.
*   **Assessed Status**: **NOT IMPLEMENTED**.

---

### F-0099: Server Productivity Report

*   **Forensic Requirement**: A report analyzing server sales performance.
*   **MagiDesk Implementation**: No server productivity report UI was found.
*   **Parity Gap**: The entire feature is missing.
*   **Assessed Status**: **NOT IMPLEMENTED**.

---

### F-0100: Hourly Labor Report

*   **Forensic Requirement**: A report showing labor costs vs. sales by hour.
*   **MagiDesk Implementation**: The `SalesReportsPage` has a section for the Labor Report, and the `SalesReportsViewModel` can fetch the data.
*   **Parity Gap**: The UI is a basic representation and may lack detailed filtering or visualization.
*   **Assessed Status**: **PARTIAL**.

---

### F-0082: Table Map View

*   **Forensic Requirement**: A visual floor layout showing table status with color-coding. Clicking a table should start or resume an order.
*   **MagiDesk Implementation**: `TableMapPage.xaml` uses a `Canvas` to position tables and a converter to color-code them by status. The `TableMapViewModel` has logic to handle table selection for starting new orders or resuming existing ones.
*   **Parity Gap**: The navigation from the table map leads to the developer-focused `TicketPage`, not a functional user-facing order entry screen. The layout is a static canvas, not a configurable floor plan.
*   **Assessed Status**: **PARTIAL**. The view and its core logic exist, but the subsequent workflow is incomplete.

---

### F-0083: Home Delivery View

*   **Forensic Requirement**: A dedicated view for managing delivery orders.
*   **MagiDesk Implementation**: No UI for home delivery management was found.
*   **Parity Gap**: The entire feature is missing.
*   **Assessed Status**: **NOT IMPLEMENTED**.

---

### F-0084: Pickup Order View

*   **Forensic Requirement**: A dedicated view for managing pickup orders.
*   **MagiDesk Implementation**: No UI for pickup order management was found.
*   **Parity Gap**: The entire feature is missing.
*   **Assessed Status**: **NOT IMPLEMENTED**.

---

### F-0085: Bar Tab Selection View

*   **Forensic Requirement**: A view for creating and managing bar tabs.
*   **MagiDesk Implementation**: No UI for bar tab management was found.
*   **Parity Gap**: The entire feature is missing.
*   **Assessed Status**: **NOT IMPLEMENTED**.

---

### F-0086: Seat Selection Dialog

*   **Forensic Requirement**: A dialog to assign items to specific seats.
*   **MagiDesk Implementation**: No UI for seat selection was found.
*   **Parity Gap**: The entire feature is missing.
*   **Assessed Status**: **NOT IMPLEMENTED**.

---

### F-0087: Table Browser

*   **Forensic Requirement**: A back-office view for CRUD operations on tables.
*   **MagiDesk Implementation**: No dedicated table browser UI was found. `TableMapPage` displays tables but does not manage them.
*   **Parity Gap**: The entire table management feature is missing.
*   **Assessed Status**: **NOT IMPLEMENTED**.

---

### F-0088: Kitchen Display Window

*   **Forensic Requirement**: A standalone window for kitchen staff to view and manage orders.
*   **MagiDesk Implementation**: No Kitchen Display System (KDS) UI was found.
*   **Parity Gap**: The entire feature is missing.
*   **Assessed Status**: **NOT IMPLEMENTED**.

---

### F-0089: Kitchen Ticket View

*   **Forensic Requirement**: A component within the KDS to display a single ticket's details.
*   **MagiDesk Implementation**: No KDS UI was found.
*   **Parity Gap**: The entire feature is missing.
*   **Assessed Status**: **NOT IMPLEMENTED**.

---

### F-0090: Kitchen Status Selector

*   **Forensic Requirement**: A filter bar in the KDS to sort tickets by status.
*   **MagiDesk Implementation**: No KDS UI was found.
*   **Parity Gap**: The entire feature is missing.
*   **Assessed Status**: **NOT IMPLEMENTED**.

---

### F-0091: Kitchen Ticket List Panel

*   **Forensic Requirement**: A panel within the KDS to display a grid of all active kitchen tickets.
*   **MagiDesk Implementation**: No KDS UI was found.
*   **Parity Gap**: The entire feature is missing.
*   **Assessed Status**: **NOT IMPLEMENTED**.

---

### F-0092: Sales Summary Report

*   **Forensic Requirement**: A back-office report showing sales performance summaries.
*   **MagiDesk Implementation**: `SalesReportsPage.xaml` has a section for the Sales Summary report and the `SalesReportsViewModel` can fetch the data.
*   **Parity Gap**: The UI is a basic representation and may lack detailed grouping and export options.
*   **Assessed Status**: **PARTIAL**.

---

### F-0093: Sales Detail Report

*   **Forensic Requirement**: A report showing an itemized breakdown of all sales.
*   **MagiDesk Implementation**: The `SalesReportsPage` framework exists, but it does not currently render a sales detail report.
*   **Parity Gap**: The specific report view is missing.
*   **Assessed Status**: **NOT IMPLEMENTED**.

---

### F-0094: Sales Balance Report

*   **Forensic Requirement**: A report to reconcile sales, payments, and cash.
*   **MagiDesk Implementation**: No specific sales balance report UI was found.
*   **Parity Gap**: The entire feature is missing.
*   **Assessed Status**: **NOT IMPLEMENTED**.

---

### F-0095: Sales Exception Report

*   **Forensic Requirement**: A report to highlight high-risk transactions like voids and discounts.
*   **MagiDesk Implementation**: No exception report UI was found.
*   **Parity Gap**: The entire feature is missing.
*   **Assessed Status**: **NOT IMPLEMENTED**.

---

### F-0096: Credit Card Report

*   **Forensic Requirement**: A report summarizing credit card transactions.
*   **MagiDesk Implementation**: No credit card report UI was found.
*   **Parity Gap**: The entire feature is missing.
*   **Assessed Status**: **NOT IMPLEMENTED**.

---

### F-0097: Payment Report

*   **Forensic Requirement**: A report breaking down payments by tender type.
*   **MagiDesk Implementation**: No payment report UI was found.
*   **Parity Gap**: The entire feature is missing.
*   **Assessed Status**: **NOT IMPLEMENTED**.

---

### F-0098: Menu Usage Report

*   **Forensic Requirement**: A report analyzing menu item sales performance.
*   **MagiDesk Implementation**: No menu usage report UI was found.
*   **Parity Gap**: The entire feature is missing.
*   **Assessed Status**: **NOT IMPLEMENTED**.

---

### F-0099: Server Productivity Report

*   **Forensic Requirement**: A report analyzing server sales performance.
*   **MagiDesk Implementation**: No server productivity report UI was found.
*   **Parity Gap**: The entire feature is missing.
*   **Assessed Status**: **NOT IMPLEMENTED**.

---

### F-0100: Hourly Labor Report

*   **Forensic Requirement**: A report showing labor costs vs. sales by hour.
*   **MagiDesk Implementation**: The `SalesReportsPage` has a section for the Labor Report, and the `SalesReportsViewModel` can fetch the data.
*   **Parity Gap**: The UI is a basic representation and may lack detailed filtering or visualization.
*   **Assessed Status**: **PARTIAL**.

---

### F-0061: Shift End Dialog

*   **Forensic Requirement**: A dialog to end a shift, including counting the closing cash balance and showing variances.
*   **MagiDesk Implementation**: `CashSessionViewModel` has a `CloseCommand`, but it is part of a developer test page and lacks a dedicated shift end dialog with reconciliation details.
*   **Parity Gap**: The user-facing dialog and reconciliation workflow are missing.
*   **Assessed Status**: **PARTIAL**. The backend command exists, but the UI workflow is **NOT IMPLEMENTED**.

---

### F-0062: Payout Dialog

*   **Forensic Requirement**: A dialog for managers to record cash payouts from the drawer.
*   **MagiDesk Implementation**: No UI for creating payouts was found. The `DrawerPullReportPage` can only display them.
*   **Parity Gap**: The entire feature for creating payouts is missing.
*   **Assessed Status**: **NOT IMPLEMENTED**.

---

### F-0063: No Sale Action

*   **Forensic Requirement**: A permission-gated action to open the cash drawer without a sale.
*   **MagiDesk Implementation**: No 'No Sale' button or corresponding action was found.
*   **Parity Gap**: The entire feature is missing.
*   **Assessed Status**: **NOT IMPLEMENTED**.

---

### F-0064: Cash Drop Action

*   **Forensic Requirement**: A dialog to record the removal of excess cash from the drawer.
*   **MagiDesk Implementation**: No UI for initiating a cash drop was found.
*   **Parity Gap**: The entire feature is missing.
*   **Assessed Status**: **NOT IMPLEMENTED**.

---

### F-0065: Drawer Assignment Action

*   **Forensic Requirement**: A dialog to assign a cash drawer to a terminal/user with a starting balance.
*   **MagiDesk Implementation**: The `CashSessionViewModel` has an `OpenCommand` on a test page, but no user-facing drawer assignment dialog exists.
*   **Parity Gap**: The dedicated user workflow for drawer assignment is missing.
*   **Assessed Status**: **PARTIAL**. The backend capability exists, but the UI is **NOT IMPLEMENTED**.

---

### F-0066: Tip Declare Action

*   **Forensic Requirement**: A UI for servers to declare cash tips at the end of a shift.
*   **MagiDesk Implementation**: No UI for declaring tips was found.
*   **Parity Gap**: The entire feature is missing.
*   **Assessed Status**: **NOT IMPLEMENTED**.

---

### F-0067: Drawer Count Dialog

*   **Forensic Requirement**: A dialog with fields for each denomination to count the cash in a drawer.
*   **MagiDesk Implementation**: No such dialog was found. The `CashSessionViewModel` has a simple text box for the final cash amount.
*   **Parity Gap**: The denomination-based counting UI is missing.
*   **Assessed Status**: **NOT IMPLEMENTED**.

---

### F-0068: Ticket Explorer

*   **Forensic Requirement**: A back-office view to browse and filter all tickets (open, closed, voided).
*   **MagiDesk Implementation**: `TicketManagementPage` provides a list of *open* tickets only.
*   **Parity Gap**: There is no UI to explore all tickets or apply filters for status, date range, etc.
*   **Assessed Status**: **PARTIAL**. A basic open ticket list exists, but the full explorer is **NOT IMPLEMENTED**.

---

### F-0069: Edit Ticket Action

*   **Forensic Requirement**: An action to open an existing ticket in the main order entry view.
*   **MagiDesk Implementation**: The `TicketManagementPage` allows selecting an open ticket, but there is no action to open it in a functional order entry screen (it would navigate to the dev-only `TicketPage`).
*   **Parity Gap**: The action to resume and edit a ticket in a proper order view is missing.
*   **Assessed Status**: **PARTIAL**. The ticket can be selected, but the editing workflow is not implemented.

---

### F-0070: View Receipt Action

*   **Forensic Requirement**: An action to view or reprint a receipt for a closed ticket.
*   **MagiDesk Implementation**: No UI for viewing or reprinting receipts was found.
*   **Parity Gap**: The entire feature is missing.
*   **Assessed Status**: **NOT IMPLEMENTED**.

---

### F-0071: Hold Ticket Action

*   **Forensic Requirement**: An explicit action to save the current ticket and return to the main navigation view.
*   **MagiDesk Implementation**: No explicit 'Hold' button or command was found. Ticket saving is handled implicitly by other actions.
*   **Parity Gap**: The specific 'Hold' user action is missing.
*   **Assessed Status**: **NOT IMPLEMENTED**.

---

### F-0072: Reopen Ticket Action

*   **Forensic Requirement**: A permission-gated action to reopen a closed ticket, which should reverse payment transactions.
*   **MagiDesk Implementation**: No UI for reopening a closed ticket was found.
*   **Parity Gap**: The entire feature is missing.
*   **Assessed Status**: **NOT IMPLEMENTED**.

---

### F-0073: Refund Action

*   **Forensic Requirement**: A dialog-based workflow to refund a paid ticket.
*   **MagiDesk Implementation**: `TicketManagementPage.xaml` has a 'Refund Selected' button and associated command. This is a developer test UI, not a user-facing workflow.
*   **Parity Gap**: The user-facing dialog and workflow are missing.
*   **Assessed Status**: **PARTIAL**. The backend command exists, but the UI is **NOT IMPLEMENTED**.

---

### F-0074: User Transfer Dialog

*   **Forensic Requirement**: A dialog to transfer an open ticket from one server to another.
*   **MagiDesk Implementation**: No UI for transferring tickets between users was found.
*   **Parity Gap**: The entire feature is missing.
*   **Assessed Status**: **NOT IMPLEMENTED**.

---

### F-0075: Merge Tickets Action

*   **Forensic Requirement**: An action to merge two or more open tickets into one.
*   **MagiDesk Implementation**: No UI for merging tickets was found.
*   **Parity Gap**: The entire feature is missing.
*   **Assessed Status**: **NOT IMPLEMENTED**.

---

### F-0076: Multi-Ticket Payment

*   **Forensic Requirement**: A workflow to allow a single payment to cover multiple tickets.
*   **MagiDesk Implementation**: No UI for group settlement was found.
*   **Parity Gap**: The entire feature is missing.
*   **Assessed Status**: **NOT IMPLEMENTED**.

---

### F-0077: Customer Selector Dialog

*   **Forensic Requirement**: A dialog to search for and assign a customer to a ticket.
*   **MagiDesk Implementation**: No UI for customer selection was found.
*   **Parity Gap**: The entire feature is missing.
*   **Assessed Status**: **NOT IMPLEMENTED**.

---

### F-0078: Customer Form

*   **Forensic Requirement**: A form to create and edit customer information.
*   **MagiDesk Implementation**: `UserManagementPage.xaml` is a placeholder and no customer form exists.
*   **Parity Gap**: The entire feature is missing.
*   **Assessed Status**: **NOT IMPLEMENTED**.

---

### F-0079: Customer Explorer

*   **Forensic Requirement**: A back-office view to browse, search, and manage all customers.
*   **MagiDesk Implementation**: No customer explorer UI was found.
*   **Parity Gap**: The entire feature is missing.
*   **Assessed Status**: **NOT IMPLEMENTED**.

---

### F-0080: Change Table Action

*   **Forensic Requirement**: An action to move a ticket from one table to another.
*   **MagiDesk Implementation**: `TicketViewModel` has a `MoveTableUiCommand` that navigates to the `TableMapPage`, implying this is where the action should occur, but the page itself does not have the full workflow.
*   **Parity Gap**: The full workflow for selecting a new table and confirming the move is not implemented.
*   **Assessed Status**: **PARTIAL**. A command exists, but the UI workflow is incomplete.

---

### F-0081: Floor Explorer

*   **Forensic Requirement**: A back-office view to configure restaurant floor plans.
*   **MagiDesk Implementation**: No UI for floor plan management was found.
*   **Parity Gap**: The entire feature is missing.
*   **Assessed Status**: **NOT IMPLEMENTED**.
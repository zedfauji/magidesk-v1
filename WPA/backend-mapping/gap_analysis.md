
# WPA Backend Gap Analysis

## 1. Handlers Missing (Logic in ViewModels/Repos)
The following WPA endpoints do not have a corresponding `ICommandHandler` or `IQueryHandler` and currently rely on direct Repository access or logic embedded in WinUI ViewModels:

*   **Auth Login:** Logic is inside `SwitchboardViewModel.BackOfficeAsync` (calling `ISecurityService`). No standalone `LoginCommand`.
*   **Menu Browsing:** `OrderEntryViewModel` loads Categories/Groups/Items directly via `IMenuCategoryRepository`, `IMenuGroupRepository`, and `IMenuRepository`. No `GetMenuQuery`.
*   **Item Search:** Performed in-memory in `OrderEntryViewModel` (`SearchText` setter). No database-side search query exists for partial matches.

## 2. Desktop-Only Assumptions
*   **Context Dependency:** `AddOrderLineCommandHandler` and others rely on injected `ITerminalContext`. In WinUI, this is a singleton initialized at startup. In web API, this must be scoped per-request and resolved from the `Terminal-ID` header.
*   **Dialog Flows:** `SwitchboardViewModel` relies on `ISwitchboardDialogService` to prompt for "Guest Count" or "Shift Start" *during* a flow. The API cannot prompt. The Frontend must gather all data (Guests, ShiftID) *before* calling the standard `StartSession` or `CreateTicket` commands.
*   **Kitchen Printing:** `AddOrderLineCommandHandler` calls `IKitchenRoutingService`. This likely attempts direct print jobs. If the Web Server is not on the POS LAN, this will fail.

## 3. Structural Mismatches
*   **Batch Ordering:** WPA expects `POST /lines` with an array of items (Draft commit). `AddOrderLineCommandHandler` handles **single** item addition.
    *   *Gap:* API Controller must loop the command, or a new `AddOrderLinesBatchCommandHandler` is needed to wrap the transaction.
*   **Session vs Ticket:** `StartTableSessionCommandHandler` can optionally create a ticket, but the WPA flow might expect to create the ticket *first* or *implicitly*. The handler logic supports both, so this is just a configuration detail.

# Runtime Risks Assessment

## 🔴 Critical Risks (Must Fix Before Go-Live)

### 1. Partial Transaction Commit in Order Submission
*   **Endpoint:** `POST /api/orders/{ticketId}/lines`
*   **Trigger:** Submitting multiple items where one fails validation (e.g., Out of Stock) or system error occurs after the first item is processed.
*   **Mechanism:** The `OrdersController` Iterate-and-Execute loop invokes `AddOrderLineCommandHandler` for each item independently. Each handler invocation likely commits its own transaction.
*   **Impact:** Ticket state becomes inconsistent. User sees an error, but first 2 items are sent to kitchen and billed. Retry leads to duplicate orders.
*   **Mitigation:** Implement `AddOrderBaatchCommandHandler` with a single `UnitOfWork` scope.

### 2. Blind Price Trust
*   **Endpoint:** `POST /api/orders/{ticketId}/lines`
*   **Trigger:** Malicious user or bug in frontend sending `UnitPrice: 0.01` for a high-value item.
*   **Mechanism:** `AddOrderLineCommandHandler` accepts `Money` value from the Command inputs (derived from Controller DTO) and uses it to create the `OrderLine`, ignoring the `MenuItem`'s current price in the database.
*   **Impact:** Financial loss.
*   **Mitigation:** Handler must ignore input price or validate it against `MenuItem.Price`.

### 3. Missing Modifier Persistence (Data Loss)
*   **Endpoint:** `POST /api/orders/{ticketId}/lines`
*   **Trigger:** Ordering any item with modifiers.
*   **Mechanism:** The generated `OrdersController` contains a `// Gap: Modifiers = ...` comment and does not map the DTO modifiers to the Command.
*   **Impact:** Kitchen receives "Burger" without "No Onions". Health risk (allergies).
*   **Mitigation:** Implement mapping logic in Controller DTO -> Command.

### 4. Identity & Authorization Gap
*   **Endpoint:** All Protected Endpoints
*   **Trigger:** Any request.
*   **Mechanism:** The existing backend relies on `ISecurityService` and `ITerminalContext` which were Singletons in the Desktop app. The API `Program.cs` registers placeholders. If these are not correctly implemented as `Scoped` services that parse the JWT/Headers, the backend will treat every request as "System" or throw NullReference exceptions.
*   **Impact:** Auditing breakage, privilege escalation, or instant 500 crashes.

## 🟠 High Risks (Operational Issues)

### 5. Silent Printing Failures
*   **Endpoint:** `POST /api/orders/{ticketId}/lines`
*   **Trigger:** Web API Server hosted on Azure/Cloud while Printers are on-premise LAN.
*   **Mechanism:** `AddOrderLineCommandHandler` calls `IKitchenRoutingService` directy. This assumes network reachability to printers.
*   **Impact:** Orders accepted but cook never sees them.
*   **Mitigation:** Verify network topology or move printing to a polling agent/bridge.

## 🟡 Medium Risks (Edge Cases)

### 6. Search Functionality Missing
*   **Endpoint:** `GET /api/menu/items/search`
*   **Trigger:** User typing in search box.
*   **Mechanism:** Endpoint throws `NotImplementedException`.
*   **Impact:** UI Feature broken.

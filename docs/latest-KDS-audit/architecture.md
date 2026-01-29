# KDS Architecture Audit

## High-Level Pattern
The KDS implements a **Polled Data Architecure** with a **Shared Database State**.

**Diagram:**
`Order Entry` -> `KitchenRoutingService` -> `SQL Database` <- `Polling (10s)` <- `KitchenDisplayViewModel`

## Components

### 1. Ingress (Routing)
*   **Service:** `KitchenRoutingService`
*   **Trigger:** Manual "Print to Kitchen" command.
*   **Behavior:**
    *   Accepts `Ticket`.
    *   Aggregates **ALL** unprinted items into **ONE** `KitchenOrder`.
    *   **CRITICAL FLAW:** Ignores `PrinterGroup` (Station) granularity when creating the `KitchenOrder`, forcing a single unified display stream.

### 2. Persistence
*   **Entity:** `KitchenOrder` (Aggregate Root) -> `KitchenOrderItem`.
*   **Store:** Entity Framework Core (SQL).
*   **State:** Status enum (`New` -> `Cooking` -> `Done` / `Void`).

### 3. Egress (Display)
*   **Mechanism:** `DispatcherQueueTimer` (Interval: 10s).
*   **Query:** `KitchenOrderRepository.GetActiveOrdersAsync`.
*   **Filtering:** None. Fetches all active orders globally.
*   **UI Binding:** `KitchenDisplayViewModel` -> `ObservableCollection<KitchenOrderViewModel>`.

### 4. Updates (Mutations)
*   **Service:** `KitchenStatusService`.
*   **Actions:** `BumpOrderAsync`, `VoidOrderAsync`.
*   **Notification:** Calls `OrderNotificationService` (Stub only).

## Data Flow Analysis
1.  **Creation:** Ticket finalized -> `PrintToKitchenCommand` -> `KitchenRoutingService.RouteToKitchenAsync` -> DB Insert.
2.  **Visualization:** Timer Tick -> `KitchenOrderRepository` -> DB Select -> UI Refresh (Full List Clear/Add).
3.  **Completion:** User Click "Bump" -> `KitchenStatusService` -> DB Update -> Notification Stub Log.

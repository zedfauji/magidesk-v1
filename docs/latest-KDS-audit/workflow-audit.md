# KDS Workflow Audit

## 1. Order Routing (Ingress)
*   **Trigger:** Manual interaction. User clicks "Print Ticket" in Order Entry.
*   **Mechanism:** `PrintToKitchenCommandHandler`.
*   **Logic:**
    1.  Checks `Ticket.OrderLines` for `ShouldPrintToKitchen == true` AND `PrintedToKitchen == false`.
    2.  Calls `KitchenRoutingService.RouteToKitchenAsync`.
    3.  Service aggregates ALL qualifying lines into **ONE** `KitchenOrder`.
    4.  Saves to DB.
    5.  Handler marks lines as `PrintedToKitchen = true`.
*   **Gap:** No segregation by Station (Bar vs Kitchen). Single consolidated order only.

## 2. Order Display (Monitoring)
*   **Trigger:** Timer Tick (10s interval).
*   **Mechanism:** `KitchenDisplayViewModel.LoadOrdersAsync`.
*   **Logic:**
    1.  Calls `Repository.GetActiveOrdersAsync`.
    2.  Fetches orders where `Status != Done` AND `Status != Void`.
    3.  **Clears** the entire observable collection.
    4.  **Re-adds** all fetched orders.
*   **Gap:** UI Flash. Clearing and re-adding collection causes UI to flicker or reset scroll position every 10 seconds.
*   **Gap:** No caching or diffing.

## 3. Order Completion (Bump)
*   **Trigger:** User clicks "Bump" button on a specific order card.
*   **Mechanism:** `KitchenStatusService.BumpOrderAsync`.
*   **Logic:**
    1.  Load Order from DB.
    2.  Transition Status: `New` -> `Cooking` -> `Done`.
    3.  Save to DB.
    4.  Logs event (Notification Stub).
*   **Side Effect:** Order disappears from UI on next Poll (or manual Refresh) because `GetActiveOrdersAsync` filters out `Done`.

## 4. History View
*   **Trigger:** User toggles "History" mode.
*   **Mechanism:** `KitchenDisplayViewModel.LoadOrdersAsync`.
*   **Logic:**
    1.  Calls `Repository.GetCompletedOrdersAsync(50)`.
    2.  Displays last 50 done items.
    3.  Bumping is typically disabled or no-op in this view (logic exists but ambiguous in ViewModel).

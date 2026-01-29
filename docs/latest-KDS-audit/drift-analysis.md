# KDS Drift Analysis

This document identifies where the code implementation diverges from expected production KDS behavior or documented intent.

## 1. Unified Stream Drift (Critical)
*   **Expectation:** A KDS should support multiple stations (e.g., "Bar", "Grill", "Fryer"). An order with a Drink and a Burger should split: Drink -> Bar Screen, Burger -> Grill Screen.
*   **Implementation:** `KitchenRoutingService` creates a single `KitchenOrder` containing *every* unprinted item on the ticket, regardless of `PrinterMapping` or `PrinterGroup`.
*   **Evidence:** `KitchenRoutingService.cs` Line 50 created one order. Line 64 inserts `PrinterGroupId` into the item, but the **Parent Order** is monolithic.
*   **Impact:** Cannot deploy KDS in multi-station restaurants.

## 2. Real-Time Notification Drift
*   **Expectation:** Orders appear on screen immediately (sub-second) when sent.
*   **Implementation:** `OrderNotificationService` is a logging container with explicit `// TODO` for SignalR/WebSockets. The UI relies on a 10s polling loop.
*   **Evidence:** `OrderNotificationService.cs` Line 104; `KitchenDisplayViewModel.cs` Line 73.
*   **Impact:** Up to 10s latency. High operational friction (staff shouting "Did you get that?").

## 3. UI Performance Drift
*   **Expectation:** Smooth updates. New orders appear, old ones vanish, existing ones stay put.
*   **Implementation:** `KitchenDisplayViewModel` performs `Orders.Clear()` and `Orders.Add(...)` on every poll.
*   **Evidence:** `KitchenDisplayViewModel.cs` Line 96.
*   **Impact:** Visual flashing, loss of scroll context, strict repaint overhead every 10s.

## 4. Resilience Drift
*   **Expectation:** If KDS fails, Printing should still work (or vice versa).
*   **Implementation:** `PrintToKitchenCommandHandler` attempts routing first, then printing. Exceptions are caught genericly.
*   **Evidence:** `PrintToKitchenCommandHandler.cs`.
*   **Impact:** Reasonable resilience, but no tailored error handling for "KDS Down" scenario.

# KDS Risk Register

| ID | Risk | Severity | Type | Description | Mitigation Strategy |
|----|------|----------|------|-------------|---------------------|
| **R-01** | **Global Station Merging** | **Critical** | Functional | All ticket items appear on every KDS screen. A dedicated Bar screen will see Kitchen food and vice versa. | **Refactor Routing Service** to split orders by `PrinterGroupId`. |
| **R-02** | **Update Latency** | High | UX / Ops | 10s latency creates confusion. Server bumps order, Cook doesn't see it disappear immediately. | **Implement SignalR** for real-time `OrdersChanged` events. |
| **R-03** | **UI Flash / Reset** | Medium | UX | Full list reload every 10s resets scroll position, making it hard to read long lists during busy periods. | **Implement ObservableCollection Merging** logic instead of Clear/Add. |
| **R-04** | **Data Integrity** | Medium | Operational | If KDS goes down, no backup printing is inherently triggered (though Command Handler does try both). State of "Cooking" is only in RAM/DB, lost if local cache conceptually drifts (though Poll fixes this). | Ensure **Resiliency** patterns in Command Handler. |
| **R-05** | **Scalability** | Low | Performance | Polling `GetActiveOrders` with full table scan might slow down if thousands of active orders exist (unlikely for restaurant, but possible if "Done" orders aren't archived/filtered efficiently). | Ensure DB **Index** on `Status` and `Timestamp`. |

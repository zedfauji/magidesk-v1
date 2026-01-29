# KDS Dependency Matrix

## Inter-Module Dependencies

| Source Module | Target Module | Coupling Type | Risk |
|---------------|---------------|---------------|------|
| **KDS UI** (`KitchenDisplayViewModel`) | **Infrastructure** (`KitchenOrderRepository`) | **Direct (Tight)** | UI Logic depends directly on DB persistence shape. |
| **KDS UI** | **App Service** (`KitchenStatusService`) | **Direct** | UI handles business logic for Bumping via Service. |
| **Order Entry** (`PrintToKitchenCommandHandler`) | **Routing Service** (`KitchenRoutingService`) | **Direct** | Order Entry assumes successful KDS routing. |
| **Routing Service** | **Domain** (`Ticket`) | **Data** | Logic explicitly depends on `Ticket` structure (Flags: `ShouldPrintToKitchen`). |

## External Dependencies

| Dependency | Purpose | Status |
|------------|---------|--------|
| **Entity Framework Core** | State Persistence | **Critical**. KDS is effectively a View over SQL tables. |
| **DispatcherQueueTimer** | Loop / Polling | **Critical**. UI relies on this for "Real-time" feel. |
| **Logger** | Diagnostics | Standard. |

## Critical Path Analysis

**Order Entry -> KDS Display:**
1.  `Ticket` (Domain)
2.  `PrintToKitchenCommand` (App)
3.  `KitchenRoutingService` (App)
4.  `KitchenOrderRepository` (Infra)
5.  `SQL Database` (External)
6.  `KitchenDisplayViewModel` (Presentation) - *Polling*

**Risk:** Failure in Step 4 (DB) breaks Step 2 (Order Entry), potentially preventing the order from being placed even if the Printer works (Dual path logic in handler attempts to mitigate this, but KDS failure is treated as an error).

**Risk:** Latency in Step 5/6. Polling interval sets minimum latency (0-10s).

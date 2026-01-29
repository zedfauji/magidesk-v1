# KDS Code Inventory

## UI Layer
| File | Responsibility | Callers | Calls |
|------|----------------|---------|-------|
| `KitchenDisplayViewModel.cs` | Main KDS Screen Logic, Polling | `KitchenDisplayPage.xaml.cs` | `IKitchenOrderRepository`, `IKitchenStatusService` |
| `KitchenOrderViewModel.cs` | Individual Order Card Logic | `KitchenDisplayViewModel` | `KitchenOrder` (Entity) |
| `KitchenDisplayPage.xaml` | View definition | N/A | `KitchenDisplayViewModel` |
| `KitchenDisplayPage.xaml.cs` | Code-behind | Navigation Frame | `KitchenDisplayViewModel` |

## Application Layer
| File | Responsibility | Callers | Calls |
|------|----------------|---------|-------|
| `KitchenRoutingService.cs` | Creates KitchenOrders from Tickets | `PrintToKitchenCommandHandler` | `IKitchenOrderRepository`, `ITicketRepository` |
| `KitchenStatusService.cs` | Handles Bump/Void logic | `KitchenDisplayViewModel` | `IKitchenOrderRepository`, `IOrderNotificationService` |
| `OrderNotificationService.cs` | Stub for notifications | `KitchenStatusService` | Logger (Future: SignalR) |
| `PrintToKitchenCommandHandler.cs` | Entry point command | UI (OrderEntry) | `IKitchenRoutingService` |

## Domain Layer
| File | Responsibility | Callers | Calls |
|------|----------------|---------|-------|
| `KitchenOrder.cs` | Aggregate Root, State Logic | App Services | `KitchenOrderItem` |
| `KitchenOrderItem.cs` | Item details | `KitchenOrder` | N/A |
| `KitchenStatus.cs` | State Enum | Domain/App | N/A |

## Infrastructure Layer
| File | Responsibility | Callers | Calls |
|------|----------------|---------|-------|
| `KitchenOrderRepository.cs` | EF persistence | App Services | `ApplicationDbContext` |
| `KitchenOrderConfiguration.cs` | EF configuration | `ApplicationDbContext` | N/A |

## Interfaces
*   `IKitchenRoutingService`
*   `IKitchenStatusService`
*   `IKitchenOrderRepository`
*   `IOrderNotificationService`

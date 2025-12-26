# Technical Design: Kitchen Display System (KDS)

## 1. Overview
The KDS is a critical missing subsystem responsible for routing Ticket Items to specific display status (Kitchen), tracking their preparation status, and broadcasting updates to KDS terminals.

**Related Features:**
- F-0027: Send to Kitchen
- F-0088: Kitchen Display Window
- F-0090: Kitchen Status Selector
- F-0091: Kitchen Ticket List

## 2. Domain Model

### 2.1. Entities

#### `KitchenOrder` (New Aggregate)
Represents a specific "fire" event to the kitchen. A single Ticket might have multiple Kitchen Orders (e.g., Appetizers fired first, Entrees later).
- `id`: UUID
- `ticketId`: UUID
- `serverName`: String (Snapshot)
- `tableNumber`: String (Snapshot)
- `timestamp`: DateTime
- `status`: Enum (NEW, COOKING, DONE, VOID)
- `items`: List<KitchenOrderItem>

#### `KitchenOrderItem`
- `id`: UUID
- `kitchenOrderId`: UUID
- `ticketItemId`: UUID (Link to original sales item)
- `itemName`: String
- `quantity`: Int
- `modifiers`: List<String>
- `destinationId`: UUID (PrinterGroup/Station ID e.g., "Hot Line" vs "Cold Line")

### 2.2. Services

#### `KitchenRoutingService`
- **Responsibility**: Determines *where* an item should go based on `PrinterGroup` configuration.
- **Logic**:
    - If Item.PrinterGroup == "Grill", create KDS entry for "Grill Station".
    - If configuration says "Print Only", send to physical printer.
    - If "Display", create `KitchenOrder`.

#### `KitchenStatusService`
- **Responsibility**: Manages state transitions.
- **Methods**:
    - `fireOrder(ticketId, itemIds)`: Creates `KitchenOrder`.
    - `bumpOrder(kitchenOrderId)`: Transitions NEW -> COOKING -> DONE.
    - `bumpItem(kitchenOrderItemId)`: Granular bumping (optional).

## 3. Data Flow

1.  **Order Entry**: Server clicks "Send to Kitchen" (F-0027).
2.  **Routing**: Backend `KitchenRoutingService` iterates items.
    - Groups items by `Destination` (e.g., Bar vs Kitchen).
3.  **Creation**: Saves `KitchenOrder` records.
4.  **Notification**: Publishes `KitchenEvent.ORDER_FIRED` to Message Bus (or WebSocket).
5.  **Display**: KDS Terminals (F-0088) listening on WebSocket receive event and refresh List (F-0091).
6.  **Action**: Cook clicks "Bump" (F-0090).
7.  **Update**: Backend updates status, logs `TimeCommand` (Cook Time).

## 4. API Specification

| Method | Endpoint | Description |
| :--- | :--- | :--- |
| `GET` | `/api/kitchen/orders` | Get active orders. Params: `stationId`, `status`. |
| `POST` | `/api/kitchen/fire` | Send items to kitchen (Ticket -> KDS). |
| `POST` | `/api/kitchen/orders/{id}/bump` | Advance status (New -> Done). |
| `GET` | `/api/kitchen/stats` | Avg Cook Time metrics. |

## 5. Migration Strategy
- Create tables: `kitchen_orders`, `kitchen_order_items`.
- Implement `KitchenRoutingService`.
- Refactor `TicketService.save()` to call Routing if "Auto-Send" is on.

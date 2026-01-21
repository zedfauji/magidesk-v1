
# WPA Endpoint to Backend Handler Map

## 1. Authentication (`/api/auth`)

| Method | Endpoint | Backend Component | Mapping Strategy |
| :--- | :--- | :--- | :--- |
| **POST** | `/login` | `ISecurityService.GetUserByPinAsync` | **Direct Service Call.** No CommandHandler exists. API Controller must call `GetUserByPinAsync` and generate JWT. |
| **POST** | `/logout` | *None* | **Stateless.** Invalidate JWT on client or blacklist on server. WinUI `SwitchboardViewModel.LogoutCommand` is UI-only navigation. |
| **GET** | `/session` | `IUserService`, `ITerminalContext` | **Context Verification.** Return claims from current JWT and resolved Terminal ID. |

## 2. Table Management (`/api/tables`)

| Method | Endpoint | Backend Component | Mapping Strategy |
| :--- | :--- | :--- | :--- |
| **GET** | `/` | `GetActiveSessionsQueryHandler` | **Direct Map.** This handler fetches `TableRepository.GetAllAsync` AND `SessionRepository.GetActiveSessionsAsync` and merges them. Matches `TableSummary` requirement effectively. |
| **GET** | `/{tableId}` | `ITableRepository.GetByIdAsync` | **Repository Wrapper.** No specific QueryHandler for single table details with `TableExtension` data (capacity, zone). Need to expose generic repository or create Query. |
| **POST** | `/{id}/session/start` | `StartTableSessionCommandHandler` | **Direct Map.** Handles validation, active session check, table status update, and optional ticket creation. |
| **POST** | `/{id}/session/pause` | `PauseTableSessionCommandHandler` | **Direct Map.** |
| **POST** | `/{id}/session/resume` | `ResumeTableSessionCommandHandler` | **Direct Map.** |
| **POST** | `/{id}/session/end` | `EndTableSessionCommandHandler` | **Direct Map.** |
| **POST** | `/move` | `ChangeTableCommandHandler` | **Direct Map.** `MoveOrder` in API maps to `ChangeTableCommand` in backend. |

## 3. Menu (`/api/menu`)

| Method | Endpoint | Backend Component | Mapping Strategy |
| :--- | :--- | :--- | :--- |
| **GET** | `/categories` | `IMenuCategoryRepository.GetAllAsync` | **Gap / Repo Wrapper.** `OrderEntryViewModel` access repos directly. Need to expose via API Controller. |
| **GET** | `/items` | `IMenuRepository.GetItemsByGroupIdAsync` | **Gap / Repo Wrapper.** Logic exists in `MenuRepository`. API needs to accept `categoryId` and map to internal Group/Category logic. |
| **GET** | `/items/search` | `IMenuRepository` (Custom Query) | **Gap.** `OrderEntryViewModel` does filtering in memory or needs new Repo method. |
| **GET** | `/items/{id}/modifiers` | `IMenuRepository.GetByIdAsync` | **Repo Wrapper.** Returns item with `Include(x => x.ModifierGroups)`. |

## 4. Orders (`/api/orders`)

| Method | Endpoint | Backend Component | Mapping Strategy |
| :--- | :--- | :--- | :--- |
| **POST** | `/{ticketId}/lines` | `AddOrderLineCommandHandler` | **Direct Map.** This handler is heavy lifter: deducts stock, creates `OrderLine`, triggers `KitchenRoutingService`. **Note:** API must loop this command for multiple lines or create a `AddOrderLinesBatchCommand`. |
| **GET** | `/tickets/{ticketId}` | `GetTicketQueryHandler` | **Direct Map.** Returns `TicketDto`. |


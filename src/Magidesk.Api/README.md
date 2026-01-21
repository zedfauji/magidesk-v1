# Magidesk.Api

This project provides the ASP.NET Core Web API layer for the Magidesk POS system, exposing functionality to the **WPA (Web Progressive App)** frontend.

## 1. Controller Mapping

| WPA Endpoint | Controller Action | Backend Handler / Logic |
| :--- | :--- | :--- |
| **Auth** | | |
| `POST /api/auth/login` | `AuthController.Login` | `ISecurityService` (Direct) |
| `GET /api/auth/session` | `AuthController.GetSession` | *Middleware Context* (Stubbed) |
| **Tables** | | |
| `GET /api/tables` | `TablesController.GetAllTables` | `GetActiveSessionsQuery` + `TableRepo` |
| `GET /api/tables/{id}` | `TablesController.GetTableDetails` | `TableRepo.GetById` |
| `POST .../session/start` | `TablesController.StartSession` | `StartTableSessionCommandHandler` |
| **Menu** | | |
| `GET /api/menu/categories` | `MenuController.GetCategories` | `MenuCategoryRepo.GetAll` |
| `GET /api/menu/items` | `MenuController.GetItems` | `MenuRepo.GetActiveItems` (Filtered) |
| `GET .../items/search` | `MenuController.SearchItems` | **Not Implemented** (Gap) |
| **Orders** | | |
| `POST .../lines` | `OrdersController.SendOrderToKitchen` | `AddOrderLineCommandHandler` (Loop) |

## 2. Known Implementation Gaps

### Context Plumbing
The `Magidesk.Api` project relies on generic DI registration (`builder.Services.AddMagideskBackend(...)`).
Specific implementations for `IUserService` and `ITerminalContext` that read from **HTTP Headers** (JWT / X-Terminal-Id) are **MISSING** and must be implemented in the `Infrastructure` folder to replace the WinUI singleton versions.

### Batch Operations
The backend `AddOrderLineCommandHandler` only supports adding a single line. The `OrdersController` currently loops through the request items. This is not transactional. A failure on the 3rd item will leave the first 2 committed.
**Recommendation:** Create `AddOrderLinesBatchCommandHandler` in the Application layer.

### Menu Search
The `GET /api/menu/items/search` endpoint throws `NotImplementedException` because the WinUI app performs search in-memory within the ViewModel, and no dedicated text-search Query exists in the backend.

### Printing
Kitchen routing is triggered by the CommandHandlers. Ensure the Web Server has network access to the physical printers defined in the database, otherwise printing will fail silently or throw errors.

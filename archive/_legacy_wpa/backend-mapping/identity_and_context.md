
# Identity & Context Mapping

## 1. User Identity (`IUserService`)
*   **Current (WinUI):** Singleton service holding `CurrentUser` property. Set after PIN entry.
*   **Target (Web API):**
    *   **Source:** JWT in `Authorization` header (`Bearer <token>`).
    *   **Resolution:** API Middleware parses JWT claims (`sub` = UserId, `role` = Role).
    *   **Injection:** A Scoped implementation of `IUserService` must be created for the Web API that reads the current HttpContext User principal.

## 2. Terminal Identity (`ITerminalContext`)
*   **Current (WinUI):** Singleton loaded from `appsettings.json` or Registry at startup. Used for `StockMovement`, `Printing`, `Session` tracking.
*   **Target (Web API):**
    *   **Source:** `X-Terminal-Id` header sent by WPA client (stored in localStorage).
    *   **Resolution:** API Middleware validates the Terminal ID against `ITerminalRepository`.
    *   **Injection:** A Scoped implementation of `ITerminalContext` must be created to return this ID.

## 3. Shift Context
*   **Current (WinUI):** `SwitchboardViewModel` checks for open session via `_cashSessionRepository.GetOpenSessionByTerminalIdAsync`.
*   **Target (Web API):**
    *   Commands like `CreateTicketCommand` require `ShiftId`.
    *   The API Controller or the WPA Client must look up the current open Shift ID for the terminal before submitting orders.

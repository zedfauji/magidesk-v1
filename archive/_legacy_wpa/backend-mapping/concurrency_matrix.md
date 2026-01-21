
# Concurrency & Locking Matrix

| Endpoint | Entity | Version Field | Conflict Risk | Strategy |
| :--- | :--- | :--- | :--- | :--- |
| `POST /api/tables/{id}/session/start` | `Table`, `TableSession` | `RowVersion` (Implicit EF) | **Medium.** Two waiters starting same table. | Backend `StartTableSessionHandler` checks `GetActiveSessionByTableId` before creating. Race condition exists but low prob. |
| `POST /api/orders/{id}/lines` | `Ticket` | `Ticket.RowVersion` | **High.** Multiple devices adding items. | `AddOrderLineCommandHandler` updates `Ticket`. EF Core will throw `DbUpdateConcurrencyException`. |
| `POST /api/tables/{id}/session/end` | `TableSession` | `RowVersion` | **Medium.** | Standard EF-Core Optimistic Concurrency. |
| `POST /api/auth/login` | `User` | N/A | **Low.** | Read-only access for auth. |

## Notes
*   **Ticket Entity:** Verified `Ticket` contains `[Timestamp] public byte[] RowVersion { get; set; }` (standard EF pattern).
*   **Handling:** The API Layer **must** catch `DbUpdateConcurrencyException` and return `409 Conflict`.
*   **WPA Client:** Must handle `409` by reloading the ticket and re-prompting/re-merging the user's draft.

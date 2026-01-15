
# WPA API Contract Validation Report

## 1. Ambiguities & Assumptions

### Ticket vs. Session ID
- **Issue:** The UI uses `ticketId` almost exclusively to identify the order context (`IOrderService.getTicket`), but the screen is a "Table Session".
- **Assumption:** There is a 1:1 mapping between an active Table Session and a Ticket. The API endpoint `GET /api/orders/tickets/{ticketId}` returns an `ActiveSession` object, which conflates ticket details (items) with session details (time, hourly rate).
- **Backend Implication:** The backend must assemble a composite object containing both time-billing state and F&B order state for this endpoint.

### Draft State Persistence
- **Issue:** `ActiveSession` includes `draftItems` and `draftState`.
- **Assumption:** These fields are purely client-side state in the current Mock implementation. The `MockOrderService.getTicket` returns `draftItems: []` and `draftState: 'Idle'`.
- **Constraint:** The API does **not** need to persist draft items. The `POST` to `/lines` sends the draft items to be committed. The Backend does not see "Draft" items until they are sent.

### Table Status derivation
- **Issue:** `TableSummary` has `elapsedSeconds` and `totalAmount`.
- **Assumption:** These are calculated fields provided by the backend on the `GET /tables` list view.
- **Risk:** Calculating these in real-time for all tables might be expensive. The backend may need caching.

## 2. Implementation Gaps in Mocks

### Missing Error Handling
- The UI Mock services mostly throw generic `Error` or just dont fail.
- **Requirement:** The Backend API must return structured errors (e.g. `409 Conflict` for version mismatches) which the UI will need to handle (part of `draftState: 'Error'` logic).

### Timer Synchronization
- The UI initializes `elapsedSeconds` from the API but likely runs a local timer.
- **Risk:** Local timer drift.
- **Mitigation:** The UI should re-fetch/sync with the backend on key actions (Pause/Resume) to correct the displayed time.

## 3. Data Integrity
- **Versioning:** The `version` field is present in `ActiveSession`, `TableSummary`, and `CommittedOrderLine`. Usage is implied for Optimistic Concurrency but not explicitly demonstrated in the simple `sendOrderToKitchen` mock logic beyond returning an `updatedVersion`.
- **Requirement:** The backend MUST enforce validation of the `version` when `sendOrderToKitchen` is called to prevent overwriting updates made by other terminals.

## 4. Naming Mismatches
- `ITableService.endSession` returns `ActiveSession`. This is semantically used as a "Receipt" or "Summary" by the UI (`SessionSummaryScreen`).
- **Recommendation:** Ensure the backend `endSession` returns the final state of that session (frozen) so the UI can display the final totals.

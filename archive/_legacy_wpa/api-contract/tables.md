
# Table Management API

**Base Path:** `/api/tables`

## 1. Get All Tables
Retrieves a summary list of all tables for the grid view.

- **Method:** `GET`
- **Path:** `/`

### Response Body
```typescript
type TableStatus = 'Available' | 'Occupied' | 'Dirty' | 'Disabled';
type SessionStatus = 'NotStarted' | 'Running' | 'Paused' | 'Ended';

interface TableSummary {
  id: string;
  name: string;
  tableStatus: TableStatus;
  sessionStatus?: SessionStatus;
  elapsedSeconds?: number;
  totalAmount?: number;
  currentUserId?: string;
  isReservationLocked?: boolean;
  version: number;
}
[];
```

### Status Codes
- `200 OK`: Success.

---

## 2. Get Table Details
Retrieves detailed information for a specific table.

- **Method:** `GET`
- **Path:** `/{tableId}`

### Response Body
```typescript
interface TableExtension extends TableSummary {
  capacity: number;
  zoneName: string;
}
```

### Status Codes
- `200 OK`: Success.
- `404 Not Found`: Table ID invalid.

---

## 3. Start Session
Starts a new session on a specific table.

- **Method:** `POST`
- **Path:** `/{tableId}/session/start`

### Request Body
None (implied context).

### Status Codes
- `200 OK`: Session started.
- `409 Conflict`: Session already active or table not available.

---

## 4. Pause Session
Pauses the timer for an active session.

- **Method:** `POST`
- **Path:** `/{tableId}/session/pause`

### Request Body
None.

### Status Codes
- `200 OK`: Session paused.
- `400 Bad Request`: Session not active or already paused.

---

## 5. Resume Session
Resumes the timer for a paused session.

- **Method:** `POST`
- **Path:** `/{tableId}/session/resume`

### Request Body
None.

### Status Codes
- `200 OK`: Session resumed.

---

## 6. End Session
Ends the current session and returns the final session summary.

- **Method:** `POST`
- **Path:** `/{tableId}/session/end`

### Request Body
None.

### Response Body
Returns `ActiveSession` (used as a summary object).

### Status Codes
- `200 OK`: Session ended successfully.

---

## 7. Move Order / Table
Moves an order/session from one table to another.

- **Method:** `POST`
- **Path:** `/move`

### Request Body
```json
{
  "sourceTableId": "string",
  "targetTableId": "string"
}
```

### Status Codes
- `200 OK`: Move successful.
- `400 Bad Request`: Validation failure (e.g., target occupied).

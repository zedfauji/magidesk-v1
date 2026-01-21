
# Authentication API

**Base Path:** `/api/auth`

## 1. Login
Authenticates a user via PIN.

- **Method:** `POST`
- **Path:** `/login`

### Request Body
```json
{
  "pin": "string" // 4-digit PIN
}
```

### Response Body
Returns the authenticated user.
```typescript
interface User {
  id: string;
  username: string;
  firstName: string;
  lastName: string;
  role: 'Server' | 'Manager';
}
```

### Status Codes
- `200 OK`: Login successful.
- `401 Unauthorized`: Invalid PIN.

---

## 2. Logout
Ends the current session.

- **Method:** `POST`
- **Path:** `/logout`

### Request Body
None.

### Response Body
None.

### Status Codes
- `200 OK` or `204 No Content`: Logout successful.

---

## 3. Get Current Session
Retrieves the active session information if valid.

- **Method:** `GET`
- **Path:** `/session`

### Response Body
```typescript
interface AuthSession {
  token: string;
  user: User;
  terminalId: string;
  startedAt: string; // ISO 8601
}
```

### Status Codes
- `200 OK`: Session active.
- `401 Unauthorized`: No active session.

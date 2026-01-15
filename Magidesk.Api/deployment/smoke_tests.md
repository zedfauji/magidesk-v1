# API Smoke Test Matrix

| Endpoint | Test Case | Expected Result | Failure Signal |
| :--- | :--- | :--- | :--- |
| **GET /api/tables** | Public Access | 401 Unauthorized | 200 OK (Security Hole) |
| **POST /api/auth/login** | Valid Credentials | 200 OK + JWT Token | 401 / 500 Error |
| **GET /api/tables** | With Token | 200 OK + JSON List | 500 Error / Empty List |
| **POST .../session/start** | Table 5 | 200 OK | 409 Conflict |
| **POST .../lines** | Add Burger | 200 OK | 500 Error (Printing?) |
| **POST .../lines** | Add Invalid Item | 400 Bad Request | 500 Error (Unhandled) |
| **GET /api/tables** | Check Status | "Seat" / "Occupied" | "Available" (State mismatch) |

# Time & Billing Integrity Audit

## Clock Dependency
*   **Source of Truth:** `DateTime.UtcNow` injected via C# System Clock on the **Web Server**.
*   **Client Independence:** Tablet clock is ignored. (Good).
*   **Risk:** If the Web API is load-balanced across multiple nodes (e.g. Azure App Service w/ 3 instances) and their clocks drift, billing anomalies occur.

## Session Timing Logic
*   **Start:** Recorded as `UtcNow` on Request Start.
*   **Pause:** Recorded as `UtcNow` on Request Pause.
*   **Resume:** Adds `UtcNow - PausedAt` to `TotalPausedDuration`.
*   **End:** Uses `UtcNow - StartTime - TotalPausedDuration`.

### Scenario: The "Time Warp" Resume
1.  **Node A (Clock Correct):** Pauses Session at 12:00:00. `PausedAt` = 12:00:00.
2.  **Node B (Clock Fast +5min):** Resumes Session at 12:01:00 (Real Time), but Clock says 12:06:00.
3.  **Calc:** `Duration` = 12:06:00 - 12:00:00 = 6 minutes.
4.  **Result:** System thinks table was paused for 6 minutes, but it was only 1.
5.  **Billing:** The customer is *under-billed* by 5 minutes (because `TotalPausedDuration` is subtracted from total time).

## Recommendation
*   Ensure NTP synchronization on all hosting servers.
*   Acceptable risk for single-server on-premise deployments.

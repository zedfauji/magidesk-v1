# Concurrency Failure Mode Simulation

## Scenario 1: The "Double Tap" Start
*   **Actors:** Server A (iPad), Server B (Android)
*   **Action:** Both press "Start Session" on Table 5 at the exact same millisecond.
*   **Flow:**
    1.  Request A hits API. Reads Table 5 (Ver 1). Checks Active Session (None). Creates Session A.
    2.  Request B hits API. Reads Table 5 (Ver 1). Checks Active Session (None). Creates Session B.
    3.  Request A commits. Update Table 5 (Ver 1 -> 2). **Success**. Returns 200 OK.
    4.  Request B commits. Update Table 5 (Ver 1 -> 2). **FAILURE** (DbUpdateConcurrencyException).
*   **Result:** Server B receives 500 Internal Server Error (unhandled exception in Controller).
*   **UI Impact:** Server B sees a generic error. Upon refresh, sees Table 5 is busy (Session A).
*   **Verdict:** **Acceptable Safety**, Poor UX.

## Scenario 2: The "Ghost" Order Line
*   **Actors:** Server A, Kitchen
*   **Action:** Server A submits order. Connection drops during processing.
*   **Flow:**
    1.  Request contains [Burger, Fries, Coke].
    2.  Server processes Burger. Commits (Stock -1). Prints to Kitchen.
    3.  Server processes Fries. DB Connection blip. **FAILURE**.
    4.  API returns 500 Error to Server A.
    5.  Server A Retry Logic kicks in (Frontend auto-retry?).
    6.  Re-submits [Burger, Fries, Coke].
    7.  Server processes Burger *again*. Commits (Stock -1). Prints *again*.
*   **Result:** Kitchen makes 2 Burgers. Customer billed for 2 Burgers.
*   **Verdict:** ❌ **UNSAFE**. Lack of Idempotency on Order Submission.

## Scenario 3: The Race to Pay
*   **Actors:** Server A (taking Cash), Server B (adding Dessert)
*   **Action:** Server A is finalizing payment. Server B adds an item.
*   **Flow:**
    1.  Server A loads Ticket (Ver 10). Calculates Total: $50.
    2.  Server B adds "Cake" ($10). Ticket Ver 10 -> 11. Total: $60.
    3.  Server A submits "Pay $50".
*   **Behavior (Likely):** 
    *   Payment Handler usually re-loads Ticket. It sees Total $60.
    *   It applies $50 Payment. Remaining Due: $10.
    *   Ticket remains `Open`.
*   **Result:** Server A thinks they closed the table, but it stays open.
*   **Verdict:** **Acceptable Integrity**, Confusing UX.

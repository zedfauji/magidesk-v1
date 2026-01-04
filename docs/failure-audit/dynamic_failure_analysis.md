# Dynamic Failure Analysis

## Scenario: Database Connection Loss
*   **Trigger**: Postgres service stops or network drops.
*   **Behavior**:
    1.  Repo throws `NpgsqlException`.
    2.  Command Handler bubbles exception.
    3.  ViewModel (e.g. `LoginViewModel`) catches generic `Exception`.
    4.  ViewModel sets `ErrorMessage = "Error: ..."`
*   **Result**: User sees red text. App stays alive.
*   **Verdict**: **ACCEPTABLE**.

## Scenario: Corrupt Navigation (Invalid Page Type)
*   **Trigger**: Developer typo in `NavigationService` or `MainWindow`.
*   **Behavior**:
    1.  `OnItemInvoked` calls `NavigationService.Navigate`.
    2.  `Navigate` throws (unlikely) or Frame throws on invalid type.
    3.  `OnItemInvoked` is `async void` (Wait, it calls `.Navigate` synchronously).
    4.  If `Navigate` throws, `OnItemInvoked` crashes.
*   **Result**: **CRASH**.
*   **Verdict**: **FAIL**.

## Scenario: Background Timer Crash
*   **Trigger**: `TableMapViewModel` timer tick throws exception (Fixed in `TableMapConcurrency` task, but theoretically).
*   **Behavior**:
    1.  Timer callback executes on thread pool.
    2.  Exception thrown.
    3.  `System.Threading.Timer` swallows exceptions in callbacks (usually) OR crashes process depending on runtime.
    4.  In strict .NET 6/8, unhandled background exception terminates process.
*   **Result**: **CRASH**.
*   **Verdict**: **FAIL**.

## Scenario: Startup Failure
*   **Trigger**: Parsing `appsettings.json` fails.
*   **Behavior**:
    1.  `App()` constructor throws.
    2.  `Main` method (auto-generated) catches? No.
    3.  `App_UnhandledException` might catch if hooked.
    4.  Current Code: `App()` constructor has try-catch loops, catches and rethrows.
    5.  Rethrow bubbles to Windows Runtime.
*   **Result**: App closes silently or shows Windows generic error.
*   **Verdict**: **FAIL** (Needs unconditional Error Dialog before death).

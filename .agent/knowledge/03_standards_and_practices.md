# Standards and Best Practices

## WinUI 3 & MVVM Conventions
-   **Components:** Every View (`Page`, `UserControl`) must have a corresponding `ViewModel`.
-   **Bindings:** Use `x:Bind` (compiled bindings) where possible for performance and compile-time safety.
-   **Commands:** Use `RelayCommand` (from Toolkit) for UI actions. Avoid event handlers in code-behind unless strictly necessary for UI-only behavior (e.g., animations).
-   **State:**
    -   ViewModels must implement `INotifyPropertyChanged` (via `ObservableObject`).
    -   Collections should be `ObservableCollection<T>` for UI updates.
    -   Use `DispatcherQueue` for cross-thread UI updates.

## Coding Standards (Enterprise C#)
-   **Formatting:** Follow standard .NET conventions (PascalCase for public, camelCase for private/locals).
-   **Nullability:** `Nullable` context is enabled (`<Nullable>enable</Nullable>`). Handle null warnings explicitly.
-   **Async/Await:** Use `async await` all the way up. Avoid `.Result` or `.Wait()`. Use `ConfigureAwait(false)` in library code (Infrastructure/Domain), but strictly adhere to UI thread context in ViewModels/Presentation.

## Debugging & Observability
-   **Logging:** Log *state* and *context*, not just "Error happened".
-   **Exceptions:**
    -   Do not swallow exceptions silently.
    -   Bubble up if actionable; log and handle if top-level.
    -   Include stack traces.
-   **Traceability:** Match logs to code paths.

## Testing Standards
-   **Unit Tests:** Focus on `Domain` logic and `Application` services.
-   **ViewModel Tests:** Test public properties, commands, and state transitions. Mock services.
-   **Integration Tests:** Verify `Infrastructure` implementations (e.g., Repository database interactions) using test containers or in-memory DBs where appropriate.

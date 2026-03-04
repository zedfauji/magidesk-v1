# Rules Knowledge Item

## Authoritative Rule Set
Derived from `.agent/rules/`:

1.  **Architecture & Boundaries (`01`)**
    -   UI View code must NOT contain business logic.
    -   ViewModels encapsulate all non-UI logic.
    -   No cross-layer calls bypassing ViewModel -> Service -> Data.

2.  **Coding Standards (`09`)**
    -   **Structure:** One file = one primary responsibility. One public class per file.
    -   **Methods:** Max 40 logical lines. Do one thing. Prefer early returns.
    -   **Naming:** Reflect intent. No abbreviations. Boolean predicates (`Is*`, `Can*`).
    -   **Mutability:** Prefer immutable. Explicit mutation. No side effects in getters.

3.  **File Size Limits (`10`)**
    -   **Max File Length:** 300 lines (hard limit).
    -   **Splitting:** Split by responsibility. ViewModels must extract sub-viewmodels/partials if growing.

4.  **Testing (`07`)**
    -   All new public methods must have tests.
    -   UI interactions must be testable via VM.

5.  **Debugging (`12`)**
    -   Every failure path must be observable.
    -   Exceptions must include actionable context.

## Architectural Guardrails
-   **MVVM Strictness:** Code-behind (`.xaml.cs`) should contain *only* trivial wiring. All state and logic must reside in the ViewModel.
-   **Layering:** `Presentation` cannot reference `Infrastructure` directly (except for DI composition root). `Application` defines interfaces; `Infrastructure` implements them.
-   **State Management:** UI state updates *only* via observable properties (`INotifyPropertyChanged`).

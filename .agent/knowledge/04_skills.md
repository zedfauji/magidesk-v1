# Skills Knowledge Item

The following skills are defined in `.agent/skills` and must be used to enforce project standards.

## Skill List

1.  **coding-standards-enforcer**
    -   **Trigger:** Generating or refactoring code.
    -   **Purpose:** Enforce method (40 lines) and file (300 lines) limits, naming conventions, and single responsibility.
    -   **Relation to Rules:** Directly enforces `09_coding_standards.md` and `10_file_size_and_structure_limits.md`.

2.  **conventional-commits**
    -   **Trigger:** Creating commit messages.
    -   **Purpose:** Standardize commit history (`feat`, `fix`, `chore`, etc.).

3.  **debug-trace-analyzer**
    -   **Trigger:** Analyzing debug logs.
    -   **Purpose:** Match stack traces to code, detect patterns (null ref, dispatch), suggest fixes.
    -   **Relation to Rules:** Supports `12_debugging_and_observability.md`.

4.  **error-handling-guardian**
    -   **Trigger:** Generating try/catch or logging.
    -   **Purpose:** Prevent silent failures, ensure context in logs.
    -   **Relation to Rules:** Enforces `05_error_handling_and_no_silent_failure.md`.

5.  **file-decomposition-advisor**
    -   **Trigger:** File exceeds standards or user requests simplification.
    -   **Purpose:** Propose strategies to split large files (extract services, sub-VMs).
    -   **Relation to Rules:** Enforces `10_file_size_and_structure_limits.md`.

6.  **file-size-and-structure-enforcer**
    -   **Trigger:** Creating/modifying files.
    -   **Purpose:** Fail operations if file > 300 lines. Propose splitting.
    -   **Relation to Rules:** Strict enforcement of `10_file_size_and_structure_limits.md`.

7.  **observable-state-validator**
    -   **Trigger:** ViewModel property changes.
    -   **Purpose:** Ensure `INotifyPropertyChanged` is safe and `OnPropertyChanged` is called.
    -   **Relation to Rules:** Supports `06_ui_state_visibility.md`.

8.  **ui-architecture-enforcer**
    -   **Trigger:** Modifying/Generating UI or ViewModels.
    -   **Purpose:** Ensure View-ViewModel matching, no logic in code-behind, correct `DataContext`.
    -   **Relation to Rules:** Enforces `01_architecture_and_boundaries.md` and `02_mvvm_pattern.md`.

9.  **ui-state-debugger** (UI Debugging Assistant)
    -   **Trigger:** UI not updating, layout bugs.
    -   **Purpose:** Diagnose binding failures, suggest `DispatcherQueue` usage.

10. **winui-mvvm-validator**
    -   **Trigger:** Generating WinUI/ViewModel code.
    -   **Purpose:** Check strict MVVM adherence (logic in VM, not XAML.cs).
    -   **Relation to Rules:** Enforces `02_mvvm_pattern.md`.

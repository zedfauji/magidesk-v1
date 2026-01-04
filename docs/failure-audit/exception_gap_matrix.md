# Exception Gap Matrix

| Architectural Layer | Responsibility | Current State | Gap Analysis |
| :--- | :--- | :--- | :--- |
| **Presentation (Views)** | Surface errors to user. | **WEAK**. Code-behind lacks safety wrappers. `async void` handlers expose risk. | High risk of invisible crashes. |
| **Presentation (ViewModels)** | Manage state & error msgs. | **PASSIVE**. Sets `ErrorMessage` property. No proactive Dialog mechanism service linked to Global Handler. | Errors sit in string properties users might miss. |
| **Application (Commands)** | Orchestrate logic. | **STRONG**. Most handlers allow exceptions to bubble up. | Correct implementation, but relies on caller handling. |
| **Domain** | Business rules. | **STRONG**. Uses Custom Exceptions (`BusinessRuleViolation`). | Correct. |
| **Infrastructure (Data)** | DB Access. | **STRONG**. Propagates `DbContext` exceptions. Some Concurrency handling exists. | Correct. |
| **Infrastructure (Services)** | External I/O. | **MIXED**. Some services might catch-and-log unnecessarily. | Need to verify specific services (Printing, Payment). |

## Critical Gaps
1.  **The "Last Mile" Gap**: Exceptions bubble up from Domain/Infra correctly, but the UI layer (`ViewModel`/`View`) drops the ball. It either crashes (`async void`) or sets a passive text property.
2.  **No Dialogue Standard**: There is no standard service to say `DialogService.ShowError(ex)`.

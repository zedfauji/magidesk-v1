# Failure Handling Policy

## Core Principle
**"No Silent Failures."**

Every failure, whether expected (business rule) or unexpected (runtime exception), must be:
1.  **Captured**: Never swallowed without logging.
2.  **Traced**: Logged with stack trace and context.
3.  **Surfaced**: Presented to the operator via a user-friendly dialog if it interrupts their workflow.

## Exception Categories

| Category | Description | Handling Strategy | User Feedback |
| :--- | :--- | :--- | :--- |
| **FATAL** | App cannot continue (Startup fail, Corrupt state). | Global Handler -> Log -> Crash Dialog -> Exit. | "System Error. Application must restart." |
| **UNEXPECTED** | Bug or Infrastructure fail (Network, DB Timeout). | Command Handler -> Log -> Generic Dialog. | "An unexpected error occurred. [Error ID]" |
| **DOMAIN** | Business Rule Violation (Insufficient Funds). | Command Handler -> Warning Dialog. | Specific message (e.g., "Shift cannot be closed"). |
| **CANCELLATION** | User cancelled operation. | Catch `OperationCanceledException` -> Ignore. | None. |

## Handling Rules

1.  **Global Handler**: `App_UnhandledException` is the last line of defense. It MUST NOT swallow exceptions.
2.  **Async Void**: `async void` is FORBIDDEN except for top-level Event Handlers. All `async void` handlers MUST have a `try-catch` block wrapping the entire body.
3.  **Dispatcher**: `DispatcherQueue.TryEnqueue` operations MUST handle exceptions if they perform unsafe work.
4.  **Backend Propagation**: Repositories and Services should propagate exceptions. Do not catch-and-log in the backend unless you are recovering.
5.  **Translation**: The Presentation layer (ViewModels) is responsible for translating Exceptions to User Messages.

## Logging Standard
Format: `[Timestamp] [Level] [Component] Message | ExceptionType: Message | StackTrace`

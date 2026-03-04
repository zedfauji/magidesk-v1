# Threading and Async Safety

WinUI and .NET UI threads have strict constraints.

## Rules
- Do not run blocking operations on the UI thread.
- Use async/await patterns correctly when calling background tasks.
- Always marshal UI updates back to the UI thread using Dispatcher or appropriate helpers.

## Expected Behavior
Agents should generate async-safe code that does not freeze or deadlock the UI.

# Failure Governance Blockers

These issues represent **CRITICAL** risks to system stability and safety. They must be resolved before any further feature development.

| ID | Component | Issue | Impact | Fix Requirement |
| :--- | :--- | :--- | :--- | :--- |
| **BLK-001** | `MainWindow.xaml.cs` | `async void OnItemInvoked` has no error handling. | Navigation bugs will CRASH the application to desktop immediately. | Wrap body in `try { ... } catch (Exception ex) { HandleFatal(ex); }`. |
| **BLK-002** | `App.xaml.cs` | `App_UnhandledException` swallows logging errors. | If logging fails, app disappears without trace. | nest try-catch. Ensure last-ditch `MessageBox` or `Debug.WriteLine` works. |
| **BLK-003** | `NavigationService.cs` | `ShowDialogAsync` throws raw `InvalidOperationException` on timeout. | Dialog failures (common in weird focus states) will crash app. | Catch internally, return Failure result or retry robustly. |
| **BLK-004** | Global | No centralized API for showing Error Dialogs from ViewModels. | Devs rely on `ErrorMessage` strings or `Debug.WriteLine`. | Implement `IDialogService.ShowErrorAsync(string title, string message)`. |

# Ticket Verification Criteria

| Ticket | Trigger | Expected Behavior | Failure State (Forbidden) |
| :--- | :--- | :--- | :--- |
| **FEH-001** | Throw Exception inside `OnItemInvoked` | App stays alive. Dialog shows error. | Crash to Desktop. |
| **FEH-002** | Initial Logging Fails (permission denied) | Exception printed to Debug/Console. App attempts exit cleanly. | Crash to Desktop without trace. |
| **FEH-003** | Throw Exception in `ShiftStartDialog` | Dialog shows error. | Crash to Desktop. |
| **FEH-004** | Throw Exception in `TableDesigner` Nav | Dialog shows error. | Crash to Desktop. |
| **FEH-005** | Throw in `UpdateUiAuthState` | Logged to file. UI might stale, but app lives. | Crash to Desktop. |
| **FEH-006** | Call `IDialogService.ShowError` | Standard Dialog appears. | Wait indefinitely / Crash. |
| **NAV-001** | Call `ShowDialog` when Window inactive | Timeout -> Logged -> Returns `None`. | `InvalidOperationException` Crash. |
| **SYS-001** | `InitializeComponent` Fails | Native Window/MessageBox with Stack Trace. | Silent Exit. |
| **BEH-001** | Disconnect Printer -> Print | `PrintingException` caught in VM -> Shown to User. | Silent swallow / Crash. |

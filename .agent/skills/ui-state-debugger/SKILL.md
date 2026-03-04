---
name: ui-debugging-assistant
description: Diagnoses WinUI and XAML rendering or data binding issues.
---
# UI Debugging Assistant

**Trigger:** When the user asks about UI not updating, runtime layout bugs, or strange control state behavior.

**Goal**  
Help isolate UI logic and binding bugs.

**Instructions**
- Identify if bindings fail due to missing INotifyPropertyChanged.
- Suggest breakpoints and log points for state entry/exit.
- Suggest a minimal repro snippet.
- Recommend DispatcherQueue usage for cross-thread UI updates.

**Examples**
User: "UI stops updating after async call"
Skill:  
1) Check if DispatchQueue.RunAsync should be used  
2) Suggest breakpoints at property setters

---
name: ui-architecture-enforcer
description: Enforce WinUI 3 + MVVM architecture and binding best practices.
---
# UI Architecture Enforcer

**Trigger:** When modifying, generating, or reviewing UI (XAML) and ViewModels.

**Goal**  
Ensure every WinUI view follows MVVM patterns correctly.

**Instructions**
- Confirm each View (.xaml) has a strongly typed matching ViewModel class.
- Ensure UI logic is limited to triggers and bindings.
- No business logic in XAML code-behind.
- Validate DataContext usage.
- Alert if bindings could be inconsistent or uncontrolled (missing properties).

**Constraints**
- Do not suggest fixes that break MVVM separation.
- When suggesting property additions, generate matching INotifyPropertyChanged implementation.

---
name: winui-mvvm-validator
description: Validate MVVM adherence in WinUI projects.
---
# WinUI MVVM Validator

Use this skill whenever generating or modifying WinUI or ViewModel code.

Checks:
- Every View has a corresponding ViewModel.
- Logic must not appear in XAML code-behind except trivial wiring.
- UI state updates only via observable properties.

If violations are detected, produce corrections that:
- Move logic into ViewModel.
- Replace code-behind logic with bindings.

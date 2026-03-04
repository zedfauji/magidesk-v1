# MVVM Pattern Enforcement

This project uses WinUI with MVVM.

## Requirements
- Every View has a corresponding ViewModel.
- Commands and state mutations must live in ViewModels.
- Views bind to observable properties and do not directly contain logic.

## Enforcement Notes
Agents must generate code that adheres to MVVM patterns and must not collapse layers.

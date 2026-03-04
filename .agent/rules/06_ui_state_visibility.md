# UI State Visibility

UI logic must reflect clear user state transitions.

## Requirements
- UI controls must only be enabled/disabled based on observable state.
- Loading, error, and success states must be represented explicitly in ViewModels.
- No implicit state toggles without observable change notifications.

## Rationale
Agents must generate UI state code that is clear and traceable.

---
name: observable-state-validator
description: Detect & suggest fixes to binding/state propagation in ViewModels.
---
# Observable State Validator

**Trigger:** When ViewModels change state properties.

**Goal**
Validate property change patterns:
- Ensure INotifyPropertyChanged is implemented correctly
- Check that property setters call OnPropertyChanged
- Alert if UI may not detect state changes

**Instructions**
- Inspect setter methods
- Identify missing OnPropertyChanged calls
- Suggest minimal fixes

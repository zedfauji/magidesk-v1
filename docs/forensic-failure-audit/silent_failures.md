# Silent Failures Audit
> Failures that occur without alerting the user, leaving the system in an indeterminate or corrupt state.

| ID | Location | Trigger | Impact | Severity |
|----|----------|---------|--------|----------|
| SF-001 | `App.xaml.cs` (Constructor) | Background Thread Exception | Process Termination | CRITICAL |
| SF-002 | `OrderEntryViewModel.PrintTicketAsync` | Printer/Driver Error | Ticket not printed, user assumes success | HIGH |
| SF-003 | `OrderEntryViewModel.InitializeAsync` | DB/Network Error | Screen loads with stale/empty data | HIGH |
| SF-004 | `OrderEntryViewModel.ApplyModifiers` | Logic/DB Error | Modifiers missed, incorrect order | MEDIUM |
| SF-005 | `OrderEntryViewModel.LoadCategoriesAsync` | DB Error | Empty Menu Grid, no error | HIGH |
| SF-006 | `OrderEntryViewModel.LoadItemsAsync` | DB Error | Empty Item Grid | HIGH |
| SF-007 | `OrderEntryViewModel.PayNowAsync` | Payment Gateway/DB Error | Financial discrepancy, phantom payment | BLOCKER |
| SF-008 | `OrderEntryViewModel.SplitTicketAsync` | DB Error | Split fails, ticket stays merged | MEDIUM |
| SF-009 | `OrderEntryViewModel.SendToKitchenAsync` | DB/Routing Error | Kitchen doesn't cook, FOH thinks sent | BLOCKER |
| SF-010 | `OrderEntryViewModel.LoadTicketAsync` | DB Error | active ticket becomes null silently | HIGH |
| SF-011 | `OrderEntryViewModel.SearchItemAsync` | DB Error | Search yields no results (false negative) | MEDIUM |
| SF-012 | `OrderEntryViewModel.ModifyQuantityAsync` | DB Error | Qty visible mismatch vs DB | HIGH |
| SF-013 | `OrderEntryViewModel.RemoveItemAsync` | DB Error | Item reappears or stays | HIGH |
| SF-014 | `OrderEntryViewModel.QuickPayAsync` | DB/Payment Error | Payment fails silently | BLOCKER |

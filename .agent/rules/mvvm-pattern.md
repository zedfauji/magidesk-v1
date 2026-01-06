# MVVM Pattern Rules

## ViewModel Rules

### Zero Business Logic
- ViewModels contain NO business logic
- ViewModels are thin coordinators
- All business logic in Application/Domain layers
- ViewModels delegate to Application layer use cases

### Responsibilities
- Handle user input
- Manage UI state
- Coordinate with Application layer
- Format data for display
- Handle navigation
- Manage view lifecycle

### Prohibited in ViewModels
- ❌ Database access (no DbContext)
- ❌ Direct repository access
- ❌ Domain entity manipulation
- ❌ Business rule validation
- ❌ Complex calculations
- ❌ Domain service calls

### Allowed in ViewModels
- ✅ Call Application layer commands/queries
- ✅ Format DTOs for display
- ✅ Handle UI events
- ✅ Manage view state
- ✅ Input validation (UI-level only)
- ✅ Property change notifications

## View Rules

### XAML Only
- Views are primarily XAML
- Code-behind minimal (event handlers only)
- No business logic in code-behind
- Use data binding, not code-behind manipulation

### Code-Behind Limitations
- Only event handlers that delegate to ViewModel
- No business logic
- No data manipulation
- No direct service calls

### Data Binding
- Use data binding for all data display
- Use commands for all user actions
- Use converters for data transformation
- Use value converters, not code-behind

## Model Rules (DTOs)

### DTOs Only
- ViewModels work with DTOs from Application layer
- Never expose domain entities to UI
- DTOs are data containers (no behavior)
- DTOs can have validation attributes (UI-level)

### DTO Design
- Flat structure (no complex nesting)
- All properties public (for binding)
- No business logic
- Can have display attributes

## Commands

### ICommand Implementation
- Use RelayCommand or AsyncRelayCommand from CommunityToolkit.Mvvm
- Commands delegate to Application layer
- Commands can be async
- Commands handle errors gracefully

### Command Naming
- Action verbs: `CreateTicketCommand`, `ProcessPaymentCommand`
- CanExecute logic in ViewModel (simple checks only)
- Complex validation in Application layer

## Property Change Notifications

### INotifyPropertyChanged
- ViewModels implement INotifyPropertyChanged
- Use `[ObservableProperty]` attribute from CommunityToolkit.Mvvm
- Or use `SetProperty` helper method
- Notify on all property changes

### Observable Collections
- Use `ObservableCollection<T>` for collections
- Notify when items added/removed
- Use for lists that UI binds to

## Error Handling in ViewModels

### Visual Feedback (REQUIRED)
- **REQUIRE**: Use `IDialogService` (`ShowErrorAsync`, `ShowMessageAsync`) for ALL errors
- **REQUIRE**: Display user-friendly error messages via Toast or Dialog
- **BLOCK**: `Debug.WriteLine` as the specific method of error reporting (silent failure)
- **BLOCK**: Empty catch blocks

### Error Display
- Use error properties on ViewModel
- Bind to error display in View
- Clear errors when user corrects input

### Logging
- Log technical details specific logger (not just Debug console)
- Never expose stack traces to end user (unless internally debugging)

## ViewModel Lifecycle

### Initialization
- Load data in constructor or OnLoaded
- Use async initialization if needed
- Handle loading states

### Cleanup
- Implement IDisposable if needed
- Unsubscribe from events
- Cancel async operations

## Navigation

### Navigation Service
- Use navigation service (abstracted)
- ViewModels don't know about specific views
- Navigation through Application layer or navigation service
- Pass DTOs, not domain entities

## Data Validation

### UI-Level Validation
- Simple validation in ViewModel (required fields, format)
- Complex validation in Application layer
- Display validation errors in UI
- Prevent invalid submissions

### Validation Attributes
- Can use data annotations on DTOs
- FluentValidation in Application layer
- ViewModel shows validation results

## Testing ViewModels

### Unit Testing
- Mock Application layer services
- Test command execution
- Test property changes
- Test error handling

### Integration Testing
- Test ViewModel + Application layer integration
- Test navigation
- Test data flow

## Prohibited Patterns

### NEVER in ViewModels:
- Business logic
- Database access
- Domain entity manipulation
- Complex calculations
- Direct service instantiation (use DI)
- Hard-coded values (use configuration)

### NEVER in Views:
- Business logic
- Data manipulation
- Service calls
- Complex code-behind
- Direct ViewModel property access (use binding)

## CommunityToolkit.Mvvm Usage

### Attributes
- `[ObservableProperty]`: Auto-generates property with change notification
- `[RelayCommand]`: Auto-generates command
- `[AsyncRelayCommand]`: Auto-generates async command
- `[ICommand]`: For custom commands

### Best Practices
- Use attributes to reduce boilerplate
- Keep ViewModels focused
- One ViewModel per View
- Share ViewModels only if truly shared logic

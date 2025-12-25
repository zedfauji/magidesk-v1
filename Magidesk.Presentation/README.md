# Magidesk.Presentation

The presentation layer contains the WinUI 3 user interface. This layer depends only on the Application layer.

## Structure

```
Magidesk.Presentation/
├── Views/              # XAML views
│   ├── CashSessionView.xaml
│   ├── TicketView.xaml
│   └── PaymentView.xaml
├── ViewModels/         # MVVM view models
│   ├── CashSessionViewModel.cs
│   ├── TicketViewModel.cs
│   └── ...
├── Converters/         # Value converters
└── Resources/          # Styles, themes, resources
```

## Rules

- **Zero business logic**: ViewModels are thin coordinators
- **No database access**: All data through Application layer
- **MVVM pattern**: Strict separation of concerns
- **DTOs only**: Never expose domain entities

## Implementation Status

- [ ] ViewModels
- [ ] Views
- [ ] Navigation
- [ ] Dependency injection setup
- [ ] Styling and theming


# EXTENDED FORENSIC FAILURE AUDIT - COMPLETE FILE INDEX

## TOTAL FILES TO AUDIT: 647

### CRITICAL STARTUP & INFRASTRUCTURE
- App.xaml.cs
- MainWindow.xaml.cs
- Program.cs (API)
- All Services (Navigation, User, Terminal, Printing, etc.)

### PRESENTATION LAYER (WinUI 3)
#### Views (60+ files)
- AuthorizationCaptureBatchDialog.xaml.cs
- AuthorizationCodeDialog.xaml.cs
- BackOfficePage.xaml.cs
- CashDropManagementDialog.xaml.cs
- CashEntryDialog.xaml.cs
- CashSessionPage.xaml.cs
- Components/TableShapePalette.xaml.cs
- Components/VirtualizedTableCanvas.xaml.cs
- Dialogs/AddOnSelectionDialog.xaml.cs
- Dialogs/CashEntryDialog.xaml.cs
- Dialogs/ComboSelectionDialog.xaml.cs
- Dialogs/CookingInstructionDialog.xaml.cs
- Dialogs/CustomerSelectionDialog.xaml.cs
- Dialogs/GuestCountDialog.xaml.cs
- Dialogs/ItemSearchDialog.xaml.cs
- Dialogs/MergeTicketsDialog.xaml.cs
- Dialogs/MiscItemDialog.xaml.cs
- Dialogs/ModifierSelectionDialog.xaml.cs
- Dialogs/NotesDialog.xaml.cs
- Dialogs/PizzaModifierDialog.xaml.cs
- Dialogs/PriceEntryDialog.xaml.cs
- Dialogs/QuantityDialog.xaml.cs
- Dialogs/SeatSelectionDialog.xaml.cs
- Dialogs/ShiftEndDialog.xaml.cs
- Dialogs/ShiftStartDialog.xaml.cs
- Dialogs/SizeSelectionDialog.xaml.cs
- Dialogs/TableSelectionDialog.xaml.cs
- Dialogs/TicketFeeDialog.xaml.cs
- DiscountTaxPage.xaml.cs
- DrawerPullReportDialog.xaml.cs
- FloorManagementPage.xaml.cs
- GroupSettleTicketDialog.xaml.cs
- GroupSettleTicketSelectionWindow.xaml.cs
- InventoryPage.xaml.cs
- KitchenDisplayPage.xaml.cs
- LoginPage.xaml.cs
- MainPage.xaml.cs
- ManagerFunctionsDialog.xaml.cs
- MenuEditorPage.xaml.cs
- ModifierEditorPage.xaml.cs
- ModifierSelectionDialog.xaml.cs
- OpenTicketsListDialog.xaml.cs
- OrderEntryPage.xaml.cs
- OrderTypeExplorerPage.xaml.cs
- OrderTypeSelectionDialog.xaml.cs
- PasswordEntryDialog.xaml.cs
- PaymentPage.xaml.cs
- PaymentProcessWaitDialog.xaml.cs
- PrintPage.xaml.cs
- PurchaseOrdersPage.xaml.cs
- RoleManagementPage.xaml.cs
- SalesReportsPage.xaml.cs
- SettingsPage.xaml.cs
- SettlePage.xaml.cs
- ShiftExplorerPage.xaml.cs
- SplitTicketDialog.xaml.cs
- SwipeCardDialog.xaml.cs
- SwitchboardPage.xaml.cs
- SystemConfigPage.xaml.cs
- TableDesignerPage.xaml.cs
- TableDesignerTestPage.xaml.cs
- TableExplorerPage.xaml.cs
- TableMapPage.xaml.cs
- TicketManagementPage.xaml.cs
- TicketPage.xaml.cs
- UserManagementPage.xaml.cs
- VendorsPage.xaml.cs
- VoidTicketDialog.xaml.cs

#### ViewModels (60+ files)
- AuthorizationCaptureBatchViewModel.cs
- AuthorizationCodeViewModel.cs
- BackOfficeViewModel.cs
- CashDropManagementViewModel.cs
- CashSessionViewModel.cs
- Dialogs/AddOnSelectionViewModel.cs
- Dialogs/ComboSelectionViewModel.cs
- Dialogs/CookingInstructionViewModel.cs
- Dialogs/CustomerSelectionViewModel.cs
- Dialogs/ItemSearchViewModel.cs
- Dialogs/MergeTicketsViewModel.cs
- Dialogs/ModifierGroupViewModel.cs
- Dialogs/ModifierItemViewModel.cs
- Dialogs/ModifierSelectionViewModel.cs
- Dialogs/NotesDialogViewModel.cs
- Dialogs/PizzaModifierViewModel.cs
- Dialogs/PriceEntryViewModel.cs
- Dialogs/SeatSelectionViewModel.cs
- Dialogs/ShiftEndViewModel.cs
- Dialogs/ShiftStartViewModel.cs
- Dialogs/SizeSelectionViewModel.cs
- Dialogs/TableSelectionViewModel.cs
- DiscountTaxViewModel.cs
- DrawerPullReportViewModel.cs
- ExportImportManagementViewModel.cs
- FloorManagementViewModel.cs
- GroupSettleTicketSelectionViewModel.cs
- GroupSettleTicketViewModel.cs
- GuestCountViewModel.cs
- InventoryViewModel.cs
- KitchenDisplayViewModel.cs
- KitchenOrderViewModel.cs
- LoginViewModel.cs
- ManagerFunctionsViewModel.cs
- MenuEditorViewModel.cs
- MiscItemViewModel.cs
- ModifierEditorViewModel.cs
- ModifierSelectionViewModel.cs
- OpenTicketsListViewModel.cs
- OrderEntryViewModel.cs
- OrderTypeExplorerViewModel.cs
- OrderTypeSelectionViewModel.cs
- PaymentProcessWaitViewModel.cs
- PaymentViewModel.cs
- PrintViewModel.cs
- PurchaseOrderViewModel.cs
- QuantityViewModel.cs
- RecipeLineViewModel.cs
- RoleManagementViewModel.cs
- SalesReportsViewModel.cs
- ServerSectionManagementViewModel.cs
- SettingsViewModel.cs
- SettleViewModel.cs
- ShiftExplorerViewModel.cs
- SplitTicketViewModel.cs
- SwipeCardViewModel.cs
- SwitchboardViewModel.cs
- SystemConfigViewModel.cs
- TableDesignerViewModel.cs
- TableExplorerViewModel.cs
- TableMapViewModel.cs
- TicketFeeViewModel.cs
- TicketManagementViewModel.cs
- TicketViewModel.cs
- UserManagementViewModel.cs
- VendorManagementViewModel.cs
- ViewModelBase.cs
- VoidTicketViewModel.cs

#### Converters (19 files)
- BoolToDesignModeTextConverter.cs
- BoolToVisibilityConverter.cs
- CollectionEmptyToVisibilityConverter.cs
- CurrencyConverter.cs
- DateTimeToTimeConverter.cs
- DecimalToDoubleConverter.cs
- EnumToBoolConverter.cs
- IntToDoubleConverter.cs
- IntToSizeConverter.cs
- InverseBooleanConverter.cs
- NullToVisibilityConverter.cs
- ShapeToCornerRadiusConverter.cs
- StringColorToBrushConverter.cs
- StringFormatConverter.cs
- StringToBoolConverter.cs
- StringVisibilityConverter.cs
- TableStatusToBrushConverter.cs
- TableToSelectionVisibilityConverter.cs

### APPLICATION LAYER
#### Commands (80+ files)
- All command handlers and command definitions
- System configuration commands
- Payment processing commands
- Ticket management commands
- User management commands

#### Services & Handlers
- All query handlers
- Application services
- Event publishers

### INFRASTRUCTURE LAYER
#### Repositories (40+ files)
- All EF Core repository implementations
- Database configurations

#### Services
- Printing services
- Payment gateways
- Security services
- Backup services

### DOMAIN LAYER
#### Entities (60+ files)
- All domain entities
- Value objects
- Domain services
- Domain events

### API LAYER
- Controllers (6 files)
- Program.cs

---

**AUDIT STATUS**: PENDING
**TOTAL LINES TO ANALYZE**: ~50,000+
**CRITICAL PATHS**: Startup, Navigation, Payment, Printing
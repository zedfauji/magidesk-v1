# Complete Codebase File Index
## Extended Forensic Failure Audit - Phase 1

**Audit Date**: 2026-01-06  
**Total Files**: 157 (84 C#, 73 XAML)  
**Scope**: ENTIRE Magidesk codebase  
**Purpose**: Baseline enumeration for line-by-line failure surface analysis

---

## 1. Application Entry Points (3 files)

### Critical Startup/Shutdown Files
| File | Path | Lines | Risk Level |
|------|------|-------|------------|
| App.xaml.cs | `/App.xaml.cs` | ~600 | **CRITICAL** |
| MainWindow.xaml.cs | `/MainWindow.xaml.cs` | ~200 | **HIGH** |
| Program.cs | `/Magidesk.Api/Program.cs` | ~300 | **CRITICAL** |

**Analysis Priority**: BLOCKER - All unhandled exceptions here crash the app silently

---

## 2. ViewModels (71 files)

### Dialog ViewModels (18 files)
- AddOnSelectionViewModel.cs
- ComboSelectionViewModel.cs
- CookingInstructionViewModel.cs
- CustomerSelectionViewModel.cs
- ItemSearchViewModel.cs
- MergeTicketsViewModel.cs
- ModifierGroupViewModel.cs
- ModifierItemViewModel.cs
- ModifierSelectionViewModel.cs
- NotesDialogViewModel.cs
- PizzaModifierViewModel.cs
- PriceEntryViewModel.cs
- SeatSelectionViewModel.cs
- ShiftEndViewModel.cs
- ShiftStartViewModel.cs
- SizeSelectionViewModel.cs
- TableSelectionViewModel.cs
- QuantityViewModel.cs

### Page ViewModels (52 files)
- AuthorizationCaptureBatchViewModel.cs
- AuthorizationCodeViewModel.cs
- BackOfficeViewModel.cs
- CashDropManagementViewModel.cs
- CashSessionViewModel.cs
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
- OpenTicketsListViewModel.cs
- OrderEntryViewModel.cs
- OrderTypeExplorerViewModel.cs
- OrderTypeSelectionViewModel.cs
- PaymentProcessWaitViewModel.cs
- PaymentViewModel.cs
- PrintTemplatesViewModel.cs
- PrintViewModel.cs
- PurchaseOrderViewModel.cs
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
- TemplateEditorViewModel.cs
- TicketFeeViewModel.cs
- TicketManagementViewModel.cs
- TicketViewModel.cs
- UserManagementViewModel.cs
- VendorManagementViewModel.cs
- ViewModelBase.cs
- VoidTicketViewModel.cs
- TicketFeeViewModel.cs

### Base Classes (1 file)
- ViewModelBase.cs

**Analysis Priority**: HIGH - ViewModels contain async void event handlers, property setters, command execution

---

## 3. Services (128 files)

### Command Handlers (80+ files)
- AddCashDropCommandHandler.cs
- AddDrawerBleedCommandHandler.cs
- AddOrderLineComboCommandHandler.cs
- AddOrderLineCommandHandler.cs
- AddPayoutCommandHandler.cs
- AddTipsToCardPaymentCommandHandler.cs
- ApplyCouponCommandHandler.cs
- ApplyDiscountCommandHandler.cs
- AssignTableToTicketCommandHandler.cs
- AuthorizeCardPaymentCommandHandler.cs
- CalculateServiceChargeCommandHandler.cs
- CaptureCardPaymentCommandHandler.cs
- ChangeSeatCommandHandler.cs
- ChangeTableCommandHandler.cs
- ClockCommandHandlers.cs
- CloseCashSessionCommandHandler.cs
- CloseTicketCommandHandler.cs
- CreateOrderTypeCommandHandler.cs
- CreateShiftCommandHandler.cs
- CreateTableCommandHandler.cs
- CreateTableLayoutCommandHandler.cs
- CreateTicketCommandHandler.cs
- DeliveryCommandHandlers.cs
- ExportImportCommandHandlers.cs
- LogoutCommandHandler.cs
- MergeTicketsCommandHandler.cs
- ModifierCommandHandlers.cs
- ModifyOrderLineCommandHandler.cs
- OpenCashSessionCommandHandler.cs
- PayNowCommandHandler.cs
- PrintReceiptCommandHandler.cs
- PrintToKitchenCommandHandler.cs
- ProcessPaymentCommandHandler.cs
- RefundPaymentCommandHandler.cs
- RefundTicketCommandHandler.cs
- ReleaseTableCommandHandler.cs
- RemoveOrderLineCommandHandler.cs
- ServerSectionCommandHandlers.cs
- SetAdjustmentCommandHandler.cs
- SetAdvancePaymentCommandHandler.cs
- SetCustomerCommandHandler.cs
- SetDeliveryChargeCommandHandler.cs
- SetServiceChargeCommandHandler.cs
- SetTaxExemptCommandHandler.cs
- SettleTicketCommandHandler.cs
- SplitTicketCommandHandler.cs
- TransferTicketCommandHandler.cs
- TransferTicketToTableCommandHandler.cs
- UpdateOrderLineInstructionCommandHandler.cs
- UpdateOrderTypeCommandHandler.cs
- UpdateShiftCommandHandler.cs
- UpdateTableCommandHandler.cs
- UpdateTicketNoteCommandHandler.cs
- VoidCardPaymentCommandHandler.cs
- VoidTicketCommandHandler.cs
- UserCommandHandlers.cs
- TicketConfigurationCommandHandlers.cs
- CreateSystemBackupCommandHandler.cs
- RestoreSystemBackupCommandHandler.cs
- UpdateRestaurantConfigCommandHandler.cs
- UpdateTerminalConfigCommandHandler.cs
- UpdateCardConfigCommandHandler.cs
- UpdatePrinterGroupsCommandHandler.cs
- UpdatePrinterMappingsCommandHandler.cs

### Query Handlers (30+ files)
- GetAttendanceReportQueryHandler.cs
- GetCashOutReportQueryHandler.cs
- GetCreditCardReportQueryHandler.cs
- GetDeliveryReportQueryHandler.cs
- GetExceptionsReportQueryHandler.cs
- GetHourlyLaborReportQueryHandler.cs
- GetJournalReportQueryHandler.cs
- GetLaborReportQueryHandler.cs
- GetMenuUsageReportQueryHandler.cs
- GetPaymentReportQueryHandler.cs
- GetProductivityReportQueryHandler.cs
- GetSalesBalanceQueryHandler.cs
- GetSalesDetailQueryHandler.cs
- GetSalesSummaryQueryHandler.cs
- GetServerProductivityReportQueryHandler.cs
- GetTipReportQueryHandler.cs
- GetRestaurantConfigQueryHandler.cs
- GetSystemBackupsQueryHandler.cs
- GetTerminalConfigQueryHandler.cs
- GetCardConfigQueryHandler.cs
- GetPrinterGroupsQueryHandler.cs
- GetPrinterMappingsQueryHandler.cs
- GetAvailableTablesQueryHandler.cs
- GetCashSessionQueryHandler.cs
- GetCurrentCashSessionQueryHandler.cs
- GetCurrentShiftQueryHandler.cs
- GetDrawerPullReportQueryHandler.cs
- GetOpenTicketsQueryHandler.cs
- GetOrderTypeQueryHandler.cs
- GetShiftQueryHandler.cs
- GetShiftReportQueryHandler.cs
- GetTableMapQueryHandler.cs
- GetTableQueryHandler.cs
- GetTicketByNumberQueryHandler.cs
- GetTicketQueryHandler.cs
- GetUsersQueryHandler.cs

### Domain Services (18 files)
- BatchPaymentDomainService.cs
- CashSessionService.cs
- EventPublisher.cs
- GroupSettleService.cs
- KitchenRoutingService.cs
- KitchenStatusService.cs
- MerchantBatchService.cs
- PriceCalculator.cs
- SystemService.cs
- TableLayoutExporter.cs
- TicketCreationService.cs
- SystemInitializationService.cs
- CashDrawerService.cs
- LiquidTemplateEngine.cs
- PostgresBackupService.cs
- PrintingService.cs
- PrintContextBuilder.cs
- StartupLogger.cs

### Infrastructure Services (6 files)
- DefaultViewRoutingService.cs
- NavigationService.cs
- OrderEntryDialogService.cs
- SwitchboardDialogService.cs
- TerminalContext.cs
- UserService.cs
- WindowsDialogService.cs

**Analysis Priority**: CRITICAL - Command/Query handlers are backend execution paths

---

## 4. Repositories (28 files)

- CashSessionRepository.cs
- CategoryRepository.cs
- ClockEntryRepository.cs
- CouponRepository.cs
- CustomerRepository.cs
- DeliveryRepository.cs
- DiscountRepository.cs
- FloorRepository.cs
- InventoryItemRepository.cs
- InventoryRepositories.cs
- KitchenOrderRepository.cs
- MenuCategoryRepository.cs
- MenuGroupRepository.cs
- MenuModifierRepository.cs
- MenuRepository.cs
- MerchantGatewayConfigurationRepository.cs
- ModifierGroupRepository.cs
- OrderTypeRepository.cs
- PaymentBatchRepository.cs
- PaymentRepository.cs
- PrinterGroupRepository.cs
- PrinterMappingRepository.cs
- PrintTemplateRepository.cs
- RestaurantConfigurationRepository.cs
- RoleRepository.cs
- SalesReportRepository.cs
- ServerSectionRepository.cs
- ShiftRepository.cs
- TableLayoutRepository.cs
- TableRepository.cs
- TerminalRepository.cs
- TicketRepository.cs
- UserRepository.cs
- DatabaseCollection.cs

**Analysis Priority**: HIGH - Database operations can throw, connection failures

---

## 5. Converters (20 files)

- BoolToDesignModeTextConverter.cs
- BoolToVisibilityConverter.cs
- BooleanToStringConverter.cs
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

**Analysis Priority**: HIGH - Converters run on UI thread, can crash silently

---

## 6. API Controllers (6 files)

- CashController.cs
- KitchenController.cs
- MenuCategoriesController.cs
- MenuGroupsController.cs
- ReportsController.cs
- SystemController.cs

**Analysis Priority**: MEDIUM - API failures should return HTTP errors, but may not surface to UI

---

## 7. Views (XAML) (73 files)

### Dialogs (22 files)
- AddOnSelectionDialog.xaml
- CashEntryDialog.xaml
- ComboSelectionDialog.xaml
- CookingInstructionDialog.xaml
- CustomerSelectionDialog.xaml
- GuestCountDialog.xaml
- ItemSearchDialog.xaml
- MergeTicketsDialog.xaml
- MiscItemDialog.xaml
- ModifierSelectionDialog.xaml
- NotesDialog.xaml
- PizzaModifierDialog.xaml
- PriceEntryDialog.xaml
- QuantityDialog.xaml
- SeatSelectionDialog.xaml
- ShiftEndDialog.xaml
- ShiftStartDialog.xaml
- SizeSelectionDialog.xaml
- TableSelectionDialog.xaml
- TicketFeeDialog.xaml
- AuthorizationCodeDialog.xaml
- PasswordEntryDialog.xaml

### Pages (49 files)
- AuthorizationCaptureBatchDialog.xaml
- BackOfficePage.xaml
- CashDropManagementDialog.xaml
- CashSessionPage.xaml
- DiscountTaxPage.xaml
- DrawerPullReportDialog.xaml
- FloorManagementPage.xaml
- GroupSettleTicketDialog.xaml
- GroupSettleTicketSelectionWindow.xaml
- InventoryPage.xaml
- KitchenDisplayPage.xaml
- LoginPage.xaml
- MainPage.xaml
- ManagerFunctionsDialog.xaml
- MenuEditorPage.xaml
- ModifierEditorPage.xaml
- OpenTicketsListDialog.xaml
- OrderEntryPage.xaml
- OrderTypeExplorerPage.xaml
- OrderTypeSelectionDialog.xaml
- PaymentPage.xaml
- PaymentProcessWaitDialog.xaml
- PrintPage.xaml
- PrintTemplatesPage.xaml
- PurchaseOrdersPage.xaml
- RoleManagementPage.xaml
- SalesReportsPage.xaml
- SettingsPage.xaml
- SettlePage.xaml
- ShiftExplorerPage.xaml
- SplitTicketDialog_Fixed.xaml
- SplitTicketDialog.xaml
- SwipeCardDialog.xaml
- SwitchboardPage.xaml
- SystemConfigPage.xaml
- TableDesignerPage.xaml
- TableDesignerTestPage.xaml
- TableExplorerPage.xaml
- TableMapPage.xaml
- TemplateEditorPage.xaml
- TicketManagementPage.xaml
- TicketPage.xaml
- UserManagementPage.xaml
- VendorsPage.xaml
- VoidTicketDialog.xaml

### Components (2 files)
- TableShapePalette.xaml
- VirtualizedTableCanvas.xaml

### Root (2 files)
- App.xaml
- MainWindow.xaml

**Analysis Priority**: MEDIUM - XAML binding failures, event handler crashes

---

## 8. Commands (Domain) (50+ files)

Located in `Magidesk.Application/Commands/`:
- All command definitions that trigger handlers
- See Command Handlers section for execution paths

**Analysis Priority**: HIGH - Command execution without UI feedback

---

## 9. Queries (Domain) (30+ files)

Located in `Magidesk.Application/Queries/`:
- All query definitions that trigger handlers
- See Query Handlers section for execution paths

**Analysis Priority**: HIGH - Query failures may return null/empty without notification

---

## 10. Domain Models

Located in `Magidesk.Domain/`:
- Entity classes
- Value objects
- Domain events
- Aggregates

**Analysis Priority**: MEDIUM - Business logic validation failures

---

## Summary Statistics

| Category | File Count | Analysis Priority |
|----------|------------|-------------------|
| Entry Points | 3 | CRITICAL |
| ViewModels | 71 | HIGH |
| Services | 128 | CRITICAL |
| Repositories | 28 | HIGH |
| Converters | 20 | HIGH |
| Controllers | 6 | MEDIUM |
| Views (XAML) | 73 | MEDIUM |
| **TOTAL** | **329** | - |

---

## Phase 2 Analysis Order

1. **CRITICAL PATH** (Phase 2a): Entry points, global exception handlers
2. **BACKEND EXECUTION** (Phase 2b): Command/Query handlers, Services
3. **UI LAYER** (Phase 2c): ViewModels, Converters
4. **DATA LAYER** (Phase 2d): Repositories
5. **VIEW LAYER** (Phase 2e): XAML code-behind, event handlers

---

## Exclusions

The following are EXCLUDED from this audit (not source code):
- `/obj/` - Build artifacts
- `/bin/` - Compiled binaries
- `/packages/` - NuGet packages
- `/.vs/` - Visual Studio metadata
- `/docs/` - Documentation
- `/memory/` - System state files

---

**Next Phase**: Line-by-line failure surface analysis starting with CRITICAL PATH files.

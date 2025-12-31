# Feature Mapping (FloreantPOS <-> Current App)

## Scope
- **FloreantPOS reference**: `C:\Users\giris\Documents\Code\Redesign-POS\floreantpos`
- **Current app**: `C:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk`
- **Docs**: `Magidesk/docs/**` used as *intent index* and to locate expected surfaces.

## Match type definitions
- **FULL**: A corresponding feature exists with comparable surface + workflow (best-effort based on static code evidence).
- **PARTIAL**: Feature exists but has missing workflow steps, TODOs, hardcoded placeholders, or missing dependent systems.
- **MISSING**: Present in FloreantPOS but no corresponding implementation evidence found in current app.
- **EXTRA**: Present in current app but no corresponding FloreantPOS feature found in reference scan.

## Mapping table (by major surface groups)

### 1) Core shell / navigation
| FloreantPOS feature (evidence) | Current app feature (evidence) | Match type |
|---|---|---|
| Application bootstrap & initialization (`floreantpos/src/com/floreantpos/main/Application.java`) | App bootstrap + DI + `ISystemInitializationService.InitializeSystemAsync()` (`App.xaml.cs`) | PARTIAL |
| POS main window shell (`floreantpos/src/com/floreantpos/main/PosWindow.java`) | Main window + status clock + navigation view (`MainWindow.xaml(.cs)`) | PARTIAL |
| Root view navigation (`floreantpos/src/com/floreantpos/ui/views/order/RootView.java`) | `NavigationService` + `Frame.Navigate` (`Services/NavigationService.cs`) | PARTIAL |
| Login screen (`floreantpos/src/com/floreantpos/ui/views/LoginView.java`) | `LoginPage.xaml` + `LoginViewModel.cs` (PIN login) | PARTIAL |

### 2) Switchboard / open tickets / manager entry
| FloreantPOS feature (evidence) | Current app feature (evidence) | Match type |
|---|---|---|
| Switchboard: Open tickets & activity (`ui/views/SwitchboardView.java`) | `SwitchboardPage.xaml` + `SwitchboardViewModel.cs` (loads open tickets, starts new tickets) | PARTIAL |
| Open tickets list dialog (`ui/dialog/OpenTicketsListDialog.java`) | `OpenTicketsListDialog.xaml` + `OpenTicketsListViewModel.cs` | PARTIAL |
| Manager dialog / manager functions (`ui/dialog/ManagerDialog.java`) | `ManagerFunctionsDialog.xaml` + `ManagerFunctionsViewModel.cs` | PARTIAL |

### 3) Order entry
| FloreantPOS feature (evidence) | Current app feature (evidence) | Match type |
|---|---|---|
| Order entry view container (`ui/views/order/OrderView.java`) | `OrderEntryPage.xaml` + `OrderEntryViewModel.cs` | PARTIAL |
| Category view (`ui/views/order/CategoryView.java`) | Category browsing via repositories in `OrderEntryViewModel.cs` (UI in `OrderEntryPage.xaml`) | PARTIAL |
| Group view (`ui/views/order/GroupView.java`) | Group browsing via repositories in `OrderEntryViewModel.cs` | PARTIAL |
| Menu item view (`ui/views/order/MenuItemView.java`) | Item grid/browsing in `OrderEntryViewModel.cs` + `OrderEntryPage.xaml` | PARTIAL |
| Item search dialog (`ui/dialog/ItemSearchDialog.java`) | `Views/Dialogs/ItemSearchDialog.xaml` + `ItemSearchViewModel.cs` | PARTIAL |
| Misc ticket item dialog (`ui/dialog/MiscTicketItemDialog.java`) | `Views/Dialogs/MiscItemDialog.xaml` + `MiscItemViewModel.cs` | PARTIAL |
| Seat selection dialog (`ui/dialog/SeatSelectionDialog.java`) | Split-by-seat exists at command level (`SplitBySeatCommand.cs`) + `SplitTicketDialog.xaml` | PARTIAL |

### 4) Modifiers / customization
| FloreantPOS feature (evidence) | Current app feature (evidence) | Match type |
|---|---|---|
| Modifier selection dialog (`ui/views/order/modifier/ModifierSelectionDialog.java`) | `Views/Dialogs/ModifierSelectionDialog.xaml` + `ModifierSelectionViewModel.cs` | PARTIAL |
| Pizza modifier selection dialog (`ui/views/order/multipart/PizzaModifierSelectionDialog.java`) | `Views/Dialogs/PizzaModifierDialog.xaml` + `PizzaModifierViewModel.cs` | PARTIAL |
| Cooking instruction dialogs (`ui/dialog/SelectCookongInstructionDialog.java`, `NewCookongInstructionDialog.java`) | `Views/Dialogs/CookingInstructionDialog.xaml` + `CookingInstructionViewModel.cs` | PARTIAL |
| Combo item selection (`ui/dialog/ComboItemSelectionDialog.java`) | `Views/Dialogs/ComboSelectionDialog.xaml` + `ComboSelectionViewModel.cs` | PARTIAL |
| Size selection (`ui/.../MenuItemSize...` via modifiers) | `Views/Dialogs/SizeSelectionDialog.xaml` + `SizeSelectionViewModel.cs` | PARTIAL |
| Variable price entry (dialogs exist in Floreant UI model) | `Views/Dialogs/PriceEntryDialog.xaml` + `PriceEntryViewModel.cs` | PARTIAL |

### 5) Payments / settlement
| FloreantPOS feature (evidence) | Current app feature (evidence) | Match type |
|---|---|---|
| Settle ticket dialog (`ui/views/payment/SettleTicketDialog.java`) | `SettlePage.xaml` + `SettleViewModel.cs` | PARTIAL |
| Payment keypad + tender types (`ui/views/payment/PaymentView.java`) | Tender entry + quick cash + exact due + next amount in `SettleViewModel.cs` | PARTIAL |
| Swipe card dialog (`ui/views/payment/SwipeCardDialog.java`) | `SwipeCardDialog.xaml` + `SwipeCardViewModel.cs` | PARTIAL |
| Authorization code dialog (`ui/views/payment/AuthorizationCodeDialog.java`) | `AuthorizationCodeDialog.xaml` + `AuthorizationCodeViewModel.cs` | PARTIAL |
| Payment processing wait dialog (`ui/views/payment/PaymentProcessWaitDialog.java`) | `PaymentProcessWaitDialog.xaml` + `PaymentProcessWaitViewModel.cs` | PARTIAL |
| Group settle dialogs (`ui/views/payment/GroupSettleTicketDialog.java`) | `GroupSettleTicketDialog.xaml` + group settle VMs + `GroupSettleCommand.cs` | PARTIAL |
| Gift certificate dialog (`ui/views/payment/GiftCertDialog.java`) | No explicit GiftCert UI located (only simulated gift cert fields in `SettleViewModel.cs` via `ProcessPaymentCommand`) | PARTIAL |

### 6) Cash management
| FloreantPOS feature (evidence) | Current app feature (evidence) | Match type |
|---|---|---|
| Cash drop dialog (`ui/dialog/CashDropDialog.java`) | `CashDropManagementDialog.xaml` + `CashDropManagementViewModel.cs` | PARTIAL |
| Payout dialog (`ui/dialog/PayoutDialog.java`) | `CashEntryDialog.xaml` used by `PerformPayoutAsync()` in `SwitchboardViewModel.cs` | PARTIAL |
| Drawer pull report dialog (`ui/dialog/DrawerPullReportDialog.java`) | `DrawerPullReportDialog.xaml` + `DrawerPullReportViewModel.cs` + `GetDrawerPullReportQuery.cs` | PARTIAL |
| No sale / drawer kick (`actions/DrawerKickAction.java`, also in payment view) | `SettleViewModel.OnNoSaleAsync()` is simulated (TODO), plus `SwitchboardViewModel.PerformOpenDrawerAsync()` logs NoSale transaction | PARTIAL |

### 7) Tickets: split/void/refund/transfer
| FloreantPOS feature (evidence) | Current app feature (evidence) | Match type |
|---|---|---|
| Split ticket dialog (`ui/views/SplitTicketDialog.java`) | `SplitTicketDialog.xaml` + `SplitTicketViewModel.cs` + commands `SplitTicketCommand.cs`, `SplitBySeatCommand.cs` | PARTIAL |
| Void ticket dialog (`ui/dialog/VoidTicketDialog.java`) | `VoidTicketDialog.xaml` + `VoidTicketViewModel.cs` + `VoidTicketCommand.cs` | PARTIAL |
| Refund action (`actions/RefundAction.java`) | Refund command exists (`RefundTicketCommand.cs`, `RefundPaymentCommand.cs`) but UI surface not confirmed in this pass | PARTIAL |
| Transfer ticket (`ui/views/UserTransferDialog.java`) | `TransferTicketCommand.cs` exists; UI surface not confirmed in this pass | PARTIAL |

### 8) Tables / dining room
| FloreantPOS feature (evidence) | Current app feature (evidence) | Match type |
|---|---|---|
| Table map view (`ui/views/TableMapView.java`) | `TableMapPage.xaml` + `TableMapViewModel.cs` | PARTIAL |
| Table selector dialog (`ui/tableselection/TableSelectorDialog.java`) | No dedicated table selection dialog found; switchboard has explicit TODO for table-required order types | MISSING |
| Seat selection dialog (`ui/dialog/SeatSelectionDialog.java`) | Split-by-seat logic exists; dedicated seat selection UI not confirmed | PARTIAL |

### 9) Kitchen / KDS
| FloreantPOS feature (evidence) | Current app feature (evidence) | Match type |
|---|---|---|
| Kitchen display view (`demo/KitchenDisplayView.java`) and related KDS surfaces | `KitchenDisplayPage.xaml` + `KitchenDisplayViewModel.cs`; API `KitchenController.cs` provides orders/bump/void | PARTIAL |

### 10) Back office / configuration / reports
| FloreantPOS feature (evidence) | Current app feature (evidence) | Match type |
|---|---|---|
| Back office window (`bo/ui/BackOfficeWindow.java`) | `BackOfficePage.xaml` + `BackOfficeViewModel.cs` | PARTIAL |
| Reports suite (sales balance, exceptions, journal, etc.) | `ReportsController.cs` + multiple report queries in `Magidesk.Application/Queries/Reports/**` + `SalesReportsPage.xaml` | PARTIAL |
| Menu management (categories/groups/items/modifiers) | Editor pages exist: `MenuEditorPage.xaml`, `ModifierEditorPage.xaml` (UI); API controllers for menu categories/groups | PARTIAL |
| Printer configuration dialogs | No dedicated printer setup UI confirmed (printing service exists). | MISSING |

### 11) Extra (current app but not clearly in Floreant reference)
| Current app feature (evidence) | Closest FloreantPOS analog | Match type |
|---|---|---|
| `SystemConfigPage.xaml` + backup commands (`CreateSystemBackupCommand`, `RestoreSystemBackupCommand`) | Floreant has DB/config dialogs, but “system backup list” is not confirmed in scanned surfaces | EXTRA (UNVERIFIED) |

## Notes / uncertainty
- This mapping is generated from static code evidence and a limited set of targeted file reads.
- Where the current app contains commands without a confirmed UI surface, mapping is marked **PARTIAL** with explicit note.
- A subsequent pass can enumerate every doc feature ID (F-0001..F-0132) and map individually; current table focuses on the highest-signal surfaces.

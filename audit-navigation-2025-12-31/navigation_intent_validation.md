# Navigation Purpose & Intent Validation (Current App)

**Classification**:
- **CORRECT**: destination + mechanism align with FloreantPOS-like intent
- **INCOMPLETE**: present but missing key behavior/guards expected in FloreantPOS
- **MISLEADING**: entry exists but likely points to a debug/utility surface vs a POS workflow surface
- **MISWIRED**: destination appears wrong for intended purpose
- **UNKNOWN**: intent not provable from code evidence

**Sources**:
- Magidesk: `navigation_entries.md`, `navigation_graph.mmd`
- FloreantPOS: `floreant_navigation.md`

## Root-level navigation (MainWindow sidebar)
- **[Home → SwitchboardPage]**
  - **Intended (FloreantPOS-like)**: return to primary operational home/switchboard
  - **Actual**: sidebar Tag `home` navigates to `SwitchboardPage`
  - **Classification**: CORRECT

- **[Cash Session → CashSessionPage]**
  - **Intended**: cash drawer/session management surface
  - **Actual**: navigates to `CashSessionPage`
  - **Classification**: UNKNOWN (no Floreant baseline mapping proven)

- **[Ticket Entry → TicketPage]**
  - **Intended**: UNKNOWN
  - **Actual**: navigates to `TicketPage` which exposes manual create/load ticket + diagnostic actions (split/move/settle)
  - **Classification**: MISLEADING (appears utility/debug vs POS flow)

- **[Payments → PaymentPage]**
  - **Intended**: payment processing flow
  - **Actual**: navigates to `PaymentPage` with manual load ticket + process payment fields
  - **Classification**: MISLEADING (appears utility/debug vs POS settle workflow)

- **[Discount & Tax → DiscountTaxPage]**
  - **Intended**: manage discount/tax configuration (Back Office function in Floreant)
  - **Actual**: navigates to `DiscountTaxPage`
  - **Classification**: INCOMPLETE (exposed at root; Floreant typically groups admin tools under back office)

- **[Printing → PrintPage]**
  - **Intended**: printing / reports
  - **Actual**: navigates to `PrintPage`
  - **Classification**: UNKNOWN

- **[Ticket Management → TicketManagementPage]**
  - **Intended**: open tickets management (resume/void/transfer/split)
  - **Actual**: navigates to `TicketManagementPage` (manual admin-like operations)
  - **Classification**: INCOMPLETE (Floreant-like access typically through manager/back office, not main root)

- **[Drawer Pull Report → DrawerPullReportDialog]**
  - **Intended**: manager report
  - **Actual**: opens report dialog directly from root
  - **Classification**: INCOMPLETE (Floreant-like: manager/back office permission gating expected)

- **[Sales Reports → SalesReportsPage]**
  - **Intended**: reporting (Back Office)
  - **Actual**: page reachable from root and also from BackOffice internal nav
  - **Classification**: INCOMPLETE (placement differs from Floreant-like grouping)

- **[User Management → UserManagementPage]**
  - **Intended**: staff/permissions management (Back Office)
  - **Actual**: page reachable from root and from BackOffice internal nav
  - **Classification**: INCOMPLETE (placement differs; no permission gating evidenced)

- **[Settings → SettingsPage]**
  - **Intended**: system/app settings
  - **Actual**: page reachable from root and from BackOffice internal nav
  - **Classification**: INCOMPLETE (placement differs)

## Switchboard (operational home)
- **[New Ticket]**
  - **Intended (FloreantPOS-like)**: start a new ticket guided by order type constraints
  - **Actual**: OrderTypeSelectionDialog → conditional GuestCount/ShiftStart dialogs → navigate to OrderEntryPage
  - **Classification**: CORRECT (conceptually aligned)

- **[Edit Selected]**
  - **Intended**: resume an open ticket
  - **Actual**: navigates to OrderEntryPage when ticket selected
  - **Classification**: CORRECT

- **[Edit Selected fallback → TicketManagementPage]**
  - **Intended**: UNKNOWN
  - **Actual**: if no ticket selected, navigates to TicketManagementPage
  - **Classification**: MISLEADING (fallback is not a user-facing “resume ticket” flow)

- **[Table Map]**
  - **Intended (FloreantPOS-like)**: table selection / resume ticket
  - **Actual**: navigates to TableMapPage
  - **Classification**: CORRECT

- **[Kitchen Display]**
  - **Intended (FloreantPOS-like)**: KDS view
  - **Actual**: navigates to KitchenDisplayPage
  - **Classification**: CORRECT

- **[Manager]**
  - **Intended (FloreantPOS-like)**: access other functions / manager actions
  - **Actual**: opens ManagerFunctionsDialog
  - **Classification**: PARTIAL/INCOMPLETE (dialog-based vs view-based; permission gating unknown)

- **[Logout]**
  - **Intended (FloreantPOS-like)**: exit to login and teardown session
  - **Actual**: navigates to LoginPage
  - **Classification**: INCOMPLETE (window/session teardown behavior unknown)

## Manager Functions dialog
- **[Open Tickets]**
  - **Intended**: manage open tickets
  - **Actual**: OpenTicketsListDialog provides Resume/Transfer/Void
  - **Classification**: CORRECT (core actions present)

- **[Cash Drop]**
  - **Intended**: cash management
  - **Actual**: CashDropManagementDialog → CashEntryDialog
  - **Classification**: CORRECT

- **[Drawer Pull]**
  - **Intended**: end-of-shift / drawer report
  - **Actual**: opens DrawerPullReportDialog
  - **Classification**: CORRECT

- **[Reports → BackOfficePage]**
  - **Intended (FloreantPOS-like)**: enter back office/reporting area
  - **Actual**: navigates to BackOfficePage inside root frame
  - **Classification**: INCOMPLETE (Floreant opens separate BackOfficeWindow; permission gating not evidenced)

- **[System Config → SystemConfigPage]**
  - **Intended**: administrative settings
  - **Actual**: navigates to SystemConfigPage
  - **Classification**: INCOMPLETE (placement + permission gating unknown)

- **[Group Settle]**
  - **Intended**: multi-ticket settlement workflow
  - **Actual**: selection dialog → group settle dialog
  - **Classification**: UNKNOWN (no Floreant mapping proven)

## Ticket/Page-level operational flows
- **[OrderEntryPage → SettlePage]**
  - **Intended**: proceed to settlement
  - **Actual**: navigates to SettlePage with Ticket.Id
  - **Classification**: CORRECT

- **[SettlePage payment dialogs]**
  - **Intended**: payment processing dialogs
  - **Actual**: SwipeCardDialog / AuthorizationCodeDialog / PaymentProcessWaitDialog
  - **Classification**: CORRECT (flow exists; parity of details unknown)

## Orphans / questionable surfaces (intent)
- **MainPage**
  - **Classification**: MISLEADING/ORPHAN (template page, no inbound navigation)

- **PasswordEntryDialog**
  - **Classification**: ORPHAN (no inbound navigation evidence)

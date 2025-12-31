# Recommendations (Neutral)

## Fix first (stabilization / correctness)
- **Establish authoritative runtime context**
  - **Why**: Multiple critical flows rely on hardcoded GUID fallbacks.
  - **Evidence**: `SwitchboardViewModel.cs` hardcoded `terminalId`, `userId`, `shiftId` fallbacks.

- **Make No Sale / open drawer behavior consistent and non-simulated**
  - **Why**: Operational cash controls require deterministic behavior.
  - **Evidence**: `SettleViewModel.OnNoSaleAsync()` is simulated; `SwitchboardViewModel.PerformOpenDrawerAsync()` logs a transaction.

- **Complete table-required and customer-required new ticket workflows**
  - **Why**: Ticket creation is explicitly blocked for such order types.
  - **Evidence**: `SwitchboardViewModel.NewTicketAsync()` returns early with "not yet linked" dialogs.

- **Harden dialog presentation to avoid XamlRoot null crashes**
  - **Why**: `NavigationService.ShowDialogAsync` throws if `Frame.XamlRoot` is null.
  - **Evidence**: `Services/NavigationService.cs`.

## Can safely wait (once core loop is stable)
- **Expand report UI coverage across all report types**
  - **Evidence**: `ReportsController.cs` provides endpoints; UI parity per report not confirmed.

- **Refine back office configuration surfaces**
  - **Evidence**: `MenuEditorPage.xaml`, `ModifierEditorPage.xaml`, `SettingsPage.xaml` exist.

## Consider removing/defer (until dependencies exist)
- **Features that imply hardware integration without a hardware abstraction layer**
  - **Reason**: Without a stable peripheral layer, UI flows tend to remain simulated.
  - **Evidence**: card processing is partially simulated in `SettleViewModel.cs`.

## Notes
- These items are ordered by dependency risk and operational impact, not feature preference.

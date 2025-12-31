# FloreantPOS Navigation Baseline (Reference)

**Scope**: Extracted from FloreantPOS source under `C:\Users\giris\Documents\Code\Redesign-POS\floreantpos`.

## Root navigation container
- **Class**: `com.floreantpos.ui.views.order.RootView`
- **Mechanism**: `CardLayout` with a map of `IView` instances.
  - `addView(IView)` registers a view and adds its component into the card layout.
  - `showView(String viewName)` / `showView(IView view)` switches the active card.
  - Header visibility is toggled:
    - `LoginView` hides the header.
    - Other views show the header.

## Startup / default routing behavior
- **Method**: `RootView.showDefaultView()`
- **Behavior**: Reads `TerminalConfig.getDefaultView()` and routes:
  - If default is `SwitchboardOtherFunctionsView.VIEW_NAME`:
    - `setAndShowHomeScreen(SwitchboardOtherFunctionsView.getInstance())`
  - If default is `KitchenDisplayView.VIEW_NAME`:
    - adds `KitchenDisplayView` if absent, hides header, shows it.
  - If default is `SwitchboardView.VIEW_NAME`:
    - if `loginScreen.isBackOfficeLogin()` then calls `showBackOffice()`; then sets home to `SwitchboardView`.
  - Else: treats it as an `OrderType` name and chooses one of:
    - `TableMapView` (if order type requires table selection)
    - `CustomerView` or plugin-provided delivery dispatch view (if customer data required)
    - else attempts to create a new ticket for `OrderView`.

## Primary view switching actions
- **Switchboard view action**
  - **Class**: `com.floreantpos.actions.SwithboardViewAction`
  - **Exec**: `RootView.getInstance().showView(SwitchboardView.getInstance())`

- **Home screen action**
  - **Class**: `com.floreantpos.actions.HomeScreenViewAction`
  - **Exec**: `RootView.getInstance().showHomeScreen()`

- **Other Functions action**
  - **Class**: `com.floreantpos.actions.ShowOtherFunctionsAction`
  - **Exec**: `RootView.getInstance().showView(SwitchboardOtherFunctionsView.getInstance())`

## Back Office behavior (separate window)
- **Action**: `com.floreantpos.actions.ShowBackofficeAction`
  - Requires permission `UserPermission.VIEW_BACK_OFFICE`.
  - Opens (or reuses) `BackOfficeWindow` and brings it to front.

- **Window**: `com.floreantpos.bo.ui.BackOfficeWindow`
  - Uses a Swing `JMenuBar` with menus built based on permissions:
    - Admin menu (`PERFORM_ADMINISTRATIVE_TASK`)
    - Explorer menu (`VIEW_EXPLORERS`)
    - Reports menu (`VIEW_REPORTS`)
    - Floor menu always added

## Logout behavior
- **Action**: `com.floreantpos.actions.LogoutAction`
  - Iterates over all `Window.getWindows()`.
  - Closes any window that is not the main `PosWindow`.
  - Calls `Application.getInstance().doLogout()`.

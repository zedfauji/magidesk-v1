# Codebase Concerns

**Analysis Date:** 2026-03-23

## Tech Debt

**Incomplete Pricing Service Implementation:**
- Issue: `SimplePricingService` is a placeholder that only performs basic time-based calculations. Does not support first-hour pricing, time rounding, minimum charges, or peak/off-peak rates.
- Files: `src/Magidesk.Infrastructure/Services/SimplePricingService.cs`
- Impact: Time-based pricing (e.g., hourly table charges) will be inaccurate. Blocks BE-A.9-01 feature set.
- Fix approach: Replace with full `AdvancedPricingService` that handles all pricing rules from domain. Track by task BE-A.9-01.

**Hardcoded Tax Rate:**
- Issue: Tax rate is hardcoded as 8% decimal literal in ViewModel rather than loaded from configuration.
- Files: `src/Magidesk.Presentation/ViewModels/OrderPageViewModel.cs:173`
- Impact: Tax calculation cannot be updated without recompiling. Multi-jurisdiction support impossible.
- Fix approach: Inject `ITaxConfigurationService` (or similar) to load tax rates from database or config. Store in `ITerminalContext` or DI.

**Manual Auth Code Bypass Without Audit Trail:**
- Issue: `AuthorizeCardPaymentCommandHandler` allows manual authorization codes without strong validation or special audit logging.
- Files: `src/Magidesk.Application/Services/AuthorizeCardPaymentCommandHandler.cs:76-87`
- Impact: Creates PCI/compliance risk. Manual auth codes bypass gateway validation. No differentiation in audit logs between gateway-authorized and manually-authorized payments.
- Fix approach: Require manager role check before allowing manual auth. Log with explicit audit event type `ManualAuthorizationApplied`. Add flag to CreditCardPayment entity.

**Temporary Routing Workaround:**
- Issue: `DefaultViewRoutingService` has hardcoded return to SwitchboardPage to bypass auto-redirect issue.
- Files: `src/Magidesk.Presentation/Services/DefaultViewRoutingService.cs:51-53`
- Impact: Terminal-specific routing rules (KDS, BAR) are never evaluated. All users land on home instead of their role-appropriate page. Performance implications for multi-terminal deployments.
- Fix approach: Properly seed OrderTypes in database. Use TerminalConfig table to map terminals to default order types. Enable conditional routing based on that config.

**Missing Refund Calculation in Cash Balance:**
- Issue: `CashBalanceTrackingService.CreateBalanceDto()` hardcodes `totalCashRefunds = 0m` with TODO comment instead of calculating from refund payments.
- Files: `src/Magidesk.Infrastructure/Services/CashBalanceTrackingService.cs:298`
- Impact: Cash balance reports are inaccurate when refunds occur. Cash drawer reconciliation will fail. Daily settlement reports cannot be trusted.
- Fix approach: Query refund payments from session and sum by payment method. Ensure only Cash-type refunds are included.

## Known Bugs

**WinUI NavigationView MeasureOverride Crash:**
- Symptoms: E_INVALIDARG (0xc000027b) crash during navigation, specifically in NavigationView's layout measurement.
- Files: `src/Magidesk.Presentation/MainWindow.xaml.cs:91-110`
- Trigger: Toggling `PaneDisplayMode`, `IsPaneVisible`, or `IsPaneToggleButtonVisible` during active navigation/layout transitions.
- Workaround: These properties are commented out and always kept in a fixed state. Navigation pane is always visible and in Left mode.
- Status: Root cause identified. Pane visibility toggles disabled as band-aid. Need to decouple property changes from navigation lifecycle or use different navigation pattern.

**Cash Balance Timer Update Race Condition:**
- Symptoms: Occasional "No Session" errors displayed when terminal has active session, or stale balance shown after transactions.
- Files: `src/Magidesk.Presentation/MainWindow.xaml.cs:62-64` (30-second timer), and `src/Magidesk.Infrastructure/Services/CashBalanceTrackingService.cs:33-68` (30-second cache TTL).
- Trigger: High-frequency transactions (multiple orders/payments) between timer ticks. Cache returns stale data if refresh hasn't completed.
- Workaround: Use TryEnqueue to prevent UI thread blocking. Explicit null checks on UI updates.
- Improvement path: Implement event-driven updates instead of polling. `ICashBalanceTrackingService` should raise `CashBalanceUpdated` event immediately after any transaction, not wait 30 seconds.

**Incomplete Order Line Status Localization:**
- Symptoms: Order line status strings are shown in English regardless of application language setting.
- Files: `src/Magidesk.Application/DTOs/OrderLineDto.cs:34` (TODO comment)
- Trigger: Using `status.ToString()` directly in DTO instead of resolving localized text via service.
- Fix approach: Move status text localization to ViewModel layer. Inject `ILocalizationService` and map status enums to localized strings at display time.

## Security Considerations

**Card Number Exposure in Manual Auth:**
- Risk: `AuthorizeCardPaymentCommand` receives full card number as plain text parameter. Handler extracts last 4 digits but full number could be logged or persisted in error messages.
- Files: `src/Magidesk.Application/Services/AuthorizeCardPaymentCommandHandler.cs`, `src/Magidesk.Application/Commands/AuthorizeCardPaymentCommand.cs`
- Current mitigation: Handler extracts last 4 digits before storing in result. Full number is not persisted to database.
- Recommendations:
  - Never pass full card numbers in command parameters. Only accept tokenized card references from PCI-compliant gateway.
  - Add SensitiveData attribute to mask card number in logs.
  - Implement cardholder data flow audit to track where raw card data is used.

**Terminal ID Null Handling:**
- Risk: `HttpTerminalContext.TerminalId` can return null if header is missing. Null checks in handlers may be inconsistent, leading to null reference exceptions or default Guid usage.
- Files: `src/Magidesk.Api/Infrastructure/HttpTerminalContext.cs:40-43`
- Current mitigation: Handler validates and returns appropriate error. But some controller actions pass `Guid.Empty` as fallback.
- Recommendations:
  - Require terminal ID on all API requests. Return 400 if missing.
  - Never silently default to Guid.Empty. This hides routing/configuration errors.
  - Implement middleware to validate terminal ID presence before handler execution.

**Manual Manager Override Without Session Audit:**
- Risk: Manager override commands allow bypassing normal business rules (e.g., discount limits, payment authorization) but audit trail may not capture the override reason or duration.
- Files: `src/Magidesk.Application/Interfaces/IUserService.cs` (ManagerOverride methods)
- Current mitigation: Audit events logged when override is applied.
- Recommendations:
  - Add `OverrideReason` field to audit event. Require manager to provide justification.
  - Implement time-limited overrides (e.g., override expires after 1 hour).
  - Log override attempts, not just successes. Track who attempted unauthorized actions.

## Performance Bottlenecks

**30-Second Polling for Cash Balance Updates:**
- Problem: MainWindow refreshes cash balance on 30-second timer. If user makes payment, balance doesn't update for up to 30 seconds.
- Files: `src/Magidesk.Presentation/MainWindow.xaml.cs:61-64`
- Cause: Polling interval is too long for user-facing updates. Cache TTL is also 30 seconds, leading to stale reads.
- Improvement path:
  - Switch to event-driven model. `ICashBalanceTrackingService` publishes `CashBalanceUpdated` event.
  - MainWindow subscribes to event and updates immediately.
  - Keep polling as fallback (e.g., 5-minute refresh) for safety.

**DTO Mapping in KitchenRoutingService:**
- Problem: `MapToDto()` method manually constructs TicketDto from Ticket entity. Inefficient for large tickets with many order lines and modifiers.
- Files: `src/Magidesk.Application/Services/KitchenRoutingService.cs:154-187`
- Cause: No mapper framework (AutoMapper). Manual LINQ-to-object construction with nested iterations.
- Improvement path:
  - Use AutoMapper or similar to reduce boilerplate.
  - Cache TicketDto if already loaded in application flow.
  - Consider breaking routing logic into separate service to avoid redundant mapping.

**No Caching of Menu Items in OrderPageViewModel:**
- Problem: `GetMenuItemsQuery` may hit database on every navigation to OrderPage if items aren't cached.
- Files: `src/Magidesk.Presentation/ViewModels/OrderPageViewModel.cs:54` (stores `_allProducts` but not clear when/how it's populated)
- Cause: ViewModel loads products but caching strategy not obvious. No explicit cache invalidation on menu changes.
- Improvement path:
  - Use `ReportCacheService` or similar to cache menu items with TTL.
  - Publish cache invalidation events when menu is modified via SystemConfig.
  - Lazy-load categories on demand rather than all products at once.

## Fragile Areas

**TableDesignerPage with Multiple Unimplemented Interactions:**
- Files: `src/Magidesk.Presentation/Views/TableDesignerPage.xaml.cs`
- Why fragile: Multiple TODO comments for critical UX features (lasso selection, undo/redo, drag validation, keyboard shortcuts). Incomplete implementation means:
  - Visual feedback missing (line 178: "TODO: Show visual lasso rectangle overlay")
  - State management incomplete (line 287: "TODO: Record undo action")
  - Validation gaps (line 290: "TODO: Validate new position")
  - User can trigger partial operations that leave inconsistent state
- Safe modification:
  - Add feature flags to disable incomplete features.
  - Write integration tests for drag/drop workflows before implementing.
  - Use finite state machine to track table designer mode (idle, dragging, selecting, resizing).

**OrderPageViewModel.TicketOperations with Missing Dialog Logic:**
- Files: `src/Magidesk.Presentation/ViewModels/OrderPageViewModel.TicketOperations.cs:79-131`
- Why fragile: Split, merge, and note operations have TODO comments indicating dialogs not implemented:
  - Line 79: "TODO: Implement split order dialog and logic"
  - Line 105: "TODO: Implement merge order dialog and logic"
  - Line 131: "TODO: Implement add note dialog"
- Issue: Commands are exposed to UI but return no feedback. User clicks, nothing visible happens.
- Safe modification:
  - Implement IDialogService integration for all three operations.
  - Add loading indicators and error handling.
  - Write mock-based tests for dialog interaction before connecting real dialog service.

**RealTimeSessionMonitoringViewModel.AlertManagement Missing Error Dialog:**
- Files: `src/Magidesk.Presentation/ViewModels/RealTimeSessionMonitoringViewModel.cs:339` (clear alerts), `src/Magidesk.Presentation/Views/RealTimeSessionMonitoringPage.xaml.cs:79` (TODO: Show error dialog)
- Why fragile: Error handling is incomplete. Hub disconnections, load failures, or command rejections don't show user feedback.
- Safe modification:
  - Implement comprehensive error handling with user-visible notifications.
  - Test SignalR reconnection scenarios.
  - Use IDialogService or IToastNotificationService (both already in DI).

**EquipmentCommandHandlers with No-Op Implementations:**
- Files: `src/Magidesk.Application/Commands/Equipment/AssignEquipmentCommandHandler.cs:39`, `src/Magidesk.Application/Commands/Equipment/ScheduleMaintenanceCommandHandler.cs:37`
- Why fragile: TODO comments indicate these are stubs awaiting `IEquipmentService`.
- Issue: Equipment commands are wired in API but do nothing. No validation. No audit trail.
- Safe modification:
  - Don't wire equipment endpoints to API until service is implemented.
  - Add feature flag to disable equipment operations in Presentation.
  - Implement `IEquipmentService` interface and wire both handlers properly.

## Scaling Limits

**In-Memory Caching of CashBalance and Monitoring Timers:**
- Current capacity: `ConcurrentDictionary<Guid, CashBalanceDto>` and `ConcurrentDictionary<Guid, Timer>` have no size limits.
- Limit: If more than ~1000 terminals are active, memory usage grows unbounded. Timers are never disposed if monitoring request is lost.
- Scaling path:
  - Implement max-size LRU cache with eviction policy.
  - Automatic cleanup of monitoring timers after inactivity (e.g., 1 hour).
  - Use distributed cache (Redis) for multi-server deployments.

**Fire-and-Forget UpdateCashBalanceAsync in MainWindow:**
- Current capacity: One fire-and-forget task per cash balance update (line 67: `_ = UpdateCashBalanceAsync()`).
- Limit: Rapid navigation or multiple MainWindow instances could spawn many untracked tasks. No way to cancel pending updates.
- Scaling path:
  - Use `CancellationTokenSource` with timeout per task.
  - Queue updates instead of spawning parallel tasks.
  - Implement max concurrent updates (e.g., only 1 update at a time per terminal).

**No Pagination in GetActiveSessionsQuery:**
- Current capacity: Query loads ALL active sessions into memory as `ObservableCollection<TableSessionDto>`.
- Limit: Restaurants with 50+ tables will experience UI lag when loading session list. Memory usage grows linearly with session count.
- Scaling path:
  - Implement pagination with page size configurable per terminal.
  - Lazy-load session details (only load top-level summary initially).
  - Use data virtualization in WinUI to render only visible rows.

## Dependencies at Risk

**GitHub Update Service with No Fallback:**
- Risk: `GithubUpdateService` (newfiles: `src/Magidesk.Infrastructure/Services/GithubUpdateService.cs`) fetches release info from GitHub API. If GitHub is unavailable, update checks fail silently.
- Impact: Users won't know updates are available. Cannot patch security issues until next manual restart.
- Migration plan:
  - Cache last-known release info locally.
  - Implement retry logic with exponential backoff.
  - Allow manual update check trigger via UI.
  - (Future) Host update metadata on private server as fallback.

**CommunityToolkit.Mvvm with Hard Dependency on Attributes:**
- Risk: MVVM Toolkit uses source generators (attributes). If NuGet package is removed/updated incompatibly, all ViewModels break.
- Impact: Code generation fails at compile time.
- Migration plan:
  - Code is committed to git, so regeneration on clean build works.
  - Monitor toolkit updates carefully before upgrading.
  - Have manual INotifyPropertyChanged implementation as fallback pattern.

## Missing Critical Features

**Notification Center Not Implemented:**
- Problem: Multiple services have TODOs referencing "Notification Center" that doesn't exist.
- Files: `src/Magidesk.Application/Services/LowStockAlertService.cs:31`, multiple others
- Blocks: Low stock alerts, system notifications, error notifications all can't be displayed to user in a unified way.
- Impact: Operators miss critical events (low stock, equipment failures, permission denials).

**Order Type Seeding Incomplete:**
- Problem: `DefaultViewRoutingService` workaround exists because OrderTypes aren't properly seeded in database.
- Files: `src/Magidesk.Presentations/Services/DefaultViewRoutingService.cs:51-52` (TODO comment)
- Blocks: Terminal-specific routing, feature flags based on order type.
- Impact: All users see same default page regardless of role/terminal type.

**Offline Mode Queue Not Implemented:**
- Problem: E2E tests have TODOs for offline mode (error handling, queue sync on reconnection).
- Files: `src/Magidesk.Tests.E2E/Tests/P2_Stability/ErrorHandlingTests.cs:31-48`
- Blocks: Application can't operate when backend is unavailable. No graceful degradation.
- Impact: Single backend outage = entire POS system down.

## Test Coverage Gaps

**Manual Auth Code Bypass Untested:**
- What's not tested: Authorization flow with manual auth codes. No tests verify audit trail or manager override checks.
- Files: `src/Magidesk.Application/Services/AuthorizeCardPaymentCommandHandler.cs`
- Risk: Compliance audit could fail. Manual auth bypasses could be created without audit trail.
- Priority: High

**Navigation Stability Not Covered:**
- What's not tested: Rapid navigation between pages. MeasureOverride crash may only reproduce under specific load/timing conditions.
- Files: `src/Magidesk.Presentation/MainWindow.xaml.cs`
- Risk: Users encounter crashes in production unpredictably.
- Priority: High

**Table Designer Incomplete Workflows:**
- What's not tested: Lasso selection, undo/redo, keyboard shortcuts in table designer.
- Files: `src/Magidesk.Presentation/Views/TableDesignerPage.xaml.cs`
- Risk: Missing features silently fail. Users can't undo mistakes.
- Priority: Medium

**OfflineMode and Reconnection Scenarios:**
- What's not tested: Any offline scenarios. Database unavailability, network disconnection, partial sync failures.
- Files: Nowhere implemented
- Risk: Application untested for production failures.
- Priority: High

**Multi-Terminal Concurrency:**
- What's not tested: Concurrent terminal updates to same table/ticket. No tests for optimistic concurrency tokens (RowVersion).
- Files: No concurrency tests found
- Risk: Data corruption or lost updates in multi-register environments.
- Priority: High

---

*Concerns audit: 2026-03-23*

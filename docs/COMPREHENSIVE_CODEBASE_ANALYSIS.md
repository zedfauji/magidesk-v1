# Magidesk POS — Comprehensive Codebase Analysis
**Date:** February 9, 2026  
**Analyst:** Kiro Deep Audit  
**Build Status:** Main app compiles (0 errors, 639 warnings). Test projects: 25 errors.

---

## EXECUTIVE SUMMARY

Magidesk is a WinUI3/.NET 8 POS system with a Clean Architecture foundation (Domain → Application → Infrastructure → Presentation) that is architecturally sound in its skeleton but suffering from severe model drift caused by vibe coding across multiple AI sessions. The domain layer is the strongest part of the codebase. The application compiles and runs, but carries ~640 warnings, duplicate implementations of core features (order entry, settlement), split namespaces from different AI models, duplicate DI registrations, and 25+ broken test files. The project is roughly 60-65% production-ready — not the 85% claimed in PROJECT_STATUS.md.

**Core problem:** No single AI session had full context of the codebase. Each session generated code in slightly different patterns (namespaces, DI registration locations, handler patterns), creating a codebase that compiles but has internal contradictions that cause cascading instability when any one area is modified.

**Path forward:** Lock the domain layer, consolidate the duplicate implementations behind the feature flag system already in place, fix the DI registration mess by centralizing everything, unify namespaces, and fix the test projects. This is a 3-4 week stabilization effort, not a rewrite.

---

## PHASE 1: DETAILED INVENTORY

### 1.1 Solution Structure (10 Projects)

| Project | Purpose | Status |
|---------|---------|--------|
| `Magidesk.Domain` | Entities, Value Objects, Domain Services, Enumerations | ✅ Solid — best part of codebase |
| `Magidesk.Application` | Commands, Queries, Handlers, DTOs, Interfaces | ✅ Functional, some duplicate registrations |
| `Magidesk.Infrastructure` | EF Core, Repositories, Services, Printing, Security | ✅ Functional, duplicate registrations |
| `Magidesk.Presentation` | WinUI3 UI, ViewModels, Views, Services | ⚠️ Compiles but has major issues |
| `Magidesk.Api` | REST API (if needed) | ❓ Not audited in depth |
| `Magidesk.Migrations` | Newer EF Core migrations (Jan 2026+) | ⚠️ Split from Infrastructure migrations |
| `Magidesk.Application.Tests` | Unit tests for Application layer | ❌ 21 compilation errors |
| `Magidesk.Infrastructure.Tests` | Unit tests for Infrastructure layer | ❌ 4 compilation errors |
| `Magidesk.Tests.Workflows` | Integration/workflow tests | ⚠️ Compiles with warnings |
| `Magidesk.Domain.Tests` | Domain layer tests | ✅ Compiles |

### 1.2 Domain Entities (64 entities)

The domain model is comprehensive for a POS system:
- **Core POS:** Ticket, OrderLine, OrderLineModifier, OrderLineDiscount, Payment (TPH: Cash, CreditCard, DebitCard, GiftCertificate, Custom), OrderType
- **Menu:** MenuItem, MenuCategory, MenuGroup, MenuModifier, ModifierGroup, MenuItemModifierGroup, ComboDefinition, ComboGroup, ComboGroupItem, FractionalModifier
- **Financial:** CashSession, CashDrop, DrawerBleed, Payout, Discount, TicketDiscount, Gratuity, PaymentBatch, PromotionSchedule
- **Table/Session:** Table, TableLayout, TableShape, TableType, TableSession, Floor, ServerSection, ServerAssignment
- **Users:** User, Role, AttendanceHistory
- **Kitchen:** KitchenOrder, KitchenOrderItem
- **Inventory:** InventoryItem, InventoryAdjustment, StockMovement, Vendor, PurchaseOrder
- **Membership:** Customer, Member, MembershipTier, MembershipStatus
- **System:** Terminal, TerminalTransaction, RestaurantConfiguration, PrinterGroup, PrinterMapping, PrintTemplate, AuditEvent, Equipment, GameHistory, GroupSettlement
- **Infrastructure:** OverrideAudit (entity), PerformanceMetric (entity), Alert (entity), SessionAudit (entity)

**Assessment:** This is a rich domain model. 63 EF Core configurations match the entities. The entity count is appropriate for a full-featured POS.

### 1.3 CRITICAL FINDING: Duplicate Implementations

#### 1.3.1 Order Entry — TWO competing implementations

| File | Lines | Namespace | Used By |
|------|-------|-----------|---------|
| `OrderEntryViewModel.cs` | 2,031 | `Magidesk.Presentation.ViewModels` | `OrderEntryPage.xaml` |
| `OrderPageViewModel.cs` | 2,284 | `Magidesk.Presentation.ViewModels` | `OrderPageView.xaml` |

**Switching mechanism:** `OrderPageNavigationHelper` + `IFeatureFlagService.UseRedesignedOrderPages` (defaults to `true`).  
**Active implementation:** `OrderPageViewModel` (the "redesigned" version) is the default.  
**Risk:** Both are registered in DI. Both consume the same command handlers. The old `OrderEntryViewModel` is dead code in production but still maintained.

#### 1.3.2 Settlement — TWO competing implementations

| File | Lines | Namespace | Used By |
|------|-------|-----------|---------|
| `SettleViewModel.cs` | 1,060 | `Magidesk.Presentation.ViewModels` | `SettlePage.xaml` |
| `SettlePageViewModel.cs` | 916 | `Magidesk.Presentation.ViewModels` | `SettlePageView.xaml` |

**Switching mechanism:** Same `OrderPageNavigationHelper.GetSettlePageType()` feature flag.  
**Active implementation:** `SettlePageViewModel` is the default.  
**Extra issue:** `SettleViewModel.cs` has DUPLICATE `using` statements (lines 1-9 are repeated verbatim on lines 10-17). Classic AI generation artifact.

#### 1.3.3 SplitTicketDialog — TWO XAML files

- `SplitTicketDialog.xaml` + `SplitTicketDialog.xaml.cs`
- `SplitTicketDialog_Fixed.xaml` (declares `x:Class="Magidesk.Presentation.Views.SplitTicketDialog"` — same class!)

The `_Fixed` file is a vibe coding artifact where an AI generated a "fixed" version but the original was never deleted.

### 1.4 CRITICAL FINDING: Namespace Schizophrenia

Two namespace conventions exist side-by-side, generated by different AI models:

**Convention A (correct):** `Magidesk.Presentation.ViewModels`, `Magidesk.Presentation.Views`  
**Convention B (incorrect):** `Magidesk.ViewModels`, `Magidesk.Views`, `Magidesk.Views.Dialogs`

**Files using wrong namespace (Convention B):**

ViewModels in `Magidesk.ViewModels`:
- `GratuitySelectionViewModel.cs`
- `MiscItemViewModel.cs`
- `TicketFeeViewModel.cs`

ViewModels in `Magidesk.ViewModels.Dialogs`:
- `AddOnSelectionViewModel.cs`
- `ComboSelectionViewModel.cs`
- `ModifierGroupViewModel.cs`
- `ModifierSelectionViewModel.cs` (the dialog one)
- `ModifierItemViewModel.cs`

Views in `Magidesk.Views.Dialogs`:
- `AddOnSelectionDialog.xaml.cs`
- `ComboSelectionDialog.xaml.cs`
- `CookingInstructionDialog.xaml.cs`
- `CustomerSelectionDialog.xaml.cs`
- `GratuitySelectionDialog.xaml.cs`
- `ItemSearchDialog.xaml.cs`
- `MergeTicketsDialog.xaml.cs`
- `MiscItemDialog.xaml.cs`
- `ModifierSelectionDialog.xaml.cs`
- `PizzaModifierDialog.xaml.cs`
- `PriceEntryDialog.xaml.cs`
- `SeatSelectionDialog.xaml.cs`
- `SizeSelectionDialog.xaml.cs`
- `TableSelectionDialog.xaml.cs`
- `TicketFeeDialog.xaml.cs`

Views in `Magidesk.Views`:
- `AuthorizationCaptureBatchDialog.xaml.cs`
- `AuthorizationCodeDialog.xaml.cs`
- `PaymentProcessWaitDialog.xaml.cs`
- `QuantityDialog.xaml.cs`
- `RefundWizardDialog.xaml.cs`
- `SwipeCardDialog.xaml.cs`

**Impact:** ~25 files use the wrong namespace. This works because they're all in the same assembly, but it creates confusion and makes refactoring dangerous.

### 1.5 CRITICAL FINDING: DI Registration Chaos

Services are registered in THREE places:

1. **`Application/DependencyInjection/ServiceCollectionExtensions.cs`** — called via `services.AddApplication()`
2. **`Infrastructure/DependencyInjection/ServiceCollectionExtensions.cs`** — called via `services.AddInfrastructure()`
3. **`App.xaml.cs`** — direct registrations in the constructor

**Confirmed duplicate registrations (registered in multiple places):**

| Service | App.xaml.cs | Application DI | Infrastructure DI |
|---------|-------------|----------------|-------------------|
| `ITableRepository` | ✅ Transient | — | ✅ Scoped |
| `ITableLayoutRepository` | ✅ Transient | — | ✅ Scoped |
| `IFloorRepository` | ✅ Transient | — | ✅ Scoped |
| `CloseCashSessionCommand` handler | ✅ Transient | ✅ Scoped | — |
| `GroupSettleCommand` handler | ✅ Transient | — | — |
| `SplitBySeatCommand` handler | ✅ Transient | — | — |
| `ChangeTableCommand` handler | ✅ Transient | ✅ Scoped | — |
| `CreateUserCommand` handler | ✅ Transient | ✅ Scoped | — |
| `UpdateUserCommand` handler | ✅ Transient | ✅ Scoped | — |
| `DeleteUserCommand` handler | ✅ Transient | ✅ Scoped | — |
| `IGroupSettleService` | — | ✅ Scoped | ✅ Scoped |
| `IMerchantBatchService` | — | ✅ Scoped | ✅ Scoped |
| `IRawPrintService` | ✅ Singleton | — | ✅ Scoped |
| `IKitchenPrintService` | ✅ Scoped | — | ✅ Scoped |
| `IReceiptPrintService` | ✅ Scoped | — | ✅ Scoped |
| `ICashDrawerService` | ✅ Singleton | — | ✅ Scoped |
| `TableDesignerViewModel` | ✅ Transient (×2!) | — | — |
| `BatchPaymentDomainService` | ✅ Transient | — | — |
| `IEventPublisher` | ✅ Singleton | — | — |

**Lifetime conflicts:**
- `IRawPrintService`: Singleton in App.xaml.cs vs Scoped in Infrastructure DI
- `ICashDrawerService`: Singleton in App.xaml.cs vs Scoped in Infrastructure DI
- `ITableRepository`: Transient in App.xaml.cs vs Scoped in Infrastructure DI

The last registration wins in .NET DI, so App.xaml.cs overrides the Infrastructure registrations. This means repositories that should be Scoped (tied to a DbContext scope) are actually Transient, which can cause DbContext tracking issues.

### 1.6 Migration Split

Two migration locations with different `ApplicationDbContextModelSnapshot.cs` files:

- **`Magidesk.Infrastructure/Migrations/`**: 14 migrations (Dec 25, 2025 → Jan 2, 2026)
- **`Magidesk.Migrations/Migrations/`**: 13 migrations (Jan 4, 2026 → Jan 29, 2026)

This is a ticking time bomb. If both projects try to apply migrations, they'll conflict. Only one should own the migration history.

### 1.7 Hardcoded Values

| Location | Issue | Severity |
|----------|-------|----------|
| `DatabaseConnection.cs` | Hardcoded `postgres/postgres` credentials | 🔴 Critical for production |
| `OrderPageViewModel.cs:181` | `TaxRate => 0.08m` with TODO comment | 🟡 Medium |
| `TableMapViewModel.cs:253` | Hardcoded `UserId = Guid.Parse("00000000-...")` | 🔴 Critical |
| `TableMapViewModel.cs:908,1070` | Hardcoded guest count `4` | 🟡 Medium |
| `TableSessionViewModel.cs:279` | Empty manager PIN placeholder | 🔴 Critical |
| `MockPaymentGateway` | All card processing is simulated | 🟡 Expected for dev |

### 1.8 Known UI Crash

`MainWindow.xaml.cs` lines 95-115: NavigationView `IsPaneVisible` and `IsPaneToggleButtonVisible` toggles are commented out with the note:
> "root cause of the persistent E_INVALIDARG (0xc000027b) MeasureOverride crash"

The workaround is to keep the pane always in Left mode with `IsPaneOpen = false`. This is a known WinUI3 bug — the workaround is correct but the pane visibility feature is lost.

### 1.9 Build Status

**Main application (6 projects):** ✅ Compiles with 0 errors, 639 warnings
- Most warnings are MVVMTK0045 (AOT compatibility) — not blocking but should be addressed
- Some MVVMTK0034 (direct field reference instead of generated property) — code smell

**Test projects (4 projects):** ❌ 25 compilation errors
- `Magidesk.Application.Tests`: 21 errors — constructor signature mismatches, missing `ILogger` parameters, stale property references
- `Magidesk.Infrastructure.Tests`: 4 errors — missing `UseInMemoryDatabase` package, stale enum values
- Root cause: Tests were written against older versions of handlers/commands that have since been modified

### 1.10 Views Inventory

**Total XAML pages:** 57 pages + 42 dialogs + 3 components = 102 XAML files  
**Orphaned/suspicious files:**
- `SplitTicketDialog_Fixed.xaml` — duplicate of `SplitTicketDialog.xaml`
- `TableDesignerTestPage.xaml` — test page, should not ship
- `MainPage.xaml` — appears unused (navigation goes Login → Switchboard, not MainPage)

---

## PHASE 2: BASELINE ESTABLISHMENT

### 2.1 What Actually Works (Lock These Down)

**Domain Layer — LOCK ENTIRELY:**
- 64 entities with proper encapsulation
- Ticket aggregate root with domain events
- Money value object
- Domain services (Tax, Payment, Discount, ServiceCharge, PriceCalculator, Gratuity)
- Enumerations are comprehensive

**Infrastructure — Mostly Stable:**
- 63 EF Core entity configurations
- Repository implementations (all follow consistent pattern)
- PostgreSQL via Npgsql — working
- Database setup/seeding flow — working
- Security services (AES encryption, SecurityService)
- Printing infrastructure (ESC/POS, layout engines, template engine)

**Application Layer — Mostly Stable:**
- CQRS command/query handlers — comprehensive set
- MediatR integration — working
- DTOs — well-structured

**Presentation — Partially Stable:**
- Login flow → Switchboard → Order Entry → Settlement — the happy path works
- NavigationService with auth guard — solid
- Feature flag system for old/new UI — clever and working
- Localization service — implemented
- Database setup page — working
- Error handling (global exception handlers, error banners) — thorough

### 2.2 What's Partially Implemented (50-80%)

| Feature | Status | What's Missing |
|---------|--------|----------------|
| Order Entry (new) | 75% | Hardcoded tax rate, some dialog flows incomplete |
| Settlement (new) | 70% | Manager PIN flow needs work, gratuity integration |
| Table Map | 65% | Server assignment is TODO, hardcoded user IDs, error dialogs missing |
| Table Sessions | 60% | Pause/resume works, but manager override is placeholder |
| Kitchen Display | 60% | SignalR initialization exists but may not connect, polling fallback works |
| Back Office | 70% | Navigation works, individual pages vary in completeness |
| Reports | 65% | 15+ report types exist but not all tested end-to-end |
| Inventory | 55% | CRUD exists, stock tracking added, but low-stock alerts are basic |
| Customer/Membership | 50% | Entities and basic CRUD exist, check-in flow started |
| Print Templates | 60% | Liquid template engine works, but template editor preview needs work |

### 2.3 What's Broken/Contradictory

1. **Duplicate order entry implementations** — both registered, both consuming resources
2. **Duplicate settlement implementations** — same issue
3. **Namespace split** — ~25 files in wrong namespace
4. **DI registration chaos** — 3 registration points with lifetime conflicts
5. **Test projects** — 25 compilation errors, tests are stale
6. **Migration split** — two migration projects with separate snapshots
7. **SettleViewModel duplicate usings** — lines 1-9 repeated on 10-17
8. **SplitTicketDialog_Fixed.xaml** — orphaned duplicate file

### 2.4 What's Missing Entirely

| Feature | Priority for Bar Deployment |
|---------|---------------------------|
| Real payment gateway integration | 🔴 Critical |
| Production database configuration (not hardcoded) | 🔴 Critical |
| Backup/restore tested end-to-end | 🟡 High |
| Multi-currency support | 🟢 Low (single-venue) |
| Pizza builder (half-and-half) | 🟡 Medium |
| Delivery dispatch | 🟢 Low (bar doesn't deliver) |
| Reservation system | 🟢 Low |
| Offline mode / sync | 🟡 High for reliability |
| End-to-end integration tests | 🔴 Critical |
| Performance testing under load | 🟡 High |
| Installer / deployment package | 🔴 Critical |

---

## PHASE 3: ROOT CAUSE ANALYSIS

### 3.1 Why This Project Keeps Destabilizing

**Root Cause #1: No Single Source of Truth for DI**

Every AI session that added a new feature also added DI registrations — sometimes in `App.xaml.cs`, sometimes in the proper extension methods. No session had visibility into what was already registered. Result: triple-registered services with conflicting lifetimes.

**Root Cause #2: Namespace Drift from Model Switching**

At least two different AI models generated code. One used `Magidesk.Presentation.ViewModels` (correct), the other used `Magidesk.ViewModels` (shorter, also valid since it's the same assembly). Neither model knew about the other's convention. The code compiles because C# doesn't care about namespace-to-folder alignment, but it creates a maintenance nightmare.

**Root Cause #3: "Redesign" Pattern Instead of "Fix" Pattern**

When the order entry or settlement had issues, the AI was asked to "redesign" it rather than fix the existing implementation. This created parallel implementations (`OrderEntryViewModel` → `OrderPageViewModel`, `SettleViewModel` → `SettlePageViewModel`) instead of fixing the originals. The feature flag system was added to manage this, which is smart, but the old code was never removed.

**Root Cause #4: Tests Written Against Moving Targets**

Tests were generated against specific handler signatures. When handlers were later modified (e.g., adding `ILogger` parameter), the tests weren't updated. This is the classic vibe coding failure mode — the AI generates tests for the current state, but the next session changes the production code without updating tests.

**Root Cause #5: No Build Verification Between Sessions**

The 639 warnings accumulated because no session was asked to "fix all warnings." Each session added a few more. The test compilation errors went unnoticed because the main app still compiled.

### 3.2 Architectural Assessment

**The architecture is NOT fundamentally flawed.** Clean Architecture with CQRS is the right choice for a POS system. The domain layer is well-designed. The problem is execution consistency, not architectural choice.

**What's structurally wrong:**
- `App.xaml.cs` at 578 lines is too large — DI should be fully delegated to extension methods
- Mixed MediatR + custom `ICommandHandler<T>` pattern — should pick one
- `DbContextFactory.CreateDbContext()` uses `.GetAwaiter().GetResult()` — sync-over-async

**What's structurally right:**
- Aggregate root pattern (Ticket)
- Value objects (Money, UserId)
- Domain services for business logic
- Repository pattern with EF Core
- Feature flag system for UI migration
- Auth guard on navigation
- Global exception handling with crash logging

---

## PHASE 4: STRATEGIC ROADMAP

### 4.1 Stabilization Strategy

**LOCK (never regenerate):**
- `Magidesk.Domain/` — entire project
- `Magidesk.Infrastructure/Data/Configurations/` — all 63 entity configs
- `Magidesk.Infrastructure/Repositories/` — all repository implementations
- `Magidesk.Application/Commands/` and `Magidesk.Application/Queries/` — all handlers
- `NavigationService.cs`, `OrderPageNavigationHelper.cs`, `FeatureFlagService.cs`

**REFACTOR (fix once, properly):**
- `App.xaml.cs` — extract ALL DI to extension methods, remove duplicates
- Namespace unification — move all `Magidesk.ViewModels` → `Magidesk.Presentation.ViewModels`
- `Infrastructure/DependencyInjection/ServiceCollectionExtensions.cs` — remove duplicates with Application DI
- Migration consolidation — pick one project, delete the other's migrations
- Fix all 25 test compilation errors
- Remove `SplitTicketDialog_Fixed.xaml`
- Remove `TableDesignerTestPage.xaml`
- Fix `SettleViewModel.cs` duplicate usings

**DISCARD (remove entirely):**
- `OrderEntryViewModel.cs` + `OrderEntryPage.xaml` — the old implementation. The redesigned `OrderPageViewModel` is the default and more complete.
- `SettleViewModel.cs` + `SettlePage.xaml` — same reasoning. `SettlePageViewModel` is better.
- After discarding, remove the feature flag switching and hardcode to the new pages.
- `DatabaseConnection.cs` hardcoded connection string — replace with configuration-based approach (already partially done via `IDatabaseConfigurationService`)

### 4.2 Tool Recommendations

**Use Kiro for:**
- Refactoring tasks (namespace unification, DI consolidation) — Kiro's semantic rename and multi-file editing are ideal
- Bug fixes in specific files — Kiro maintains context within a session
- Test fixes — Kiro can read the handler signatures and update tests to match
- Code review and analysis (like this audit)

**Use Antigravity for:**
- Large feature implementation that spans many files (new payment gateway integration)
- UI page creation (XAML + ViewModel + code-behind in one shot)
- When you need to generate boilerplate across multiple layers

**General rules:**
- NEVER let any AI regenerate domain entities or EF configurations
- ALWAYS provide the DI registration file as context when adding new services
- ALWAYS run `dotnet build` after every AI session
- Keep sessions focused — one feature per session, not "fix everything"

### 4.3 Guardrails

Create a `.kiro/steering/architecture.md` with:
- Namespace convention: always `Magidesk.Presentation.ViewModels`, never `Magidesk.ViewModels`
- DI registration: ALL registrations go in the appropriate `ServiceCollectionExtensions.cs`, NEVER in `App.xaml.cs`
- Lifetime rules: Repositories = Scoped, ViewModels = Transient, Navigation/User services = Singleton
- Test convention: always include `ILogger<T>` mock in handler tests
- NEVER create `_Fixed` variants of files — fix in place

### 4.4 Implementation Sequence

**Week 1: DI & Namespace Consolidation**
1. Move all DI registrations from `App.xaml.cs` to proper extension methods
2. Remove duplicate registrations between Application and Infrastructure DI
3. Fix lifetime conflicts (repositories must be Scoped)
4. Unify all namespaces to `Magidesk.Presentation.*`

**Week 2: Dead Code Removal & Test Fixes**
1. Remove old `OrderEntryViewModel` + `OrderEntryPage` + feature flag switching
2. Remove old `SettleViewModel` + `SettlePage`
3. Delete `SplitTicketDialog_Fixed.xaml`, `TableDesignerTestPage.xaml`
4. Fix all 25 test compilation errors
5. Consolidate migrations to one project

**Week 3: Hardcoded Values & Missing Implementations**
1. Replace hardcoded tax rate with configuration lookup
2. Replace hardcoded user IDs in TableMapViewModel
3. Implement manager PIN dialog properly in TableSessionViewModel
4. Add `UseInMemoryDatabase` package to Infrastructure.Tests
5. Address the top 50 most impactful warnings

**Week 4: Integration & Deployment Prep**
1. End-to-end test: Login → New Ticket → Add Items → Settle → Close
2. Production database configuration (remove hardcoded credentials)
3. Create deployment package / installer
4. Payment gateway integration planning

---

## PHASE 5: IMMEDIATE NEXT STEPS

### This Week — Top 3 Actions

**Action 1: Consolidate DI Registrations (2-3 hours)**

Move everything from `App.xaml.cs` ConfigureServices into a new `Presentation/DependencyInjection/ServiceCollectionExtensions.cs`. Remove duplicates. Fix lifetime conflicts. `App.xaml.cs` should only call:
```csharp
services.AddApplication();
services.AddInfrastructure();
services.AddPresentation(); // NEW — all ViewModels, UI services, dialog services
```

**Action 2: Unify Namespaces (1-2 hours)**

Use Kiro's semantic rename to move all `Magidesk.ViewModels` → `Magidesk.Presentation.ViewModels` and `Magidesk.Views` → `Magidesk.Presentation.Views`. This affects ~25 files.

**Action 3: Remove Dead Implementations (1 hour)**

Delete `OrderEntryViewModel.cs`, `OrderEntryPage.xaml`, `SettleViewModel.cs`, `SettlePage.xaml`, `SplitTicketDialog_Fixed.xaml`. Update `OrderPageNavigationHelper` to always return the new pages. Remove the feature flag.

### Files to Protect from Regeneration

Add these to a "do not modify" list in your steering file:
- All files in `Magidesk.Domain/`
- All files in `Magidesk.Infrastructure/Data/Configurations/`
- `NavigationService.cs`
- `MainWindow.xaml.cs` (the crash workaround is correct)
- `App.xaml.cs` (after consolidation)

### How to Structure Prompts to Avoid Hallucination

1. **Always provide the interface file** when asking AI to implement something
2. **Always provide the DI registration file** when adding new services
3. **State the namespace explicitly**: "Use namespace `Magidesk.Presentation.ViewModels`"
4. **State the lifetime explicitly**: "Register as Scoped"
5. **Never say "redesign"** — say "fix" or "modify"
6. **After every session**: run `dotnet build` and fix any new errors before the next session

---

## LONG-TERM COMPLETION PLAN

### Path to Deployable v1.0 (Bar/Restaurant)

**Month 1 (Weeks 1-4): Stabilization** — as described above

**Month 2 (Weeks 5-8): Core Feature Completion**
- Real payment gateway integration (Stripe Terminal or similar)
- Receipt printing end-to-end tested with actual thermal printer
- Kitchen display tested with actual KDS screen
- Cash session open/close/drawer pull fully tested
- All reports generating correct data

**Month 3 (Weeks 9-12): Hardening**
- Performance testing (100+ tickets/day simulation)
- Error recovery testing (network drops, printer offline, DB connection loss)
- User acceptance testing with actual bar staff
- Installer creation and deployment documentation
- Backup/restore verified

**Feature Priority for Bar Deployment:**
1. 🔴 Order entry → Settlement → Payment (cash + card) → Receipt print
2. 🔴 Cash session management (open, close, drawer pull)
3. 🔴 User login with PIN
4. 🟡 Table map (if table service)
5. 🟡 Kitchen display (if food service)
6. 🟡 Reports (sales summary, cash out)
7. 🟢 Inventory tracking
8. 🟢 Customer/membership
9. 🟢 Advanced features (combos, promotions, delivery)

**Realistic assessment:** With focused effort and disciplined AI-assisted development (one feature per session, build verification, no regeneration of stable code), v1.0 for bar deployment is achievable in 8-10 weeks from today.

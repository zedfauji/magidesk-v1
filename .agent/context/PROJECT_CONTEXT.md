# PROJECT_CONTEXT.md — Magidesk POS

**Last Updated:** 2026-03-03  
**Purpose:** Runtime context injected into every agent alongside their neutral role definition. This file contains all project-specific state. Update this file at the end of every sprint. The role definitions in AGENT_ROLES.md never need to change — only this file does.

---

## 1. Project Identity

- **Name:** Magidesk POS (v1)
- **Platform:** Windows, WinUI 3 / Windows App SDK 1.6+
- **Language:** C# 12 / .NET 8
- **Architecture:** Clean Architecture (Onion)
- **ORM:** EF Core 8 — Infrastructure layer only
- **Database:** PostgreSQL, local server, passwordless, DB name: `magidesk_pos`
- **MVVM Toolkit:** CommunityToolkit.Mvvm (ObservableObject, RelayCommand, ObservableProperty)
- **Command Pattern:** Custom `ICommandHandler<TCommand, TResult>` — not standard MediatR
- **Validation:** FluentValidation — Application layer only
- **DI:** Microsoft.Extensions.DependencyInjection

---

## 2. Solution Structure

| Project | Path | Type |
|---------|------|------|
| `Magidesk.Domain` | `src/Magidesk.Domain` | Domain |
| `Magidesk.Application` | `src/Magidesk.Application` | Application |
| `Magidesk.Infrastructure` | `src/Magidesk.Infrastructure` | Infrastructure |
| `Magidesk.Migrations` | `src/Magidesk.Migrations` | Migrations / Seeding |
| `Magidesk.Presentation` | `src/Magidesk.Presentation` | WinUI 3 Presentation |
| `Magidesk.Api` | `src/Magidesk.Api` | REST API (secondary) |
| `Magidesk.Domain.Tests` | `src/Magidesk.Domain.Tests` | Unit Tests |
| `Magidesk.Application.Tests` | `src/Magidesk.Application.Tests` | Application Tests |
| `Magidesk.Infrastructure.Tests` | `src/Magidesk.Infrastructure.Tests` | Infrastructure Tests |
| `Magidesk.Tests.E2E` | `src/Magidesk.Tests.E2E` | E2E Tests |
| `Magidesk.Tests.Workflows` | `src/Magidesk.Tests.Workflows` | Workflow Tests |

---

## 3. DI Registration Locations

| Layer | File | Method |
|-------|------|--------|
| Application | `src/Magidesk.Application/DependencyInjection/ServiceCollectionExtensions.cs` | `AddApplicationServices()` |
| Infrastructure | `src/Magidesk.Infrastructure/DependencyInjection/ServiceCollectionExtensions.cs` | `AddInfrastructureServices()` |
| Presentation | `src/Magidesk.Presentation/DependencyInjection/PresentationServiceExtensions.cs` | `AddPresentationServices()` |

**Rule:** Register new services in the appropriate layer extension only. Never register in `App.xaml.cs`.

---

## 4. Key Interfaces

| Interface | Location | Implementation | Lifetime |
|-----------|----------|----------------|----------|
| `IUserContextService` | `Application/Interfaces/IUserContextService.cs` | `UserService.cs` (Presentation) | Singleton |
| `IOrderNotificationService` | Application | `OrderNotificationService` | Scoped |
| `IKitchenRoutingService` | Application | Infrastructure impl | Scoped |
| `ICommandHandler<TCommand,TResult>` | Application | Per-handler | Scoped |

**Rule for all agents:** `IUserContextService.GetCurrentUserId()` is the only valid source of user identity. Never substitute a default/empty Guid.

---

## 5. Frozen Decisions

These cannot be changed without explicit owner approval. If a task requires violating any of these, output a BLOCKER.

| Decision | Detail |
|----------|--------|
| Clean Architecture | Strict inward dependency. No shortcuts. |
| No business logic in UI | ViewModels are coordinators only |
| No DB access from Presentation | All data access through Application layer |
| Immutable financial records | Tickets and Payments are immutable once finalized. Refunds create new transactions — never modify originals. |
| Single gratuity per ticket | Re-applying replaces the existing one. Totals recalculate from base. Never stack. |
| EF-managed concurrency tokens | Never manually mutate Version or RowVersion fields |
| Fail-loud printing | No silent print failures. Throw `PrintingContractViolationException` on contract violations. |
| PostgreSQL only | No SQLite, no in-memory for production paths |
| Custom ICommandHandler CQRS | Not standard MediatR. All handlers registered explicitly in DI. |
| EF Core in Infrastructure only | No EF in Domain or Application |

---

## 6. Project-Specific Code Rules

These extend the universal rules in AGENT_ROLES.md:

- **Max file size:** 300 lines per `.cs` file
- **Binding:** Use `x:Bind` (compiled) — never `{Binding}` (reflection)
- **Commands:** `[RelayCommand]` attribute — never manual `ICommand`
- **Properties:** `[ObservableProperty]` attribute
- **Namespaces:**
  - Domain → `Magidesk.Domain.*`
  - Application → `Magidesk.Application.*`
  - Infrastructure → `Magidesk.Infrastructure.*`
  - Presentation → `Magidesk.Presentation.*`
- **Money:** Always use the `Money` value object — never raw decimal for currency
- **Identity:** Always use `IUserContextService.GetCurrentUserId()` — never `Guid.Empty`
- **Kitchen routing:** Any new kitchen routing path must call `IOrderNotificationService.NotifyAsync()` after successful persistence. Do not add callers to the legacy `PrintingService` wrapper — route through `PrintToKitchenCommand` instead.
- **Payment finalization:** Payment is not rolled back on receipt print failure — these are independent operations.

---

## 7. XAML Rules (Presentation-Specific)

- All XAML changes require a manual clean + rebuild in Visual Studio Insider before marking complete
- AI tools inside IDEs do not reliably catch XAML compilation errors
- No logic in `.xaml.cs` code-behind — event handlers delegate immediately to ViewModel
- `EnhancedTableControl.xaml` uses code-behind pattern — do not treat it as a standard MVVM control

---

## 8. Problem Files (As of 2026-03-03)

These files are oversized or have known constraints. Any agent receiving a task that touches these files must apply the noted constraint — do not rewrite, only make targeted edits.

| File | Lines | Constraint |
|------|-------|-----------|
| `Presentation/ViewModels/OrderPageViewModel.cs` | 2,293 | Targeted edits only. Extract new state to a new partial file: `OrderPageViewModel.[Feature].cs` |
| `Presentation/ViewModels/TableMapViewModel.cs` | 1,105 | Targeted edits only. Extract additions to partial: `TableMapViewModel.[Feature].cs` |
| `Presentation/ViewModels/SettlePageViewModel.cs` | 919 | Targeted edits only |
| `Presentation/ViewModels/TableDesignerViewModel.cs` | 908 | Targeted edits only |
| `Domain/Entities/Ticket.cs` | 1,315 | Use partial class for additions: `Ticket.Behaviors.cs` |
| `Infrastructure/Repositories/SalesReportRepository.cs` | 1,485 | New report queries go in separate `[ReportName]Repository.cs` |
| `Infrastructure/Services/PrintingService.cs` | Unknown | Contains a legacy kitchen routing wrapper that bypasses KDS notification. Do not add new callers. |

---

## 9. Current Build State (2026-03-03)

| Metric | Value |
|--------|-------|
| Build result | ✅ Succeeds |
| Errors | 0 |
| Warnings | 590 (majority from EF migration Designer files and nullable warnings — expected) |

---

**Section 10 (Test State):**
```
| Workflow — DI gap | 0 | ✅ Fixed in Sprint 000 |
| Application — business logic | 12 | Pre-existing, unrelated to sprint scope |
| Total passing | 144/156 | — |
```

**Note for agents:** Do not touch the 28 existing failing tests unless the task spec explicitly targets them. The 17 E2E and 3 Workflow failures are infrastructure issues, not behavioral failures — they will be fixed in Sprint 000.

---

## 11. Active Blockers

| Blocker | Severity | Status |
|---------|----------|--------|


---

## 12. Things Already Tried — Do Not Revisit

| Approach | Why Rejected |
|----------|-------------|
| Multiple gratuities per ticket | Caused cumulative tip bug. Single-gratuity invariant is a frozen domain rule. |
| Manual Version field increment | Caused double-increment with EF interceptor. EF manages this exclusively. |
| Feature flags for parallel implementations | Caused drift and duplicate maintenance. Removed. |
| DI registrations in App.xaml.cs | Caused conflicting registrations. Centralized in layer extensions. |
| Silent print failure handling | Hid production issues. Now fail-loud with PrintingContractViolationException. |
| Reflection-based {Binding} broadly | Replaced with x:Bind for performance and compile-time safety. |

---

## 13. Sprint History

| Sprint | Status | Summary |
|--------|--------|---------|
| Sprint 000 | ✅ Complete | DB reset fixed, DI gap fixed, Guid.Empty fixed, KDS AutoRoute notification closed |
| Sprint 001 | 🔄 In Progress | File size reduction via partial class split |

---

## Update Instructions

At the end of every sprint, update:
- Section 9 (Build State) — re-run diagnostic if significant changes
- Section 10 (Test State) — update failure counts and categories
- Section 11 (Active Blockers) — close resolved, add newly discovered
- Section 13 (Sprint History) — mark sprint complete, add summary

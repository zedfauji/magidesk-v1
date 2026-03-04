# Magidesk POS — AI Assistant Rules & Constraints

**Last Updated:** 2026-03-03  
**Purpose:** Instructions for any AI assistant (Claude, ChatGPT, Copilot, or IDE agent) working on this codebase. Read this before generating any code or making any suggestions.

---

## 1. Non-Negotiable Rules

These rules are frozen. Do not suggest changing them. Do not work around them. If a task seems to require violating one, stop and flag it to the owner.

### Architecture
- **No business logic in Presentation layer.** ViewModels call Application layer services/commands. They do not contain domain logic.
- **No database access from Presentation.** No EF Core, no repositories, no DbContext in ViewModels or Views.
- **No domain entities exposed to UI.** Use DTOs only across the Application → Presentation boundary.
- **Domain layer has zero external dependencies.** No EF, no HTTP, no file I/O, no NuGet packages except pure utilities.
- **Infrastructure implements interfaces defined in Application or Domain.** Never the reverse.
- **EF Core lives in Infrastructure only.**

### Financial Integrity
- **Tickets and Payments are immutable once finalized.** Never modify a closed ticket. Refunds create new transactions.
- **Only one active gratuity per ticket.** Applying a new gratuity replaces the existing one. Totals recalculate from base.
- **EF Core manages concurrency tokens.** Never manually mutate `Version` or `RowVersion` fields.
- **All financial mutations emit domain events.**

### Code Quality
- **Maximum 300 lines per file.** If a file exceeds this, split it.
- **One class per file.**
- **No silent failures.** If something fails, throw or log explicitly. No swallowed exceptions.
- **No `Guid.Empty` for identity.** Use `UserContextService` for the current user's identity.

### Printing
- **No silent print failures.** Throw `PrintingContractViolationException` on contract violations.
- **Payment is not rolled back on receipt print failure.** These are independent operations.

---

## 2. Dependency Injection Rules

- DI is centralized. Do not add registrations in `App.xaml.cs` for new services.
- Lifetime rules: Repositories → Scoped. Domain Services → Transient or Scoped. `UserContextService` → Singleton.
- If adding a new service, register it in the appropriate layer's DI extension method, not in the entry point.

---

## 3. Namespace Convention

All namespaces must follow this pattern:

| Layer | Namespace |
|---|---|
| Domain | `Magidesk.Domain.*` |
| Application | `Magidesk.Application.*` |
| Infrastructure | `Magidesk.Infrastructure.*` |
| Presentation | `Magidesk.Presentation.*` |

Never use `Magidesk.ViewModels`, `Magidesk.Services`, etc. at the root level.

---

## 4. XAML Rules

- Use `x:Bind` (compiled binding), not `{Binding}` (reflection-based), wherever possible.
- No code-behind logic in `.xaml.cs` files. Event handlers may exist but must immediately delegate to the ViewModel.
- **All XAML changes must be verified in Visual Studio (Insider) with a clean build.** XAML compilation errors are not reliably caught by IDE AI tools.
- If you generate XAML, flag it explicitly: _"This XAML change requires a manual clean + rebuild in Visual Studio to verify."_

---

## 5. ViewModel Rules

- ViewModels inherit from `ObservableObject` (CommunityToolkit.Mvvm).
- Commands use `[RelayCommand]` attribute or `RelayCommand<T>` — not manual `ICommand` implementations.
- Observable properties use `[ObservableProperty]` attribute.
- ViewModels must not exceed 300 lines. Split into partial classes or extract sub-ViewModels if needed.
- ViewModels do not call repositories directly. They call Application layer services or send MediatR commands.

---

## 6. Testing Rules

- Do not modify test infrastructure to make tests pass artificially.
- 31 behavioral test failures exist. Before fixing any of them, trace the failure to the specific domain rule it covers.
- New features must have at minimum a Domain unit test and an Application command/query test.
- E2E tests use UIA3. No `Thread.Sleep`. Deterministic wait only (retry-based polling with timeout).

---

## 7. Things That Have Already Been Tried and Should Not Be Revisited

| Approach | Why rejected |
|---|---|
| Multiple gratuities per ticket | Caused cumulative tip bug. Single-gratuity invariant is now a domain rule. |
| Manual `Version` field increment | Caused double-increment with EF interceptor. EF manages this exclusively now. |
| Feature flags for parallel implementations | Caused drift and duplicate maintenance burden. Removed. |
| DI in `App.xaml.cs` | Caused conflicting registrations. Centralized now. |
| Silent print failure handling | Hid production issues. Now fail-loud with `PrintingContractViolationException`. |
| Reflection-based `{Binding}` broadly | Replaced with `x:Bind` for performance and compile-time safety. |

---

## 8. How to Work on This Project

### For a new feature:
1. Define entity or modify existing entity in Domain
2. Define repository interface in Domain (if needed)
3. Implement repository in Infrastructure
4. Define Command or Query + Handler in Application
5. Create or update ViewModel in Presentation
6. Create or update View (XAML) in Presentation
7. Register new services in DI
8. Write Domain unit test + Application test
9. Verify clean build in Visual Studio

### For a bug fix:
1. Identify which layer owns the bug
2. Fix in the correct layer only — do not leak fixes across layers
3. Add a regression test
4. Verify clean build

### Before ending a session:
- Update `07_current_state_and_open_work.md` with what was completed and what remains
- Commit with a meaningful message
- Note any new blockers discovered

---

## 9. Red Flags — Stop and Ask the Owner

Stop generating code and ask if you encounter any of these:

- A task seems to require adding business logic to a ViewModel
- A task seems to require querying the DB from Presentation
- A proposed solution requires modifying a finalized financial record in place
- A proposed solution adds a new EF dependency to Domain or Application
- A file is approaching or exceeding 300 lines and splitting isn't obvious
- A change affects the concurrency token or `Version` field behavior
- A change affects how payments are finalized or reversed

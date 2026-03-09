# Task Spec: TICKET-S001 — Tests Layer

## Ticket Summary
Verify that all partial class splits from TICKET-S001 produce zero test regressions, and add a structural lint test that enforces the 300-line file limit going forward.

## This Task's Responsibility
This task does NOT add new behavioral tests — no new domain logic or application handlers were introduced.

The Test Agent must:
1. Run the existing test suite to confirm 144/156 passing (no regressions from the splits)
2. Add one structural enforcement test to `Magidesk.Domain.Tests` that confirms `Ticket.cs` (the primary partial file) compiles as a partial class
3. Add one structural enforcement test to `Magidesk.Infrastructure.Tests` that confirms `SalesReportRepository.cs` compiles as a partial class
4. Add a file-size compliance test in an appropriate test project that asserts no `.cs` file in the production source exceeds 300 lines

## Input Contract
- Output contract from ViewModel Agent confirming all 4 ViewModels are split and build passes
- Output contracts from Domain Agent (Ticket.cs split) and Infrastructure Agent (SalesReportRepository.cs split)

## Output Contract (Required)
The Test Agent must end its response with the standard OUTPUT CONTRACT block listing:
- All new test files created (paths)
- Confirmation that existing test suite result is 144/156 (no regressions)
- Confirmation that new tests pass
- XAML build required: NO
- Handoff to: Review Agent

## Files to Create

### Domain Tests
- `src/Magidesk.Domain.Tests/Entities/TicketPartialClassTests.cs`
  - One test: `Ticket_IsPartialClass_DefinedAcrossMultipleFiles` — asserts that the `Ticket` type is a partial class (using reflection: check `typeof(Ticket).GetCustomAttributes()` or verify the type exists and compiles correctly; alternatively verify multiple source files exist via file system check)

### Infrastructure Tests
- `src/Magidesk.Infrastructure.Tests/Repositories/SalesReportRepositoryPartialClassTests.cs`
  - One test: `SalesReportRepository_IsPartialClass_DefinedAcrossMultipleFiles` — asserts the repository type compiles and resolves correctly as a split class

### File Size Compliance Test
- `src/Magidesk.Domain.Tests/Compliance/FileSizeComplianceTests.cs`
  - One test: `AllProductionCsFiles_AreUnder300Lines` — scans all `.cs` files under `src/` (excluding `*.Designer.cs` migration designer files, which are auto-generated and exempt) and asserts each is ≤ 300 lines
  - Uses `System.IO` to enumerate files — no EF or network dependencies
  - Fails with a descriptive message listing all violating files and their line counts

## Files to Modify
None — do NOT modify existing test infrastructure.

## Constraints
- Follow all rules in `.agent/rules/`
- No `Thread.Sleep` or arbitrary delays
- Do not modify existing failing tests
- Add to existing test projects — do not create new test projects
- Each test must be independently runnable
- Mock any injected services with non-null, non-empty values where needed
- Tests must use the project's existing test conventions (check existing test files for naming pattern)
- Test naming convention: `[Subject]_[Condition]_[ExpectedResult]`

## Acceptance Criteria
- [ ] `dotnet test` runs and produces 144/156 passing (or better — no regressions)
- [ ] 3 new tests added and all 3 pass
- [ ] `AllProductionCsFiles_AreUnder300Lines` passes (confirms every split succeeded)
- [ ] `TicketPartialClassTests` passes
- [ ] `SalesReportRepositoryPartialClassTests` passes
- [ ] No existing test was modified

## Do NOT
- Add behavioral tests for Ticket domain logic (out of scope — no logic changed)
- Add handler tests (no handlers changed)
- Modify existing test infrastructure or base classes
- Create a new test project
- Use `Thread.Sleep` or `Task.Delay` as a synchronization mechanism
- Use a null or empty Guid as a mock user identity if IUserContextService is injected

## XAML Flag
NO — this task does not produce or modify XAML

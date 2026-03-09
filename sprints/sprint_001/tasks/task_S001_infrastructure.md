# Task Spec: TICKET-S001 — Infrastructure Layer

## Ticket Summary
Split `SalesReportRepository.cs` (1,485 lines) into multiple partial class files to reduce every file to under 300 lines. No logic changes, no behavior changes, no query modifications.

## This Task's Responsibility
Split `Magidesk.Infrastructure/Repositories/SalesReportRepository.cs` into partial class files by report category. The class declaration, constructor, injected dependencies, and shared private utility methods remain in `SalesReportRepository.cs`. All report query methods are distributed to named partial files in the same directory. Class name, namespace, all method signatures, and all behavior remain identical.

### ⚠️ FROZEN PATH — Owner Authorized via TICKET-S001
`Magidesk.Infrastructure/Repositories/**` is listed in FROZEN.md. This structural split is explicitly authorized by TICKET-S001. The agent must make NO logic changes — only move code between partial files within the same class.

### Suggested Split Plan
Based on pre-flight analysis of 1,485 lines:

| Partial File | Feature Area | Target Max Lines |
|---|---|---|
| `SalesReportRepository.cs` (retain) | Class declaration, constructor, injected `MagideskDbContext`, shared private helpers (e.g. date range helpers) | ≤ 150 lines |
| `SalesReportRepository.FinancialSummary.cs` | Balance reports, sales summary queries, sales detail queries | ≤ 280 lines |
| `SalesReportRepository.PaymentReports.cs` | Payment method breakdown, card processing reports | ≤ 200 lines |
| `SalesReportRepository.LaborReports.cs` | Labor costs, productivity, server performance | ≤ 280 lines |
| `SalesReportRepository.MenuReports.cs` | Menu item sales, category performance reports | ≤ 200 lines |
| `SalesReportRepository.OperationalReports.cs` | Delivery reports, tips reports, attendance reports | ≤ 280 lines |
| `SalesReportRepository.CashReports.cs` | Cash session reports, settlement reports, drawer pull | ≤ 200 lines |

**Note:** Adjust split boundaries at implementation time. If any suggested partial file would exceed 299 lines, split it further. Keep the split label descriptive of its content.

## Input Contract
- Output contract from Domain Agent: `Ticket` partial class split confirmed, build passing
- Source file: `src/Magidesk.Infrastructure/Repositories/SalesReportRepository.cs` (1,485 lines)

## Output Contract (Required)
The Infrastructure Agent must end its response with the standard OUTPUT CONTRACT block listing:
- All new partial files created (paths)
- Confirmation that `SalesReportRepository.cs` was modified to add `partial` keyword
- Confirmation that all resulting files are under 300 lines
- Confirmation of zero behavior changes
- Handoff to: ViewModel Agent

## Files to Create
- `src/Magidesk.Infrastructure/Repositories/SalesReportRepository.FinancialSummary.cs` — Partial class: financial summary and sales detail queries
- `src/Magidesk.Infrastructure/Repositories/SalesReportRepository.PaymentReports.cs` — Partial class: payment method and card processing queries
- `src/Magidesk.Infrastructure/Repositories/SalesReportRepository.LaborReports.cs` — Partial class: labor, productivity, and server performance queries
- `src/Magidesk.Infrastructure/Repositories/SalesReportRepository.MenuReports.cs` — Partial class: menu item and category report queries
- `src/Magidesk.Infrastructure/Repositories/SalesReportRepository.OperationalReports.cs` — Partial class: delivery, tips, and attendance report queries
- `src/Magidesk.Infrastructure/Repositories/SalesReportRepository.CashReports.cs` — Partial class: cash session and settlement report queries

## Files to Modify
- `src/Magidesk.Infrastructure/Repositories/SalesReportRepository.cs` — Add `partial` keyword; retain class declaration, constructor, DbContext field, and shared private utility methods. Remove all report query methods that move to partial files. Must end under 300 lines.
  - **FROZEN PATH CONSTRAINT:** Targeted edits only. Do not reformat, reorder, or rename any members. Do not change any EF Core queries or ORM logic.

## Constraints
- Follow all rules in `.agent/rules/`
- Maximum file line limit: 300 lines per `.cs` file
- One class per file (partial keyword allowed — same class, same namespace)
- No silent failures
- NO logic changes of any kind — identical query behavior before and after
- All partial files must declare the same namespace and `public partial class SalesReportRepository`
- EF Core tracking state must not be altered
- Do NOT manually mutate any ORM-managed fields (Version, RowVersion, concurrency tokens)
- Do NOT add business logic
- No new DI registrations required

## Acceptance Criteria
- [ ] `SalesReportRepository.cs` has the `partial` keyword on the class declaration
- [ ] All new partial files compile as part of the same `SalesReportRepository` class
- [ ] Every resulting file is under 300 lines
- [ ] `dotnet build` passes with 0 errors
- [ ] Test results remain 144/156 passing (no regressions)
- [ ] No query was renamed, removed, or had its return type/signature altered

## Do NOT
- Change any EF query logic, filter, join, or projection
- Change any method signature, access modifier, or return type
- Rename any class, method, property, or field
- Add new EF configurations or migrations
- Create a new non-partial class
- Modify any other Infrastructure file
- Register anything new in DI (no DI changes for this ticket)

## XAML Flag
NO — this task does not produce or modify XAML

# Task Spec: TICKET-S001 — Domain Layer

## Ticket Summary
Split `Ticket.cs` (1,315 lines) into multiple partial class files to reduce every file to under 300 lines. No logic changes, no behavior changes, no invariant modifications.

## This Task's Responsibility
Split `Magidesk.Domain/Entities/Ticket.cs` into partial class files by feature area. The class declaration, constructor(s), and core factory methods remain in `Ticket.cs`. All other logical groups are extracted to named partial files in the same directory. The class name, namespace, all method signatures, and all behavior remain identical.

### ⚠️ FROZEN PATH — Owner Authorized via TICKET-S001
`Magidesk.Domain/**` is listed in FROZEN.md. This structural split is explicitly authorized by TICKET-S001. The agent must make NO logic changes — only move code between partial files within the same class.

### Suggested Split Plan
Based on pre-flight analysis:

| Partial File | Feature Area | Approx Source Lines |
|---|---|---|
| `Ticket.cs` (retain) | Class declaration, fields, constructor, factory method, and core computed properties | ~150 lines |
| `Ticket.OrderLines.cs` | AddOrderLine, RemoveOrderLine, UpdateOrderLine methods | ~100 lines |
| `Ticket.Payments.cs` | Payment validation, AddPayment, payment-related methods | ~120 lines |
| `Ticket.StatusTransitions.cs` | Close, Void, Reopen, Refund, status transition methods | ~260 lines |
| `Ticket.Discounts.cs` | ApplyDiscount, RemoveDiscount, discount validation, financial operations | ~230 lines |
| `Ticket.Charges.cs` | Gratuity methods, fee methods, charge management | ~230 lines |
| `Ticket.TableOperations.cs` | AssignTable, ReleaseTable, delivery operations, seat operations | ~130 lines |
| `Ticket.State.cs` | State metadata helpers, audit properties, domain event emission, misc state | ~130 lines |

**Note:** Adjust split boundaries at implementation time to ensure every resulting partial file is under 300 lines. Do not leave any partial file over 299 lines.

## Input Contract
- Source file: `src/Magidesk.Domain/Entities/Ticket.cs` (1,315 lines)
- No upstream agent — this is Step 1

## Output Contract (Required)
The Domain Agent must end its response with the standard OUTPUT CONTRACT block listing:
- All new partial files created (paths)
- Confirmation that `Ticket.cs` was modified to add `partial` keyword
- Confirmation that all resulting files are under 300 lines
- Confirmation of zero behavior changes
- Handoff to: Infrastructure Agent

## Files to Create
- `src/Magidesk.Domain/Entities/Ticket.OrderLines.cs` — Partial class: order line management methods
- `src/Magidesk.Domain/Entities/Ticket.Payments.cs` — Partial class: payment methods
- `src/Magidesk.Domain/Entities/Ticket.StatusTransitions.cs` — Partial class: status transition methods
- `src/Magidesk.Domain/Entities/Ticket.Discounts.cs` — Partial class: discount and financial operation methods
- `src/Magidesk.Domain/Entities/Ticket.Charges.cs` — Partial class: gratuity and charge methods
- `src/Magidesk.Domain/Entities/Ticket.TableOperations.cs` — Partial class: table and delivery operations
- `src/Magidesk.Domain/Entities/Ticket.State.cs` — Partial class: state metadata, domain events, misc

## Files to Modify
- `src/Magidesk.Domain/Entities/Ticket.cs` — Add `partial` keyword to class declaration; remove the methods that move to partial files; retain: using directives, namespace, class declaration, fields, constructor, factory method, computed properties. Must end under 300 lines.
  - **FROZEN PATH CONSTRAINT:** Targeted edits only. Do not reformat, reorder, or rename any members. Do not change any logic, invariants, or access modifiers.

## Constraints
- Follow all rules in `.agent/rules/`
- Maximum file line limit: 300 lines per `.cs` file
- One class per file (partial keyword allowed — same class, same namespace)
- No silent failures
- NO logic changes of any kind — identical behavior before and after
- NO new using directives unless the split requires moving a `using` that was file-scoped
- All partial files must declare: `namespace Magidesk.Domain.Entities;` and `public partial class Ticket`
- Do NOT change any method signature, access modifier, return type, or XML doc comment
- Do NOT add or remove domain events
- Do NOT change any invariant checks

## Acceptance Criteria
- [ ] `Ticket.cs` has the `partial` keyword on the class declaration
- [ ] All new partial files compile as part of the same `Ticket` class in `Magidesk.Domain.Entities`
- [ ] Every resulting file (including `Ticket.cs`) is under 300 lines
- [ ] `dotnet build` passes with 0 errors
- [ ] Test results remain 144/156 passing (no regressions)
- [ ] No method was renamed, removed, or had its signature altered
- [ ] No logic was changed

## Do NOT
- Change any business logic, invariant, or domain rule
- Change any method signature or access modifier
- Rename any class, method, property, or field
- Add new dependencies or using directives beyond what moved with the code
- Create a new non-partial class
- Modify any other Domain file besides Ticket.cs and its new partials
- Register anything in DI (no DI changes for this ticket)

## XAML Flag
NO — this task does not produce or modify XAML

# Guardrail Compliance Matrix

> This document maps each ticket to the applicable guardrails and quality gates.

---

## Guardrail Reference

| ID | Rule | Source File |
|----|------|-------------|
| G01 | No silent failures | `no-silent-failure.md` |
| G02 | MVVM pattern (no logic in ViewModel) | `mvvm-pattern.md` |
| G03 | Rich domain model | `domain-model.md` |
| G04 | Exception handling contract | `exception-handling-contract.md` |
| G05 | Testing requirements (Domain 90%, App 80%, Infra 70%) | `testing-requirements.md` |
| G06 | Code quality (complexity, naming) | `code-quality.md` |
| G07 | Layer violations (no upward deps) | `guardrails.md` |
| G08 | Security (no PII in logs, PIN hashing) | `security-policies.md` |
| G09 | Database rules (migrations, PostgreSQL compatibility) | `database-rules.md` |
| G10 | Async safety (no async void, no fire-and-forget) | `async-and-background-safety.md` |
| G11 | Invariant enforcement (domain-level validation) | `invariants-enforcement.md` |
| G12 | Library usage (Context7 MCP, approved libraries) | `library-usage.md` |
| G13 | UI controls (WinUI 3 patterns, accessibility) | `ui-controls.md` |
| G14 | Startup safety (initialization order, error handling) | `startup-and-lifecycle-safety.md` |
| G15 | Git workflow (feature branches, PR reviews) | `git-workflow.md` |

---

## Backend Ticket Compliance

### Category A - Table & Game Management

| Ticket ID | G01 | G03 | G04 | G05 | G06 | G07 | G08 | G09 | G10 | G11 | Notes |
|-----------|-----|-----|-----|-----|-----|-----|-----|-----|-----|-----|-------|
| BE-A.1-01 | ✓ | ✓✓ | ✓ | ✓✓ | ✓ | ✓ | - | ✓ | ✓ | ✓✓ | Domain entity - G03, G11 critical |
| BE-A.1-02 | ✓ | ✓ | ✓✓ | ✓ | ✓ | ✓ | - | - | ✓ | ✓ | Command - G04 critical |
| BE-A.2-01 | ✓ | ✓ | ✓✓ | ✓ | ✓ | ✓ | - | - | ✓ | ✓ | Command |
| BE-A.3-01 | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ | - | - | ✓ | - | Query - inferred requirements |
| BE-A.5-01 | ✓ | ✓✓ | ✓ | ✓✓ | ✓ | ✓ | - | ✓✓ | - | ✓✓ | Domain entity - G09 migration, G11 invariants |
| BE-A.9-01 | ✓ | ✓✓ | ✓ | ✓✓ | ✓ | ✓ | - | - | - | ✓ | Domain service - G05 critical |

### Category E - Reservations

| Ticket ID | G01 | G03 | G04 | G05 | G06 | G07 | G08 | Notes |
|-----------|-----|-----|-----|-----|-----|-----|-----|-------|
| BE-E.1-01 | ✓ | ✓✓ | ✓ | ✓✓ | ✓ | ✓ | - | Domain entity |
| BE-E.1-02 | ✓ | ✓ | ✓✓ | ✓ | ✓ | ✓ | - | Command |
| BE-E.5-01 | ✓ | ✓✓ | ✓ | ✓✓ | ✓ | ✓ | - | Domain service |

### Category F - Customer/Member

| Ticket ID | G01 | G03 | G04 | G05 | G06 | G07 | G08 | Notes |
|-----------|-----|-----|-----|-----|-----|-----|-----|-------|
| BE-F.1-01 | ✓ | ✓✓ | ✓ | ✓✓ | ✓ | ✓ | ✓✓ | Customer - PII concerns |
| BE-F.3-01 | ✓ | ✓✓ | ✓ | ✓✓ | ✓ | ✓ | ✓ | Member entity |

### Category J - Security

| Ticket ID | G01 | G03 | G04 | G05 | G06 | G07 | G08 | Notes |
|-----------|-----|-----|-----|-----|-----|-----|-----|-------|
| BE-J.1-01 | ✓ | ✓ | ✓✓ | ✓ | ✓ | ✓ | ✓✓ | Manager auth - G08 critical |

---

## Frontend Ticket Compliance

| Ticket ID | G01 | G02 | G04 | G06 | G10 | G13 | Notes |
|-----------|-----|-----|-----|-----|-----|-----|-------|
| FE-A.1-01 | ✓ | ✓✓ | ✓ | ✓ | ✓✓ | ✓ | Dialog - G02, G10 critical |
| FE-A.2-01 | ✓ | ✓✓ | ✓ | ✓ | ✓✓ | ✓ | Dialog |
| FE-E.2-01 | ✓ | ✓✓ | ✓ | ✓ | ✓ | ✓ | Page |
| FE-F.1-01 | ✓ | ✓✓ | ✓ | ✓ | ✓ | ✓ | Page |
| FE-J.1-01 | ✓ | ✓✓ | ✓ | ✓ | ✓ | ✓ | PIN dialog - security focus |
| FE-J.1-02 | ✓ | ✓✓ | ✓ | ✓ | ✓ | ✓ | Login - G01, G08 critical |

---

## Code Review Checklist (Per Guardrail)

### G01: No Silent Failures
- [ ] All catch blocks have user notification
- [ ] No empty catch blocks
- [ ] Error messages are user-friendly
- [ ] Severity classified correctly

### G02: MVVM Pattern
- [ ] No business logic in ViewModel
- [ ] ViewModel calls Application layer only
- [ ] Uses DTOs, not domain entities
- [ ] ICommand implementations proper
- [ ] IDialogService for errors

### G03: Rich Domain Model
- [ ] Entity has behavior methods
- [ ] Invariants enforced in constructor
- [ ] No public setters on properties
- [ ] State transitions validated
- [ ] Domain events raised if applicable

### G04: Exception Handling
- [ ] Result pattern OR exception-based (consistent)
- [ ] Severity classification applied
- [ ] Layer-appropriate handling
- [ ] User-visible error messages

### G05: Testing Requirements
- [ ] Domain tests ≥90% coverage
- [ ] Application tests ≥80% coverage
- [ ] Infrastructure tests ≥70% coverage
- [ ] Invariant tests present
- [ ] Edge cases covered

### G06: Code Quality
- [ ] Method ≤30 lines
- [ ] Parameters ≤5
- [ ] Cyclomatic complexity ≤10
- [ ] Naming follows conventions
- [ ] Comments for non-obvious logic

### G07: Layer Violations
- [ ] No Infrastructure refs from Domain
- [ ] No Application refs from Domain
- [ ] No UI refs from Application
- [ ] Dependency direction: UI → App → Domain

### G08: Security
- [ ] No PII in log statements
- [ ] PINs are hashed, not plain text
- [ ] Sensitive operations audited
- [ ] Authorization checks present

### G09: Database Rules
- [ ] All schema changes via EF Core migrations
- [ ] PostgreSQL compatibility (use .ToLower() for case-insensitive)
- [ ] Proper indexes on foreign keys
- [ ] Decimal precision for money (10,2)
- [ ] Migrations tested before applying

### G10: Async Safety
- [ ] No async void (except UI event handlers)
- [ ] No fire-and-forget tasks
- [ ] All background tasks observed/awaited
- [ ] AsyncRelayCommand for ViewModel async operations
- [ ] Exception handling in async methods

### G11: Invariant Enforcement
- [ ] Invariants enforced in domain entities
- [ ] Unit tests for all invariants
- [ ] Proper domain exceptions thrown
- [ ] No bypassing domain validation
- [ ] Invariants documented

### G12: Library Usage
- [ ] Use Context7 MCP for library documentation
- [ ] Use approved libraries (EF Core, MVVM Toolkit, FluentValidation)
- [ ] No unmaintained libraries
- [ ] Follow library best practices
- [ ] Keep libraries updated

### G13: UI Controls
- [ ] WinUI 3 patterns followed
- [ ] Accessibility support (keyboard nav, screen readers)
- [ ] Proper data binding
- [ ] No business logic in code-behind
- [ ] Consistent UI/UX patterns

### G14: Startup Safety
- [ ] Proper initialization order
- [ ] Database connection delayed until needed
- [ ] Fatal errors surface to user
- [ ] Graceful degradation when possible
- [ ] Startup errors logged

### G15: Git Workflow
- [ ] Feature branches for all work
- [ ] Pull request reviews required
- [ ] No direct commits to master
- [ ] Descriptive commit messages
- [ ] Squash commits before merge

---

*Last Updated: 2026-01-08*

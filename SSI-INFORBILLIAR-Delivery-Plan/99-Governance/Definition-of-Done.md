# Definition of Done

> **All tickets must satisfy these criteria before marking as DONE.**

---

## General Criteria (All Tickets)

### 1. Code Complete
- [ ] All acceptance criteria from ticket are met
- [ ] Code compiles without errors
- [ ] No compiler warnings (or documented exceptions)

### 2. Quality Standards
- [ ] Follows coding conventions in `code-quality.md`
- [ ] Method length ≤30 lines
- [ ] Cyclomatic complexity ≤10
- [ ] Naming follows conventions

### 3. Guardrail Compliance
- [ ] Applicable guardrails from compliance matrix satisfied
- [ ] No silent failures (G01)
- [ ] Layer violations checked (G07)

### 4. Documentation
- [ ] Public APIs documented with XML comments
- [ ] Non-obvious logic explained in comments
- [ ] README updated if applicable

---

## Backend Ticket Criteria

### Domain Layer Tickets

- [ ] Entity has rich behavior (not anemic)
- [ ] All invariants enforced in entity
- [ ] No public property setters
- [ ] State transitions validated
- [ ] Unit tests ≥90% coverage
- [ ] All invariant tests pass

### Application Layer Tickets

- [ ] One use case per handler
- [ ] FluentValidation for input validation
- [ ] Result pattern or proper exception handling
- [ ] Unit tests ≥80% coverage
- [ ] Integration tests for workflows

### Infrastructure Layer Tickets

- [ ] Repository implements interface correctly
- [ ] EF configuration complete
- [ ] Migration runs successfully
- [ ] Can rollback migration
- [ ] Unit tests ≥70% coverage

---

## Frontend Ticket Criteria

### Page Tickets

- [ ] Page loads without errors
- [ ] All bindings work correctly
- [ ] Keyboard navigation functional
- [ ] Screen reader accessible (WCAG 2.1)
- [ ] Localized strings (no hardcoded text)
- [ ] Loading states implemented
- [ ] Error states handled and displayed

### Dialog Tickets

- [ ] Dialog opens/closes correctly
- [ ] Focus management correct
- [ ] Escape key closes dialog
- [ ] Validation feedback visible
- [ ] Submit disables during processing
- [ ] Success/failure feedback provided

### Control Tickets

- [ ] Control reusable
- [ ] Proper dependency properties
- [ ] Event handlers documented
- [ ] Works in design mode

---

## Testing Criteria

### Unit Tests

- [ ] Tests are isolated (no external deps)
- [ ] Arrange-Act-Assert pattern
- [ ] Descriptive test names
- [ ] Edge cases covered
- [ ] Negative cases covered

### Integration Tests

- [ ] Database transactions rolled back
- [ ] Test data isolated
- [ ] Cleanup performed

---

## Review Process

1. **Self-Review**: Developer reviews own code against this checklist
2. **Peer Review**: At least one other developer reviews
3. **CI/CD Pass**: All automated checks pass
4. **Merge**: Squash and merge to main branch

---

## Exception Process

If a criterion cannot be met:

1. Document the exception in the PR description
2. Get approval from tech lead
3. Create follow-up ticket for remediation
4. Mark ticket as DONE with exception noted

---

*Last Updated: 2026-01-08*

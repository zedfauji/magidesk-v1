---
name: payment-sideeffect-auditor
description: Detects missing required side-effects (e.g. receipt printing) in payment success workflows.
---

# Payment Side-Effect Auditor

## Trigger
Activate this skill when:
- Modifying payment workflows
- Reviewing payment-related ViewModels or CommandHandlers
- Adding or refactoring payment processing logic
- Auditing transaction or settlement flows

Examples:
- “Fix payment flow”
- “Review SettleViewModel”
- “Why is receipt not printing after payment?”

---

## Purpose
Ensure that **required operational side-effects** occur when a payment succeeds.

This skill does **not** modify code automatically.
It **detects and flags** missing side-effects so they are fixed intentionally.

---

## Authoritative Assumptions (Project-Specific)

In this project:
- A successful payment MUST trigger:
  - Receipt printing (automatic)
- Receipt printing logic already exists
- Manual reprint is NOT a substitute for automated behavior
- Kitchen printing is intentionally manual and must NOT be inferred as missing

---

## What to Analyze

1. Identify payment success paths, such as:
   - `ProcessPaymentCommandHandler`
   - `SettleViewModel.ProcessPaymentAsync`
   - Any method that:
     - Records a payment
     - Returns `ProcessPaymentResult.Success`
     - Commits a transaction

2. For each success path:
   - Inspect whether required side-effects are invoked:
     - Receipt printing
     - Relevant domain notifications (if any)

---

## Detection Rules (Strict)

Flag a **MISSING SIDE-EFFECT** if ALL are true:
- A payment is successfully processed
- The workflow completes normally
- NO receipt printing is triggered via:
  - `PrintReceiptCommand`
  - `IReceiptPrintService`
  - Equivalent existing mechanism

DO NOT flag:
- Manual reprint commands
- Kitchen printing paths
- Failed or cancelled payments

---

## Output Requirements

If a missing side-effect is detected, report:

- **Location**
  - Class name
  - Method name
- **What is missing**
  - e.g. “Receipt printing not triggered”
- **Why it matters**
  - Customer does not receive receipt automatically
- **Recommended fix (high-level)**
  - Where to invoke receipt printing
  - Which existing service or command to use

Example output:

> ❗ Missing Side-Effect Detected  
> Payment succeeds in `SettleViewModel.ProcessPaymentAsync`,  
> but no receipt printing is triggered.  
> Recommendation: invoke existing `PrintReceiptCommand`  
> after `ProcessPaymentResult.Success`.

---

## Constraints (Non-Negotiable)

- Do NOT introduce new printing logic
- Do NOT auto-refactor code
- Do NOT guess business rules
- Do NOT infer side-effects that are not explicitly required
- Do NOT change behavior silently

If requirements are ambiguous, STOP and ask.

---

## Skill Philosophy

This skill acts as:
- A **senior engineer reviewing payment logic**
- A **guardrail against operational regressions**
- A **deterministic check**, not an AI guess

Correctness > cleverness.

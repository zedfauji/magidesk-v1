# Production Readiness Score

**Overall Status:** 🚫 **UNSAFE FOR PRODUCTION**

## Category Scores

| Category | Score | Notes |
| :--- | :--- | :--- |
| **Transactional Integrity** | ❌ **Critical Fail** | Order submission loop allows partial commits. Data corruption guaranteed on network instability. |
| **Data Safety** | ❌ **Critical Fail** | Modifiers are silently dropped (Data Loss). Price can be spoofed by client. |
| **Concurrency** | ⚠️ **Caution** | EF Core Optimistic Concurrency handles the happy path, but failure handling (retries/UX) is missing. |
| **Billing Accuracy** | ✅ **Ready** | Assuming server clock sync. Logic is sound. |
| **Security** | ⚠️ **Caution** | Relies on yet-to-be-implemented `Scoped` context services. Current state is unverifiable. |

## Blocking Issues (Must Fix)
1.  **Batch Transaction:** Create `AddOrderBatchCommandHandler` to ensure atomicity of order submission.
2.  **Modifier Mapping:** Implement DTO-to-Entity mapping for Modifiers in `OrdersController`.
3.  **Price Validation:** Ignore `UnitPrice` from client or strictly validate against `MenuRepo`.

## Timeline to Fix
*   Est. Effort: 1-2 Days (Backend Engineering)

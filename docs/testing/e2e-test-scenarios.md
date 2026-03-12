# E2E Test Scenarios

## Test Coverage Matrix

This document maps requirements to test scenarios.

### P0 - Financial Safety Tests

| Requirement | Test Class | Coverage |
|-------------|------------|----------|
| 1. Authentication | AuthenticationTests | Complete |
| 5. Single Payment | SinglePaymentTests | Complete |
| 6. Split Payment | SplitPaymentTests | Complete |
| 7. Split Tickets | SplitTicketTests | Complete |
| 8. Cash Sessions | CashSessionTests | Complete |
| 3. Pool Tables | PoolTableTests | Complete |
| 12. Reporting | ReportingTests | Complete |

### P1 - Operational Integrity Tests

| Requirement | Test Class | Coverage |
|-------------|------------|----------|
| 2. Order Entry | OrderEntryBasicTests | Complete |
| 4. Dining Tables | DiningTableTests | Complete |
| 9. KDS Integration | KDSIntegrationTests | Complete |
| 10. Inventory | InventoryTests | Complete |
| 11. Customer Management | CustomerManagementTests | Complete |
| 13. Menu Configuration | MenuConfigurationTests | Complete |

### P2 - Stability Tests

| Requirement | Test Class | Coverage |
|-------------|------------|----------|
| 14. Localization | LocalizationTests | Partial |
| 15. Error Handling | ErrorHandlingTests | Partial |
| 16. Performance | PerformanceTests | Partial |

## Property-Based Tests

- Property 1-27: Comprehensive property tests covering round-trip, invariants, idempotence, and metamorphic properties

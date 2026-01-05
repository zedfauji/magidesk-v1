# TECH-B003: Printing Service Unit Tests

## Description
Fix the `NotImplementedException` failures in the infrastructure test suite.

## Scope
-   **File**: `Magidesk.Infrastructure.Tests.Services.PrintingServiceTests.cs`

## Implementation Tasks
- [ ] Implement `TestPrintingService_PrintReceipt_ShouldPrintReceipt`.
- [ ] Implement `TestPrintingService_PrintOrder_ShouldPrintOrder`.
- [ ] Mock `IPrinterAdapter` and `IPrinterRepository` dependencies.
- [ ] Verify that `Print` method is called with correct parameters.

## Acceptance Criteria
-   All tests in `PrintingServiceTests` pass.
-   Code coverage for `PrintingService` > 80%.

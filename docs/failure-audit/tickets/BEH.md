# Backend Exception Handling (BEH) Tickets

| ID | Title | Severity | Status | Owner |
| :--- | :--- | :--- | :--- | :--- |
| **BEH-001** | Audit `PrintingService` for Swallowed Exceptions | **MEDIUM** | DONE | Antigravity |
| **BEH-002** | Audit `PaymentService` for Swallowed Exceptions | **MEDIUM** | DONE | Antigravity |

## Details

### BEH-001: Audit `PrintingService` Exception Propagation
*   **Location**: `Magidesk.Infrastructure.Services.PrintingService`
*   **Failure**: Risk of catching printer errors and logging them without notifying the UI (Silent Failure).
*   **Requirement**: Ensure `PrintAsync` methods throw `PrintingException`. Do not swallow errors.

### BEH-002: Audit `PaymentService` Exception Propagation
*   **Location**: `Magidesk.Infrastructure.Services` (Check for Payment implementation)
*   **Failure**: Financial operations must fail loudly if they don't complete.
*   **Requirement**: Verify no "pokemon" catches (`catch (Exception) { return false; }`) in payment logic.

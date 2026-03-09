# Review Report: TICKET-S001

## Result: FAIL

---

## Violations Found

### CRITICAL: Access Modifier Mismatch — Infrastructure Layer

**Affected Files:** All 8 partial files in `src/Magidesk.Infrastructure/Repositories/SalesReportRepository*.cs` (excluding primary)

- `src/Magidesk.Infrastructure/Repositories/SalesReportRepository.CashReports.cs` — Declared `internal partial class` but primary is `public partial class`
- `src/Magidesk.Infrastructure/Repositories/SalesReportRepository.DeliveryReports.cs` — Declared `internal partial class` but primary is `public partial class`
- `src/Magidesk.Infrastructure/Repositories/SalesReportRepository.FinancialSummary.cs` — Declared `internal partial class` but primary is `public partial class`
- `src/Magidesk.Infrastructure/Repositories/SalesReportRepository.LaborProductivity.cs` — Declared `internal partial class` but primary is `public partial class`
- `src/Magidesk.Infrastructure/Repositories/SalesReportRepository.LaborReports.cs` — Declared `internal partial class` but primary is `public partial class`
- `src/Magidesk.Infrastructure/Repositories/SalesReportRepository.MenuReports.cs` — Declared `internal partial class` but primary is `public partial class`
- `src/Magidesk.Infrastructure/Repositories/SalesReportRepository.PaymentReports.cs` — Declared `internal partial class` but primary is `public partial class`
- `src/Magidesk.Infrastructure/Repositories/SalesReportRepository.TipsAttendance.cs` — Declared `internal partial class` but primary is `public partial class`

**Rule Violated:** AGENT_ROLES.md specifies "All partial class files must use correct access modifier matching the primary class"

**Required Fix:** Change all partial files from `internal partial class` to `public partial class` to match the primary class declaration. Infrastructure Agent must correct this.

---

### CRITICAL: Access Modifier Mismatch — ViewModel Layer (TableDesignerViewModel)

**Affected Files:** 5 partial files of TableDesignerViewModel have inconsistent access modifiers

- `src/Magidesk.Presentation/ViewModels/TableDesignerViewModel.LayoutLifecycle.cs` — Declared `public partial class` but others are `internal`
- `src/Magidesk.Presentation/ViewModels/TableDesignerViewModel.LayoutPublish.cs` — Declared `public partial class` but others are `internal`
- `src/Magidesk.Presentation/ViewModels/TableDesignerViewModel.SaveLoad.cs` — Declared `public partial class` but others are `internal`
- `src/Magidesk.Presentation/ViewModels/TableDesignerViewModel.Selection.cs` — Declared `public partial class` but others are `internal`
- `src/Magidesk.Presentation/ViewModels/TableDesignerViewModel.Validation.cs` — Declared `public partial class` but others are `internal`
- `src/Magidesk.Presentation/ViewModels/TableDesignerViewModel.cs` — Declared `public partial class` (primary)

All others (DataLoading, LayoutManagement, Performance, TableOperations, UIInteractions) are `internal partial class`.

**Rule Violated:** AGENT_ROLES.md specifies "All partial class files must use correct access modifier matching the primary class"

**Required Fix:** Change all partial files to `internal partial class` to match the primary class declaration at line 18: `internal partial class TableDesignerViewModel`. ViewModel Agent must correct this.

---

### CRITICAL: Structural Violation — OrderPageViewModel.HelperModels.cs

**File:** `src/Magidesk.Presentation/ViewModels/OrderPageViewModel.HelperModels.cs`

**Rule Violated:** AGENT_ROLES.md specifies "One class per file (partial keyword allowed)"

**Issue:** This file contains 4 separate classes/types:
1. `OrderItemViewModel` (line 9)
2. `ProductViewModel` (line 39)
3. `ProductCategoryViewModel` (line 54)
4. `SessionState` enum (line 64)

**Required Fix:** Extract each class/enum into its own file:
- `src/Magidesk.Presentation/ViewModels/OrderItemViewModel.cs` (new)
- `src/Magidesk.Presentation/ViewModels/ProductViewModel.cs` (new)
- `src/Magidesk.Presentation/ViewModels/ProductCategoryViewModel.cs` (new)
- `src/Magidesk.Presentation/ViewModels/SessionState.cs` (new)

ViewModel Agent must split this file and delete the multi-class version.

---

### CRITICAL: Structural Violation — SettlePageViewModel.HelperModels.cs

**File:** `src/Magidesk.Presentation/ViewModels/SettlePageViewModel.HelperModels.cs`

**Rule Violated:** AGENT_ROLES.md specifies "One class per file"

**Issue:** This file contains a malformed class definition — duplicate `public class PaymentMethodViewModel` declarations (lines 8 and 10). The code structure is broken:

```csharp
public class PaymentMethodViewModel
{
public class PaymentMethodViewModel
{
    // ... code here ...
}
}
```

This is syntactically invalid and indicates a structural merge error or incomplete refactoring.

**Required Fix:** Either:
1. If `PaymentMethodViewModel` is a helper nested class in `SettlePageViewModel`, convert it to a proper partial declaration and add it to a SettlePageViewModel partial file, OR
2. If it is a standalone class, create `src/Magidesk.Presentation/ViewModels/PaymentMethodViewModel.cs` with correct syntax.

The current file structure is broken and must be repaired by the ViewModel Agent.

---

### CRITICAL: Namespace Violation — SalesReportRepository Partial Files

**File:** `src/Magidesk.Infrastructure/Repositories/SalesReportRepository.FinancialSummary.cs` (and all other partials)

**Rule Verified:** ✅ Correct namespace: `namespace Magidesk.Infrastructure.Repositories;`

**Status:** This check passed. All partial files have correct namespaces.

---

### CRITICAL: Domain Layer — Ticket Partial Files

**Files Checked:** All 9 Ticket*.cs files

**Line Count Summary:**
- Ticket.cs: 165 lines ✅
- Ticket.Charges.cs: 162 lines ✅
- Ticket.Discounts.cs: 155 lines ✅
- Ticket.HoldRelease.cs: 74 lines ✅
- Ticket.OrderLines.cs: 65 lines ✅
- Ticket.Payments.cs: 78 lines ✅
- Ticket.Refunds.cs: 155 lines ✅
- Ticket.State.cs: 294 lines ✅
- Ticket.StatusTransitions.cs: 143 lines ✅
- Ticket.TableOperations.cs: 129 lines ✅

**Status:** ✅ All Domain partial files are under 300 lines. Namespaces correct. Access modifier (`public partial class Ticket`) consistent.

---

### CRITICAL: Infrastructure Layer — SalesReportRepository Partial Files (Line Count)

**Files Checked:** All 9 SalesReportRepository*.cs files

**Line Count Summary:**
- SalesReportRepository.cs: 48 lines ✅
- SalesReportRepository.CashReports.cs: 143 lines ✅
- SalesReportRepository.DeliveryReports.cs: 124 lines ✅
- SalesReportRepository.FinancialSummary.cs: 272 lines ✅
- SalesReportRepository.LaborProductivity.cs: 194 lines ✅
- SalesReportRepository.LaborReports.cs: 184 lines ✅
- SalesReportRepository.MenuReports.cs: 166 lines ✅
- SalesReportRepository.PaymentReports.cs: 177 lines ✅
- SalesReportRepository.TipsAttendance.cs: 227 lines ✅

**Status:** ✅ All Infrastructure partial files are under 300 lines EXCEPT for the access modifier violations noted above.

---

### WARNING: Presentation Layer — OrderPageViewModel Partial Files (Line Count)

**Files Checked:** All 14 OrderPageViewModel*.cs files

**File with Concern:**
- OrderPageViewModel.OrderItemAddition.cs: 300 lines (AT THE LIMIT, technically compliant but at edge)

**Status:** ✅ All OrderPageViewModel partial files are ≤ 300 lines. Namespaces correct. Access modifiers consistent (`public partial class` in primary, `internal partial class` in partials).

---

### WARNING: Presentation Layer — TableMapViewModel Partial Files (Line Count)

**Files Checked:** All 7 TableMapViewModel*.cs files

**Line Count Summary:**
- TableMapViewModel.cs: 199 lines ✅
- TableMapViewModel.DataRefresh.cs: 215 lines ✅
- TableMapViewModel.Permissions.cs: 131 lines ✅
- TableMapViewModel.ServerManagement.cs: 131 lines ✅
- TableMapViewModel.SessionDialogs.cs: 262 lines ✅
- TableMapViewModel.TableActions.cs: 185 lines ✅
- TableMapViewModel.TableOperations.cs: 99 lines ✅

**Status:** ✅ All TableMapViewModel partial files are under 300 lines. Namespaces correct. Access modifiers consistent (`public partial class` in primary, `internal partial class` in partials).

---

### WARNING: Presentation Layer — SettlePageViewModel Partial Files (Line Count)

**Files Checked:** All 6 SettlePageViewModel*.cs files

**Line Count Summary:**
- SettlePageViewModel.cs: 171 lines ✅
- SettlePageViewModel.AdditionalOperations.cs: 152 lines ✅
- SettlePageViewModel.PaymentProcessing.cs: 275 lines ✅
- SettlePageViewModel.TenderEntry.cs: 116 lines ✅
- SettlePageViewModel.TicketModifications.cs: 244 lines ✅
- SettlePageViewModel.HelperModels.cs: 27 lines (BUT STRUCTURALLY BROKEN) ❌

**Status:** ⚠️ SettlePageViewModel.HelperModels.cs is structurally invalid (noted above in critical violations).

---

### WARNING: Presentation Layer — TableDesignerViewModel Partial Files (Line Count)

**Files Checked:** All 11 TableDesignerViewModel*.cs files

**Line Count Summary:**
- TableDesignerViewModel.cs: 155 lines ✅
- TableDesignerViewModel.DataLoading.cs: 105 lines ✅
- TableDesignerViewModel.LayoutLifecycle.cs: 261 lines ✅
- TableDesignerViewModel.LayoutManagement.cs: 153 lines ✅
- TableDesignerViewModel.LayoutPublish.cs: 78 lines ✅
- TableDesignerViewModel.Performance.cs: 298 lines ✅
- TableDesignerViewModel.SaveLoad.cs: 98 lines ✅
- TableDesignerViewModel.Selection.cs: 218 lines ✅
- TableDesignerViewModel.TableOperations.cs: 85 lines ✅
- TableDesignerViewModel.UIInteractions.cs: 87 lines ✅
- TableDesignerViewModel.Validation.cs: 172 lines ✅

**Status:** ⚠️ All files are under 300 lines, but access modifier inconsistencies exist (noted above in critical violations).

---

### COMPLIANCE TESTS

**File:** `src/Magidesk.Domain.Tests/Compliance/FileSizeComplianceTests.cs`

**Status:** ✅ Test exists and correctly scans for files exceeding 300 lines. Test structure is sound.

**File:** `src/Magidesk.Domain.Tests/Compliance/TicketPartialClassTests.cs`

**Status:** ✅ Test exists and verifies Ticket partial class structure. Test instantiates Ticket correctly and checks file count.

**File:** `src/Magidesk.Infrastructure.Tests/Compliance/SalesReportRepositoryPartialClassTests.cs`

**Status:** ✅ Test exists and verifies SalesReportRepository partial class structure. Test checks file count and type resolution.

---

## Scope Discrepancy Finding

**SIGNIFICANT INCONSISTENCY DISCOVERED:**

The ticket description (in PROJECT_CONTEXT.md Section 8) states:
> "These files are oversized or have known constraints... Six production files exceed the 300 line limit"

**But** the acceptance criteria (in all task specs) state:
> "Every file in the codebase is under 300 lines"

**ACTUAL CODEBASE STATE:**
A file system scan reveals **20+ files** in the Presentation layer that exceed 300 lines, including but not limited to:
- AdvancedPricingConfigurationViewModel.cs: 471 lines
- AuditLogViewModel.cs: 422 lines
- CashDropManagementViewModel.cs: 310 lines
- ExportImportManagementViewModel.cs: 429 lines
- FloorManagementViewModel.cs: 311 lines
- GratuitySelectionViewModel.cs: 306 lines
- KitchenDisplayViewModel.cs: 404 lines
- LoginViewModel.cs: 380 lines
- ManagerFunctionsViewModel.cs: 320 lines
- MenuEditorViewModel.cs: 578 lines
- RealTimeSessionMonitoringViewModel.cs: 415 lines
- RefundWizardViewModel.cs: 355 lines
- SalesReportsViewModel.cs: 461 lines
- ServerSectionManagementViewModel.cs: 381 lines
- SplitTicketViewModel.cs: 527 lines
- SwitchboardViewModel.cs: 904 lines
- SystemConfigViewModel.cs: 486 lines
- TableSessionViewModel.cs: 564 lines
- TicketManagementViewModel.cs: 303 lines
- TicketViewModel.cs: 378 lines

**TICKET INTERPRETATION:**
TICKET-S001 is explicitly scoped to split **only these 6 specific files:**
1. `Ticket.cs` (Domain)
2. `SalesReportRepository.cs` (Infrastructure)
3. `OrderPageViewModel.cs` (Presentation)
4. `TableMapViewModel.cs` (Presentation)
5. `SettlePageViewModel.cs` (Presentation)
6. `TableDesignerViewModel.cs` (Presentation)

The acceptance criteria states "every file in the codebase is under 300 lines" but this is **aspirational language that exceeds the ticket scope**. The ticket is NOT responsible for refactoring the other 20+ files.

**RECOMMENDATION:**
The review team should clarify whether:
1. The acceptance criteria wording is aspirational (ticket only splits the 6 named files), or
2. A follow-up sprint should be created to split the remaining 20+ oversized ViewModels

For this review, acceptance is interpreted as: **All 6 target files must be split such that all resulting partials are under 300 lines.** The other 20+ files are out of scope.

---

## XAML Build Required

YES — The ViewModel Agent's task spec explicitly requires a manual clean + rebuild after splitting ViewModels to verify compiled x:Bind still resolves correctly.

---

## Approved for Commit

NO

### Required Fixes Before Commit (By Agent Responsible)

**For Infrastructure Agent:**
1. Correct access modifier in all 8 SalesReportRepository partial files:
   - Change `internal partial class` to `public partial class` in:
     - SalesReportRepository.CashReports.cs
     - SalesReportRepository.DeliveryReports.cs
     - SalesReportRepository.FinancialSummary.cs
     - SalesReportRepository.LaborProductivity.cs
     - SalesReportRepository.LaborReports.cs
     - SalesReportRepository.MenuReports.cs
     - SalesReportRepository.PaymentReports.cs
     - SalesReportRepository.TipsAttendance.cs

**For ViewModel Agent:**
1. Fix SettlePageViewModel.HelperModels.cs:
   - Either convert `PaymentMethodViewModel` to a proper nested class declaration, or
   - Extract to its own file `PaymentMethodViewModel.cs` with correct syntax
   - The current duplicate/malformed class definition must be repaired

2. Fix OrderPageViewModel.HelperModels.cs:
   - Split into separate files (one class per file):
     - OrderItemViewModel.cs
     - ProductViewModel.cs
     - ProductCategoryViewModel.cs
     - SessionState.cs
   - Delete the current multi-class HelperModels.cs file

3. Fix TableDesignerViewModel access modifiers:
   - Change these 5 partial files from `public partial class` to `internal partial class`:
     - TableDesignerViewModel.LayoutLifecycle.cs
     - TableDesignerViewModel.LayoutPublish.cs
     - TableDesignerViewModel.SaveLoad.cs
     - TableDesignerViewModel.Selection.cs
     - TableDesignerViewModel.Validation.cs

### After Fixes Are Applied:
1. Ensure `dotnet build` passes with 0 errors
2. Run `dotnet test` to confirm all tests pass (expect 144/156 or better)
3. Re-run Review Agent to produce PASS report
4. Perform manual clean + rebuild in Visual Studio Insider (XAML verification)
5. Only then proceed to commit

---

## Summary Table

| Category | Result | Details |
|----------|--------|---------|
| Domain Layer (Ticket) | ✅ PASS | All files under 300 lines, correct access modifiers, correct namespaces |
| Infrastructure Layer (SalesReportRepository) | ❌ FAIL | Access modifier mismatch: partials are `internal`, primary is `public` |
| ViewModel Layer (OrderPageViewModel) | ❌ FAIL | HelperModels.cs violates "one class per file" rule (contains 4 types) |
| ViewModel Layer (TableMapViewModel) | ✅ PASS | All files under 300 lines, correct access modifiers, correct namespaces |
| ViewModel Layer (SettlePageViewModel) | ❌ FAIL | HelperModels.cs is structurally broken (malformed class definition) |
| ViewModel Layer (TableDesignerViewModel) | ❌ FAIL | Access modifier mismatch: some partials are `public`, others are `internal` |
| Compliance Tests | ✅ PASS | All required tests exist and are well-formed |
| Line Limit Compliance (Target 6 Files) | ✅ PASS | All 6 target files are split with all resulting partials under 300 lines |
| Scope Discrepancy | ⚠️ NOTED | Acceptance criteria states "every file" but ticket only covers 6 files. 20+ other files remain over 300 lines. |

---

**Review Completed:** 2026-03-04
**Review Agent:** Senior Engineering Lead (Role 8)

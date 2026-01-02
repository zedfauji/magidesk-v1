# Orphan Pages Analysis
**Audit Date:** 2026-01-01

## 1. Potentially Unreachable Pages
The following pages exist in the codebase but lack clear navigation entry points from the main Switchboard or Dashboard:

- **AuthorizationCaptureBatchDialog.xaml**:
  - **Status:** Potential Orphan.
  - **Context:** Likely intended for "End of Day" settlements, but no button on `SettlePage` or `BackOfficePage` explicitly triggers this dialog in the analyzed navigation flow. It may only be triggered by failing background jobs, which makes it practically unreachable for manual override.

- **DiscountTaxPage.xaml**:
  - **Status:** Integrated but Buried.
  - **Context:** Exists in `Views`, but `BackOfficePage` tabs need verification to ensure this specific page is loaded. If it relies on a "Discount" button in Order Entry, it's not a management page. Parity requires it to be a Management Page.

## 2. Dead Code / Legacy Artifacts
- **SplitTicketDialog_Backup.xaml**:
  - **Status:** Dead Code.
  - **Recommendation:** Delete. Confusing for developers maintenance.
- **SplitTicketDialog_Minimal.xaml**:
  - **Status:** Experimental/Dead.
  - **Recommendation:** Delete.

# TECH-U004: Drawer Pull Report Implementation

## Description
Connect the `DrawerPullReportDialog` to real data instead of displaying placeholder "$0.00" values.

## Scope
-   **File**: `Magidesk/Views/DrawerPullReportDialog.xaml`
-   **Feature**: `F-0012`

## Implementation Tasks
- [ ] Bind `TextBlock` values to `DrawerPullReportViewModel` properties.
- [ ] Ensure `Tips`, `Sales`, and `Tax` are calculated correctly in the ViewModel.
- [ ] Remove hardcoded "($0.00)" strings.

## Acceptance Criteria
-   Report accurately reflects the current shift's financial data.
-   Totals match the database query results.

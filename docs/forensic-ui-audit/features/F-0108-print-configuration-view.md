# Feature: Print Configuration View (F-0108)

## Classification
- **Parity classification**: PARITY REQUIRED
- **MagiDesk status**: MISSING

## Problem / Why this exists (grounded)
- **Operational need**: POS needs to print receipts, kitchen orders, reports. Printers must be configurable (which printer for what, paper size, receipt format).
- **Evidence**: `PrintConfigurationView.java` - printer selection, printer groups, receipt templates, kitchen printer assignment.

## User-facing surfaces
- **Surface type**: Configuration panel
- **UI entry points**: BackOfficeWindow → Configuration → Printing; Add Printer dialogs
- **Exit paths**: Save / Cancel

## Preconditions & protections
- **User/role/permission checks**: Configuration permission
- **State checks**: Printers must be available on system
- **Manager override**: Not required

## Step-by-step behavior (forensic)
1. User opens Configuration → Printing
2. View shows:
   - Receipt printer selection
   - Kitchen printer(s) selection
   - Report printer selection
   - Printer groups (for menu item routing)
   - Receipt format options (full, compact)
   - Paper size settings
   - Cash drawer connection
3. AddPrinterDialog for new printers
4. AddPrinterGroupDialog for grouping
5. Test print option
6. Save persists printer configuration

## Edge cases & failure paths
- **Printer offline**: Error on test print; warning on save
- **No printers found**: List empty
- **Printer group empty**: May cause routing failures

## Data / audit / financial impact
- **Writes/updates**: Printer configuration; PrinterGroup entities
- **Audit events**: Config changes logged
- **Financial risk**: Low - operational; missed kitchen prints affect service

## Code traceability (REQUIRED)
- **Primary UI class(es)**: 
  - `PrintConfigurationView` → `config/ui/PrintConfigurationView.java`
  - `AddPrinterDialog` → `config/ui/AddPrinterDialog.java`
  - `PrinterGroupView` → `config/ui/PrinterGroupView.java`
- **Entry action(s)**: Part of ConfigurationDialog
- **Workflow/service enforcement**: PrintConfig.java for settings
- **Messages/labels**: Printer names, group names

## Uncertainties (STOP; do not guess)
- Network printer discovery
- Printer driver compatibility (platform specific)
- Virtual printer support

## MagiDesk parity notes
- **What exists today**: No printer configuration
- **What differs / missing**: Entire print configuration system

## Porting strategy (PLAN ONLY)
- **Backend requirements**: 
  - Printer entity with name, type, connection
  - PrinterGroup with associated menu items
- **API/DTO requirements**: Printer config stored locally or in DB
- **UI requirements**: Full print configuration view
- **Constraints for implementers**: Windows printing APIs; consider ESC/POS for receipt printers

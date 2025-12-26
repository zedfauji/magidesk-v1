# Feature: Terminal Configuration View (F-0106)

## Classification
- **Parity classification**: PARITY REQUIRED
- **MagiDesk status**: MISSING

## Problem / Why this exists (grounded)
- **Operational need**: Each terminal may have different settings - screen orientation, default order type, timeout, auto-logout, receipt printing options.
- **Evidence**: `TerminalConfigurationView.java` + `TerminalConfig.java` - per-terminal settings distinct from restaurant-wide settings.

## User-facing surfaces
- **Surface type**: Configuration view
- **UI entry points**: BackOfficeWindow → Configuration → Terminal; initial setup
- **Exit paths**: Save / Cancel

## Preconditions & protections
- **User/role/permission checks**: Configuration permission
- **State checks**: Terminal must be registered
- **Manager override**: Not required

## Step-by-step behavior (forensic)
1. Open Terminal Configuration
2. View shows terminal-specific settings:
   - Terminal number
   - Terminal name
   - Default order type
   - Screen orientation (portrait/landscape)
   - Full screen mode
   - Auto-logout timeout
   - Receipt auto-print
   - Kitchen print enabled
   - Customer display enabled
   - Payment filters
   - Order type filters
3. Save applies to current terminal
4. May require restart for some settings

## Edge cases & failure paths
- **Invalid terminal number**: Validation error
- **Conflicting settings**: Warned but allowed

## Data / audit / financial impact
- **Writes/updates**: Terminal entity; TerminalConfig properties
- **Audit events**: Config changes logged
- **Financial risk**: Low - terminal behavior

## Code traceability (REQUIRED)
- **Primary UI class(es)**: 
  - `TerminalConfigurationView` → `config/ui/TerminalConfigurationView.java`
  - `TerminalConfig` → `config/TerminalConfig.java`
- **Entry action(s)**: Part of ConfigurationDialog
- **Workflow/service enforcement**: TerminalConfig persistence
- **Messages/labels**: Setting labels

## MagiDesk parity notes
- **What exists today**: Basic terminal concept
- **What differs / missing**: TerminalConfigurationView with all settings

## Porting strategy (PLAN ONLY)
- **Backend requirements**: Terminal entity with settings
- **API/DTO requirements**: GET/PUT /terminals/{id}/config
- **UI requirements**: TerminalConfigurationView
- **Constraints for implementers**: Settings persist per-terminal

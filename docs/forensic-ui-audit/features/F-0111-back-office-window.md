# Feature: Back Office Window (F-0111)

## Classification
- **Parity classification**: PARITY REQUIRED
- **MagiDesk status**: PARTIAL (Admin pages exist but not unified BO window)

## Problem / Why this exists (grounded)
- **Operational need**: Managers need access to configuration, reports, user management, menu management outside of the POS workflow. Separate management interface.
- **Evidence**: `BackOfficeWindow.java` - main window with JMenuBar containing menus for Reports, Menu Management, Configuration, Users, etc. Uses tabbed pane for content.

## User-facing surfaces
- **Surface type**: Standalone window with menus and tabs
- **UI entry points**: LoginView → Back Office button; ShowBackofficeAction
- **Exit paths**: Close window (saves size/location)

## Preconditions & protections
- **User/role/permission checks**: Back office access permission
- **State checks**: User must be logged in with appropriate role
- **Manager override**: Not required (permission-based)

## Step-by-step behavior (forensic)
1. User clicks Back Office button from login or switchboard
2. BackOfficeWindow opens (size/position from saved config)
3. Menu bar provides access to:
   - **File**: Data import/export, language selection
   - **Edit**: (editing options)
   - **Explorers**: Menu categories, groups, items, modifiers
   - **Reports**: All report types
   - **Configuration**: Restaurant, terminal, card, print settings
   - **Floor Plan**: Table layout management (plugin)
4. Each menu action opens content in tabbed pane
5. Multiple tabs can be open simultaneously
6. On close: Window size/position saved

## Edge cases & failure paths
- **No permission for menu item**: Item hidden or disabled
- **Multiple BO windows**: Singleton pattern (getInstance)
- **Unsaved changes in tab**: Prompt on close

## Data / audit / financial impact
- **Writes/updates**: All configuration, menu, user data through explorers
- **Audit events**: Changes logged in ActionHistory
- **Financial risk**: Misconfiguration affects pricing, taxes, operations

## Code traceability (REQUIRED)
- **Primary UI class(es)**: `BackOfficeWindow` → `bo/ui/BackOfficeWindow.java`
- **Entry action(s)**: `ShowBackofficeAction` → `actions/ShowBackofficeAction.java`
- **Workflow/service enforcement**: All BO actions in `bo/actions/`; explorers in `bo/ui/explorer/`
- **Messages/labels**: Menu item labels; tab titles

## Uncertainties (STOP; do not guess)
- Plugin system for floor plan menu
- Permission granularity for individual menu items

## MagiDesk parity notes
- **What exists today**: Admin pages for tables, users, menu (scattered)
- **What differs / missing**: Unified Back Office window; tabbed interface; full menu structure

## Porting strategy (PLAN ONLY)
- **Backend requirements**: Permission checks for each BO function
- **API/DTO requirements**: All CRUD endpoints for managed entities
- **UI requirements**: 
  - BackOfficeWindow with menu bar
  - Tabbed content area
  - All explorer/browser views
- **Constraints for implementers**: Maintain consistent navigation; permission-gate all sections

# Feature: Restaurant Configuration View (F-0105)

## Classification
- **Parity classification**: PARITY REQUIRED
- **MagiDesk status**: MISSING

## Problem / Why this exists (grounded)
- **Operational need**: Restaurant settings (name, address, currency, tax config, receipt headers, business rules) must be configurable without code changes.
- **Evidence**: `RestaurantConfigurationView.java` - settings panel for restaurant-wide configuration; part of ConfigurationDialog.

## User-facing surfaces
- **Surface type**: Configuration panel (in Configuration Dialog)
- **UI entry points**: BackOfficeWindow → Configuration → Restaurant
- **Exit paths**: Save / Cancel / Close dialog

## Preconditions & protections
- **User/role/permission checks**: Configuration permission; manager/admin level
- **State checks**: None
- **Manager override**: Action itself requires admin

## Step-by-step behavior (forensic)
1. User opens Back Office → Configuration
2. Selects Restaurant tab
3. Configuration view shows:
   - Restaurant name
   - Address lines
   - Phone, email, website
   - Currency symbol
   - Tax settings (included in price, rounding rules)
   - Receipt header/footer text
   - Logo upload
   - Operating hours
4. User modifies settings
5. On Save: Settings persisted to database
6. Changes take effect for new operations

## Edge cases & failure paths
- **Invalid currency**: Prevented by dropdown
- **Missing required fields**: Validation error
- **Concurrent edit**: Last save wins

## Data / audit / financial impact
- **Writes/updates**: RestaurantConfig or globalized settings table
- **Audit events**: Config changes logged
- **Financial risk**: Currency, tax settings affect all transactions

## Code traceability (REQUIRED)
- **Primary UI class(es)**: `RestaurantConfigurationView` → `config/ui/RestaurantConfigurationView.java`
- **Entry action(s)**: `ConfigureRestaurantAction` → `bo/actions/ConfigureRestaurantAction.java`
- **Workflow/service enforcement**: Configuration persistence
- **Messages/labels**: Field labels

## Uncertainties (STOP; do not guess)
- Multi-location support (separate configs per location)
- Timezone handling

## MagiDesk parity notes  
- **What exists today**: Hardcoded or limited configuration
- **What differs / missing**: Full restaurant configuration UI

## Porting strategy (PLAN ONLY)
- **Backend requirements**: 
  - RestaurantSettings entity or config table
  - UpdateRestaurantSettingsCommand
- **API/DTO requirements**: GET/PUT /restaurant-config
- **UI requirements**: RestaurantConfigurationView with all fields
- **Constraints for implementers**: Changes must not affect in-flight transactions

# Feature: Tax Configuration View (F-0109)

## Classification
- **Parity classification**: PARITY REQUIRED
- **MagiDesk status**: PARTIAL (taxes exist but config UI may differ)

## Problem / Why this exists (grounded)
- **Operational need**: Configure tax rates and rules - state/local rates, tax-inclusive pricing, item-level overrides, tax exemptions.
- **Evidence**: `TaxConfigurationView.java` - tax configuration as part of restaurant settings.

## User-facing surfaces
- **Surface type**: Configuration view
- **UI entry points**: BackOfficeWindow → Configuration → Taxes
- **Exit paths**: Save / Cancel

## Preconditions & protections
- **User/role/permission checks**: Configuration permission
- **State checks**: None
- **Manager override**: Not required

## Step-by-step behavior (forensic)
1. Open Tax Configuration
2. View shows global tax settings:
   - Default tax rate
   - Tax included in price (toggle)
   - Rounding rules
   - Tax on gratuities
   - Tax on discounts
   - Tax breakdown on receipt
3. Links to Tax Explorer for managing rates
4. Links to Tax Groups
5. Save applies settings
6. Affects all new transactions

## Edge cases & failure paths
- **Invalid rate**: Range validation
- **Changing included to excluded**: May affect existing menu prices

## Data / audit / financial impact
- **Writes/updates**: Tax configuration; global settings
- **Audit events**: Config changes logged
- **Financial risk**: Incorrect tax collection is compliance violation

## Code traceability (REQUIRED)
- **Primary UI class(es)**: `TaxConfigurationView` → `config/ui/TaxConfigurationView.java`
- **Entry action(s)**: Part of ConfigurationDialog
- **Workflow/service enforcement**: Tax calculation engine
- **Messages/labels**: Tax setting labels

## MagiDesk parity notes
- **What exists today**: Tax entity exists
- **What differs / missing**: TaxConfigurationView for global settings

## Porting strategy (PLAN ONLY)
- **Backend requirements**: Tax settings (global); Tax rates (per-item)
- **API/DTO requirements**: GET/PUT /tax-config
- **UI requirements**: TaxConfigurationView
- **Constraints for implementers**: Tax changes should not affect closed tickets

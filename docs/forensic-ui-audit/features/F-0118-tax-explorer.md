# Feature: Tax Explorer (F-0118)

## Classification
- **Parity classification**: PARITY REQUIRED
- **MagiDesk status**: PARTIAL (tax entities exist but management UI may differ)

## Problem / Why this exists (grounded)
- **Operational need**: Configure tax rates, tax groups, and tax rules. Different items may have different tax rates; some orders may be tax-exempt.
- **Evidence**: `TaxExplorer.java` + `TaxGroupExplorer.java` - CRUD for tax definitions; percent rates; tax groups for combining taxes.

## User-facing surfaces
- **Surface type**: Explorer panel (in Back Office)
- **UI entry points**: BackOfficeWindow → Explorers → Taxes
- **Exit paths**: Close tab

## Preconditions & protections
- **User/role/permission checks**: Tax configuration permission; admin only
- **State checks**: None
- **Manager override**: Action requires admin

## Step-by-step behavior (forensic)
1. Open Tax Explorer from Back Office
2. View shows table of tax definitions:
   - Tax name
   - Rate (percent)
   - Active status
3. Actions:
   - New: Create new tax rate
   - Edit: Modify tax rate
   - Delete: Remove tax (if unused)
4. Tax form includes:
   - Name
   - Rate (decimal/percent)
   - Description
5. TaxGroupExplorer for combining taxes:
   - Group name
   - Associated tax rates
   - Effective combined rate

## Edge cases & failure paths
- **Delete tax in use**: Prevent or show warning
- **Invalid rate**: Must be 0-100% range
- **Tax group circular reference**: Prevented by design

## Data / audit / financial impact
- **Writes/updates**: Tax entity; TaxGroup entity
- **Audit events**: Tax changes logged (critical for compliance)
- **Financial risk**: Incorrect tax collection is compliance violation

## Code traceability (REQUIRED)
- **Primary UI class(es)**: 
  - `TaxExplorer` → `bo/ui/explorer/TaxExplorer.java`
  - `TaxGroupExplorer` → `bo/ui/explorer/TaxGroupExplorer.java`
- **Entry action(s)**: `TaxExplorerAction` → `bo/actions/TaxExplorerAction.java`
- **Workflow/service enforcement**: TaxDAO
- **Messages/labels**: Tax name, rate labels

## Uncertainties (STOP; do not guess)
- Tax-exempt order handling
- Multi-jurisdiction tax support

## MagiDesk parity notes
- **What exists today**: Tax entity exists
- **What differs / missing**: Tax Explorer UI; Tax Group management

## Porting strategy (PLAN ONLY)
- **Backend requirements**: 
  - Tax entity with name, rate
  - TaxGroup entity for combined taxes
  - Menu items link to tax or tax group
- **API/DTO requirements**: GET/POST/PUT/DELETE /taxes, /tax-groups
- **UI requirements**: TaxExplorer and TaxGroupExplorer
- **Constraints for implementers**: Rate changes should not affect closed tickets

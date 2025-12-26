# Feature: Tip Declare Action (F-0066)

## Classification
- **Parity classification**: PARITY WITH MODERNIZATION
- **MagiDesk status**: MISSING

## Problem / Why this exists (grounded)
- **Operational need**: Servers declare cash tips received. Required for tax reporting.
- **Evidence**: Gratuity management + tip declaration - servers report cash tips.

## User-facing surfaces
- **Surface type**: Action + dialog
- **UI entry points**: Shift end; Manager → Tip Declaration
- **Exit paths**: Tips declared / Skip

## Preconditions & protections
- **User/role/permission checks**: Server with tips
- **State checks**: End of shift preferred
- **Manager override**: Not typically required

## Step-by-step behavior (forensic)
1. Server initiates tip declaration
2. TipDeclareDialog shows:
   - Server name
   - Shift period
   - Credit card tips (auto-calculated)
   - Cash tips (entry field)
3. Server enters cash tips estimate
4. On confirm:
   - Declaration recorded
   - Added to tip report
   - Used for tax reporting

## Edge cases & failure paths
- **Zero tips**: Allowed
- **Excessive amount**: Warning
- **Missing declaration**: Follow-up required

## Data / audit / financial impact
- **Writes/updates**: Tip declaration record
- **Audit events**: Declaration logged
- **Financial risk**: IRS compliance

## Code traceability (REQUIRED)
- **Primary UI class(es)**: Tip declaration dialog
- **Entry action(s)**: Shift end or manager action
- **Workflow/service enforcement**: Gratuity tracking
- **Messages/labels**: Declaration prompts

## MagiDesk parity notes
- **What exists today**: Gratuity entity
- **What differs / missing**: Cash tip declaration flow

## Porting strategy (PLAN ONLY)
- **Backend requirements**: TipDeclaration entity
- **API/DTO requirements**: POST /tip-declarations
- **UI requirements**: TipDeclareDialog
- **Constraints for implementers**: Link to shift and user

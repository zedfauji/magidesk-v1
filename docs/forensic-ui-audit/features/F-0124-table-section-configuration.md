# Feature: Table Section Configuration (F-0124)

## Classification
- **Parity classification**: PARITY WITH MODERNIZATION
- **MagiDesk status**: MISSING

## Problem / Why this exists (grounded)
- **Operational need**: Group tables into server sections for assignment. Manage server responsibilities.
- **Evidence**: `TableSection` entity + related - section-based server assignment.

## User-facing surfaces
- **Surface type**: Configuration view (in Back Office)
- **UI entry points**: BackOfficeWindow → Floor Plan → Sections
- **Exit paths**: Save / Cancel

## Preconditions & protections
- **User/role/permission checks**: Floor plan management permission
- **State checks**: None
- **Manager override**: Not required

## Step-by-step behavior (forensic)
1. Open Section Configuration
2. View shows sections:
   - Section name
   - Assigned tables
   - Assigned server (current)
3. Create/Edit/Delete sections
4. Section form:
   - Name
   - Table assignment (multi-select)
   - Color (for display)
5. Save persists section
6. Sections available for server assignment

## Edge cases & failure paths
- **Table in multiple sections**: Prevented
- **Delete with tickets**: Warning
- **No sections**: Default to whole floor

## Data / audit / financial impact
- **Writes/updates**: TableSection entity
- **Audit events**: Config changes
- **Financial risk**: None (operational)

## Code traceability (REQUIRED)
- **Primary UI class(es)**: Configuration views
- **Entry action(s)**: Part of floor configuration
- **Workflow/service enforcement**: Section-table mapping
- **Messages/labels**: Section labels

## MagiDesk parity notes
- **What exists today**: No section concept
- **What differs / missing**: Entire section system

## Porting strategy (PLAN ONLY)
- **Backend requirements**: TableSection entity
- **API/DTO requirements**: GET/POST/PUT/DELETE /sections
- **UI requirements**: Section configuration view
- **Constraints for implementers**: Tables reference sections

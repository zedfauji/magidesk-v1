# Feature: Customer Explorer (F-0079)

## Classification
- **Parity classification**: PARITY WITH MODERNIZATION
- **MagiDesk status**: PARTIAL (customers exist but explorer may differ)

## Problem / Why this exists (grounded)
- **Operational need**: View all customers, search, edit, delete. Manage customer database for CRM, loyalty, delivery routing.
- **Evidence**: `CustomerExplorer.java` - customer list with CRUD; search; filters.

## User-facing surfaces
- **Surface type**: Explorer panel (in Back Office)
- **UI entry points**: BackOfficeWindow → Explorers → Customers
- **Exit paths**: Close tab

## Preconditions & protections
- **User/role/permission checks**: Customer management permission
- **State checks**: None
- **Manager override**: Not required

## Step-by-step behavior (forensic)
1. Open Customer Explorer
2. View shows customer table:
   - Name
   - Phone
   - Email
   - VIP status
   - Total orders
   - Last order date
3. Search/filter controls
4. New/Edit/Delete actions
5. Edit opens CustomerForm
6. View order history button

## Edge cases & failure paths
- **Delete with orders**: Soft delete or prevent
- **Large customer base**: Pagination/search required

## Data / audit / financial impact
- **Writes/updates**: Customer entity
- **Audit events**: Customer changes logged
- **Financial risk**: Low - customer data

## Code traceability (REQUIRED)
- **Primary UI class(es)**: `CustomerExplorer` → `customer/CustomerExplorer.java`
- **Entry action(s)**: `CustomerExplorerAction` → `bo/actions/CustomerExplorerAction.java`
- **Workflow/service enforcement**: CustomerDAO
- **Messages/labels**: Column headers

## MagiDesk parity notes
- **What exists today**: Basic customer list
- **What differs / missing**: Full explorer with order history, VIP, pagination

## Porting strategy (PLAN ONLY)
- **Backend requirements**: Customer queries with pagination, search
- **API/DTO requirements**: GET /customers?search=&page=
- **UI requirements**: CustomerExplorer
- **Constraints for implementers**: Performance with large customer bases

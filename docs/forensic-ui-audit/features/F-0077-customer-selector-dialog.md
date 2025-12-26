# Feature: Customer Selector Dialog (F-0077)

## Classification
- **Parity classification**: PARITY WITH MODERNIZATION
- **MagiDesk status**: PARTIAL (customer selection exists but workflow may differ)

## Problem / Why this exists (grounded)
- **Operational need**: Orders need to be associated with customers for delivery, loyalty, CRM, and accounting. Staff need to search/select existing customers or create new ones quickly.
- **Evidence**: `CustomerSelectorDialog.java` - wraps CustomerSelector panel; can attach to ticket; supports caller ID for phone orders; create new ticket option.

## User-facing surfaces
- **Surface type**: Full-screen modal dialog
- **UI entry points**: OrderView → Customer button; delivery order type auto-opens; caller ID integration
- **Exit paths**: Select customer / Create new / Cancel

## Preconditions & protections
- **User/role/permission checks**: Standard order permissions
- **State checks**: Can be called with or without existing ticket
- **Manager override**: Not required

## Step-by-step behavior (forensic)
1. Dialog opens (full PosWindow size)
2. CustomerSelector panel displayed with:
   - Search by phone, name, email
   - Customer list table
   - New customer button
   - Quick customer form
3. User can:
   - Search and select existing customer
   - Create new customer inline
   - Set caller ID (for phone order integration)
4. If createNewTicket flag set, new ticket created with customer
5. Selected customer returned to caller
6. Customer attached to ticket

## Edge cases & failure paths
- **No customer found**: Allow new customer creation
- **Duplicate customer**: Handled during creation (phone number unique)
- **Invalid phone format**: Validation in customer form
- **Caller ID lookup failure**: Gracefully handles missing customer

## Data / audit / financial impact
- **Writes/updates**: Customer entity (if new); Ticket.customer association
- **Audit events**: Customer creation logged
- **Financial risk**: Low - customer data for operational use

## Code traceability (REQUIRED)
- **Primary UI class(es)**: 
  - `CustomerSelectorDialog` → `customer/CustomerSelectorDialog.java`
  - `CustomerSelector` → `customer/CustomerSelector.java`
  - `DefaultCustomerListView` → `customer/DefaultCustomerListView.java`
- **Entry action(s)**: Called from OrderView, delivery flow
- **Workflow/service enforcement**: CustomerDAO for persistence
- **Messages/labels**: `CustomerSelectorDialog.0` (title)

## Uncertainties (STOP; do not guess)
- Caller ID integration method (phone system specific)
- Customer loyalty points integration

## MagiDesk parity notes
- **What exists today**: Customer selection capability (needs verification)
- **What differs / missing**: Caller ID integration; quick customer form

## Porting strategy (PLAN ONLY)
- **Backend requirements**: Customer entity; CustomerService; phone number uniqueness
- **API/DTO requirements**: GET /customers (search); POST /customers (create)
- **UI requirements**: CustomerSelectorDialog with search, list, quick form; attach to ticket
- **Constraints for implementers**: Phone number should be searchable; customer history desirable

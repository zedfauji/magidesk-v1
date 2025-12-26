# Feature: Customer Form (F-0078)

## Classification
- **Parity classification**: PARITY WITH MODERNIZATION
- **MagiDesk status**: PARTIAL (customer entity exists but form may differ)

## Problem / Why this exists (grounded)
- **Operational need**: Collect and store customer information - name, phone, email, address, loyalty info. Essential for delivery, CRM, marketing.
- **Evidence**: `CustomerForm.java` - customer data entry with all fields; validation; address management.

## User-facing surfaces
- **Surface type**: Form panel
- **UI entry points**: CustomerSelectorDialog → New; CustomerExplorer → Edit
- **Exit paths**: Save / Cancel

## Preconditions & protections
- **User/role/permission checks**: Customer management permission
- **State checks**: None
- **Manager override**: Not required

## Step-by-step behavior (forensic)
1. Open Customer Form (new or edit)
2. Form shows fields:
   - First name, Last name
   - Phone (primary)
   - Phone (alternate)
   - Email
   - Address line 1, 2
   - City, State, ZIP
   - Birthday (for promotions)
   - Loyalty number
   - Notes
   - Credit limit (for house accounts)
   - VIP flag
3. Validation on required fields (phone)
4. Address validation/lookup (if integrated)
5. Save persists customer
6. Available for order association

## Edge cases & failure paths
- **Duplicate phone**: Warning or merge suggestion
- **Invalid email format**: Validation error
- **Invalid address**: Allow override for non-standard

## Data / audit / financial impact
- **Writes/updates**: Customer entity
- **Audit events**: Customer creation logged
- **Financial risk**: Credit limits affect house accounts

## Code traceability (REQUIRED)
- **Primary UI class(es)**: `CustomerForm` → `customer/CustomerForm.java`
- **Entry action(s)**: Called from CustomerSelectorDialog, CustomerExplorer
- **Workflow/service enforcement**: CustomerDAO
- **Messages/labels**: Field labels

## MagiDesk parity notes
- **What exists today**: Customer entity exists
- **What differs / missing**: Full CustomerForm with all fields

## Porting strategy (PLAN ONLY)
- **Backend requirements**: Customer entity with all fields
- **API/DTO requirements**: POST/PUT /customers
- **UI requirements**: CustomerForm
- **Constraints for implementers**: Phone number should be searchable

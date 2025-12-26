# Feature: Coupon Explorer (F-0117)

## Classification
- **Parity classification**: PARITY REQUIRED
- **MagiDesk status**: PARTIAL (discounts exist but coupon explorer may differ)

## Problem / Why this exists (grounded)
- **Operational need**: Manage discounts and coupons - percentage off, fixed amount off, BOGO, happy hour specials. Need to create, edit, enable/disable promotions.
- **Evidence**: `CouponExplorer.java` - CRUD for discounts/coupons with various types and rules.

## User-facing surfaces
- **Surface type**: Explorer panel (in Back Office)
- **UI entry points**: BackOfficeWindow → Explorers → Coupons/Discounts
- **Exit paths**: Close tab

## Preconditions & protections
- **User/role/permission checks**: Discount management permission
- **State checks**: None
- **Manager override**: Not required

## Step-by-step behavior (forensic)
1. Open Coupon Explorer
2. View shows coupons/discounts table:
   - Name
   - Type (%, $, BOGO)
   - Value
   - Active
   - Valid dates
3. New/Edit/Delete actions
4. Coupon form includes:
   - Name
   - Coupon code (if required)
   - Type selection
   - Value (% or amount)
   - Minimum purchase
   - Valid date range
   - Applicable items/categories
   - Max uses
   - Active toggle
5. Save persists coupon
6. Available in POS coupon dialog

## Edge cases & failure paths
- **Expired coupon**: Remains in DB, not shown in POS
- **Max uses reached**: Disabled automatically
- **Delete in use**: Historical references preserved

## Data / audit / financial impact
- **Writes/updates**: Discount/Coupon entity
- **Audit events**: Discount creation/changes logged
- **Financial risk**: Unauthorized discounts; revenue loss

## Code traceability (REQUIRED)
- **Primary UI class(es)**: `CouponExplorer` → `bo/ui/explorer/CouponExplorer.java`
- **Entry action(s)**: `CouponExplorerAction` → `bo/actions/CouponExplorerAction.java`
- **Workflow/service enforcement**: DiscountDAO
- **Messages/labels**: Discount type labels

## MagiDesk parity notes
- **What exists today**: Discount entity
- **What differs / missing**: Full CouponExplorer with date ranges, max uses, codes

## Porting strategy (PLAN ONLY)
- **Backend requirements**: Discount entity with type, value, rules
- **API/DTO requirements**: GET/POST/PUT/DELETE /discounts
- **UI requirements**: CouponExplorer with comprehensive form
- **Constraints for implementers**: Usage tracking; expiry enforcement

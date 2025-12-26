# Feature: Multi-Currency Tender Dialog (F-0058)

## Classification
- **Parity classification**: DEFER (advanced feature)
- **MagiDesk status**: MISSING

## Problem / Why this exists (grounded)
- **Operational need**: In tourist areas or border towns, customers may pay with multiple currencies. Staff need to accept and track different currencies.
- **Evidence**: `MultiCurrencyTenderDialog.java` - accepts payment in multiple currencies; calculates exchange rates; tracks per-currency amounts.

## User-facing surfaces
- **Surface type**: Modal dialog
- **UI entry points**: PaymentView → Cash button (when multi-currency enabled)
- **Exit paths**: OK (records payment) / Cancel

## Preconditions & protections
- **User/role/permission checks**: Cash payment permission
- **State checks**: Multi-currency enabled in config; currencies configured
- **Manager override**: Not required

## Step-by-step behavior (forensic)
1. User initiates cash payment
2. If multi-currency enabled, MultiCurrencyTenderDialog opens
3. Dialog shows:
   - Amount due
   - Input fields for each configured currency
   - Exchange rate display
   - Calculated totals
   - Change calculation
4. User enters amounts in each currency
5. System calculates total in base currency
6. Change calculated (in base currency or chosen currency)
7. Payment recorded with per-currency breakdown

## Edge cases & failure paths
- **Insufficient total**: Cannot proceed
- **Exchange rate change**: Use configured rate
- **Exotic currency**: Must be pre-configured

## Data / audit / financial impact
- **Writes/updates**: Payment with currency breakdown; Terminal multi-currency balance
- **Audit events**: Multi-currency transaction logged
- **Financial risk**: Exchange rate errors; currency reconciliation complexity

## Code traceability (REQUIRED)
- **Primary UI class(es)**: `MultiCurrencyTenderDialog` → `ui/dialog/MultiCurrencyTenderDialog.java`
- **Entry action(s)**: Called from PaymentView
- **Workflow/service enforcement**: Currency entity; exchange rate lookup
- **Messages/labels**: Currency symbols, labels

## MagiDesk parity notes
- **What exists today**: Single currency only
- **What differs / missing**: Entire multi-currency system

## Porting strategy (PLAN ONLY)
- **Backend requirements**: Currency entity; exchange rates; multi-currency transaction tracking
- **API/DTO requirements**: Payment with currency breakdown
- **UI requirements**: MultiCurrencyTenderDialog
- **Constraints for implementers**: Exchange rates must be configurable; drawer tracks per-currency

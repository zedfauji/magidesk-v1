# Category D: Tax, Currency & Financial Rules

## D.1 Currency configuration

**Feature ID:** D.1  
**Feature Name:** Currency configuration  
**Status:** PARTIAL

**Backend Evidence:**
- Database tables / columns: `RestaurantConfiguration.CurrencySymbol`
- Domain entities: `RestaurantConfiguration.cs`, `Money.cs` value object
- Services: `RestaurantConfigurationRepository.cs`
- APIs / handlers: System config commands
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): `SystemConfigPage.xaml`
- ViewModels: `SystemConfigViewModel.cs`
- Navigation path: Admin → System Config
- User-visible workflow: Set currency symbol

**Notes:**
- Currency symbol configurable (default: $)
- Single currency only
- Money value object handles precision

**Risks / Gaps:**
- Symbol only, no full currency code (USD, MXN)
- No locale-based formatting

**Recommendation:** EXTEND - Add full currency code and locale formatting

---

## D.2 Multiple currencies (optional)

**Feature ID:** D.2  
**Feature Name:** Multiple currencies  
**Status:** NOT IMPLEMENTED

**Backend Evidence:**
- Database tables / columns: NO EVIDENCE FOUND
- Domain entities: NO EVIDENCE FOUND
- Services: NO EVIDENCE FOUND
- APIs / handlers: NO EVIDENCE FOUND
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): NO EVIDENCE FOUND
- ViewModels: NO EVIDENCE FOUND
- Navigation path: NO EVIDENCE FOUND
- User-visible workflow: NO EVIDENCE FOUND

**Notes:**
- Single currency system only
- No exchange rate handling
- Border locations may need this

**Risks / Gaps:**
- Cannot accept foreign currency
- No FX conversion

**Recommendation:** DEFER - Optional feature, low priority

---

## D.3 Tax configuration (inclusive / exclusive)

**Feature ID:** D.3  
**Feature Name:** Tax configuration (inclusive / exclusive)  
**Status:** PARTIAL

**Backend Evidence:**
- Database tables / columns: Tax configuration in system settings
- Domain entities: Tax handling in `Ticket.CalculateTotals()`
- Services: `TicketDomainService.CalculateTotals()`
- APIs / handlers: Tax calculation
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): `SystemConfigPage.xaml`
- ViewModels: `SystemConfigViewModel.cs`
- Navigation path: Admin → System Config
- User-visible workflow: Tax settings

**Notes:**
- Tax rate configurable
- Inclusive/exclusive mode uncertain from code
- Simplified tax calculation noted in comments

**Risks / Gaps:**
- Price-includes-tax mode may not be fully implemented
- Tax display on receipts unclear

**Recommendation:** HARDEN - Verify inclusive/exclusive modes work correctly

---

## D.4 Multiple tax rates

**Feature ID:** D.4  
**Feature Name:** Multiple tax rates  
**Status:** NOT IMPLEMENTED

**Backend Evidence:**
- Database tables / columns: NO EVIDENCE FOUND (no TaxRates table)
- Domain entities: NO EVIDENCE FOUND
- Services: Single tax rate used
- APIs / handlers: NO EVIDENCE FOUND
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): NO EVIDENCE FOUND
- ViewModels: NO EVIDENCE FOUND
- Navigation path: NO EVIDENCE FOUND
- User-visible workflow: NO EVIDENCE FOUND

**Notes:**
- Single global tax rate only
- No per-item or per-category tax rates
- Some jurisdictions require multiple rates

**Risks / Gaps:**
- Cannot handle food vs beverage different rates
- Cannot handle state + local taxes

**Recommendation:** EXTEND - Add TaxRate entity, link to MenuItems

---

## D.5 Tax by product vs service

**Feature ID:** D.5  
**Feature Name:** Tax by product vs service  
**Status:** NOT IMPLEMENTED

**Backend Evidence:**
- Database tables / columns: NO EVIDENCE FOUND
- Domain entities: NO EVIDENCE FOUND
- Services: NO EVIDENCE FOUND
- APIs / handlers: NO EVIDENCE FOUND
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): NO EVIDENCE FOUND
- ViewModels: NO EVIDENCE FOUND
- Navigation path: NO EVIDENCE FOUND
- User-visible workflow: NO EVIDENCE FOUND

**Notes:**
- No distinction between product and service tax
- Table time = service, drinks = product
- Some jurisdictions tax differently

**Risks / Gaps:**
- Non-compliance in some regions
- Over/under taxation

**Recommendation:** EXTEND - Add when multiple tax rates implemented

---

## D.6 Tax exemptions

**Feature ID:** D.6  
**Feature Name:** Tax exemptions  
**Status:** PARTIAL

**Backend Evidence:**
- Database tables / columns: `Tickets.IsTaxExempt`
- Domain entities: `Ticket.IsTaxExempt` property
- Services: `SetTaxExemptCommandHandler.cs`
- APIs / handlers: `SetTaxExemptCommand`
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): Order Entry - tax exempt option
- ViewModels: Tax exempt handling
- Navigation path: Order Entry → Tax Exempt button
- User-visible workflow: Mark ticket tax exempt

**Notes:**
- Ticket-level tax exemption exists
- No customer-level permanent exemption
- No exempt ID/certificate storage

**Risks / Gaps:**
- Cannot store customer tax-exempt status
- No audit trail for exemptions

**Recommendation:** EXTEND - Add customer-level exemption with certificate

---

## D.7 Rounding rules per currency

**Feature ID:** D.7  
**Feature Name:** Rounding rules per currency  
**Status:** NOT IMPLEMENTED

**Backend Evidence:**
- Database tables / columns: NO EVIDENCE FOUND
- Domain entities: `Money.cs` uses decimal precision
- Services: NO EVIDENCE FOUND
- APIs / handlers: NO EVIDENCE FOUND
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): NO EVIDENCE FOUND
- ViewModels: NO EVIDENCE FOUND
- Navigation path: NO EVIDENCE FOUND
- User-visible workflow: NO EVIDENCE FOUND

**Notes:**
- Standard decimal rounding only
- No configurable rounding rules
- Some currencies need specific rules (no pennies)

**Risks / Gaps:**
- May give change that doesn't exist
- Cash handling issues

**Recommendation:** EXTEND - Add rounding configuration

---

## D.8 Financial precision guarantees

**Feature ID:** D.8  
**Feature Name:** Financial precision guarantees  
**Status:** FULL

**Backend Evidence:**
- Database tables / columns: `decimal` types for money columns
- Domain entities: `Money.cs` value object with decimal
- Services: All calculations use Money type
- APIs / handlers: Consistent Money usage
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): Currency formatting consistent
- ViewModels: Money type usage
- Navigation path: All financial displays
- User-visible workflow: Consistent precision

**Notes:**
- Money value object ensures precision
- Decimal type in database
- No floating point for money

**Risks / Gaps:**
- None identified

**Recommendation:** KEEP AS-IS

---

## D.9 End-of-day financial reconciliation

**Feature ID:** D.9  
**Feature Name:** End-of-day reconciliation  
**Status:** PARTIAL

**Backend Evidence:**
- Database tables / columns: `CashSessions`
- Domain entities: `CashSession.CalculateExpectedCash()`
- Services: `GetDrawerPullReportQueryHandler.cs`
- APIs / handlers: `GetDrawerPullReportQuery`
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): `DrawerPullReportDialog.xaml`
- ViewModels: `DrawerPullReportViewModel.cs`
- Navigation path: Manager Functions → Drawer Pull Report
- User-visible workflow: View expected vs actual cash

**Notes:**
- Cash session tracking exists
- Expected cash calculation
- Variance display

**Risks / Gaps:**
- No Z-report equivalent
- No automated reconciliation workflow
- No close-of-business prompts

**Recommendation:** EXTEND - Add Z-report and EOD workflow

---

## Category D COMPLETE

- Features audited: 9
- Fully implemented: 1
- Partially implemented: 4
- Not implemented: 4

# Category C: Billing, Payments & Pricing

## C.1 Real-time billing per table

**Feature ID:** C.1  
**Feature Name:** Real-time billing per table  
**Status:** NOT IMPLEMENTED

**Backend Evidence:**
- Database tables / columns: NO EVIDENCE FOUND (no TableSession with timer)
- Domain entities: NO EVIDENCE FOUND (no time-billing logic)
- Services: NO EVIDENCE FOUND
- APIs / handlers: NO EVIDENCE FOUND
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): NO EVIDENCE FOUND
- ViewModels: NO EVIDENCE FOUND
- Navigation path: NO EVIDENCE FOUND
- User-visible workflow: NO EVIDENCE FOUND

**Notes:**
- No real-time time-based billing exists
- Billing is product/item based only
- No running cost display per table

**Risks / Gaps:**
- Core billiard billing functionality missing
- Cannot show accumulating charges in real-time

**Recommendation:** IMPLEMENT - Create TableSession timer with live billing calculation

---

## C.2 Close now / charge later

**Feature ID:** C.2  
**Feature Name:** Close now / charge later  
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
- No deferred payment concept
- Must pay at time of close
- No "house account" for tables

**Risks / Gaps:**
- Cannot accommodate regular customers
- No credit/tab system for tables

**Recommendation:** IMPLEMENT - Add deferred payment with balance tracking

---

## C.3 Multiple payment methods

**Feature ID:** C.3  
**Feature Name:** Multiple payment methods  
**Status:** FULL

**Backend Evidence:**
- Database tables / columns: `Payments.PaymentType`
- Domain entities: `PaymentType` enum (Cash=0, CreditCard=1, DebitCard=2, GiftCertificate=3, Custom=4)
- Services: `ProcessPaymentCommandHandler.cs`
- APIs / handlers: `ProcessPaymentCommand`
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): `SettlePage.xaml` - payment method buttons
- ViewModels: `SettleViewModel.cs`
- Navigation path: Order Entry → Settle
- User-visible workflow: Select payment type buttons

**Notes:**
- Cash, Credit, Debit, Gift Certificate supported
- Custom payment type available
- Multiple payment types per ticket

**Risks / Gaps:**
- None identified

**Recommendation:** KEEP AS-IS

---

## C.4 Split payments

**Feature ID:** C.4  
**Feature Name:** Split payments  
**Status:** FULL

**Backend Evidence:**
- Database tables / columns: `Payments` - multiple per Ticket
- Domain entities: `Ticket.Payments` collection
- Services: `ProcessPaymentCommandHandler.cs`
- APIs / handlers: `ProcessPaymentCommand`
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): `SettlePage.xaml`
- ViewModels: `SettleViewModel.cs`
- Navigation path: Settle → Partial payment
- User-visible workflow: Can pay partial amounts, multiple methods

**Notes:**
- Multiple payments per ticket supported
- Different payment types combinable
- Running balance tracking

**Risks / Gaps:**
- None identified

**Recommendation:** KEEP AS-IS

---

## C.5 Group billing (multiple tables / players)

**Feature ID:** C.5  
**Feature Name:** Group billing  
**Status:** PARTIAL

**Backend Evidence:**
- Database tables / columns: `GroupSettlements`
- Domain entities: `GroupSettlement.cs`
- Services: `GroupSettleCommandHandler.cs`, `GroupSettleService.cs`
- APIs / handlers: `GroupSettleCommand`
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): `GroupSettleTicketDialog.xaml`, `GroupSettleTicketSelectionWindow.xaml`
- ViewModels: `GroupSettleTicketViewModel.cs`, `GroupSettleTicketSelectionViewModel.cs`
- Navigation path: Manager Functions → Group Settle
- User-visible workflow: Select multiple tickets, settle together

**Notes:**
- Group settlement exists for tickets
- Can settle multiple tickets as one
- Limited to tickets, not table sessions

**Risks / Gaps:**
- Ticket-centric, not player-centric
- No "combine and split among players" workflow

**Recommendation:** EXTEND - Enhance for player-based splitting

---

## C.6 Tips handling

**Feature ID:** C.6  
**Feature Name:** Tips handling  
**Status:** PARTIAL

**Backend Evidence:**
- Database tables / columns: `Payments.TipsAmount`
- Domain entities: `Payment.TipsAmount`, `Gratuity.cs`
- Services: `AddTipsToCardPaymentCommandHandler.cs`
- APIs / handlers: `AddTipsToCardPaymentCommand`
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): `SettlePage.xaml` - tips input
- ViewModels: `SettleViewModel.cs`
- Navigation path: Settle → Tips field
- User-visible workflow: Enter tip amount

**Notes:**
- Tips can be added to card payments
- Gratuity entity exists
- Basic tip handling present

**Risks / Gaps:**
- Tip adjustment post-auth unclear
- Tip pooling/distribution not implemented

**Recommendation:** HARDEN - Add tip adjustment workflow

---

## C.7 Discounts (time-only)

**Feature ID:** C.7  
**Feature Name:** Discounts (time-only)  
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
- No time-based billing = no time-only discounts
- Discount entity exists but is product-based
- Cannot apply "10% off table time"

**Risks / Gaps:**
- Common promotion type not supported
- Cannot discount time separate from consumables

**Recommendation:** IMPLEMENT - When time billing exists, add time-specific discounts

---

## C.8 Discounts (full bill)

**Feature ID:** C.8  
**Feature Name:** Discounts (full bill)  
**Status:** FULL

**Backend Evidence:**
- Database tables / columns: `TicketDiscounts`, `Discounts`
- Domain entities: `Discount.cs`, `TicketDiscount.cs`
- Services: `ApplyDiscountCommandHandler.cs`
- APIs / handlers: `ApplyDiscountCommand`
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): `DiscountTaxPage.xaml` - discount management
- ViewModels: `DiscountTaxViewModel.cs`
- Navigation path: Order Entry → Apply Discount
- User-visible workflow: Apply percentage or fixed discount

**Notes:**
- Ticket-level discounts supported
- Percentage and fixed amount types
- Permission-restricted

**Risks / Gaps:**
- None identified

**Recommendation:** KEEP AS-IS

---

## C.9 Happy Hour / promotional pricing

**Feature ID:** C.9  
**Feature Name:** Happy Hour / promotional pricing  
**Status:** PARTIAL

**Backend Evidence:**
- Database tables / columns: `Discounts.ExpirationDate`
- Domain entities: `Discount.cs` - has expiration
- Services: `ApplyDiscountCommandHandler.cs`
- APIs / handlers: `ApplyDiscountCommand`
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): Discount management
- ViewModels: `DiscountTaxViewModel.cs`
- Navigation path: Admin → Discounts
- User-visible workflow: Create discount with expiration

**Notes:**
- Can create time-limited discounts
- No day-of-week/hour-of-day rules
- No auto-activation based on schedule

**Risks / Gaps:**
- Cannot define "4pm-6pm weekdays" automatically
- Manual activation required

**Recommendation:** EXTEND - Add time-of-day/day-of-week scheduling

---

## C.10 Automatic promotion scheduling

**Feature ID:** C.10  
**Feature Name:** Automatic promotion scheduling  
**Status:** NOT IMPLEMENTED

**Backend Evidence:**
- Database tables / columns: NO EVIDENCE FOUND (no schedule tables)
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
- No scheduled promotions
- No background job for activation/deactivation
- All promotions require manual toggle

**Risks / Gaps:**
- Operator must remember to enable Happy Hour
- Missed promotions = customer complaints

**Recommendation:** IMPLEMENT - Add PromotionSchedule entity and background service

---

## C.11 Manual promotion override

**Feature ID:** C.11  
**Feature Name:** Manual promotion override  
**Status:** PARTIAL

**Backend Evidence:**
- Database tables / columns: `Discounts.IsActive`
- Domain entities: `Discount.Activate()`, `Discount.Deactivate()`
- Services: Discount CRUD
- APIs / handlers: Update discount
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): `DiscountTaxPage.xaml`
- ViewModels: `DiscountTaxViewModel.cs`
- Navigation path: Admin → Discounts → Toggle active
- User-visible workflow: Enable/disable discounts

**Notes:**
- Can toggle discount active status
- No schedule to override per se
- Manual control available

**Risks / Gaps:**
- No "apply anyway" override at checkout
- All or nothing activation

**Recommendation:** EXTEND - Add checkout-level override option

---

## C.12 Price override with permission

**Feature ID:** C.12  
**Feature Name:** Price override with permission  
**Status:** PARTIAL

**Backend Evidence:**
- Database tables / columns: `OrderLines.UnitPrice`
- Domain entities: `OrderLine.cs`
- Services: NO EVIDENCE FOUND for explicit override
- APIs / handlers: NO EVIDENCE FOUND
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): NO EVIDENCE FOUND
- ViewModels: NO EVIDENCE FOUND
- Navigation path: NO EVIDENCE FOUND
- User-visible workflow: NO EVIDENCE FOUND

**Notes:**
- Price is set when item added
- No explicit "override price" UI
- Misc Item can be used as workaround

**Risks / Gaps:**
- Cannot adjust individual item price easily
- No permission check for price changes

**Recommendation:** EXTEND - Add price override dialog with permission

---

## C.13 Price override audit trail

**Feature ID:** C.13  
**Feature Name:** Price override audit trail  
**Status:** NOT IMPLEMENTED

**Backend Evidence:**
- Database tables / columns: `AuditEvents` table exists
- Domain entities: `AuditEvent.cs`
- Services: `AuditEventRepository.cs`
- APIs / handlers: NO EVIDENCE FOUND for price override logging
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): NO EVIDENCE FOUND
- ViewModels: NO EVIDENCE FOUND
- Navigation path: NO EVIDENCE FOUND
- User-visible workflow: NO EVIDENCE FOUND

**Notes:**
- Audit infrastructure exists
- Not wired to price changes
- Would need: who, when, old price, new price, reason

**Risks / Gaps:**
- Cannot track unauthorized discounting
- Fraud detection limited

**Recommendation:** IMPLEMENT - Log price overrides to AuditEvent

---

## C.14 Invoice / ticket printing

**Feature ID:** C.14  
**Feature Name:** Invoice / ticket printing  
**Status:** FULL

**Backend Evidence:**
- Database tables / columns: `PrintTemplates`
- Domain entities: `PrintTemplate.cs`
- Services: `ReceiptPrintService.cs`, `PrintReceiptCommandHandler.cs`
- APIs / handlers: `PrintReceiptCommand`
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): `PrintPage.xaml`, `PrintTemplatesPage.xaml`
- ViewModels: `PrintViewModel.cs`, `PrintTemplatesViewModel.cs`
- Navigation path: Settle → Print Receipt
- User-visible workflow: Print receipt on payment

**Notes:**
- Receipt printing implemented
- Template-based with Liquid engine
- USB/Network printer support

**Risks / Gaps:**
- None identified

**Recommendation:** KEEP AS-IS

---

## C.15 Reprint / void ticket

**Feature ID:** C.15  
**Feature Name:** Reprint / void ticket  
**Status:** PARTIAL

**Backend Evidence:**
- Database tables / columns: `Tickets.Status`
- Domain entities: `Ticket.Void()`
- Services: `VoidTicketCommandHandler.cs`
- APIs / handlers: `VoidTicketCommand`
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): `VoidTicketDialog.xaml`
- ViewModels: `VoidTicketViewModel.cs`
- Navigation path: Manager Functions → Void Ticket
- User-visible workflow: Manager can void ticket

**Notes:**
- Void ticket implemented with reason
- Reprint receipt unclear - need to verify
- Permission-restricted

**Risks / Gaps:**
- Reprint may not be easily accessible
- No "reprint" button on closed tickets

**Recommendation:** EXTEND - Add reprint option for closed tickets

---

## C.16 Cashbox visibility (pending vs paid)

**Feature ID:** C.16  
**Feature Name:** Cashbox visibility  
**Status:** PARTIAL

**Backend Evidence:**
- Database tables / columns: `CashSessions`, `Payments`
- Domain entities: `CashSession.cs`
- Services: `GetCashSessionQueryHandler.cs`, `GetDrawerPullReportQueryHandler.cs`
- APIs / handlers: `GetCurrentCashSessionQuery`, `GetDrawerPullReportQuery`
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): `CashSessionPage.xaml`, `DrawerPullReportDialog.xaml`
- ViewModels: `CashSessionViewModel.cs`, `DrawerPullReportViewModel.cs`
- Navigation path: Manager Functions → Cash Session
- User-visible workflow: View cash session status

**Notes:**
- Cash session tracking exists
- Drawer pull report shows totals
- "Pending" tickets visibility unclear

**Risks / Gaps:**
- No clear "pending orders" vs "completed" breakdown
- Open tickets value may not be visible in cashbox

**Recommendation:** EXTEND - Add pending orders value to cash display

---

## Category C COMPLETE

- Features audited: 16
- Fully implemented: 4
- Partially implemented: 7
- Not implemented: 5

# FloreantPOS Reference System Analysis

## Purpose
This document captures the behaviors, workflows, and features observed in FloreantPOS for use as reference in designing Magidesk POS. This is NOT code to copy, but behavioral patterns to understand and improve upon.

## Core Domain Entities Observed

### Ticket (Order)
**Key Properties:**
- `id`, `globalId`, `createDate`, `closingDate`, `activeDate`, `deliveryDate`
- `paid` (Boolean), `voided` (Boolean), `closed` (Boolean), `refunded` (Boolean), `wasted` (Boolean)
- `subtotalAmount`, `discountAmount`, `taxAmount`, `totalAmount`, `paidAmount`, `dueAmount`
- `advanceAmount`, `adjustmentAmount`, `serviceCharge`, `deliveryCharge`
- `numberOfGuests`, `status` (String - kitchen status), `ticketType` (OrderType name)
- `taxExempt`, `barTab`, `reOpened`, `drawerResetted`
- `shift`, `owner` (User), `assignedDriver`, `gratuity`, `voidedBy`, `terminal`
- `ticketItems` (List), `discounts` (List), `transactions` (Set), `tableNumbers` (List)
- `properties` (Map<String, String>) - flexible key-value storage

**Key Behaviors:**
- `calculatePrice()` - recalculates all totals (subtotal, tax, discount, total, due)
- Supports price-includes-tax mode
- Can have multiple transactions (split payments)
- Can be voided even if already voided (with confirmation)
- Can be reopened (`reOpened` flag)
- Status is string-based (not enum): "Waiting (Kitchen)", "Ready", "Not Sent", "Driving", "Void"
- Supports consolidation of ticket items
- Kitchen printing workflow (needsKitchenPrint, markPrintedToKitchen)
- Supports table assignment (multiple tables per ticket)
- Customer association via properties and customerId
- Delivery address and delivery date tracking

**State Transitions:**
- Created → Open (has items)
- Open → Paid (has transactions totaling ticket amount)
- Paid → Closed (settled)
- Any state → Voided (with reason)
- Closed → Refunded

### TicketItem (OrderLine)
**Key Properties:**
- `id`, `itemId`, `name`, `itemCount`, `itemQuantity` (fractional), `itemUnitName`
- `unitPrice`, `subtotalAmount`, `subtotalAmountWithoutModifiers`
- `discountAmount`, `taxAmount`, `taxAmountWithoutModifiers`
- `totalAmount`, `totalAmountWithoutModifiers`
- `taxRate`, `beverage`, `fractionalUnit`, `treatAsSeat`, `seatNumber`
- `shouldPrintToKitchen`, `printedToKitchen`
- `ticketItemModifiers` (List), `addOns` (List), `cookingInstructions` (List)
- `discounts` (List), `sizeModifier`, `printerGroup`

**Key Behaviors:**
- `calculatePrice()` - recalculates line totals
- Supports modifiers (normal, extra/add-on, info-only)
- Supports pizza-style section modifiers (full, half, quarter)
- Can merge identical items
- Supports fractional quantities (weight-based items)
- Seat-based ordering support
- Kitchen printing per item

### PosTransaction (Payment)
**Key Properties:**
- `id`, `globalId`, `transactionTime`
- `amount`, `tipsAmount`, `tipsExceedAmount`, `tenderAmount`
- `transactionType` (CREDIT/DEBIT), `paymentType` (String - "CASH", "CREDIT CARD", etc.)
- `captured`, `voided`, `authorizable`
- Card fields: `cardHolderName`, `cardNumber`, `cardAuthCode`, `cardType`, `cardTransactionId`, etc.
- Gift cert fields: `giftCertNumber`, `giftCertFaceValue`, `giftCertPaidAmount`, `giftCertCashBackAmount`
- Custom payment fields: `customPaymentName`, `customPaymentRef`, `customPaymentFieldName`
- `drawerResetted`, `note`
- `terminal`, `ticket`, `user`, `reason` (PayoutReason), `recepient` (PayoutRecepient)
- `properties` (Map)

**Payment Types:**
- CASH
- CREDIT_CARD, CREDIT_VISA, CREDIT_MASTER_CARD, CREDIT_AMEX, CREDIT_DISCOVERY
- DEBIT_CARD, DEBIT_VISA, DEBIT_MASTER_CARD
- GIFT_CERTIFICATE
- CUSTOM_PAYMENT

**Transaction Types:**
- CREDIT (money in)
- DEBIT (money out - payouts, refunds, cash drops)

**Key Behaviors:**
- Can be authorizable (credit card pre-auth)
- Can be captured (authorized transaction finalized)
- Can be voided
- Supports tips (tipsAmount, tipsExceedAmount)
- Gift certificates can have cash back
- Custom payments have flexible fields

### CashDrawer
**Key Properties:**
- `id`, `terminal`
- `currencyBalanceList` (Set<CurrencyBalance>)

**Key Behaviors:**
- One drawer per terminal
- Multi-currency support via CurrencyBalance
- No explicit open/close - tied to terminal/shift

### DrawerPullReport (Cash Session Closing)
**Key Properties:**
- `beginCash`, `cashReceiptAmount`, `creditCardReceiptAmount`, `debitCardReceiptAmount`
- `giftCertReturnAmount`, `giftCertChangeAmount`, `cashBack`, `refundAmount`
- `drawerBleedAmount`, `payOutAmount`, `tipsPaid`, `chargedTips`
- `netSales`, `salesTax`, `salesDeliveryCharge`, `totalRevenue`, `grossReceipts`
- `receiptDifferential`, `tipsDifferential`, `drawerAccountable`, `totalVoid`
- `voidTickets` (Set), `currencyBalances` (Set)

**Key Behaviors:**
- `calculate()` - calculates all totals and differentials
- Tracks all cash movements (receipts, payouts, bleeds, refunds)
- Tracks tips (charged vs paid)
- Tracks void tickets
- Multi-currency support

### Gratuity (Tips)
**Key Properties:**
- `id`, `amount`, `paid` (Boolean)
- `ticket`, `terminal`, `owner` (User)

**Key Behaviors:**
- Can be added to tickets
- Can be paid separately from ticket
- Default gratuity percentage support

### Discount
**Key Properties:**
- `id`, `name`, `type`, `value`, `minimunBuy`
- Types: AMOUNT, PERCENTAGE, RE_PRICE, ALT_PRICE
- Qualification: ITEM, ORDER
- Application: FREE_AMOUNT, FIXED_PER_CATEGORY, FIXED_PER_ITEM, FIXED_PER_ORDER, PERCENTAGE_PER_CATEGORY, PERCENTAGE_PER_ITEM, PERCENTAGE_PER_ORDER

**Key Behaviors:**
- Can apply at item or order level
- Multiple calculation methods
- Minimum buy requirements

### TicketDiscount / TicketItemDiscount
**Key Properties:**
- Snapshot of discount at time of application
- `discountId`, `name`, `type`, `value`, `minimumAmount`/`minimumQuantity`
- Applied to ticket or ticket item

**Key Behaviors:**
- Immutable snapshot (discount definition can change, but applied discount doesn't)
- Max discount selection (only one discount applies if multiple eligible)

### Shift
**Key Properties:**
- `id`, `name`, `properties` (JSON)

**Key Behaviors:**
- Defines work periods
- Tickets belong to shifts
- Drawer pulls are per shift

### OrderType
**Key Properties:**
- `id`, `name`, `properties` (JSON)
- Examples: "DINE IN", "TAKE OUT", "PICKUP", "HOME DELIVERY", "DRIVE THRU", "BAR_TAB"

**Key Behaviors:**
- Affects pricing (items can have different prices per order type)
- Affects tax calculation
- Can have properties (e.g., allowSeatBasedOrder, allowToAddTipsLater)
- Bar tabs support authorization/capture workflow

## Key Workflows Observed

### Ticket Lifecycle
1. **Create**: New ticket created with owner, terminal, shift
2. **Add Items**: Add ticket items (menu items with modifiers)
3. **Calculate**: Recalculate totals after any change
4. **Apply Discounts**: Apply item or ticket-level discounts
5. **Open**: Ticket becomes active (can be sent to kitchen)
6. **Settle**: Process payments (can be split across multiple payment types)
7. **Close**: Ticket finalized (paid = true, closed = true)
8. **Void**: Cancel ticket (voided = true, requires reason)
9. **Refund**: Refund closed ticket (refunded = true)

### Payment Processing
1. **Select Payment Type**: Cash, Card, Gift Cert, Custom
2. **Enter Amount**: Can be partial (split payment)
3. **Process**: 
   - Cash: Enter tender, calculate change
   - Card: Swipe/manual entry, authorize/capture
   - Gift Cert: Enter number, calculate cash back if needed
4. **Add Tips**: Can add tips to card transactions
5. **Complete**: Transaction added to ticket
6. **Settle**: When sum of transactions >= ticket total, ticket can be closed

### Cash Drawer Management
1. **Drawer Assignment**: Drawer assigned to terminal/user
2. **Transactions**: All cash transactions affect drawer balance
3. **Drawer Pull**: Generate report at end of shift
   - Calculate expected cash
   - Count actual cash
   - Calculate differential
   - Track payouts, bleeds, refunds
4. **Drawer Reset**: Can reset drawer (drawerResetted flag)

### Refund Workflow
1. **Select Paid Ticket**: Must be paid, not already refunded
2. **Enter Refund Amount**: Cannot exceed paid amount
3. **Process Refund**: Creates refund transaction
4. **Update Ticket**: Sets refunded flag, updates paid amount

### Split Ticket Workflow
1. **Select Items**: Choose items to split
2. **Create New Ticket**: New ticket created with selected items
3. **Update Original**: Original ticket updated (items removed)
4. **Both Active**: Both tickets can be paid separately

## Business Rules Observed

### Ticket Rules
- Cannot settle ticket that's already paid
- Cannot void ticket with payments (must refund instead)
- Can void already-voided ticket (with confirmation)
- Ticket totals recalculated on any change
- Price includes tax mode affects tax calculation
- Tax exempt tickets have zero tax
- Service charge calculated on subtotal (after discounts)
- Delivery charge added to total
- Gratuity added to total
- Adjustment amount can modify total

### Payment Rules
- Multiple payments per ticket allowed (split payment)
- Payment amount can be partial
- Ticket closed when paidAmount >= totalAmount
- Card transactions can be authorized then captured
- Gift certificates can have cash back
- Tips can be added to card transactions
- Tips can exceed transaction amount (tipsExceedAmount)

### Discount Rules
- Only one discount applies (max discount selected)
- Discounts can be item-level or ticket-level
- Discounts have minimum buy requirements
- Discounts are snapshotted when applied
- Tolerance discount is separate from regular discounts

### Cash Drawer Rules
- Drawer accountable = beginCash + cashReceipts - tips - payouts - cashBack - refunds - bleeds
- Receipt differential = grossReceipts - (cash + card + gift cert receipts - cashBack - refunds)
- Tips differential = chargedTips - tipsPaid
- Void tickets tracked separately

## Features to Support (Full POS)

### Core Features
- [x] Ticket creation and management
- [x] Multiple items per ticket
- [x] Item modifiers and add-ons
- [x] Multiple payment types (Cash, Credit, Debit, Gift Cert, Custom)
- [x] Split payments (multiple payments per ticket)
- [x] Tips/Gratuity
- [x] Item-level and ticket-level discounts
- [x] Tax calculation (including tax-exempt)
- [x] Service charges
- [x] Delivery charges
- [x] Ticket voiding
- [x] Refunds
- [x] Ticket splitting
- [x] Cash drawer management
- [x] Drawer pull reports
- [x] Shifts
- [x] Order types
- [x] Table management
- [x] Customer management
- [x] Kitchen printing
- [x] Receipt printing

### Advanced Features
- [ ] Bar tab workflow (authorize/capture)
- [ ] Seat-based ordering
- [ ] Fractional quantities (weight-based)
- [ ] Multi-currency support
- [ ] Price includes tax mode
- [ ] Adjustment amounts
- [ ] Advance payments
- [ ] Driver assignment
- [ ] Delivery tracking
- [ ] Cooking instructions
- [ ] Pizza section modifiers
- [ ] Modifier multipliers
- [ ] Item consolidation
- [ ] Reopen closed tickets

## Behaviors to REJECT/IMPROVE

1. **String-based Status**: Use enum instead of string for ticket status
2. **Multiple Boolean Flags**: Consider state machine pattern for ticket state
3. **Flexible Properties Map**: While useful, ensure type safety where possible
4. **Re-voiding Tickets**: Should this be allowed? Need business justification
5. **Price Calculation Complexity**: Simplify calculation logic, make it more testable
6. **No Explicit Cash Session**: Drawer pulls are implicit - make cash sessions explicit
7. **Drawer Reset Flag**: Unclear workflow - clarify or remove

## Next Steps

1. Revise domain model based on full feature set
2. Design proper state machines for tickets and payments
3. Create comprehensive invariants
4. Design for all payment types from start
5. Support split payments architecture
6. Design audit trail for all operations
7. Plan implementation phases (can still be incremental, but architecture supports all features)


# Feature: Payment keypad + tender types

## Classification
- **Parity classification**: PARITY REQUIRED
- **MagiDesk status**: UNCERTAIN (not assessed yet)

## Problem / Why this exists (grounded)
- **Operational need**: Provide an in-POS payment terminal UI with quick tender entry, multiple payment types, multi-currency cash, gratuity, coupon/discount, printing, and a drawer “NO SALE” function.
- **Evidence**: `PaymentView` builds keypad + quick amounts and routes to `SettleTicketProcessor.doSettle(...)` by `PaymentType`.

## User-facing surfaces
- **Surface type**: Panel / view (embedded in `SettleTicketDialog` and retail mode `OrderView`)
- **Controls observed**:
  - Due amount display + tendered amount input
  - Keypad digits + dot + clear
  - Quick add buttons: 1,2,5,10,20,50,100, Exact Amount, Next Amount, NO SALE
  - Tender actions: Cash, Multi Currency Cash (optional), Credit Card, Debit Card, Gift, Other (custom)
  - Gratuity, Coupon/Discount, Print Ticket
  - Cancel

## Preconditions & protections
- **Ticket empty guard for cash**: `doPayByCash()` errors if ticket null or no items.
- **Multi-currency gating**: Multi-currency cash button is only visible when `TerminalConfig.isEnabledMultiCurrency()`.

## Step-by-step behavior (forensic)
- **UpdateView**: sets due and tendered to the current due amount.
- **Numeric keypad**: appends digits/dot into `txtTenderedAmount` with some clearing behavior (`clearPreviousAmount`).
- **Quick buttons**:
  - Exact Amount: sets tendered = due.
  - Next Amount: rounds due up to next integer (`Math.ceil(dd)`) formatted as `##.00`.
  - NO SALE: `DrawerUtil.kickDrawer()`.
  - Other amount buttons: add their amount to existing tendered.
- **Tender actions**:
  - Cash: parses tendered and calls `ticketProcessor.doSettle(PaymentType.CASH, amount)`.
  - Credit/debit/gift/other: call `ticketProcessor.doSettle(<type>, getTenderedAmount())` with stale-state reload behavior for some.
  - Multi-currency cash: opens `MultiCurrencyTenderDialog`, may persist `CashDrawer` via `TerminalDAO.performBatchSave(cashDrawer)`, then settles cash.
- **Gratuity**: delegates to `ticketProcessor.doSetGratuity()`.
- **Coupon/discount**: delegates to `ticketProcessor.doApplyCoupon()`.
- **Print**: prints ticket; if order type allows tips later and gratuity <= 0 prints with “add tips later” flag.

## Edge cases & failure paths
- **Stale state**: Credit/debit/cash show reload dialog via `POSMessageDialog.showMessageDialogWithReloadButton`.
- **Parse errors**: Some paths catch generic exception and log; exact user-visible error handling varies by button.

## Data / audit / financial impact
- **Writes/updates**:
  - Payment settlement creates transactions and sets ticket paid/closed downstream (in `SettleTicketProcessor` and related processors).
  - Multi-currency can persist cash drawer balances.
- **Financial risk**: “NO SALE” drawer kick is security-sensitive; incorrect gating or logging can create shrink risk.

## Code traceability (REQUIRED)
- **Primary UI**: `com.floreantpos.ui.views.payment.PaymentView` → `/projects/Code/Redesign-POS/floreantpos/src/com/floreantpos/ui/views/payment/PaymentView.java`
- **Processors**: `SettleTicketProcessor` (not inspected fully here)
- **Dialogs**: `MultiCurrencyTenderDialog`
- **Utilities**: `DrawerUtil.kickDrawer`, `TerminalConfig.isEnabledMultiCurrency`

## Uncertainties (STOP; do not guess)
- **Authorization flows**: Card authorization/capture/void flows likely live in processors and gateway plugins; not proven from `PaymentView` alone.

## MagiDesk parity notes
- **What exists today**: Not yet assessed.
- **What differs / missing**: Not yet assessed.

## Porting strategy (PLAN ONLY)
- **Backend requirements**:
  - Settlement orchestration by payment type
  - Drawer kick integration and audit logging requirements
  - Multi-currency tendering + drawer-balance persistence
- **UI requirements**:
  - Keypad + quick amount behaviors must match (exact amount, next amount rounding, clear semantics)
- **Constraints for implementers**:
  - Preserve NO SALE behavior (drawer kick) as a distinct action path.
  - Preserve Next Amount rounding semantics (`ceil(due)` and format).

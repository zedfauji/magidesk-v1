# Workflow Gaps
**Audit Date:** 2026-01-01

## 1. Split Payment Workflow
- **FloreantPOS:** "Settle Ticket" -> Select Payment Method -> Enter Amount -> If Partial, System *locks* ticket in "Partially Paid" state and prompts for next tender immediately or allows saving.
- **Magidesk:** `SettlePage` allows adding partial payments. The gap is in the *guidance*. Floreant guides the user "Due: $X.XX" -> "Tendered: $Y.YY" -> "Remaining: $Z.ZZ". Parity requires verifying if Magidesk's `PaymentViewModel` enforces the *completion* of the loop or allows leaving a ticket in a "zombie" partially paid state without clear indicator.

## 2. Shift & End-of-Day
- **FloreantPOS:** Rigid `Clock Out` -> `Drawer Pull` -> `Shift End` sequence. Often blocks Clock Out if open tickets exist.
- **Magidesk:** `CashSession` and `Shift` seem decoupled. `CloseCashSessionCommandHandler` and `UpdateShiftCommandHandler` exist. The "Guard" that prevents closing a shift with open orders (critical preventing data drift) needs verification in `CloseCashSessionCommandHandler`.

## 3. Table Booking / Reservations
- **FloreantPOS:** `TableBookingInfo` allows marking tables as "Reserved" for specific times, preventing walk-in seating.
- **Magidesk:** **MISSING**. No Reservation workflow exists. Tables are either Free or Occupied.

## 4. Refund Authentication
- **FloreantPOS:** Refund often requires Manager Approval *step* (Manager Password Dialog) embedded in the flow.
- **Magidesk:** `RefundPaymentCommand` exists. The *workflow gap* is the authentication intercept. Does Magidesk trigger `PasswordEntryDialog` automatically via a decorator or middleware? If not, unchecked refunds are a security hole.

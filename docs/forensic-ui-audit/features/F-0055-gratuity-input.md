# Feature: Gratuity Input (F-0055)

## Classification
- **Parity classification**: PARITY REQUIRED
- **MagiDesk status**: PARTIAL (Gratuity exists but input dialog may differ)

## Problem / Why this exists (grounded)
- **Operational need**: Servers need to add tips/gratuity to tickets. This can happen at payment time or after (for bar tabs, credit cards with tip line).
- **Evidence**: `GratuityInputDialog.java` - numeric keypad for gratuity amount entry. Called from PaymentView when adding tips.

## User-facing surfaces
- **Surface type**: Modal dialog
- **UI entry points**: PaymentView → Add Gratuity button; also from settle flow for tip-later order types
- **Exit paths**: OK (applies gratuity) / Cancel (no gratuity added)

## Preconditions & protections
- **User/role/permission checks**: Standard payment permissions; may require owner of ticket
- **State checks**: Ticket must exist; order type must allow gratuity (checked via `OrderType.isAllowToAddTipsLater()`)
- **Manager override**: May be required for excessive tips (tipsExceedAmount handling in FloreantPOS)

## Step-by-step behavior (forensic)
1. User initiates gratuity entry (from PaymentView or settle dialog)
2. GratuityInputDialog opens with numeric keypad
3. User enters gratuity amount using keypad
4. Amount displayed in text field (right-aligned, bold, large font)
5. On OK: Gratuity amount returned to caller
6. Gratuity applied to ticket/transaction
7. If order type allows tips later, receipt prints with tip line

## Edge cases & failure paths
- **Zero gratuity**: Allowed (user may cancel instead)
- **Negative amount**: Prevented by DoubleTextField
- **Tips exceeding transaction amount**: Tracked separately as tipsExceedAmount
- **Tips on void/refund**: Must be handled separately (VoidTicketDialog has tips refund option)

## Data / audit / financial impact
- **Writes/updates**: Ticket.gratuity (Gratuity entity with amount, paid status, owner, terminal)
- **Audit events**: Gratuity added to ticket logged
- **Financial risk**: Tip skimming; tips not paid out to servers; tip pooling calculations

## Code traceability (REQUIRED)
- **Primary UI class(es)**: `GratuityInputDialog` → `ui/views/payment/GratuityInputDialog.java`
- **Entry action(s)**: Called from PaymentView, SettleTicketDialog
- **Workflow/service enforcement**: Gratuity entity persisted; GratuityDAO for queries
- **Messages/labels**: `GratuityInputDialog.0` (title)

## Uncertainties (STOP; do not guess)
- Default gratuity percentage calculation (mentioned in analysis but not in this dialog)
- Tip pooling rules (business policy, not in code)

## MagiDesk parity notes
- **What exists today**: Gratuity entity and basic support in Ticket model
- **What differs / missing**: GratuityInputDialog not implemented; tip-later receipt flow not verified

## Porting strategy (PLAN ONLY)
- **Backend requirements**: SetGratuityCommand; validate gratuity amount; Gratuity entity tracking
- **API/DTO requirements**: AddGratuityRequest with ticketId, amount
- **UI requirements**: NumericKeypad dialog for gratuity entry; integration with payment flow
- **Constraints for implementers**: Gratuity must be auditable; cannot modify after ticket closed

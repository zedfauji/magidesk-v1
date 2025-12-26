# Feature: Swipe card input dialog (with manual entry + auth-code entry options)

## Classification
- **Parity classification**: PARITY REQUIRED
- **MagiDesk status**: UNCERTAIN (not assessed yet)

## Problem / Why this exists (grounded)
- **Operational need**: Capture card input via magnetic swipe (raw track data), with fallback paths for manual entry and external-terminal authorization code entry based on configuration.
- **Evidence**: Provides Manual Entry and “Enter Authorization Code” buttons whose enabled state is driven by `CardConfig`.

## User-facing surfaces
- **Surface type**: Dialog
- **UI entry points**: Not proven here; invoked by payment processors via a `CardInputListener`.
- **Controls observed**:
  - Password field used to capture swipe string
  - Manual Entry button (opens `ManualCardEntryDialog`)
  - Enter Authorization Code button (opens `AuthorizationCodeDialog`)
  - Submit (submits swipe)
  - Cancel

## Preconditions & protections
- **Config gating**:
  - Manual entry disabled if `!CardConfig.isManualEntrySupported()`.
  - Authorization code entry disabled if `!CardConfig.isExtTerminalSupported()`.
- **Development mode**:
  - In development mode, pre-fills the field with a sample swipe string.

## Step-by-step behavior (forensic)
1. User swipes card into the field (or types).
2. Submit (or Enter key) reads `cardString = new String(passwordField.getPassword())`.
3. Dialog sets `canceled=false`, disposes, and calls `cardInputListener.cardInputted(this, PaymentType.CREDIT_CARD)`.
4. Manual Entry path disposes and opens `ManualCardEntryDialog`.
5. Authorization Code path disposes and opens `AuthorizationCodeDialog`.

## Data / audit / financial impact
- **Security**: Raw card swipe strings are highly sensitive; any logging/storage behavior must be verified in processors (not proven here).

## Code traceability (REQUIRED)
- **Primary UI**: `com.floreantpos.ui.views.payment.SwipeCardDialog` → `/projects/Code/Redesign-POS/floreantpos/src/com/floreantpos/ui/views/payment/SwipeCardDialog.java`
- **Config**: `com.floreantpos.config.CardConfig`
- **Follow-on dialogs**: `ManualCardEntryDialog`, `AuthorizationCodeDialog`

## Uncertainties (STOP; do not guess)
- **PCI handling**: This UI captures raw track data; downstream handling must be audited separately.

## MagiDesk parity notes
- **What exists today**: Not yet assessed.

## Porting strategy (PLAN ONLY)
- **UI requirements**: Swipe capture dialog with controlled alternative entry methods.
- **Constraints for implementers**: Preserve config-driven availability of manual vs auth-code entry.

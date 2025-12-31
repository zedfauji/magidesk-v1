# Navigation Recommendations (NO IMPLEMENTATION)

**Goal**: Move Magidesk navigation closer to FloreantPOS patterns (RootView-centered operator flows + guarded Back Office).

## Immediate (high priority)
- **[REC-001] Define a single operator “home” and make it the primary navigation hub**
  - **Recommendation**: treat `SwitchboardPage` as the canonical home (analogous to Floreant switchboard/home).
  - **Rationale**: aligns with Floreant’s home-first operator model.

- **[REC-002] Reduce root sidebar surface area to operator-safe items**
  - **Recommendation**: restrict MainWindow sidebar to operator-safe navigation:
    - Home/Switchboard
    - Table Map
    - Kitchen Display (if operator-facing)
    - (Optional) Cash Session if required by workflow
  - **Recommendation**: move administrative/reporting items behind a manager/back office entry point.
  - **Rationale**: Floreant does not expose broad admin tooling at the same level as operational navigation.

- **[REC-003] Establish a Floreant-like “default view” routing policy**
  - **Recommendation**: define and document an equivalent to Floreant `TerminalConfig.getDefaultView()` routing (even if implementation is later).
  - **Expected behaviors (baseline-aligned)**:
    - Default to Switchboard/Home
    - Allow default to Kitchen Display
    - Allow default to order-type-driven entry flow (table map/customer/quick ticket)
  - **Rationale**: enables consistent terminal behavior and reduces operator clicks.

- **[REC-004] Back office should be clearly separated and guarded**
  - **Recommendation**: treat Back Office as a distinct navigation context (Floreant uses a separate window).
  - **At minimum (documentation-level recommendation)**:
    - Back Office entry only from manager flow
    - Role/permission gating should be required (baseline expectation)
    - Back Office menu items should be conditioned by permission
  - **Rationale**: closer to Floreant’s model and reduces risk of accidental admin access.

## Near-term (medium priority)
- **[REC-005] Resolve unreachable navigation actions**
  - **Recommendation**: either add an explicit UI entry (if intended) or remove the dead code path (if not intended) for:
    - OpenTicketsList: Split action exists in ViewModel but no UI binding.
  - **Rationale**: prevents “phantom” capabilities that can’t be accessed.

- **[REC-006] Remove or quarantine orphan/variant UI assets**
  - **Recommendation**: decide whether these are intended:
    - `MainPage`
    - `PasswordEntryDialog`
    - `SplitTicketDialog_Backup/_Fixed/_Minimal`
    - `Views/Dialogs/CashEntryDialog` (Dialogs folder version)
  - **Rationale**: reduces audit ambiguity and future navigation drift.

- **[REC-007] Align “Open Tickets” behavior with Floreant expectations**
  - **Recommendation**: ensure Open Tickets entry provides operator-appropriate actions:
    - Resume
    - Transfer (manager-only)
    - Void (manager-only)
    - Split (if intended)
  - **Rationale**: Floreant separates operator vs privileged actions.

## Longer-term (lower priority)
- **[REC-008] Standardize navigation primitives**
  - **Recommendation**: document where each primitive is allowed:
    - Page navigation (`Navigate`) for primary flows
    - Dialogs for short, blocking decisions
    - Avoid mixing debug/admin pages into operator routing
  - **Rationale**: reduces navigation traps and keeps behavior closer to RootView semantics.

- **[REC-009] Define explicit parity targets per major Floreant flow**
  - **Recommendation**: adopt a written parity target set:
    - Login → Default view routing
    - Home/Switchboard
    - Order type rules (table/customer/guest count)
    - Kitchen Display
    - Back Office window/context
    - Logout teardown
  - **Rationale**: keeps parity work measurable.

## Status
- **No code changes proposed here.** This file is recommendations-only.

# High-Level Implementation Plan

## Phase 1: Stabilization
- **Feature groups**
  - Runtime context (current user, terminal, cash session, shift)
  - Dialog safety and error surfacing
  - Deterministic No Sale/open drawer behavior
- **Dependencies**
  - `IUserService` (or equivalent) must be authoritative
  - `ISystemInitializationService` must provide terminal/session identity
- **Risk notes**
  - Hardcoded IDs and simulated flows can produce silent data corruption.

## Phase 2: Core Parity
- **Feature groups**
  - Switchboard -> New Ticket -> Order Entry -> Send to Kitchen -> Settle -> Close
  - Modifiers (required group enforcement), pizza modifiers, combos
  - Ticket resume/edit and core ticket list behaviors
- **Dependencies**
  - Ticket lifecycle state machine must be consistent across UI and backend
  - Kitchen routing must be reliable (even if print-only initially)
- **Risk notes**
  - Partial modifier enforcement changes totals/taxes and impacts reporting.

## Phase 3: Operational Parity
- **Feature groups**
  - Cash management: cash drops, payouts, drawer bleed, drawer pull report
  - Table workflows: table selection, change/release table
  - Manager workflows: void/refund/transfer
- **Dependencies**
  - Cash session accounting model must be authoritative
  - Permission gating must be enforceable (manager override)
- **Risk notes**
  - Financial workflows require audit trail consistency.

## Phase 4: Advanced / Optional
- **Feature groups**
  - Full KDS state machine and kitchen display parity
  - Back office completeness (printer config/routing, deep reports)
  - Hardware integrations beyond baseline (barcode scanning, integrated gateways)
- **Dependencies**
  - Peripheral abstraction + test harness
- **Risk notes**
  - Hardware variability increases support surface; keep behind interfaces and mocks.

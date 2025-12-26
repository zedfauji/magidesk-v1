# Phase 3: Implementation Roadmap

## Overview
This roadmap defines the execution order for implementing the "Missing" backend subsystems identified in the Forensic Audit and Detailed in TDD-001 through TDD-004.

## Phase 3.1: Foundation (Entities & Database)
*Goal: Establish the data structures required for the new subsystems.*
- [ ] **Migration 1**: Create `KitchenOrder` and `KitchenRouting` tables (TDD-001).
- [ ] **Migration 2**: Create `DrawerAssignment` and `TerminalTransaction` tables (TDD-002).
- [ ] **Migration 3**: Create `PaymentBatch` and `GroupSettlement` tables (TDD-003).
- [ ] **Migration 4**: Update Menu Schema for `FractonalModifier` and `Combos` (TDD-004).

## Phase 3.2: Core Services (Business Logic)
*Goal: Implement the services logic sans UI.*
- [ ] **KDS**: Implement `KitchenRoutingService` and `KitchenStatusService`.
- [ ] **Cash**: Implement `DrawerService` (Open, Close, Calculate Balance).
- [ ] **Batch**: Implement `GroupSettleService` (Atomic Split Logic).
- [ ] **Menu**: Update `PriceCalculator` for Pizza/Combo logic.

## Phase 3.3: API & Integration
*Goal: Expose logic to the Frontend.*
- [ ] Create REST/WebSocket endpoints for KDS.
- [ ] Create Endpoints for Drawer Mgmt.
- [ ] Update Order Entry API to handle Combos.

## Phase 3.4: Verification
*Goal: Ensure parity with Forensic Audit requirements.*
- [ ] Test KDS "Bump" flow.
- [ ] Test Drawer "Blind Close" variance calculation.
- [ ] Test Group Payment splitting.

## Execution Order
1.  **Database Schemas (All)** - Unblock all data layers.
2.  **Cash Management** - Critical for financial integrity (High Risk).
3.  **KDS** - Critical for operations (High Value).
4.  **Batch/Group** - Enhances workflow (High Value).
5.  **Complex Menu** - Enhances selling (Medium Value, Product dependent).

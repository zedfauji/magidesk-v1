# Table System Navigation Flows

## 1. Table Map -> Order Entry
- **Trigger**: Click Table (Available).
- **Process**:
  1. `TableMapViewModel` creates Ticket (with Table #).
  2. Navigates to `OrderEntryPage` with `TicketId`.
- **Outcome**:
  - Ticket created.
  - Table # attached to ticket.
  - **BUT**: Table Entity Status NOT updated (Sync Bug).
- **Return Path**: "Back" / "Close" -> Returns to Map.

## 2. Table Explorer -> Order Entry
- **Trigger**: Click "Select" on Table.
- **Process**:
  1. Navigates to `OrderEntryPage` with `TicketId = null`.
  2. `OrderEntryViewModel.InitializeAsync` sees null.
  3. Calls `CreateNewTicketAsync`.
- **Outcome**:
  - **FAILURE**: Ticket created with **NO TABLE NUMBER**.
  - The intent of selecting "Table 5" is completely lost.
  - User is creating a "Walk-In" ticket.

## 3. Order Entry -> Return
- **Trigger**: "Close" button.
- **Logic**: Checks `FromTableMap` flag.
- **Outcome**:
  - If from Map: Goes to Map.
  - If from Explorer: Goes to Map (because Explorer sets flag=true).
  - **UX Defect**: User expects to return to Explorer list.

## 4. Designer -> Map
- **Trigger**: No direct link.
- **Process**: Global Navigation (Menu).
- **Outcome**: Safe, but relies on global state.

## 5. Summary of Navigation Risks
- **Critical**: Explorer flow is deceptive. Users think they are assigning a table but are not.
- **Medium**: Return path from Explorer is hardcoded to Map.

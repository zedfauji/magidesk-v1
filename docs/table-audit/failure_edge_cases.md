# Table System Failure & Edge Cases

## 1. Map Topology & Real-time Desync
- **Scenario**: Manager adds, moves, or deletes a table in Designer while floor staff has Map open.
- **Behavior**:
  - Map polling **only updates Status**.
  - **Added Tables**: Do not appear.
  - **Deleted Tables**: Remain visible (Zombie tables). Clicking them typically fails or crashes.
  - **Moved Tables**: Remain at old coordinates.
- **Risk**: Operational confusion. Staff directs guests to a table that doesn't exist or is elsewhere.

## 2. Deleting Occupied Tables
- **Protection**: `DeleteTableAsync` checks `Status != Available`.
- **Paradox**: Designer **filters out** occupied tables (see Consistency Gaps).
- **Result**: User cannot select an occupied table to delete it.
- **Edge Case (State Corruption)**:
  - If Sync Bug (Trace Phase) leaves Table "Available" but Ticket thinks it has the table.
  - Designer SHOWS table (Status=Available).
  - User can DELETE it.
  - **Result**: Ticket refers to non-existent table. Future queries by Table Number fail.

## 3. Off-Canvas Tables
- **Scenario**: Designer places table at X=2100.
- **Map**: Hardcoded canvas size 2000x2000 clips the table.
- **Result**: Table exists but is invisible to staff.

## 4. Unsaved Work Loss
- **Scenario**: User edits layout in Designer. Accidental click on "Reload" or navigation away.
- **Behavior**: No "Unsaved Changes" confirmation.
- **Result**: Immediate data loss.

## 5. Concurrent Ticket Creation
- **Scenario**: Two waiters click "Table 5" simultaneously on Map.
- **Behavior**:
  - Both ViewModels execute `CreateTicketCommand`.
  - Both create NEW tickets for Table 5.
  - **Race Condition**: `AssignTicket` forces one-to-one, but `CreateTicket` (currently) does **not** check/set Table status correctly.
  - **Result**: Two tickets created for same table. Table Status remains "Available" (due to Sync Bug). Infinite tickets can be created for one table.

## 6. Drag & Drop Network Failure
- **Scenario**: User drags table. Network fails during `UpdateTablePositionAsync`.
- **Code-Behind**: Updates UI X/Y immediately.
- **ViewModel**: Sends command. If fails, `ShowErrorAsync`.
- **Reversion**: UI does **not** snap back to original position on failure.
- **Result**: UI shows new position, DB has old position. Next reload causes "jump".

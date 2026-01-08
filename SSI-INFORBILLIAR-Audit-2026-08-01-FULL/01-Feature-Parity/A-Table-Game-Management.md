# Category A: Table & Game Management

## A.1 Open table session

**Feature ID:** A.1  
**Feature Name:** Open table session  
**Status:** PARTIAL

**Backend Evidence:**
- Database tables / columns: `Tables` (Id, TableNumber, Status, CurrentTicketId), `Tickets` (Id, Status, CreatedAt)
- Domain entities: `Table.cs`, `Ticket.cs`
- Services: `CreateTicketCommandHandler.cs`, `AssignTableToTicketCommandHandler.cs`
- APIs / handlers: `CreateTicketCommand`, `AssignTableToTicketCommand`
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): `TableMapPage.xaml`, `SwitchboardPage.xaml`
- ViewModels: `TableMapViewModel.cs`, `SwitchboardViewModel.cs`
- Navigation path: Login → Switchboard → Table Map → Select Table → Order Entry
- User-visible workflow: User clicks table on map, creates ticket

**Notes:**
- This is a restaurant-style "seat table" workflow, NOT a billiard "start timer" workflow
- No time-based session concept exists
- Table assignment is ticket-based, not session-based

**Risks / Gaps:**
- No concept of "table session" separate from "ticket"
- No timer starts on table open
- No session entity to track duration

**Recommendation:** REDESIGN - Implement `TableSession` entity with StartTime, EndTime, Status

---

## A.2 Close table session

**Feature ID:** A.2  
**Feature Name:** Close table session  
**Status:** PARTIAL

**Backend Evidence:**
- Database tables / columns: `Tables.Status`, `Tickets.Status`, `Tickets.ClosedAt`
- Domain entities: `Table.ReleaseTicket()`, `Ticket.Close()`
- Services: `CloseTicketCommandHandler.cs`, `ReleaseTableCommandHandler.cs`
- APIs / handlers: `CloseTicketCommand`, `ReleaseTableCommand`
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): `SettlePage.xaml`
- ViewModels: `SettleViewModel.cs`
- Navigation path: Order Entry → Settle → Payment → Close
- User-visible workflow: Complete payment, release table

**Notes:**
- Closing is payment-driven, not time-driven
- No "close session without payment" option
- No session duration capture at close

**Risks / Gaps:**
- No session close timestamp storage
- No session total time calculation
- Table release tied to ticket close only

**Recommendation:** REDESIGN - Add session close workflow with duration calculation

---

## A.3 Per-table time tracking

**Feature ID:** A.3  
**Feature Name:** Per-table time tracking  
**Status:** NOT IMPLEMENTED

**Backend Evidence:**
- Database tables / columns: NO EVIDENCE FOUND
- Domain entities: NO EVIDENCE FOUND
- Services: NO EVIDENCE FOUND
- APIs / handlers: NO EVIDENCE FOUND
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): NO EVIDENCE FOUND
- ViewModels: NO EVIDENCE FOUND
- Navigation path: NO EVIDENCE FOUND
- User-visible workflow: NO EVIDENCE FOUND

**Notes:**
- No timer entity exists
- No real-time tracking mechanism
- No elapsed time display anywhere in UI

**Risks / Gaps:**
- Core billiard functionality missing
- Cannot bill by time
- Cannot display elapsed time to operator

**Recommendation:** IMPLEMENT - Create `TableSession` with timer, background service for live updates

---

## A.4 Color-coded table states (idle / active / paused / closed)

**Feature ID:** A.4  
**Feature Name:** Color-coded table states  
**Status:** PARTIAL

**Backend Evidence:**
- Database tables / columns: `Tables.Status`
- Domain entities: `TableStatus.cs` enum (Available=0, Seat=1, Booked=2, Dirty=3, Disable=4)
- Services: NO EVIDENCE FOUND
- APIs / handlers: NO EVIDENCE FOUND
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): `TableMapPage.xaml` uses status-based coloring
- ViewModels: `TableMapViewModel.cs`
- Navigation path: Switchboard → Table Map
- User-visible workflow: Tables display with status colors

**Notes:**
- Available = Green, Seat = Blue, Booked = Yellow, Dirty = Orange, Disable = Gray (typical restaurant colors)
- No "Paused" state exists
- States are restaurant-oriented, not billiard-oriented

**Risks / Gaps:**
- Missing "Paused" state for table breaks
- No "Active" with timer indication
- Color scheme not optimized for game status

**Recommendation:** EXTEND - Add Paused state, adjust colors for billiard context

---

## A.5 Support for multiple table types (pool, carambola, domino, etc.)

**Feature ID:** A.5  
**Feature Name:** Multiple table types  
**Status:** NOT IMPLEMENTED

**Backend Evidence:**
- Database tables / columns: `Tables.Shape` (Rectangle, Circle, Diamond, Square) - visual only
- Domain entities: `TableShapeType.cs` - visual geometry only
- Services: NO EVIDENCE FOUND
- APIs / handlers: NO EVIDENCE FOUND
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): `TableDesignerPage.xaml` - shape selection
- ViewModels: `TableDesignerViewModel.cs`
- Navigation path: Admin → Table Designer
- User-visible workflow: Can set visual shape only

**Notes:**
- No "Table Type" concept (Pool Table, Snooker, Domino)
- No type-based pricing
- Shape is purely visual, not business logic

**Risks / Gaps:**
- Cannot categorize tables by game type
- Cannot apply different pricing per type
- Cannot filter/report by table type

**Recommendation:** IMPLEMENT - Add `TableType` entity with Name, BasePricePerHour

---

## A.6 Ability to manage 1–256+ tables

**Feature ID:** A.6  
**Feature Name:** Scalability (1-256+ tables)  
**Status:** PARTIAL

**Backend Evidence:**
- Database tables / columns: `Tables` - no hard limit
- Domain entities: `Table.cs`, `TableLayout.cs`
- Services: `TableLayoutRepository.cs`
- APIs / handlers: CRUD commands exist
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): `TableDesignerPage.xaml`, `TableMapPage.xaml`
- ViewModels: `TableDesignerViewModel.cs`
- Navigation path: Admin → Table Designer
- User-visible workflow: Can add tables

**Notes:**
- No hard-coded limit in database
- UI/UX not tested for 256+ tables
- Canvas may have performance issues at scale

**Risks / Gaps:**
- No pagination on table map
- Floor switching required for scale
- Performance unknown at 100+ tables

**Recommendation:** HARDEN - Add floor-based organization, test performance at scale

---

## A.7 Move an active session between tables

**Feature ID:** A.7  
**Feature Name:** Move session between tables  
**Status:** PARTIAL

**Backend Evidence:**
- Database tables / columns: `Tickets.TableNumbers` (string), `Tables.CurrentTicketId`
- Domain entities: `Ticket.AddTableNumber()`, `Table.AssignTicket()`, `Table.ReleaseTicket()`
- Services: `TransferTicketToTableCommandHandler.cs`
- APIs / handlers: `TransferTicketToTableCommand`
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): `OrderEntryPage.xaml` - "Change Table" button
- ViewModels: `OrderEntryViewModel.cs`
- Navigation path: Order Entry → Change Table → Select New Table
- User-visible workflow: Button exists, navigates to table map

**Notes:**
- Ticket-based transfer, not session-based
- No time continuity guarantee
- Change Table button visible in Order Entry

**Risks / Gaps:**
- If session/timer exists, transfer may break continuity
- No audit trail of table moves
- No "swap tables" (mutual exchange) feature

**Recommendation:** EXTEND - Ensure session timer continuity, add audit logging

---

## A.8 Move consumption between tables

**Feature ID:** A.8  
**Feature Name:** Move consumption between tables  
**Status:** PARTIAL

**Backend Evidence:**
- Database tables / columns: `OrderLines.TicketId`
- Domain entities: `Ticket.AddOrderLine()`, `Ticket.RemoveOrderLine()`
- Services: `SplitTicketCommandHandler.cs`
- APIs / handlers: `SplitTicketCommand`
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): `SplitTicketDialog.xaml`
- ViewModels: `SplitTicketViewModel.cs`
- Navigation path: Manager Functions → Split Ticket
- User-visible workflow: Can split items to new ticket

**Notes:**
- Split ticket moves items to NEW ticket
- No direct "move to existing table" workflow
- Requires close coordination with table assignment

**Risks / Gaps:**
- Cannot move single items to existing active table
- Split creates new ticket, not moves to existing
- UX requires manual table assignment after split

**Recommendation:** EXTEND - Add "Move Items to Table" direct workflow

---

## A.9 Different pricing per table type

**Feature ID:** A.9  
**Feature Name:** Pricing per table type  
**Status:** NOT IMPLEMENTED

**Backend Evidence:**
- Database tables / columns: NO EVIDENCE FOUND
- Domain entities: NO EVIDENCE FOUND
- Services: NO EVIDENCE FOUND
- APIs / handlers: NO EVIDENCE FOUND
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): NO EVIDENCE FOUND
- ViewModels: NO EVIDENCE FOUND
- Navigation path: NO EVIDENCE FOUND
- User-visible workflow: NO EVIDENCE FOUND

**Notes:**
- No TableType entity exists
- No pricing rules per table
- All tables priced identically (via menu item selection)

**Risks / Gaps:**
- Cannot charge premium for VIP tables
- Cannot differentiate pool vs snooker pricing
- Requires manual item selection per table type

**Recommendation:** IMPLEMENT - Add TableType with PricePerHour, link to pricing engine

---

## A.10 First-hour pricing rules

**Feature ID:** A.10  
**Feature Name:** First-hour pricing rules  
**Status:** NOT IMPLEMENTED

**Backend Evidence:**
- Database tables / columns: NO EVIDENCE FOUND
- Domain entities: NO EVIDENCE FOUND
- Services: NO EVIDENCE FOUND
- APIs / handlers: NO EVIDENCE FOUND
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): NO EVIDENCE FOUND
- ViewModels: NO EVIDENCE FOUND
- Navigation path: NO EVIDENCE FOUND
- User-visible workflow: NO EVIDENCE FOUND

**Notes:**
- No time-based pricing engine
- No concept of "first hour" vs "subsequent hours"
- All pricing is product/item based

**Risks / Gaps:**
- Cannot implement common billiard pricing model
- First hour flat rate + subsequent per-minute billing not possible

**Recommendation:** IMPLEMENT - Create PricingRule engine with time-tier support

---

## A.11 Partial-hour billing rules

**Feature ID:** A.11  
**Feature Name:** Partial-hour billing rules  
**Status:** NOT IMPLEMENTED

**Backend Evidence:**
- Database tables / columns: NO EVIDENCE FOUND
- Domain entities: NO EVIDENCE FOUND
- Services: NO EVIDENCE FOUND
- APIs / handlers: NO EVIDENCE FOUND
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): NO EVIDENCE FOUND
- ViewModels: NO EVIDENCE FOUND
- Navigation path: NO EVIDENCE FOUND
- User-visible workflow: NO EVIDENCE FOUND

**Notes:**
- No partial-hour billing logic
- No "minimum 30 minutes" rules
- No pro-rata calculation

**Risks / Gaps:**
- Cannot bill for 15 minutes of play accurately
- Cannot implement "round up to half hour" rules

**Recommendation:** IMPLEMENT - Add to PricingRule engine

---

## A.12 Time rounding rules

**Feature ID:** A.12  
**Feature Name:** Time rounding rules  
**Status:** NOT IMPLEMENTED

**Backend Evidence:**
- Database tables / columns: NO EVIDENCE FOUND
- Domain entities: NO EVIDENCE FOUND
- Services: NO EVIDENCE FOUND
- APIs / handlers: NO EVIDENCE FOUND
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): NO EVIDENCE FOUND
- ViewModels: NO EVIDENCE FOUND
- Navigation path: NO EVIDENCE FOUND
- User-visible workflow: NO EVIDENCE FOUND

**Notes:**
- No rounding logic exists
- Common rules: round to 5min, 15min, 30min
- Could favor house or customer

**Risks / Gaps:**
- Manual billing uncertainty
- Operator-dependent rounding = inconsistency

**Recommendation:** IMPLEMENT - Configurable rounding rules in PricingRule

---

## A.13 Ability to hide table view and show only consumptions

**Feature ID:** A.13  
**Feature Name:** Hide table view / show consumptions only  
**Status:** NOT IMPLEMENTED

**Backend Evidence:**
- Database tables / columns: NO EVIDENCE FOUND
- Domain entities: NO EVIDENCE FOUND
- Services: NO EVIDENCE FOUND
- APIs / handlers: NO EVIDENCE FOUND
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): NO EVIDENCE FOUND (no toggle in TableMap or Order Entry)
- ViewModels: NO EVIDENCE FOUND
- Navigation path: NO EVIDENCE FOUND
- User-visible workflow: NO EVIDENCE FOUND

**Notes:**
- Some workflows may not need table view
- Quick-service mode where tables are irrelevant
- No UI toggle to hide floor plan

**Risks / Gaps:**
- Cannot operate in "counter service" mode easily
- Always shows table-oriented UI

**Recommendation:** DEFER - Low priority for billiard clubs

---

## A.14 Notes per table session

**Feature ID:** A.14  
**Feature Name:** Notes per table session  
**Status:** PARTIAL

**Backend Evidence:**
- Database tables / columns: `Tickets.Note`
- Domain entities: `Ticket.Note` property
- Services: `UpdateTicketNoteCommandHandler.cs`
- APIs / handlers: `UpdateTicketNoteCommand`
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): `NotesDialog.xaml`
- ViewModels: `NotesDialogViewModel.cs`
- Navigation path: Order Entry → Notes button
- User-visible workflow: Can add notes to ticket

**Notes:**
- Notes are per-ticket, not per-session
- If ticket = session, this works
- Notes dialog exists and functional

**Risks / Gaps:**
- If session spans multiple tickets, notes fragmented
- No session-level notes entity

**Recommendation:** KEEP AS-IS - Ticket notes sufficient if session = ticket

---

## A.15 Manual time adjustment (permission restricted)

**Feature ID:** A.15  
**Feature Name:** Manual time adjustment  
**Status:** NOT IMPLEMENTED

**Backend Evidence:**
- Database tables / columns: NO EVIDENCE FOUND
- Domain entities: NO EVIDENCE FOUND
- Services: NO EVIDENCE FOUND
- APIs / handlers: NO EVIDENCE FOUND
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): NO EVIDENCE FOUND
- ViewModels: NO EVIDENCE FOUND
- Navigation path: NO EVIDENCE FOUND
- User-visible workflow: NO EVIDENCE FOUND

**Notes:**
- No time tracking = no time adjustment
- Permission system exists but no time adjustment action
- AuditEvent table exists for logging

**Risks / Gaps:**
- Cannot correct timer errors
- Cannot apply courtesy time adjustments
- Without time tracking, this feature is moot

**Recommendation:** IMPLEMENT - Add with AuditEvent logging when timer exists

---

## A.16 Pause / resume table session

**Feature ID:** A.16  
**Feature Name:** Pause / resume session  
**Status:** NOT IMPLEMENTED

**Backend Evidence:**
- Database tables / columns: NO EVIDENCE FOUND (no Paused status)
- Domain entities: `TableStatus.cs` has no Paused value
- Services: NO EVIDENCE FOUND
- APIs / handlers: NO EVIDENCE FOUND
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): NO EVIDENCE FOUND
- ViewModels: NO EVIDENCE FOUND
- Navigation path: NO EVIDENCE FOUND
- User-visible workflow: NO EVIDENCE FOUND

**Notes:**
- Essential for breaks, disputes, food orders
- Must pause timer and resume accurately
- No mechanism exists

**Risks / Gaps:**
- Cannot pause for bathroom breaks
- Cannot pause during disputes
- Operators must manually track paused time

**Recommendation:** IMPLEMENT - Add Paused status, pause/resume commands

---

## A.17 Forced close (manager override)

**Feature ID:** A.17  
**Feature Name:** Forced close (manager override)  
**Status:** PARTIAL

**Backend Evidence:**
- Database tables / columns: `Tickets.Status`
- Domain entities: `Ticket.Void()` method exists
- Services: `VoidTicketCommandHandler.cs`
- APIs / handlers: `VoidTicketCommand`
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): `VoidTicketDialog.xaml`
- ViewModels: `VoidTicketViewModel.cs`
- Navigation path: Manager Functions → Void Ticket
- User-visible workflow: Manager can void unpaid ticket

**Notes:**
- Void = cancel without payment
- Permission-restricted (VoidTicket permission)
- No "force close with balance write-off"

**Risks / Gaps:**
- Void is all-or-nothing, no partial write-off
- No "close with outstanding balance" option
- Audit trail exists via VoidReason

**Recommendation:** EXTEND - Add force-close with balance tracking option

---

## A.18 Session history per table

**Feature ID:** A.18  
**Feature Name:** Session history per table  
**Status:** PARTIAL

**Backend Evidence:**
- Database tables / columns: `Tickets.TableNumbers`, `Tickets.CreatedAt`, `Tickets.ClosedAt`
- Domain entities: `Ticket.cs`
- Services: `SalesReportRepository.cs`
- APIs / handlers: Report queries
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): `SalesReportsPage.xaml`
- ViewModels: `SalesReportsViewModel.cs`
- Navigation path: Back Office → Reports
- User-visible workflow: Can filter by table in some reports

**Notes:**
- Ticket history exists
- Can query tickets by table number
- No dedicated "Table History" view

**Risks / Gaps:**
- No dedicated table history screen
- Must use reports with filters
- No quick table-level analytics

**Recommendation:** EXTEND - Add dedicated "Table History" view/report

---

## A.19 Session audit trail (who modified time, when, why)

**Feature ID:** A.19  
**Feature Name:** Session audit trail  
**Status:** NOT IMPLEMENTED

**Backend Evidence:**
- Database tables / columns: `AuditEvents` table exists
- Domain entities: `AuditEvent.cs`
- Services: `AuditEventRepository.cs`
- APIs / handlers: NO EVIDENCE FOUND for time modifications
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): NO EVIDENCE FOUND
- ViewModels: NO EVIDENCE FOUND
- Navigation path: NO EVIDENCE FOUND
- User-visible workflow: NO EVIDENCE FOUND

**Notes:**
- AuditEvent infrastructure exists
- No time tracking = no time audit trail
- Would need: UserId, EventType, OldValue, NewValue, Reason

**Risks / Gaps:**
- Cannot investigate time disputes
- Cannot prove operator modifications
- Critical for fraud prevention

**Recommendation:** IMPLEMENT - When time tracking exists, log all modifications

---

## Category A COMPLETE

- Features audited: 19
- Fully implemented: 0
- Partially implemented: 9
- Not implemented: 10

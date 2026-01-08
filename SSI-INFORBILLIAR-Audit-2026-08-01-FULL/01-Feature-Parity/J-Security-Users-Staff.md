# Category J: Security, Users & Staff Operations

## J.1 User accounts

**Feature ID:** J.1  
**Feature Name:** User accounts  
**Status:** FULLY IMPLEMENTED

**Backend Evidence:**
- Database tables / columns: Users table
- Domain entities: `User.cs` - Id, Username, Name, EncryptedPin, RoleId
- Services: User CRUD operations
- APIs / handlers: User management commands
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): User management in back office
- ViewModels: User management exists
- Navigation path: Back Office → Users
- User-visible workflow: Create/edit users

**Notes:**
- Full user entity with PIN encryption
- HourlyRate, PreferredLanguage supported

**Risks / Gaps:**
- None significant

**Recommendation:** COMPLETE - Core implementation done

---

## J.2 Role-based access control

**Feature ID:** J.2  
**Feature Name:** Role-based access control  
**Status:** FULLY IMPLEMENTED

**Backend Evidence:**
- Database tables / columns: Roles table, User.RoleId
- Domain entities: `Role.cs` with Permissions field
- Services: Role assignment in User entity
- APIs / handlers: Role CRUD, permission checks
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): Role management
- ViewModels: Role assignment
- Navigation path: Back Office → Roles
- User-visible workflow: Assign roles to users

**Notes:**
- Role has Permissions (UserPermission flags enum)
- User.Role for authorization checks

**Risks / Gaps:**
- None significant

**Recommendation:** COMPLETE - Full RBAC implementation

---

## J.3 Permission-restricted actions

**Feature ID:** J.3  
**Feature Name:** Permission-restricted actions  
**Status:** FULLY IMPLEMENTED

**Backend Evidence:**
- Database tables / columns: Role.Permissions
- Domain entities: `UserPermission.cs` flags enum
- Services: Permission checks in handlers
- APIs / handlers: Authorization decorators
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): Buttons may be disabled
- ViewModels: Permission checks before actions
- Navigation path: Action → Permission check
- User-visible workflow: Access denied for unauthorized

**Notes:**
- UserPermission covers: ViewTickets, CreateTickets, VoidTicket, ViewReports, ManageUsers, etc.

**Risks / Gaps:**
- May need more granular permissions

**Recommendation:** VERIFY - Ensure all sensitive actions are protected

---

## J.4 Staff clock-in / clock-out

**Feature ID:** J.4  
**Feature Name:** Staff clock-in / clock-out  
**Status:** PARTIALLY IMPLEMENTED

**Backend Evidence:**
- Database tables / columns: Attendance related
- Domain entities: `AttendanceReportDto.cs` exists
- Services: `GetAttendanceReportQueryHandler.cs`
- APIs / handlers: Attendance report endpoint
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): NO EVIDENCE FOUND for clock dialog
- ViewModels: NO EVIDENCE FOUND
- Navigation path: NO EVIDENCE FOUND
- User-visible workflow: NO EVIDENCE FOUND

**Notes:**
- Report exists but clock UI missing
- May track via shift/login times

**Risks / Gaps:**
- Staff cannot explicitly clock in/out
- Labor tracking incomplete

**Recommendation:** IMPLEMENT - Add clock-in/out UI

---

## J.5 Shift start / end

**Feature ID:** J.5  
**Feature Name:** Shift start / end  
**Status:** FULLY IMPLEMENTED

**Backend Evidence:**
- Database tables / columns: Shifts table, CashSession
- Domain entities: `Shift.cs` with StartTime, EndTime
- Services: Shift management handlers
- APIs / handlers: Start/end shift commands
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): Shift dialogs exist
- ViewModels: Shift management
- Navigation path: Manager → Shift
- User-visible workflow: Open/close shifts

**Notes:**
- Full shift entity with time tracking
- Links to CashSession

**Risks / Gaps:**
- None significant

**Recommendation:** COMPLETE - Works as expected

---

## J.6 Cashier shift assignment

**Feature ID:** J.6  
**Feature Name:** Cashier shift assignment  
**Status:** FULLY IMPLEMENTED

**Backend Evidence:**
- Database tables / columns: CashSession.UserId
- Domain entities: `CashSession.cs` links to User
- Services: Cash session management
- APIs / handlers: Open session for user
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): Cash session dialogs
- ViewModels: User context in cash session
- Navigation path: Open Drawer → Assign User
- User-visible workflow: User owns session

**Notes:**
- CashSession tracks which user opened it
- Full audit trail

**Risks / Gaps:**
- None significant

**Recommendation:** COMPLETE - User assignment works

---

## J.7 Action attribution (who did what)

**Feature ID:** J.7  
**Feature Name:** Action attribution (who did what)  
**Status:** PARTIALLY IMPLEMENTED

**Backend Evidence:**
- Database tables / columns: CreatedBy, ModifiedBy fields
- Domain entities: Some entities have user tracking
- Services: User context in commands
- APIs / handlers: Some attribution
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): Some displays show user
- ViewModels: User info in displays
- Navigation path: Various
- User-visible workflow: Ticket shows server

**Notes:**
- Basic attribution exists
- Not comprehensive across all actions

**Risks / Gaps:**
- Some actions not tracked
- Audit gaps possible

**Recommendation:** EXTEND - Add consistent user tracking

---

## J.8 Server-controlled date/time (anti-tampering)

**Feature ID:** J.8  
**Feature Name:** Server-controlled date/time (anti-tampering)  
**Status:** NOT IMPLEMENTED

**Backend Evidence:**
- Database tables / columns: Uses DateTime.Now
- Domain entities: NO EVIDENCE FOUND for server time
- Services: NO EVIDENCE FOUND
- APIs / handlers: NO EVIDENCE FOUND
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): NO EVIDENCE FOUND
- ViewModels: NO EVIDENCE FOUND
- Navigation path: NO EVIDENCE FOUND
- User-visible workflow: NO EVIDENCE FOUND

**Notes:**
- System uses local clock
- Could be manipulated

**Risks / Gaps:**
- Time tampering possible
- Report manipulation risk

**Recommendation:** CONSIDER - NTP sync or DB server time

---

## J.9 Audit log of sensitive actions

**Feature ID:** J.9  
**Feature Name:** Audit log of sensitive actions  
**Status:** PARTIALLY IMPLEMENTED

**Backend Evidence:**
- Database tables / columns: Some audit fields
- Domain entities: Domain events for logging
- Services: Structured logging via ILogger
- APIs / handlers: Log statements exist
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): NO EVIDENCE FOUND for audit viewer
- ViewModels: NO EVIDENCE FOUND
- Navigation path: NO EVIDENCE FOUND
- User-visible workflow: NO EVIDENCE FOUND

**Notes:**
- Logging exists but no audit table
- No UI to view audit history

**Risks / Gaps:**
- Cannot review actions in app
- Logs may not be retained

**Recommendation:** IMPLEMENT - Add AuditLog table and viewer

---

## J.10 Permission escalation workflow

**Feature ID:** J.10  
**Feature Name:** Permission escalation workflow  
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
- Manager override for restricted actions
- PIN prompt for authorization

**Risks / Gaps:**
- Cannot get manager approval
- Staff blocked on restricted actions

**Recommendation:** IMPLEMENT - Add manager PIN override dialog

---

## Category J COMPLETE

- Features audited: 10
- Fully implemented: 5
- Partially implemented: 3
- Not implemented: 2

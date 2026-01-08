# Category E: Reservations & Scheduling

## E.1 Table reservations

**Feature ID:** E.1  
**Feature Name:** Table reservations  
**Status:** NOT IMPLEMENTED

**Backend Evidence:**
- Database tables / columns: NO EVIDENCE FOUND (no Reservations table)
- Domain entities: `TableStatus.Booked` exists but no `Reservation` entity
- Services: NO EVIDENCE FOUND
- APIs / handlers: NO EVIDENCE FOUND
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): NO EVIDENCE FOUND
- ViewModels: NO EVIDENCE FOUND
- Navigation path: NO EVIDENCE FOUND
- User-visible workflow: NO EVIDENCE FOUND

**Notes:**
- Table can be marked "Booked" but no reservation data attached
- No booking time, customer, or duration
- Cannot actually reserve a table for future time

**Risks / Gaps:**
- Core club management feature missing
- Cannot manage future table availability
- Walk-in only operation

**Recommendation:** IMPLEMENT - Create Reservation entity with complete workflow

---

## E.2 Waiting list

**Feature ID:** E.2  
**Feature Name:** Waiting list  
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
- No waitlist functionality
- No queue management
- No estimated wait time

**Risks / Gaps:**
- Cannot manage busy periods
- Customers leave without waiting

**Recommendation:** IMPLEMENT - Add WaitlistEntry entity

---

## E.3 Calendar-based reservations (day)

**Feature ID:** E.3  
**Feature Name:** Calendar-based reservations (day view)  
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
- No calendar component
- No day view for reservations
- Core UI for reservation management missing

**Risks / Gaps:**
- Cannot visualize daily bookings
- No availability view

**Recommendation:** IMPLEMENT - Add calendar UI when Reservation entity exists

---

## E.4 Calendar-based reservations (week)

**Feature ID:** E.4  
**Feature Name:** Calendar-based reservations (week view)  
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
- No week view calendar
- Essential for planning ahead

**Risks / Gaps:**
- Cannot plan weekly schedule
- Tournament/event planning impossible

**Recommendation:** IMPLEMENT - Add week view calendar

---

## E.5 Calendar-based reservations (month)

**Feature ID:** E.5  
**Feature Name:** Calendar-based reservations (month view)  
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
- No month overview
- Useful for trends and capacity planning

**Risks / Gaps:**
- Cannot see monthly patterns
- Event planning difficult

**Recommendation:** IMPLEMENT - Add month view calendar

---

## E.6 Color-coded reservation types

**Feature ID:** E.6  
**Feature Name:** Color-coded reservation types  
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
- No reservation types (Regular, VIP, Tournament, Private Party)
- No color coding system
- Visual distinction needed

**Risks / Gaps:**
- Cannot differentiate reservation types at glance
- Priority handling difficult

**Recommendation:** IMPLEMENT - Add ReservationType enum with colors

---

## E.7 Reservation notes

**Feature ID:** E.7  
**Feature Name:** Reservation notes  
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
- Would be a field on Reservation entity
- Special requests, accessibility needs, etc.
- Entity doesn't exist

**Risks / Gaps:**
- Cannot record special instructions
- Customer preferences lost

**Recommendation:** IMPLEMENT - Include Notes field in Reservation entity

---

## E.8 Reservation linked to member

**Feature ID:** E.8  
**Feature Name:** Reservation linked to member  
**Status:** NOT IMPLEMENTED

**Backend Evidence:**
- Database tables / columns: NO EVIDENCE FOUND
- Domain entities: NO EVIDENCE FOUND (no Member entity either)
- Services: NO EVIDENCE FOUND
- APIs / handlers: NO EVIDENCE FOUND
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): NO EVIDENCE FOUND
- ViewModels: NO EVIDENCE FOUND
- Navigation path: NO EVIDENCE FOUND
- User-visible workflow: NO EVIDENCE FOUND

**Notes:**
- Requires Member entity first
- FK relationship: Reservation → Member
- Member benefits would apply

**Risks / Gaps:**
- Cannot identify reservations by member
- No member history of bookings

**Recommendation:** IMPLEMENT - Link when both entities exist

---

## E.9 Reservation linked to guest

**Feature ID:** E.9  
**Feature Name:** Reservation linked to guest  
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
- Guest = non-member customer
- Contact info for booking (phone, name)
- Minimal data capture

**Risks / Gaps:**
- Cannot contact for confirmation
- No callback capability

**Recommendation:** IMPLEMENT - Add guest name/phone to Reservation

---

## E.10 Conflict detection

**Feature ID:** E.10  
**Feature Name:** Conflict detection  
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
- Would check table availability before booking
- Prevent double-booking
- Essential business rule

**Risks / Gaps:**
- Double bookings possible
- Customer disappointment
- Operational chaos

**Recommendation:** IMPLEMENT - Add conflict validation in Reservation creation

---

## E.11 Reservation cancellation rules

**Feature ID:** E.11  
**Feature Name:** Reservation cancellation rules  
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
- Cancellation window rules (e.g., 24 hours notice)
- Cancellation fees
- Policy enforcement

**Risks / Gaps:**
- No control over last-minute cancellations
- Revenue loss from no-shows

**Recommendation:** IMPLEMENT - Add cancellation policy rules

---

## E.12 No-show handling

**Feature ID:** E.12  
**Feature Name:** No-show handling  
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
- Mark reservation as no-show
- Track no-show history per customer
- Auto-release table after grace period

**Risks / Gaps:**
- Tables held unnecessarily
- No customer accountability
- No automatic release

**Recommendation:** IMPLEMENT - Add no-show status and tracking

---

## Category E COMPLETE

- Features audited: 12
- Fully implemented: 0
- Partially implemented: 0
- Not implemented: 12

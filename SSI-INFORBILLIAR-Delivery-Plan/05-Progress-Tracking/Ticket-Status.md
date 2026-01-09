# Ticket Status Tracker

> **Machine-Updatable**: This file is designed for automated updates. Update status column as tickets progress.

---

## Legend

| Status | Description |
|--------|-------------|
| `NOT_STARTED` | Ticket not yet begun |
| `IN_PROGRESS` | Actively being worked on |
| `IN_REVIEW` | Code complete, awaiting review |
| `BLOCKED` | Cannot proceed due to dependency |
| `DONE` | Completed and merged |

---

## P0 Tickets (Critical Blockers)

### Backend P0

| Ticket ID | Title | Assignee | Status | Last Updated |
|-----------|-------|----------|--------|--------------|
| BE-A.1-01 | Create TableSession Entity | Agent | DONE | 2026-01-08 |
| BE-A.1-02 | Create StartTableSessionCommand | Agent | DONE | 2026-01-08 |
| BE-A.1-03 | Add Table Status Management Methods | Agent | DONE | 2026-01-08 |
| BE-A.2-01 | Create EndTableSessionCommand | Agent | DONE | 2026-01-08 |
| BE-A.3-01 | Create GetActiveSessionsQuery | Agent | DONE | 2026-01-08 |
| BE-A.4-01 | Add Session Status to Table Query | Agent | DONE | 2026-01-09 |
| BE-A.5-01 | Create TableType Entity | Agent | DONE | 2026-01-08 |
| BE-A.6-01 | Add TableTypeId to Table Entity | TBD | NOT_STARTED | 2026-01-08 |
| BE-A.9-01 | Create PricingService | TBD | NOT_STARTED | 2026-01-08 |
| BE-A.16-01 | Create PauseTableSessionCommand | TBD | NOT_STARTED | 2026-01-08 |
| BE-A.17-01 | Create AdjustSessionTimeCommand | TBD | NOT_STARTED | 2026-01-08 |
| BE-E.1-01 | Create Reservation Entity | TBD | NOT_STARTED | 2026-01-08 |
| BE-E.1-02 | Create CreateReservationCommand | TBD | NOT_STARTED | 2026-01-08 |
| BE-E.2-01 | Create GetReservationsQuery | TBD | NOT_STARTED | 2026-01-08 |
| BE-E.3-01 | Create UpdateReservationCommand | TBD | NOT_STARTED | 2026-01-08 |
| BE-E.4-01 | Create CancelReservationCommand | TBD | NOT_STARTED | 2026-01-08 |
| BE-E.5-01 | Implement AvailabilityService | TBD | NOT_STARTED | 2026-01-08 |
| BE-E.6-01 | Create ConvertReservationToSession | TBD | NOT_STARTED | 2026-01-08 |
| BE-F.1-01 | Create Customer Entity | TBD | NOT_STARTED | 2026-01-08 |
| BE-F.2-01 | Create Customer Search Query | TBD | NOT_STARTED | 2026-01-08 |
| BE-F.3-01 | Create Member Entity | TBD | NOT_STARTED | 2026-01-08 |
| BE-F.4-01 | Create MembershipTier Entity | TBD | NOT_STARTED | 2026-01-08 |
| BE-F.5-01 | Implement Member Discount Service | TBD | NOT_STARTED | 2026-01-08 |
| BE-C.1-01 | Complete Ticket with Session Link | TBD | NOT_STARTED | 2026-01-08 |
| BE-C.2-01 | Implement Time-Based Line Items | TBD | NOT_STARTED | 2026-01-08 |
| BE-J.1-01 | Manager Authorization Flow | User | DONE | 2026-01-08 |

### Frontend P0

| Ticket ID | Title | Assignee | Status | Last Updated |
|-----------|-------|----------|--------|--------------|
| FE-A.1-01 | Create StartSessionDialog | Agent | DONE | 2026-01-08 |
| FE-A.1-02 | Add Session Timer to Table Card | Agent | DONE | 2026-01-08 |
| FE-A.2-01 | Create EndSessionDialog | Agent | DONE | 2026-01-08 |
| FE-A.3-01 | Create Active Sessions Panel | Agent | DONE | 2026-01-08 |
| FE-A.4-01 | Add Status Indicators | Agent | DONE | 2026-01-09 |
| FE-A.16-01 | Add Pause/Resume Controls | TBD | NOT_STARTED | 2026-01-08 |
| FE-E.1-01 | Create ReservationDialog | TBD | NOT_STARTED | 2026-01-08 |
| FE-E.2-01 | Create ReservationCalendarPage | TBD | NOT_STARTED | 2026-01-08 |
| FE-E.5-01 | Create AvailabilityCheckPanel | TBD | NOT_STARTED | 2026-01-08 |
| FE-E.6-01 | Create CheckInReservationDialog | TBD | NOT_STARTED | 2026-01-08 |
| FE-F.1-01 | Create CustomerListPage | TBD | NOT_STARTED | 2026-01-08 |
| FE-F.2-01 | Create CustomerSearchControl | TBD | NOT_STARTED | 2026-01-08 |
| FE-F.3-01 | Create MemberProfilePage | TBD | NOT_STARTED | 2026-01-08 |
| FE-C.2-01 | Display Time Charges on Ticket | TBD | NOT_STARTED | 2026-01-08 |
| FE-J.1-01 | Create ManagerPinDialog | User | DONE | 2026-01-08 |
| FE-J.1-02 | Create LoginPage | User | DONE | 2026-01-08 |

### Cross-Cutting P0

| Ticket ID | Title | Assignee | Status | Last Updated |
|-----------|-------|----------|--------|--------------|
| CC-INFRA-01 | Migration: Session Entities | TBD | NOT_STARTED | 2026-01-08 |
| CC-INFRA-02 | Migration: Reservation Entities | TBD | NOT_STARTED | 2026-01-08 |
| CC-INFRA-03 | Migration: Customer/Member | TBD | NOT_STARTED | 2026-01-08 |
| CC-INFRA-04 | Register Services in DI | TBD | NOT_STARTED | 2026-01-08 |
| CC-SEC-01 | Session-Based Authentication | TBD | NOT_STARTED | 2026-01-08 |
| CC-SEC-03 | Manager Authorization Service | User | DONE | 2026-01-08 |
| CC-TEST-01 | Unit Tests: PricingService | TBD | NOT_STARTED | 2026-01-08 |
| CC-TEST-02 | Unit Tests: ReservationService | TBD | NOT_STARTED | 2026-01-08 |

---

## Summary Statistics

| Priority | Total | Not Started | In Progress | Done | Completion % |
|----------|-------|-------------|-------------|------|--------------|
| P0 | 49 | 49 | 0 | 0 | 0% |
| P1 | 52 | 52 | 0 | 0 | 0% |
| P2 | 39 | 39 | 0 | 0 | 0% |
| **Total** | **140** | **140** | **0** | **0** | **0%** |

---

## Sprint Velocity Tracking

| Sprint | Start Date | End Date | Planned | Completed | Notes |
|--------|------------|----------|---------|-----------|-------|
| Sprint 1 | TBD | TBD | TBD | 0 | Foundation phase |
| Sprint 2 | TBD | TBD | TBD | 0 | |
| Sprint 3 | TBD | TBD | TBD | 0 | |

---

*Last Updated: 2026-01-09*

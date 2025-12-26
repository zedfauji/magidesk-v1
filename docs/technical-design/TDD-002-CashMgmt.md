# Technical Design: Cash Management Subsystem

## 1. Overview
Manages the physical cash drawer lifecycle: Assignment -> Transactions (Sales/Drops) -> Reconciliation (Count) -> Close.

**Related Features:**
- F-0012: Drawer Pull
- F-0060: Drawer Assignment
- F-0094: Sales Balance Report
- F-0010: Cash Drops

## 2. Domain Model

### 2.1. Entities

#### `DrawerAssignment`
Represents the exclusive ownership of a drawer by a user/terminal for a session.
- `id`: UUID
- `terminalId`: UUID
- `userId`: UUID
- `startTime`: DateTime
- `endTime`: DateTime (Nullable - Open if null)
- `startBalance`: Decimal (Opening Float)
- `endBalance`: Decimal (Closing Count)
- `status`: Enum (OPEN, BLIND_CLOSED, RECONCILED)

#### `TerminalTransaction` (Existing but needs formalization)
- `id`: UUID
- `assignmentId`: UUID (Link to current session)
- `type`: Enum (SALE, REFUND, DROP, PAYOUT, BLEED, OPEN_FLOAT)
- `amount`: Decimal
- `reference`: String (TicketID or Note)

### 2.2. Services

#### `DrawerService`
- **Methods**:
    - `assignDrawer(userId, terminalId, startBalance)`: F-0060. Fails if Terminal already has OPEN assignment.
    - `performTransaction(type, amount)`: Records transaction. F-0072/73/74.
    - `calculateExpectedBalance(assignmentId)`: Dynamic Sum of Start + Sales - Drops.
    - `closeDrawer(assignmentId, actualCount)`: F-0012. Records Actual vs Expected. Generates Variance.

## 3. Workflows

### 3.1. Start of Shift
1.  Manager assigns Drawer to Terminal/User.
2.  Input "Starting Cash" (e.g., $200).
3.  System creates `DrawerAssignment` (Status: OPEN).

### 3.2. Mid-Shift
1.  Sale (Cash): `TerminalTransaction` created (+$50).
2.  Drop (Safe): `TerminalTransaction` created (-$100).
3.  Current Balance = $200 + $50 - $100 = $150.

### 3.3. End of Shift (Drawer Pull)
1.  User requests "Drawer Pull".
2.  System hides Expected Balance ("Blind Count").
3.  User counts cash, enters $148.
4.  System closes Assignment.
5.  Variance = $148 - $150 = -$2.00 (Shortage).
6.  Alert Manager if Variance > Threshold.

## 4. API Specification

| Method | Endpoint | Description |
| :--- | :--- | :--- |
| `POST` | `/api/drawers/assign` | Open a drawer session. |
| `POST` | `/api/drawers/transaction` | Log Drop/Payout. |
| `GET` | `/api/drawers/current/balance` | Get calculated balance. |
| `POST` | `/api/drawers/close` | Submit blind count and close. |

## 5. Data Integrity
- **Concurrency**: `DrawerAssignment` table must be optimistically locked to prevent double-assignment.
- **Audit**: All Variance events logged to Journal (F-0103).

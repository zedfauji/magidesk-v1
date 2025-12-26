# Technical Design: Batch Payments & Group Settle

## 1. Overview
Handles complex payment scenarios: Paying for multiple tickets in one go (Group Settle) and Settle-later workflows (Batch Capture).

**Related Features:**
- F-0046: Group Settle Ticket
- F-0018: Auth Batch Capture
- F-0017: Auth Code (Manual)

## 2. Domain Model

### 2.1. Entities

#### `PaymentBatch` (For Credit Cards)
- `id`: UUID
- `terminalId`: UUID
- `status`: OPEN, CLOSED, SUBMITTED
- `transactions`: List<PaymentTransaction>

#### `GroupSettlement` (For F-0046)
- `id`: UUID
- `masterTransactionId`: UUID (The Payment)
- `childTicketIds`: List<UUID>
- `distributionStrategy`: Enum (EQUAL_SPLIT, PROPORTIONAL, MANUAL)

### 2.2. Services

#### `BatchPaymentService`
- **Responsibility**: Orchestrates paying multiple tickets with one Tender.
- **Workflow (Group Settle)**:
    1.  User selects Tickets A ($50), B ($50). Total $100.
    2.  User pays $100 Cash.
    3.  Service creates `Payment` ($100).
    4.  Service "allocates" payment to Ticket A ($50) and Ticket B ($50).
    5.  Both tickets marked PAID.
    6.  Receipt shows "Group Settle - 2 Tickets".

#### `MerchantBatchService`
- **Responsibility**: End-of-day Credit Card Capture.
- **Workflow**:
    1.  During day, `AuthOnly` transactions accumulate.
    2.  At Close, `closeBatch()` is called.
    3.  Gateway API invoked to "Capture" all Auths.
    4.  Updates local status to SETTLED.

## 3. Architecture

### Group Settle Controller
- **Endpoint**: `POST /api/payments/group`
- **Payload**:
    ```json
    {
      "ticketIds": ["uuid-1", "uuid-2"],
      "tenderType": "CARD",
      "amount": 100.00
    }
    ```
- **Atomicity**: Critical. Database Transaction must wrap the entire allocation. If Ticket A update fails, Ticket B and Payment must roll back.

## 4. Edge Cases
- **Partial Group Pay**: Paying $50 towards a $100 group total.
    - *Decision*: Apply pro-rata? Or allow User to specify? Default: Pro-rata.
- **Locked Ticket**: One of the group tickets is being edited by another server.
    - *Action*: Fail whole request.

## 5. Implementation Steps
1.  Create `GroupSettlement` Logic in `PaymentService`.
2.  Implement `BatchCapture` job.


# Order & Ticket API

**Base Path:** `/api/orders`

## 1. Send Order to Kitchen (Submit Items)
Adds new items to an existing ticket/session. This is the "Send" action.

- **Method:** `POST`
- **Path:** `/{ticketId}/lines`

### Request Body
```typescript
interface SelectedModifier {
    groupId: string;
    optionId: string;
    priceDelta: number;
}

interface DraftOrderLine {
    tempId: string; // Client-generated ID for idempotency/tracking
    menuItemId: string;
    quantity: number;
    modifiers: SelectedModifier[];
    instructions?: string;
    // Price details are typically re-calculated by backend, but sent context may be useful
    name?: string; 
    unitPrice?: number;
}
{
  items: DraftOrderLine[]
}
```

### Response Body
```typescript
interface TicketResult {
    success: boolean;
    ticketId: string;
    updatedVersion: number;
}
```

### Status Codes
- `200 OK`: Items added successfully.
- `409 Conflict`: Version mismatch (optimistic concurrency).
- `400 Bad Request`: Validation error (e.g. out of stock).

---

## 2. Get Ticket (Session Context)
Retrieves the full active state of a ticket, including committed items and session totals.
*Note: The UI currently uses `getTicket` to populate the `TableSessionScreen`, implying this endpoint returns the full `ActiveSession` aggregate.*

- **Method:** `GET`
- **Path:** `/tickets/{ticketId}`

### Response Body
```typescript
interface CommittedOrderLine {
    id: string;
    menuItemId: string;
    name: string;
    quantity: number;
    unitPrice: number;
    total: number;
    version: number;
}

interface ActiveSession {
    tableId: string;
    ticketId: string;
    ticketNumber: string;
    startTime: string; // ISO
    isPaused: boolean;
    hourlyRate: number;
    
    // UI state for draft logic
    draftState: 'Idle' | 'Dirty' | 'Submitting' | 'Error'; 
    draftItems: any[]; // Usually empty from server
    
    committedItems: CommittedOrderLine[];
    
    totals: {
        sessionTimeAmount: number;
        fnBSubtotal: number;
        tax: number;
        grandTotal: number;
    };
    version: number;
}
```

### Status Codes
- `200 OK`: Success.
- `404 Not Found`: Ticket not found.

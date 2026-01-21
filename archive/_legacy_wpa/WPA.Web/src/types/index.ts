
// Common Types
export interface Versioned {
    version: number;
}

// Auth State
export interface User {
    id: string;
    username: string;
    firstName: string;
    lastName: string;
    role: 'Server' | 'Manager';
}

export interface AuthSession {
    token: string;
    user: User;
    terminalId: string;
    startedAt: string; // ISO
}

// Table State
export type TableStatus = 'Available' | 'Occupied' | 'Dirty' | 'Disabled';
export type SessionStatus = 'NotStarted' | 'Running' | 'Paused' | 'Ended';

export interface TableSummary extends Versioned {
    id: string;
    name: string;
    tableStatus: TableStatus;
    sessionStatus?: SessionStatus;
    elapsedSeconds?: number;
    totalAmount?: number;

    currentUserId?: string;
    activeTicketId?: string;
    isReservationLocked?: boolean;
}

export interface TableExtension extends TableSummary {
    capacity: number;
    zoneName: string;
}


// Menu
export interface MenuCategory {
    id: string;
    name: string;
    subcategories?: MenuCategory[];
}

export interface MenuItem {
    id: string;
    name: string;
    price: number;
    description?: string;
    categoryId: string;
    stockQuantity?: number;
}

export interface ModifierGroup {
    id: string;
    name: string;
    minSelection: number;
    maxSelection: number;
    options: ModifierOption[];
}

export interface ModifierOption {
    id: string;
    name: string;
    priceDelta: number;
}

// Order / Session State
export interface SelectedModifier {
    groupId: string;
    optionId: string;
    priceDelta: number;
    name: string; // Denormalized for display
}

export interface DraftOrderLine {
    tempId: string; // Local GUI GUID
    menuItemId: string;
    name: string;
    quantity: number;
    unitPrice: number;
    modifiers: SelectedModifier[];
    instructions?: string;
}

export interface CommittedOrderLine extends Versioned {
    id: string;
    menuItemId: string;
    name: string;
    quantity: number;
    unitPrice: number;
    total: number;
    modifiers?: SelectedModifier[]; // Simplification for display
}

export interface ActiveSession extends Versioned {
    tableId: string;
    ticketId: string;
    ticketNumber: string;
    startTime: string; // ISO
    isPaused: boolean;
    hourlyRate: number;

    draftState: 'Idle' | 'Dirty' | 'Submitting' | 'Error';

    draftItems: DraftOrderLine[];
    committedItems: CommittedOrderLine[];

    totals: {
        sessionTimeAmount: number;
        fnBSubtotal: number;
        tax: number;
        grandTotal: number;
    };
}

export interface TicketResult {
    success: boolean;
    ticketId: string;
    updatedVersion: number;
}

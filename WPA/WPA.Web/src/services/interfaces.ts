
import type {
    ActiveSession,
    AuthSession,
    DraftOrderLine,
    MenuCategory,
    MenuItem,
    ModifierGroup,
    TableExtension,
    TableSummary,
    TicketResult,
    User
} from "../types";

export interface IAuthService {
    login(pin: string): Promise<User>;
    logout(): Promise<void>;
    getCurrentSession(): Promise<AuthSession | null>;
}

export interface ITableService {
    getAllTables(): Promise<TableSummary[]>;
    getTableDetails(tableId: string): Promise<TableExtension>;
    startSession(tableId: string): Promise<void>;
    pauseSession(tableId: string): Promise<void>;
    resumeSession(tableId: string): Promise<void>;
    endSession(tableId: string): Promise<ActiveSession>; // Valid to return session summary? Using ActiveSession for now as summary
}

export interface IMenuService {
    getCategories(): Promise<MenuCategory[]>;
    getItems(categoryId: string): Promise<MenuItem[]>;
    searchItems(query: string): Promise<MenuItem[]>;
    getItemModifiers(menuItemId: string): Promise<ModifierGroup[]>;
}

export interface IOrderService {
    sendOrderToKitchen(ticketId: string, items: DraftOrderLine[]): Promise<TicketResult>;
    getTicket(ticketId: string): Promise<ActiveSession>;
    moveOrder(sourceTableId: string, targetTableId: string): Promise<void>;
}

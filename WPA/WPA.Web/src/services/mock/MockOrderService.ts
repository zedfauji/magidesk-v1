
import type { ActiveSession, DraftOrderLine, TicketResult } from "../../types";
import type { IOrderService } from "../interfaces";

export class MockOrderService implements IOrderService {
    async sendOrderToKitchen(ticketId: string, items: DraftOrderLine[]): Promise<TicketResult> {
        console.log(`[Mock] Sending ${items.length} items to kitchen for ticket ${ticketId}`);
        return {
            success: true,
            ticketId,
            updatedVersion: 2
        };
    }

    async getTicket(ticketId: string): Promise<ActiveSession> {
        // Return a mock active session
        return {
            tableId: '1',
            ticketId: ticketId,
            ticketNumber: '10492',
            startTime: new Date(Date.now() - 3600000).toISOString(),
            isPaused: false,
            hourlyRate: 18,
            draftState: 'Idle',
            draftItems: [],
            committedItems: [
                { id: 'line-1', menuItemId: 'item-1', name: 'IPA Local Draft', quantity: 2, unitPrice: 7.00, total: 14.00, version: 1 },
                { id: 'line-2', menuItemId: 'item-5', name: 'Loaded Nachos', quantity: 1, unitPrice: 12.50, total: 12.50, version: 1 }
            ],
            totals: {
                sessionTimeAmount: 25.20,
                fnBSubtotal: 26.50,
                tax: 4.14,
                grandTotal: 55.84
            },
            version: 1
        };
    }

    async moveOrder(sourceTableId: string, targetTableId: string): Promise<void> {
        console.log(`[Mock] Moving order from ${sourceTableId} to ${targetTableId}`);
    }
}

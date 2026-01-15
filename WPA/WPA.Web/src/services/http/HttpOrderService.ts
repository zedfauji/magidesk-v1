import { api } from "./api";
import type { IOrderService } from "../interfaces";
import type { ActiveSession, DraftOrderLine, TicketResult } from "../../types";

interface AddLinesRequest {
    items: {
        menuItemId: string;
        quantity: number;
        unitPrice: number;
        // modifiers here
    }[];
}

interface TicketResultDto {
    success: boolean;
    ticketId: string;
    updatedVersion: number;
}

// Reuse ActiveSessionDto interface structure (implied)
interface ActiveSessionDto extends ActiveSession { }

export class HttpOrderService implements IOrderService {
    async sendOrderToKitchen(ticketId: string, items: DraftOrderLine[]): Promise<TicketResult> {
        const payload: AddLinesRequest = {
            items: items.map(i => ({
                menuItemId: i.menuItemId,
                quantity: i.quantity,
                unitPrice: i.unitPrice
                // modifiers mapping needed here
            }))
        };

        const result = await api.post<TicketResultDto>(`/orders/${ticketId}/lines`, payload);

        return {
            success: result.success,
            ticketId: result.ticketId,
            updatedVersion: result.updatedVersion
        };
    }

    async getTicket(ticketId: string): Promise<ActiveSession> {
        return api.get<ActiveSessionDto>(`/orders/tickets/${ticketId}`);
    }

    async moveOrder(sourceTableId: string, targetTableId: string): Promise<void> {
        // Mapped to TablesController logic
        await api.post('/tables/move', { sourceTableId, targetTableId });
    }
}

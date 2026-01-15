```typescript
import { api } from "./api";
import type { ITableService } from "../interfaces";
import type {
    ActiveSession,
    TableExtension,
    TableSummary,
    TableStatus,
    SessionStatus
} from "../../types";

// DTOs
interface TableSummaryDto {
    id: string;
    name: string;
    tableStatus: string;
    sessionStatus: string;
    elapsedSeconds: number;
    totalAmount: number;
    currentUserId?: string;
    isReservationLocked: boolean;
    version: number;
}

interface TableExtensionDto {
    id: string;
    name: string;
    tableStatus: string;
    capacity: number;
    zoneName: string;
}

interface ActiveSessionDto {
    // Basic fields matching frontend requirements
    tableId?: string;
    ticketId?: string;
    ticketNumber?: string;
    startTime?: string;
}

export class HttpTableService implements ITableService {
    async getAllTables(): Promise<TableSummary[]> {
        const dtos = await api.get<TableSummaryDto[]>('/tables');
        return dtos.map(d => ({
            id: d.id,
            name: d.name,
            tableStatus: d.tableStatus as TableStatus,
            sessionStatus: d.sessionStatus as SessionStatus,
            elapsedSeconds: d.elapsedSeconds,
            totalAmount: d.totalAmount,
            currentUserId: d.currentUserId,
            isReservationLocked: d.isReservationLocked,
            version: d.version
        }));
    }

    async getTableDetails(tableId: string): Promise<TableExtension> {
        const dto = await api.get<TableExtensionDto>(`/ tables / ${ tableId } `);
        return {
            id: dto.id,
            name: dto.name,
            tableStatus: dto.tableStatus as TableStatus,
            // Re-fetch summary fields or default them (Backend separation gap)
            // For now, defaulting non-extension fields as they aren't critical for the "Details" view usually
            version: 1,
            sessionStatus: 'NotStarted',
            elapsedSeconds: 0,

            capacity: dto.capacity,
            zoneName: dto.zoneName
        };
    }

    async startSession(tableId: string): Promise<void> {
        await api.post(`/ tables / ${ tableId } /session/start`, {});
    }

    async pauseSession(tableId: string): Promise<void> {
        await api.post(`/ tables / ${ tableId } /session/pause`, {});
    }

    async resumeSession(tableId: string): Promise<void> {
        await api.post(`/ tables / ${ tableId } /session/resume`, {});
    }

    async endSession(tableId: string): Promise<ActiveSession> {
        const result = await api.post<ActiveSessionDto>(`/ tables / ${ tableId } /session/end`, {});

        // Return a minimal ActiveSession based on result
        return {
            tableId: result.tableId || tableId,
            ticketId: result.ticketId || '',
            ticketNumber: result.ticketNumber || '',
            startTime: result.startTime || new Date().toISOString(),
            isPaused: false,
            hourlyRate: 0,
            draftState: 'Idle',
            draftItems: [],
            committedItems: [],
            totals: {
                sessionTimeAmount: 0,
                fnBSubtotal: 0,
                tax: 0,
                grandTotal: 0
            },
            version: 1
        };
    }
}

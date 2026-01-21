
import type { ActiveSession, TableExtension, TableSummary } from "../../types";
import type { ITableService } from "../interfaces";

const MOCK_TABLES: TableSummary[] = [
    {
        id: '1',
        name: 'TABLE 01',
        tableStatus: 'Occupied',
        sessionStatus: 'Running',
        elapsedSeconds: 5085, // 01:24:45
        totalAmount: 18.50,
        version: 1
    },
    {
        id: '2',
        name: 'TABLE 02',
        tableStatus: 'Occupied',
        sessionStatus: 'Paused',
        elapsedSeconds: 2712, // 00:45:12
        totalAmount: 9.00,
        currentUserId: 'u1', // Matches mock logged in user expectation
        version: 1
    },
    {
        id: '3',
        name: 'TABLE 03',
        tableStatus: 'Available',
        sessionStatus: 'NotStarted',
        version: 1
    },
    {
        id: '4',
        name: 'TABLE 04',
        tableStatus: 'Available', // Actually ended but pending settlement usually keeps it occupied? Following stitch status-ended visual.
        sessionStatus: 'Ended',
        elapsedSeconds: 7800, // 02:10:00
        totalAmount: 26.00,
        version: 1
    },
    {
        id: '5',
        name: 'TABLE 05',
        tableStatus: 'Occupied',
        sessionStatus: 'Running',
        elapsedSeconds: 930, // 00:15:30
        totalAmount: 5.00,
        version: 1
    },
    {
        id: '6',
        name: 'TABLE 06',
        tableStatus: 'Available',
        sessionStatus: 'NotStarted',
        isReservationLocked: true,
        version: 1
    }
];

export class MockTableService implements ITableService {
    async getAllTables(): Promise<TableSummary[]> {
        return MOCK_TABLES;
    }

    async getTableDetails(tableId: string): Promise<TableExtension> {
        const table = MOCK_TABLES.find(t => t.id === tableId);
        if (!table) throw new Error("Table not found");

        return {
            ...table,
            capacity: 6,
            zoneName: 'Main Floor'
        };
    }

    async startSession(tableId: string): Promise<void> {
        console.log(`[Mock] Start Session Table ${tableId}`);
    }

    async pauseSession(tableId: string): Promise<void> {
        console.log(`[Mock] Pause Session Table ${tableId}`);
    }

    async resumeSession(tableId: string): Promise<void> {
        console.log(`[Mock] Resume Session Table ${tableId}`);
    }

    async endSession(tableId: string): Promise<ActiveSession> {
        console.log(`[Mock] End Session Table ${tableId}`);
        // Return a handy mock session summary
        return {
            tableId: tableId,
            ticketId: 'ticket-99',
            ticketNumber: '99',
            startTime: new Date().toISOString(),
            isPaused: false,
            hourlyRate: 15,
            draftState: 'Idle',
            draftItems: [],
            committedItems: [],
            totals: {
                sessionTimeAmount: 25.00,
                fnBSubtotal: 10.00,
                tax: 3.50,
                grandTotal: 38.50
            },
            version: 1
        };
    }
}


import { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { orderService, tableService } from '../../services';
import type { ActiveSession, TableExtension } from '../../types';

export const TableSessionScreen = () => {
    const { tableId } = useParams<{ tableId: string }>();
    const navigate = useNavigate();
    const [session, setSession] = useState<ActiveSession | null>(null);
    const [table, setTable] = useState<TableExtension | null>(null);

    useEffect(() => {
        if (!tableId) return;
        tableService.getTableDetails(tableId).then(details => {
            setTable(details);
            if (details.activeTicketId) {
                orderService.getTicket(details.activeTicketId).then(setSession);
            } else {
                // Initialize empty/idle session state for UI rendering
                setSession({
                    version: 1,
                    tableId: details.id,
                    ticketId: '',
                    ticketNumber: '---',
                    startTime: new Date().toISOString(),
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
                    }
                });
            }
        });
    }, [tableId]);

    if (!session || !table) return <div className="text-white p-8">Loading...</div>;

    // Helper to format duration logic (mocked for visual fidelity as per Stitch)
    // Real app would calculate this from session.startTime
    const hours = '01';
    const minutes = '24';
    const seconds = '45';

    const handleStartSession = async () => {
        if (!tableId || !session) return;
        try {
            await tableService.startSession(tableId);
            // Refresh logic - ideally listen for update or re-fetch
            const details = await tableService.getTableDetails(tableId);
            setTable(details);
            if (details.activeTicketId) {
                const ticket = await orderService.getTicket(details.activeTicketId);
                setSession(ticket);
            }
        } catch (error) {
            console.error("Failed to start session:", error);
        }
    };

    const handleCreateTicket = async () => {
        if (!tableId) return;
        try {
            const result = await orderService.createTicket(tableId, 1); // Default to 1 guest
            if (result.success) {
                const details = await tableService.getTableDetails(tableId);
                setTable(details);
                if (details.activeTicketId) {
                    const ticket = await orderService.getTicket(details.activeTicketId);
                    setSession(ticket);
                }
            }
        } catch (error) {
            console.error("Failed to create ticket:", error);
        }
    };

    return (
        <div className="bg-background-light dark:bg-background-dark font-display text-white selection:bg-primary/30 h-screen overflow-hidden flex flex-col">
            {/* TopNavBar Component */}
            <header className="flex items-center justify-between whitespace-nowrap border-b border-solid border-white/10 px-6 py-4 bg-background-dark/50 backdrop-blur-md">
                <div className="flex items-center gap-6">
                    <button onClick={() => navigate('/tables')} className="flex items-center justify-center size-10 rounded-lg bg-white/5 hover:bg-white/10 transition-colors">
                        <span className="material-symbols-outlined text-white">arrow_back</span>
                    </button>
                    <div className="flex items-center gap-3">
                        <div className="size-8 bg-primary rounded-lg flex items-center justify-center shadow-lg shadow-primary/20">
                            <span className="material-symbols-outlined text-background-dark font-bold">sports_handball</span>
                        </div>
                        <div>
                            <h2 className="text-white text-2xl font-bold leading-tight tracking-tight">{table.name}</h2>
                            <p className="text-xs text-primary/70 font-medium tracking-widest uppercase">{table.zoneName}</p>
                        </div>
                    </div>
                </div>
                <div className="flex flex-1 justify-end gap-6 items-center">
                    <div className="flex items-center gap-8 mr-4">
                        <a className="text-white/60 hover:text-white text-sm font-medium transition-colors cursor-pointer">Floor Map</a>
                        <a className="text-white/60 hover:text-white text-sm font-medium transition-colors cursor-pointer">Waitlist (4)</a>
                        <a className="text-white/60 hover:text-white text-sm font-medium transition-colors cursor-pointer">Daily Reports</a>
                    </div>
                    <div className="flex gap-2 border-l border-white/10 pl-6">
                        <button className="flex size-10 cursor-pointer items-center justify-center rounded-lg bg-white/5 text-white hover:bg-white/10 transition-all">
                            <span className="material-symbols-outlined">notifications</span>
                        </button>
                        <button className="flex size-10 cursor-pointer items-center justify-center rounded-lg bg-white/5 text-white hover:bg-white/10 transition-all">
                            <span className="material-symbols-outlined">settings</span>
                        </button>
                    </div>
                </div>
            </header>

            <main className="flex flex-1 overflow-hidden">
                {/* Left Panel: Control Zone */}
                <div className="flex-1 flex flex-col p-6 gap-6 overflow-y-auto">
                    {/* Session Header & State Indicator */}
                    <div className="flex justify-between items-end px-2">
                        <div className="space-y-1">
                            <h4 className="text-white/40 text-xs font-bold tracking-[0.2em] uppercase">Session Control</h4>
                            <div className="flex items-center gap-3">
                                <span className={`flex h-3 w-3 rounded-full ${session.ticketId ? 'bg-emerald-green active-pulse' : 'bg-white/20'}`}></span>
                                <span className={`text-xl font-bold tracking-tight ${session.ticketId ? 'text-emerald-green' : 'text-white/40'}`}>
                                    {session.ticketId ? (session.hourlyRate > 0 ? (session.isPaused ? 'SESSION PAUSED' : 'ACTIVE SESSION') : 'ORDER ACTIVE') : 'TABLE IDLE'}
                                </span>
                            </div>
                        </div>
                        <div className="text-right">
                            <p className="text-white/40 text-xs font-bold tracking-[0.2em] uppercase">Rate</p>
                            <p className="text-xl font-bold text-white">${session.hourlyRate.toFixed(2)}<span className="text-sm font-normal text-white/40">/hr</span></p>
                        </div>
                    </div>

                    {/* Massive Timer Display - Only show if hourly billing applied */}
                    {session.hourlyRate > 0 && (
                        <div className="bg-charcoal-surface border border-white/5 rounded-2xl p-8 flex flex-col items-center justify-center shadow-2xl relative overflow-hidden group">
                            <div className="absolute inset-0 bg-gradient-to-br from-primary/5 to-transparent pointer-events-none"></div>
                            <p className="text-primary/40 text-xs font-bold tracking-[0.3em] uppercase mb-4 relative z-10">Elapsed Playing Time</p>
                            <div className="flex gap-6 relative z-10">
                                {/* ... Timer components ... */}
                                {/* Note: In a real app, calculate diff from session.startTime if ticketId exists */}
                                <div className="flex flex-col items-center gap-2">
                                    <div className="flex h-32 w-28 items-center justify-center rounded-xl bg-background-dark border border-white/5 shadow-inner">
                                        <p className={`text-6xl font-bold tracking-tighter timer-glow ${session.ticketId ? 'text-primary' : 'text-white/10'}`}>
                                            {session.ticketId ? hours : '--'}
                                        </p>
                                    </div>
                                    <p className="text-white/30 text-xs font-bold uppercase tracking-widest">Hours</p>
                                </div>
                                <div className="text-6xl font-bold text-white/10 pt-6">:</div>
                                <div className="flex flex-col items-center gap-2">
                                    <div className="flex h-32 w-28 items-center justify-center rounded-xl bg-background-dark border border-white/5 shadow-inner">
                                        <p className={`text-6xl font-bold tracking-tighter timer-glow ${session.ticketId ? 'text-primary' : 'text-white/10'}`}>
                                            {session.ticketId ? minutes : '--'}
                                        </p>
                                    </div>
                                    <p className="text-white/30 text-xs font-bold uppercase tracking-widest">Minutes</p>
                                </div>
                                <div className="text-6xl font-bold text-white/10 pt-6">:</div>
                                <div className="flex flex-col items-center gap-2">
                                    <div className="flex h-32 w-28 items-center justify-center rounded-xl bg-background-dark border border-white/5 shadow-inner">
                                        <p className={`text-6xl font-bold tracking-tighter timer-glow ${session.ticketId ? 'text-primary' : 'text-white/10'}`}>
                                            {session.ticketId ? seconds : '--'}
                                        </p>
                                    </div>
                                    <p className="text-white/30 text-xs font-bold uppercase tracking-widest">Seconds</p>
                                </div>
                            </div>
                        </div>
                    )}

                    {/* Order Only Placeholder when no session */}
                    {session.ticketId && session.hourlyRate === 0 && (
                        <div className="bg-charcoal-surface/50 border border-white/5 rounded-2xl p-8 flex flex-col items-center justify-center mb-6">
                            <span className="material-symbols-outlined text-4xl text-emerald-500 mb-2">restaurant</span>
                            <p className="text-white/60 font-medium">Standard Dining Order</p>
                        </div>
                    )}



                    {/* Primary Control Cluster */}
                    <div className="grid grid-cols-2 gap-4">
                        <button
                            onClick={handleStartSession}
                            disabled={!!session.ticketId}
                            className={`flex flex-col items-center justify-center h-40 rounded-2xl border transition-all group ${session.ticketId
                                ? 'bg-white/5 border-white/5 opacity-50 cursor-not-allowed'
                                : 'bg-white/5 border-white/10 hover:bg-white/10'
                                }`}
                        >
                            <span className="material-symbols-outlined text-4xl mb-3 text-white/60 group-hover:scale-110 transition-transform">play_arrow</span>
                            <span className="text-lg font-bold tracking-widest uppercase">Start Session</span>
                        </button>
                        <button className="flex flex-col items-center justify-center h-40 rounded-2xl bg-amber-500/10 border border-amber-500/30 hover:bg-amber-500/20 transition-all group">
                            <span className="material-symbols-outlined text-4xl mb-3 text-amber-500 group-hover:scale-110 transition-transform">pause</span>
                            <span className="text-lg font-bold text-amber-500 tracking-widest uppercase">Pause Timer</span>
                        </button>
                        <button className="flex flex-col items-center justify-center h-40 rounded-2xl bg-primary/10 border border-primary/30 hover:bg-primary/20 transition-all group">
                            <span className="material-symbols-outlined text-4xl mb-3 text-primary group-hover:scale-110 transition-transform">resume</span>
                            <span className="text-lg font-bold text-primary tracking-widest uppercase">Resume Session</span>
                        </button>
                        <button
                            onClick={handleCreateTicket}
                            disabled={!!session.ticketId}
                            className={`flex flex-col items-center justify-center h-40 rounded-2xl bg-emerald-500/10 border border-emerald-500/30 hover:bg-emerald-500/20 transition-all group ${session.ticketId ? 'opacity-50 cursor-not-allowed hidden' : ''}`}
                        >
                            <span className="material-symbols-outlined text-4xl mb-3 text-emerald-500 group-hover:scale-110 transition-transform">receipt_long</span>
                            <span className="text-lg font-bold text-emerald-500 tracking-widest uppercase">Create Order</span>
                        </button>
                        <button
                            onClick={() => navigate(`/summary/${tableId}`)}
                            className="flex flex-col items-center justify-center h-40 rounded-2xl bg-red-500/10 border border-red-500/30 hover:bg-red-500/20 transition-all group"
                        >
                            <span className="material-symbols-outlined text-4xl mb-3 text-red-500 group-hover:scale-110 transition-transform">stop</span>
                            <span className="text-lg font-bold text-red-500 tracking-widest uppercase">End & Checkout</span>
                        </button>
                    </div>

                    {/* Quick-Add F&B Bar */}
                    <div className="mt-auto">
                        <p className="text-white/40 text-xs font-bold tracking-[0.2em] uppercase mb-4 px-2">Quick Add Order</p>
                        <div className="flex gap-3 overflow-x-auto pb-2 scrollbar-hide">
                            <button
                                onClick={() => navigate('/menu', { state: { ticketId: session?.ticketId, tableId: tableId } })}
                                disabled={!session.ticketId}
                                className={`flex-none flex items-center gap-3 px-6 py-4 border rounded-xl transition-colors ${!session.ticketId
                                    ? 'bg-white/5 border-white/5 opacity-50 cursor-not-allowed text-white/40'
                                    : 'bg-charcoal-surface border-white/5 hover:bg-white/5'
                                    }`}
                            >
                                <span className="material-symbols-outlined text-primary">sports_bar</span>
                                <span className="font-bold">Draft Beer</span>
                            </button>
                            {/* ... other quick adds ... */}
                            <button
                                onClick={() => navigate('/menu', { state: { ticketId: session?.ticketId, tableId: tableId } })}
                                // Allow entering menu even if no session? Then we need "Create Ticket" logic in menu.
                                // For now, disabled to match current flow
                                disabled={!session.ticketId}
                                className={`flex-none flex items-center gap-3 px-6 py-4 border rounded-xl transition-colors ${!session.ticketId
                                    ? 'bg-white/5 border-white/5 opacity-50 cursor-not-allowed text-white/40'
                                    : 'bg-charcoal-surface border-white/5 hover:bg-white/5'
                                    }`}
                            >
                                <span className="material-symbols-outlined text-primary">add</span>
                                <span className="font-bold">Full Menu</span>
                            </button>
                        </div>
                    </div>
                </div>

                {/* Right Panel: Transaction Zone */}
                <div className="w-[420px] bg-charcoal-surface border-l border-white/10 flex flex-col">
                    <div className="p-6 border-b border-white/5">
                        <h3 className="text-xl font-bold">Ticket Summary</h3>
                        <p className="text-sm text-white/40">
                            {session.ticketId ? `Ticket ID: #${session.ticketNumber} • Opened ${new Date(session.startTime).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}` : 'No Active Ticket'}
                        </p>
                    </div>

                    <div className="flex-1 overflow-y-auto p-6 space-y-6">
                        {!session.ticketId ? (
                            <div className="h-full flex flex-col items-center justify-center opacity-20 select-none">
                                <span className="material-symbols-outlined text-6xl mb-4">receipt_long</span>
                                <p className="text-sm font-medium text-center uppercase tracking-widest">No active orders</p>
                            </div>
                        ) : (
                            <>
                                {/* Session Time Item */}
                                <div className="flex justify-between items-start">
                                    <div className="flex gap-4">
                                        <div className="size-10 rounded-lg bg-primary/10 flex items-center justify-center">
                                            <span className="material-symbols-outlined text-primary text-xl">schedule</span>
                                        </div>
                                        <div>
                                            <p className="font-bold">Table Time</p>
                                            <p className="text-xs text-white/40">Session started at {new Date(session.startTime).toLocaleTimeString()}</p>
                                        </div>
                                    </div>
                                    <p className="font-bold">${session.totals.sessionTimeAmount.toFixed(2)}</p>
                                </div>

                                {/* F&B Items */}
                                <div className="space-y-4">
                                    {session.committedItems.map(item => (
                                        <div key={item.id} className="flex justify-between items-start">
                                            <div className="flex gap-4">
                                                <div className="size-10 rounded-lg bg-white/5 flex items-center justify-center">
                                                    <span className="material-symbols-outlined text-white/60 text-xl">fastfood</span>
                                                </div>
                                                <div>
                                                    <p className="font-bold text-white/90">{item.name}</p>
                                                    <p className="text-xs text-white/40">Qty: {item.quantity} • ${item.unitPrice.toFixed(2)} ea</p>
                                                    {item.modifiers && item.modifiers.map(mod => <p key={mod.optionId} className="text-[10px] text-white/30">+ {mod.name}</p>)}
                                                </div>
                                            </div>
                                            <p className="font-bold">${item.total.toFixed(2)}</p>
                                        </div>
                                    ))}
                                </div>
                            </>
                        )}
                    </div>

                    {/* Financial Totals */}
                    <div className="p-6 bg-background-dark/50 border-t border-white/10 space-y-4">
                        <div className="space-y-2">
                            <div className="flex justify-between text-sm text-white/60">
                                <span>Subtotal Time</span>
                                <span>${session.totals.sessionTimeAmount.toFixed(2)}</span>
                            </div>
                            <div className="flex justify-between text-sm text-white/60">
                                <span>Subtotal F&B</span>
                                <span>${session.totals.fnBSubtotal.toFixed(2)}</span>
                            </div>
                            <div className="flex justify-between text-sm text-white/60">
                                <span>Tax</span>
                                <span>${session.totals.tax.toFixed(2)}</span>
                            </div>
                        </div>

                        <div className="bg-primary p-5 rounded-xl flex justify-between items-center shadow-lg shadow-primary/10">
                            <div>
                                <p className="text-background-dark text-xs font-bold uppercase tracking-widest">Total Balance</p>
                                <p className="text-background-dark text-4xl font-bold tracking-tighter leading-none">${session.totals.grandTotal.toFixed(2)}</p>
                            </div>
                            <span className="material-symbols-outlined text-background-dark text-3xl">payments</span>
                        </div>

                        <div className="flex gap-3 pt-2">
                            <button className="flex-1 h-14 rounded-xl border border-white/10 font-bold hover:bg-white/5 transition-colors">
                                Move Table
                            </button>
                            <button className="flex-1 h-14 rounded-xl border border-white/10 font-bold hover:bg-white/5 transition-colors flex items-center justify-center gap-2">
                                <span className="material-symbols-outlined text-xl">print</span>
                                Print Bill
                            </button>
                        </div>
                    </div>
                </div>
            </main>
        </div>
    );
};

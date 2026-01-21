
import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { tableService } from '../../services';
import type { TableSummary } from '../../types';

export const TableGridScreen = () => {
    const navigate = useNavigate();
    const [tables, setTables] = useState<TableSummary[]>([]);
    const [activeTab, setActiveTab] = useState<'ALL' | 'MY' | 'AVAILABLE'>('ALL');

    useEffect(() => {
        tableService.getAllTables().then(setTables);
    }, []);

    const handleTableClick = (tableId: string) => {
        navigate(`/session/${tableId}`);
    };

    const getStatusClasses = (status: string, sessionStatus?: string) => {
        if (status === 'Occupied') {
            if (sessionStatus === 'Running') return 'bg-white dark:bg-card-dark glow-running';
            if (sessionStatus === 'Paused') return 'bg-white dark:bg-card-dark glow-paused';
            if (sessionStatus === 'Ended') return 'bg-white dark:bg-card-dark glow-ended';
        }
        return 'bg-white dark:bg-card-dark glow-idle border-dashed border-slate-700';
    };

    return (
        <div className="bg-background-light dark:bg-background-dark font-display text-slate-800 dark:text-slate-200 min-h-screen flex flex-col overflow-hidden">
            {/* Top Navigation Header */}
            <header className="flex items-center justify-between px-8 py-4 bg-white dark:bg-background-dark border-b border-slate-200 dark:border-slate-800 shrink-0">
                <div className="flex items-center gap-6">
                    <div className="flex items-center gap-3">
                        <div className="p-2 bg-primary rounded-lg text-background-dark">
                            <span className="material-symbols-outlined block text-2xl">sports_golf</span>
                        </div>
                        <h1 className="text-xl font-bold tracking-tight">Magidesk <span className="text-primary">POS</span></h1>
                    </div>
                    <div className="h-8 w-px bg-slate-200 dark:bg-slate-800"></div>
                    <div className="flex items-center gap-2 text-slate-500">
                        <span className="material-symbols-outlined text-sm">schedule</span>
                        <span className="text-sm font-medium">10:42 PM</span>
                    </div>
                </div>
                <div className="flex items-center gap-4">
                    <div className="flex flex-col items-end">
                        <p className="text-sm font-bold">Alex G.</p>
                        <p className="text-xs text-slate-500">Floor Manager</p>
                    </div>
                    <div className="size-10 rounded-full bg-slate-700 border-2 border-primary/30 flex items-center justify-center text-primary font-bold">AG</div>
                </div>
            </header>

            {/* Main Content Area */}
            <main className="flex-1 flex flex-col overflow-hidden relative">
                {/* Tab Bar */}
                <div className="px-8 bg-white dark:bg-background-dark border-b border-slate-200 dark:border-slate-800 shrink-0">
                    <div className="flex gap-8">
                        <button
                            onClick={() => setActiveTab('ALL')}
                            className={`py-4 border-b-2 font-bold text-sm tracking-wider flex items-center gap-2 transition-colors ${activeTab === 'ALL' ? 'border-primary text-primary' : 'border-transparent text-slate-500 hover:text-slate-300'}`}
                        >
                            ALL TABLES <span className="bg-primary/20 text-[10px] px-1.5 py-0.5 rounded text-primary">{tables.length}</span>
                        </button>
                        <button
                            onClick={() => setActiveTab('MY')}
                            className={`py-4 border-b-2 font-bold text-sm tracking-wider transition-colors ${activeTab === 'MY' ? 'border-primary text-primary' : 'border-transparent text-slate-500 hover:text-slate-300'}`}
                        >
                            MY SESSIONS
                        </button>
                        <button
                            onClick={() => setActiveTab('AVAILABLE')}
                            className={`py-4 border-b-2 font-bold text-sm tracking-wider transition-colors ${activeTab === 'AVAILABLE' ? 'border-primary text-primary' : 'border-transparent text-slate-500 hover:text-slate-300'}`}
                        >
                            AVAILABLE
                        </button>
                    </div>
                </div>

                {/* Table Grid Container */}
                <div className="flex-1 overflow-y-auto p-8 no-scrollbar pb-32">
                    <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
                        {tables.map(table => {
                            const isRunning = table.sessionStatus === 'Running';
                            const isPaused = table.sessionStatus === 'Paused';
                            const isEnded = table.sessionStatus === 'Ended';
                            const isIdle = !table.sessionStatus || table.sessionStatus === 'NotStarted';

                            return (
                                <div
                                    key={table.id}
                                    onClick={() => handleTableClick(table.id)}
                                    className={`rounded-xl p-5 flex flex-col gap-4 cursor-pointer transition-all active:scale-[0.98] ${getStatusClasses(table.tableStatus, table.sessionStatus)}`}
                                >
                                    <div className="flex justify-between items-start">
                                        <div>
                                            <h3 className={`text-2xl font-bold tracking-tighter ${isIdle ? 'opacity-50' : ''}`}>{table.name}</h3>

                                            {isRunning && (
                                                <span className="text-xs font-bold text-status-running flex items-center gap-1 uppercase">
                                                    <span className="size-1.5 bg-status-running rounded-full animate-pulse"></span> Running
                                                </span>
                                            )}
                                            {isPaused && (
                                                <span className="text-xs font-bold text-status-paused flex items-center gap-1 uppercase">
                                                    <span className="size-1.5 bg-status-paused rounded-full"></span> Paused
                                                </span>
                                            )}
                                            {isEnded && (
                                                <span className="text-xs font-bold text-status-ended flex items-center gap-1 uppercase">
                                                    <span className="size-1.5 bg-status-ended rounded-full"></span> Ended - Pending Bill
                                                </span>
                                            )}
                                            {isIdle && (
                                                <span className="text-xs font-bold text-slate-500 flex items-center gap-1 uppercase">
                                                    <span className="size-1.5 bg-slate-500 rounded-full"></span> Available
                                                </span>
                                            )}
                                        </div>

                                        {!isIdle && (
                                            <button className="size-10 rounded-lg bg-slate-100 dark:bg-slate-800 flex items-center justify-center hover:bg-primary/20 text-slate-400 hover:text-primary transition-colors" onClick={(e) => e.stopPropagation()}>
                                                <span className="material-symbols-outlined text-xl">more_vert</span>
                                            </button>
                                        )}
                                    </div>

                                    {!isIdle ? (
                                        <>
                                            <div className="py-4 flex flex-col items-center">
                                                <p className="text-4xl font-mono font-bold tracking-widest text-slate-900 dark:text-white">
                                                    {table.elapsedSeconds ? new Date(table.elapsedSeconds * 1000).toISOString().substr(11, 8) : '--:--:--'}
                                                </p>
                                                <p className="text-xs text-slate-500 mt-1 uppercase tracking-widest">
                                                    {isPaused ? "Paused At" : isEnded ? "Total Duration" : "Elapsed Time"}
                                                </p>
                                            </div>
                                            <div className="flex justify-between items-end border-t border-slate-100 dark:border-slate-800 pt-4">
                                                <div>
                                                    <p className="text-[10px] text-slate-500 uppercase font-bold">{isEnded ? "Amount Due" : "Total Bill"}</p>
                                                    <p className={`text-lg font-bold ${isEnded ? 'text-status-ended' : 'text-primary'}`}>
                                                        ${table.totalAmount?.toFixed(2) ?? "0.00"}
                                                    </p>
                                                </div>

                                                <div className="flex gap-2">
                                                    {isRunning && (
                                                        <>
                                                            <button className="size-11 rounded-lg bg-status-paused/20 text-status-paused flex items-center justify-center hover:bg-status-paused/30" onClick={(e) => e.stopPropagation()}>
                                                                <span className="material-symbols-outlined">pause</span>
                                                            </button>
                                                            <button className="h-11 px-4 rounded-lg bg-primary text-background-dark font-bold flex items-center gap-2 hover:bg-primary/90" onClick={(e) => { e.stopPropagation(); navigate(`/session/${table.id}`); }}>
                                                                <span className="material-symbols-outlined text-sm">payments</span> BILL
                                                            </button>
                                                        </>
                                                    )}
                                                    {isPaused && (
                                                        <button className="h-11 px-6 rounded-lg bg-status-running text-white font-bold flex items-center gap-2 hover:brightness-110" onClick={(e) => e.stopPropagation()}>
                                                            <span className="material-symbols-outlined text-sm">play_arrow</span> RESUME
                                                        </button>
                                                    )}
                                                    {isEnded && (
                                                        <button className="h-12 px-6 rounded-lg bg-status-ended text-white font-bold flex items-center gap-2 hover:brightness-110" onClick={(e) => { e.stopPropagation(); navigate(`/session/${table.id}`); }}>
                                                            <span className="material-symbols-outlined text-sm">receipt_long</span> SETTLE
                                                        </button>
                                                    )}
                                                </div>
                                            </div>
                                        </>
                                    ) : (
                                        <>
                                            <div className="py-8 flex flex-col items-center">
                                                <span className="material-symbols-outlined text-5xl text-slate-800">add_circle</span>
                                            </div>
                                            <div className="flex flex-col gap-2 border-t border-slate-100 dark:border-slate-800 pt-4">
                                                <button className="w-full h-11 rounded-lg bg-primary/10 text-primary font-bold hover:bg-primary hover:text-background-dark transition-colors">
                                                    START NEW SESSION
                                                </button>
                                            </div>
                                        </>
                                    )}
                                </div>
                            );
                        })}
                    </div>
                </div>

                {/* Floating Quick Action */}
                <div className="fixed right-8 bottom-28 z-40">
                    <button className="flex items-center gap-3 bg-primary text-background-dark font-bold px-6 py-4 rounded-xl shadow-2xl hover:scale-105 transition-transform">
                        <span className="material-symbols-outlined font-bold">add</span>
                        <span className="tracking-tight uppercase">Quick Order Entry</span>
                    </button>
                </div>
            </main>

            {/* Fixed Bottom Navigation */}
            <nav className="bg-white dark:bg-background-dark border-t border-slate-200 dark:border-slate-800 px-8 py-3 flex items-center justify-around z-50 shrink-0">
                <a className="flex flex-col items-center gap-1 text-primary cursor-pointer">
                    <span className="material-symbols-outlined fill-1">grid_view</span>
                    <span className="text-[10px] font-bold uppercase tracking-widest">Tables</span>
                </a>
                <a className="flex flex-col items-center gap-1 text-slate-500 hover:text-slate-300 cursor-pointer">
                    <span className="material-symbols-outlined">receipt</span>
                    <span className="text-[10px] font-bold uppercase tracking-widest">Orders</span>
                </a>
                <a className="flex flex-col items-center gap-1 text-slate-500 hover:text-slate-300 cursor-pointer">
                    <span className="material-symbols-outlined">restaurant</span>
                    <span className="text-[10px] font-bold uppercase tracking-widest">Kitchen</span>
                </a>
                <a className="flex flex-col items-center gap-1 text-slate-500 hover:text-slate-300 cursor-pointer">
                    <span className="material-symbols-outlined">bar_chart</span>
                    <span className="text-[10px] font-bold uppercase tracking-widest">Reports</span>
                </a>
                <a className="flex flex-col items-center gap-1 text-slate-500 hover:text-slate-300 cursor-pointer">
                    <span className="material-symbols-outlined">settings</span>
                    <span className="text-[10px] font-bold uppercase tracking-widest">Settings</span>
                </a>
            </nav>
        </div>
    );
};

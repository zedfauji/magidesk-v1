
import { useNavigate, useParams } from 'react-router-dom';

export const SessionSummaryScreen = () => {
    const { tableId } = useParams<{ tableId: string }>();
    const navigate = useNavigate();

    return (
        <div className="flex flex-col h-screen bg-slate-900 text-white font-sans overflow-hidden">
            <header className="flex items-center justify-between px-6 py-4 border-b border-white/10">
                <div>
                    <h2 className="text-lg font-bold">Table {tableId} <span className="text-cyan-400">/</span> Session Summary</h2>
                    <p className="text-xs text-white/50 font-medium uppercase tracking-widest">Post-Session View</p>
                </div>
                <button
                    onClick={() => navigate('/tables')}
                    className="h-12 px-6 bg-cyan-500 text-slate-900 text-sm font-bold uppercase tracking-wide rounded-lg"
                >
                    Back to Tables
                </button>
            </header>

            <main className="flex-1 overflow-y-auto px-6 py-8">
                <div className="mb-8 p-4 rounded-xl border border-cyan-500/30 bg-cyan-500/5 flex items-center gap-4">
                    <div className="flex-1">
                        <h3 className="font-bold text-cyan-400 uppercase text-sm tracking-widest">Notice: Open Ticket</h3>
                        <p className="text-white/60 text-sm">Session recording is finalized.</p>
                    </div>
                </div>

                <div className="grid grid-cols-1 md:grid-cols-2 gap-8">
                    <div className="bg-slate-800 rounded-2xl p-8 border border-white/5 text-center shadow-xl">
                        <p className="text-white/50 text-xs font-bold uppercase tracking-[0.2em] mb-2">Total Duration</p>
                        <h1 className="text-6xl font-bold font-mono py-2">02:45:12</h1>
                        <div className="mt-4 inline-flex items-center gap-2 px-3 py-1 bg-red-500/10 text-red-500 rounded-full text-xs font-bold uppercase tracking-wider">
                            Session Ended
                        </div>
                    </div>

                    <div className="bg-slate-200 text-slate-900 p-6 space-y-2 rounded-2xl">
                        <div className="flex justify-between items-center text-sm font-medium">
                            <span className="text-slate-500">Subtotal</span>
                            <span className="font-mono">$118.00</span>
                        </div>
                        <div className="flex justify-between items-center pt-2 border-t border-slate-300 mt-2">
                            <span className="text-lg font-black uppercase tracking-widest text-cyan-700">Grand Total</span>
                            <span className="text-3xl font-black font-mono">$128.03</span>
                        </div>
                    </div>
                </div>
            </main>
        </div>
    );
};

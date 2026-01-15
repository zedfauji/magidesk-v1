
import { useNavigate } from 'react-router-dom';
import { orderService } from '../../services';

export const OrderReviewScreen = () => {
    const navigate = useNavigate();

    const handleSend = async () => {
        // Mock send
        await orderService.sendOrderToKitchen('ticket-mock', []);
        navigate(-1); // Go back to where we came from (Session or Menu)
    };

    return (
        <div className="flex h-screen bg-slate-900 text-white font-sans overflow-hidden">
            <aside className="w-64 border-r border-white/5 bg-slate-900 flex flex-col p-6">
                <h1 className="text-lg font-bold">CUE & CHILL</h1>
                <button onClick={() => navigate(-1)} className="mt-8 text-left text-white/60 hover:text-white">← Back</button>
            </aside>

            <main className="flex-1 flex flex-col">
                <header className="h-16 border-b border-white/5 flex items-center px-8 bg-slate-900/80">
                    <span className="text-cyan-400 text-sm font-bold uppercase tracking-wider">Ticket Draft</span>
                </header>

                <div className="flex-1 flex">
                    {/* Items List */}
                    <section className="flex-1 p-8 overflow-y-auto">
                        <h3 className="text-2xl font-bold tracking-tight mb-6">Review Items</h3>

                        {/* Mock Draft Items */}
                        <div className="space-y-4">
                            <div className="bg-slate-800 p-6 rounded-2xl border border-white/5 flex items-center justify-between">
                                <span className="text-2xl font-black">x2</span>
                                <div className="flex-1 ml-6">
                                    <h4 className="text-xl font-bold">Heineken Draft</h4>
                                    <p className="text-white/40 text-sm">Bar Station</p>
                                </div>
                                <span className="font-mono font-bold">$14.00</span>
                            </div>
                        </div>
                    </section>
                </div>

                <div className="p-8 border-t border-white/10 flex justify-end">
                    <button
                        onClick={handleSend}
                        className="h-20 px-12 bg-cyan-500 text-slate-900 rounded-2xl font-black uppercase tracking-widest text-2xl hover:bg-cyan-400"
                    >
                        Send to Kitchen
                    </button>
                </div>
            </main>
        </div>
    );
};

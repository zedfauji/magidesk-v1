
import type { DraftOrderLine } from '../../types';

interface OrderReviewDialogProps {
    items: DraftOrderLine[];
    subtotal: number;
    isSending: boolean;
    onClose: () => void;
    onConfirm: () => void;
}

export const OrderReviewDialog = ({ items, subtotal, isSending, onClose, onConfirm }: OrderReviewDialogProps) => {
    return (
        <div className="fixed inset-0 bg-black/90 backdrop-blur-sm flex items-center justify-center z-50 p-4">
            <div className="bg-background-dark w-full max-w-2xl flex flex-col rounded-2xl border border-white/10 shadow-2xl overflow-hidden max-h-[80vh]">
                <div className="p-6 border-b border-white/10 flex justify-between items-center bg-surface-dark/50">
                    <div>
                        <h2 className="text-2xl font-bold text-white mb-1">Review Order</h2>
                        <p className="text-[#9db2b9]">Confirm these items for the kitchen</p>
                    </div>
                    <button onClick={onClose} disabled={isSending} className="p-2 hover:bg-white/10 rounded-full transition-colors">
                        <span className="material-symbols-outlined text-white">close</span>
                    </button>
                </div>

                <div className="flex-1 overflow-y-auto p-6 space-y-4 custom-scrollbar">
                    {items.map(item => (
                        <div key={item.tempId} className="flex justify-between items-start p-4 bg-surface-dark rounded-xl border border-white/5">
                            <div className="flex gap-4">
                                <div className="bg-white/5 h-12 w-12 rounded-lg flex items-center justify-center text-white font-bold text-lg">
                                    {item.quantity}x
                                </div>
                                <div>
                                    <h3 className="text-white font-bold text-lg">{item.name}</h3>
                                    {item.modifiers.length > 0 && (
                                        <div className="mt-1 space-y-1">
                                            {item.modifiers.map((mod, idx) => (
                                                <p key={idx} className="text-[#9db2b9] text-sm flex items-center gap-2">
                                                    <span className="w-1.5 h-1.5 rounded-full bg-primary/50"></span>
                                                    {mod.name}
                                                    {mod.priceDelta > 0 && <span className="text-primary font-mono ml-auto">+${mod.priceDelta.toFixed(2)}</span>}
                                                </p>
                                            ))}
                                        </div>
                                    )}
                                    {item.instructions && (
                                        <p className="text-yellow-400/80 text-sm mt-2 italic flex items-center gap-2">
                                            <span className="material-symbols-outlined text-[16px]">sticky_note_2</span>
                                            {item.instructions}
                                        </p>
                                    )}
                                </div>
                            </div>
                            <div className="text-right">
                                <span className="text-primary font-mono font-bold text-lg">
                                    ${((item.unitPrice + item.modifiers.reduce((s, m) => s + m.priceDelta, 0)) * item.quantity).toFixed(2)}
                                </span>
                            </div>
                        </div>
                    ))}
                </div>

                <div className="p-6 border-t border-white/10 bg-surface-dark">
                    <div className="flex justify-between items-center mb-6">
                        <span className="text-[#9db2b9] font-bold uppercase tracking-wider">Total Amount</span>
                        <span className="text-3xl font-bold text-white font-mono">${subtotal.toFixed(2)}</span>
                    </div>

                    <div className="grid grid-cols-2 gap-4">
                        <button
                            onClick={onClose}
                            disabled={isSending}
                            className="py-4 rounded-xl font-bold text-white border border-white/10 hover:bg-white/5 transition-colors uppercase tracking-widest"
                        >
                            Back
                        </button>
                        <button
                            onClick={onConfirm}
                            disabled={isSending}
                            className="py-4 rounded-xl font-black text-background-dark bg-primary hover:shadow-[0_0_20px_rgba(43,189,238,0.4)] transition-all uppercase tracking-widest flex items-center justify-center gap-2"
                        >
                            {isSending ? (
                                <>
                                    <span className="material-symbols-outlined animate-spin">progress_activity</span>
                                    Sending...
                                </>
                            ) : (
                                <>
                                    <span className="material-symbols-outlined">check_circle</span>
                                    Confirm Order
                                </>
                            )}
                        </button>
                    </div>
                </div>
            </div>
        </div>
    );
};


import { useNavigate, useLocation } from 'react-router-dom';
import { useMenuBrowserViewModel } from './useMenuBrowserViewModel';
import { ModifierSelectionDialog } from './ModifierSelectionDialog';
import { OrderReviewDialog } from './OrderReviewDialog';

// Helper for Category Icons (View helper only)
const getCategoryIcon = (name: string): string => {
    const n = name.toLowerCase();
    if (n.includes('beer')) return 'sports_bar';
    if (n.includes('cocktail')) return 'local_bar';
    if (n.includes('spirit') || n.includes('liquor')) return 'liquor';
    if (n.includes('wine')) return 'wine_bar';
    if (n.includes('food') || n.includes('main')) return 'restaurant';
    if (n.includes('snack') || n.includes('appetizer')) return 'fastfood';
    if (n.includes('coffee') || n.includes('tea') || n.includes('soft')) return 'coffee';
    return 'category';
};

export const MenuBrowserScreen = () => {
    const navigate = useNavigate();
    const location = useLocation();
    const { ticketId, tableId } = location.state || {}; // Route params stay in View

    const vm = useMenuBrowserViewModel(ticketId, tableId);

    return (
        <div className="flex h-screen bg-background-light dark:bg-background-dark text-slate-900 dark:text-white font-sans overflow-hidden">

            {vm.isReviewing && (
                <OrderReviewDialog
                    items={vm.draftItems}
                    subtotal={vm.subtotal}
                    isSending={vm.isSending}
                    onClose={vm.handleReviewClose}
                    onConfirm={vm.confirmSendOrder}
                />
            )}

            {vm.selectedItemForModifiers && (
                <ModifierSelectionDialog
                    item={vm.selectedItemForModifiers}
                    onClose={vm.handleModifiersClose}
                    onConfirm={vm.handleModifierConfirm}
                />
            )}

            {/* Left Sidebar: Categories */}
            <nav className="w-24 bg-background-dark border-r border-[#283539] flex flex-col items-center py-4 gap-4 shrink-0 z-10">
                <button
                    onClick={() => navigate(-1)}
                    className="mb-2 p-3 bg-surface-dark rounded-xl text-[#9db2b9] hover:text-white hover:bg-card-dark transition-colors"
                >
                    <span className="material-symbols-outlined">arrow_back</span>
                </button>

                {vm.categories.map(cat => {
                    const isSelected = vm.selectedCat === cat.id;
                    return (
                        <div
                            key={cat.id}
                            onClick={() => vm.setSelectedCat(cat.id)}
                            className={`flex flex-col items-center gap-1 cursor-pointer w-full py-3 transition-colors ${isSelected
                                ? 'bg-primary/10 border-r-4 border-primary'
                                : 'hover:bg-white/5 border-r-4 border-transparent'
                                }`}
                        >
                            <span className={`material-symbols-outlined text-3xl ${isSelected ? 'text-primary' : 'text-[#9db2b9]'}`}>
                                {getCategoryIcon(cat.name)}
                            </span>
                            <span className={`text-[10px] font-bold uppercase tracking-tighter ${isSelected ? 'text-primary' : 'text-[#9db2b9]'}`}>
                                {cat.name}
                            </span>
                        </div>
                    );
                })}
            </nav>

            {/* Main Product Grid */}
            <main className="flex-1 flex flex-col bg-background-dark/50 relative">
                {/* Search / Top Bar */}
                <div className="px-6 py-3 bg-background-dark/80 border-b border-[#283539] flex justify-between items-center backdrop-blur-md sticky top-0 z-10">
                    <h2 className="text-xl font-bold tracking-tight text-white">Menu</h2>
                    <div className="relative w-64">
                        <span className="material-symbols-outlined absolute left-3 top-1/2 -translate-y-1/2 text-[#9db2b9]">search</span>
                        <input
                            className="w-full bg-surface-dark border-none rounded-lg py-2 pl-11 pr-4 text-white placeholder:text-[#9db2b9] focus:ring-1 focus:ring-primary h-10 text-sm"
                            placeholder="Search items..."
                            type="text"
                        />
                    </div>
                </div>

                {/* Sub-categories tabs */}
                <div className="px-6 py-2 bg-background-dark/80 border-b border-[#283539] overflow-x-auto">
                    <div className="flex gap-4 min-w-max">
                        <button className="px-4 py-1.5 rounded-full bg-primary text-background-dark font-bold text-sm shadow-lg shadow-primary/20">All Items</button>
                    </div>
                </div>

                {/* Grid */}
                <div className="flex-1 overflow-y-auto p-6 custom-scrollbar">
                    <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 xl:grid-cols-5 gap-4 pb-20">
                        {vm.items.map(item => (
                            <div
                                key={item.id}
                                onClick={() => vm.handleItemClick(item)}
                                className="aspect-square bg-card-dark rounded-xl p-4 flex flex-col justify-between border border-[#3b4d54] hover:border-primary/50 transition-all cursor-pointer active:scale-95 group relative overflow-hidden"
                            >
                                <div className="absolute top-0 left-0 w-full h-1 bg-primary/0 group-hover:bg-primary/20 transition-colors"></div>
                                <div className="flex justify-between items-start">
                                    <span className="material-symbols-outlined text-primary/40 group-hover:text-primary transition-colors">add_circle</span>
                                    <span className="text-primary font-bold font-mono">${item.price.toFixed(2)}</span>
                                </div>
                                <div>
                                    <p className="text-white text-lg font-bold leading-tight group-hover:text-primary transition-colors max-h-[3rem] overflow-hidden text-ellipsis line-clamp-2">{item.name}</p>
                                    <p className="text-[#9db2b9] text-xs font-medium uppercase mt-1">
                                        {item.stockQuantity !== undefined ? `Stock: ${item.stockQuantity}` : 'In Stock'}
                                    </p>
                                </div>
                            </div>
                        ))}
                    </div>
                </div>
            </main>

            {/* Right Sidebar: Ticket Draft */}
            <aside className="w-80 bg-background-dark border-l border-[#283539] flex flex-col shrink-0 z-10 shadow-xl">
                <div className="p-4 border-b border-[#283539] flex justify-between items-center bg-surface-dark/30">
                    <h2 className="text-white text-base font-bold uppercase tracking-widest">Current Order</h2>
                    <span className="bg-primary/20 text-primary text-[10px] font-bold px-2 py-0.5 rounded">DRAFT</span>
                </div>

                {/* Draft Items List */}
                <div className="flex-1 overflow-y-auto p-4 space-y-3 custom-scrollbar">
                    {vm.draftItems.length === 0 ? (
                        <div className="h-full flex flex-col items-center justify-center opacity-20 select-none">
                            <span className="material-symbols-outlined text-4xl mb-2">touch_app</span>
                            <p className="text-xs font-medium text-center uppercase tracking-widest leading-relaxed">Select items to<br />add to ticket</p>
                        </div>
                    ) : (
                        vm.draftItems.map(item => (
                            <div key={item.tempId} className="bg-surface-dark p-3 rounded-lg border border-white/5 relative group">
                                <button
                                    onClick={() => vm.handleRemoveItem(item.tempId)}
                                    className="absolute -top-2 -right-2 bg-red-500 text-white rounded-full p-1 opacity-0 group-hover:opacity-100 transition-opacity shadow-lg"
                                >
                                    <span className="material-symbols-outlined text-14px font-bold">close</span>
                                </button>
                                <div className="flex justify-between items-start">
                                    <div>
                                        <p className="font-bold text-white text-sm">{item.quantity}x {item.name}</p>
                                        <div className="text-xs text-[#9db2b9] pl-2 border-l-2 border-primary/20 my-1 space-y-0.5">
                                            {item.modifiers.map((mod, idx) => (
                                                <p key={idx}>{mod.name} {mod.priceDelta > 0 && `(+${mod.priceDelta.toFixed(2)})`}</p>
                                            ))}
                                        </div>
                                    </div>
                                    <p className="font-mono text-sm font-bold text-primary">
                                        ${((item.unitPrice + item.modifiers.reduce((s, m) => s + m.priceDelta, 0)) * item.quantity).toFixed(2)}
                                    </p>
                                </div>
                            </div>
                        ))
                    )}
                </div>

                {/* Footer Controls */}
                <div className="p-6 border-t border-[#283539] bg-surface-dark/50 mt-auto">
                    <div className="space-y-2 mb-6 opacity-80">
                        <div className="flex justify-between text-[#9db2b9] text-xs font-medium uppercase tracking-wider">
                            <span>Subtotal</span>
                            <span className="font-mono">${vm.subtotal.toFixed(2)}</span>
                        </div>
                        <div className="flex justify-between text-white text-xl font-bold mt-4 pt-4 border-t border-dashed border-[#3b4d54]">
                            <span>Total</span>
                            <span className="font-mono text-primary">${vm.subtotal.toFixed(2)}</span>
                        </div>
                    </div>

                    <button
                        onClick={vm.handleSendOrder}
                        disabled={vm.draftItems.length === 0 || vm.isSending}
                        className={`
                            w-full py-4 rounded-xl font-black text-sm uppercase tracking-[0.2em] flex items-center justify-center gap-2 transition-all
                            ${vm.draftItems.length === 0 || vm.isSending ? 'bg-white/10 text-white/30 cursor-not-allowed' : 'bg-primary text-background-dark shadow-[0_0_20px_rgba(43,189,238,0.3)] hover:shadow-primary/50'}
                        `}
                    >
                        {vm.isSending ? (
                            <span className="material-symbols-outlined animate-spin">progress_activity</span>
                        ) : (
                            <>
                                <span className="material-symbols-outlined font-bold">send</span>
                                Review Order
                            </>
                        )}
                    </button>
                </div>
            </aside>
        </div>
    );
};

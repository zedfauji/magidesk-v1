
import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { menuService } from '../../services';
import type { MenuCategory, MenuItem } from '../../types';

export const MenuBrowserScreen = () => {
    const navigate = useNavigate();
    const [categories, setCategories] = useState<MenuCategory[]>([]);
    const [items, setItems] = useState<MenuItem[]>([]);
    const [selectedCat, setSelectedCat] = useState<string>('');

    useEffect(() => {
        menuService.getCategories().then(cats => {
            setCategories(cats);
            if (cats.length > 0) setSelectedCat(cats[0].id);
        });
    }, []);

    useEffect(() => {
        if (selectedCat) {
            menuService.getItems(selectedCat).then(setItems);
        }
    }, [selectedCat]);

    return (
        <div className="flex h-screen bg-slate-900 text-white font-sans overflow-hidden">
            {/* Categories Sidebar */}
            <nav className="w-24 bg-slate-900 border-r border-white/5 flex flex-col items-center py-4 gap-4">
                <button onClick={() => navigate(-1)} className="mb-4 p-2 bg-white/10 rounded-full">
                    ←
                </button>
                {categories.map(cat => (
                    <div
                        key={cat.id}
                        onClick={() => setSelectedCat(cat.id)}
                        className={`flex flex-col items-center gap-1 cursor-pointer w-full py-3 transition-colors ${selectedCat === cat.id ? 'bg-cyan-500/10 border-r-4 border-cyan-500' : 'hover:bg-white/5'}`}
                    >
                        <span className="text-[10px] font-bold uppercase tracking-tighter">{cat.name}</span>
                    </div>
                ))}
            </nav>

            {/* Main Area */}
            <main className="flex-1 flex flex-col bg-slate-800/50">
                <div className="flex-1 overflow-y-auto p-6">
                    <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-4">
                        {items.map(item => (
                            <div key={item.id} className="aspect-square bg-slate-800 rounded-xl p-4 flex flex-col justify-between border border-white/5 hover:border-cyan-500/50 cursor-pointer active:scale-95 transition-all">
                                <div className="flex justify-between items-start">
                                    <span className="text-cyan-400 font-bold font-mono">${item.price.toFixed(2)}</span>
                                </div>
                                <div>
                                    <p className="text-white text-lg font-bold leading-tight">{item.name}</p>
                                    <p className="text-white/40 text-xs font-medium uppercase mt-1">Stock: {item.stockQuantity ?? '∞'}</p>
                                </div>
                            </div>
                        ))}
                    </div>
                </div>
            </main>

            {/* Right: Draft Sidebar (Static for now) */}
            <aside className="w-80 bg-slate-900 border-l border-white/5 flex flex-col">
                <div className="p-4 border-b border-white/5 bg-slate-800/30">
                    <h2 className="text-base font-bold uppercase tracking-widest">Current Order</h2>
                </div>

                <div className="flex-1 p-8 flex flex-col items-center justify-center opacity-20">
                    <p className="text-xs font-medium text-center uppercase tracking-widest">Select items to<br />add to ticket</p>
                </div>

                <div className="p-6 border-t border-white/5 bg-slate-800/50">
                    <button
                        onClick={() => navigate('/order-review')}
                        className="w-full bg-cyan-500 py-4 rounded-xl text-slate-900 font-black text-sm uppercase tracking-[0.2em] shadow-lg shadow-cyan-500/20"
                    >
                        Review Order
                    </button>
                </div>
            </aside>
        </div>
    );
};

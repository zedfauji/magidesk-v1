
import { useEffect, useState } from 'react';
import { menuService } from '../../services';
import type { MenuItem, ModifierGroup, SelectedModifier } from '../../types';

interface ModifierSelectionDialogProps {
    item: MenuItem;
    onClose: () => void;
    onConfirm: (modifiers: SelectedModifier[]) => void;
}

export const ModifierSelectionDialog = ({ item, onClose, onConfirm }: ModifierSelectionDialogProps) => {
    const [loading, setLoading] = useState(true);
    const [groups, setGroups] = useState<ModifierGroup[]>([]);
    const [selections, setSelections] = useState<Record<string, string[]>>({}); // groupId -> array of optionIds

    useEffect(() => {
        menuService.getItemModifiers(item.id)
            .then(data => {
                setGroups(data);
                // Initialize required selections?
                setLoading(false);
            })
            .catch(err => {
                console.error("Failed to load modifiers", err);
                setLoading(false);
            });
    }, [item.id]);

    const toggleSelection = (group: ModifierGroup, optionId: string) => {
        setSelections(prev => {
            const current = prev[group.id] || [];
            const isSelected = current.includes(optionId);

            // Single Select Logic
            if (group.maxSelection === 1) {
                if (isSelected && group.minSelection === 0) {
                    // Toggle off if optional
                    return { ...prev, [group.id]: [] };
                }
                // Switch to new selection
                return { ...prev, [group.id]: [optionId] };
            }

            // Multi Select Logic
            if (isSelected) {
                return { ...prev, [group.id]: current.filter(id => id !== optionId) };
            } else {
                if (current.length < group.maxSelection) {
                    return { ...prev, [group.id]: [...current, optionId] };
                }
                return prev; // Max reached
            }
        });
    };

    const handleConfirm = () => {
        // Flatten selections into SelectedModifier objects
        const result: SelectedModifier[] = [];

        for (const group of groups) {
            const groupSelections = selections[group.id] || [];
            // Validation
            if (groupSelections.length < group.minSelection) {
                alert(`Please select at least ${group.minSelection} option(s) for ${group.name}`);
                return;
            }

            for (const optId of groupSelections) {
                const option = group.options.find(o => o.id === optId);
                if (option) {
                    result.push({
                        groupId: group.id,
                        optionId: option.id,
                        name: option.name,
                        priceDelta: option.priceDelta
                    });
                }
            }
        }

        onConfirm(result);
    };

    if (loading) return <div className="fixed inset-0 bg-black/80 flex items-center justify-center z-50 text-white">Loading Modifiers...</div>;

    // If no modifiers exist, maybe auto-confirm? 
    // For now, show that there are no modifiers or handle in parent.
    // Ideally parent checks before opening, but fetching is async here.
    if (groups.length === 0 && !loading) {
        return (
            <div className="fixed inset-0 bg-black/80 flex items-center justify-center z-50">
                <div className="bg-surface-dark p-6 rounded-xl border border-white/10 text-center">
                    <p className="text-white mb-4">No modifiers available for this item.</p>
                    <button onClick={() => onConfirm([])} className="bg-primary text-background-dark font-bold px-6 py-2 rounded-lg">Add to Order</button>
                </div>
            </div>
        );
    }

    return (
        <div className="fixed inset-0 bg-black/90 backdrop-blur-sm flex items-center justify-center z-50 p-4">
            <div className="bg-background-dark w-full max-w-2xl max-h-[90vh] flex flex-col rounded-2xl border border-white/10 shadow-2xl overflow-hidden">
                {/* Header */}
                <div className="p-6 border-b border-white/10 flex justify-between items-center bg-surface-dark/50">
                    <div>
                        <h2 className="text-2xl font-bold text-white mb-1">{item.name}</h2>
                        <p className="text-primary font-mono">${item.price.toFixed(2)} Base Price</p>
                    </div>
                    <button onClick={onClose} className="p-2 hover:bg-white/10 rounded-full transition-colors">
                        <span className="material-symbols-outlined text-white">close</span>
                    </button>
                </div>

                {/* Modifiers List */}
                <div className="flex-1 overflow-y-auto p-6 space-y-8 custom-scrollbar">
                    {groups.map(group => {
                        const currentSelections = selections[group.id] || [];
                        const reachedMax = currentSelections.length >= group.maxSelection;

                        return (
                            <div key={group.id} className="space-y-4">
                                <div className="flex justify-between items-end border-b border-white/5 pb-2">
                                    <h3 className="text-lg font-bold text-white uppercase tracking-wider">{group.name}</h3>
                                    <div className="text-xs font-medium px-2 py-1 rounded bg-white/5 text-[#9db2b9]">
                                        {group.minSelection > 0 ? `Required: Select ${group.minSelection}` : 'Optional'}
                                        {group.maxSelection > 1 && ` (Max ${group.maxSelection})`}
                                    </div>
                                </div>

                                <div className="grid grid-cols-1 md:grid-cols-2 gap-3">
                                    {group.options.map(option => {
                                        const isSelected = currentSelections.includes(option.id);
                                        const isDisabled = !isSelected && reachedMax && group.maxSelection > 1; // Disable only if max reached in multi-select

                                        return (
                                            <div
                                                key={option.id}
                                                onClick={() => !isDisabled && toggleSelection(group, option.id)}
                                                className={`
                                                    p-4 rounded-xl border flex justify-between items-center cursor-pointer transition-all
                                                    ${isSelected
                                                        ? 'bg-primary/20 border-primary shadow-[0_0_15px_rgba(43,189,238,0.2)]'
                                                        : 'bg-surface-dark border-white/5 hover:border-white/20 hover:bg-white/5'}
                                                    ${isDisabled ? 'opacity-50 cursor-not-allowed' : ''}
                                                `}
                                            >
                                                <div className="flex items-center gap-3">
                                                    <div className={`
                                                        size-5 rounded-full border flex items-center justify-center
                                                        ${isSelected ? 'border-primary bg-primary' : 'border-white/30'}
                                                    `}>
                                                        {isSelected && <span className="material-symbols-outlined text-[14px] text-background-dark font-bold">check</span>}
                                                    </div>
                                                    <span className={`font-medium ${isSelected ? 'text-white' : 'text-white/70'}`}>{option.name}</span>
                                                </div>
                                                {option.priceDelta > 0 && (
                                                    <span className="text-primary text-sm font-mono">+${option.priceDelta.toFixed(2)}</span>
                                                )}
                                            </div>
                                        );
                                    })}
                                </div>
                            </div>
                        );
                    })}
                </div>

                {/* Footer */}
                <div className="p-6 border-t border-white/10 bg-surface-dark flex justify-between items-center">
                    <div className="text-right">
                        {/* Ideally calculate total with modifiers here for display */}
                    </div>
                    <div className="flex gap-4 w-full md:w-auto">
                        <button
                            onClick={onClose}
                            className="flex-1 md:flex-none px-6 py-3 rounded-xl border border-white/10 text-white font-bold hover:bg-white/5 transition-colors"
                        >
                            Cancel
                        </button>
                        <button
                            onClick={handleConfirm}
                            className="flex-1 md:flex-none px-8 py-3 rounded-xl bg-primary text-background-dark font-black uppercase tracking-widest hover:shadow-[0_0_20px_rgba(43,189,238,0.4)] transition-all"
                        >
                            Add to Order
                        </button>
                    </div>
                </div>
            </div>
        </div>
    );
};

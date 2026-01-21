
import { useEffect, useState, useCallback } from 'react';
import { useNavigate } from 'react-router-dom';
import { menuService, orderService } from '../../services';
import type { MenuCategory, MenuItem, DraftOrderLine, SelectedModifier } from '../../types';

export const useMenuBrowserViewModel = (ticketId?: string, tableId?: string) => {
    const navigate = useNavigate();

    // Data State
    const [categories, setCategories] = useState<MenuCategory[]>([]);
    const [items, setItems] = useState<MenuItem[]>([]);
    const [selectedCat, setSelectedCat] = useState<string>('');

    // Logic State
    const [draftItems, setDraftItems] = useState<DraftOrderLine[]>([]);
    const [selectedItemForModifiers, setSelectedItemForModifiers] = useState<MenuItem | null>(null);
    const [isSending, setIsSending] = useState(false);
    const [isReviewing, setIsReviewing] = useState(false); // New state for review modal

    // Initial Load
    useEffect(() => {
        menuService.getCategories().then(cats => {
            setCategories(cats);
            if (cats.length > 0) setSelectedCat(cats[0].id);
        });
    }, []);

    // Category Change
    useEffect(() => {
        if (selectedCat) {
            menuService.getItems(selectedCat).then(setItems);
        }
    }, [selectedCat]);

    const handleItemClick = useCallback((item: MenuItem) => {
        setSelectedItemForModifiers(item);
    }, []);

    const handleModifierConfirm = useCallback((modifiers: SelectedModifier[]) => {
        if (!selectedItemForModifiers) return;

        const newLine: DraftOrderLine = {
            tempId: crypto.randomUUID(),
            menuItemId: selectedItemForModifiers.id,
            name: selectedItemForModifiers.name,
            quantity: 1,
            unitPrice: selectedItemForModifiers.price,
            modifiers: modifiers
        };

        setDraftItems(prev => [...prev, newLine]);
        setSelectedItemForModifiers(null);
    }, [selectedItemForModifiers]);

    const handleModifiersClose = useCallback(() => {
        setSelectedItemForModifiers(null);
    }, []);

    const handleRemoveItem = useCallback((tempId: string) => {
        setDraftItems(prev => prev.filter(i => i.tempId !== tempId));
    }, []);

    // Trigger Review Mode instead of sending immediately
    const handleSendOrder = useCallback(() => {
        if (!ticketId) {
            alert("No active ticket found. Cannot send order.");
            return;
        }
        if (draftItems.length === 0) return;
        setIsReviewing(true);
    }, [ticketId, draftItems]);

    // Close Review Modal
    const handleReviewClose = useCallback(() => {
        setIsReviewing(false);
    }, []);

    // Actually Send Order (moved from handleSendOrder)
    const confirmSendOrder = useCallback(async () => {
        if (!ticketId) return;

        setIsSending(true);
        try {
            await orderService.sendOrderToKitchen(ticketId, draftItems);
            // Navigate back to table session
            if (tableId) {
                navigate(`/session/${tableId}`);
            } else {
                navigate(-1);
            }
        } catch (err) {
            console.error("Failed to send order", err);
            alert("Failed to send order to kitchen. Please try again.");
            setIsSending(false);
        }
    }, [ticketId, draftItems, tableId, navigate]);

    const subtotal = draftItems.reduce((sum, item) => {
        const modTotal = item.modifiers.reduce((mSum, mod) => mSum + mod.priceDelta, 0);
        return sum + (item.unitPrice + modTotal) * item.quantity;
    }, 0);

    return {
        // State
        categories,
        items,
        selectedCat,
        draftItems,
        selectedItemForModifiers,
        isSending,
        subtotal,
        isReviewing,

        // Actions
        setSelectedCat,
        handleItemClick,
        handleModifierConfirm,
        handleModifiersClose,
        handleRemoveItem,
        handleSendOrder,
        handleReviewClose,
        confirmSendOrder
    };
};

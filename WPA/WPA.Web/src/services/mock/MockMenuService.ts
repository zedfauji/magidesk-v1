
import type { MenuCategory, MenuItem, ModifierGroup } from "../../types";
import type { IMenuService } from "../interfaces";

export class MockMenuService implements IMenuService {
    async getCategories(): Promise<MenuCategory[]> {
        return [
            { id: 'cat-1', name: 'Beer' },
            { id: 'cat-2', name: 'Cocktails' },
            { id: 'cat-3', name: 'Spirits' },
            { id: 'cat-4', name: 'Snacks' },
            { id: 'cat-5', name: 'Food' },
            { id: 'cat-6', name: 'Softs' },
        ];
    }

    async getItems(_categoryId: string): Promise<MenuItem[]> {
        return [
            { id: 'item-1', name: 'Guinness Draught', price: 7.50, categoryId: 'cat-1', stockQuantity: 20 },
            { id: 'item-2', name: 'Heineken', price: 6.00, categoryId: 'cat-1', stockQuantity: 50 },
            { id: 'item-3', name: 'Lagunitas IPA', price: 8.00, categoryId: 'cat-1', stockQuantity: 15 },
            { id: 'item-4', name: 'Miller Lite', price: 5.50, categoryId: 'cat-1', stockQuantity: 100 },
            { id: 'item-5', name: 'Buffalo Wings', price: 12.00, categoryId: 'cat-4' },
        ];
    }

    async searchItems(_query: string): Promise<MenuItem[]> {
        return [];
    }

    async getItemModifiers(_menuItemId: string): Promise<ModifierGroup[]> {
        return [
            {
                id: 'mod-group-1',
                name: 'Spice Level',
                minSelection: 1,
                maxSelection: 1,
                options: [
                    { id: 'opt-1', name: 'Mild', priceDelta: 0 },
                    { id: 'opt-2', name: 'Medium', priceDelta: 0 },
                    { id: 'opt-3', name: 'Hot', priceDelta: 0 },
                    { id: 'opt-4', name: 'Nuclear', priceDelta: 0 },
                ]
            },
            {
                id: 'mod-group-2',
                name: 'Dipping Sauce',
                minSelection: 0,
                maxSelection: 1,
                options: [
                    { id: 'opt-5', name: 'Ranch', priceDelta: 0.50 },
                    { id: 'opt-6', name: 'Blue Cheese', priceDelta: 0.50 },
                ]
            }
        ];
    }
}

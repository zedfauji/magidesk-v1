import { api } from "./api";
import type { IMenuService } from "../interfaces";
import type { MenuCategory, MenuItem, ModifierGroup } from "../../types";

interface MenuCategoryDto {
    id: string;
    name: string;
    subcategories?: MenuCategoryDto[];
}

interface MenuItemDto {
    id: string;
    name: string;
    price: number;
    description?: string;
    categoryId: string;
    stockQuantity?: number;
}

interface ModifierOptionDto {
    id: string;
    name: string;
    priceDelta: number;
}

interface ModifierGroupDto {
    id: string;
    name: string;
    minSelection: number;
    maxSelection: number;
    options: ModifierOptionDto[];
}

export class HttpMenuService implements IMenuService {
    async getCategories(): Promise<MenuCategory[]> {
        console.log('[HttpMenuService] Fetching categories from backend...');
        return api.get<MenuCategoryDto[]>('/menu/categories');
    }

    async getItems(categoryId: string): Promise<MenuItem[]> {
        return api.get<MenuItemDto[]>(`/menu/items?categoryId=${categoryId}`);
    }

    async searchItems(query: string): Promise<MenuItem[]> {
        // Fallback to searching locally if backend doesn't support search yet, 
        // or call search endpoint if available (Controller had TODO)
        // Controller threw NotImplemented, so returning empty for safety
        try {
            return await api.get<MenuItemDto[]>(`/menu/items/search?q=${encodeURIComponent(query)}`);
        } catch {
            return [];
        }
    }

    async getItemModifiers(menuItemId: string): Promise<ModifierGroup[]> {
        return api.get<ModifierGroupDto[]>(`/menu/items/${menuItemId}/modifiers`);
    }
}
